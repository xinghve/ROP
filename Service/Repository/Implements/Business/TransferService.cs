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
using Tools.IdentityModels;
using Tools.WebSocket;

namespace Service.Repository.Implements.Business
{
    /// <summary>
    /// 调拨
    /// </summary>
    public class TransferService : DbContext, ITransferService
    {
        /// <summary>
        /// 添加调拨
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> AddAsync(Transfer entity)
        {
            return await AddTransfer(entity, 26);
        }

        /// <summary>
        /// 添加调拨（草稿）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> AddDraftAsync(Transfer entity)
        {
            return await AddTransfer(entity, 30);
        }

        private async Task<bool> AddTransfer(Transfer entity, short state)
        {
            if (entity.transfer_Bill_Detials.Count == 0 && entity.transfer_Bill_Detials_assets.Count == 0)
            {
                throw new MessageException("请选择需要调拨的物资");
            }
            //查询用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            var result = Db.Ado.UseTran(() =>
            {
                var bill = new bus_transfer_bill();
                bill.bill_no = "DB" + DateTime.Now.ToString("yyMMdd");
                //查询最大单号
                var max_no = Db.Queryable<bus_transfer_bill>().Where(w => w.bill_no.StartsWith(bill.bill_no)).OrderBy(o => o.bill_no, OrderByType.Desc).Select(s => s.bill_no).WithCache().First();
                if (max_no == null)
                {
                    bill.bill_no += "0001";
                }
                else
                {
                    max_no = max_no.Substring(bill.bill_no.Length);
                    bill.bill_no += (int.Parse(max_no) + 1).ToString().PadLeft(4, '0');
                }
                bill.org_id = userInfo.org_id;
                bill.apply_no = entity.apply_no;
                bill.transfer_time = entity.transfer_time;
                bill.type = entity.type;
                bill.type_id = entity.type_id;
                bill.out_store_id = entity.out_store_id;
                bill.out_store_name = entity.out_store_name;
                if (bill.out_store_id == 0)
                {
                    var org = Db.Queryable<p_org>().Where(w => w.id == userInfo.org_id).WithCache().First();
                    bill.out_store_name = org.name;
                }
                bill.in_store_id = entity.in_store_id;
                bill.in_store_name = entity.in_store_name;
                bill.remark = entity.remark;
                bill.creater = userInfo.name;
                bill.creater_id = userInfo.id;
                bill.create_time = DateTime.Now;
                bill.process_id = 0;

                bill.state = state;
                if (entity.type_id == 1)//采购
                {
                    bill.apply_time = DateTime.Now;
                    bill.state = 34;
                }
                if (bill.state != 30)
                {
                    bill.apply_time = DateTime.Now;

                    if (entity.type_id != 1)//采购
                    {
                        //审核相关
                        SetProcess(bill);
                    }
                }

                //添加调拨单返回ID
                var command_num = Db.Insertable(bill).ExecuteCommand();
                redisCache.RemoveAll<bus_transfer_bill>();
                if (command_num <= 0)
                {
                    throw new MessageException("创建调拨失败！");
                }

                Detials(entity.transfer_Bill_Detials, entity.transfer_Bill_Detials_assets, userInfo, bill);
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 调拨明细、相关单及库存操作
        /// </summary>
        /// <param name="transfer_Bill_Detials_list"></param>
        /// <param name="transfer_Bill_Detials_assets"></param>
        /// <param name="userInfo"></param>
        /// <param name="bill"></param>
        private void Detials(List<bus_transfer_bill_detials> transfer_Bill_Detials_list, List<transfer_Bill_Detials_assets> transfer_Bill_Detials_assets, GetUser.UserInfo userInfo, bus_transfer_bill bill)
        {
            if (transfer_Bill_Detials_assets != null)
            {
                //固定资产附加到调拨明细
                transfer_Bill_Detials_assets.ForEach(item =>
                {
                    transfer_Bill_Detials_list.Add(new bus_transfer_bill_detials { std_item_id = item.std_item_id, manufactor_id = item.manufactor_id, spec = item.spec, buy_num = -1, num = Convert.ToInt16(item.assets_ids.Count), remark = item.remark });
                });
            }

            //添加调拨单明细，修改关联单
            var ids = transfer_Bill_Detials_list.Select(s => s.std_item_id).ToList();
            var specs = transfer_Bill_Detials_list.Select(s => s.spec).ToList();

            var std_item_detials = Db.Queryable<p_std_item_detials, p_std_item>((sid, si) => new object[] { JoinType.Left, sid.id == si.id }).Where((sid, si) => ids.Contains(sid.id) && specs.Contains(sid.spec)).Select((sid, si) => new { sid.spec, sid.id, sid.buy_multiple, sid.buy_price, sid.buy_unit, sid.manufactor_id, sid.manufactor, sid.price, sid.approval_no, si.unit, si.type, si.type_id, si.name }).WithCache().ToList();


            var transfer_Bill_Detials = new List<bus_transfer_bill_detials>();//调拨单明细
            var transfer_detials_storage = new List<bus_transfer_detials_storage>();//调拨单明细对应库存明细

            transfer_Bill_Detials_list.ForEach(item =>
            {
                var this_std_item_detials = std_item_detials.Where(w => w.id == item.std_item_id && w.manufactor_id == item.manufactor_id && w.spec == item.spec).FirstOrDefault();
                //添加调拨单明细
                var this_transfer_Bill_Detials = new bus_transfer_bill_detials { bill_no = bill.bill_no, name = this_std_item_detials.name, spec = this_std_item_detials.spec, std_item_id = this_std_item_detials.id, unit = this_std_item_detials.unit, approval_no = this_std_item_detials.approval_no, buy_multiple = this_std_item_detials.buy_multiple, buy_price = this_std_item_detials.buy_price, buy_unit = this_std_item_detials.buy_unit, manufactor = this_std_item_detials.manufactor, manufactor_id = this_std_item_detials.manufactor_id, price = this_std_item_detials.price, type = this_std_item_detials.type, type_id = this_std_item_detials.type_id.Value, aog_num = 0, buy_num = item.buy_num == -1 ? Math.Round(Convert.ToDecimal(item.num.Value) / this_std_item_detials.buy_multiple, 2) : item.buy_num, num = item.num, remark = item.remark };

                if (Math.Round(Convert.ToDecimal(this_transfer_Bill_Detials.num.Value) / this_transfer_Bill_Detials.buy_multiple.Value, 2) != this_transfer_Bill_Detials.buy_num)
                {
                    if (this_transfer_Bill_Detials.buy_num * this_transfer_Bill_Detials.buy_multiple != this_transfer_Bill_Detials.num)
                    {
                        throw new MessageException("非法数据，数量不匹配");
                    }
                }

                transfer_Bill_Detials.Add(this_transfer_Bill_Detials);

                var num = this_transfer_Bill_Detials.num;//最小单位数量
                decimal? bill_num = this_transfer_Bill_Detials.buy_num;//采购单位数量

                //查询库存
                var storage = Db.Queryable<bus_storage>().Where(w => w.std_item_id == item.std_item_id && w.org_id == userInfo.org_id && w.store_id == bill.out_store_id).WithCache().First();
                //修改库存可用数量
                storage.use_num -= Convert.ToInt32(num);
                if (storage.use_num < 0)
                {
                    throw new MessageException("库存数量不足");
                }
                Db.Updateable<bus_storage>().SetColumns(s => new bus_storage { use_num = storage.use_num }).Where(w => w.std_item_id == item.std_item_id && w.org_id == userInfo.org_id && w.store_id == bill.out_store_id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                //查询库存明细
                var storage_detials_list = Db.Queryable<bus_storage_detials>().Where(w => w.id == storage.id && w.spec == item.spec && w.manufactor_id == item.manufactor_id).OrderBy(o => o.expire_date).OrderBy(o => o.no).WithCache().ToList();

                var d_num = Convert.ToInt32(num);
                foreach (var storage_detials in storage_detials_list)
                {
                    if (storage_detials.use_num > 0)
                    {
                        short this_num = 0;
                        //修改库存明细可用数量
                        if (storage_detials.use_num >= d_num)
                        {
                            storage_detials.use_num -= d_num;
                            this_num = Convert.ToInt16(d_num);
                            d_num = 0;
                        }
                        else
                        {
                            this_num = Convert.ToInt16(storage_detials.use_num);
                            d_num -= storage_detials.use_num;
                            storage_detials.use_num = 0;
                        }
                        Db.Updateable<bus_storage_detials>().SetColumns(s => new bus_storage_detials { use_num = storage_detials.use_num }).Where(w => w.id == storage_detials.id && w.no == storage_detials.no).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                        transfer_detials_storage.Add(new bus_transfer_detials_storage { bill_no = bill.bill_no, expire_date = storage_detials.expire_date, id = storage_detials.id, manufactor_id = storage_detials.manufactor_id, no = storage_detials.no, num = this_num, spec = storage_detials.spec, std_item_id = storage_detials.std_item_id, buy_date = storage_detials.buy_date });
                        if (d_num == 0)
                        {
                            break;
                        }
                    }
                }

                if (bill.type_id == 1)//采购
                {
                    //查询采购申请单单明细
                    var detials = Db.Queryable<bus_buy_apply_detials>().Where(w => w.apply_no == bill.apply_no && w.std_item_id == item.std_item_id && w.manufactor_id == item.manufactor_id).WithCache().First();
                    //查询采购申请单明细到货数量
                    var dispose_num = detials.dispose_num;
                    dispose_num += item.num;
                    var transfer_num = detials.transfer_num;
                    transfer_num += bill_num;
                    if (dispose_num > detials.num * detials.buy_multiple)
                    {
                        throw new MessageException("到货数量大于采购数量，请确认");
                    }
                    if (transfer_num > detials.num)
                    {
                        transfer_num = detials.num;
                    }
                    //修改采购申请单明细数量
                    Db.Updateable<bus_buy_apply_detials>().SetColumns(it => new bus_buy_apply_detials { dispose_num = dispose_num, transfer_num = transfer_num }).Where(w => w.apply_no == bill.apply_no && w.std_item_id == item.std_item_id && w.manufactor_id == item.manufactor_id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                }
            });

            //添加调拨明细
            Db.Insertable(transfer_Bill_Detials).ExecuteCommand();
            redisCache.RemoveAll<bus_transfer_bill_detials>();

            //添加调拨明细库存明细
            Db.Insertable(transfer_detials_storage).ExecuteCommand();
            redisCache.RemoveAll<bus_transfer_detials_storage>();

            if (transfer_Bill_Detials_assets != null)
            {
                //添加调拨明细对应固定资产
                var transfer_assets = new List<bus_transfer_assets>();
                var assets_ids = new List<int>();
                transfer_Bill_Detials_assets.ForEach(item =>
                {
                    item.assets_ids.ForEach(id =>
                    {
                        assets_ids.Add(id);
                        transfer_assets.Add(new bus_transfer_assets { assets_id = id, bill_no = bill.bill_no, manufactor_id = item.manufactor_id, spec = item.spec, std_item_id = item.std_item_id });
                    });
                });
                Db.Insertable(transfer_assets).ExecuteCommand();
                redisCache.RemoveAll<bus_transfer_assets>();
                //修改固定资产状态
                Db.Updateable<bus_assets>().SetColumns(s => new bus_assets { state = 41 }).Where(w => assets_ids.Contains(w.id)).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
            }

            //修改采购申请单状态
            if (bill.type_id == 1)//采购
            {
                short apply_state = 25;
                //是否调拨全部
                var isExiste = Db.Queryable<bus_buy_apply_detials>().Any(a => a.apply_no == bill.apply_no && a.num != a.transfer_num);
                if (isExiste)
                {
                    apply_state = 33;
                }
                Db.Updateable<bus_buy_apply>().SetColumns(s => new bus_buy_apply { state = apply_state }).Where(w => w.apply_no == bill.apply_no).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
            }
        }

        //设置审核信息
        private void SetProcess(bus_transfer_bill entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            //查询调拨单需要进行的审核流程
            var store = 0;
            if (entity.out_store_id != 0)
            {
                store = 99;
            }
            var process = Db.Queryable<p_process>().Where(w => w.type_id == 2 && w.org_id == userInfo.org_id && w.dept_id == 0 && w.state == 1 && w.store_id == store).OrderBy(o => o.use_money, OrderByType.Desc).WithCache().First();
            if (process == null)
            {
                throw new MessageException("请先设置调拨申请审核流程");
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
                    isExiste = Db.Queryable<p_role>().Any(w => w.link_id == item.role_id && w.store_id == entity.out_store_id);
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
                    model.store_id = entity.in_store_id;
                    model.store = entity.in_store_name;
                    model.applicant_id = userInfo.id;
                    model.applicant = userInfo.name;
                    model.process_type_id = 2;
                    model.process_type = "调拨";

                    ApprovalLeaveModel entityA = new ApprovalLeaveModel();
                    entityA.apply_no = entity.bill_no;
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
                //调拨通知
                notice_content = $"{{\"name\":\"{entity.creater}\",\"no\":\"{entity.bill_no}\",\"date\":\"{entity.apply_time.ToString()}\",\"remark\":\"{entity.remark}\",\"msg\":\" 调拨待审批\"}}";
                var archives = new c_archives();
                archives.id = userInfo.id;
                archives.name = userInfo.name;
                archives.phone = userInfo.phone_no;
                var employeenotice = new List<employeeMes>();


                rolenotice.ForEach(r =>
                {
                    var con = $"{{\"name\":\"{entity.creater}\",\"msg\":\"提交了调拨申请，请处理！\"}}";
                    employeeSocket.Add(new WebSocketModel { userId = r.id, content = con });

                    employeenotice.Add(new employeeMes { employee_id = r.id, employee = r.name });

                });
                notice.NewMethod(entity.bill_no, archives, entity.in_store_id, notice, noticeList, 3, 5, notice_content, entity.creater, employeenotice);

                //新增
                notice.AddNotice(noticeList);
                ChatWebSocketMiddleware.SendListAsync(employeeSocket);
            }
        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> CancelAsync(bus_transfer_bill entity)
        {
            //查询用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            var result = Db.Ado.UseTran(() =>
            {
                var bill = Db.Queryable<bus_transfer_bill>().Where(w => w.bill_no == entity.bill_no).WithCache().First();
                var stateList = new List<short> { 26, 34, 36 };
                if (bill.creater_id != userInfo.id)
                {
                    throw new MessageException("非本人创建调拨单，不能取消");
                }
                if (!stateList.Contains(bill.state))
                {
                    throw new MessageException("调拨单物资已调出，不能取消");
                }
                //修改调拨单状态
                Db.Updateable<bus_transfer_bill>().SetColumns(s => new bus_transfer_bill { state = 7 }).Where(w => w.bill_no == entity.bill_no).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                //生成调拨单负单
                bill.state = 7;
                bill.delete_no = bill.bill_no;
                bill.bill_no = "-" + bill.bill_no;
                bill.creater = userInfo.name;
                bill.creater_id = userInfo.id;
                bill.create_time = DateTime.Now;
                Db.Insertable(bill).ExecuteCommand();
                redisCache.RemoveAll<bus_transfer_bill>();

                //查询调拨单明细
                var transfer_Bill_Detials = Db.Queryable<bus_transfer_bill_detials>().Where(w => w.bill_no == entity.bill_no).WithCache().ToList();

                //生成调拨单明细负单
                var del_transfer_Bill_Detials = transfer_Bill_Detials.Select(s => new bus_transfer_bill_detials { aog_num = Convert.ToInt16(0 - s.aog_num), approval_no = s.approval_no, bill_no = bill.bill_no, buy_multiple = s.buy_multiple, buy_num = 0 - s.buy_num, buy_price = s.buy_price, buy_unit = s.buy_unit, manufactor = s.manufactor, manufactor_id = s.manufactor_id, name = s.name, num = Convert.ToInt16(0 - s.num), price = s.price, remark = s.remark, spec = s.spec, std_item_id = s.std_item_id, type = s.type, type_id = s.type_id, unit = s.unit }).ToList();
                Db.Insertable(del_transfer_Bill_Detials).ExecuteCommand();
                redisCache.RemoveAll<bus_transfer_bill_detials>();

                //生成调拨单明细对应库存明细负单
                var transfer_detials_storage = Db.Queryable<bus_transfer_detials_storage>().Where(w => w.bill_no == entity.bill_no).WithCache().ToList();

                var del_transfer_detials_storage = transfer_detials_storage.Select(s => new bus_transfer_detials_storage { bill_no = bill.bill_no, expire_date = s.expire_date, id = s.id, manufactor_id = s.manufactor_id, no = s.no, num = Convert.ToInt16(0 - s.num), spec = s.spec, std_item_id = s.std_item_id }).ToList();
                Db.Insertable(del_transfer_detials_storage).ExecuteCommand();
                redisCache.RemoveAll<bus_transfer_detials_storage>();
                CancelDetials(bill, transfer_Bill_Detials, transfer_detials_storage);

                //获取固定资产ID
                var assets = Db.Queryable<bus_transfer_assets>().Where(w => w.bill_no == entity.bill_no).ToList();
                if (assets != null)
                {
                    var assets_ids = assets.Select(s => s.assets_id).ToList();
                    //修改固定资产状态
                    Db.Updateable<bus_assets>().SetColumns(s => new bus_assets { state = 30 }).Where(w => assets_ids.Contains(w.id)).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                    //生成调拨单明细对应固定资产负单
                    var del_assets = assets.Select(s => new bus_transfer_assets { assets_id = s.assets_id, bill_no = bill.bill_no, manufactor_id = s.manufactor_id, spec = s.spec, std_item_id = s.std_item_id }).ToList();
                    Db.Insertable(del_assets).ExecuteCommand();
                    redisCache.RemoveAll<bus_transfer_assets>();
                }
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 取消调拨处理相关单及库存
        /// </summary>
        /// <param name="bill"></param>
        /// <param name="transfer_Bill_Detials"></param>
        /// <param name="transfer_detials_storage"></param>
        internal void CancelDetials(bus_transfer_bill bill, List<bus_transfer_bill_detials> transfer_Bill_Detials, List<bus_transfer_detials_storage> transfer_detials_storage)
        {
            if (bill.type_id == 1)//采购
            {
                //修改采购单
                transfer_Bill_Detials.ForEach(item =>
                {
                    var num = item.num;//最小单位数量
                    decimal? bill_num = item.buy_num;//采购单位数量

                    //查询采购申请单单明细
                    var detials = Db.Queryable<bus_buy_apply_detials>().Where(w => w.apply_no == bill.apply_no && w.std_item_id == item.std_item_id && w.manufactor_id == item.manufactor_id).WithCache().First();
                    //查询采购申请单明细处理数量
                    var dispose_num = detials.dispose_num;
                    dispose_num -= item.num;
                    var transfer_num = detials.transfer_num;
                    transfer_num -= bill_num;
                    if (transfer_num < 0)
                    {
                        transfer_num = 0;
                    }
                    //修改采购申请单明细数量
                    Db.Updateable<bus_buy_apply_detials>().SetColumns(it => new bus_buy_apply_detials { dispose_num = dispose_num, transfer_num = transfer_num }).Where(w => w.apply_no == bill.apply_no && w.std_item_id == item.std_item_id && w.manufactor_id == item.manufactor_id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                });
            }

            //修改库存
            transfer_detials_storage.ForEach(item =>
            {
                var num = item.num;//最小单位数量

                //查询库存
                var storage = Db.Queryable<bus_storage>().Where(w => w.id == item.id).WithCache().First();
                //修改库存可用数量
                storage.use_num += Convert.ToInt32(num);
                Db.Updateable<bus_storage>().SetColumns(s => new bus_storage { use_num = storage.use_num }).Where(w => w.id == item.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                //查询库存明细
                var storage_detials = Db.Queryable<bus_storage_detials>().Where(w => w.id == item.id && w.no == item.no).WithCache().First();
                //修改库存明细可用数量
                storage_detials.use_num += Convert.ToInt32(num);
                Db.Updateable<bus_storage_detials>().SetColumns(s => new bus_storage_detials { use_num = storage_detials.use_num }).Where(w => w.id == item.id && w.no == item.no).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
            });

            //修改采购申请单状态
            if (bill.type_id == 1)//采购
            {
                Db.Updateable<bus_buy_apply>().SetColumns(s => new bus_buy_apply { state = 33 }).Where(w => w.apply_no == bill.apply_no).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
            }
        }

        /// <summary>
        /// 获得分页列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Page<Transfer>> GetPageAsync(TransferPageSearch entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            //-2=已审核（34、41、15、28）
            var verify_list = new List<short> { 34, 41, 15, 28 };
            //-3=申请页所有状态（26、36、34、41、15、7）
            var apply_list = new List<short> { 26, 36, 34, 41, 15, 7 };
            //-4=调拨页所有状态（34、41、15、7）
            var transfer_list = new List<short> { 34, 41, 15, 7 };
            //获取分页列表
            var pages = await Db.Queryable<bus_transfer_bill>()
                            .Where(w => w.org_id == userInfo.org_id)
                            .WhereIF(!string.IsNullOrEmpty(entity.bill_no), w => w.bill_no == entity.bill_no)
                            .WhereIF(!string.IsNullOrEmpty(entity.apply_no), w => w.apply_no == entity.apply_no)
                            .WhereIF(entity.type_id != -1, w => w.type_id == entity.type_id)
                            .WhereIF(entity.out_store_id != -1, w => w.out_store_id == entity.out_store_id)
                            .WhereIF(entity.in_store_id != -1, w => w.in_store_id == entity.in_store_id)
                            .WhereIF(entity.state >= 0, w => w.state == entity.state)
                            .WhereIF(entity.state == -2, w => verify_list.Contains(w.state))
                            .WhereIF(entity.state == -3, w => apply_list.Contains(w.state))
                            .WhereIF(entity.state == -4, w => transfer_list.Contains(w.state))
                            .WhereIF(entity.in_or_out == -1 && entity.store_id != -1, w => w.in_store_id == entity.store_id || w.out_store_id == entity.store_id)
                            .WhereIF(entity.in_or_out == 1 && entity.store_id != -1, w => w.in_store_id == entity.store_id)
                            .WhereIF(entity.in_or_out == 2 && entity.store_id != -1, w => w.out_store_id == entity.store_id)
                            .Select(s => new Transfer { bill_no = s.bill_no, org_id = s.org_id, remark = s.remark, state = s.state, creater = s.creater, creater_id = s.creater_id, type = s.type, type_id = s.type_id, level = s.level, apply_no = s.apply_no, out_store_id = s.out_store_id, await_verifier = s.await_verifier, await_verifier_id = s.await_verifier_id, create_time = s.create_time, delete_no = s.delete_no, in_employee_id = s.in_employee_id, in_employee_name = s.in_employee_name, in_store_id = s.in_store_id, in_store_name = s.in_store_name, in_time = s.in_time, out_employee_id = s.out_employee_id, out_employee_name = s.out_employee_name, out_store_name = s.out_store_name, out_time = s.out_time, total_level = s.total_level, transfer_time = s.transfer_time, verifier = s.verifier, verifier_id = s.verifier_id, verify_remark = s.verify_remark, verify_time = s.verify_time, is_org = s.is_org, process_id = s.process_id, apply_time = s.apply_time })
                            .OrderBy(entity.order + orderTypeStr)
                            .WithCache()
                            .ToPageAsync(entity.page, entity.limit);
            //列表中的调拨单号
            var billnos = pages.Items.Select(s => s.bill_no).ToList();

            //列表中的审核流程ID
            var process_ids = pages.Items.Select(s => s.process_id).ToList();

            //审核流程
            var process_list = await Db.Queryable<p_process>().Where(w => process_ids.Contains(w.id)).WithCache().ToListAsync();

            //审核流程明细
            var process_detials_list = await Db.Queryable<p_process_detials>().Where(w => process_ids.Contains(w.id)).WithCache().ToListAsync();

            //审核记录
            var verify_detials_list = await Db.Queryable<r_verify_detials>().Where(w => billnos.Contains(w.realted_no)).WithCache().ToListAsync();

            //调拨明细
            var transfer_Bill_Detials = await Db.Queryable<bus_transfer_bill_detials>().Where(w => billnos.Contains(w.bill_no)).WithCache().ToListAsync();

            //获取固定资产基础项目ID
            var assets_std_list = await Db.Queryable<p_std_item, b_codebase>((si, cb) => new object[] { JoinType.Left, si.type_id == cb.id }).Where((si, cb) => cb.property_id == 1).WithCache().Select((si, cb) => si.id).ToListAsync();

            //获取明细对应固定资产
            var assets = await Db.Queryable<bus_transfer_assets, bus_assets>((ta, a) => new object[] { JoinType.Left, ta.assets_id == a.id }).Where((ta, a) => billnos.Contains(ta.bill_no)).OrderBy((ta, a) => a.id).Select((ta, a) => new { ta, a }).WithCache().ToListAsync();

            //固定资产
            var assets_detials = transfer_Bill_Detials.Where(w => assets_std_list.Contains(w.std_item_id)).Select(s => new transfer_Bill_Detials_assets { aog_num = s.aog_num, approval_no = s.approval_no, bus_Assets = assets.Where(w => w.ta.bill_no == s.bill_no).Select(ss => ss.a).ToList(), bill_no = s.bill_no, buy_multiple = s.buy_multiple, buy_num = s.buy_num, buy_price = s.buy_price, buy_unit = s.buy_unit, manufactor = s.manufactor, manufactor_id = s.manufactor_id, name = s.name, num = s.num, price = s.price, remark = s.remark, spec = s.spec, std_item_id = s.std_item_id, type = s.type, type_id = s.type_id, unit = s.unit }).ToList();

            //非固定资产
            var detials = transfer_Bill_Detials.Where(w => !assets_std_list.Contains(w.std_item_id)).ToList();

            //赋值调拨明细
            pages.Items = pages.Items.Select(s => new Transfer { bill_no = s.bill_no, org_id = s.org_id, remark = s.remark, state = s.state, creater = s.creater, creater_id = s.creater_id, type = s.type, type_id = s.type_id, level = s.level, apply_no = s.apply_no, out_store_id = s.out_store_id, await_verifier = s.await_verifier, await_verifier_id = s.await_verifier_id, create_time = s.create_time, delete_no = s.delete_no, in_employee_id = s.in_employee_id, in_employee_name = s.in_employee_name, in_store_id = s.in_store_id, in_store_name = s.in_store_name, in_time = s.in_time, out_employee_id = s.out_employee_id, out_employee_name = s.out_employee_name, out_store_name = s.out_store_name, out_time = s.out_time, total_level = s.total_level, transfer_time = s.transfer_time, verifier = s.verifier, verifier_id = s.verifier_id, verify_remark = s.verify_remark, verify_time = s.verify_time, transfer_Bill_Detials = detials.Where(w => w.bill_no == s.bill_no).ToList(), transfer_Bill_Detials_assets = assets_detials.Where(w => w.bill_no == s.bill_no).ToList(), is_org = s.is_org, process_id = s.process_id, apply_time = s.apply_time, verifyInfos = verify(s.state, s.process_id.Value, s.bill_no, process_list, process_detials_list, verify_detials_list) }).ToList();

            return pages;
        }

        private List<verifyInfo> verify(short state, int process_id, string bill_no, List<p_process> process_list, List<p_process_detials> process_detials_list, List<r_verify_detials> verify_detials_list)
        {
            var list = new List<verifyInfo>();
            if (process_id > 0)
            {
                if (state != 30)
                {
                    //审核流程
                    var process = process_list.Where(w => w.id == process_id).FirstOrDefault();
                    //审核流程明细
                    var p_process_detials = process_detials_list.Where(w => w.id == process.id).OrderBy(o => o.level).ToList();
                    //审核记录
                    var Verify_Detials = verify_detials_list.Where(w => w.realted_no == bill_no).ToList();
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

            }
            return list;
        }

        /// <summary>
        /// 根据调拨单号获取调拨信息
        /// </summary>
        /// <param name="bill_no">调拨单号</param>
        /// <returns></returns>
        public async Task<Transfer> GetTransferAsync(string bill_no)
        {
            var transfer = await Db.Queryable<bus_transfer_bill>().Where(w => w.bill_no == bill_no).WithCache().FirstAsync();
            var transferDetials = await Db.Queryable<bus_transfer_bill_detials>().Where(w => w.bill_no == bill_no).WithCache().ToListAsync();

            //审核流程
            var process_list = await Db.Queryable<p_process>().Where(w => w.id == transfer.process_id).WithCache().ToListAsync();

            //审核流程明细
            var process_detials_list = await Db.Queryable<p_process_detials>().Where(w => w.id == transfer.process_id).WithCache().ToListAsync();

            //审核记录
            var verify_detials_list = await Db.Queryable<r_verify_detials>().Where(w => w.realted_no == transfer.bill_no).WithCache().ToListAsync();

            //获取固定资产基础项目ID
            var assets_std_list = await Db.Queryable<p_std_item, b_codebase>((si, cb) => new object[] { JoinType.Left, si.type_id == cb.id }).Where((si, cb) => cb.property_id == 1).WithCache().Select((si, cb) => si.id).ToListAsync();

            //获取明细对应固定资产

            var assets = await Db.Queryable<bus_assets>().Where(w => w.org_id == transfer.org_id && w.store_id == transfer.out_store_id && w.state == 30).OrderBy(o => o.id).WithCache().ToListAsync();

            //固定资产
            var assets_detials = transferDetials.Where(w => assets_std_list.Contains(w.std_item_id)).Select(s => new transfer_Bill_Detials_assets { aog_num = s.aog_num, approval_no = s.approval_no, bus_Assets = assets.Where(w => w.std_item_id == s.std_item_id && w.spec == s.spec && w.manufactor_id == s.manufactor_id).ToList(), bill_no = s.bill_no, buy_multiple = s.buy_multiple, buy_num = s.buy_num, buy_price = s.buy_price, buy_unit = s.buy_unit, manufactor = s.manufactor, manufactor_id = s.manufactor_id, name = s.name, num = s.num, price = s.price, remark = s.remark, spec = s.spec, std_item_id = s.std_item_id, type = s.type, type_id = s.type_id, unit = s.unit }).ToList();

            //非固定资产
            var detials = transferDetials.Where(w => !assets_std_list.Contains(w.std_item_id)).ToList();

            return new Transfer { apply_no = transfer.apply_no, await_verifier = transfer.await_verifier, await_verifier_id = transfer.await_verifier_id, bill_no = transfer.bill_no, creater = transfer.creater, creater_id = transfer.creater_id, create_time = transfer.create_time, delete_no = transfer.delete_no, in_employee_id = transfer.in_employee_id, in_employee_name = transfer.in_employee_name, in_store_id = transfer.in_store_id, in_store_name = transfer.in_store_name, in_time = transfer.in_time, level = transfer.level, org_id = transfer.org_id, out_employee_id = transfer.out_employee_id, out_employee_name = transfer.out_employee_name, out_store_id = transfer.out_store_id, out_store_name = transfer.out_store_name, out_time = transfer.out_time, remark = transfer.remark, state = transfer.state, total_level = transfer.total_level, transfer_time = transfer.transfer_time, type = transfer.type, type_id = transfer.type_id, verifier = transfer.verifier, verifier_id = transfer.verifier_id, verify_remark = transfer.verify_remark, verify_time = transfer.verify_time, transfer_Bill_Detials = detials, transfer_Bill_Detials_assets = assets_detials, is_org = transfer.is_org, process_id = transfer.process_id, apply_time = transfer.apply_time, verifyInfos = verify(transfer.state, transfer.process_id.Value, transfer.bill_no, process_list, process_detials_list, verify_detials_list) };
        }

        /// <summary>
        /// 确认调入
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> VerifyInAsync(bus_transfer_bill entity)
        {
            //查询用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            var result = Db.Ado.UseTran(() =>
            {
                var bill = Db.Queryable<bus_transfer_bill>().Where(w => w.bill_no == entity.bill_no).WithCache().First();

                //修改调拨单状态
                Db.Updateable<bus_transfer_bill>().SetColumns(s => new bus_transfer_bill { state = 15 }).Where(w => w.bill_no == entity.bill_no).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                //查询调拨单明细
                var transfer_Bill_Detials = Db.Queryable<bus_transfer_detials_storage, bus_transfer_bill_detials>((tds, tbd) => new object[] { JoinType.Left, tds.bill_no == tbd.bill_no && tds.std_item_id == tbd.std_item_id && tds.manufactor_id == tbd.manufactor_id && tds.spec == tbd.spec }).Where((tds, tbd) => tds.bill_no == entity.bill_no).Select((tds, tbd) => new { tbd.bill_no, tbd.std_item_id, tbd.name, tbd.type, tbd.type_id, tbd.spec, tbd.buy_multiple, tbd.buy_price, tbd.buy_unit, tbd.unit, tds.num, tbd.price, tbd.approval_no, tbd.manufactor, tbd.manufactor_id, tbd.remark, tds.expire_date, tds.buy_date }).WithCache().ToList();

                //入库
                var putInStorage = new PutInStorage();
                putInStorage.realted_no = entity.bill_no;
                putInStorage.store_id = bill.in_store_id;
                putInStorage.store_name = bill.in_store_name;
                putInStorage.type_id = 2;
                putInStorage.type = "调拨";
                putInStorage.remark = "调拨入库";
                putInStorage.put_In_Storage_Detials = transfer_Bill_Detials.Select(s => new bus_put_in_storage_detials { approval_no = s.approval_no, num = s.num, buy_num = Convert.ToDecimal(s.num / s.buy_multiple), buy_multiple = s.buy_multiple, buy_price = s.buy_price, buy_unit = s.buy_unit, expire_date = s.expire_date, manufactor = s.manufactor, manufactor_id = s.manufactor_id, name = s.name, price = s.price.Value, spec = s.spec, std_item_id = s.std_item_id, type = s.type, type_id = s.type_id, unit = s.unit, buy_date = s.buy_date.Value }).ToList();


                //查询调拨固定资产
                var transfer_assets = Db.Queryable<bus_transfer_assets>().Where(w => w.bill_no == entity.bill_no).WithCache().ToList();
                //入库固定资产
                var put_in_assets = transfer_assets.Select(s => new bus_put_in_assets { bill_no = "", assets_id = s.assets_id, manufactor_id = s.manufactor_id, spec = s.spec, std_item_id = s.std_item_id }).ToList();
                putInStorage.put_In_Assets = put_in_assets;

                var putIn = new PutInStorageService();
                putIn.PutIn(putInStorage);
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 确认调出
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> VerifyOutAsync(Transfer entity)
        {
            //查询用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            var result = Db.Ado.UseTran(() =>
            {
                var bill = Db.Queryable<bus_transfer_bill>().Where(w => w.bill_no == entity.bill_no).WithCache().First();

                //修改调拨单状态
                Db.Updateable<bus_transfer_bill>().SetColumns(s => new bus_transfer_bill { state = 41, transfer_time = DateTime.Now }).Where(w => w.bill_no == entity.bill_no).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                //查询调拨单明细
                var transfer_Bill_Detials = Db.Queryable<bus_transfer_detials_storage, bus_transfer_bill_detials>((tds, tbd) => new object[] { JoinType.Left, tds.bill_no == tbd.bill_no && tds.std_item_id == tbd.std_item_id && tds.manufactor_id == tbd.manufactor_id && tds.spec == tbd.spec }).Where((tds, tbd) => tds.bill_no == entity.bill_no).Select((tds, tbd) => new { tbd.bill_no, tbd.std_item_id, tbd.name, tbd.type, tbd.type_id, tbd.spec, tbd.buy_multiple, tbd.buy_num, tbd.buy_price, tbd.buy_unit, tbd.unit, tds.num, tbd.aog_num, tbd.price, tbd.approval_no, tbd.manufactor, tbd.manufactor_id, tbd.remark, tds.expire_date, tds.id, tds.no, tds.buy_date }).WithCache().ToList();

                //出库单号
                var bill_no = "CK" + DateTime.Now.ToString("yyMMdd");
                //查询最大单号
                var max_no = Db.Queryable<bus_out_storage>().Where(w => w.bill_no.StartsWith(bill_no)).OrderBy(o => o.bill_no, OrderByType.Desc).Select(s => s.bill_no).WithCache().First();
                if (max_no == null)
                {
                    bill_no += "0001";
                }
                else
                {
                    max_no = max_no.Substring(bill_no.Length);
                    bill_no += (int.Parse(max_no) + 1).ToString().PadLeft(4, '0');
                }

                //生成出库单
                var out_storage = new bus_out_storage { bill_no = bill_no, creater = userInfo.name, creater_id = userInfo.id, create_time = DateTime.Now, org_id = userInfo.org_id, out_time = DateTime.Now, realted_no = entity.bill_no, remark = "调拨出库", state = 42, store_id = bill.out_store_id, store_name = bill.out_store_name, type = "调拨", type_id = 1 };
                Db.Insertable(out_storage).ExecuteCommand();
                redisCache.RemoveAll<bus_out_storage>();

                //生成出库单明细
                var out_storage_detials = transfer_Bill_Detials.Select(s => new bus_out_storage_detials { approval_no = s.approval_no, bill_no = bill_no, bill_num = Convert.ToInt32(s.num), buy_multiple = s.buy_multiple, buy_price = s.buy_price, buy_unit = s.buy_unit, expire_date = s.expire_date, manufactor = s.manufactor, manufactor_id = s.manufactor_id, name = s.name, price = s.price.Value, spec = s.spec, std_item_id = s.std_item_id, storage_id = s.id, storage_no = s.no, type = s.type, type_id = s.type_id, unit = s.unit, buy_date = s.buy_date }).ToList();
                Db.Insertable(out_storage_detials).ExecuteCommand();
                redisCache.RemoveAll<bus_out_storage_detials>();

                //修改库存
                transfer_Bill_Detials.ForEach(item =>
                {
                    var num = item.num;//最小单位数量

                    //查询库存
                    var storage = Db.Queryable<bus_storage>().Where(w => w.id == item.id).WithCache().First();
                    //修改库存可用数量
                    storage.num -= Convert.ToInt32(num);
                    Db.Updateable<bus_storage>().SetColumns(s => new bus_storage { num = storage.num }).Where(w => w.id == item.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                    //查询库存明细
                    var storage_detials = Db.Queryable<bus_storage_detials>().Where(w => w.id == item.id && w.no == item.no).WithCache().First();
                    //修改库存明细可用数量
                    storage_detials.num -= Convert.ToInt32(num);
                    Db.Updateable<bus_storage_detials>().SetColumns(s => new bus_storage_detials { num = storage_detials.num }).Where(w => w.id == item.id && w.no == item.no).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                });

                //调拨固定资产
                var transfer_assets = new List<bus_transfer_assets>();
                if (bill.type_id == 1)//采购
                {
                    //查询调拨固定资产
                    transfer_assets = Db.Queryable<bus_transfer_assets>().Where(w => w.bill_no == entity.bill_no).WithCache().ToList();
                }
                else
                {
                    if (entity.transfer_Bill_Detials_assets != null)
                    {
                        var assets_ids = new List<int>();
                        entity.transfer_Bill_Detials_assets.ForEach(item =>
                        {
                            if (item.num != item.assets_ids.Count)
                            {
                                throw new MessageException("选择的固定资产数量不正确");
                            }
                            item.assets_ids.ForEach(id =>
                            {
                                assets_ids.Add(id);
                                transfer_assets.Add(new bus_transfer_assets { assets_id = id, bill_no = entity.bill_no, manufactor_id = item.manufactor_id, spec = item.spec, std_item_id = item.std_item_id });
                            });
                        });
                        //修改固定资产状态
                        Db.Updateable<bus_assets>().SetColumns(s => new bus_assets { state = 41 }).Where(w => assets_ids.Contains(w.id)).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                        //调拨固定资产
                        Db.Insertable(transfer_assets).ExecuteCommand();
                        redisCache.RemoveAll<bus_transfer_assets>();
                    }
                }
                //出库固定资产
                var out_assets = transfer_assets.Select(s => new bus_out_assets { bill_no = bill_no, assets_id = s.assets_id, manufactor_id = s.manufactor_id, spec = s.spec, std_item_id = s.std_item_id }).ToList();
                Db.Insertable(out_assets).ExecuteCommand();
                redisCache.RemoveAll<bus_out_assets>();
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 编辑调拨单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> ModifyAsync(Transfer entity)
        {
            entity.state = 26;
            return await ModifyTransfer(entity);
        }

        /// <summary>
        /// 编辑调拨单（草稿）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> ModifyDraftAsync(Transfer entity)
        {
            entity.state = 30;
            return await ModifyTransfer(entity);
        }

        private async Task<bool> ModifyTransfer(Transfer entity)
        {
            //查询用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            var result = Db.Ado.UseTran(() =>
            {
                if (entity.out_store_id == 0)
                {
                    var org = Db.Queryable<p_org>().Where(w => w.id == userInfo.org_id).WithCache().First();
                    entity.out_store_name = org.name;
                }

                //修改调拨单
                Db.Updateable<bus_transfer_bill>().SetColumns(s => new bus_transfer_bill { out_store_id = entity.out_store_id, out_store_name = entity.out_store_name, remark = entity.remark, state = entity.state }).Where(w => w.bill_no == entity.bill_no).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                var bill = Db.Queryable<bus_transfer_bill>().Where(w => w.bill_no == entity.bill_no).WithCache().First();

                if (entity.state != 30)
                {
                    //审核相关
                    SetProcess(bill);

                    //修改调拨单
                    Db.Updateable<bus_transfer_bill>().SetColumns(s => new bus_transfer_bill { apply_time = DateTime.Now, total_level = bill.total_level, level = bill.level, process_id = bill.process_id, await_verifier_id = bill.await_verifier_id, await_verifier = bill.await_verifier, is_org = bill.is_org }).Where(w => w.bill_no == entity.bill_no).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                }

                //修改前调拨明细
                var transfer_Bill_Detials = Db.Queryable<bus_transfer_bill_detials>().Where(w => w.bill_no == entity.bill_no).WithCache().ToList();

                //删除调拨明细
                Db.Deleteable<bus_transfer_bill_detials>().Where(w => w.bill_no == entity.bill_no).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                //修改前对应调拨明细库存
                var transfer_detials_storage = Db.Queryable<bus_transfer_detials_storage>().Where(w => w.bill_no == entity.bill_no).WithCache().ToList();

                //删除对应调拨明细库存
                Db.Deleteable<bus_transfer_detials_storage>().Where(w => w.bill_no == entity.bill_no).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                //取消修改关联单及库存
                CancelDetials(bill, transfer_Bill_Detials, transfer_detials_storage);

                //调拨明细、相关单及库存操作
                Detials(entity.transfer_Bill_Detials, entity.transfer_Bill_Detials_assets, userInfo, bill);
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> CommitAsync(Transfer entity)
        {
            //查询调拨单
            var bill = await Db.Queryable<bus_transfer_bill>().Where(w => w.bill_no == entity.bill_no).WithCache().FirstAsync();

            //审核相关
            SetProcess(bill);

            //修改调拨单
            return await Db.Updateable<bus_transfer_bill>().SetColumns(s => new bus_transfer_bill { state = 26, apply_time = DateTime.Now, total_level = bill.total_level, level = bill.level, process_id = bill.process_id, await_verifier_id = bill.await_verifier_id, await_verifier = bill.await_verifier, is_org = bill.is_org }).Where(w => w.bill_no == entity.bill_no).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
        }

        /// <summary>
        /// 删除草稿
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(bus_transfer_bill entity)
        {
            //查询用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            var result = Db.Ado.UseTran(() =>
            {
                var bill = Db.Queryable<bus_transfer_bill>().Where(w => w.bill_no == entity.bill_no).WithCache().First();
                if (bill.creater_id != userInfo.id)
                {
                    throw new MessageException("非本人创建调拨单，不能删除");
                }
                if (bill.state != 30)
                {
                    throw new MessageException("非草稿调拨单，不能删除");
                }

                //查询调拨单明细
                var transfer_Bill_Detials = Db.Queryable<bus_transfer_bill_detials>().Where(w => w.bill_no == entity.bill_no).WithCache().ToList();

                //调拨单明细对应库存明细
                var transfer_detials_storage = Db.Queryable<bus_transfer_detials_storage>().Where(w => w.bill_no == entity.bill_no).WithCache().ToList();

                CancelDetials(bill, transfer_Bill_Detials, transfer_detials_storage);

                //删除调拨单
                Db.Deleteable<bus_transfer_bill>().Where(w => w.bill_no == entity.bill_no).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                //删除调拨单明细对应库存明细
                Db.Deleteable<bus_transfer_detials_storage>().Where(w => w.bill_no == entity.bill_no).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                //获取固定资产ID
                var assets_ids = Db.Queryable<bus_transfer_assets>().Where(w => w.bill_no == entity.bill_no).Select(s => s.assets_id).ToList();
                //修改固定资产状态
                Db.Updateable<bus_assets>().SetColumns(s => new bus_assets { state = 30 }).Where(w => assets_ids.Contains(w.id)).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                //删除调拨单明细对应固定资产
                Db.Deleteable<bus_transfer_assets>().Where(w => w.bill_no == entity.bill_no).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
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
                throw new MessageException("请选择调拨单进行导出");
            }
            var billnos = No.Split(',').ToList();

            var bills = await Db.Queryable<bus_transfer_bill>()
                            .Where(w => billnos.Contains(w.bill_no))
                            .WithCache().ToListAsync();

            //调拨明细
            var transfer_Bill_Detials = await Db.Queryable<bus_transfer_bill_detials>().Where(w => billnos.Contains(w.bill_no)).WithCache().ToListAsync();

            //获取固定资产基础项目ID
            var assets_std_list = await Db.Queryable<p_std_item, b_codebase>((si, cb) => new object[] { JoinType.Left, si.type_id == cb.id }).Where((si, cb) => cb.property_id == 1).WithCache().Select((si, cb) => si.id).ToListAsync();

            //获取明细对应固定资产
            var assets = await Db.Queryable<bus_transfer_assets, bus_assets>((ta, a) => new object[] { JoinType.Left, ta.assets_id == a.id }).Where((ta, a) => billnos.Contains(ta.bill_no)).OrderBy((ta, a) => a.id).Select((ta, a) => new { ta, a }).WithCache().ToListAsync();

            //固定资产
            var assets_detials = transfer_Bill_Detials.Where(w => assets_std_list.Contains(w.std_item_id)).Select(s => new transfer_Bill_Detials_assets { aog_num = s.aog_num, approval_no = s.approval_no, bus_Assets = assets.Where(w => w.ta.bill_no == s.bill_no).Select(ss => ss.a).ToList(), bill_no = s.bill_no, buy_multiple = s.buy_multiple, buy_num = s.buy_num, buy_price = s.buy_price, buy_unit = s.buy_unit, manufactor = s.manufactor, manufactor_id = s.manufactor_id, name = s.name, num = s.num, price = s.price, remark = s.remark, spec = s.spec, std_item_id = s.std_item_id, type = s.type, type_id = s.type_id, unit = s.unit }).ToList();

            //非固定资产
            var detials = transfer_Bill_Detials.Where(w => !assets_std_list.Contains(w.std_item_id)).ToList();

            //定义表头
            var headers = new List<string>() { "序号", "名称", "规格", "分类", "厂家", "数量", "备注" };

            string sWebRootFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot\\tempExcel");
            if (!Directory.Exists(sWebRootFolder))
            {
                Directory.CreateDirectory(sWebRootFolder);
            }

            //查询用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            string sFileName = $@"transfer_{userInfo.id}.xlsx";
            var path = Path.Combine(sWebRootFolder, sFileName);
            FileInfo file = new FileInfo(path);
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(path);
            }
            using (ExcelPackage package = new ExcelPackage(file))
            {
                bills.ForEach(item =>
                {
                    //创建sheet
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add($"{item.bill_no}");
                    worksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[1, 1, 2, 7].Merge = true;
                    worksheet.Cells[1, 1].Value = "调拨单";
                    worksheet.Cells[1, 1].Style.Font.Size = 20;
                    worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[3, 1, 3, 4].Merge = true;
                    worksheet.Cells[3, 1].Value = $"单号：{item.bill_no}";
                    worksheet.Cells[3, 5, 3, 7].Merge = true;
                    worksheet.Cells[3, 5].Value = $"类型：{item.type}";
                    worksheet.Cells[4, 1, 4, 4].Merge = true;
                    worksheet.Cells[4, 1].Value = $"调出门店：{item.out_store_name}";
                    worksheet.Cells[4, 5, 4, 7].Merge = true;
                    worksheet.Cells[4, 5].Value = $"调入门店：{item.in_store_name}";
                    worksheet.Cells[5, 1, 5, 4].Merge = true;
                    worksheet.Cells[5, 1].Value = $"创建人：{item.creater}";
                    worksheet.Cells[5, 5, 5, 7].Merge = true;
                    worksheet.Cells[5, 5].Value = $"创建时间：{item.create_time.ToString("yyyy年MM月dd日 HH:mm:ss")}";
                    worksheet.Cells[6, 1, 6, 7].Merge = true;
                    worksheet.Cells[6, 1].Value = $"备注：{item.remark}";

                    worksheet.Cells[7, 1, 7, 7].Merge = true;
                    worksheet.Cells[7, 1].Value = "普通物资";
                    worksheet.Cells[7, 1].Style.Font.Bold = true;
                    worksheet.Cells[7, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    //表头字段
                    for (int i = 0; i < headers.Count; i++)
                    {
                        worksheet.Cells[8, i + 1].Value = headers[i];
                    }
                    var row = 9;

                    //非固定资产
                    var dataList = detials.Where(w => w.bill_no == item.bill_no).ToList();
                    for (int i = 0; i < dataList.Count; i++)
                    {
                        worksheet.Cells[row, 1].Value = i + 1;
                        worksheet.Cells[row, 2].Value = dataList[i].name;
                        worksheet.Cells[row, 3].Value = dataList[i].spec;
                        worksheet.Cells[row, 4].Value = dataList[i].type;
                        worksheet.Cells[row, 5].Value = dataList[i].manufactor;
                        worksheet.Cells[row, 6].Value = $"{dataList[i].buy_num}{dataList[i].buy_unit}={dataList[i].num}{dataList[i].unit}";
                        worksheet.Cells[row, 7].Value = dataList[i].remark;
                        row++;
                    }

                    worksheet.Cells[row, 1, row, 7].Merge = true;
                    worksheet.Cells[row, 1].Style.Font.Bold = true;
                    worksheet.Cells[row, 1].Value = "固定资产";
                    worksheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    row++;

                    //固定资产
                    var dataList_assets = assets_detials.Where(w => w.bill_no == item.bill_no).ToList();
                    for (int i = 0; i < dataList_assets.Count; i++)
                    {
                        worksheet.Cells[row, 1, row, 4].Merge = true;
                        worksheet.Cells[row, 1].Value = $"{dataList_assets[i].name}~{dataList_assets[i].spec}~{dataList_assets[i].manufactor}";

                        worksheet.Cells[row, 5, row, 7].Merge = true;
                        worksheet.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells[row, 5].Value = $"调拨数量：{dataList_assets[i].buy_num}{dataList_assets[i].buy_unit}={dataList_assets[i].num}{dataList_assets[i].unit}";
                        row++;

                        worksheet.Cells[row, 1, row, 7].Merge = true;
                        worksheet.Cells[row, 1].Value = $"资产编号：{string.Join(',', dataList_assets[i].bus_Assets.Select(s => s.no).ToArray())}";
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
