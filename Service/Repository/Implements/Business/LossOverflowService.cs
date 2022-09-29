using Models.DB;
using Models.View.Business;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Service.Extensions;
using Service.Repository.Interfaces.Business;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;
using Tools.Filter;
using Tools.WebSocket;

namespace Service.Repository.Implements.Business
{
    /// <summary>
    /// 报损报溢
    /// </summary>
    public class LossOverflowService : DbContext, ILossOverflowService
    {
        /// <summary>
        /// 新增报损报溢
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> AddAsync(LossOverflow entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            var result = Db.Ado.UseTran(() =>
            {
                //查询盘点单
                var stocktaking = Db.Queryable<bus_stocktaking>().Where(w => w.no == entity.realted_no).WithCache().First();
                if (stocktaking.state != 24 && stocktaking.state != 33)
                {
                    throw new MessageException("盘点单不为待处理状态，请确认是否已结束盘点");
                }

                //报损报溢单
                var loss_overflow = new bus_loss_overflow();
                loss_overflow.creater = userInfo.name;
                loss_overflow.creater_id = userInfo.id;
                loss_overflow.create_time = DateTime.Now;
                loss_overflow.org_id = userInfo.org_id;
                loss_overflow.remark = entity.remark;
                loss_overflow.store_id = entity.store_id;
                loss_overflow.store_name = entity.store_name;
                loss_overflow.state = 26;
                loss_overflow.realted_no = entity.realted_no;
                loss_overflow.type_id = entity.type_id;
                loss_overflow.std_item_id = entity.std_item_id;
                loss_overflow.name = entity.name;
                loss_overflow.item_type_id = entity.item_type_id;
                loss_overflow.item_type = entity.item_type;
                loss_overflow.spec = entity.spec;
                loss_overflow.manufactor = entity.manufactor;
                loss_overflow.manufactor_id = entity.manufactor_id;
                loss_overflow.unit = entity.unit;
                loss_overflow.num = entity.num;
                loss_overflow.property_id = entity.property_id;
                loss_overflow.no = (entity.type_id == 1 ? "BY" : "BS") + userInfo.org_id + DateTime.Now.ToString("yyMMdd");
                //查询最大单号
                var max_no = Db.Queryable<bus_loss_overflow>().Where(w => w.no.StartsWith(loss_overflow.no) && w.org_id == userInfo.org_id).OrderBy(o => o.no, OrderByType.Desc).Select(s => s.no).WithCache().First();
                if (max_no == null)
                {
                    loss_overflow.no += "0001";
                }
                else
                {
                    max_no = max_no.Substring(loss_overflow.no.Length);
                    loss_overflow.no += (int.Parse(max_no) + 1).ToString().PadLeft(4, '0');
                }

                //报损报溢流程
                loss_overflow.process_id = 0;
                SetProcess(loss_overflow);

                Db.Insertable(loss_overflow).ExecuteCommand();
                redisCache.RemoveAll<bus_loss_overflow>();

                //查询盘点明细
                var stocktaking_detials = Db.Queryable<bus_stocktaking_detials>().Where(w => w.no == loss_overflow.realted_no && w.std_item_id == loss_overflow.std_item_id && w.spec == loss_overflow.spec && w.manufactor_id == loss_overflow.manufactor_id).WithCache().First();
                if (loss_overflow.type_id == 1)//报溢
                {
                    stocktaking_detials.not_report_num -= loss_overflow.num;
                    stocktaking_detials.report_num += loss_overflow.num;
                }
                else//报损
                {
                    stocktaking_detials.not_report_num += loss_overflow.num;
                    stocktaking_detials.report_num -= loss_overflow.num;
                }

                //修改盘点待报已报数量
                Db.Updateable<bus_stocktaking_detials>().SetColumns(s => new bus_stocktaking_detials { not_report_num = stocktaking_detials.not_report_num, report_num = stocktaking_detials.report_num }).Where(w => w.no == loss_overflow.realted_no && w.std_item_id == loss_overflow.std_item_id && w.spec == loss_overflow.spec && w.manufactor_id == loss_overflow.manufactor_id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                //报损报溢明细
                if (entity.loss_Overflow_Detials.Count > 0)
                {
                    var loss_Overflow_Detials = entity.loss_Overflow_Detials.Select(s => new bus_loss_overflow_detials { assets_no = s.assets_no, no = loss_overflow.no, remark = s.remark }).ToList();
                    Db.Insertable(loss_Overflow_Detials).ExecuteCommand();
                    redisCache.RemoveAll<bus_loss_overflow_detials>();

                    //修改固定资产状态为处理中
                    var nos = loss_Overflow_Detials.Select(s => s.assets_no).ToList();
                    Db.Updateable<bus_assets>().SetColumns(s => new bus_assets { state = 33 }).Where(w => nos.Contains(w.no)).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                }

                //查询是否报损报溢完
                var isExiste = Db.Queryable<bus_stocktaking_detials>().WithCache().Any(a => a.no == loss_overflow.realted_no && a.not_report_num != 0);
                short state = 33;
                if (!isExiste)
                {
                    state = 15;
                }
                //修改盘点单状态
                Db.Updateable<bus_stocktaking>().SetColumns(s => new bus_stocktaking { state = state }).Where(w => w.no == entity.realted_no).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        //设置审核信息
        private void SetProcess(bus_loss_overflow entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            //查询调拨单需要进行的审核流程
            var store = 0;
            if (entity.store_id != 0)
            {
                store = 99;
            }
            var process = Db.Queryable<p_process>().Where(w => w.type_id == 6 && w.org_id == userInfo.org_id && w.dept_id == 0 && w.state == 1 && w.store_id == store).OrderBy(o => o.use_money, OrderByType.Desc).WithCache().First();
            if (process == null)
            {
                throw new MessageException("请先设置固定资产报损报溢审核流程");
            }
            entity.total_level = process.total_level;
            entity.level = 0;
            entity.process_id = process.id;

            entity.await_verifier_id = 0;
            entity.is_org = false;

            //审核流程明细
            var process_detials = Db.Queryable<p_process_detials>().Where(w => w.id == process.id).OrderBy(o => o.level).WithCache().ToList();
            //循环流程明细
            foreach (var item in process_detials)
            {
                var isExiste = false;
                if (!item.is_org.Value)
                {
                    isExiste = Db.Queryable<p_role>().Any(w => w.link_id == item.role_id && w.store_id == entity.store_id);
                }
                else
                {
                    isExiste = Db.Queryable<p_role>().Any(w => w.id == item.role_id && w.store_id == 0);
                }
                if (isExiste)
                {
                    entity.level = item.level;
                    entity.await_verifier_id = item.role_id;
                    entity.await_verifier = item.role_name;
                    entity.is_org = item.is_org;
                    break;
                }
                else
                {
                    //审核记录
                    CommonModel model = new CommonModel();
                    model.total_level = process.total_level;
                    model.level = item.level;
                    model.store_id = entity.store_id;
                    model.store = entity.store_name;
                    model.applicant_id = userInfo.id;
                    model.applicant = userInfo.name;
                    model.process_type_id = 6;
                    model.process_type = "报损报溢";

                    ApprovalLeaveModel entityA = new ApprovalLeaveModel();
                    entityA.apply_no = entity.no;
                    entityA.approval_state = 34;
                    entityA.verify_remark = " ";
                    var ver = new ApprovalService();

                    var process_currentMes = Db.Queryable<p_process_detials, p_process>((p, pr) => new object[] { JoinType.Left, p.id == pr.id })
                                .Where((p, pr) => p.id == process.id && p.level == item.level)
                                .Select((p, pr) => new process_currentMesModel { dept_id = p.dept_id, role_id = p.role_id, employee_id = p.employee_id.Value, dept_name = p.dept_name, role_name = p.role_name, id = p.id, name = pr.name, leave_type_id = pr.leave_type_id, leave_type = pr.leave_type })
                                .WithCache()
                                .First();
                    if (process_currentMes == null)
                    {
                        throw new MessageException("未获取到当前审核流程信息");
                    }
                    var nextp = process_detials.Where(dd => dd.level == item.level + 1).FirstOrDefault();
                    entity.state = 36;
                    if (nextp == null)
                    {
                        entity.state = 34;
                    }
                    //添加审核记录                               
                    ver.SetVerify(entityA, process_currentMes, userInfo, item.level, model, 2, nextp);

                }
            }

            //通知
            var notice = new Business.NoticeService();
            var noticeList = new List<AddNoticeModel>();
            var notice_content = "";
            var employeeSocket = new List<WebSocketModel>();

            //查询此角色下面的人员    
            var rolenotice = Db.Queryable<p_employee_role, p_role, p_employee>((a, b, e) => new object[] { JoinType.Left, a.role_id == b.id, JoinType.Left, e.id == a.employee_id })
                                   .Where((a, b, e) => b.disabled_code == 1)
                                   .WhereIF(entity.is_org == false, (a, b, e) => b.link_id == entity.await_verifier_id && a.store_id > 0)
                                   .WhereIF(entity.is_org == true, (a, b, e) => b.id == entity.await_verifier_id && a.store_id == 0)
                                   .Select((a, b, e) => new { e.id, e.name })
                                   .WithCache()
                                   .ToList();

            if (rolenotice.Count > 0)
            {
                //报损报溢通知
                notice_content = $"{{\"name\":\"{entity.creater}\",\"no\":\"{entity.no}\",\"date\":\"{entity.create_time.Value.ToString("yyyy-MM-dd")}\",\"remark\":\"{entity.remark}\",\"msg\":\" 报损报溢待审批\"}}";
                var archives = new c_archives();
                archives.id = userInfo.id;
                archives.name = userInfo.name;
                archives.phone = userInfo.phone_no;
                var employeenotice = new List<employeeMes>();


                rolenotice.ForEach(r =>
                {
                    var con = $"{{\"name\":\"{entity.creater}\",\"msg\":\"提交了报损报溢申请，请处理！\"}}";
                    employeeSocket.Add(new WebSocketModel { userId = r.id, content = con });

                    employeenotice.Add(new employeeMes { employee_id = r.id, employee = r.name });

                });
                notice.NewMethod(entity.no, archives, entity.store_id, notice, noticeList, 3, 11, notice_content, entity.creater, employeenotice);

                //新增
                notice.AddNotice(noticeList);
                ChatWebSocketMiddleware.SendListAsync(employeeSocket);
            }
        }

        /// <summary>
        /// 撤销报损报溢
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> CancelAsync(bus_loss_overflow entity)
        {
            //查询用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            var result = Db.Ado.UseTran(() =>
            {
                var loss_overflow = Db.Queryable<bus_loss_overflow>().Where(w => w.no == entity.no).WithCache().First();
                if (loss_overflow.creater_id != userInfo.id)
                {
                    throw new MessageException("非本人创建调拨单，不能取消");
                }
                var stateList = new List<short> { 26, 36 };
                if (!stateList.Contains(loss_overflow.state.Value))
                {
                    throw new MessageException("报损报溢单已审核完成，不能取消");
                }

                //修改报损报溢单状态
                Db.Updateable<bus_loss_overflow>().SetColumns(s => new bus_loss_overflow { state = 7 }).Where(w => w.no == entity.no).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                //生成调拨单负单
                loss_overflow.state = 7;
                loss_overflow.delete_no = loss_overflow.no;
                loss_overflow.no = "-" + loss_overflow.no;
                loss_overflow.creater = userInfo.name;
                loss_overflow.creater_id = userInfo.id;
                loss_overflow.create_time = DateTime.Now;
                loss_overflow.num = -loss_overflow.num;
                Db.Insertable(loss_overflow).ExecuteCommand();
                redisCache.RemoveAll<bus_loss_overflow>();

                //查询盘点明细
                var stocktaking_detials = Db.Queryable<bus_stocktaking_detials>().Where(w => w.no == loss_overflow.realted_no && w.std_item_id == loss_overflow.std_item_id && w.spec == loss_overflow.spec && w.manufactor_id == loss_overflow.manufactor_id).WithCache().First();
                if (loss_overflow.type_id == 1)//还原报溢
                {
                    stocktaking_detials.not_report_num -= loss_overflow.num;
                    stocktaking_detials.report_num += loss_overflow.num;
                }
                else//还原报损
                {
                    stocktaking_detials.not_report_num += loss_overflow.num;
                    stocktaking_detials.report_num -= loss_overflow.num;
                }
                //修改盘点待报已报数量
                Db.Updateable<bus_stocktaking_detials>().SetColumns(s => new bus_stocktaking_detials { not_report_num = stocktaking_detials.not_report_num, report_num = stocktaking_detials.report_num }).Where(w => w.no == loss_overflow.realted_no && w.std_item_id == loss_overflow.std_item_id && w.spec == loss_overflow.spec && w.manufactor_id == loss_overflow.manufactor_id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                //修改盘点单状态
                Db.Updateable<bus_stocktaking>().SetColumns(s => new bus_stocktaking { state = 33 }).Where(w => w.no == loss_overflow.realted_no).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                //查询报损报溢单明细
                var loss_overflow_detials = Db.Queryable<bus_loss_overflow_detials>().Where(w => w.no == entity.no).WithCache().ToList();

                //修改固定资产状态为闲置中
                var nos = loss_overflow_detials.Select(s => s.assets_no).ToList();
                Db.Updateable<bus_assets>().SetColumns(s => new bus_assets { state = 30 }).Where(w => nos.Contains(w.no)).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                //生成报损报溢单明细负单
                var del_loss_overflow_detials = loss_overflow_detials.Select(s => new bus_loss_overflow_detials { remark = s.remark, no = loss_overflow.no, assets_no = s.assets_no }).ToList();
                Db.Insertable(del_loss_overflow_detials).ExecuteCommand();
                redisCache.RemoveAll<bus_loss_overflow_detials>();
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 报损报溢分页
        /// </summary>
        /// <returns></returns>
        public async Task<Page<LossOverflow>> GetPageAsync(LossOverflowPageSearch entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            if (entity.start_date != null)
            {
                entity.start_date = entity.start_date.Value.Date;
            }
            if (entity.end_date != null)
            {
                entity.end_date = entity.end_date.Value.AddDays(1).Date;
            }
            var pages = await Db.Queryable<bus_loss_overflow>()
                            .Where(w => w.org_id == userInfo.org_id)
                            .WhereIF(!string.IsNullOrEmpty(entity.no), w => w.no.Contains(entity.no))
                            .WhereIF(!string.IsNullOrEmpty(entity.realted_no), w => w.realted_no.Contains(entity.realted_no))
                            .WhereIF(entity.start_date != null, w => w.create_time >= entity.start_date)
                            .WhereIF(entity.end_date != null, w => w.create_time < entity.end_date)
                            .WhereIF(entity.store_id != -1, w => w.store_id == entity.store_id)
                            .WhereIF(entity.state != -1, w => w.state == entity.state)
                            .WhereIF(entity.type_id != -1, w => w.type_id == entity.type_id)
                            .Select(s => new LossOverflow { creater = s.creater, store_id = s.store_id, creater_id = s.creater_id, create_time = s.create_time, no = s.no, org_id = s.org_id, remark = s.remark, store_name = s.store_name, state = s.state, await_verifier = s.await_verifier, realted_no = s.realted_no, level = s.level, delete_no = s.delete_no, await_verifier_id = s.await_verifier_id, total_level = s.total_level, verifier = s.verifier, verifier_id = s.verifier_id, verify_remark = s.verify_remark, verify_time = s.verify_time, is_org = s.is_org, type_id = s.type_id, num = s.num, item_type = s.item_type, item_type_id = s.item_type_id, manufactor = s.manufactor, manufactor_id = s.manufactor_id, name = s.name, process_id = s.process_id, property_id = s.property_id, spec = s.spec, std_item_id = s.std_item_id, unit = s.unit })
                            .OrderBy(entity.order + orderTypeStr)
                            .WithCache()
                            .ToPageAsync(entity.page, entity.limit);

            //列表中的盘点单号
            var nos = pages.Items.Select(s => s.no).ToList();

            //查询盘点单明细
            var loss_overflow_detials_list = await Db.Queryable<bus_loss_overflow_detials>().Where(w => nos.Contains(w.no)).WithCache().ToListAsync();

            //列表中的审核流程ID
            var process_ids = pages.Items.Select(s => s.process_id).ToList();

            //审核流程
            var process_list = await Db.Queryable<p_process>().Where(w => process_ids.Contains(w.id)).WithCache().ToListAsync();

            //审核流程明细
            var process_detials_list = await Db.Queryable<p_process_detials>().Where(w => process_ids.Contains(w.id)).WithCache().ToListAsync();

            //审核记录
            var verify_detials_list = await Db.Queryable<r_verify_detials>().Where(w => nos.Contains(w.realted_no)).WithCache().ToListAsync();

            //赋值调拨明细
            pages.Items = pages.Items.Select(s => new LossOverflow { creater = s.creater, store_id = s.store_id, creater_id = s.creater_id, create_time = s.create_time, no = s.no, org_id = s.org_id, remark = s.remark, store_name = s.store_name, state = s.state, await_verifier = s.await_verifier, realted_no = s.realted_no, level = s.level, delete_no = s.delete_no, await_verifier_id = s.await_verifier_id, total_level = s.total_level, verifier = s.verifier, verifier_id = s.verifier_id, verify_remark = s.verify_remark, verify_time = s.verify_time, is_org = s.is_org, type_id = s.type_id, num = s.num, item_type = s.item_type, item_type_id = s.item_type_id, manufactor = s.manufactor, manufactor_id = s.manufactor_id, name = s.name, process_id = s.process_id, property_id = s.property_id, spec = s.spec, std_item_id = s.std_item_id, unit = s.unit, loss_Overflow_Detials = loss_overflow_detials_list.Where(w => w.no == s.no).ToList(), verifyInfos = verify(s.process_id.Value, s.no, process_list, process_detials_list, verify_detials_list) }).ToList();

            return pages;
        }
        private List<verifyInfo> verify(int process_id, string no, List<p_process> process_list, List<p_process_detials> process_detials_list, List<r_verify_detials> verify_detials_list)
        {
            var list = new List<verifyInfo>();
            if (process_id > 0)
            {
                //审核流程
                var process = process_list.Where(w => w.id == process_id).FirstOrDefault();
                //审核流程明细
                var p_process_detials = process_detials_list.Where(w => w.id == process.id).OrderBy(o => o.level).ToList();
                //审核记录
                var Verify_Detials = verify_detials_list.Where(w => w.realted_no == no).ToList();
                foreach (var item in p_process_detials)
                {
                    var vd = Verify_Detials.Where(w => w.process_level == item.level).FirstOrDefault();
                    if (vd == null)
                    {
                        list.Add(new verifyInfo { is_org = item.is_org.Value, state = 26, verifier = "", verify_remark = "", verify_time = "", dept = item.dept_name, role = item.role_name });
                    }
                    else
                    {
                        list.Add(new verifyInfo { is_org = item.is_org.Value, state = vd.state, verifier = vd.verifier, verify_remark = vd.verify_remark, verify_time = vd.verify_time.ToString("yyyy-MM-dd HH:mm:ss"), dept = item.dept_name, role = item.role_name });
                        if (vd.state == 28)
                        {
                            break;
                        }
                    }
                }
            }
            return list;
        }


        /// <summary>
        /// 获取报损报溢详情
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        public async Task<LossOverflow> GetLossDetail(string no)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
                
            //查询单据
            var loss = await Db.Queryable<bus_loss_overflow>()
                             .Where(a => a.no == no && a.org_id == userInfo.org_id)
                             .Select(a => new LossOverflow { creater = a.creater, store_id = a.store_id, creater_id = a.creater_id, create_time = a.create_time, no = a.no, org_id = a.org_id, remark = a.remark, store_name = a.store_name, state = a.state, await_verifier = a.await_verifier, realted_no = a.realted_no, level = a.level, delete_no = a.delete_no, await_verifier_id = a.await_verifier_id, total_level = a.total_level, verifier = a.verifier, verifier_id = a.verifier_id, verify_remark = a.verify_remark, verify_time = a.verify_time, is_org = a.is_org, type_id = a.type_id, num = a.num, item_type = a.item_type, item_type_id = a.item_type_id, manufactor = a.manufactor, manufactor_id = a.manufactor_id, name = a.name, process_id = a.process_id, property_id = a.property_id, spec = a.spec, std_item_id = a.std_item_id, unit = a.unit })
                             .WithCache()
                             .FirstAsync();

            if (loss==null)
            {
                throw new MessageException("未获取到报损报溢单！");
            }

            //查询盘点单明细
            var loss_overflow_detials_list = await Db.Queryable<bus_loss_overflow_detials>().Where(w => w.no==loss.no).WithCache().ToListAsync();
            
            //审核流程
            var process_list = await Db.Queryable<p_process>().Where(w => w.id==loss.process_id).WithCache().ToListAsync();

            //审核流程明细
            var process_detials_list = await Db.Queryable<p_process_detials>().Where(w => w.id==loss.process_id).WithCache().ToListAsync();

            //审核记录
            var verify_detials_list = await Db.Queryable<r_verify_detials>().Where(w => w.realted_no==loss.no).WithCache().ToListAsync();

            return new LossOverflow { creater = loss.creater, store_id = loss.store_id, creater_id = loss.creater_id, create_time = loss.create_time, no = loss.no, org_id = loss.org_id, remark = loss.remark, store_name = loss.store_name, state = loss.state, await_verifier = loss.await_verifier, realted_no = loss.realted_no, level = loss.level, delete_no = loss.delete_no, await_verifier_id = loss.await_verifier_id, total_level = loss.total_level, verifier = loss.verifier, verifier_id = loss.verifier_id, verify_remark = loss.verify_remark, verify_time = loss.verify_time, is_org = loss.is_org, type_id = loss.type_id, num = loss.num, item_type = loss.item_type, item_type_id = loss.item_type_id, manufactor = loss.manufactor, manufactor_id = loss.manufactor_id, name = loss.name, process_id = loss.process_id, property_id = loss.property_id, spec = loss.spec, std_item_id = loss.std_item_id, unit = loss.unit, loss_Overflow_Detials = loss_overflow_detials_list.Where(w => w.no == loss.no).ToList(), verifyInfos = verify(loss.process_id.Value, loss.no, process_list, process_detials_list, verify_detials_list) };


        }

        /// <summary>
        /// 导出报损报溢
        /// </summary>
        /// <param name="list_no"></param>
        /// <returns></returns>
        public async Task<string> ExportAsync(List<string> list_no)
        {
            if (list_no.Count == 0)
            {
                throw new MessageException("请选择报损(溢)单进行导出");
            }


            var applys = await Db.Queryable<bus_loss_overflow>()
                            .Where(w => list_no.Contains(w.no))
                            .WithCache().ToListAsync();

            //申请单号对应的所有明细
            var applyDetials = await Db.Queryable<bus_loss_overflow_detials>()
                            .Where(w => list_no.Contains(w.no))
                            .WithCache().ToListAsync();

            //定义表头
            var headers = new List<string>() { "序号", "物资编号"};

            string sWebRootFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot\\tempExcel");//如果用浏览器url下载的方式  存放excel的文件夹一定要建在网站首页的同级目录下！！！
            if (!Directory.Exists(sWebRootFolder))
            {
                Directory.CreateDirectory(sWebRootFolder);
            }

            //查询用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            string sFileName = $@"lossover_{userInfo.id}.xlsx";
            var path = Path.Combine(sWebRootFolder, sFileName);
            FileInfo file = new FileInfo(path);
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(path);
            }
            using (ExcelPackage package = new ExcelPackage(file))
            {
                applys.ForEach(item =>
                {
                    var type = item.type_id == 1 ? "报溢单" : "报损单";
                    //创建sheet
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add($"{item.no}");
                    worksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[1, 1, 2, 9].Merge = true;
                    worksheet.Cells[1, 1].Value = "报损(溢)单";
                    worksheet.Cells[1, 1].Style.Font.Size = 20;
                    worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[3, 1, 3, 3].Merge = true;
                    worksheet.Cells[3, 1].Value = $"单号：{item.no}";
                    worksheet.Cells[3, 4, 3, 6].Merge = true;
                    worksheet.Cells[3, 4].Value = $"关联盘点单：{item.realted_no}";
                    worksheet.Cells[3, 7, 3, 9].Merge = true;
                    worksheet.Cells[3, 7].Value = $"单据类型：{type}";
                    worksheet.Cells[4, 1, 4, 3].Merge = true;
                    worksheet.Cells[4, 1].Value = $"物资名称：{item.name}";
                    worksheet.Cells[4, 4, 4, 6].Merge = true;
                    worksheet.Cells[4, 6].Value = $"规格：{item.spec}";
                    worksheet.Cells[4, 7, 4, 9].Merge = true;
                    worksheet.Cells[4, 7].Value = $"厂家：{item.manufactor}";
                    worksheet.Cells[5, 1, 5, 3].Merge = true;
                    worksheet.Cells[5, 1].Value = $"处理数量：{item.num}";
                    worksheet.Cells[5, 4, 5, 6].Merge = true;
                    worksheet.Cells[5, 6].Value = $"创建人：{item.creater}";
                    worksheet.Cells[5, 7, 5, 9].Merge = true;
                    worksheet.Cells[5, 7].Value = $"创建时间：{item.create_time.Value.ToString("yyyy-MM-dd hh:mm:ss")}";
                    worksheet.Cells[6, 1, 6, 9].Merge = true;
                    worksheet.Cells[6, 1].Value = $"备注：{item.remark}";
                    //表头字段
                    for (int i = 0; i < headers.Count; i++)
                    {
                        worksheet.Cells[7, i + 1].Value = headers[i];
                    }
                    var row = 8;
                    var dataList = applyDetials.Where(w => w.no == item.no).ToList();
                    for (int i = 0; i < dataList.Count; i++)
                    {
                        worksheet.Cells[row, 1].Value = i + 1;
                        worksheet.Cells[row, 2].Value = dataList[i].assets_no;                       
                        row++;
                    }
                });

                package.Save();
            }
            return path;//这是返回文件的方式
            //return "tempExcel/" + sFileName;    //如果用浏览器url下载的方式  这里直接返回生成的文件名就可以了
        }

    }
}
