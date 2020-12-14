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

namespace Service.Repository.Implements.Business
{
    /// <summary>
    /// 出库
    /// </summary>
    public class OutStorageService : DbContext, IOutStorageService
    {
        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> CancelAsync(bus_out_storage entity)
        {
            //查询用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            var result = await Db.Ado.UseTranAsync(() =>
            {
                //修改出库状态
                Db.Updateable<bus_out_storage>().SetColumns(s => new bus_out_storage { state = 7 }).Where(w => w.bill_no == entity.bill_no).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                //查询出库信息
                var out_Storage = Db.Queryable<bus_out_storage>().Where(w => w.bill_no == entity.bill_no).WithCache().First();
                //查询出库明细
                var out_Storage_Detials = Db.Queryable<bus_out_storage_detials>().Where(w => w.bill_no == entity.bill_no).WithCache().ToList();

                //添加作废负单
                out_Storage.delete_no = out_Storage.bill_no;
                out_Storage.bill_no = "-" + out_Storage.bill_no;
                out_Storage.creater = userInfo.name;
                out_Storage.creater_id = userInfo.id;
                out_Storage.out_time = DateTime.Now;
                out_Storage.state = 7;
                Db.Insertable(out_Storage).ExecuteCommand();
                redisCache.RemoveAll<bus_out_storage>();
                //添加负单明细
                var f_list = out_Storage_Detials.Select(s => new bus_out_storage_detials { bill_no = out_Storage.bill_no, buy_date = s.buy_date, expire_date = s.expire_date, type = s.type, type_id = s.type_id, approval_no = s.approval_no, buy_multiple = s.buy_multiple, buy_price = s.buy_price, buy_unit = s.buy_unit, manufactor = s.manufactor, manufactor_id = s.manufactor_id, name = s.name, price = s.price, spec = s.spec, std_item_id = s.std_item_id, unit = s.unit, bill_num = 0 - s.bill_num, storage_id = s.storage_id, storage_no = s.storage_no }).ToList();
                Db.Insertable(f_list).ExecuteCommand();
                redisCache.RemoveAll<bus_out_storage_detials>();

                if (out_Storage.type_id == 1)//调拨
                {
                    //修改调拨单状态
                    Db.Updateable<bus_transfer_bill>().SetColumns(s => new bus_transfer_bill { state = 29 }).Where(w => w.bill_no == out_Storage.realted_no).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                }
                else if (out_Storage.type_id == 2)//领用
                {

                }
                else if (out_Storage.type_id == 3)//盘点
                {

                }
                else if (out_Storage.type_id == 4)//退货
                {

                }

                ////查询库存明细
                //var storage_detials = Db.Queryable<bus_storage_detials>().Where(w => w.bill_no == entity.bill_no).WithCache().ToList();
                ////查询库存id
                //var ids = storage_detials.Select(s => s.id).Distinct().ToList();
                ////查询库存
                //var storages = Db.Queryable<bus_storage>().Where(w => ids.Contains(w.id)).WithCache().ToList();
                //ids.ForEach(id =>
                //{
                //    var out_num = storage_detials.Where(w => w.id == id).Sum(s => s.out_num);
                //    var this_storage = storages.Where(w => w.id == id).FirstOrDefault();
                //    this_storage.use_num -= out_num;
                //    this_storage.num -= out_num;
                //    if (this_storage.use_num < 0 || this_storage.num < 0)
                //    {
                //        throw new MessageException("库存数量不足，不能取消");
                //    }
                //    //修改库存数量
                //    Db.Updateable<bus_storage>().SetColumns(s => new bus_storage { num = this_storage.num, use_num = this_storage.use_num }).Where(w => w.id == id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                //});
                ////删除库存明细
                //Db.Deleteable<bus_storage_detials>().Where(w => w.bill_no == entity.bill_no).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
            });
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
        public async Task<Page<OutStorage>> GetPageAsync(OutStoragePageSearch entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            //获取分页列表
            var pages = await Db.Queryable<bus_out_storage>()
                            .Where(w => w.org_id == userInfo.org_id)
                            .WhereIF(!string.IsNullOrEmpty(entity.bill_no), w => w.bill_no == entity.bill_no)
                            .WhereIF(entity.state != -1, w => w.state == entity.state)
                            .WhereIF(entity.store_id != -1, w => w.store_id == entity.store_id)
                            .WhereIF(entity.type_id != -1, w => w.type_id == entity.type_id)
                            .Select(s => new OutStorage { bill_no = s.bill_no, org_id = s.org_id, remark = s.remark, state = s.state, creater = s.creater, store_id = s.store_id, creater_id = s.creater_id, realted_no = s.realted_no, store_name = s.store_name, type = s.type, type_id = s.type_id, await_verifier = s.await_verifier, await_verifier_id = s.await_verifier_id, create_time = s.create_time, level = s.level, out_time = s.out_time, total_level = s.total_level, verifier = s.verifier, verifier_id = s.verifier_id, verify_remark = s.verify_remark, verify_time = s.verify_time, delete_no = s.delete_no })
                            .OrderBy(entity.order + orderTypeStr)
                            .WithCache()
                            .ToPageAsync(entity.page, entity.limit);
            //列表中的出库单号
            var billnos = pages.Items.Select(s => s.bill_no).ToList();
            //出库明细
            var out_Storage_Detials = await Db.Queryable<bus_out_storage_detials>().Where(w => billnos.Contains(w.bill_no)).WithCache().ToListAsync();

            //获取固定资产基础项目ID
            var assets_std_list = await Db.Queryable<p_std_item, b_codebase>((si, cb) => new object[] { JoinType.Left, si.type_id == cb.id }).Where((si, cb) => cb.property_id == 1).WithCache().Select((si, cb) => si.id).ToListAsync();

            //获取明细对应固定资产
            var assets = await Db.Queryable<bus_out_assets, bus_assets>((oa, a) => new object[] { JoinType.Left, oa.assets_id == a.id }).Where((oa, a) => billnos.Contains(oa.bill_no)).OrderBy((oa, a) => a.id).Select((oa, a) => new { oa, a }).WithCache().ToListAsync();

            //固定资产
            var assets_detials = out_Storage_Detials.Where(w => assets_std_list.Contains(w.std_item_id)).Select(s => new bus_out_storage_detials_assets { approval_no = s.approval_no, bus_Assets = assets.Where(w => w.oa.bill_no == s.bill_no).Select(ss => ss.a).ToList(), bill_no = s.bill_no, buy_multiple = s.buy_multiple, buy_price = s.buy_price, buy_unit = s.buy_unit, manufactor = s.manufactor, manufactor_id = s.manufactor_id, name = s.name, bill_num = s.bill_num, price = s.price, spec = s.spec, std_item_id = s.std_item_id, type = s.type, type_id = s.type_id, unit = s.unit, buy_date = s.buy_date, expire_date = s.expire_date, storage_id = s.storage_id, storage_no = s.storage_no }).ToList();

            //非固定资产
            var detials = out_Storage_Detials.Where(w => !assets_std_list.Contains(w.std_item_id)).ToList();

            //赋值出库明细
            pages.Items = pages.Items.Select(s => new OutStorage { bill_no = s.bill_no, org_id = s.org_id, remark = s.remark, state = s.state, creater = s.creater, store_id = s.store_id, creater_id = s.creater_id, realted_no = s.realted_no, store_name = s.store_name, type = s.type, type_id = s.type_id, await_verifier = s.await_verifier, await_verifier_id = s.await_verifier_id, create_time = s.create_time, level = s.level, out_time = s.out_time, total_level = s.total_level, verifier = s.verifier, verifier_id = s.verifier_id, verify_remark = s.verify_remark, verify_time = s.verify_time, delete_no = s.delete_no, out_Storage_Detials = detials.Where(w => w.bill_no == s.bill_no).ToList(), out_Storage_Detials_Assets = assets_detials.Where(w => w.bill_no == s.bill_no).ToList() }).ToList();

            return pages;
        }

        /// <summary>
        /// 根据出库单号获取出库信息
        /// </summary>
        /// <param name="bill_no">出库单号</param>
        /// <returns></returns>
        public async Task<OutStorage> GetOutAsync(string bill_no)
        {
            var bus_out_storage = await Db.Queryable<bus_out_storage>().Where(w => w.bill_no == bill_no).WithCache().FirstAsync();
            var bus_out_storage_detials = await Db.Queryable<bus_out_storage_detials>().Where(w => w.bill_no == bill_no).WithCache().ToListAsync();

            //获取固定资产基础项目ID
            var assets_std_list = await Db.Queryable<p_std_item, b_codebase>((si, cb) => new object[] { JoinType.Left, si.type_id == cb.id }).Where((si, cb) => cb.property_id == 1).WithCache().Select((si, cb) => si.id).ToListAsync();

            //获取明细对应固定资产
            var assets = await Db.Queryable<bus_out_assets, bus_assets>((oa, a) => new object[] { JoinType.Left, oa.assets_id == a.id }).Where((oa, a) => bill_no == oa.bill_no).OrderBy((oa, a) => a.id).Select((oa, a) => a).WithCache().ToListAsync();

            //固定资产
            var assets_detials = bus_out_storage_detials.Where(w => assets_std_list.Contains(w.std_item_id)).Select(s => new bus_out_storage_detials_assets { approval_no = s.approval_no, bus_Assets = assets, bill_no = s.bill_no, buy_multiple = s.buy_multiple, buy_price = s.buy_price, buy_unit = s.buy_unit, manufactor = s.manufactor, manufactor_id = s.manufactor_id, name = s.name, bill_num = s.bill_num, price = s.price, spec = s.spec, std_item_id = s.std_item_id, type = s.type, type_id = s.type_id, unit = s.unit, buy_date = s.buy_date, expire_date = s.expire_date, storage_id = s.storage_id, storage_no = s.storage_no }).ToList();

            //非固定资产
            var detials = bus_out_storage_detials.Where(w => !assets_std_list.Contains(w.std_item_id)).ToList();

            return new OutStorage { bill_no = bus_out_storage.bill_no, creater = bus_out_storage.creater, creater_id = bus_out_storage.creater_id, org_id = bus_out_storage.org_id, realted_no = bus_out_storage.realted_no, remark = bus_out_storage.remark, state = bus_out_storage.state, store_id = bus_out_storage.store_id, store_name = bus_out_storage.store_name, type = bus_out_storage.type, type_id = bus_out_storage.type_id, await_verifier = bus_out_storage.await_verifier, await_verifier_id = bus_out_storage.await_verifier_id, create_time = bus_out_storage.create_time, delete_no = bus_out_storage.delete_no, verify_time = bus_out_storage.verify_time, verify_remark = bus_out_storage.verify_remark, verifier_id = bus_out_storage.verifier_id, level = bus_out_storage.level, out_time = bus_out_storage.out_time, total_level = bus_out_storage.total_level, verifier = bus_out_storage.verifier, out_Storage_Detials = detials, out_Storage_Detials_Assets = assets_detials };
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <returns></returns>
        public async Task<string> ExportAsync(string No)
        {
            if (string.IsNullOrEmpty(No))
            {
                throw new MessageException("请选择出库单进行导出");
            }
            var billnos = No.Split(',').ToList();

            var bills = await Db.Queryable<bus_out_storage>()
                            .Where(w => billnos.Contains(w.bill_no))
                            .WithCache().ToListAsync();

            //出库明细
            var out_Storage_Detials = await Db.Queryable<bus_out_storage_detials>().Where(w => billnos.Contains(w.bill_no)).WithCache().ToListAsync();

            //获取固定资产基础项目ID
            var assets_std_list = await Db.Queryable<p_std_item, b_codebase>((si, cb) => new object[] { JoinType.Left, si.type_id == cb.id }).Where((si, cb) => cb.property_id == 1).WithCache().Select((si, cb) => si.id).ToListAsync();

            //获取明细对应固定资产
            var assets = await Db.Queryable<bus_out_assets, bus_assets>((oa, a) => new object[] { JoinType.Left, oa.assets_id == a.id }).Where((oa, a) => billnos.Contains(oa.bill_no)).OrderBy((oa, a) => a.id).Select((oa, a) => new { oa, a }).WithCache().ToListAsync();

            //固定资产
            var assets_detials = out_Storage_Detials.Where(w => assets_std_list.Contains(w.std_item_id)).Select(s => new bus_out_storage_detials_assets { approval_no = s.approval_no, bus_Assets = assets.Where(w => w.oa.bill_no == s.bill_no).Select(ss => ss.a).ToList(), bill_no = s.bill_no, buy_multiple = s.buy_multiple, buy_price = s.buy_price, buy_unit = s.buy_unit, manufactor = s.manufactor, manufactor_id = s.manufactor_id, name = s.name, bill_num = s.bill_num, price = s.price, spec = s.spec, std_item_id = s.std_item_id, type = s.type, type_id = s.type_id, unit = s.unit, buy_date = s.buy_date, expire_date = s.expire_date, storage_id = s.storage_id, storage_no = s.storage_no }).ToList();

            //非固定资产
            var detials = out_Storage_Detials.Where(w => !assets_std_list.Contains(w.std_item_id)).ToList();

            //定义表头
            var headers = new List<string>() { "序号", "名称", "规格", "分类", "厂家", "库存批次编号", "数量", "过期时间" };

            string sWebRootFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot\\tempExcel");
            if (!Directory.Exists(sWebRootFolder))
            {
                Directory.CreateDirectory(sWebRootFolder);
            }

            //查询用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            string sFileName = $@"out_storage_{userInfo.id}.xlsx";
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
                    worksheet.Cells[1, 1].Value = "出库单";
                    worksheet.Cells[1, 1].Style.Font.Size = 20;
                    worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[3, 1, 3, 4].Merge = true;
                    worksheet.Cells[3, 1].Value = $"单号：{item.bill_no}";
                    worksheet.Cells[3, 5, 3, 8].Merge = true;
                    worksheet.Cells[3, 5].Value = $"类型：{item.type}";
                    worksheet.Cells[4, 1, 4, 4].Merge = true;
                    worksheet.Cells[4, 1].Value = $"操作人：{item.creater}";
                    worksheet.Cells[4, 5, 4, 8].Merge = true;
                    worksheet.Cells[4, 5].Value = $"出库时间：{item.out_time.Value.ToString("yyyy年MM月dd日 HH:mm:ss")}";
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
                        worksheet.Cells[row, 6].Value = dataList[i].storage_no;
                        worksheet.Cells[row, 7].Value = $"{dataList[i].bill_num / dataList[i].buy_multiple}{dataList[i].buy_unit}={dataList[i].bill_num}{dataList[i].unit}";
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
                        worksheet.Cells[row, 5].Value = $"出库数量：{dataList_assets[i].bill_num}{dataList_assets[i].unit}";
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
