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

namespace Service.Repository.Implements.His
{
    /// <summary>
    /// 康复预约业务
    /// </summary>
    public class RecoverRegisterService : DbContext, IRecoverRegisterService
    {
        /// <summary>
        /// 康复预约分页数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Page<RecoverRegisterPage>> GetRecoverPageAsync(RecoverRegisterSearch entity)
        {
            if (entity.store_id <= 0)
            {
                throw new MessageException("未获取到门店！");
            }
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            var name = "";
            if (!string.IsNullOrEmpty(entity.name))
            {
                name = entity.name.Trim();
            }

            var recoverList = await Db.Queryable<his_applybill, c_archives, his_applycontent>((ha, ca, content) => new object[] { JoinType.Left, ha.centerid == ca.id, JoinType.Left, ha.applyid == content.applyid })
                                    .Where((ha, ca, content) => ha.orgid == userInfo.org_id && ha.store_id == entity.store_id && ha.issettle == 2 && ha.stateid == 15 && ha.typeid != 6 && content.use_quantity < content.quantity)
                                    .WhereIF(!string.IsNullOrEmpty(name), (ha, ca, content) => ca.name.Contains(name) || ca.phone.Contains(name) || ca.to_director.Contains(name) || ha.typename.Contains(name))
                                    .WhereIF(entity.is_me, (ha, ca, content) => ca.to_director_id == userInfo.id)
                                    .GroupBy((ha, ca, content) => new { ha.applyid, ha.actualamount, ha.applydate, ca.name, ha.centerid, ha.clinicid, ha.deptname, ha.doctorname, ha.issettle, ca.phone, ha.shouldamount, ca.to_director, ha.typename })
                                    .Select((ha, ca, content) => new RecoverRegisterPage { applyid = ha.applyid, actualamount = ha.actualamount, applydate = ha.applydate, archives = ca.name, centerid = ha.centerid, clinicid = ha.clinicid, deptname = ha.deptname, doctorname = ha.doctorname, issettle = ha.issettle, phone = ca.phone, shouldamount = ha.shouldamount, to_director = ca.to_director, typename = ha.typename })
                                    .OrderBy(entity.order + orderTypeStr)
                                    .WithCache()
                                    .ToPageAsync(entity.page, entity.limit);

            //查询申请单关联项目
            var applyContentList = await Db.Queryable<his_applycontent, his_applybill>((ac, a) => new object[] { JoinType.Left, ac.applyid == a.applyid })
                                       .Where((ac, a) => a.orgid == userInfo.org_id && a.store_id == entity.store_id)
                                       .Select((ac, a) => new RecoverItemList { applyid = ac.applyid, item_id = ac.item_id.Value, item_name = ac.item_name, orderid = ac.orderid, quantity = ac.quantity, specid = ac.specid, specname = ac.specname, use_quantity = ac.use_quantity })
                                       .WithCache()
                                       .ToListAsync();

            var newList = recoverList.Items.Select(s => new RecoverRegisterPage { applyid = s.applyid, actualamount = s.actualamount, applydate = s.applydate, archives = s.archives, centerid = s.centerid, clinicid = s.clinicid, deptname = s.deptname, doctorname = s.doctorname, issettle = s.issettle, phone = s.phone, shouldamount = s.shouldamount, to_director = s.to_director, typename = s.typename, RecoverItem = applyContentList.Where(w => w.applyid == s.applyid && w.use_quantity < w.quantity).ToList() }).ToList();

            recoverList.Items = newList;
            return recoverList;



        }
        /// <summary>
        /// 获取是否可预约
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<RecordIfUse> GetIfOrder(RecoverIfOrder entity)
        {
            if (entity.regdate == null)
            {
                throw new MessageException("请选择康复预约时间！");
            }
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            //查询项目是否需要设备
            var IfEquipment = await Db.Queryable<h_item>()
                                   .Where(w => w.org_id == userInfo.org_id && w.item_id == entity.item_id && w.state_id == 1)
                                   .WithCache()
                                   .FirstAsync();
            if (IfEquipment == null)
            {
                throw new MessageException("未查询到项目！");
            }
            //查询设备是否可预约
            if (IfEquipment.equipment == 2)
            {
                //查询设备规格时间表
                var equipmentItem = await Db.Queryable<p_equipment_detials, p_equipment_itemspec, h_item>((ed, e, i) => new object[] { JoinType.Left, e.equipment_id == ed.id, JoinType.Left, e.itemid == i.item_id })
                                           .Where((ed, e, i) => e.itemid == entity.item_id && e.specid == entity.specid && i.org_id == userInfo.org_id)
                                           .Select((ed, e, i) => new { total_times = ed.work_times, e.work_times })
                                           .WithCache()
                                           .ToListAsync();
                if (equipmentItem == null || equipmentItem.Count <= 0)
                {
                    throw new MessageException("未查询到设备工作时间！");
                }
                //总时长
                var totalTime = equipmentItem.Sum(s => s.total_times);
                //查询预约记录表
                var recoverRecord = await Db.Queryable<his_recover, p_equipment_itemspec>((w, e) => new object[] { JoinType.Left, w.specid == e.specid && w.item_id == e.itemid })
                                          .Where((w, e) => w.item_id == entity.item_id && w.specid == entity.specid && w.regdate == entity.regdate && w.canceled == 3 && w.stateid == 5 && w.orgid == userInfo.org_id && w.store_id == entity.store_id)
                                          .Select((w, e) => new { e.work_times })
                                          .WithCache()
                                          .ToListAsync();
                var useTime = 0;
                if (recoverRecord.Count > 0)
                {
                    //已使用时长
                    useTime = recoverRecord.Sum(s => s.work_times.Value);
                }


                //该规格需要最小时长
                var mixTime = equipmentItem.Min(s => s.work_times);
                if (mixTime <= (totalTime - useTime - 30))
                {
                    return new RecordIfUse { state = true };
                }
                else
                {
                    return new RecordIfUse { state = false, specname = entity.specname };
                }


            }
            //查询房间是否可预约
            else if (IfEquipment.equipment == 3)
            {
                //查询房间规格时间表
                var roomItem = await Db.Queryable<p_room, p_room_itemspec>((r, ri) => new object[] { JoinType.Left, ri.room_id == r.id })
                                           .Where((r, ri) => ri.itemid == entity.item_id && ri.specid == entity.specid && r.org_id == userInfo.org_id && r.store_id == entity.store_id && r.state != 0)
                                           .Select((r, ri) => new { total_times = r.work_times, ri.work_times })
                                           .WithCache()
                                           .ToListAsync();
                if (roomItem == null || roomItem.Count <= 0)
                {
                    throw new MessageException("未查询到房间工作时间！");
                }
                //总时长
                var totalTime = roomItem.Sum(s => s.total_times);
                //查询预约记录表
                var recoverRecord = await Db.Queryable<his_recover, p_room_itemspec>((w, e) => new object[] { JoinType.Left, w.specid == e.specid && w.item_id == e.itemid })
                                          .Where((w, e) => w.item_id == entity.item_id && w.specid == entity.specid && w.regdate == entity.regdate && w.canceled == 3 && w.stateid == 5 && w.orgid == userInfo.org_id && w.store_id == entity.store_id)
                                          .Select((w, e) => new { e.work_times })
                                          .WithCache()
                                          .ToListAsync();
                var useTime = 0;
                if (recoverRecord.Count > 0)
                {
                    //已使用时长
                    useTime = recoverRecord.Sum(s => s.work_times.Value);
                }


                //该规格需要最小时长
                var mixTime = roomItem.Min(s => s.work_times);
                if (mixTime <= (totalTime - useTime - 30))
                {
                    return new RecordIfUse { state = true };
                }
                else
                {
                    return new RecordIfUse { state = false, specname = entity.specname };
                }
            }
            else
            {
                return new RecordIfUse { state = false };
            }

        }

        /// <summary>
        /// 添加康复预约记录
        /// </summary>
        /// <param name="entityCount"></param>
        /// <returns></returns>
        public async Task<bool> AddOrder(List<RecoreOrderAdd> entityCount)
        {
            if (entityCount.Count <= 0)
            {
                throw new MessageException("请传输正确信息！");
            }
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var recoverEntity = new his_recover();
            var archivesnotice = new c_archives();
            var reId = 0;
            var result = await Db.Ado.UseTranAsync(() =>
            {
                foreach (var entity in entityCount)
                {
                    //根据申请单id查询其他信息
                    var userNeed = Db.Queryable<his_applybill>()
                                         .Where(w => w.applyid == entity.applyid && w.orgid == userInfo.org_id && w.store_id == entity.store_id && w.centerid == entity.centerid)
                                         .WithCache()
                                         .First();

                    //预约信息
                    archivesnotice = Db.Queryable<c_archives>()
                                         .Where(a => a.id == entity.centerid)
                                         .WithCache()
                                         .First();

                    if (userNeed == null)
                    {
                        throw new MessageException("未获取到用户所需信息！");
                    }

                    //查询剩余次数
                    var applycontent = Db.Queryable<his_applycontent>()
                                             .Where(w => w.applyid == entity.applyid && w.specid == entity.specid && w.item_id == entity.item_id && w.orderid == entity.orderid && w.store_id == entity.store_id && w.org_id == userInfo.org_id)
                                             .WithCache()
                                             .First();
                    if (applycontent.use_quantity >= applycontent.quantity)
                    {
                        throw new MessageException("没有可用次数！");
                    }

                    recoverEntity.orgid = userInfo.org_id;
                    recoverEntity.deptid = userNeed.deptid;
                    recoverEntity.deptname = userNeed.deptname;
                    recoverEntity.doctorid = userNeed.doctorid;
                    recoverEntity.doctorname = userNeed.doctorname;
                    recoverEntity.typeid = userNeed.typeid;
                    recoverEntity.clinicid = userNeed.clinicid;
                    recoverEntity.recorddate = DateTime.Now;
                    recoverEntity.operatorid = userInfo.id;
                    recoverEntity.operator_name = userInfo.name;
                    recoverEntity.canceled = 3;
                    recoverEntity.stateid = 5;
                    recoverEntity.sourceid = 1;
                    recoverEntity.source = "窗口";
                    recoverEntity.specid = entity.specid;
                    recoverEntity.item_id = entity.item_id;
                    recoverEntity.applyid = entity.applyid;
                    recoverEntity.store_id = entity.store_id;
                    recoverEntity.typename = userNeed.typename;
                    recoverEntity.centerid = entity.centerid;
                    recoverEntity.regdate = entity.regdate;
                    recoverEntity.item_name = applycontent.item_name;
                    recoverEntity.specname = applycontent.specname;

                    //查询是否还可用
                    RecoverIfOrder orderEntity = new RecoverIfOrder();
                    orderEntity.specid = entity.specid;
                    orderEntity.item_id = entity.item_id;
                    orderEntity.regdate = entity.regdate;
                    orderEntity.store_id = entity.store_id;
                    orderEntity.specname = entity.specname;
                    var ifuse = GetIfOrder(orderEntity);
                    if (ifuse.Result.state == false)
                    {
                        throw new MessageException($"{ifuse.Result.specname}" + "，此规格当天已约满！");
                    }


                    //新增康复记录
                     reId= Db.Insertable(recoverEntity).ExecuteReturnIdentity();
                    redisCache.RemoveAll<his_recover>();

                    short num = Convert.ToInt16(applycontent.use_quantity + 1);

                    //修改次数
                    var applynum = Db.Updateable<his_applycontent>()
                      .SetColumns(w => new his_applycontent { use_quantity = num })
                                                              .Where(w => w.applyid == entity.applyid && w.specid == entity.specid && w.item_id == entity.item_id && w.orderid == entity.orderid && w.store_id == entity.store_id && w.org_id == userInfo.org_id)
                                                              .RemoveDataCache()
                                                              .EnableDiffLogEvent()
                                                              .ExecuteCommand();
                    if (applynum <= 0)
                    {
                        throw new MessageException("未预约成功！");
                    }
                }
            });

            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }

            if (archivesnotice.to_director_id>0)
            {
                //通知
                var notice = new Business.NoticeService();
                var noticeList = new List<AddNoticeModel>();
                var notice_content = "";
                var employeeSocket = new List<WebSocketModel>();
                             

                //康复预约通知
                notice_content = $"{{\"name\":\"{archivesnotice.name}\",\"item\":\" （{recoverEntity.item_name}-{recoverEntity.specname}）\",\"date\":\"{recoverEntity.regdate.ToString("yyyy-MM-dd")}\",\"msg\":\" 康复预约成功\"}}";

                employeeSocket.Add(new WebSocketModel { userId = archivesnotice.to_director_id.Value, content = notice_content });

                var employeenotice = new List<employeeMes>();
                employeenotice.Add(new employeeMes { employee_id =archivesnotice.to_director_id.Value, employee = archivesnotice.to_director });

                notice.NewMethod(reId.ToString(), archivesnotice, recoverEntity.store_id, notice, noticeList, 7, 5, notice_content, archivesnotice.name, employeenotice);

                //新增通知
                await notice.AddNoticeAsync(noticeList);
                //消息提醒
                ChatWebSocketMiddleware.SendListAsync(employeeSocket);

            }

            //发送短信
            var store_org = await Db.Queryable<p_store, p_org>((s, o) => new object[] { JoinType.Left, s.org_id == o.id }).Where((s, o) => s.id == recoverEntity.store_id).Select((s, o) => new { store_name = s.name, org_name = o.name }).WithCache().FirstAsync();
            var archives_employee = await Db.Queryable<c_archives, p_employee>((a, e) => new object[] { JoinType.Left, a.to_director_id == e.id }).Where((a, e) => a.id == recoverEntity.centerid).Select((a, e) => new { a, e }).WithCache().FirstAsync();
            var archives = archives_employee.a;
            var employee = archives_employee.e;
            var hospital = $"{store_org.store_name}（{store_org.org_name}）";
            var content = $"您在{hospital}预约康复成功（{recoverEntity.item_name}-{recoverEntity.specname}），预约日期：{recoverEntity.regdate.ToString("yyyy-MM-dd")}，请及时进行康复。";
            Dictionary<string, string> values = new Dictionary<string, string>();
            values.Add("hospital", hospital);
            values.Add("time", recoverEntity.regdate.ToString("yyyy-MM-dd"));
            var toValues = JsonConvert.SerializeObject(values);
            var sendSms = new Public.SendSMSService();
            //await sendSms.SendSmsAsync(archives.phone, 6, toValues, content, 1, archives.org_id);
            content = $"{archives.name}在{hospital}预约康复成功（{recoverEntity.item_name}-{recoverEntity.specname}），预约日期：{recoverEntity.regdate.ToString("yyyy-MM-dd")}，请及时进行康复。";
            //await sendSms.SendSmsAsync(employee.phone_no, 6, toValues, content, 1, archives.org_id);

            return result.IsSuccess;

        }

        /// <summary>
        /// 康复预约记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Page<RecoverRecordPage>> GetRecordPageAsync(RecoverRecordSearch entity)
        {
            if (entity.store_id <= 0)
            {
                throw new MessageException("未获取到门店！");
            }
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            var name = "";
            if (!string.IsNullOrEmpty(entity.name))
            {
                name = entity.name.Trim();
            }

            var staerTime = entity.startTime != null ? DateTime.Parse(entity.startTime + " 00:00:00") : DateTime.Now;
            var endTime = entity.endTime != null ? DateTime.Parse(entity.endTime + " 23:59:59") : DateTime.Now;

            return await Db.Queryable<his_recover, c_archives>((ha, ca) => new object[] { JoinType.Left, ha.centerid == ca.id })
                                    .Where((ha, ca) => ha.orgid == userInfo.org_id && ha.store_id == entity.store_id)
                                    .WhereIF(!string.IsNullOrEmpty(name), (ha, ca) => ca.name.Contains(name) || ca.phone.Contains(name) || ca.to_director.Contains(name) || ha.typename.Contains(name))
                                     .WhereIF(entity.startTime != null, (ha, ca) => ha.regdate >= staerTime)
                                     .WhereIF(entity.endTime != null, (ha, ca) => ha.regdate <= endTime)
                                     .WhereIF(entity.stateid > 0, (ha, ca) => ha.stateid == entity.stateid)
                                    .Select((ha, ca) => new RecoverRecordPage { applyid = ha.applyid, archives = ca.name, centerid = ha.centerid, clinicid = ha.clinicid, deptname = ha.deptname, doctorname = ha.doctorname, phone = ca.phone, to_director = ca.to_director, typename = ha.typename, canceled = ha.canceled, operator_name = ha.operator_name, recorddate = ha.recorddate, regdate = ha.regdate, recoverid = ha.recoverid, source = ha.source, summary = ha.summary, stateid = ha.stateid, specname = ha.specname, item_name = ha.item_name, item_id = ha.item_id, specid = ha.specid, store_id = ha.store_id, doctorid = ha.doctorid, deptid = ha.deptid, operatorid = ha.operatorid, orgid = ha.orgid, typeid = ha.typeid })
                                    .OrderBy(entity.order + orderTypeStr)
                                    .WithCache()
                                    .ToPageAsync(entity.page, entity.limit);
        }

        /// <summary>
        /// 康复预约改期
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> ModifyRecoverTime(ModifyModel entity)
        {
            if (entity == null)
            {
                throw new MessageException("请传入改期数据！");
            }
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var recoverContent = new his_recover();
            var newrecover = 0;
            var result = await Db.Ado.UseTranAsync(() =>
            {
                //查询康复记录
                recoverContent = Db.Queryable<his_recover>()
                                     .Where(s => s.recoverid == entity.recoverid)
                                     .WithCache()
                                     .First();

                if (recoverContent == null)
                {
                    throw new MessageException("未查询到康复信息！");
                }

                //查询是否还可用
                RecoverIfOrder orderEntity = new RecoverIfOrder();
                orderEntity.specid = recoverContent.specid;
                orderEntity.item_id = recoverContent.item_id;
                orderEntity.regdate = entity.recoverTime;
                orderEntity.store_id = recoverContent.store_id;
                orderEntity.specname = recoverContent.specname;
                var ifuse = GetIfOrder(orderEntity);
                if (ifuse.Result.state == false)
                {
                    throw new MessageException($"{ifuse.Result.specname}" + "，此规格当天已约满！");
                }

                //将之前预约状态改为已改期，并作废
                var isSus = Db.Updateable<his_recover>()
                            .SetColumns(s => new his_recover { canceled = 2, stateid = 21 })
                            .Where(s => s.recoverid == entity.recoverid)
                            .RemoveDataCache()
                            .EnableDiffLogEvent()
                            .ExecuteCommand();
                if (isSus <= 0)
                {
                    throw new MessageException("改期未成功！");
                }

                recoverContent.orgid = userInfo.org_id;
                recoverContent.recorddate = DateTime.Now;
                recoverContent.operatorid = userInfo.id;
                recoverContent.operator_name = userInfo.name;
                recoverContent.canceled = 3;
                recoverContent.stateid = 5;
                recoverContent.sourceid = 1;
                recoverContent.source = "窗口";
                recoverContent.regdate = entity.recoverTime;


                //添加新预约
                newrecover= Db.Insertable(recoverContent).ExecuteReturnIdentity();
                redisCache.RemoveAll<his_recover>();

            });

            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }

            //发送短信
            var store_org = await Db.Queryable<p_store, p_org>((s, o) => new object[] { JoinType.Left, s.org_id == o.id }).Where((s, o) => s.id == recoverContent.store_id).Select((s, o) => new { store_name = s.name, org_name = o.name }).WithCache().FirstAsync();
            var archives_employee = await Db.Queryable<c_archives, p_employee>((a, e) => new object[] { JoinType.Left, a.to_director_id == e.id }).Where((a, e) => a.id == recoverContent.centerid).Select((a, e) => new { a, e }).WithCache().FirstAsync();
            var archives = archives_employee.a;
            var employee = archives_employee.e;
            var hospital = $"{store_org.store_name}（{store_org.org_name}）";
            var content = $"您在{hospital}预约康复改期成功（{recoverContent.item_name}-{recoverContent.specname}），预约日期：{recoverContent.regdate.ToString("yyyy-MM-dd")}，请及时进行康复。";
            Dictionary<string, string> values = new Dictionary<string, string>();
            values.Add("hospital", hospital);
            values.Add("time", recoverContent.regdate.ToString("yyyy-MM-dd"));
            var toValues = JsonConvert.SerializeObject(values);
            var sendSms = new Public.SendSMSService();
            //await sendSms.SendSmsAsync(archives.phone, 6, toValues, content, 1, archives.org_id);
            content = $"{archives.name}在{hospital}预约康复改期成功（{recoverContent.item_name}-{recoverContent.specname}），预约日期：{recoverContent.regdate.ToString("yyyy-MM-dd")}，请及时进行康复。";
            //await sendSms.SendSmsAsync(employee.phone_no, 6, toValues, content, 1, archives.org_id);

            if (archives.to_director_id>0)
            {
                //通知
                var notice = new Business.NoticeService();
                var noticeList = new List<AddNoticeModel>();
                var notice_content = "";
                var employeeSocket = new List<WebSocketModel>();

                //康复预约改期通知
                notice_content = $"{{\"name\":\"{archives.name}\",\"item\":\" （{recoverContent.item_name}-{recoverContent.specname}）\",\"date\":\"{recoverContent.regdate.ToString("yyyy-MM-dd")}\",\"msg\":\" 康复预约改期\"}}";

                employeeSocket.Add(new WebSocketModel { userId = archives.to_director_id.Value, content = notice_content });
                var employeenotice = new List<employeeMes>();
                employeenotice.Add(new employeeMes { employee_id = archives.to_director_id.Value, employee = archives.to_director });

                notice.NewMethod(newrecover.ToString(), archives, recoverContent.store_id, notice, noticeList, 7, 3, notice_content, archives.name, employeenotice);

                //新增通知
                await notice.AddNoticeAsync(noticeList);
                //消息提醒
                ChatWebSocketMiddleware.SendListAsync(employeeSocket);

            }

            return result.IsSuccess;
        }

        /// <summary>
        /// 取消预约
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> CancelRecoverTime(CancelModel entity)
        {
            if (entity.recoverid <= 0)
            {
                throw new MessageException("请选择需要取消的预约！");
            }

            //查询康复数据
            var recoverContent = await Db.Queryable<his_recover>()
                                       .Where(w => w.recoverid == entity.recoverid)
                                       .WithCache()
                                       .FirstAsync();

            if (recoverContent == null)
            {
                throw new MessageException("未获取到此预约！");
            }

            var result = await Db.Ado.UseTranAsync(() =>
            {
                //修改当前康复预约状态
                var isSus = Db.Updateable<his_recover>()
                   .SetColumns(s => new his_recover { canceled = 2, stateid = 7 })
                   .Where(s => s.recoverid == entity.recoverid)
                   .RemoveDataCache()
                   .EnableDiffLogEvent()
                   .ExecuteCommand();

                if (isSus <= 0)
                {
                    throw new MessageException("取消未成功！");
                }

                //查询申请单内容
                var applyContent = Db.Queryable<his_applycontent>()
                                   .Where(w => w.applyid == recoverContent.applyid && w.specid == recoverContent.specid && w.item_id == recoverContent.item_id && w.store_id == recoverContent.store_id && w.org_id == recoverContent.orgid)
                                   .WithCache()
                                   .First();
                if (applyContent == null)
                {
                    throw new MessageException("未获取到申请单！");
                }

                short num = Convert.ToInt16(applyContent.use_quantity - 1);

                var isModify = Db.Updateable(applyContent)
                               .SetColumns(s => new his_applycontent { use_quantity = num })
                               .Where(s => s.applyid == recoverContent.applyid && s.specid == recoverContent.specid && s.item_id == recoverContent.item_id)
                               .RemoveDataCache()
                               .EnableDiffLogEvent()
                               .ExecuteCommand();
                if (isModify <= 0)
                {
                    throw new MessageException("数量扣减失败！");
                }
                var archives = Db.Queryable<c_archives>().Where(a=> a.id == recoverContent.centerid).WithCache().First();

                if (archives.to_director_id > 0)
                {
                    //通知
                    var notice = new Business.NoticeService();
                    var noticeList = new List<AddNoticeModel>();
                    var notice_content = "";
                    var employeeSocket = new List<WebSocketModel>();

                    //预约取消通知
                    notice_content = $"{{\"name\":\"{archives.name}\",\"item\":\"（{recoverContent.item_name}-{recoverContent.specname}）\",\"date\":\"{recoverContent.regdate.ToString("yyyy-MM-dd")}\",\"msg\":\" 康复预约取消\"}}";

                    employeeSocket.Add(new WebSocketModel { userId = archives.to_director_id.Value, content = notice_content });
                    var employeenotice = new List<employeeMes>();
                    employeenotice.Add(new employeeMes { employee_id = archives.to_director_id.Value, employee = archives.to_director });

                    notice.NewMethod(entity.recoverid.ToString(), archives, recoverContent.store_id, notice, noticeList, 7, 4, notice_content, archives.name, employeenotice);

                    //新增通知
                     notice.AddNotice(noticeList);
                    //消息提醒
                    ChatWebSocketMiddleware.SendListAsync(employeeSocket);

                }

            });

            return result.IsSuccess;

        }

        /// <summary>
        /// 获取康复记录（客户端）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Page<RecoverRecordPC>> GetRecordPCAsync(RecoverPCSearch entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetArchives().archives;
            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }

            return await Db.Queryable<his_recover, p_store, b_basecode>((r, s, b) => new object[] { JoinType.Left, r.store_id == s.id, JoinType.Left, r.stateid == b.valueid })
                                    .Where((r, s, b) => r.centerid == userInfo.id && r.orgid == userInfo.org_id && b.baseid == 0)
                                    .Select((r, s, b) => new RecoverRecordPC { recoverid = r.recoverid, item_name = r.item_name, operator_name = r.operator_name, recorddate = r.recorddate, regdate = r.regdate, strore_name = s.name, state_name = b.value })
                                    .OrderBy(entity.order + orderTypeStr)
                                    .WithCache()
                                    .ToPageAsync(entity.page, entity.limit);
        }

        /// <summary>
        /// 获取康复列表
        /// </summary>
        /// <param name="store_id"></param>
        /// <param name="record_type">0进行中 1已完成</param>
        /// <returns></returns>
        public async Task<List<RecoverList>> GetRecoverList(int store_id, int record_type)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetArchives().archives;

            //已完成康复/进行中
            if (record_type == 1 || record_type == 0)
            {
                return await Db.Queryable<his_applybill, his_clinicrecord>((ab, a) => new object[] { JoinType.Left, a.clinicid == ab.clinicid })
                           .Where((ab, a) => a.orgid == userInfo.org_id && ab.store_id == store_id && ab.centerid == userInfo.id)
                           .WhereIF(record_type == 1, (ab, a) => a.stateid == 15)
                           .WhereIF(record_type == 0, (ab, a) => a.stateid == 13)
                           .Select((ab, a) => new RecoverList { dept = ab.deptname, doctor = ab.doctorname, applyid = ab.applyid, regdate = ab.applydate })
                           .WithCache()
                           .ToListAsync();
            }
            else
            {
                return null;
            }


        }

        /// <summary>
        /// 获取客户端详情
        /// </summary>
        /// <param name="applyid"></param>
        /// <param name="record_type">0进行中 1已完成</param>
        /// <returns></returns>
        public async Task<RecoverDetail> GetRecoverListDetail(int applyid, int record_type)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetArchives().archives;

            //查询康复
            var applyDetail = await Db.Queryable<his_applybill, his_clinicrecord, c_archives, p_employee>((ab, c, a, e) => new object[] { JoinType.Left, c.clinicid == ab.clinicid, JoinType.Left, ab.centerid == a.id, JoinType.Left, a.to_director_id == e.id })
                                          .Where((ab, c, a, e) => ab.applyid == applyid && ab.centerid == userInfo.id && ab.orgid == userInfo.org_id)
                                          .WhereIF(record_type == 1, (ab, c, a, e) => c.stateid == 15)
                                          .WhereIF(record_type == 0, (ab, c, a, e) => c.stateid == 13)
                                          .Select((ab, c, a, e) => new { ab.applyid, ab.applydate, ab.deptname, ab.doctorname, a.to_director, e.phone_no, ab.clinicid, c.enddate })
                                          .WithCache()
                                          .FirstAsync();

            if (applyDetail == null)
            {
                throw new MessageException("未查询到开方！");
            }

            //查询康复详情
            var applyContentDetail = await Db.Queryable<his_applycontent>()
                                                .Where(a => a.applyid == applyDetail.applyid && a.org_id == userInfo.org_id)
                                                .Select(a => new RecoverDetailList { item_name = a.item_name, use_count = a.use_quantity, total_count = a.quantity })
                                                .WithCache()
                                                .ToListAsync();


            if (record_type == 1)
            {
                return new RecoverDetail { dept = applyDetail.deptname, doctor = applyDetail.doctorname, phone = applyDetail.phone_no, applyid = applyDetail.applyid, regdate = applyDetail.applydate, to_director = applyDetail.to_director, recoverList = applyContentDetail, last_time = applyDetail.enddate.Value };
            }
            else if (record_type == 0)
            {
                return new RecoverDetail { dept = applyDetail.deptname, doctor = applyDetail.doctorname, applyid = applyDetail.applyid, regdate = applyDetail.applydate, recoverList = applyContentDetail };
            }
            else
            {
                return null;
            }




        }
    }
}
