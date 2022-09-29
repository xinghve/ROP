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
    /// 采购单
    /// </summary>
    public class BuyBillService : DbContext, IBuyBillService
    {
        /// <summary>
        /// 添加采购单
        /// </summary>
        /// <param name="buyBills">采购单实体</param>
        /// <returns></returns>
        public async Task<bool> AddAsync(List<BuyBill> buyBills)
        {
            //查询用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            var result = Db.Ado.UseTran(() =>
            {
                //所有勾选的采购申请单号
                var apply_nos = new List<string>();
                var no = DateTime.Now.ToString("yyMMdd");
                var thisno = DateTime.Now.ToString("yyMMddHHmmssffffff");
                buyBills.ForEach(entity =>
                {
                    var bill = new bus_buy_bill();
                    bill.bill_no = "CG" + no;
                    //查询最大单号
                    var max_no = Db.Queryable<bus_buy_bill>().Where(w => w.bill_no.StartsWith(bill.bill_no)).OrderBy(o => o.bill_no, OrderByType.Desc).Select(s => s.bill_no).WithCache().First();
                    if (max_no == null)
                    {
                        bill.bill_no += "0001";
                    }
                    else
                    {
                        max_no = max_no.Substring(bill.bill_no.Length);
                        bill.bill_no += (int.Parse(max_no) + 1).ToString().PadLeft(4, '0');
                    }
                    bill.buy_time = entity.buy_time;
                    bill.creater = userInfo.name;
                    bill.creater_id = userInfo.id;
                    bill.create_time = DateTime.Now;
                    bill.manufactor = entity.manufactor;
                    bill.manufactor_id = entity.manufactor_id;
                    bill.org_id = userInfo.org_id;
                    bill.remark = entity.remark;
                    bill.state = 39;

                    //添加采购单返回ID
                    var command_num = Db.Insertable(bill).ExecuteCommand();
                    redisCache.RemoveAll<bus_buy_bill>();
                    if (command_num <= 0)
                    {
                        throw new MessageException("采购单未添加成功！");
                    }

                    //添加采购单明细
                    if (entity.bus_Buy_Bill_Detials.Count > 0)
                    {
                        var bus_Buy_Bill_Detials = entity.bus_Buy_Bill_Detials.Select(s => new bus_buy_bill_detials { bill_no = bill.bill_no, name = s.name, num = s.num, price = s.price, remark = s.remark, spec = s.spec, std_item_id = s.std_item_id, total_price = s.num * s.buy_price, unit = s.unit, aog_num = 0, approval_no = s.approval_no, buy_multiple = s.buy_multiple, buy_price = s.buy_price, buy_unit = s.buy_unit }).ToList();
                        Db.Insertable(bus_Buy_Bill_Detials).ExecuteCommand();
                        redisCache.RemoveAll<bus_buy_bill_detials>();
                    }

                    //添加采购单对应申请单明细
                    if (entity.buy_Bill_To_Apply_Detials.Count > 0)
                    {
                        var buy_Bill_To_Apply_Detials = entity.buy_Bill_To_Apply_Detials.Select(s => new bus_buy_bill_to_apply_detials { no = thisno, bill_no = bill.bill_no, apply_no = s.apply_no, std_item_id = s.std_item_id, num = s.num, spec = s.spec }).ToList();
                        Db.Insertable(buy_Bill_To_Apply_Detials).ExecuteCommand();
                        redisCache.RemoveAll<bus_buy_bill_to_apply_detials>();

                        buy_Bill_To_Apply_Detials.ForEach(item =>
                        {
                            //修改采购申请单明细
                            Db.Updateable<bus_buy_apply_detials>().SetColumns(it => new bus_buy_apply_detials { state = 39, buy_num = item.num }).Where(w => w.apply_no == item.apply_no && w.std_item_id == item.std_item_id && w.manufactor_id == entity.manufactor_id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                        });
                    }

                    //采购申请单号
                    var this_apply_nos = entity.buy_Bill_To_Apply_Detials.Select(s => s.apply_no).Distinct().ToList();
                    //添加到采购申请单号列表
                    this_apply_nos.ForEach(item =>
                    {
                        if (!apply_nos.Contains(item))
                        {
                            apply_nos.Add(item);
                        }
                    });
                });

                //修改采购申请单
                Db.Updateable<bus_buy_apply>().SetColumns(it => new bus_buy_apply { state = 39 }).Where(w => apply_nos.Contains(w.apply_no)).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 根据采购单ID获取采购单信息
        /// </summary>
        /// <param name="bill_no">采购单号</param>
        /// <returns></returns>
        public async Task<BuyBill> GetAsync(string bill_no)
        {
            //查询用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            //获取采购单基础信息
            var bill = await Db.Queryable<bus_buy_bill>().Where(w => w.bill_no == bill_no).WithCache().FirstAsync();

            //获取采购单明细
            var detials = await Db.Queryable<bus_buy_bill_detials>().Where(w => w.bill_no == bill_no).WithCache().ToListAsync();

            //采购单对应申请单明细
            var bill_apply_Detials = await Db.Queryable<bus_buy_bill_to_apply_detials>().Where(w => w.bill_no == bill_no).WithCache().ToListAsync();

            return new BuyBill { bus_Buy_Bill_Detials = detials, buy_time = bill.buy_time, creater = bill.creater, creater_id = bill.creater_id, create_time = bill.create_time, manufactor = bill.manufactor, manufactor_id = bill.manufactor_id, bill_no = bill.bill_no, org_id = bill.org_id, remark = bill.remark, state = bill.state, total_price = detials.Sum(s => s.total_price), buy_Bill_To_Apply_Detials = bill_apply_Detials };
        }

        /// <summary>
        /// 获得分页列表（采购单明细）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Page<bus_buy_bill_detials>> GetDetialsPageAsync(BuyBillDetialsPageSearch entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }

            return await Db.Queryable<bus_buy_bill_detials>()
                            .Where(w => w.bill_no == entity.bill_no)
                            .OrderBy(entity.order + orderTypeStr)
                            .WithCache()
                            .ToPageAsync(entity.page, entity.limit);
        }

        /// <summary>
        /// 获得分页列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Page<BuyBill>> GetPageAsync(BuyBillPageSearch entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            //获取分页列表
            var pages = await Db.Queryable<bus_buy_bill>()
                            .Where(w => w.org_id == userInfo.org_id)
                            .WhereIF(!string.IsNullOrEmpty(entity.bill_no), w => w.bill_no == entity.bill_no)
                            .WhereIF(entity.state != -1, w => w.state == entity.state)
                            .WhereIF(entity.manufactor_id != -1, w => w.manufactor_id == entity.manufactor_id)
                            .Select(s => new BuyBill { bill_no = s.bill_no, buy_time = s.buy_time, creater = s.creater, creater_id = s.creater_id, create_time = s.create_time, manufactor = s.manufactor, manufactor_id = s.manufactor_id, org_id = s.org_id, remark = s.remark, state = s.state })
                            .OrderBy(entity.order + orderTypeStr)
                            .WithCache()
                            .ToPageAsync(entity.page, entity.limit);
            //列表中的采购单号
            var billnos = pages.Items.Select(s => s.bill_no).ToList();
            //采购单号对应的所有明细
            var billDetials = await Db.Queryable<bus_buy_bill_detials>().Where(w => billnos.Contains(w.bill_no)).WithCache().ToListAsync();
            //采购单对应申请单明细
            var bill_apply_Detials = await Db.Queryable<bus_buy_bill_to_apply_detials>().Where(w => billnos.Contains(w.bill_no)).WithCache().ToListAsync();
            //赋值采购明细
            pages.Items = pages.Items.Select(s => new BuyBill { bill_no = s.bill_no, buy_time = s.buy_time, creater = s.creater, creater_id = s.creater_id, create_time = s.create_time, manufactor = s.manufactor, manufactor_id = s.manufactor_id, org_id = s.org_id, remark = s.remark, state = s.state, total_price = billDetials.Where(w => w.bill_no == s.bill_no).Sum(sum => sum.total_price), bus_Buy_Bill_Detials = billDetials.Where(w => w.bill_no == s.bill_no).ToList(), buy_Bill_To_Apply_Detials = bill_apply_Detials.Where(w => w.bill_no == s.bill_no).ToList() }).ToList();

            return pages;
        }

        /// <summary>
        /// 编辑采购单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> ModifyAsync(bus_buy_bill entity)
        {
            return await Db.Updateable<bus_buy_bill>().SetColumns(it => new bus_buy_bill { remark = entity.remark }).Where(w => w.bill_no == entity.bill_no).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CancelAsync(List<string> list)
        {
            //查询用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            var result = Db.Ado.UseTran(() =>
            {
                //查询采购单
                var buy_list = Db.Queryable<bus_buy_bill>().Where(w => list.Contains(w.bill_no)).WithCache().ToList();

                //获取是否有已入库的采购单
                var isExiste = buy_list.Any(a => a.state == 15 || a.state == 43);
                if (isExiste)
                {
                    throw new MessageException("采购单或相关采购单存在入库，不能取消");
                }

                //修改采购单状态
                Db.Updateable<bus_buy_bill>().SetColumns(it => new bus_buy_bill { state = 7 }).Where(w => list.Contains(w.bill_no)).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                //查询采购明细
                var billToApplyDetials = Db.Queryable<bus_buy_bill_to_apply_detials>().Where(w => list.Contains(w.bill_no)).WithCache().ToList();
                //获取关联申请单号
                var apply_nos = billToApplyDetials.Select(s => s.apply_no).Distinct().ToList();

                //修改采购申请单明细
                Db.Updateable<bus_buy_apply_detials>().SetColumns(it => new bus_buy_apply_detials { state = 29 }).Where(w => apply_nos.Contains(w.apply_no)).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                //修改采购申请单
                Db.Updateable<bus_buy_apply>().SetColumns(it => new bus_buy_apply { state = 34 }).Where(w => apply_nos.Contains(w.apply_no)).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                //查询采购单明细
                var detialsList = Db.Queryable<bus_buy_bill_detials>().Where(w => list.Contains(w.bill_no)).WithCache().ToList();

                //查询采购单对应申请单明细
                var applyDetialsList = Db.Queryable<bus_buy_bill_to_apply_detials>().Where(w => list.Contains(w.bill_no)).WithCache().ToList();

                //生成负单
                buy_list.ForEach(item =>
                {
                    //生成采购单负单
                    item.delete_no = item.bill_no;
                    item.bill_no = "-" + item.bill_no;
                    item.creater = userInfo.name;
                    item.creater_id = userInfo.id;
                    item.create_time = DateTime.Now;
                    item.state = 7;
                    //添加采购单返回ID
                    var command_num = Db.Insertable(item).ExecuteCommand();
                    redisCache.RemoveAll<bus_buy_bill>();
                    if (command_num <= 0)
                    {
                        throw new MessageException("采购单取消失败");
                    }
                    //生成采购单明细负单
                    var detials = detialsList.Where(w => w.bill_no == item.delete_no).Select(s => new bus_buy_bill_detials { aog_num = short.Parse((0 - s.aog_num).ToString()), approval_no = s.approval_no, bill_no = item.bill_no, buy_multiple = s.buy_multiple, buy_price = s.buy_price, buy_unit = s.buy_unit, name = s.name, num = short.Parse((0 - s.num).ToString()), price = s.price, remark = s.remark, spec = s.spec, std_item_id = s.std_item_id, total_price = 0 - s.total_price, unit = s.unit }).ToList();
                    Db.Insertable(detials).ExecuteCommand();
                    redisCache.RemoveAll<bus_buy_bill_detials>();

                    //生成采购单对应申请单明细负单
                    var thisno = DateTime.Now.ToString("yyMMddHHmmssffffff");
                    var applyDetials = applyDetialsList.Where(w => w.bill_no == item.delete_no).Select(s => new bus_buy_bill_to_apply_detials { apply_no = s.apply_no, bill_no = item.bill_no, no = thisno, spec = s.spec, num = short.Parse((0 - s.num).ToString()), std_item_id = s.std_item_id }).ToList();
                    Db.Insertable(applyDetials).ExecuteCommand();
                    redisCache.RemoveAll<bus_buy_bill_to_apply_detials>();
                });
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 获取指定采购单号生成采购单时勾选的所有采购申请单生成的所有采购单号
        /// </summary>
        /// <param name="No"></param>
        /// <returns></returns>
        public async Task<List<string>> GetLinkNoAsync(string No)
        {
            var no = await Db.Queryable<bus_buy_bill_to_apply_detials>().Where(w => w.bill_no == No).Select(s => s.no).WithCache().FirstAsync();
            var bills = await Db.Queryable<bus_buy_bill_to_apply_detials>().Where(w => w.no == no).GroupBy(s => s.bill_no).Select(s => s.bill_no).WithCache().ToListAsync();
            return bills;
        }

        /// <summary>
        /// 完成
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> FinishAsync(bus_buy_bill entity)
        {
            //查询用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            var result = Db.Ado.UseTran(() =>
            {
                Finish(entity.bill_no, 15);
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 完成
        /// </summary>
        /// <param name="bill_no"></param>
        /// <param name="bill_state"></param>
        public void Finish(string bill_no, short bill_state)
        {
            //修改采购单状态
            Db.Updateable<bus_buy_bill>().SetColumns(s => new bus_buy_bill { state = bill_state }).Where(w => w.bill_no == bill_no).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

            //查询采购单关联的申请单门店ID为0的（机构）
            var list = Db.Queryable<bus_buy_bill_to_apply_detials, bus_buy_apply>((bad, ba) => new object[] { JoinType.Left, bad.apply_no == ba.apply_no }).Where((bad, ba) => ba.store_id == 0 && bad.bill_no == bill_no && ba.state != 7).Select((bad, ba) => ba.apply_no).WithCache().ToList();

            //查询申请单对应的采购单
            var bill_list = Db.Queryable<bus_buy_bill_to_apply_detials, bus_buy_bill>((bad, bb) => new object[] { JoinType.Left, bad.bill_no == bb.bill_no }).Where((bad, bb) => list.Contains(bad.apply_no) && bb.state != 7).Select((bad, bb) => new { bad.apply_no, bb.state }).WithCache().ToList();
            list.ForEach(item =>
            {
                short state = 25;
                //是否调拨全部
                var isExiste = bill_list.Exists(e => e.apply_no == item && e.state != 15);
                if (isExiste)
                {
                    state = 33;
                }
                Db.Updateable<bus_buy_apply>().SetColumns(s => new bus_buy_apply { state = state }).Where(w => w.apply_no == item).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
            });
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <returns></returns>
        public async Task<string> ExportAsync(string No)
        {
            if (string.IsNullOrEmpty(No))
            {
                throw new MessageException("请选择采购单进行导出");
            }
            var billnos = No.Split(',').ToList();

            var bills = await Db.Queryable<bus_buy_bill>()
                            .Where(w => billnos.Contains(w.bill_no))
                            .WithCache().ToListAsync();

            //申请单号对应的所有明细
            var billDetials = await Db.Queryable<bus_buy_bill_detials>()
                            .Where(w => billnos.Contains(w.bill_no))
                            .WithCache().ToListAsync();

            //定义表头
            var headers = new List<string>() { "序号", "名称", "规格", "采购单位", "采购数量", "采购单价", "采购总价" };

            string sWebRootFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot\\tempExcel");
            if (!Directory.Exists(sWebRootFolder))
            {
                Directory.CreateDirectory(sWebRootFolder);
            }

            //查询用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            string sFileName = $@"buybill_{userInfo.id}.xlsx";
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
                    worksheet.Cells[1, 1].Value = "采购单";
                    worksheet.Cells[1, 1].Style.Font.Size = 20;
                    worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[3, 1, 3, 2].Merge = true;
                    worksheet.Cells[3, 1].Value = $"单号：{item.bill_no}";
                    worksheet.Cells[3, 3, 3, 4].Merge = true;

                    var money = billDetials.Where(w => w.bill_no == item.bill_no).Sum(sum => sum.total_price);

                    worksheet.Cells[3, 3].Value = $"金额：{money}";
                    worksheet.Cells[3, 5, 3, 7].Merge = true;
                    worksheet.Cells[3, 5].Value = $"厂家：{item.manufactor}";
                    worksheet.Cells[4, 1, 4, 4].Merge = true;
                    worksheet.Cells[4, 1].Value = $"创建人：{item.creater}";
                    worksheet.Cells[4, 5, 4, 7].Merge = true;
                    worksheet.Cells[4, 5].Value = $"创建时间：{item.create_time.Value.ToString("yyyy年MM月dd日 HH:mm:ss")}";
                    worksheet.Cells[5, 1, 5, 7].Merge = true;
                    worksheet.Cells[5, 1].Value = $"备注：{item.remark}";
                    //表头字段
                    for (int i = 0; i < headers.Count; i++)
                    {
                        worksheet.Cells[6, i + 1].Value = headers[i];
                    }
                    var row = 7;
                    var dataList = billDetials.Where(w => w.bill_no == item.bill_no).ToList();
                    for (int i = 0; i < dataList.Count; i++)
                    {
                        worksheet.Cells[row, 1].Value = i + 1;
                        worksheet.Cells[row, 2].Value = dataList[i].name;
                        worksheet.Cells[row, 3].Value = dataList[i].spec;
                        worksheet.Cells[row, 4].Value = dataList[i].buy_unit;
                        worksheet.Cells[row, 5].Value = dataList[i].num;
                        worksheet.Cells[row, 6].Value = dataList[i].buy_price;
                        worksheet.Cells[row, 7].Value = dataList[i].total_price;
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
