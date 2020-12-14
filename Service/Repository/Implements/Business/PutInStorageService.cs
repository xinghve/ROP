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

namespace Service.Repository.Implements.Business
{
    /// <summary>
    /// 入库
    /// </summary>
    public class PutInStorageService : DbContext, IPutInStorageService
    {
        /// <summary>
        /// 入库
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> AddAsync(PutInStorage entity)
        {
            var result = await Db.Ado.UseTranAsync(() =>
            {
                PutIn(entity);
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 入库
        /// </summary>
        /// <param name="entity"></param>
        public void PutIn(PutInStorage entity)
        {
            //查询用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var bill = new bus_put_in_storage();
            bill.bill_no = "RK" + DateTime.Now.ToString("yyMMdd");
            //查询最大单号
            var max_no = Db.Queryable<bus_put_in_storage>().Where(w => w.bill_no.StartsWith(bill.bill_no)).OrderBy(o => o.bill_no, OrderByType.Desc).Select(s => s.bill_no).WithCache().First();
            if (max_no == null)
            {
                bill.bill_no += "0001";
            }
            else
            {
                max_no = max_no.Substring(bill.bill_no.Length);
                bill.bill_no += (int.Parse(max_no) + 1).ToString().PadLeft(4, '0');
            }
            bill.realted_no = entity.realted_no;
            bill.org_id = userInfo.org_id;
            bill.store_id = entity.store_id;
            bill.store_name = entity.store_name;
            bill.type_id = entity.type_id;
            bill.type = entity.type;
            bill.state = 40;
            bill.remark = entity.remark;
            bill.creater = userInfo.name;
            bill.creater_id = userInfo.id;
            bill.put_in_time = DateTime.Now;

            //添加入库单返回ID
            var command_num = Db.Insertable(bill).ExecuteCommand();
            redisCache.RemoveAll<bus_put_in_storage>();
            if (command_num <= 0)
            {
                throw new MessageException("入库失败！");
            }

            List<bus_put_in_assets> put_in_assets = new List<bus_put_in_assets>();

            //添加入库单明细及库存，修改关联单
            if (entity.put_In_Storage_Detials.Count > 0)
            {
                //查询基础项目
                var std_item_list = Db.Queryable<p_std_item>().WithCache().ToList();

                //添加入库单明细
                var put_In_Storage_Detials = entity.put_In_Storage_Detials.Select((s, index) => new bus_put_in_storage_detials { bill_no = bill.bill_no, name = s.name, spec = s.spec, std_item_id = s.std_item_id, unit = s.unit, approval_no = s.approval_no, buy_multiple = s.buy_multiple, buy_price = s.buy_price, buy_unit = s.buy_unit, expire_date = s.expire_date, manufactor = s.manufactor, manufactor_id = s.manufactor_id, price = s.price, type = std_item_list.Where(w => w.id == s.std_item_id).FirstOrDefault().type, type_id = std_item_list.Where(w => w.id == s.std_item_id).FirstOrDefault().type_id.Value, buy_date = s.buy_date == null ? DateTime.Now.Date : s.buy_date, num = s.num == 0 ? Convert.ToInt16(s.buy_num * s.buy_multiple) : s.num, buy_num = s.buy_num == 0 ? Math.Round(Convert.ToDecimal(s.num) / s.buy_multiple.Value, 2) : s.buy_num, no = Convert.ToInt16(index) }).ToList();
                Db.Insertable(put_In_Storage_Detials).ExecuteCommand();
                redisCache.RemoveAll<bus_put_in_storage_detials>();

                //获取固定资产基础项目ID
                var assets_std_list = Db.Queryable<p_std_item, b_codebase>((si, cb) => new object[] { JoinType.Left, si.type_id == cb.id }).Where((si, cb) => cb.property_id == 1).WithCache().Select((si, cb) => new { si.id, cb.year }).ToList();

                if (assets_std_list.Any(w => w.year <= 0))
                {
                    throw new MessageException("请先设置固定资产类型使用年限");
                }

                var assets_std_ids = assets_std_list.Select(s => s.id).ToList();

                //固定资产列表
                var assetsList = new List<bus_assets>();

                //添加库存，修改关联单
                var storage_no = 1;
                put_In_Storage_Detials.ForEach(item =>
                {
                    var num = Convert.ToInt32(item.num);//最小单位数量
                    decimal bill_num = item.buy_num;//采购单位数量
                    if (entity.type_id == 1)//采购
                    {
                        item.buy_date = DateTime.Now.Date;
                        //查询采购单明细
                        var detials = Db.Queryable<bus_buy_bill_detials>().Where(w => w.bill_no == entity.realted_no && w.std_item_id == item.std_item_id).WithCache().First();
                        //查询采购单明细到货数量
                        var aog_num = detials.aog_num;
                        aog_num = short.Parse((aog_num.Value + bill_num).ToString());
                        if (aog_num > detials.num)
                        {
                            throw new MessageException("到货数量大于采购数量，请确认");
                        }
                        //修改采购单明细到货数量
                        Db.Updateable<bus_buy_bill_detials>().SetColumns(it => new bus_buy_bill_detials { aog_num = aog_num }).Where(w => w.bill_no == entity.realted_no && w.std_item_id == item.std_item_id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                        //修改采购单状态
                        var isExiste = Db.Queryable<bus_buy_bill_detials>().WithCache().Any(a => a.bill_no == entity.realted_no && a.num != a.aog_num);
                        if (!isExiste)
                        {
                            BuyBillService buyBillService = new BuyBillService();
                            buyBillService.Finish(entity.realted_no, 15);
                        }
                        else
                        {
                            Db.Updateable<bus_buy_bill>().SetColumns(s => new bus_buy_bill { state = 43 }).Where(w => w.bill_no == entity.realted_no).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                        }
                    }
                    else if (entity.type_id == 2)//调拨
                    {

                    }
                    else if (entity.type_id == 3)//盘点
                    {

                    }
                    else if (entity.type_id == 4)//退货
                    {

                    }
                    else if (entity.type_id == 99)//其他
                    {

                    }
                    var storage = Db.Queryable<bus_storage>().Where(w => w.org_id == userInfo.org_id && w.store_id == entity.store_id && w.std_item_id == item.std_item_id).WithCache().First();
                    //添加库存返回ID
                    var id = 0;
                    if (storage == null)
                    {
                        storage = new bus_storage { min_num = std_item_list.Where(w => w.id == item.std_item_id).FirstOrDefault().min_num, name = item.name, num = num, org_id = userInfo.org_id, std_item_id = item.std_item_id, store_id = entity.store_id, store_name = entity.store_name, type = item.type, type_id = item.type_id, unit = item.unit, use_num = num };
                        //添加库存返回ID
                        id = Db.Insertable(storage).ExecuteReturnIdentity();
                        redisCache.RemoveAll<bus_storage>();
                    }
                    else
                    {
                        //修改库存数量
                        id = storage.id;
                        var this_num = storage.num + num;
                        var this_use_num = storage.use_num + num;
                        Db.Updateable<bus_storage>().SetColumns(s => new bus_storage { num = this_num, use_num = this_use_num }).Where(w => w.id == id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                        var max = Db.Queryable<bus_storage_detials>().Where(w => w.id == id).OrderBy(o => o.no, OrderByType.Desc).Select(s => s.no).First();
                        if (max != null)
                        {
                            storage_no = int.Parse(max.Substring(6 + id.ToString().Length)) + 1;
                        }
                    }

                    //添加库存明细
                    var no = DateTime.Now.ToString("yyMMdd") + id + storage_no;
                    var storage_detials = new bus_storage_detials { approval_no = item.approval_no, bill_no = bill.bill_no, bill_num = Math.Round(bill_num, 2), buy_multiple = item.buy_multiple, buy_price = item.buy_price, buy_unit = item.buy_unit, expire_date = item.expire_date, id = id, manufactor = item.manufactor, manufactor_id = item.manufactor_id, name = item.name, no = no, num = num, price = item.price, spec = item.spec, std_item_id = item.std_item_id, type = item.type, type_id = item.type_id, unit = item.unit, use_num = num, put_in_type_id = entity.type_id, put_in_type = entity.type, put_in_num = num, buy_date = item.buy_date };

                    //添加库存明细
                    Db.Insertable(storage_detials).ExecuteCommand();
                    redisCache.RemoveAll<bus_storage_detials>();


                    if (assets_std_ids.Contains(item.std_item_id))
                    {
                        if (entity.type_id == 1 || entity.type_id == 99)//采购添加固定资产
                        {
                            //固定资产
                            for (int i = 1; i <= item.num; i++)
                            {
                                assetsList.Add(new bus_assets { bill_no = item.bill_no, buy_date = item.buy_date.Value, buy_price = item.buy_price, manufactor = item.manufactor, manufactor_id = item.manufactor_id, name = item.name, no = $"{item.type_id}{bill.bill_no.Replace("RK", "")}{item.no}{i}", org_id = userInfo.org_id, price = item.price, spec = item.spec, state = 30, std_item_id = item.std_item_id, store_id = 0, type = item.type, type_id = item.type_id, unit = item.unit, net_salvage_rate = 5, net_salvage = item.buy_price.Value * 5 / 100, total_depreciation = 0, depreciation = item.buy_price.Value * 95 / 100, remaining_depreciation = item.buy_price.Value * 95 / 100, net_residual = item.buy_price.Value, month_depreciation = 95 / assets_std_list.Where(w => w.id == item.std_item_id).FirstOrDefault().year / 100 / 12 * item.buy_price.Value });//storage_id = storage_detials.id, storage_no = storage_detials.no,
                            }
                        }
                    }

                    storage_no += 1;
                });

                //添加固定资产
                Db.Insertable(assetsList).ExecuteCommand();
                redisCache.RemoveAll<bus_assets>();

                if (entity.type_id == 1 || entity.type_id == 99)//采购添加固定资产
                {
                    var assets = Db.Queryable<bus_assets>().Where(w => w.bill_no == bill.bill_no).OrderBy(o => o.no).WithCache().ToList();
                    assets.ForEach(item =>
                    {
                        put_in_assets.Add(new bus_put_in_assets { assets_id = item.id, bill_no = item.bill_no, manufactor_id = item.manufactor_id, spec = item.spec, std_item_id = item.std_item_id });
                    });
                }
                if (entity.put_In_Assets != null)
                {
                    entity.put_In_Assets.ForEach(item =>
                    {
                        put_in_assets.Add(new bus_put_in_assets { assets_id = item.assets_id, bill_no = bill.bill_no, manufactor_id = item.manufactor_id, spec = item.spec, std_item_id = item.std_item_id });
                    });
                }
            }

            //添加固定资产入库
            Db.Insertable(put_in_assets).ExecuteCommand();
            redisCache.RemoveAll<bus_put_in_assets>();
            //修改固定资产状态
            var assets_ids = put_in_assets.Select(s => s.assets_id).ToList();
            Db.Updateable<bus_assets>().SetColumns(s => new bus_assets { state = 30, store_id = bill.store_id, store = bill.store_name, address = bill.store_name, dept_id = 0, dept = "", use_employee = "", use_employee_id = 0 }).Where(w => assets_ids.Contains(w.id)).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> CancelAsync(bus_put_in_storage entity)
        {
            //查询用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            var result = await Db.Ado.UseTranAsync(() =>
            {
                //修改入库状态
                Db.Updateable<bus_put_in_storage>().SetColumns(s => new bus_put_in_storage { state = 7 }).Where(w => w.bill_no == entity.bill_no).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                //查询入库信息
                var put_In_Storage = Db.Queryable<bus_put_in_storage>().Where(w => w.bill_no == entity.bill_no).WithCache().First();
                //查询入库明细
                var put_In_Storage_Detials = Db.Queryable<bus_put_in_storage_detials>().Where(w => w.bill_no == entity.bill_no).WithCache().ToList();

                //添加作废负单
                put_In_Storage.delete_no = put_In_Storage.bill_no;
                put_In_Storage.bill_no = "-" + put_In_Storage.bill_no;
                put_In_Storage.creater = userInfo.name;
                put_In_Storage.creater_id = userInfo.id;
                put_In_Storage.put_in_time = DateTime.Now;
                put_In_Storage.state = 7;
                Db.Insertable(put_In_Storage).ExecuteCommand();
                redisCache.RemoveAll<bus_put_in_storage>();
                //添加负单明细
                var f_list = put_In_Storage_Detials.Select((s, index) => new bus_put_in_storage_detials { bill_no = put_In_Storage.bill_no, num = Convert.ToInt16(0 - s.num), buy_num = 0 - s.num, buy_date = s.buy_date, expire_date = s.expire_date, type = s.type, type_id = s.type_id, approval_no = s.approval_no, buy_multiple = s.buy_multiple, buy_price = s.buy_price, buy_unit = s.buy_unit, manufactor = s.manufactor, manufactor_id = s.manufactor_id, name = s.name, price = s.price, spec = s.spec, std_item_id = s.std_item_id, unit = s.unit, no = Convert.ToInt16(index) }).ToList();
                Db.Insertable(f_list).ExecuteCommand();
                redisCache.RemoveAll<bus_put_in_storage_detials>();

                if (put_In_Storage.type_id == 1)//采购
                {
                    //查询采购单
                    var buy_bill = Db.Queryable<bus_buy_bill>().Where(w => w.bill_no == put_In_Storage.realted_no).WithCache().First();
                    if (buy_bill.state == 15)
                    {
                        throw new MessageException("对应采购单已确认完成，不能取消入库");
                    }
                    //查询采购明细
                    var buy_detials = Db.Queryable<bus_buy_bill_detials>().Where(w => w.bill_no == put_In_Storage.realted_no).WithCache().ToList();
                    put_In_Storage_Detials.ForEach(item =>
                    {
                        var this_buy_detials = buy_detials.Where(w => w.std_item_id == item.std_item_id).FirstOrDefault();
                        var aog_num = this_buy_detials.aog_num - item.buy_num;
                        //修改采购单明细到货数量
                        Db.Updateable<bus_buy_bill_detials>().SetColumns(it => new bus_buy_bill_detials { aog_num = aog_num }).Where(w => w.bill_no == this_buy_detials.bill_no && w.std_item_id == this_buy_detials.std_item_id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                    });
                }
                else if (put_In_Storage.type_id == 2)//调拨
                {
                    //修改调拨单状态
                    Db.Updateable<bus_transfer_bill>().SetColumns(s => new bus_transfer_bill { state = 41 }).Where(w => w.bill_no == put_In_Storage.realted_no).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                }
                else if (put_In_Storage.type_id == 3)//盘点
                {

                }
                else if (put_In_Storage.type_id == 4)//退货
                {

                }

                //查询库存明细
                var storage_detials = Db.Queryable<bus_storage_detials>().Where(w => w.bill_no == entity.bill_no).WithCache().ToList();
                //查询库存id
                var ids = storage_detials.Select(s => s.id).Distinct().ToList();
                //查询库存
                var storages = Db.Queryable<bus_storage>().Where(w => ids.Contains(w.id)).WithCache().ToList();
                ids.ForEach(id =>
                {
                    var put_in_num = storage_detials.Where(w => w.id == id).Sum(s => s.put_in_num);
                    var this_storage = storages.Where(w => w.id == id).FirstOrDefault();
                    this_storage.use_num -= put_in_num;
                    this_storage.num -= put_in_num;
                    if (this_storage.use_num < 0 || this_storage.num < 0)
                    {
                        throw new MessageException("库存数量不足，不能取消");
                    }
                    //修改库存数量
                    Db.Updateable<bus_storage>().SetColumns(s => new bus_storage { num = this_storage.num, use_num = this_storage.use_num }).Where(w => w.id == id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                });
                //删除库存明细
                Db.Deleteable<bus_storage_detials>().Where(w => w.bill_no == entity.bill_no).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                //获取固定资产ID
                var assets = Db.Queryable<bus_put_in_assets>().Where(w => w.bill_no == entity.bill_no).ToList();
                if (assets != null)
                {
                    var assets_ids = assets.Select(s => s.assets_id).ToList();
                    //修改固定资产状态
                    var store_id = 0;
                    var store_name = "";
                    if (put_In_Storage.type_id == 1)//采购
                    {
                        Db.Deleteable<bus_assets>().Where(w => assets_ids.Contains(w.id)).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                        Db.Deleteable<bus_put_in_assets>().Where(w => assets_ids.Contains(w.assets_id)).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                        //Db.Updateable<bus_assets>().SetColumns(s => new bus_assets { state = 41, no = "-" + s.no }).Where(w => assets_ids.Contains(w.id)).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                    }
                    else if (put_In_Storage.type_id == 2)//调拨
                    {
                        var transfer = Db.Queryable<bus_transfer_bill>().Where(w => w.bill_no == put_In_Storage.realted_no).WithCache().First();
                        store_id = transfer.out_store_id;
                        store_name = transfer.out_store_name;
                        Db.Updateable<bus_assets>().SetColumns(s => new bus_assets { state = 41, store_id = store_id, store = store_name, address = store_name, dept_id = 0, dept = "", use_employee = "", use_employee_id = 0 }).Where(w => assets_ids.Contains(w.id)).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                        //生成入库单明细对应固定资产负单
                        var del_assets = assets.Select(s => new bus_put_in_assets { assets_id = s.assets_id, bill_no = put_In_Storage.bill_no, manufactor_id = s.manufactor_id, spec = s.spec, std_item_id = s.std_item_id }).ToList();
                        Db.Insertable(del_assets).ExecuteCommand();
                        redisCache.RemoveAll<bus_put_in_assets>();
                    }
                }
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 根据入库单号获取入库信息
        /// </summary>
        /// <param name="bill_no">入库单号</param>
        /// <returns></returns>
        public async Task<PutInStorage> GetPutInAsync(string bill_no)
        {
            var bus_put_in_storage = await Db.Queryable<bus_put_in_storage>().Where(w => w.bill_no == bill_no).WithCache().FirstAsync();
            var bus_put_in_storage_detials = await Db.Queryable<bus_put_in_storage_detials>().Where(w => w.bill_no == bill_no).WithCache().ToListAsync();

            //获取固定资产基础项目ID
            var assets_std_list = await Db.Queryable<p_std_item, b_codebase>((si, cb) => new object[] { JoinType.Left, si.type_id == cb.id }).Where((si, cb) => cb.property_id == 1).WithCache().Select((si, cb) => si.id).ToListAsync();

            //获取明细对应固定资产
            var assets = await Db.Queryable<bus_put_in_assets, bus_assets>((oa, a) => new object[] { JoinType.Left, oa.assets_id == a.id }).Where((oa, a) => bill_no == oa.bill_no).OrderBy((oa, a) => oa.assets_id).Select((oa, a) => a).WithCache().ToListAsync();

            //固定资产
            var assets_detials = bus_put_in_storage_detials.Where(w => assets_std_list.Contains(w.std_item_id)).Select(s => new bus_put_in_storage_detials_assets { approval_no = s.approval_no, bus_Assets = assets, bill_no = s.bill_no, buy_multiple = s.buy_multiple, buy_price = s.buy_price, buy_unit = s.buy_unit, manufactor = s.manufactor, manufactor_id = s.manufactor_id, name = s.name, price = s.price, spec = s.spec, std_item_id = s.std_item_id, type = s.type, type_id = s.type_id, unit = s.unit, buy_date = s.buy_date, expire_date = s.expire_date, buy_num = s.buy_num, num = s.num, no = s.no }).ToList();

            //非固定资产
            var detials = bus_put_in_storage_detials.Where(w => !assets_std_list.Contains(w.std_item_id)).ToList();

            return new PutInStorage { bill_no = bus_put_in_storage.bill_no, creater = bus_put_in_storage.creater, creater_id = bus_put_in_storage.creater_id, org_id = bus_put_in_storage.org_id, put_in_time = bus_put_in_storage.put_in_time, realted_no = bus_put_in_storage.realted_no, remark = bus_put_in_storage.remark, state = bus_put_in_storage.state, store_id = bus_put_in_storage.store_id, store_name = bus_put_in_storage.store_name, type = bus_put_in_storage.type, type_id = bus_put_in_storage.type_id, put_In_Storage_Detials = detials, put_In_Storage_Detials_Assets = assets_detials, delete_no = bus_put_in_storage.delete_no };
        }

        /// <summary>
        /// 获得分页列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Page<PutInStorage>> GetPageAsync(PutInStoragePageSearch entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            //获取分页列表
            var pages = await Db.Queryable<bus_put_in_storage>()
                            .Where(w => w.org_id == userInfo.org_id)
                            .WhereIF(!string.IsNullOrEmpty(entity.bill_no), w => w.bill_no == entity.bill_no)
                            .WhereIF(entity.state != -1, w => w.state == entity.state)
                            .WhereIF(entity.store_id != -1, w => w.store_id == entity.store_id)
                            .WhereIF(entity.type_id != -1, w => w.type_id == entity.type_id)
                            .Select(s => new PutInStorage { bill_no = s.bill_no, org_id = s.org_id, remark = s.remark, state = s.state, creater = s.creater, store_id = s.store_id, creater_id = s.creater_id, put_in_time = s.put_in_time, realted_no = s.realted_no, store_name = s.store_name, type = s.type, type_id = s.type_id, delete_no = s.delete_no })
                            .OrderBy(entity.order + orderTypeStr)
                            .WithCache()
                            .ToPageAsync(entity.page, entity.limit);
            //列表中的入库单号
            var billnos = pages.Items.Select(s => s.bill_no).ToList();
            //入库明细
            var put_In_Storage_Detials = await Db.Queryable<bus_put_in_storage_detials>().Where(w => billnos.Contains(w.bill_no)).WithCache().ToListAsync();

            //获取固定资产基础项目ID
            var assets_std_list = await Db.Queryable<p_std_item, b_codebase>((si, cb) => new object[] { JoinType.Left, si.type_id == cb.id }).Where((si, cb) => cb.property_id == 1).WithCache().Select((si, cb) => si.id).ToListAsync();

            //获取明细对应固定资产
            var assets = await Db.Queryable<bus_put_in_assets, bus_assets>((oa, a) => new object[] { JoinType.Left, oa.assets_id == a.id }).Where((oa, a) => billnos.Contains(oa.bill_no)).OrderBy((oa, a) => oa.assets_id).Select((oa, a) => new { oa, a }).WithCache().ToListAsync();

            //固定资产
            var assets_detials = put_In_Storage_Detials.Where(w => assets_std_list.Contains(w.std_item_id)).Select(s => new bus_put_in_storage_detials_assets { approval_no = s.approval_no, bus_Assets = assets.Where(w => w.oa.bill_no == s.bill_no).Select(ss => ss.a).ToList(), bill_no = s.bill_no, buy_multiple = s.buy_multiple, buy_price = s.buy_price, buy_unit = s.buy_unit, manufactor = s.manufactor, manufactor_id = s.manufactor_id, name = s.name, price = s.price, spec = s.spec, std_item_id = s.std_item_id, type = s.type, type_id = s.type_id, unit = s.unit, buy_date = s.buy_date, expire_date = s.expire_date, buy_num = s.buy_num, no = s.no, num = s.num }).ToList();

            //非固定资产
            var detials = put_In_Storage_Detials.Where(w => !assets_std_list.Contains(w.std_item_id)).ToList();

            //赋值入库明细
            pages.Items = pages.Items.Select(s => new PutInStorage { bill_no = s.bill_no, org_id = s.org_id, remark = s.remark, state = s.state, creater = s.creater, store_id = s.store_id, creater_id = s.creater_id, put_in_time = s.put_in_time, realted_no = s.realted_no, store_name = s.store_name, type = s.type, type_id = s.type_id, delete_no = s.delete_no, put_In_Storage_Detials = detials.Where(w => w.bill_no == s.bill_no).ToList(), put_In_Storage_Detials_Assets = assets_detials.Where(w => w.bill_no == s.bill_no).ToList() }).ToList();

            return pages;
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <returns></returns>
        public async Task<string> ExportAsync(string No)
        {
            if (string.IsNullOrEmpty(No))
            {
                throw new MessageException("请选择入库单进行导出");
            }
            var billnos = No.Split(',').ToList();

            var bills = await Db.Queryable<bus_put_in_storage>()
                            .Where(w => billnos.Contains(w.bill_no))
                            .WithCache().ToListAsync();

            //入库明细
            var put_In_Storage_Detials = await Db.Queryable<bus_put_in_storage_detials>().Where(w => billnos.Contains(w.bill_no)).WithCache().ToListAsync();

            //获取固定资产基础项目ID
            var assets_std_list = await Db.Queryable<p_std_item, b_codebase>((si, cb) => new object[] { JoinType.Left, si.type_id == cb.id }).Where((si, cb) => cb.property_id == 1).WithCache().Select((si, cb) => si.id).ToListAsync();

            //获取明细对应固定资产
            var assets = await Db.Queryable<bus_put_in_assets, bus_assets>((oa, a) => new object[] { JoinType.Left, oa.assets_id == a.id }).Where((oa, a) => billnos.Contains(oa.bill_no)).OrderBy((oa, a) => oa.assets_id).Select((oa, a) => new { oa, a }).WithCache().ToListAsync();

            //固定资产
            var assets_detials = put_In_Storage_Detials.Where(w => assets_std_list.Contains(w.std_item_id)).Select(s => new bus_put_in_storage_detials_assets { approval_no = s.approval_no, bus_Assets = assets.Where(w => w.oa.bill_no == s.bill_no).Select(ss => ss.a).ToList(), bill_no = s.bill_no, buy_multiple = s.buy_multiple, buy_price = s.buy_price, buy_unit = s.buy_unit, manufactor = s.manufactor, manufactor_id = s.manufactor_id, name = s.name, price = s.price, spec = s.spec, std_item_id = s.std_item_id, type = s.type, type_id = s.type_id, unit = s.unit, buy_date = s.buy_date, expire_date = s.expire_date, buy_num = s.buy_num, no = s.no, num = s.num }).ToList();

            //非固定资产
            var detials = put_In_Storage_Detials.Where(w => !assets_std_list.Contains(w.std_item_id)).ToList();
            //定义表头
            var headers = new List<string>() { "序号", "名称", "规格", "分类", "厂家", "单价", "数量", "过期时间" };

            string sWebRootFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot\\tempExcel");
            if (!Directory.Exists(sWebRootFolder))
            {
                Directory.CreateDirectory(sWebRootFolder);
            }

            //查询用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            string sFileName = $@"put_in_storage_{userInfo.id}.xlsx";
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
                    worksheet.Cells[1, 1, 2, 8].Merge = true;
                    worksheet.Cells[1, 1].Value = "入库单";
                    worksheet.Cells[1, 1].Style.Font.Size = 20;
                    worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[3, 1, 3, 4].Merge = true;
                    worksheet.Cells[3, 1].Value = $"单号：{item.bill_no}";
                    worksheet.Cells[3, 5, 3, 8].Merge = true;
                    worksheet.Cells[3, 5].Value = $"类型：{item.type}";
                    worksheet.Cells[4, 1, 4, 4].Merge = true;
                    worksheet.Cells[4, 1].Value = $"操作人：{item.creater}";
                    worksheet.Cells[4, 5, 4, 8].Merge = true;
                    worksheet.Cells[4, 5].Value = $"入库时间：{item.put_in_time.Value.ToString("yyyy年MM月dd日 HH:mm:ss")}";
                    worksheet.Cells[5, 1, 5, 8].Merge = true;
                    worksheet.Cells[5, 1].Value = $"备注：{item.remark}";

                    worksheet.Cells[6, 1, 6, 8].Merge = true;
                    worksheet.Cells[6, 1].Value = "普通物资";
                    worksheet.Cells[6, 1].Style.Font.Bold = true;
                    worksheet.Cells[6, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    //表头字段
                    for (int i = 0; i < headers.Count; i++)
                    {
                        worksheet.Cells[7, i + 1].Value = headers[i];
                    }
                    var row = 8;

                    //非固定资产
                    var dataList = detials.Where(w => w.bill_no == item.bill_no).ToList();
                    for (int i = 0; i < dataList.Count; i++)
                    {
                        worksheet.Cells[row, 1].Value = i + 1;
                        worksheet.Cells[row, 2].Value = dataList[i].name;
                        worksheet.Cells[row, 3].Value = dataList[i].spec;
                        worksheet.Cells[row, 4].Value = dataList[i].type;
                        worksheet.Cells[row, 5].Value = dataList[i].manufactor;
                        worksheet.Cells[row, 6].Value = dataList[i].price;
                        worksheet.Cells[row, 7].Value = $"{dataList[i].buy_num}{dataList[i].buy_unit}={dataList[i].num}{dataList[i].unit}";
                        worksheet.Cells[row, 8].Value = dataList[i].expire_date.Value.ToString("yyyy年MM月dd日");
                        row++;
                    }

                    worksheet.Cells[row, 1, row, 8].Merge = true; 
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

                        worksheet.Cells[row, 5, row, 8].Merge = true;
                        worksheet.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells[row, 5].Value = $"入库数量：{dataList_assets[i].buy_num}{dataList_assets[i].buy_unit}={dataList_assets[i].num}{dataList_assets[i].unit}";
                        row++;

                        worksheet.Cells[row, 1, row, 8].Merge = true;
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
