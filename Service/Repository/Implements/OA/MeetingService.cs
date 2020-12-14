using Models.DB;
using Models.View.Business;
using Models.View.OA;
using Newtonsoft.Json;
using Service.Extensions;
using Service.Repository.Interfaces.OA;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;
using Tools.Filter;
using Tools.WebSocket;

namespace Service.Repository.Implements.OA
{
    /// <summary>
    /// 会议
    /// </summary>
    public class MeetingService : DbContext, IMeetingService
    {
        /// <summary>
        /// 添加会议
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> AddMeetingAsync(Meeting entity)
        {
            //通知
            var notice = new Business.NoticeService();
            var noticeList = new List<AddNoticeModel>();
            var notice_content = "";
            var meeting_id = 0;
            var employeenotice = new List<employeeMes>();

            //查询用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var employee_ids = new List<int>();
            var oa_meeting = new oa_meeting();
            var result = await Db.Ado.UseTranAsync(() =>
            {
                //添加会议基础信息
                oa_meeting = new oa_meeting { address = entity.address, content = entity.content, convenor = entity.convenor, convenor_id = entity.convenor_id, creater = userInfo.name, creater_id = userInfo.id, create_time = DateTime.Now, data = entity.data, dept_id = entity.dept_id, dept_name = entity.dept_name, host = entity.host, host_id = entity.host_id, issue = entity.issue, no = $"HY{DateTime.Now.ToString("yyyyMMddHHmmssffffff")}", org_id = userInfo.org_id, remark = entity.remark, start_date = entity.start_date, start_time = entity.start_time, state = 37, store = entity.store, store_id = entity.store_id, theme = entity.theme, times = entity.times, type = entity.type, type_id = entity.type_id, note_taker = entity.note_taker, note_taker_id = entity.note_taker_id };
                meeting_id = Db.Insertable(oa_meeting).ExecuteReturnIdentity();
                redisCache.RemoveAll<oa_meeting>();

                //添加参加会议人员
                var employees = entity.meeting_Employees.Select(s => new oa_meeting_employee { employee = s.employee, employee_id = s.employee_id, id = meeting_id }).ToList();
                employee_ids = employees.Select(s => s.employee_id).ToList();
                //添加召集人
                if (!employee_ids.Contains(entity.convenor_id))
                {
                    employee_ids.Add(entity.convenor_id);
                    employees.Add(new oa_meeting_employee { employee = entity.convenor, employee_id = entity.convenor_id, id = meeting_id });
                }
                //添加创建人
                if (!employee_ids.Contains(userInfo.id))
                {
                    employee_ids.Add(userInfo.id);
                    employees.Add(new oa_meeting_employee { employee = userInfo.name, employee_id = userInfo.id, id = meeting_id });
                }
                //添加主持人
                if (!employee_ids.Contains(entity.host_id))
                {
                    employee_ids.Add(entity.host_id);
                    employees.Add(new oa_meeting_employee { employee = entity.host, employee_id = entity.host_id, id = meeting_id });
                }
                //添加记录人
                if (!employee_ids.Contains(entity.note_taker_id))
                {
                    employee_ids.Add(entity.note_taker_id);
                    employees.Add(new oa_meeting_employee { employee = entity.note_taker, employee_id = entity.note_taker_id, id = meeting_id });
                }
                Db.Insertable(employees).ExecuteCommand();
                redisCache.RemoveAll<oa_meeting_employee>();

                //添加附件
                if (entity.meeting_Accessories.Count > 0)
                {
                    var accessories = entity.meeting_Accessories.Select(s => new oa_meeting_accessories { id = meeting_id, name = s.name, url = s.url }).ToList();
                    Db.Insertable(accessories).ExecuteCommand();
                    redisCache.RemoveAll<oa_meeting_accessories>();
                }
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            var employeeSocket = new List<WebSocketModel>();
            #region 发送通知
            employee_ids.ForEach(id =>
            {
                var con = $"{{\"theme\":\"{oa_meeting.theme}\",\"start_date\":\"{oa_meeting.start_date}\",\"start_time\":\"{oa_meeting.start_time}\",\"no\":\"{oa_meeting.no}\",\"msg\":\"请准时参加\",\"type\":\"meeting\"}}";
                employeeSocket.Add(new WebSocketModel { userId = id, content = con });


                //查询用户名
                var employee = Db.Queryable<p_employee>()
                               .Where(e => e.id == id)
                               .WithCache()
                               .First();

                 employeenotice.Add(new employeeMes { employee_id = id, employee = employee.name  }); 
               
            });

            if (employee_ids.Count>0)
            {
                //调用通知
                notice_content = $"{{\"name\":\"{oa_meeting.theme}\",\"date\":\"{oa_meeting.start_date}\",\"time\":\"{oa_meeting.start_time}\",\"msg\":\"会议信息\",\"times\":\"{oa_meeting.times}\",\"content\":\"{oa_meeting.content}\",\"address\":\"{oa_meeting.address}\"}}";
                var archives = new c_archives();
                notice.NewMethod(meeting_id.ToString(), archives, entity.store_id.Value, notice, noticeList, 1, 1, notice_content, oa_meeting.theme, employeenotice);

                //新增
                notice.AddNotice(noticeList);
                //消息提醒
                ChatWebSocketMiddleware.SendListAsync(employeeSocket);

            }

            #endregion
            #region 发送短信
            //var employees = await Db.Queryable<p_employee>().Where(w => employee_ids.Contains(w.id)).ToListAsync();
            //Dictionary<string, string> values = new Dictionary<string, string>();
            //values.Add("archives", archives.name);
            //var toValues = JsonConvert.SerializeObject(values);
            //var sendSms = new Public.SendSMSService();
            //await sendSms.SendSmsAsync(employee.phone_no, 6, toValues, content, 1, archives.org_id);
            #endregion
            return result.IsSuccess;
        }

        /// <summary>
        /// 取消会议
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> CancelMeetingAsync(oa_meeting entity)
        {
            //通知
            var notice = new Business.NoticeService();
            var noticeList = new List<AddNoticeModel>();
            var notice_content = "";
            var employeenotice = new List<employeeMes>();

            entity = await Db.Queryable<oa_meeting>().Where(w => w.id == entity.id).WithCache().FirstAsync();
            var employees = await Db.Queryable<oa_meeting_employee>().Where(w => w.id == entity.id).WithCache().ToListAsync();
            var employeeSocket = new List<WebSocketModel>();
            #region 发送通知
            employees.ForEach(item =>
            {
                var con = $"{{\"theme\":\"{entity.theme}\",\"start_date\":\"{entity.start_date}\",\"start_time\":\"{entity.start_time}\",\"no\":\"{entity.no}\",\"msg\":\"已取消\",\"type\":\"meeting\"}}";
                employeeSocket.Add(new WebSocketModel { userId = item.employee_id, content = con });

                employeenotice.Add(new employeeMes { employee_id = item.employee_id, employee = item.employee });

              });
            if (employees.Count > 0)
            {
                //调用通知
                notice_content = $"{{\"name\":\"{entity.theme}\",\"date\":\"{entity.start_date}\",\"time\":\"{entity.start_time}\",\"msg\":\"会议已取消\",\"times\":\"{entity.times.ToString("f1")}\",\"content\":\"{entity.content}\",\"address\":\"{entity.address}\"}}";
                var archives = new c_archives();
                notice.NewMethod(entity.id.ToString(), archives, entity.store_id.Value, notice, noticeList, 1, 3, notice_content, entity.theme, employeenotice);

                //新增
                notice.AddNotice(noticeList);
                //消息提醒
                ChatWebSocketMiddleware.SendListAsync(employeeSocket);

            }
            #endregion
            return await Db.Updateable<oa_meeting>().SetColumns(s => new oa_meeting { state = 7 }).Where(w => w.id == entity.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
        }

        /// <summary>
        /// 延期会议
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> DelayMeetingAsync(oa_meeting entity)
        {
            //通知
            var notice = new Business.NoticeService();
            var noticeList = new List<AddNoticeModel>();
            var notice_content = "";
            var employeenotice = new List<employeeMes>();

            var result = await Db.Updateable<oa_meeting>().SetColumns(s => new oa_meeting { state = 38, start_time = entity.start_time, start_date = entity.start_date }).Where(w => w.id == entity.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
            entity = await Db.Queryable<oa_meeting>().Where(w => w.id == entity.id).WithCache().FirstAsync();
            var employees = await Db.Queryable<oa_meeting_employee>().Where(w => w.id == entity.id).WithCache().ToListAsync();
            var employeeSocket = new List<WebSocketModel>();
            #region 发送通知
            employees.ForEach(item =>
            {
                var con = $"{{\"theme\":\"{entity.theme}\",\"start_date\":\"{entity.start_date}\",\"start_time\":\"{entity.start_time}\",\"no\":\"{entity.no}\",\"msg\":\"已改期\",\"type\":\"meeting\"}}";
                employeeSocket.Add(new WebSocketModel { userId = item.employee_id, content = con });

                employeenotice.Add(new employeeMes { employee_id = item.employee_id, employee = item.employee });

            });
            if (employees.Count > 0)
            {
                //调用通知
                notice_content = $"{{\"name\":\"{entity.theme}\",\"date\":\"{entity.start_date}\",\"time\":\"{entity.start_time}\",\"msg\":\"会议已改期\",\"times\":\"{entity.times.ToString("f1")}\",\"content\":\"{entity.content}\",\"address\":\"{entity.address}\"}}";
                var archives = new c_archives();
                notice.NewMethod(entity.id.ToString(), archives, entity.store_id.Value, notice, noticeList, 1, 2, notice_content, entity.theme, employeenotice);

                //新增
                notice.AddNotice(noticeList);
                //消息提醒
                ChatWebSocketMiddleware.SendListAsync(employeeSocket);


            }
            #endregion
            return result;
        }

        /// <summary>
        /// 获得分页列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Page<Meeting>> GetPageAsync(MeetingPageSearch entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }

            var list = await Db.Queryable<oa_meeting_employee, oa_meeting>((ome, om) => new object[] { JoinType.Left, ome.id == om.id })
                            .Where((ome, om) => om.org_id == userInfo.org_id && om.store_id == entity.store_id)
                            .WhereIF(!string.IsNullOrEmpty(entity.no), (ome, om) => om.no.Contains(entity.no))
                            .WhereIF(entity.dept_id > 0, (ome, om) => om.dept_id == entity.dept_id)
                            .WhereIF(entity.type_id > 0, (ome, om) => om.type_id == entity.type_id)
                            .WhereIF(entity.is_me, (ome, om) => om.convenor_id == userInfo.id || om.creater_id == userInfo.id || om.host_id == userInfo.id || om.note_taker_id == userInfo.id || ome.employee_id == userInfo.id)
                            .GroupBy((ome, om) => new { om.address, om.content, om.convenor, om.convenor_id, om.creater, om.creater_id, om.create_time, om.data, om.dept_id, om.dept_name, om.host, om.start_date, om.host_id, om.id, om.issue, om.no, om.org_id, om.start_time, om.state, om.store, om.store_id, om.theme, om.times, om.type, om.type_id, om.note_taker, om.note_taker_id, om.remark })
                            .Select((ome, om) => new Meeting { address = om.address, content = om.content, convenor = om.convenor, convenor_id = om.convenor_id, creater = om.creater, creater_id = om.creater_id, create_time = om.create_time, data = om.data, dept_id = om.dept_id, dept_name = om.dept_name, host = om.host, start_date = om.start_date, host_id = om.host_id, id = om.id, issue = om.issue, no = om.no, org_id = om.org_id, start_time = om.start_time, state = om.state, store = om.store, store_id = om.store_id, theme = om.theme, times = om.times, type = om.type, type_id = om.type_id, note_taker = om.note_taker, note_taker_id = om.note_taker_id, remark = om.remark })
                            .OrderBy(entity.order + orderTypeStr)
                            .WithCache()
                            .ToPageAsync(entity.page, entity.limit);

            var ids = list.Items.Select(s => s.id).ToList();
            // 参会人员
            var meeting_Employees = await Db.Queryable<oa_meeting_employee>().Where(w => ids.Contains(w.id)).WithCache().ToListAsync();
            // 会议附件
            var meeting_Accessories = await Db.Queryable<oa_meeting_accessories>().Where(w => ids.Contains(w.id)).WithCache().ToListAsync();
            // 会议发言
            var meeting_Speaks = await Db.Queryable<oa_meeting_speak>().Where(w => ids.Contains(w.id)).WithCache().ToListAsync();
            // 会议决议
            var meeting_Resolutions = await Db.Queryable<oa_meeting_resolution>().Where(w => ids.Contains(w.id)).WithCache().ToListAsync();
            list.Items = list.Items.Select(s => new Meeting { address = s.address, content = s.content, convenor = s.convenor, convenor_id = s.convenor_id, creater = s.creater, creater_id = s.creater_id, create_time = s.create_time, data = s.data, dept_id = s.dept_id, dept_name = s.dept_name, host = s.host, start_date = s.start_date, host_id = s.host_id, id = s.id, issue = s.issue, no = s.no, org_id = s.org_id, start_time = s.start_time, state = s.state, store = s.store, store_id = s.store_id, theme = s.theme, times = s.times, type = s.type, type_id = s.type_id, note_taker = s.note_taker, note_taker_id = s.note_taker_id, remark = s.remark, meeting_Accessories = meeting_Accessories.Where(w => w.id == s.id).ToList(), meeting_Employees = meeting_Employees.Where(w => w.id == s.id).ToList(), meeting_Speaks = meeting_Speaks.Where(w => w.id == s.id).ToList(), meeting_Resolutions = meeting_Resolutions.Where(w => w.id == s.id).ToList() }).ToList();
            return list;
        }

        /// <summary>
        /// 获取会议记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<string> GetRecordAsync(int id)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var meeting_employee = await Db.Queryable<oa_meeting_employee>().Where(w => w.id == id && w.employee_id == userInfo.id).FirstAsync();
            return meeting_employee.record;
        }

        /// <summary>
        /// 修改会议记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> ModifyRecordAsync(oa_meeting_employee entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            entity.employee = userInfo.name;
            entity.employee_id = userInfo.id;

            var result = await Db.Ado.UseTranAsync(() =>
            {
                var ret = -1;
                //删除已有会议记录
                ret = Db.Deleteable<oa_meeting_employee>().Where(w => w.id == entity.id && w.employee_id == userInfo.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                if (ret != -1)
                {
                    //添加会议记录
                    Db.Insertable(entity).ExecuteCommand();
                    redisCache.RemoveAll<oa_meeting_employee>();
                }
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 保存决议
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> SaveResolutionAsync(ResolutionList entity)
        {
            var result = await Db.Ado.UseTranAsync(() =>
            {
                var ret = -1;
                //删除已有决议
                ret = Db.Deleteable<oa_meeting_resolution>().Where(w => w.id == entity.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                if (ret != -1)
                {
                    //获取需要添加的决议
                    var addlst = entity.resolutions.Select((s, index) => new oa_meeting_resolution { id = entity.id, matter = s.matter, target = s.target, no = "JY" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + index }).ToList();
                    //添加决议
                    Db.Insertable(addlst).ExecuteCommand();
                    redisCache.RemoveAll<oa_meeting_resolution>();
                }
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 保存发言
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> SaveSpeakAsync(SpeakList entity)
        {
            var result = await Db.Ado.UseTranAsync(() =>
            {
                var ret = -1;
                //删除已有发言
                ret = Db.Deleteable<oa_meeting_speak>().Where(w => w.id == entity.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                if (ret != -1)
                {
                    //获取需要添加的发言
                    var addlst = entity.speaks.Select(s => new oa_meeting_speak { id = entity.id, point = s.point, spokesman = s.spokesman, spokesman_id = s.spokesman_id }).ToList();
                    //添加发言
                    Db.Insertable(addlst).ExecuteCommand();
                    redisCache.RemoveAll<oa_meeting_speak>();
                }
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }
    }
}
