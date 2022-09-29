using Models.DB;
using Models.View.Business;
using Models.View.His;
using Newtonsoft.Json;
using Service.Extensions;
using Service.Repository.Interfaces.His;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;
using Tools.Filter;
using Tools.WebSocket;
using static Tools.Utils;

namespace Service.Repository.Implements.His
{
    /// <summary>
    /// 就诊
    /// </summary>
    public class ClinicRecordService : DbContext, IClinicRecordService
    {
        /// <summary>
        /// 根据医生获取待接诊人
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="state">状态（0=待接诊，1=已接诊，2=已完成）</param>
        /// <returns></returns>
        public async Task<dynamic> GetByDoctorAsync(int store_id, short state)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            //获取排班时间段            
            var now = DateTime.Now.ToString("yyyy-MM-dd");
            var scheduletimes = Db.Queryable<his_scheduletimes>();
            var list = await Db.Queryable<his_register, his_outpatient_no, c_archives, c_archives_level, c_account>((r, hon, a, al, account) => new object[] { JoinType.Left, r.regid == hon.regid, JoinType.Left, r.centerid == a.id, JoinType.Left, a.grade_id == al.id, JoinType.Left, account.archives_id == a.id })
                .Where((r, hon, a, al, account) => r.orgid == userInfo.org_id && r.store_id == store_id && r.doctorid == userInfo.id && r.stateid == 11 && r.clinicstateid != 15)
                .Where("to_char(r.regdate,'yyyy-MM-dd')=@now", new { now })
                .Select((r, hon, a, al, account) => new { a.image_url, a.name, hon.order_no, r.regid, hon.scheduleid, hon.no, hon.days, a.sex, a.phone, r.clinicstateid, r.centerid, a.age, al.discount_rate, r.clinicid, account.balance, account.coupon })
                .WithCache()
                .ToListAsync();
            var wait_num = list.Where(w => w.clinicstateid == 14).Count();
            var verify_num = list.Where(w => w.clinicstateid == 13).Count();
            var finish_num = list.Where(w => w.clinicstateid == 15).Count();
            var clinicstateid = 14;
            if (state == 1)
            {
                clinicstateid = 13;
            }
            else if (state == 2)
            {
                clinicstateid = 15;
            }
            var clinic_list = list.Where(w => w.clinicstateid == clinicstateid).Select(s => new ClinicInfo { coupon = s.coupon, balance = s.balance, image_url = s.image_url, archives_id = s.centerid, name = s.name, sex = s.sex, age = s.age, phone = s.phone, order_no = s.order_no, regid = s.regid, discount_rate = s.discount_rate, clinicid = s.clinicid, times = GetTimes(s.scheduleid, s.no, s.days, scheduletimes) }).ToList();
            return new { wait_num, verify_num, finish_num, clinic_list };
        }

        /// <summary>
        /// 获取时间段
        /// </summary>
        /// <returns></returns>
        private static string GetTimes(int scheduleid, short no, short days, ISugarQueryable<his_scheduletimes> scheduletimes)
        {
            //获取排班时间段
            var item = scheduletimes.Clone().Where(w => w.scheduleid == scheduleid && w.no == no && w.days == days).WithCache().First();
            var begintime = item.begintime;
            var endtime = item.endtime;
            return $"{begintime} - {endtime}";
        }

        /// <summary>
        /// 获得分页列表（签到）
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="state_id">状态（1=已签到，0=未签到，7=已取消）</param>
        /// <param name="type_id">分类（1=门诊，2=康复，-1=所有）</param>
        /// <param name="name">客户名称/手机号</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <param name="is_me">是否自己客户</param>
        /// <returns></returns>
        public async Task<Page<SignRecord>> GetPagesAsync(int store_id, short state_id, short type_id, string name, string order, int orderType, int limit, int page, bool is_me = false)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            var list = new List<SignRecord>();

            var now = DateTime.Now.ToString("yyyy-MM-dd");

            //门诊
            if (type_id == 1 || type_id == -1)
            {
                //挂号记录
                var registerList = await Db.Queryable<his_register, c_archives, his_outpatient_no, p_employee>((r, a, hon, e) => new object[] { JoinType.Left, r.centerid == a.id, JoinType.Left, r.regid == hon.regid, JoinType.Left, a.to_director_id == e.id })
                    .Where((r, a, hon, e) => r.stateid == 11 && r.store_id == store_id && r.canceled == 3)
                    .Where("to_char(r.regdate,'yyyy-MM-dd')=@now", new { now })
                    .WhereIF(is_me, (r, a, hon, e) => a.to_director_id == userInfo.id)
                    .WhereIF(state_id == 1, (r, a, hon, e) => r.clinicstateid == 14 || r.clinicstateid == 15 || r.clinicstateid == 13)
                    .WhereIF(state_id == 0, (r, a, hon, e) => r.clinicstateid == 5)
                    .WhereIF(!string.IsNullOrEmpty(name), (r, a, hon, e) => a.name.Contains(name) || a.phone.Contains(name))
                    .Select((r, a, hon, e) => new SignRecord { content = SqlFunc.MergeString(r.billtype, "就诊"), id = r.regid, name = a.name, phone = a.phone, type_id = 1, type_name = "门诊", archives_id = a.id, doctorname = r.doctorname, to_director = a.to_director, sign_time = hon.order_time, to_director_phone = e.phone_no })
                    .WithCache()
                    .ToListAsync();

                //合并处理
                registerList.ForEach(c =>
                {
                    list.Add(c);
                });
            }

            //康复
            if (type_id == 2 || type_id == -1)
            {
                //康复预约
                var recoverList = await Db.Queryable<his_recover, c_archives, h_item, p_employee>((r, a, i, e) => new object[] { JoinType.Left, r.centerid == a.id, JoinType.Left, r.item_id == i.item_id, JoinType.Left, a.to_director_id == e.id })
                    .Where((r, a, i, e) => r.orgid == userInfo.org_id && r.store_id == store_id && r.canceled == 3)
                    .Where("to_char(r.regdate,'yyyy-MM-dd')=@now", new { now })
                    .WhereIF(is_me, (r, a, i, e) => a.to_director_id == userInfo.id || r.operatorid == userInfo.id || a.to_director_id == userInfo.id)
                    .WhereIF(state_id == 1, (r, a, i, e) => r.stateid != 5 && r.stateid != 7)
                    .WhereIF(state_id == 0, (r, a, i, e) => r.stateid == 5)
                    .WhereIF(state_id == 7, (r, a, i, e) => r.stateid == 7)
                    .WhereIF(!string.IsNullOrEmpty(name), (r, a, i, e) => a.name.Contains(name) || a.phone.Contains(name))
                    .Select((r, a, i, e) => new SignRecord { content = SqlFunc.MergeString(r.specname, "（", r.item_name, "）"), id = r.recoverid, name = a.name, phone = a.phone, type_id = 2, type_name = "康复", archives_id = a.id, doctorname = r.doctorname, to_director = a.to_director, sign_time = r.sign_time, item_id = r.item_id, spec_id = r.specid, equipment = i.equipment, to_director_phone = e.phone_no })
                    .WithCache()
                    .ToListAsync();

                var item_ids = recoverList.Select(s => s.item_id).ToList();

                //设备安排
                var equipment = await Db.Queryable<his_equipment_scheduling, p_equipment_detials, p_equipment>((es, ed, e) => new object[] { JoinType.Left, es.equipment_id == ed.id && es.no == ed.no, JoinType.Left, ed.id == e.id }).Where((es, ed, e) => item_ids.Contains(es.itemid)).Select((es, ed, e) => new equipment { recoverid = es.recoverid, no = ed.no, name = e.name, model = e.model, equipment_address = es.equipment_address, room_name = es.room_name }).WithCache().ToListAsync();

                //医疗室安排
                var room = await Db.Queryable<his_room_scheduling, p_room>((rs, r) => new object[] { JoinType.Left, rs.room_id == r.id }).Where((rs, r) => item_ids.Contains(rs.itemid)).Select((rs, r) => new room { position = r.position, name = r.name, recoverid = rs.recoverid }).WithCache().ToListAsync();

                recoverList = recoverList.Select(s => new SignRecord { archives_id = s.archives_id, content = s.content, doctorname = s.doctorname, equipment = s.equipment, id = s.id, item_id = s.item_id, name = s.name, phone = s.phone, sign_time = s.sign_time, spec_id = s.spec_id, to_director = s.to_director, type_id = s.type_id, type_name = s.type_name, equipment_info = GetEquipmentInfo(s, equipment), room_info = GetRoomInfo(s, room, equipment), to_director_phone = s.to_director_phone }).ToList();

                //合并处理
                recoverList.ForEach(c =>
                {
                    list.Add(c);
                });
            }

            return list.ToPage(page, limit);
        }

        /// <summary>
        /// 设备
        /// </summary>
        private class equipment
        {
            public int recoverid { get; set; }
            public string name { get; set; }
            public string no { get; set; }
            public string model { get; set; }

            /// <summary>
            /// 存放位置
            /// </summary>
            public string equipment_address { get; set; }

            public string room_name { get; set; }
        };

        /// <summary>
        /// 获取设备信息
        /// </summary>
        /// <param name="s"></param>
        /// <param name="equipment"></param>
        /// <returns></returns>
        private static string GetEquipmentInfo(SignRecord s, List<equipment> equipment)
        {
            var equipment_info = "";
            if (s.equipment == 2)
            {
                var thisEquipment = equipment.Where(w => w.recoverid == s.id).FirstOrDefault();
                if (thisEquipment != null)
                {
                    equipment_info = $"{thisEquipment.name}-{thisEquipment.model}（{thisEquipment.no}）";
                }
            }
            return equipment_info;
        }

        /// <summary>
        /// 医疗室
        /// </summary>
        private class room
        {
            public int recoverid { get; set; }
            public string name { get; set; }
            public string position { get; set; }
        };

        /// <summary>
        /// 获取医疗室信息
        /// </summary>
        /// <param name="s"></param>
        /// <param name="room"></param>
        /// <param name="equipment"></param>
        /// <returns></returns>
        private static string GetRoomInfo(SignRecord s, List<room> room, List<equipment> equipment)
        {
            var room_info = "";

            if (s.equipment == 2)
            {
                var thisEquipment = equipment.Where(w => w.recoverid == s.id).FirstOrDefault();
                if (thisEquipment != null)
                {
                    room_info = $"{thisEquipment.room_name}（{thisEquipment.equipment_address}）";
                }
            }
            else
            {
                var thisRoom = room.Where(w => w.recoverid == s.id).FirstOrDefault();
                if (thisRoom != null)
                {
                    room_info = $"{thisRoom.name}（{thisRoom.position}）";
                }
            }
            return room_info;
        }

        /// <summary>
        /// 签到
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SignAsync(Sign entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var register = new his_register();
            var archives = new c_archives();
            var employee = new p_employee();
            var content = "";
            //通知
            var notice = new Business.NoticeService();
            var noticeList = new List<AddNoticeModel>();
            var notice_content = "";
            var employeeSocket = new List<WebSocketModel>();

            var result = Db.Ado.UseTran(() =>
            {
                //门诊
                if (entity.type_id == 1)
                {
                    var register_archives = Db.Queryable<his_register, c_archives, p_employee>((r, a, e) => new object[] { JoinType.Left, r.centerid == a.id, JoinType.Left, a.to_director_id == e.id })
                    .Where((r, a, e) => r.regid == entity.sign_id)
                    .Select((r, a, e) => new { register = r, archives = a, employee = e })
                    .WithCache().ToList();

                    if (register_archives.Count == 0)
                    {
                        throw new MessageException("门诊签到失败，请稍后再试");
                    }

                    register = register_archives[0].register;
                    archives = register_archives[0].archives;
                    employee = register_archives[0].employee;

                    if (register.stateid != 11)
                    {
                        throw new MessageException("您未缴纳挂号费用，请先缴费");
                    }
                    //修改挂号记录
                    register.clinicstateid = 14;
                    register.orderflag = 3;
                    Db.Updateable(register).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                    //门诊号
                    var outpatient_no = Db.Queryable<his_outpatient_no>().Where(w => w.regid == entity.sign_id).WithCache().First();
                    var max_no = Db.Queryable<his_outpatient_no>().Where(w => w.days == outpatient_no.days && w.no == outpatient_no.no && w.register_date == outpatient_no.register_date).WithCache().Max(m => m.order_no);
                    max_no += 1;
                    Db.Updateable<his_outpatient_no>().SetColumns(s => new his_outpatient_no { order_time = DateTime.Now, order_no = max_no }).Where(w => w.regid == entity.sign_id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                    if (archives.to_director_id>0)
                    {
                        var con = $"{{\"name\":\"{archives.name}\",\"msg\":\"已经到达前台，快去接待吧！\",\"img_url\":\"{archives.image_url}\"}}";
                        employeeSocket.Add(new WebSocketModel { userId = archives.to_director_id.Value, content = con });

                        //到达通知
                        notice_content = $"{{\"name\":\"{archives.name}\",\"msg\":\" 已经到达前台，快去接待吧！\"}}";
                        var employeenotice = new List<employeeMes>();
                        employeenotice.Add(new employeeMes { employee_id = archives.to_director_id.Value, employee = archives.to_director });

                        notice.NewMethod(register.regid.ToString(),archives, register.store_id, notice, noticeList, 4, 1, notice_content, archives.name, employeenotice);

                    }

                    var con1 = $"{{\"name\":\"{archives.name}\",\"msg\":\"已签到，请准备接诊！\",\"img_url\":\"{archives.image_url}\",\"type\":\"doctor\"}}";
                    employeeSocket.Add(new WebSocketModel { userId = register.doctorid, content = con1 });
                                       
                    content = $"{archives.name}已经到达前台，快去接待吧！";

                    //签到通知
                    notice_content = $"{{\"name\":\"{archives.name}\",\"msg\":\" 已签到，请准备接诊！\"}}";
                    var employeenotice2 = new List<employeeMes>();
                    employeenotice2.Add(new employeeMes { employee_id = register.doctorid, employee = register.doctorname });
                    notice.NewMethod(register.regid.ToString(), archives, register.store_id, notice, noticeList, 4, 1, notice_content, archives.name, employeenotice2);
                }
                //康复
                else
                {
                    //修改康复预约信息
                    Db.Updateable<his_recover>().SetColumns(s => new his_recover { sign_time = DateTime.Now, stateid = 15 }).Where(w => w.recoverid == entity.sign_id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                    //查询康复项目是否使用设备
                    var useEquipment = Db.Queryable<his_recover, h_item>((r, i) => new object[] { JoinType.Left, r.item_id == i.item_id }).Where((r, i) => r.recoverid == entity.sign_id).Select((r, i) => i.equipment).WithCache().First();

                    //查询客户预约信息
                    var archives_recover = Db.Queryable<his_recover, c_archives>((r, a) => new object[] { JoinType.Left, r.centerid == a.id }).Where((r, a) => r.recoverid == entity.sign_id).Select((r, a) => new { a, r }).WithCache().First();
                    archives = archives_recover.a;
                    var recover = archives_recover.r;

                    if (useEquipment == 2)
                    {
                        //查询设备详情列表
                        var equipment_detials_list = Db.Queryable<p_equipment_detials>().WithCache().ToList();

                        Db.Insertable(new his_equipment_scheduling { address = archives.address, age = archives.age.ToString(), archives = archives.name, archives_id = archives.id, clinicid = recover.clinicid, contactno = archives.phone, contactsno = archives.contacts_phone, contancts = archives.contacts, creater = userInfo.name, creater_id = userInfo.id, create_date = DateTime.Now, emergencyno = archives.emergencyno, equipment_address = entity.room_address, room_name = entity.room_name, equipment_id = entity.equipment_id, id = DateTime.Now.ToString("yyyyMMddHHmmssffffff"), itemid = recover.item_id, no = entity.equipment_no, order_num = GetOrderNum(DateTime.Now, entity.equipment_id, entity.equipment_no), org_id = userInfo.org_id, relation = archives.relation, relationid = archives.relationid, room_id = entity.room_id, sign_time = DateTime.Now, specid = recover.specid, specname = recover.specname, stateid = 16, store_id = recover.store_id, summary = "", tradename = recover.item_name, work_date = DateTime.Now.Date, work_times = GetWorkTimes(entity.equipment_id, recover.item_id, recover.specid, entity.equipment_no, DateTime.Now.Date), wait_times = equipment_detials_list.Where(w => w.id == entity.equipment_id && w.no == entity.equipment_no).First().wait_times, recoverid = entity.sign_id }).ExecuteCommand();
                        redisCache.RemoveAll<his_equipment_scheduling>();
                    }
                    else
                    {
                        //查询医疗室列表
                        var rooms = Db.Queryable<p_room>().WithCache().ToList();

                        Db.Insertable(new his_room_scheduling { address = archives.address, age = archives.age.ToString(), archives = archives.name, archives_id = archives.id, clinicid = recover.clinicid, contactno = archives.phone, contactsno = archives.contacts_phone, contancts = archives.contacts, creater = userInfo.name, creater_id = userInfo.id, create_date = DateTime.Now, emergencyno = archives.emergencyno, id = DateTime.Now.ToString("yyyyMMddHHmmssffffff"), itemid = recover.item_id, order_num = GetOrderNumByRoom(DateTime.Now, entity.room_id), org_id = userInfo.org_id, relation = archives.relation, relationid = archives.relationid, room_id = entity.room_id, sign_time = DateTime.Now, specid = recover.specid, specname = recover.specname, stateid = 16, store_id = recover.store_id, summary = "", tradename = recover.item_name, work_date = DateTime.Now.Date, work_times = GetWorkTimesByRoom(entity.room_id, recover.item_id, recover.specid, DateTime.Now.Date), wait_times = rooms.Where(w => w.id == entity.room_id).First().wait_times, recoverid = entity.sign_id }).ExecuteCommand();
                        redisCache.RemoveAll<his_room_scheduling>();
                    }

                    if (archives.to_director_id>0)
                    {
                        var con1 = $"{{\"name\":\"{archives.name}\",\"msg\":\"即将进行康复治疗（{recover.specname}-{recover.item_name}），请做好准备！（{entity.room_name}-{entity.room_address}）\",\"img_url\":\"{archives.image_url}\"}}";
                        employeeSocket.Add(new WebSocketModel { userId = archives.to_director_id.Value, content = con1 });
                                              
                        //即将进行康复治疗通知
                        notice_content = $"{{\"name\":\"{archives.name}\",\"item\":\"（{recover.specname}-{recover.item_name}）\",\"address\":\"（{entity.room_name}-{entity.room_address}）\",\"msg\":\" 即将康复治疗\"}}";
                        var employeenotice3 = new List<employeeMes>();
                        employeenotice3.Add(new employeeMes { employee_id = archives.to_director_id.Value, employee = archives.to_director });

                        notice.NewMethod(archives.id.ToString(),archives, recover.store_id, notice, noticeList, 4, 2, notice_content, archives.name, employeenotice3);

                    }

                }

                //新增
                notice.AddNotice(noticeList);
                //消息提醒
                ChatWebSocketMiddleware.SendListAsync(employeeSocket);

            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }

            //发送短信
            Dictionary<string, string> values = new Dictionary<string, string>();
            values.Add("archives", archives.name);
            var toValues = JsonConvert.SerializeObject(values);
            var sendSms = new Public.SendSMSService();
            //await sendSms.SendSmsAsync(employee.phone_no, 6, toValues, content, 1, archives.org_id);

            return result.IsSuccess;
        }



        /// <summary>
        /// 接诊
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> ClinicAsync(Clinic entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var clinic_id = 0;
            var result = Db.Ado.UseTran(() =>
            {
                //获取挂号及档案信息
                var register_archives = Db.Queryable<his_register, c_archives>((r, a) => new object[] { JoinType.Left, r.centerid == a.id }).Where((r, a) => r.regid == entity.regid).Select((r, a) => new { register = r, archives = a }).WithCache().First();
                //挂号信息
                var register = register_archives.register;
                //档案信息
                var archives = register_archives.archives;
                //计算档案
                var archives_info = new IDCardInfo();
                if (!string.IsNullOrEmpty(archives.id_card))
                {
                    archives_info = GetInfoFromIDCard(archives.id_card);
                    if (!archives_info.IsSuccess)
                    {
                        throw new MessageException("客户身份证有误！");
                    }
                }
                //就诊记录
                var clinicrecord = new his_clinicrecord { address = archives.address, age = archives_info.age.ToString(), amount = register.actualamount, begindate = DateTime.Now, centerid = archives.id, contactno = archives.phone, contactsno = archives.contacts_phone, contancts = archives.contacts, deptid = register.deptid, deptname = register.deptname, doctorid = userInfo.id, doctorname = register.doctorname, education = archives.education, educationid = archives.educationid, emergencyno = archives.emergencyno, marriage = archives.marital_status, marriageid = archives.marital_status_code, orgid = userInfo.org_id, outid = register.regid, patienttype = archives.type, regdate = DateTime.Now, relation = archives.relation, relationid = archives.relationid, stateid = 13, vocation = archives.occupation, vocationid = archives.occupation_code };
                //添加就诊记录返回ID
                clinic_id = Db.Insertable(clinicrecord).ExecuteReturnIdentity();
                redisCache.RemoveAll<his_clinicrecord>();
                //修改挂号就诊
                Db.Updateable<his_register>().SetColumns(s => new his_register { clinicstateid = 13, clinicid = clinic_id }).Where(w => w.regid == entity.regid).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                //修改档案信息
                Db.Updateable<c_archives>().SetColumns(s => new c_archives { age = archives_info.age, day = archives_info.day, month = archives_info.month }).Where(w => w.id == archives.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                //添加门诊病历
                var clinic_detial = Db.Queryable<his_clinic_mr>().Where(w => w.archives_id == archives.id)
                                      .OrderBy(w => w.clinic_time, OrderByType.Desc)
                                      .WithCache().First();

                var bmi = clinic_detial?.bmi==null?0:clinic_detial.bmi;
                var bp1 =Convert.ToInt16( clinic_detial?.bp1 == null ? 0 : clinic_detial.bp1);
                var bp2 = Convert.ToInt16(clinic_detial?.bp2 == null ? 0 : clinic_detial.bp2);
                var height = clinic_detial?.height == null ? 0 : clinic_detial.height;
                var p = Convert.ToInt16(clinic_detial?.p == null ? 0 : clinic_detial.p);
                var rt = Convert.ToInt16(clinic_detial?.r == null ? 0 : clinic_detial.r);
                var t = clinic_detial?.t == null ? 0 : clinic_detial.t;
                var weight = clinic_detial?.weight == null ? 0 : clinic_detial.weight;

                Db.Insertable(new his_clinic_mr { org_id = userInfo.org_id, bmi = bmi, bp1 = bp1, bp2 = bp2, clinicid = clinic_id, clinic_time = DateTime.Now, complaint =clinic_detial?.complaint==null?" ":clinic_detial.complaint, diagnosis = clinic_detial?.diagnosis == null ? " " : clinic_detial.diagnosis, height = height, p = p, past_history = clinic_detial?.past_history == null ? " " : clinic_detial.past_history, phisical_exam = clinic_detial?.phisical_exam == null ? " " : clinic_detial.phisical_exam, present_history = clinic_detial?.present_history == null ? " " : clinic_detial.present_history, r = rt, store_id = register.store_id, t = t, treatment = clinic_detial?.treatment == null ? " " : clinic_detial.treatment, weight = weight, archives_id = archives.id }).ExecuteCommand();
                redisCache.RemoveAll<his_clinic_mr>();
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return clinic_id;
        }

        /// <summary>
        /// 获取门诊病历
        /// </summary>
        /// <param name="clinic_id">就诊ID</param>
        /// <returns></returns>
        public async Task<his_clinic_mr> GetMedicalRecordAsync(int clinic_id)
        {
            return await Db.Queryable<his_clinic_mr>().Where(w => w.clinicid == clinic_id).WithCache().FirstAsync();
        }

        /// <summary>
        /// 获取用户最后一条就诊记录
        /// </summary>
        /// <param name="user_id"></param>
        /// <returns></returns>
        public async Task<his_clinic_mr> GetMedicalUserRecordAsync(int user_id)
        {
            return await Db.Queryable<his_clinic_mr>().Where(w => w.archives_id == user_id).OrderBy(w=>w.clinic_time,OrderByType.Desc).WithCache().FirstAsync();
        }

        /// <summary>
        /// 门诊病历
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> MedicalRecordAsync(his_clinic_mr entity)
        {
            return await Db.Updateable(entity).IgnoreColumns(i => new { i.org_id, i.clinicid, i.clinic_time, i.store_id }).Where(w => w.id == entity.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
        }

        /// <summary>
        /// 开处方
        /// </summary>
        /// <returns></returns>
        public async Task<int> PrescriptionAsync(Prescription entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            //查询档案账户
            //var archives = await Db.Queryable<c_archives, c_account, c_archives_level>((a, ac, al) => new object[] { JoinType.Left, a.id == ac.archives_id, a.grade_id == al.id }).Where((a, ac, al) => a.id == entity.archives_id).Select((a, ac, al) => new { a, ac, al }).WithCache().FirstAsync();
            //if (archives.ac.balance < entity.shouldamount)
            //{
            //    throw new MessageException("账户余额不足，请先充值");
            //}
            var applybill_id = 0;

            var archives = new c_archives();
            var employee = new p_employee();
            var content = "";

            //通知
            var notice = new Business.NoticeService();
            var noticeList = new List<AddNoticeModel>();
            var notice_content = "";
            var employeeSocket = new List<WebSocketModel>();

            var result = Db.Ado.UseTran(() =>
            {
                //获取档案当前就诊信息
                var clinicrecord = Db.Queryable<his_clinicrecord>().Where(w => w.clinicid == entity.clinicid).WithCache().First();
                //获取挂号记录
                var register = Db.Queryable<his_register>().Where(w => w.clinicid == entity.clinicid).WithCache().First();

                //获取商品规格对应项目及票据类型
                var specids = entity.prescriptionItems.Select(s => s.specid).ToList();
                var itemlist = Db.Queryable<h_itemspec, h_item, b_basecode>((isp, i, b) => new object[] { JoinType.Left, isp.itemid == i.item_id, JoinType.Left, isp.incomeid == b.valueid && b.baseid == 66 }).Where((isp, i, b) => specids.Contains(isp.specid)).Select((isp, i, b) => new { itemspec = isp, item = i, b.value }).WithCache().ToList();
                var items = itemlist.Select(s => new { s.itemspec, s.item, s.value, pre_entity = entity.prescriptionItems.Where(w => w.specid == s.itemspec.specid).FirstOrDefault() }).ToList();

                //判断传入金额是否正确
                var money = items.Sum(s => s.itemspec.sale_price * s.pre_entity.quantity);
                if (money != entity.shouldamount)
                {
                    throw new MessageException("非法操作，应收金额不正确");
                }
                //如果是诊前检查
                if (entity.typeid == 6)
                {
                    //查询档案账户
                    var account_al = Db.Queryable<c_account, c_archives, c_archives_level, p_employee>((a, ac, al, e) => new object[] { JoinType.Left, a.archives_id == ac.id, JoinType.Left, ac.grade_id == al.id, JoinType.Left, ac.to_director_id == e.id }).Where((a, ac, al, e) => a.archives_id == entity.archives_id && al.org_id == userInfo.org_id).Select((a, ac, al, e) => new { account = a, archives = ac, level = al, employee = e }).WithCache().First();

                    if (account_al == null)
                    {
                        throw new MessageException("未获取到此用户档案！");
                    }
                    //等级
                    var level = account_al.level;
                    //账户
                    var account = account_al.account;
                    //基本信息
                    archives = account_al.archives;
                    employee = account_al.employee;
                    //优惠券支付
                    decimal coupon_money = 0;
                    //账户支付
                    decimal accountpay = 0;

                    //折扣率
                    decimal discountRate = 100;
                    //实收金额
                    decimal actual_amount = 0;

                    if ((account.balance + account.coupon) < entity.shouldamount)
                    {
                        throw new MessageException("账户余额不足，请先充值");
                    }
                    if (account.balance >= entity.shouldamount)
                    {

                        discountRate = level.discount_rate.Value;
                        actual_amount = entity.shouldamount * discountRate / 100;
                        account.balance -= entity.shouldamount * discountRate / 100;
                        accountpay = entity.shouldamount * discountRate / 100;
                    }
                    else if (account.coupon >= entity.shouldamount)
                    {
                        actual_amount = entity.shouldamount;
                        account.coupon -= entity.shouldamount;
                        coupon_money = entity.shouldamount;
                    }
                    else
                    {
                        coupon_money = account.coupon;
                        actual_amount = entity.shouldamount;
                        account.balance -= entity.shouldamount - account.coupon;
                        accountpay = entity.shouldamount - account.coupon;
                        account.coupon = 0;
                    }

                    //扣除档案账户
                    var Encrypt = account.amount + account.archives_id + account.balance + account.consume + account.coupon + account.integral + account.noneamount + account.password + account.rate + account.recharge + account.salseamount + account.settleamount + account.total_coupon + account.total_integral;
                    var code = MetarnetRegex.Encrypt(MetarnetRegex.SHA512Encrypt(Encrypt.ToString()));
                    account.code = code;
                    Db.Updateable(account).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                    //门诊申请单
                    var before_apply = new his_applybill { orgid = userInfo.org_id, clinicid = clinicrecord.clinicid, centerid = entity.archives_id, typeid = entity.typeid, typename = entity.typename, regid = register.regid, recorddate = DateTime.Now, doctorid = clinicrecord.doctorid, doctorname = clinicrecord.doctorname, deptid = clinicrecord.deptid, deptname = clinicrecord.deptname, stateid = 15, statename = "已完成", issettle = 2, shouldamount = entity.shouldamount, actualamount = actual_amount, days = 1, diagnosiscode = entity.diagnosiscode, diagnosisname = entity.diagnosisname, double_num = entity.double_num, summay = entity.summay, store_id = entity.store_id };
                    applybill_id = Db.Insertable(before_apply).ExecuteReturnIdentity();
                    redisCache.RemoveAll<his_applybill>();

                    //门诊申请单明细
                    var before_applybilldetails = items.Select((s, index) => new his_applycontent { applyid = applybill_id, orderid = short.Parse(index.ToString()), specid = s.itemspec.specid, specname = s.itemspec.specname, packid = "0", content = s.pre_entity.content, quantity = s.pre_entity.quantity, price = s.itemspec.sale_price, cost = s.itemspec.buy_price, shouldamount = s.itemspec.sale_price * s.pre_entity.quantity, actualamount = GetActualAmount(s.itemspec, s.pre_entity, discountRate, account, entity.shouldamount), usageid = s.pre_entity.usageid, usagename = s.pre_entity.usagename, frequecyname = s.pre_entity.frequecyname, frequecyid = s.pre_entity.frequecyid, sigle = 1, dosageunit = s.itemspec.dosageunit, execdeptid = clinicrecord.deptid, execdept = clinicrecord.deptname, execstateid = 17, execstate = "已执行", unitname = s.itemspec.salseunit, modulus = s.itemspec.salsemodulus, groupid = s.pre_entity.groupid, item_id = s.item.item_id, item_name = s.item.trade_name, org_id = userInfo.org_id, store_id = entity.store_id }).ToList();
                    Db.Insertable(before_applybilldetails).ExecuteCommand();
                    redisCache.RemoveAll<his_applycontent>();

                    //查询使用设备的项目
                    var use_equipment_item = Db.Queryable<h_item>().Where(w => w.equipment == 2).Select(s => s.item_id).WithCache().ToList();

                    //查询设备详情列表
                    var equipment_detials_list = Db.Queryable<p_equipment_detials>().WithCache().ToList();

                    if (equipment_detials_list.Count==0)
                    {
                        throw new MessageException("请设置器械的基础数据！");
                    }

                    //添加器械安排
                    var equipment_list = items.Where(w => use_equipment_item.Contains(w.item.item_id)).Select((s, index) => new his_equipment_scheduling { address = archives.address, age = archives.age.ToString(), archives = archives.name, archives_id = archives.id, clinicid = entity.clinicid, contactno = archives.phone, contactsno = archives.contacts_phone, contancts = archives.contacts, creater = userInfo.name, creater_id = userInfo.id, create_date = DateTime.Now, emergencyno = archives.emergencyno, equipment_address = s.pre_entity.room_address, equipment_id = s.pre_entity.equipment_id, id = DateTime.Now.ToString("yyyyMMddHHmmssffff") + index.ToString(), itemid = s.item.item_id, no = s.pre_entity.equipment_no, order_num = GetOrderNum(DateTime.Now, s.pre_entity.equipment_id, s.pre_entity.equipment_no), org_id = userInfo.org_id, relation = archives.relation, relationid = archives.relationid, room_id = s.pre_entity.room_id, sign_time = DateTime.Now, specid = s.itemspec.specid, specname = s.itemspec.specname, stateid = 16, store_id = entity.store_id, summary = entity.summay, tradename = s.item.trade_name, work_date = DateTime.Now.Date, work_times = GetWorkTimes(s.pre_entity.equipment_id, s.item.item_id, s.itemspec.specid, s.pre_entity.equipment_no, DateTime.Now.Date), wait_times = equipment_detials_list.Where(w => w.id == s.pre_entity.equipment_id && w.no == s.pre_entity.equipment_no).First().wait_times }).ToList();
                    Db.Insertable(equipment_list).ExecuteCommand();
                    redisCache.RemoveAll<his_equipment_scheduling>();

                    //查询医疗室列表
                    var rooms = Db.Queryable<p_room>().WithCache().ToList();

                    if (rooms.Count == 0)
                    {
                        throw new MessageException("请设置器械房间的基础数据！");
                    }

                    //添加医疗室安排
                    var room_list = items.Where(w => !use_equipment_item.Contains(w.item.item_id)).Select((s, index) => new his_room_scheduling { address = archives.address, age = archives.age.ToString(), archives = archives.name, archives_id = archives.id, clinicid = entity.clinicid, contactno = archives.phone, contactsno = archives.contacts_phone, contancts = archives.contacts, creater = userInfo.name, creater_id = userInfo.id, create_date = DateTime.Now, emergencyno = archives.emergencyno, id = DateTime.Now.ToString("yyyyMMddHHmmssffff") + (99 - index).ToString(), itemid = s.item.item_id, order_num = GetOrderNumByRoom(DateTime.Now, s.pre_entity.room_id), org_id = userInfo.org_id, relation = archives.relation, relationid = archives.relationid, room_id = s.pre_entity.room_id, sign_time = DateTime.Now, specid = s.itemspec.specid, specname = s.itemspec.specname, stateid = 16, store_id = entity.store_id, summary = entity.summay, tradename = s.item.trade_name, work_date = DateTime.Now.Date, work_times = GetWorkTimesByRoom(s.pre_entity.room_id, s.item.item_id, s.itemspec.specid, DateTime.Now.Date), wait_times = rooms.Where(w => w.id == s.pre_entity.room_id).First().wait_times }).ToList();
                    Db.Insertable(room_list).ExecuteCommand();
                    redisCache.RemoveAll<his_room_scheduling>();

                    //#region 项目设备使用排班

                    //var v_clinic_id = new SugarParameter("v_clinic_id", clinicrecord.clinicid);
                    //var v_creater_id = new SugarParameter("v_creater_id", userInfo.id);
                    //var v_creater = new SugarParameter("v_creater", userInfo.name);
                    //var resultProcedure = Db.Ado.UseStoredProcedure().GetString("f_his_equipment", v_clinic_id, v_creater_id, v_creater);
                    //if (resultProcedure != "成功")
                    //{
                    //    throw new MessageException(resultProcedure);
                    //}
                    //redisCache.RemoveAll<his_equipment_work>();
                    //redisCache.RemoveAll<his_equipment_scheduling>();

                    //#endregion

                    //结算单
                    var balance = new f_balance { couponpay = coupon_money, accountpay = accountpay, actualamount = actual_amount, alipay = 0, bankpay = 0, cashpay = 0, centerid = clinicrecord.centerid, checkoutid = 0, clinicid = clinicrecord.clinicid, deptid = clinicrecord.deptid, deptname = clinicrecord.deptname, doctorid = clinicrecord.doctorid, doctorname = clinicrecord.doctorname, insuranceid = 0, insurancepay = 0, issettle = 2, operatorid = clinicrecord.doctorid, operator_name = clinicrecord.doctorname, orgid = clinicrecord.orgid, otherpay = 0, recorddate = DateTime.Now, returnid = 0, shouldamount = entity.shouldamount, stateid = 19, store_id = register.store_id, summay = "诊前检查结算", typeid = 3, wechatpay = 0, is_register = 3, balancedate = DateTime.Now, sourceid = 3, source = "同步结算" };
                    var balance_id = Db.Insertable(balance).ExecuteReturnIdentity();
                    redisCache.RemoveAll<f_balance>();

                    //结算清单,获取最大处方ID
                    var max_receipeid = Db.Queryable<f_balancedetail>().Max(m => m.receipeid);
                    var balancedetails = items.Select((s, index) => new f_balancedetail { actualamount = GetActualAmount(s.itemspec, s.pre_entity, discountRate, account, entity.shouldamount), balanceid = balance_id, cost = s.itemspec.buy_price, execdeptid = clinicrecord.deptid, execdeptname = clinicrecord.deptname, execstateid = 17, execstate = "已执行", frequency = "1次/1天", frequencyid = 1, incomeid = s.itemspec.incomeid, invoicename = s.value, itemname = s.item.trade_name, modulus = s.itemspec.salsemodulus, numbers = 1, orderid = short.Parse(index.ToString()), price = s.itemspec.sale_price, quantity = s.pre_entity.quantity, rebate = discountRate, receipeid = max_receipeid + 1, shouldamount = s.itemspec.sale_price * s.pre_entity.quantity, sigledousage = 1, specid = s.itemspec.specid, specname = s.itemspec.specname, unitname = s.itemspec.salseunit, usageid = 0, usage = "" }).ToList();
                    Db.Insertable(balancedetails).ExecuteCommand();
                    redisCache.RemoveAll<f_balancedetail>();

                    //查询客户所属分销人员
                    var distributor = Db.Queryable<c_distributor, p_distributor>((cd, pd) => new object[] { JoinType.Left, cd.distributor_id == pd.id }).Where((cd, pd) => cd.archives_id == archives.id).Select((cd, pd) => pd).WithCache().First();
                    //查询分销人员提成比例
                    var royalty_rate = Db.Queryable<p_org>().Where(w => w.id == userInfo.org_id).Select(s => s.royalty_rate).WithCache().First();
                    if (distributor != null && royalty_rate > 0)
                    {
                        var sum = actual_amount * royalty_rate / 100;
                        //写入提成记录
                        var amount = new r_amount { archives = archives.name, archives_id = archives.id, archives_phone = archives.phone, amount_date = DateTime.Now, balance_id = balance_id, card_no = archives.card_no, distributor = distributor.name, distributor_id = distributor.id, money = sum, org_id = userInfo.org_id, store_id = balance.store_id };
                        Db.Insertable(amount).ExecuteCommand();
                        redisCache.RemoveAll<r_amount>();
                        //修改分销人员账户信息
                        Db.Updateable<p_distributor>().SetColumns(s => new p_distributor { amount = s.amount + sum, noneamount = s.noneamount + sum }).Where(w => w.id == distributor.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                    }


                    ////修改门诊收费
                    //Db.Updateable<his_applybill>().SetColumns(s => new his_applybill { issettle = 2 }).Where(w => w.applyid == applybill_id && w.store_id == entity.store_id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();


                    //消费记录
                    var recharge = new r_recharge { archives = account_al.archives.name, archives_id = account.archives_id, archives_phone = account_al.archives.phone, balance = account.balance, bill_no = entity.archives_id + DateTime.Now.ToString("yyyyMMddhhmmssffffff"), card_no = account_al.archives.card_no, categroy_id = 2, check_id = balance_id, consultation_id = clinicrecord.clinicid, creater = userInfo.name, creater_id = userInfo.id, level = level.name, give_balance = account.coupon, give_money = coupon_money, give_total_money = account.total_coupon, integral = Convert.ToInt32(account.integral), money_integral = level.money_integral, no = account_al.archives.id + DateTime.Now.ToString("yyMMddhhmmssffff"), occurrence_date = DateTime.Now, org_id = userInfo.org_id, recharge_date = DateTime.Now.AddMinutes(1), recharge_integral = 0, recharge_money = actual_amount, state = "已消费", state_id = 20, store_id = entity.store_id, total_integral = account.total_integral, total_money = account.recharge, way_code = "5", way = "账户", to_director_id = account_al.archives.to_director_id, to_director = account_al.archives.to_director, discount_rate = discountRate };
                    Db.Insertable(recharge).ExecuteCommand();
                    redisCache.RemoveAll<r_recharge>();

                    if (account_al.archives.to_director_id > 0)
                    {
                        ////签到
                        //var signCount = Db.Updateable<his_equipment_scheduling>().SetColumns(s => new his_equipment_scheduling { sign_time = DateTime.Now, stateid = 16 }).Where(w => w.clinicid == clinicrecord.clinicid && w.creater_id == userInfo.id && w.creater == userInfo.name && w.work_date == DateTime.Now.Date && w.stateid == 5 && w.store_id == entity.store_id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                        //if (signCount <= 0)
                        //{
                        //    throw new MessageException("未查到检查诊室！");
                        //}
                        ////查询检查地址
                        //var address = Db.Queryable<p_room, his_equipment_scheduling, p_equipment_detials>((r, es, ed) => new object[] { JoinType.Left, r.id == ed.id, JoinType.Left, es.equipment_id == ed.id }).Where((r, es, ed) => es.clinicid == clinicrecord.clinicid && es.creater_id == userInfo.id && es.creater == userInfo.name && es.work_date == DateTime.Now.Date && es.stateid == 5 && es.store_id == entity.store_id).Select((r, es, ed) => r.position).First();

                        if (account_al.archives.to_director_id>0)
                        {
                            //发送通知
                            var con = $"{{\"name\":\"{account_al.archives.name}\",\"msg\":\"已在{ clinicrecord.deptname}开诊前检查,快去接待吧！\",\"img_url\":\"{account_al.archives.image_url}\"}}";
                            employeeSocket.Add(new WebSocketModel { userId = account_al.archives.to_director_id.Value, content = con });
                                                      
                            content = $"{archives.name}已在{ clinicrecord.deptname}开诊前检查,快去接待吧！";

                            //查询客户余额下限
                            var balance_lower = Db.Queryable<c_archives, c_account, c_archives_level>((a, ac, al) => new object[] { JoinType.Left, ac.archives_id == a.id, JoinType.Left, a.grade_id == al.id })
                                             .Where((a, ac, al) => a.id == entity.archives_id && a.org_id == userInfo.org_id && ac.balance <= al.balance_limit_lower)
                                             .Select((a, ac, al) => new { a.name, a.id, a.to_director_id, a.image_url,ac.balance,al.balance_limit_lower })
                                             .WithCache()
                                             .First();
                            if (balance_lower != null && balance_lower?.to_director_id > 0)
                            {
                                //发送通知
                                var con1 = $"{{\"name\":\"{balance_lower.name}\",\"msg\":\"余额已达下限，请及时处理！\"}}";
                                employeeSocket.Add(new WebSocketModel { userId =archives.to_director_id.Value, content = con1 });
                                                              
                                //调用通知
                                notice_content = $"{{\"name\":\"{balance_lower.name}\",\"msg\":\" 余额已达下限！\",\"balance\":\"{balance_lower.balance}\",\"balance_limit_lower\":\"{balance_lower.balance_limit_lower}\"}}";
                                var employeenotice1 = new List<employeeMes>();
                                employeenotice1.Add(new employeeMes { employee_id = archives.to_director_id.Value, employee = archives.to_director });

                                notice.NewMethod(archives.id.ToString(), archives, entity.store_id, notice, noticeList, 8, 3, notice_content, balance_lower.name, employeenotice1);

                            }

                            var itemnotice = items.Select(s => new { specname=s.itemspec.specname,itemname=s.item.trade_name }).ToList();
                            var itemstring = "";
                            itemnotice.ForEach(c => {
                                itemstring +=c.specname + "-" + c.itemname + ",";

                            });
                            itemstring=itemstring.TrimEnd(',');
                            //诊前检查通知
                            notice_content = $"{{\"name\":\"{account_al.archives.name}\",\"dept\":\"{clinicrecord.deptname}\",\"item\":\"{itemstring}\",\"doctorname\":\"{clinicrecord.doctorname}\",\"msg\":\" 诊前检查通知\"}}";
                            var employeenotice = new List<employeeMes>();
                            employeenotice.Add(new employeeMes { employee_id = account_al.archives.to_director_id.Value, employee =account_al.archives.to_director });

                            notice.NewMethod(applybill_id.ToString(),archives, entity.store_id, notice, noticeList, 5, 4, notice_content, account_al.archives.name, employeenotice);
                            //新增通知
                            notice.AddNotice(noticeList);
                            //消息提醒
                            ChatWebSocketMiddleware.SendListAsync(employeeSocket);

                        }
                    }
                    //发送短信


                }
                else
                {
                    //删除已存在申请单
                    Db.Deleteable<his_applybill>().Where(w => w.applyid == entity.applyid).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                    Db.Deleteable<his_applycontent>().Where(w => w.applyid == entity.applyid).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                    //门诊申请单
                    var applybill = new his_applybill { orgid = userInfo.org_id, clinicid = clinicrecord.clinicid, centerid = entity.archives_id, typeid = entity.typeid, typename = entity.typename, regid = register.regid, recorddate = DateTime.Now, doctorid = clinicrecord.doctorid, doctorname = clinicrecord.doctorname, deptid = clinicrecord.deptid, deptname = clinicrecord.deptname, stateid = 5, statename = "正常", issettle = 3, shouldamount = entity.shouldamount, actualamount = 0, days = entity.days, diagnosiscode = entity.diagnosiscode, diagnosisname = entity.diagnosisname, double_num = entity.double_num, summay = entity.summay, store_id = entity.store_id };
                    applybill_id = Db.Insertable(applybill).ExecuteReturnIdentity();
                    redisCache.RemoveAll<his_applybill>();

                    //门诊申请单明细
                    var applybilldetails = items.Select((s, index) => new his_applycontent { applyid = applybill_id, orderid = short.Parse(index.ToString()), specid = s.itemspec.specid, specname = s.itemspec.specname, packid = "0", content = s.pre_entity.content, quantity = s.pre_entity.quantity, price = s.itemspec.sale_price, cost = s.itemspec.buy_price, shouldamount = s.itemspec.sale_price * s.pre_entity.quantity, actualamount = 0, usageid = s.pre_entity.usageid, usagename = s.pre_entity.usagename, frequecyname = s.pre_entity.frequecyname, frequecyid = s.pre_entity.frequecyid, sigle = s.pre_entity.sigle, dosageunit = s.itemspec.dosageunit, execdeptid = 0, execdept = "", execstateid = 16, execstate = "待执行", unitname = s.itemspec.salseunit, modulus = s.itemspec.salsemodulus, groupid = s.pre_entity.groupid, item_id = s.item.item_id, item_name = s.item.trade_name, org_id = userInfo.org_id, store_id = entity.store_id }).ToList();
                    Db.Insertable(applybilldetails).ExecuteCommand();
                    redisCache.RemoveAll<his_applycontent>();
                }

            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            //如果是诊前检查
            if (entity.typeid == 6)
            {
                //发送短信
                Dictionary<string, string> values = new Dictionary<string, string>();
                values.Add("archives", archives.name);
                var toValues = JsonConvert.SerializeObject(values);
                var sendSms = new Public.SendSMSService();
                //await sendSms.SendSmsAsync(employee.phone_no, 6, toValues, content, 1, archives.org_id);
            }
            return applybill_id;
        }

        /// <summary>
        /// 返回号序（设备）
        /// </summary>
        /// <param name="dateTime">日期</param>
        /// <param name="equipment_id">设备ID</param>
        /// <param name="equipment_no">设备编号</param>
        /// <returns></returns>
        public short GetOrderNum(DateTime dateTime, int equipment_id, string equipment_no)
        {
            short order_num = 1;

            //查询指定日期设备的最大号序
            short? max = Db.Queryable<his_equipment_scheduling>().Where(w => w.work_date == dateTime.Date && w.equipment_id == equipment_id && w.no == equipment_no).WithCache().Max(m => m.order_num);
            if (max != null)
            {
                order_num = short.Parse((max.Value + 1).ToString());
            }

            return order_num;
        }

        /// <summary>
        /// 返回号序（医疗室）
        /// </summary>
        /// <param name="dateTime">日期</param>
        /// <param name="room_id">医疗室ID</param>
        /// <returns></returns>
        public short GetOrderNumByRoom(DateTime dateTime, int room_id)
        {
            short order_num = 1;

            //查询指定日期设备的最大号序
            short? max = Db.Queryable<his_room_scheduling>().Where(w => w.work_date == dateTime.Date && w.room_id == room_id).WithCache().Max(m => m.order_num);
            if (max != null)
            {
                order_num = short.Parse((max.Value + 1).ToString());
            }

            return order_num;
        }

        /// <summary>
        /// 返回工作时长并添加已工作时长（设备）
        /// </summary>
        /// <param name="equipment_id">设备ID</param>
        /// <param name="itemid">项目ID</param>
        /// <param name="specid">规格ID</param>
        /// <param name="no">设备编号</param>
        /// <param name="dateTime">日期</param>
        /// <returns></returns>
        public short? GetWorkTimes(int equipment_id, int itemid, int specid, string no, DateTime dateTime)
        {
            var work_times = Db.Queryable<p_equipment_itemspec>().Where(w => w.equipment_id == equipment_id && w.itemid == itemid && w.specid == specid).Select(s => s.work_times).WithCache().First();
            var equipment_work = Db.Queryable<his_equipment_work>().Where(w => w.equipment_id == equipment_id && w.no == no && w.work_date == dateTime).WithCache().First();
            if (equipment_work == null)
            {
                var equipment_detials = Db.Queryable<p_equipment_detials>().Where(w => w.id == equipment_id && w.no == no).WithCache().First();
                Db.Insertable(new his_equipment_work { equipment_id = equipment_id, no = no, total_times = equipment_detials.work_times.Value, work_date = dateTime, work_times = work_times.Value }).ExecuteCommand();
                redisCache.RemoveAll<his_equipment_work>();
            }
            else
            {
                work_times += equipment_work.work_times;
                Db.Updateable<his_equipment_work>().SetColumns(s => new his_equipment_work { work_times = work_times.Value }).Where(w => w.equipment_id == equipment_id && w.no == no && w.work_date == dateTime).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
            }
            return work_times;
        }

        /// <summary>
        /// 返回工作时长并添加已工作时长（医疗室）
        /// </summary>
        /// <param name="room_id">医疗室ID</param>
        /// <param name="itemid">项目ID</param>
        /// <param name="specid">规格ID</param>
        /// <param name="dateTime">日期</param>
        /// <returns></returns>
        public short? GetWorkTimesByRoom(int room_id, int itemid, int specid, DateTime dateTime)
        {
            var work_times = Db.Queryable<p_room_itemspec>().Where(w => w.room_id == room_id && w.itemid == itemid && w.specid == specid).Select(s => s.work_times).WithCache().First();
            var room_work = Db.Queryable<his_room_work>().Where(w => w.room_id == room_id && w.work_date == dateTime).WithCache().First();
            if (room_work == null)
            {
                var room = Db.Queryable<p_room>().Where(w => w.id == room_id).WithCache().First();
                Db.Insertable(new his_room_work { room_id = room_id, total_times = room.work_times.Value, work_date = dateTime, work_times = work_times.Value }).ExecuteCommand();
                redisCache.RemoveAll<his_room_work>();
            }
            else
            {
                work_times += room_work.work_times;
                Db.Updateable<his_room_work>().SetColumns(s => new his_room_work { work_times = work_times.Value }).Where(w => w.room_id == room_id && w.work_date == dateTime).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
            }
            return work_times;
        }

        /// <summary>
        /// 返回应收金额
        /// </summary>
        /// <param name="itemspec"></param>
        /// <param name="pre_entity"></param>
        /// <param name="discountRate"></param>
        /// <param name="account"></param>
        /// <param name="shouldamount"></param>
        /// <returns></returns>
        public decimal GetActualAmount(h_itemspec itemspec, PrescriptionItem pre_entity, decimal discountRate, c_account account, decimal shouldamount)
        {

            if (account.balance >= shouldamount)
            {
                return (itemspec.sale_price * pre_entity.quantity) * discountRate / 100;
            }
            else
            {
                return itemspec.sale_price * pre_entity.quantity;
            }
        }

        /// <summary>
        /// 获取处方
        /// </summary>
        /// <param name="clinic_id">就诊ID</param>
        /// <param name="typeid">单据类型ID</param>
        /// <returns></returns>
        public async Task<List<Prescription>> GetPrescriptionAsync(int clinic_id, int typeid)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            //获取申请单列表
            var applys = await Db.Queryable<his_applybill>().Where(w => w.clinicid == clinic_id && w.typeid == typeid).WithCache().ToListAsync();
            //获取所有申请单ID
            var applyids = applys.Select(s => s.applyid).ToList();
            //获取所有申请单明细规格ID
            var applydetail_specids = await Db.Queryable<his_applycontent>().Where(w => applyids.Contains(w.applyid)).Select(s => s.specid).WithCache().ToListAsync();
            //获取项目ID
            var applydetail_itemids = await Db.Queryable<h_itemspec>().Where(w => applydetail_specids.Contains(w.specid)).Select(s => s.itemid).WithCache().ToListAsync();
            //获取项目规格
            var itemspecs = await Db.Queryable<h_itemspec>().Where(w => applydetail_itemids.Contains(w.itemid)).WithCache().ToListAsync();
            //获取所有申请单对应申请明细
            var applydetails = await Db.Queryable<his_applycontent, h_itemspec, h_item>((b, ip, i) => new object[] { JoinType.Left, b.specid == ip.specid, JoinType.Left, ip.itemid == i.item_id }).Where((b, ip, i) => applyids.Contains(b.applyid)).Select((b, ip, i) => new { b.applyid, b.frequecyname, b.frequecyid, b.quantity, b.content, b.specid, b.usagename, b.usageid, ip.specname, ip.itemid, i.trade_name, b.groupid, b.packid, b.sigle }).ToListAsync();
            return applys.Select(s => new Prescription { issettle = s.issettle, typeid = s.typeid, typename = s.typename, applyid = s.applyid, archives_id = s.centerid, shouldamount = s.shouldamount, store_id = s.store_id, summay = s.summay, clinicid = s.clinicid, days = s.days, diagnosiscode = s.diagnosiscode, diagnosisname = s.diagnosisname, double_num = s.double_num, prescriptionItems = applydetails.Where(w => w.applyid == s.applyid).Select(ss => new PrescriptionItem { content = ss.content, quantity = ss.quantity, specid = ss.specid, usagename = ss.usagename, usageid = ss.usageid, itemid = ss.itemid, itemname = ss.trade_name, specname = ss.specname, frequecyid = ss.frequecyid, frequecyname = ss.frequecyname, groupid = ss.groupid, packid = ss.packid, sigle = ss.sigle, specList = itemspecs.Where(w => w.itemid == ss.itemid).Select(sss => new ItemSpecs { sale_price = sss.sale_price, salseunit = sss.salseunit, specid = sss.specid, specname = sss.specname }).ToList() }).ToList() }).ToList();
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <returns></returns>
        public async Task<int> DeleteAsync(int id)
        {
            //删除已存在申请信息
            await Db.Deleteable<his_applybill>().Where(w => w.applyid == id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
            await Db.Deleteable<his_applycontent>().Where(w => w.applyid == id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
            return 1;
        }

        /// <summary>
        /// 完成诊疗
        /// </summary>
        /// <param name="clinic_id">就诊ID</param>
        /// <returns></returns>
        public async Task<bool> FinishAsync(int clinic_id)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            var archives = new c_archives();
            var employee = new p_employee();
            var content = "";

            //通知
            var notice = new Business.NoticeService();
            var noticeList = new List<AddNoticeModel>();
            var notice_content = "";
            var employeeSocket = new List<WebSocketModel>();

            var result = Db.Ado.UseTran(() =>
            {
                //查询就诊记录
                var clinicrecord = Db.Queryable<his_clinicrecord>().Where(w => w.clinicid == clinic_id).WithCache().First();
                //修改就诊记录完成时间
                Db.Updateable<his_clinicrecord>().SetColumns(s => new his_clinicrecord { enddate = DateTime.Now }).Where(w => w.clinicid == clinic_id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                var register_archives_employee = Db.Queryable<his_register, c_archives, p_employee>((r, a, e) => new object[] { JoinType.Left, r.centerid == a.id, JoinType.Left, a.to_director_id == e.id }).Where((r, a, e) => r.clinicid == clinic_id).Select((r, a, e) => new { register = r, archives = a, employee = e }).WithCache().First();

                var register = register_archives_employee.register;
                archives = register_archives_employee.archives;
                employee = register_archives_employee.employee;

                Db.Updateable<his_register>().SetColumns(s => new his_register { clinicstateid = 15 }).Where(w => w.clinicid == clinic_id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                //查询收费单信息
                var applys = Db.Queryable<his_applybill>().Where(w => w.clinicid == clinic_id && w.typeid != 6).WithCache().ToList();

                //总金额
                var sum = applys.Sum(s => s.shouldamount);

                short stateid = 18;//状态
                short issettle = 3;//是否结算
                short sourceid = 0;//来源ID
                string source = "";//来源
                DateTime? balancedate = null;//结算日期

                if (sum == 0)
                {
                    stateid = 19;
                    issettle = 2;
                    sourceid = 3;
                    source = "同步结算";
                    balancedate = DateTime.Now;

                    //修改就诊记录总费用
                    var clinicrecord_amount = Db.Queryable<his_clinicrecord>().Where(w => w.clinicid == clinic_id).First().amount;
                    var amount = Db.Queryable<f_balance>().Where(w => w.clinicid == clinic_id).Sum(s => s.actualamount);
                    clinicrecord_amount += amount;
                    Db.Updateable<his_clinicrecord>().SetColumns(s => new his_clinicrecord { amount = clinicrecord_amount }).Where(w => w.clinicid == clinic_id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                }

                //修改门诊收费
                Db.Updateable<his_applybill>().SetColumns(s => new his_applybill { applydate = DateTime.Now, stateid = 15, statename = "已完成", issettle = issettle }).Where(w => w.clinicid == clinic_id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                //添加门诊收费结算单
                var balance = new f_balance { accountpay = 0, actualamount = 0, alipay = 0, bankpay = 0, cashpay = 0, centerid = clinicrecord.centerid, checkoutid = 0, clinicid = clinicrecord.clinicid, deptid = clinicrecord.deptid, deptname = clinicrecord.deptname, doctorid = clinicrecord.doctorid, doctorname = clinicrecord.doctorname, insuranceid = 0, insurancepay = 0, issettle = issettle, operatorid = 0, operator_name = "", orgid = clinicrecord.orgid, otherpay = 0, recorddate = DateTime.Now, returnid = 0, shouldamount = sum, stateid = stateid, store_id = register.store_id, summay = "完成诊疗", typeid = 1, wechatpay = 0, is_register = 3, sourceid = sourceid, source = source, balancedate = balancedate };
                var balance_id = Db.Insertable(balance).ExecuteReturnIdentity();
                redisCache.RemoveAll<f_balance>();

                //获取商品规格对应项目及票据类型
                var items = Db.Queryable<his_applycontent, his_applybill, h_itemspec, h_item, b_basecode>((ac, ab, isp, i, b) => new object[] { JoinType.Left, ac.applyid == ab.applyid, JoinType.Left, ac.specid == isp.specid, JoinType.Left, isp.itemid == i.item_id, JoinType.Left, isp.incomeid == b.valueid && b.baseid == 66 }).Where((ac, ab, isp, i, b) => ab.clinicid == clinic_id && ab.typeid != 6).Select((ac, ab, isp, i, b) => new { applycontent = ac, applybill = ab, itemspec = isp, item = i, b.value }).WithCache().ToList();

                //添加门诊收费结算单明细
                var discount_rate = Db.Queryable<c_archives, c_archives_level>((a, al) => new object[] { JoinType.Left, a.grade_id == al.id }).Where((a, al) => a.id == clinicrecord.centerid).Select((a, al) => al.discount_rate).WithCache().First();
                //获取最大处方ID
                var max_receipeid = Db.Queryable<f_balancedetail>().Max(m => m.receipeid);
                var balancedetails = items.Select((s, index) => new f_balancedetail { actualamount = 0, balanceid = balance_id, cost = s.itemspec.buy_price, execdeptid = 0, execdeptname = "", execstateid = 16, execstate = "待执行", frequency = s.applycontent.frequecyname, frequencyid = s.applycontent.frequecyid, incomeid = s.itemspec.incomeid, invoicename = s.value, itemname = s.item.trade_name, modulus = s.itemspec.salsemodulus, numbers = s.applybill.double_num, orderid = short.Parse(index.ToString()), price = s.itemspec.sale_price, quantity = s.applycontent.quantity, rebate = discount_rate.Value, receipeid = max_receipeid + 1, shouldamount = s.itemspec.sale_price * s.applycontent.quantity, sigledousage = s.applycontent.sigle, specid = s.itemspec.specid, specname = s.itemspec.specname, unitname = s.itemspec.salseunit, usageid = int.Parse(s.applycontent.usageid.ToString()), usage = s.applycontent.usagename }).ToList();
                Db.Insertable(balancedetails).ExecuteCommand();
                redisCache.RemoveAll<f_balancedetail>();


                //#region 项目设备使用排班

                //var v_clinic_id = new SugarParameter("v_clinic_id", clinic_id);
                //var v_creater_id = new SugarParameter("v_creater_id", userInfo.id);
                //var v_creater = new SugarParameter("v_creater", userInfo.name);
                //var resultProcedure = Db.Ado.UseStoredProcedure().GetString("f_his_equipment", v_clinic_id, v_creater_id, v_creater);
                //if (resultProcedure != "成功")
                //{
                //    throw new MessageException(resultProcedure);
                //}
                //redisCache.RemoveAll<his_equipment_work>();
                //redisCache.RemoveAll<his_equipment_scheduling>();

                //#endregion

                //发送通知
                var con = $"{{\"name\":\"{archives.name}\",\"msg\":\"已在{ clinicrecord.deptname}完成诊疗,快去接待吧！\",\"img_url\":\"{archives.image_url}\"}}";
                employeeSocket.Add(new WebSocketModel { userId = archives.to_director_id.Value, content = con });
                               
                content = $"{archives.name}已在{ clinicrecord.deptname}完成诊疗,快去接待吧！";

                //完成诊疗通知
                notice_content = $"{{\"name\":\"{archives.name}\",\"dept\":\"{ clinicrecord.deptname}\",\"msg\":\" 完成诊疗\"}}";
                var employeenotice = new List<employeeMes>();
                employeenotice.Add(new employeeMes { employee_id = archives.to_director_id.Value, employee = archives.to_director });

                notice.NewMethod(clinic_id.ToString(),archives, register.store_id, notice, noticeList, 5, 3, notice_content, archives.name, employeenotice);
                //新增通知
                notice.AddNotice(noticeList);
                //消息提醒
                ChatWebSocketMiddleware.SendListAsync(employeeSocket);

            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            //发送短信
            Dictionary<string, string> values = new Dictionary<string, string>();
            values.Add("archives", archives.name);
            var toValues = JsonConvert.SerializeObject(values);
            var sendSms = new Public.SendSMSService();
            //await sendSms.SendSmsAsync(employee.phone_no, 6, toValues, content, 1, archives.org_id);
            return result.IsSuccess;
        }

        /// <summary>
        /// 获得病历列表
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="archives_id">会员ID</param>
        /// <param name="start_clinic_time">就诊时间（开始）</param>
        /// <param name="end_clinic_time">就诊时间（结束）</param>
        /// <returns></returns>
        public async Task<List<ClinicMr>> GetClinicMrListAsync(int store_id, short archives_id, DateTime? start_clinic_time, DateTime? end_clinic_time)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            if (start_clinic_time != null)
            {
                start_clinic_time = start_clinic_time.Value.Date;
            }
            if (end_clinic_time != null)
            {
                end_clinic_time = end_clinic_time.Value.AddDays(1).Date;
            }
            return await Db.Queryable<his_clinic_mr>()
                .Where(w => w.org_id == userInfo.org_id)
                .WhereIF(store_id > 0, w => w.store_id == store_id)
                .WhereIF(archives_id > 0, w => w.archives_id == archives_id)
                .WhereIF(start_clinic_time != null, w => w.clinic_time >= start_clinic_time)
                .WhereIF(end_clinic_time != null, w => w.clinic_time < end_clinic_time)
                .Select(s => new ClinicMr { clinicid = s.clinicid, clinic_time = s.clinic_time, diagnosis = s.diagnosis })
                .OrderBy(s => s.clinicid)
                .WithCache()
                .ToListAsync();
        }

        /// <summary>
        /// 获取负责人客户签到列表
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="state_id">状态（1=已签到，0=未签到，7=已取消）</param>
        /// <returns></returns>
        public async Task<List<SignRecord>> GetSignListAsync(int store_id, short state_id)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            var list = new List<SignRecord>();

            var now = DateTime.Now.ToString("yyyy-MM-dd");
            //门诊
            var registerList = await Db.Queryable<his_register, c_archives, his_outpatient_no>((r, a, hon) => new object[] { JoinType.Left, r.centerid == a.id, JoinType.Left, r.regid == hon.regid })
                .Where((r, a, hon) => r.stateid == 11 && r.store_id == store_id && r.canceled == 3 && a.to_director_id == userInfo.id)
                .Where("to_char(r.regdate,'yyyy-MM-dd')=@now", new { now })
                .WhereIF(state_id == 0, (r, a, hon) => r.clinicstateid == 5)
                .WhereIF(state_id == 1, (r, a, hon) => r.clinicstateid == 14 || r.clinicstateid == 15 || r.clinicstateid == 13)
                .Select((r, a, hon) => new SignRecord { content = SqlFunc.MergeString(r.billtype, "就诊"), id = r.regid, name = a.name, phone = a.phone, type_id = 1, type_name = "门诊", archives_id = a.id, doctorname = r.doctorname, to_director = a.to_director, sign_time = hon.order_time })
                .WithCache()
                .ToListAsync();

            //合并处理
            registerList.ForEach(c =>
            {
                list.Add(c);
            });

            //康复预约
            var recoverList = await Db.Queryable<his_recover, c_archives, h_item>((r, a, i) => new object[] { JoinType.Left, r.centerid == a.id, JoinType.Left, r.item_id == i.item_id })
                 .Where((r, a, i) => r.orgid == userInfo.org_id && r.store_id == store_id && r.canceled == 3 && a.to_director_id == userInfo.id)
                .Where("to_char(r.regdate,'yyyy-MM-dd')=@now", new { now })
                .WhereIF(state_id == 1, (r, a, i) => r.stateid != 5 && r.stateid != 7)
                .WhereIF(state_id == 0, (r, a, i) => r.stateid == 5)
                .WhereIF(state_id == 7, (r, a, i) => r.stateid == 7)
                .Select((r, a, i) => new SignRecord { content = SqlFunc.MergeString(r.specname, "（", r.item_name, "）"), id = r.recoverid, name = a.name, phone = a.phone, type_id = 2, type_name = "康复", archives_id = a.id, doctorname = r.doctorname, to_director = a.to_director, sign_time = r.sign_time, item_id = r.item_id, spec_id = r.specid, equipment = i.equipment })
                .WithCache()
                .ToListAsync();

            var item_ids = recoverList.Select(s => s.item_id).ToList();

            //设备安排
            var equipment = await Db.Queryable<his_equipment_scheduling, p_equipment_detials, p_equipment>((es, ed, e) => new object[] { JoinType.Left, es.equipment_id == ed.id && es.no == ed.no, JoinType.Left, ed.id == e.id }).Where((es, ed, e) => item_ids.Contains(es.itemid)).Select((es, ed, e) => new equipment { recoverid = es.recoverid, no = ed.no, name = e.name, model = e.model, equipment_address = es.equipment_address, room_name = es.room_name }).WithCache().ToListAsync();
            //医疗室安排
            var room = await Db.Queryable<his_room_scheduling, p_room>((rs, r) => new object[] { JoinType.Left, rs.room_id == r.id }).Where((rs, r) => item_ids.Contains(rs.itemid)).Select((rs, r) => new room { position = r.position, name = r.name, recoverid = rs.recoverid }).WithCache().ToListAsync();

            recoverList = recoverList.Select(s => new SignRecord { archives_id = s.archives_id, content = s.content, doctorname = s.doctorname, equipment = s.equipment, id = s.id, item_id = s.item_id, name = s.name, phone = s.phone, sign_time = s.sign_time, spec_id = s.spec_id, to_director = s.to_director, type_id = s.type_id, type_name = s.type_name, equipment_info = GetEquipmentInfo(s, equipment), room_info = GetRoomInfo(s, room, equipment) }).ToList();

            //合并处理
            recoverList.ForEach(c =>
            {
                list.Add(c);
            });

            return list;
        }

        /// <summary>
        /// 获取就诊记录分页
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="name">客户名称/手机号</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <param name="start_regdate">就诊时间（开始）</param>
        /// <param name="end_regdate">就诊时间（结束）</param>
        /// <param name="is_me">是否自己客户</param>
        /// <returns></returns>
        public async Task<dynamic> GetRecordPagesAsync(int store_id, string name, string order, int orderType, int limit, int page, DateTime? start_regdate, DateTime? end_regdate, bool is_me = false)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var orderTypeStr = " Asc";
            if (orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            if (start_regdate != null)
            {
                start_regdate = start_regdate.Value.Date;
            }
            if (end_regdate != null)
            {
                end_regdate = end_regdate.Value.AddDays(1).Date;
            }
            return await Db.Queryable<his_clinicrecord, his_register, c_archives>((c, r, a) => new object[] { JoinType.Left, c.clinicid == r.clinicid, JoinType.Left, c.centerid == a.id })
                .Where((c, r, a) => r.store_id == store_id && r.orgid == userInfo.org_id && r.clinicstateid == 15)
                .WhereIF(!string.IsNullOrEmpty(name), (c, r, a) => a.name.Contains(name) || a.phone.Contains(name))
                .WhereIF(is_me, (c, r, a) => a.to_director_id == userInfo.id || c.doctorid == userInfo.id)
                .WhereIF(start_regdate != null, (c, r, a) => c.regdate >= start_regdate)
                .WhereIF(end_regdate != null, (c, r, a) => c.regdate < end_regdate)
                .Select((c, r, a) => new { a.name, c.address, c.age, c.amount, c.begindate, c.clinicid, c.contactno, c.contactsno, c.contancts, c.customer, c.deptname, c.diagnosisname, c.doctorname, c.education, c.emergencyno, c.enddate, c.marriage, c.nursename, c.patienttype, c.regdate, c.relation, c.salseman, c.summary, c.vocation })
                .OrderBy(order + orderTypeStr)
                .WithCache()
                .ToPageAsync(page, limit);
        }
    }
}
