using Models.DB;
using Models.View.Business;
using Models.View.His;
using Newtonsoft.Json;
using Service.Extensions;
using Service.Repository.Interfaces.His;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tools;
using Tools.Filter;
using Tools.WebSocket;

namespace Service.Repository.Implements.His
{
    /// <summary>
    /// 医疗室排班
    /// </summary>
    public class RoomSchedulingService : DbContext, IRoomSchedulingService
    {
        /// <summary>
        /// 结束
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> EndAsync(Execute entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var next_e = false;
            var next = new { name = "", image_url = "", to_director_id = 0, specname = "", tradename = "", room_name = "", position = "", store_id = 0, id = 0, phone = "", to_director = "" };
            //通知
            var notice = new Business.NoticeService();
            var noticeList = new List<AddNoticeModel>();
            var notice_content = "";
            var employeeSocket = new List<WebSocketModel>();

            var result = Db.Ado.UseTran(() =>
            {
                //执行康复项目
                Db.Updateable<his_room_scheduling>().SetColumns(s => new his_room_scheduling { stateid = 15, summary = entity.summary, work_time_end = DateTime.Now.ToLongTimeString() }).Where(w => w.id == entity.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                var room_archives = Db.Queryable<his_room_scheduling, c_archives>((es, a) => new object[] { JoinType.Left, es.archives_id == a.id }).Where((es, a) => es.id == entity.id).Select((es, a) => new { a.to_director_id, a.to_director, a.id, a.name, a.phone, a.id_card, es.specname, es.tradename, es.store_id, es.clinicid, es.room_id, es.order_num }).WithCache().First();

                //添加随访计划
                var r_follow_up = new r_follow_up { archives_id = room_archives.id, archives = room_archives.name, archives_phone = room_archives.phone, content = room_archives.specname + "（" + room_archives.tradename + "）", task_date = DateTime.Now.AddDays(1), state = 16, creater = userInfo.name, creater_id = userInfo.id, create_date = DateTime.Now, store_id = room_archives.store_id, org_id = userInfo.org_id, id_card = room_archives.id_card, answer = "" };
                var follow_id = Db.Insertable(r_follow_up).ExecuteReturnIdentity();
                redisCache.RemoveAll<r_follow_up>();


                if (room_archives.to_director_id > 0)
                {
                    //随访通知
                    notice_content = $"{{\"date\":\"{r_follow_up.task_date}\",\"name\":\"{r_follow_up.archives}\",\"msg\":\"随访计划\"}}";
                    var archives = new c_archives();
                    archives.id = r_follow_up.archives_id.Value;
                    archives.name = r_follow_up.archives;
                    archives.phone = r_follow_up.archives_phone;
                    var employeenotice = new List<employeeMes>();
                    employeenotice.Add(new employeeMes { employee_id = room_archives.to_director_id.Value, employee = room_archives.to_director });

                    notice.NewMethod(follow_id.ToString(), archives, r_follow_up.store_id.Value, notice, noticeList, 8, 4, notice_content, r_follow_up.archives, employeenotice);

                }

                //修改医疗室状态
                Db.Updateable<p_room>().SetColumns(s => new p_room { state = 30 }).Where(w => w.id == room_archives.room_id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                var room_exist = Db.Queryable<his_room_scheduling>().Where(w => w.clinicid == room_archives.clinicid && w.stateid != 15).WithCache().Any();
                var exist = Db.Queryable<his_equipment_scheduling>().Where(w => w.clinicid == room_archives.clinicid && w.stateid != 15).WithCache().Any();
                if (!exist && !room_exist)
                {
                    //修改就诊记录状态
                    Db.Updateable<his_clinicrecord>().SetColumns(s => new his_clinicrecord { stateid = 15 }).Where(w => w.clinicid == room_archives.clinicid).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                    //查询门诊病历
                    var his_clinic_mr = Db.Queryable<his_clinic_mr>().Where(w => w.clinicid == room_archives.clinicid).First();
                    //添加回访计划
                    var r_practice = new r_practice { archives_id = room_archives.id, archives = room_archives.name, archives_phone = room_archives.phone, content = his_clinic_mr.diagnosis, task_date = DateTime.Now.AddDays(1), state = 16, creater = userInfo.name, creater_id = userInfo.id, create_date = DateTime.Now, store_id = room_archives.store_id, org_id = userInfo.org_id, id_card = room_archives.id_card, answer = "" };
                    Db.Insertable(r_practice).ExecuteCommand();
                    redisCache.RemoveAll<r_practice>();


                }

                //通知下一个负责人
                next = Db.Queryable<his_room_scheduling, c_archives, p_room>((es, a, r) => new object[] { JoinType.Left, es.archives_id == a.id, JoinType.Left, es.room_id == r.id }).Where((es, a, r) => es.room_id == room_archives.room_id && es.order_num > room_archives.order_num).OrderBy((es, a, r) => es.order_num).Select((es, a, r) => new { a.name, a.image_url, to_director_id = a.to_director_id.Value, es.specname, es.tradename, room_name = r.name, r.position, a.store_id, a.id, a.phone, a.to_director }).WithCache().First();
                if (next != null && next?.to_director_id > 0)
                {
                    next_e = true;
                    //发送通知
                    var con = $"{{\"name\":\"{next.name}\",\"msg\":\"即将进行康复治疗（{next.specname}-{next.tradename}），请做好准备！（{next.room_name}-{next.position}）\",\"img_url\":\"{next.image_url}\"}}";
                    employeeSocket.Add(new WebSocketModel { userId = next.to_director_id, content = con });

                    //调用通知
                    notice_content = $"{{\"name\":\"{next.name}\",\"item\":\" （{next.specname}-{next.tradename}）\",\"address\":\"（{next.room_name}-{next.position}）\",\"msg\":\"即将康复治疗\"}}";
                    var archives = new c_archives();
                    archives.id = next.id;
                    archives.name = next.name;
                    archives.phone = next.phone;
                    var employeenotice = new List<employeeMes>();
                    employeenotice.Add(new employeeMes { employee_id = next.to_director_id, employee = next.to_director });

                    notice.NewMethod(room_archives.clinicid.ToString(), archives, next.store_id, notice, noticeList, 5, 5, notice_content, next.name, employeenotice);
                }
            });
            if (next_e)
            {
                //发送短信
                var employee = await Db.Queryable<p_employee>().Where(w => w.id == next.to_director_id).WithCache().FirstAsync();
                var content = $"{next.name}即将进行康复治疗（{next.specname}-{next.tradename}），请做好准备！（{next.room_name}-{next.position}）。";
                Dictionary<string, string> values = new Dictionary<string, string>();
                values.Add("archives", next.name);
                var toValues = JsonConvert.SerializeObject(values);
                var sendSms = new Public.SendSMSService();
                //await sendSms.SendSmsAsync(employee.phone_no, 6, toValues, content, 1, employee.org_id);
            }
            if (noticeList.Count > 0)
            {
                //新增通知
                notice.AddNotice(noticeList);
                //消息提醒
                ChatWebSocketMiddleware.SendListAsync(employeeSocket);

            }
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 获得分页列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<dynamic> GetPagesAsync(RoomSchedulingSearch entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            if (entity.startTime != null)
            {
                entity.startTime = entity.startTime.Value.Date;
            }
            if (entity.endTime != null)
            {
                entity.endTime = entity.endTime.Value.AddDays(1).Date;
            }
            return await Db.Queryable<his_room_scheduling, c_archives, p_room>((es, a, r) => new object[] { JoinType.Left, es.archives_id == a.id, JoinType.Left, es.room_id == r.id })
                .Where((es, a, r) => es.org_id == userInfo.org_id)
                .WhereIF(entity.store_id > 0, (es, a, r) => es.store_id == entity.store_id)
                .WhereIF(entity.state_id > 0, (es, a, r) => es.stateid == entity.state_id)
                .WhereIF(entity.room_id > 0, (es, a, r) => es.room_id == entity.room_id)
                .WhereIF(entity.is_me, (es, a, r) => a.to_director_id == userInfo.id || es.creater_id == userInfo.id || es.executor_id == userInfo.id)
                .WhereIF(!string.IsNullOrEmpty(entity.name), (es, a, r) => es.archives.Contains(entity.name) || es.contactno.Contains(entity.name))
                .WhereIF(entity.startTime != null, (es, a, r) => es.work_date >= entity.startTime)
                .WhereIF(entity.endTime != null, (es, a, r) => es.work_date < entity.endTime)
                .Select((es, a, r) => new { es.id, a.name, es.order_num, es.tradename, es.specname, es.sign_time, es.stateid, es.work_date, es.work_time_start, es.work_time_end, room_name = r.name, a.phone })
                .OrderBy(entity.order + orderTypeStr)
                .WithCache()
                .ToPageAsync(entity.page, entity.limit);
        }

        /// <summary>
        /// 通知
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> NoticeAsync(Execute entity)
        {
            //通知
            var notice = new Business.NoticeService();
            var noticeList = new List<AddNoticeModel>();
            var notice_content = "";
            var employeeSocket = new List<WebSocketModel>();

            //查询设备排班信息
            var his_room_scheduling = await Db.Queryable<his_room_scheduling>().Where(w => w.id == entity.id).WithCache().FirstAsync();
            //查询客户信息
            var archives = await Db.Queryable<c_archives>().Where(w => w.id == his_room_scheduling.archives_id).WithCache().FirstAsync();
            //查询医疗室
            var room = await Db.Queryable<p_room>().Where(w => w.id == his_room_scheduling.room_id).WithCache().FirstAsync();
            if (archives.to_director_id > 0)
            {
                //发送通知
                var con = $"{{\"name\":\"{his_room_scheduling.archives}\",\"msg\":\"康复即将结束，快去接待吧！（位置：{room.position}）\",\"img_url\":\"{archives.image_url}\"}}";
                employeeSocket.Add(new WebSocketModel { userId = archives.to_director_id.Value, content = con });

                //调用通知
                notice_content = $"{{\"name\":\"{his_room_scheduling.archives}\",\"address\":\"{room.position}\",\"msg\":\"康复将结束\"}}";
                var employeenotice = new List<employeeMes>();
                employeenotice.Add(new employeeMes { employee_id = archives.to_director_id.Value, employee = archives.to_director });

                notice.NewMethod(archives.id.ToString(), archives, his_room_scheduling.store_id, notice, noticeList, 5, 1, notice_content, his_room_scheduling.archives, employeenotice);
                //新增通知
                await notice.AddNoticeAsync(noticeList);
                //消息提醒
                ChatWebSocketMiddleware.SendListAsync(employeeSocket);

                //发送短信
                var employee = await Db.Queryable<p_employee>().Where(w => w.id == archives.to_director_id).WithCache().FirstAsync();
                var content = $"{his_room_scheduling.archives}康复即将结束，快去接待吧！（位置：{room.position}）";
                Dictionary<string, string> values = new Dictionary<string, string>();
                values.Add("archives", his_room_scheduling.archives);
                var toValues = JsonConvert.SerializeObject(values);
                var sendSms = new Public.SendSMSService();
                //await sendSms.SendSmsAsync(employee.phone_no, 6, toValues, content, 1, employee.org_id);
            }
            return true;
        }

        /// <summary>
        /// 开始
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> StartAsync(Execute entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            var result = Db.Ado.UseTran(() =>
            {
                //执行康复项目
                Db.Updateable<his_room_scheduling>().SetColumns(s => new his_room_scheduling { execute_date = DateTime.Now, stateid = 17, executor = userInfo.name, executor_id = userInfo.id, work_time_start = DateTime.Now.ToLongTimeString() }).Where(w => w.id == entity.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                //查询医疗室排班信息
                var room_scheduling = Db.Queryable<his_room_scheduling>().Where(w => w.id == entity.id).WithCache().First();

                //修改医疗室状态
                Db.Updateable<p_room>().SetColumns(s => new p_room { state = 31 }).Where(w => w.id == room_scheduling.room_id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
            });

            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }
    }
}
