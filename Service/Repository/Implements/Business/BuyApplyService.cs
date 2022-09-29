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
using static Tools.IdentityModels.GetUser;

namespace Service.Repository.Implements.Business
{
    /// <summary>
    /// 采购申请业务--ty
    /// </summary>
    public class BuyApplyService : DbContext, IBuyApplyService
    {
        //获取用户信息
        private UserInfo userInfo = new Tools.IdentityModels.GetUser().userInfo;
        /// <summary>
        /// 采购申请分页数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Page<bus_buy_apply>> GetPageAsync(BuyApplySearch entity)
        {
            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            if (entity.pass)
            {
                entity.state = -1;
            }
            var passList = new List<short?> { 34, 39, 33, 25 };
            //门店
            if (entity.store_id > 0)
            {
                var applyList = Db.Queryable<bus_buy_apply>()
                                         .Where(s => s.org_id == userInfo.org_id && s.store_id == entity.store_id)
                                         .WhereIF(!String.IsNullOrEmpty(entity.name), s => s.apply_no.Contains(entity.name) || s.applicant.Contains(entity.name))
                                         .WhereIF(entity.state >= 0 && entity.state != 34, s => s.state == entity.state)
                                         .WhereIF(entity.pass && entity.state == 34, s => passList.Contains(s.state))
                                         .WhereIF(!entity.pass && entity.state == 34, s => s.state == 34)
                                         .OrderBy(entity.order + orderTypeStr);

                return await applyList.WithCache().ToPageAsync(entity.page, entity.limit);
            }
            //机构
            else
            {
                var applyList = Db.Queryable<bus_buy_apply>()
                         .Where(s => s.org_id == userInfo.org_id)
                         .WhereIF(entity.state >= 0 && entity.state != 34, s => s.state == entity.state)
                         .WhereIF(entity.pass && entity.state == 34, s => passList.Contains(s.state))
                         .WhereIF(!entity.pass && entity.state == 34, s => s.state == 34)
                         .WhereIF(!String.IsNullOrEmpty(entity.name), s => s.apply_no.Contains(entity.name) || s.applicant.Contains(entity.name) || s.store.Contains(entity.name))
                         .WhereIF(entity.store_select_id == -1, s => s.store_id == 0 || (s.store_id != 0 && s.state != 30))
                         .WhereIF(entity.store_select_id == 0, s => s.store_id == 0)
                         .WhereIF(entity.store_select_id > 0, s => s.store_id == entity.store_select_id && s.state != 30)
                         .OrderBy(entity.order + orderTypeStr);

                return await applyList.WithCache().ToPageAsync(entity.page, entity.limit);
            }



        }

        /// <summary>
        /// 采购申请详情分页数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Page<bus_buy_apply_detials>> GetPageDetailAsync(BuyApplyDetailSearch entity)
        {
            if (String.IsNullOrEmpty(entity.apply_no))
            {
                throw new MessageException("请选择申请单！");
            }
            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }

            var applyList = Db.Queryable<bus_buy_apply_detials>()
                            .Where(s => s.apply_no == entity.apply_no)
                            .WhereIF(!String.IsNullOrEmpty(entity.name), s => s.manufactor.Contains(entity.name))
                            .OrderBy(entity.order + orderTypeStr);

            return await applyList.WithCache().ToPageAsync(entity.page, entity.limit);
        }

        /// <summary>
        ///新增采购申请单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> AddBuyApply(BuyApplyAddModel entity)
        {
            //添加采购申请
            var result = await Add(entity, 30);
            return result;

        }

        //添加采购申请
        private async Task<bool> Add(BuyApplyAddModel entity, short state)
        {
            var applyEntity = new bus_buy_apply();
            applyEntity.apply_no = "CGSQ" + DateTime.Now.ToString("yyMMdd");
            var result = Db.Ado.UseTran(() =>
            {
                //查询最大单号
                var max_no = Db.Queryable<bus_buy_apply>().Where(w => w.apply_no.StartsWith(applyEntity.apply_no)).OrderBy(o => o.apply_no, OrderByType.Desc).Select(s => s.apply_no).WithCache().First();
                if (max_no == null)
                {
                    applyEntity.apply_no += "0001";
                }
                else
                {
                    max_no = max_no.Substring(applyEntity.apply_no.Length);
                    applyEntity.apply_no += (int.Parse(max_no) + 1).ToString().PadLeft(4, '0');
                }
                applyEntity.applicant_id = userInfo.id;
                applyEntity.applicant = userInfo.name;
                applyEntity.create_time = DateTime.Now;
                applyEntity.org_id = userInfo.org_id;
                applyEntity.state = state;
                applyEntity.store_id = entity.store_id;
                applyEntity.remark = entity.remark;
                applyEntity.store_id = entity.store_id;
                applyEntity.total_price = entity.total_price;

                if (entity.store_id > 0)
                {
                    //查询门店
                    var storeName = Db.Queryable<p_store>()
                                             .Where(s => s.org_id == userInfo.org_id && s.id == entity.store_id)
                                             .Select(s => s.name)
                                             .WithCache()
                                             .First();

                    applyEntity.store = storeName;
                }
                else
                {
                    //查询机构
                    var orgName = Db.Queryable<p_org>()
                                             .Where(s => s.id == userInfo.org_id)
                                             .Select(s => s.name)
                                             .WithCache()
                                             .First();
                    applyEntity.store = orgName;
                }

                if (state != 30)
                {
                    //设置审核信息
                    SetProcess(applyEntity);


                }

                //添加申请单
                Db.Insertable(applyEntity).ExecuteCommand();
                redisCache.RemoveAll<bus_buy_apply>();

                //添加申请单明细
                if (entity.applyDetailList.Count > 0)
                {
                    var applyDetails_list = entity.applyDetailList.Select(s => new bus_buy_apply_detials { apply_no = applyEntity.apply_no, name = s.name, buy_num = s.buy_num, manufactor = s.manufactor, manufactor_id = s.manufactor_id, num = s.num, remark = s.remark, state = state, std_item_id = s.std_item_id, unit = s.unit, dispose_num = 0, price = s.price, transfer_num = 0, approval_no = s.approval_no, buy_multiple = s.buy_multiple, buy_price = s.buy_price, buy_unit = s.buy_unit, spec = s.spec }).ToList();
                    Db.Insertable(applyDetails_list).ExecuteCommand();
                    redisCache.RemoveAll<bus_buy_apply_detials>();
                }
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        //设置审核信息
        private void SetProcess(bus_buy_apply applyEntity)
        {
            //查询申请单需要进行的审核流程
            var process = Db.Queryable<p_process>().Where(w => w.type_id == 1 && w.org_id == userInfo.org_id && w.dept_id == 0 && w.state == 1 && w.use_money <= applyEntity.total_price).WhereIF(applyEntity.store_id > 0, w => w.store_id == 99).WhereIF(applyEntity.store_id == 0, w => w.store_id == 0).OrderBy(o => o.use_money, OrderByType.Desc).WithCache().First();

            if (process == null)
            {
                process = Db.Queryable<p_process>().Where(w => w.type_id == 1 && w.org_id == userInfo.org_id && w.dept_id == 0 && w.state == 1 && w.use_money >= applyEntity.total_price).WhereIF(applyEntity.store_id > 0, w => w.store_id == 99).WhereIF(applyEntity.store_id == 0, w => w.store_id == 0).OrderBy(o => o.use_money, OrderByType.Asc).WithCache().First();
            }
            if (process == null)
            {
                throw new MessageException("请先设置采购申请审核流程");
            }
            applyEntity.apply_time = DateTime.Now;
            applyEntity.total_level = process.total_level;
            applyEntity.org_process_id = process.id;

            applyEntity.level = 0;
            applyEntity.await_verifier_id = 0;
            applyEntity.is_org = false;

            //审核流程明细
            var process_detials = Db.Queryable<p_process_detials>().Where(w => w.id == process.id).OrderBy(o => o.level).WithCache().ToList();
            //循环流程明细
            foreach (var item in process_detials)
            {
                var isExiste = false;
                if (!item.is_org.Value)
                {
                    isExiste = Db.Queryable<p_role>().Any(w => w.link_id == item.role_id && w.store_id == applyEntity.store_id);
                }
                else
                {
                    isExiste = Db.Queryable<p_role>().Any(w => w.id == item.role_id && w.store_id == 0);
                }
                if (isExiste)
                {
                    applyEntity.level = item.level;
                    applyEntity.await_verifier_id = item.role_id;
                    applyEntity.await_verifier = item.role_name;
                    applyEntity.is_org = item.is_org;
                    break;
                }
                else
                {
                    //审核记录
                    CommonModel model = new CommonModel();
                    model.total_level = process.total_level;
                    model.level = item.level;
                    model.store_id = applyEntity.store_id.Value;
                    model.store = applyEntity.store;
                    model.applicant_id = userInfo.id;
                    model.applicant = userInfo.name;
                    model.process_type_id = 1;
                    model.process_type = "采购";

                    ApprovalLeaveModel entity = new ApprovalLeaveModel();
                    entity.apply_no = applyEntity.apply_no;
                    entity.approval_state = 34;
                    entity.verify_remark = " ";
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
                    applyEntity.state = 36;
                    if (nextp == null)
                    {
                        applyEntity.state = 34;
                    }

                    //添加审核记录                               
                    ver.SetVerify(entity, process_currentMes, userInfo, item.level, model, 2, nextp);

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
                                   .WhereIF(applyEntity.is_org == false, (a, b, e) => b.link_id == applyEntity.await_verifier_id && a.store_id > 0)
                                   .WhereIF(applyEntity.is_org == true, (a, b, e) => b.id == applyEntity.await_verifier_id && a.store_id == 0)
                                   .Select((a, b, e) => new { e.id, e.name })
                                   .WithCache()
                                   .ToList();

            if (rolenotice.Count > 0)
            {
                //采购通知
                notice_content = $"{{\"name\":\"{applyEntity.applicant}\",\"no\":\"{applyEntity.apply_no}\",\"date\":\"{applyEntity.apply_time.ToString()}\",\"total_price\":\"{applyEntity.total_price}\",\"remark\":\"{applyEntity.remark}\",\"msg\":\" 采购待审批\"}}";
                var archives = new c_archives();
                archives.id = userInfo.id;
                archives.name = userInfo.name;
                archives.phone = userInfo.phone_no;
                var employeenotice = new List<employeeMes>();


                rolenotice.ForEach(r =>
                {
                    var con = $"{{\"name\":\"{applyEntity.applicant}\",\"msg\":\"提交了采购申请，请处理！\"}}";
                    employeeSocket.Add(new WebSocketModel { userId = r.id, content = con });

                    employeenotice.Add(new employeeMes { employee_id = r.id, employee = r.name });

                });
                notice.NewMethod(applyEntity.apply_no, archives, applyEntity.store_id.Value, notice, noticeList, 3, 3, notice_content, applyEntity.applicant, employeenotice);

                //新增
                notice.AddNotice(noticeList);
                ChatWebSocketMiddleware.SendListAsync(employeeSocket);
            }
        }

        /// <summary>
        /// 编辑采购申请
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> ModifyApply(BuyApplyModifyModel entity)
        {
            if (String.IsNullOrEmpty(entity.apply_no))
            {
                throw new MessageException("请选择申请单!");
            }
            //查询是否本人申请
            var creator = await Db.Queryable<bus_buy_apply>()
                                .Where(s => s.org_id == userInfo.org_id && s.apply_no == entity.apply_no)
                                .Select(s => new { s.applicant_id, s.state })
                                .WithCache()
                                .FirstAsync();

            if (creator.applicant_id != userInfo.id)
            {
                throw new MessageException("非本人申请无法编辑!");
            }
            if (creator.state != 30)
            {
                throw new MessageException("此单据无法编辑!");
            }

            var result = Db.Ado.UseTran(() =>
            {
                //编辑申请单
                Db.Updateable<bus_buy_apply>().SetColumns(a => new bus_buy_apply { remark = entity.remark })
                                              .Where(a => a.apply_no == entity.apply_no)
                                              .RemoveDataCache()
                                              .EnableDiffLogEvent()
                                              .ExecuteCommand();
                //删除之前申请单明细
                Db.Deleteable<bus_buy_apply_detials>()
                  .Where(w => w.apply_no == entity.apply_no)
                  .RemoveDataCache()
                  .EnableDiffLogEvent()
                  .ExecuteCommand();

                //添加申请单明细
                if (entity.applyDetailList.Count > 0)
                {
                    var applyDetails_list = entity.applyDetailList.Select(s => new bus_buy_apply_detials { apply_no = entity.apply_no, name = s.name, buy_num = s.buy_num, manufactor = s.manufactor, manufactor_id = s.manufactor_id, num = s.num, remark = s.remark, state = 30, std_item_id = s.std_item_id, unit = s.unit, price = s.price, dispose_num = 0, transfer_num = 0, approval_no = s.approval_no, buy_multiple = s.buy_multiple, buy_price = s.buy_price, buy_unit = s.buy_unit, spec = s.spec }).ToList();
                    Db.Insertable(applyDetails_list).ExecuteCommand();
                    redisCache.RemoveAll<bus_buy_apply_detials>();
                }


            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 提交采购申请
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> CommitApply(CommitModel entity)
        {
            if (String.IsNullOrEmpty(entity.apply_no))
            {
                throw new MessageException("请选择采购单!");
            }

            var result = Db.Ado.UseTran(() =>
            {
                var applyEntity = Db.Queryable<bus_buy_apply>().Where(w => w.apply_no == entity.apply_no).WithCache().First();
                //设置审核信息
                SetProcess(applyEntity);

                Db.Updateable<bus_buy_apply>()
                       .SetColumns(w => new bus_buy_apply { apply_time = DateTime.Now, state = 26, await_verifier = applyEntity.await_verifier, await_verifier_id = applyEntity.await_verifier_id, level = applyEntity.level, total_level = applyEntity.total_level, org_process_id = applyEntity.org_process_id, is_org = applyEntity.is_org })
                       .Where(w => w.apply_no == entity.apply_no)
                       .RemoveDataCache()
                       .EnableDiffLogEvent()
                       .ExecuteCommand();


                //改明细为待处理
                Db.Updateable<bus_buy_apply_detials>()
                           .SetColumns(it => new bus_buy_apply_detials { state = 24 })
                           .Where(it => it.apply_no == entity.apply_no)
                           .RemoveDataCache()
                           .EnableDiffLogEvent()
                           .ExecuteCommand();
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 作废采购申请单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> DeleteApply(CommitModel entity)
        {
            if (String.IsNullOrEmpty(entity.apply_no))
            {
                throw new MessageException("请选择采购申请单!");
            }
            //查询是否本人申请
            var creator = await Db.Queryable<bus_buy_apply>()
                                .Where(s => s.org_id == userInfo.org_id && s.apply_no == entity.apply_no)
                                .WithCache()
                                .FirstAsync();

            if (creator.applicant_id != userInfo.id)
            {
                throw new MessageException("非本人申请无法作废!");
            }
            var stateList = new List<short> { 39, 33, 25 };
            if (stateList.Contains(creator.state.Value))
            {
                throw new MessageException("此单据无法作废!");
            }
            var result = Db.Ado.UseTran(() =>
            {

                Db.Updateable<bus_buy_apply>().SetColumns(a => new bus_buy_apply { state = 32 })
                                              .Where(a => a.apply_no == entity.apply_no)
                                              .RemoveDataCache()
                                              .EnableDiffLogEvent()
                                              .ExecuteCommand();
                //添加作废负单
                creator.delete_no = creator.apply_no;
                creator.apply_no = "-" + creator.apply_no;
                creator.applicant_id = userInfo.id;
                creator.applicant = userInfo.name;
                creator.create_time = DateTime.Now;
                creator.state = 32;
                creator.total_price = 0 - creator.total_price;
                Db.Insertable(creator).ExecuteCommand();
                redisCache.RemoveAll<bus_buy_apply>();
                //添加负单明细
                var list = Db.Queryable<bus_buy_apply_detials>().Where(w => w.apply_no == entity.apply_no).WithCache().ToList();
                var f_list = list.Select(s => new bus_buy_apply_detials { apply_no = creator.apply_no, approval_no = s.approval_no, buy_multiple = s.buy_multiple, buy_num = short.Parse((0 - s.buy_num).ToString()), buy_price = s.buy_price, buy_unit = s.buy_unit, dispose_num = short.Parse((0 - s.dispose_num).ToString()), manufactor = s.manufactor, manufactor_id = s.manufactor_id, name = s.name, num = short.Parse((0 - s.num).ToString()), price = s.price, remark = s.remark, spec = s.spec, state = s.state, std_item_id = s.std_item_id, transfer_num = short.Parse((0 - s.transfer_num).ToString()), unit = s.unit }).ToList();
                Db.Insertable(f_list).ExecuteCommand();
                redisCache.RemoveAll<bus_buy_apply_detials>();
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 获得分页列表（已审核）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Page<ApplyBill>> GetApplyPageAsync(ApplyBillPageSearch entity)
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
            var passList = new List<short?> { 34, 39, 33, 25 };
            //获取分页列表
            var pages = await Db.Queryable<bus_buy_apply_detials, bus_buy_apply>((bbad, bba) => new object[] { JoinType.Left, bbad.apply_no == bba.apply_no })
                            .Where((bbad, bba) => bba.org_id == userInfo.org_id && passList.Contains(bba.state))
                            .WhereIF(!string.IsNullOrEmpty(entity.apply_no), (bbad, bba) => bbad.apply_no == entity.apply_no)
                            .WhereIF(entity.store_id > 0, (bbad, bba) => bba.store_id == entity.store_id)
                            .WhereIF(entity.dispose_state == 0, (bbad, bba) => bbad.dispose_num != bbad.num)
                            .WhereIF(entity.dispose_state == 1, (bbad, bba) => bbad.dispose_num == bbad.num)
                            .WhereIF(entity.start_date != null, (bbad, bba) => bba.apply_time >= entity.start_date)
                            .WhereIF(entity.end_date != null, (bbad, bba) => bba.apply_time < entity.end_date)
                            .GroupBy((bbad, bba) => new { bba.applicant, bba.applicant_id, bba.apply_no, bba.apply_time, bba.create_time, bba.org_id, bba.remark, bba.state, bba.store, bba.store_id, bba.verifier, bba.verifier_id, bba.verify_remark, bba.verify_time })
                            .Select((bbad, bba) => new ApplyBill { applicant = bba.applicant, applicant_id = bba.applicant_id, apply_no = bba.apply_no, apply_time = bba.apply_time, create_time = bba.create_time, org_id = bba.org_id, remark = bba.remark, state = bba.state, store = bba.store, store_id = bba.store_id, verifier = bba.verifier, verifier_id = bba.verifier_id, verify_remark = bba.verify_remark, verify_time = bba.verify_time })
                            .OrderBy(entity.order + orderTypeStr)
                            .WithCache()
                            .ToPageAsync(entity.page, entity.limit);
            //列表中的申请单号
            var applynos = pages.Items.Select(s => s.apply_no).ToList();
            //申请单号对应的所有明细
            var applyDetials = await Db.Queryable<bus_buy_apply_detials>()
                            .Where(w => applynos.Contains(w.apply_no))
                            .WhereIF(entity.dispose_state == 0, w => w.dispose_num != w.num)
                            .WhereIF(entity.dispose_state == 1, w => w.dispose_num == w.num)
                            .WithCache().ToListAsync();
            //赋值申请明细
            pages.Items = pages.Items.Select(s => new ApplyBill { applicant = s.applicant, applicant_id = s.applicant_id, apply_no = s.apply_no, apply_time = s.apply_time, create_time = s.create_time, store = s.store, store_id = s.store_id, verifier = s.verifier, verifier_id = s.verifier_id, verify_remark = s.verify_remark, verify_time = s.verify_time, org_id = s.org_id, remark = s.remark, state = s.state, bus_Buy_Apply_Detials = applyDetials.Where(w => w.apply_no == s.apply_no).ToList() }).ToList();

            return pages;
        }
        /// <summary>
        /// 立即提交采购申请
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> ImmediatelyCommit(BuyApplyAddModel entity)
        {
            //添加采购申请
            var result = await Add(entity, 26);
            return result;
        }

        /// <summary>
        /// 获取明细及审核信息
        /// </summary>
        /// <param name="apply_no">采购申请单号</param>
        /// <returns></returns>
        public async Task<BuyApplyDetials> GetDetialsAndVerifyAsync(string apply_no)
        {
            //查询申请单
            var apply = await Db.Queryable<bus_buy_apply>().Where(w => w.apply_no == apply_no).WithCache().FirstAsync();
            //申请单明细
            var Detials = await Db.Queryable<bus_buy_apply_detials>().Where(w => w.apply_no == apply_no).WithCache().ToListAsync();

            //获取固定资产基础项目ID
            var assets_std_list = await Db.Queryable<p_std_item, b_codebase>((si, cb) => new object[] { JoinType.Left, si.type_id == cb.id }).Where((si, cb) => cb.property_id == 1).WithCache().Select((si, cb) => si.id).ToListAsync();

            //固定资产
            var assets_detials = Detials.Where(w => assets_std_list.Contains(w.std_item_id)).ToList();
            var assets = await Db.Queryable<bus_assets>().Where(w => w.org_id == userInfo.org_id && w.store_id == 0 && w.state == 30).OrderBy(o => o.id).WithCache().ToListAsync();
            var buy_Apply_Detials_assets = assets_detials.Select(s => new buy_Apply_Detials_assets { apply_no = s.apply_no, approval_no = s.approval_no, state = s.state, buy_multiple = s.buy_multiple, buy_num = s.buy_num, buy_price = s.buy_price, buy_unit = s.buy_unit, dispose_num = s.dispose_num, manufactor = s.manufactor, manufactor_id = s.manufactor_id, name = s.name, num = s.num, price = s.price, remark = s.remark, spec = s.spec, std_item_id = s.std_item_id, transfer_num = s.transfer_num, unit = s.unit, bus_Assets = assets.Where(w => w.std_item_id == s.std_item_id && w.spec == s.spec && w.manufactor_id == s.manufactor_id).ToList() }).ToList();

            //非固定资产
            var detials = Detials.Where(w => !assets_std_list.Contains(w.std_item_id)).ToList();
            var list = new List<verifyInfo>();
            if (apply.state != 30)
            {
                //审核流程
                var process = await Db.Queryable<p_process>().Where(w => w.id == apply.org_process_id).WithCache().FirstAsync();
                //审核流程明细
                var p_process_detials = await Db.Queryable<p_process_detials>().Where(w => w.id == process.id).OrderBy(o => o.level).WithCache().ToListAsync();
                //审核记录
                var Verify_Detials = await Db.Queryable<r_verify_detials>().Where(w => w.realted_no == apply_no).WithCache().ToListAsync();
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
            return new BuyApplyDetials { buy_Apply_Detials = detials, buy_Apply_Detials_assets = buy_Apply_Detials_assets, verifyInfos = list };
        }

        /// <summary>
        /// 根据申请单号集合获取对应明细
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task<List<bus_buy_apply_detials>> GetDetialsAsync(List<string> list)
        {
            return await Db.Queryable<bus_buy_apply_detials>().Where(w => list.Contains(w.apply_no)).WithCache().ToListAsync();
        }

        /// <summary>
        /// 作废草稿
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> DeleteDrafyAsync(CommitModel entity)
        {
            //查询用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            var result = Db.Ado.UseTran(() =>
            {
                var bill = Db.Queryable<bus_buy_apply>().Where(w => w.apply_no == entity.apply_no).WithCache().First();
                if (bill == null)
                {
                    throw new MessageException("单据不存在");
                }
                if (bill.applicant_id != userInfo.id)
                {
                    throw new MessageException("非本人创建采购单，不能删除草稿");
                }
                if (bill.state != 30)
                {
                    throw new MessageException("非草稿，不能删除");
                }

                //查询采购单明细
                var details = Db.Queryable<bus_buy_apply_detials>().Where(w => w.apply_no == entity.apply_no).WithCache().ToList();

                //删除采购单
                Db.Deleteable<bus_buy_apply>().Where(w => w.apply_no == entity.apply_no).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                //删除明细
                Db.Deleteable<bus_buy_apply_detials>().Where(w => w.apply_no == entity.apply_no).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <returns></returns>
        public async Task<string> ExportAsync(string No)
        {
            if (string.IsNullOrEmpty(No))
            {
                throw new MessageException("请选择采购申请单进行导出");
            }
            var applynos = No.Split(',').ToList();

            var applys = await Db.Queryable<bus_buy_apply>()
                            .Where(w => applynos.Contains(w.apply_no))
                            .WithCache().ToListAsync();

            //申请单号对应的所有明细
            var applyDetials = await Db.Queryable<bus_buy_apply_detials>()
                            .Where(w => applynos.Contains(w.apply_no))
                            .WithCache().ToListAsync();

            //定义表头
            var headers = new List<string>() { "序号", "名称", "规格", "数量", "采购单位", "采购单价", "采购总价", "厂家", "备注" };

            string sWebRootFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot\\tempExcel");
            if (!Directory.Exists(sWebRootFolder))
            {
                Directory.CreateDirectory(sWebRootFolder);
            }

            //查询用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            string sFileName = $@"buyapply_{userInfo.id}.xlsx";
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
                    //创建sheet
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add($"{item.apply_no}");
                    worksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[1, 1, 2, 9].Merge = true;
                    worksheet.Cells[1, 1].Value = "采购申请单";
                    worksheet.Cells[1, 1].Style.Font.Size = 20;
                    worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[3, 1, 3, 3].Merge = true;
                    worksheet.Cells[3, 1].Value = $"单号：{item.apply_no}";
                    worksheet.Cells[3, 4, 3, 6].Merge = true;
                    worksheet.Cells[3, 4].Value = $"门店：{item.store}";
                    worksheet.Cells[3, 7, 3, 9].Merge = true;
                    worksheet.Cells[3, 7].Value = $"总金额：{item.total_price}";
                    worksheet.Cells[4, 1, 4, 5].Merge = true;
                    worksheet.Cells[4, 1].Value = $"申请人：{item.applicant}";
                    worksheet.Cells[4, 6, 4, 9].Merge = true;
                    worksheet.Cells[4, 6].Value = $"申请时间：{item.apply_time.Value.ToString("yyyy年MM月dd日 HH:mm:ss")}";
                    worksheet.Cells[5, 1, 5, 9].Merge = true;
                    worksheet.Cells[5, 1].Value = $"备注：{item.remark}";
                    //表头字段
                    for (int i = 0; i < headers.Count; i++)
                    {
                        worksheet.Cells[6, i + 1].Value = headers[i];
                    }
                    var row = 7;
                    var dataList = applyDetials.Where(w => w.apply_no == item.apply_no).ToList();
                    for (int i = 0; i < dataList.Count; i++)
                    {
                        worksheet.Cells[row, 1].Value = i + 1;
                        worksheet.Cells[row, 2].Value = dataList[i].name;
                        worksheet.Cells[row, 3].Value = dataList[i].spec;
                        worksheet.Cells[row, 4].Value = dataList[i].buy_num;
                        worksheet.Cells[row, 5].Value = dataList[i].buy_unit;
                        worksheet.Cells[row, 6].Value = dataList[i].buy_price;
                        worksheet.Cells[row, 7].Value = dataList[i].buy_price * dataList[i].buy_num;
                        worksheet.Cells[row, 8].Value = dataList[i].manufactor;
                        worksheet.Cells[row, 9].Value = dataList[i].remark;
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
