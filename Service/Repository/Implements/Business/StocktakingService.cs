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
    /// 盘点
    /// </summary>
    public class StocktakingService : DbContext, IStocktakingService
    {
        /// <summary>
        /// 新增盘点
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> AddAsync(Stocktaking entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            var result = Db.Ado.UseTran(() =>
            {
                if (entity.stocktaking_Detials.Count == 0)
                {
                    throw new MessageException("请选择物资进行盘点");
                }

                //查询库存明细
                var std_item_ids = entity.stocktaking_Detials.Select(s => s.std_item_id).ToList();
                var storage_detials = Db.Queryable<bus_storage_detials, bus_storage>((sd, s) => new object[] { JoinType.Left, sd.id == s.id }).Where((sd, s) => std_item_ids.Contains(sd.std_item_id) && s.store_id == entity.store_id).WithCache().ToList();
                entity.stocktaking_Detials.ForEach(item =>
                {
                    var isExiste = storage_detials.Any(w => w.std_item_id == item.std_item_id && w.spec == item.spec && w.manufactor_id == item.manufactor_id && w.num != w.use_num);
                    if (isExiste)
                    {
                        throw new MessageException("勾选物资存在未完成的单据");
                    }

                    //查询物资是否已在盘点中
                    isExiste = Db.Queryable<bus_stocktaking_detials, bus_stocktaking>((sd, s) => new object[] { JoinType.Left, sd.no == s.no }).WithCache().Any((sd, s) => s.state != 15 && s.state != 7 && sd.std_item_id == item.std_item_id && sd.spec == item.spec && sd.manufactor_id == item.manufactor_id && s.store_id == entity.store_id);
                    if (isExiste)
                    {
                        throw new MessageException($"{item.name}-{item.spec}（{item.manufactor}）已在盘点中");
                    }
                    //查询物资是否已在报损报溢中
                    isExiste = Db.Queryable<bus_loss_overflow>().WithCache().Any(a => a.state != 28 && a.state != 34 && a.std_item_id == item.std_item_id && a.spec == item.spec && a.manufactor_id == item.manufactor_id && a.store_id == entity.store_id);
                    if (isExiste)
                    {
                        throw new MessageException($"{item.name}-{item.spec}（{item.manufactor}）已在报损报溢中");
                    }
                });

                //盘点单
                var stocktaking = new bus_stocktaking();
                stocktaking.creater = userInfo.name;
                stocktaking.creater_id = userInfo.id;
                stocktaking.create_time = DateTime.Now;
                stocktaking.date = entity.date;
                stocktaking.org_id = userInfo.org_id;
                stocktaking.remark = entity.remark;
                stocktaking.store_id = entity.store_id;
                stocktaking.store_name = entity.store_name;
                stocktaking.state = 45;
                stocktaking.responsible_employee_id = entity.responsible_employee_id;
                stocktaking.responsible_employee = entity.responsible_employee;
                stocktaking.no = "PD" + userInfo.org_id + DateTime.Now.ToString("yyMMdd");
                //查询最大单号
                var max_no = Db.Queryable<bus_stocktaking>().Where(w => w.no.StartsWith(stocktaking.no) && w.org_id == userInfo.org_id).OrderBy(o => o.no, OrderByType.Desc).Select(s => s.no).WithCache().First();
                if (max_no == null)
                {
                    stocktaking.no += "0001";
                }
                else
                {
                    max_no = max_no.Substring(stocktaking.no.Length);
                    stocktaking.no += (int.Parse(max_no) + 1).ToString().PadLeft(4, '0');
                }
                Db.Insertable(stocktaking).ExecuteCommand();
                redisCache.RemoveAll<bus_stocktaking>();

                //盘点单明细
                var stocktaking_Detials = entity.stocktaking_Detials.Select(s => new bus_stocktaking_detials { manufactor = s.manufactor, manufactor_id = s.manufactor_id, name = s.name, no = stocktaking.no, not_report_num = -s.num, num = s.num, num_difference = -s.num, report_num = 0, spec = s.spec, std_item_id = s.std_item_id, stocktaking_num = 0, type = s.type, type_id = s.type_id, unit = s.unit }).ToList();
                Db.Insertable(stocktaking_Detials).ExecuteCommand();
                redisCache.RemoveAll<bus_stocktaking_detials>();
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 删除盘点
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> DeleteAsync(bus_stocktaking entity)
        {
            var stocktaking = await Db.Queryable<bus_stocktaking>().Where(w => w.no == entity.no).WithCache().FirstAsync();
            if (stocktaking.state != 45)
            {
                throw new MessageException("盘点单已经开始盘点，不能删除");
            }
            return await Db.Deleteable<bus_stocktaking>().Where(w => w.no == entity.no).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
        }

        /// <summary>
        /// 结束盘点
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> EndAsync(bus_stocktaking entity)
        {
            var stocktaking = await Db.Queryable<bus_stocktaking>().Where(w => w.no == entity.no).WithCache().FirstAsync();
            if (stocktaking.state != 46)
            {
                throw new MessageException("盘点单不在盘点进行中，不能结束");
            }
            short state = 15;
            //查询是否存在需要报损报溢的物资
            var isExiste = await Db.Queryable<bus_stocktaking_detials>().AnyAsync(w => w.no == entity.no && w.num_difference != 0);
            if (isExiste)
            {
                state = 24;
            }
            return await Db.Updateable<bus_stocktaking>().SetColumns(s => new bus_stocktaking { state = state }).Where(w => w.no == entity.no).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
        }

        /// <summary>
        /// 盘点单分页
        /// </summary>
        /// <returns></returns>
        public async Task<Page<Stocktaking>> GetPageAsync(StocktakingPageSearch entity)
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
            var pages = await Db.Queryable<bus_stocktaking>()
                            .Where(w => w.org_id == userInfo.org_id)
                            .WhereIF(!string.IsNullOrEmpty(entity.no), w => w.no.Contains(entity.no))
                            .WhereIF(entity.start_date != null, w => w.date >= entity.start_date)
                            .WhereIF(entity.end_date != null, w => w.date < entity.end_date)
                            .WhereIF(entity.store_id != -1, w => w.store_id == entity.store_id)
                            .WhereIF(entity.state != -1, w => w.state == entity.state)
                            .Select(s => new Stocktaking { creater = s.creater, store_id = s.store_id, creater_id = s.creater_id, create_time = s.create_time, date = s.date, no = s.no, org_id = s.org_id, remark = s.remark, store_name = s.store_name, responsible_employee = s.responsible_employee, state = s.state, responsible_employee_id = s.responsible_employee_id })
                            .OrderBy(entity.order + orderTypeStr)
                            .WithCache()
                            .ToPageAsync(entity.page, entity.limit);

            //列表中的盘点单号
            var nos = pages.Items.Select(s => s.no).ToList();

            //查询盘点单明细
            var stocktaking_detials_list = await Db.Queryable<bus_stocktaking_detials, b_codebase>((sd, c) => new object[] { JoinType.Left, sd.type_id == c.id }).Where((sd, c) => nos.Contains(sd.no)).Select((sd, c) => new StocktakingDetials { type_id = sd.type_id, manufactor = sd.manufactor, spec = sd.spec, manufactor_id = sd.manufactor_id, name = sd.name, no = sd.no, not_report_num = sd.not_report_num, num = sd.num, num_difference = sd.num_difference, property_id = c.property_id, report_num = sd.report_num, std_item_id = sd.std_item_id, stocktaking_num = sd.stocktaking_num, type = sd.type, unit = sd.unit }).WithCache().ToListAsync();

            //赋值调拨明细
            pages.Items = pages.Items.Select(s => new Stocktaking { creater = s.creater, store_id = s.store_id, creater_id = s.creater_id, create_time = s.create_time, date = s.date, no = s.no, org_id = s.org_id, remark = s.remark, store_name = s.store_name, responsible_employee = s.responsible_employee, responsible_employee_id = s.responsible_employee_id, state = s.state, stocktaking_Detials = stocktaking_detials_list.Where(w => w.no == s.no).ToList() }).ToList();

            return pages;
        }

        /// <summary>
        /// 编辑盘点
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> ModifyAsync(Stocktaking entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            var result = Db.Ado.UseTran(() =>
            {
                var stocktaking = Db.Queryable<bus_stocktaking>().Where(w => w.no == entity.no).WithCache().First();
                if (stocktaking.state != 45)
                {
                    throw new MessageException("盘点单已经开始盘点，不能编辑");
                }

                if (entity.stocktaking_Detials.Count == 0)
                {
                    throw new MessageException("请选择物资进行盘点");
                }

                //盘点单修改                
                Db.Updateable<bus_stocktaking>().SetColumns(s => new bus_stocktaking { date = entity.date, remark = entity.remark, responsible_employee = entity.responsible_employee, responsible_employee_id = entity.responsible_employee_id }).Where(w => w.no == entity.no).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                //删除原有的盘点单明细
                Db.Deleteable<bus_stocktaking_detials>().Where(w => w.no == entity.no).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                //添加盘点单明细
                var stocktaking_Detials = entity.stocktaking_Detials.Select(s => new bus_stocktaking_detials { manufactor = s.manufactor, manufactor_id = s.manufactor_id, name = s.name, no = entity.no, not_report_num = s.num, num = s.num, num_difference = s.num, report_num = 0, spec = s.spec, std_item_id = s.std_item_id, stocktaking_num = 0, type = s.type, type_id = s.type_id, unit = s.unit }).ToList();
                Db.Insertable(stocktaking_Detials).ExecuteCommand();
                redisCache.RemoveAll<bus_stocktaking_detials>();
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 开始盘点
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> StartAsync(bus_stocktaking entity)
        {
            var stocktaking = Db.Queryable<bus_stocktaking>().Where(w => w.no == entity.no).WithCache().First();
            if (stocktaking.state != 45)
            {
                throw new MessageException("盘点单已经开始盘点");
            }
            return await Db.Updateable<bus_stocktaking>().SetColumns(s => new bus_stocktaking { state = 46 }).Where(w => w.no == entity.no).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
        }

        /// <summary>
        /// 盘点数量录入
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> StocktakingNumAsync(bus_stocktaking_detials entity)
        {
            if (entity.stocktaking_num < 0)
            {
                throw new MessageException("盘点数量必须大于或等于0");
            }
            var stocktaking_detials = await Db.Queryable<bus_stocktaking_detials>().Where(w => w.no == entity.no && w.std_item_id == entity.std_item_id && w.spec == entity.spec && w.manufactor_id == entity.manufactor_id).WithCache().FirstAsync();
            entity.num_difference = entity.stocktaking_num - stocktaking_detials.num;
            return await Db.Updateable<bus_stocktaking_detials>().SetColumns(s => new bus_stocktaking_detials { num_difference = entity.num_difference, stocktaking_num = entity.stocktaking_num, not_report_num = entity.num_difference }).Where(w => w.no == entity.no && w.std_item_id == entity.std_item_id && w.spec == entity.spec && w.manufactor_id == entity.manufactor_id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
        }

        /// <summary>
        /// 待盘点物资分页
        /// </summary>
        /// <returns></returns>
        public async Task<Page<WaitStocktaking>> GetWaitStocktakingPageAsync(WaitStocktakingPageSearch entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }

            //获取分页列表
            return await Db.Queryable<bus_storage_detials, bus_storage, b_codebase>((sd, s, c) => new object[] { JoinType.Left, sd.id == s.id, JoinType.Left, s.type_id == c.id })
                            .Where((sd, s, c) => s.org_id == userInfo.org_id && s.store_id == entity.store_id)
                            .WhereIF(entity.catalog_type == 0 && entity.type_id != -1, (sd, s, c) => sd.type_id == entity.type_id)
                            .WhereIF(entity.catalog_type == 1, (sd, s, c) => c.catalog_id == entity.type_id)
                            .WhereIF(entity.catalog_type == 2, (sd, s, c) => sd.type_id == entity.type_id)
                            .WhereIF(!string.IsNullOrEmpty(entity.name), (sd, s, c) => sd.spec.Contains(entity.name) || sd.name.Contains(entity.name))
                            .WhereIF(entity.manufactor_id > 0, (sd, s, c) => sd.manufactor_id == entity.manufactor_id)
                            .GroupBy((sd, s, c) => new { sd.manufactor, sd.manufactor_id, sd.name, sd.spec, sd.std_item_id, sd.type, sd.type_id, sd.unit })
                            .Select((sd, s, c) => new WaitStocktaking { manufactor = sd.manufactor, manufactor_id = sd.manufactor_id, name = sd.name, spec = sd.spec, std_item_id = sd.std_item_id, type = sd.type, type_id = sd.type_id, unit = sd.unit, num = SqlFunc.AggregateSum(sd.num) })
                            .OrderBy(entity.order + orderTypeStr)
                            .WithCache()
                            .ToPageAsync(entity.page, entity.limit);
        }

        /// <summary>
        /// 根据单号获取明细
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        public async Task<List<StocktakingDetials>> GetStocktakingDetialsAsync(string no)
        {
            return await Db.Queryable<bus_stocktaking_detials, b_codebase>((sd, c) => new object[] { JoinType.Left, sd.type_id == c.id }).Where((sd, c) => sd.no == no).OrderBy((sd, c) => sd.spec).Select((sd, c) => new StocktakingDetials { type_id = sd.type_id, manufactor = sd.manufactor, spec = sd.spec, manufactor_id = sd.manufactor_id, name = sd.name, no = sd.no, not_report_num = sd.not_report_num, num = sd.num, num_difference = sd.num_difference, property_id = c.property_id, report_num = sd.report_num, std_item_id = sd.std_item_id, stocktaking_num = sd.stocktaking_num, type = sd.type, unit = sd.unit }).WithCache().ToListAsync();
        }

        /// <summary>
        /// 导出盘点
        /// </summary>
        /// <param name="list_no"></param>
        /// <returns></returns>
        public async Task<string> ExportAsync(List<string> list_no)
        {
            if (list_no.Count==0)
            {
                throw new MessageException("请选择盘点单进行导出");
            }
            

            var applys = await Db.Queryable<bus_stocktaking>()
                            .Where(w => list_no.Contains(w.no))
                            .WithCache().ToListAsync();

            //申请单号对应的所有明细
            var applyDetials = await Db.Queryable<bus_stocktaking_detials>()
                            .Where(w => list_no.Contains(w.no))
                            .WithCache().ToListAsync();

            //定义表头
            var headers = new List<string>() { "序号", "名称", "规格", "厂家",  "单位", "库存数量", "盘点数量", "数量差值"};

            string sWebRootFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot\\tempExcel");//如果用浏览器url下载的方式  存放excel的文件夹一定要建在网站首页的同级目录下！！！
            if (!Directory.Exists(sWebRootFolder))
            {
                Directory.CreateDirectory(sWebRootFolder);
            }

            //查询用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            string sFileName = $@"stocktaking_{userInfo.id}.xlsx";
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
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add($"{item.no}");
                    worksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;                
                    worksheet.Cells[1, 1, 2, 9].Merge = true;
                    worksheet.Cells[1, 1].Value = "资产盘点单";
                    worksheet.Cells[1, 1].Style.Font.Size = 20;
                    worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[3, 1, 3, 3].Merge = true;
                    worksheet.Cells[3, 1].Value = $"单号：{item.no}";
                    worksheet.Cells[3, 4, 3, 6].Merge = true;
                    worksheet.Cells[3, 4].Value = $"盘点时间：{item.date}";
                    worksheet.Cells[3, 7, 3, 9].Merge = true;
                    worksheet.Cells[3, 7].Value = $"创建门店：{item.store_name}";
                    worksheet.Cells[4, 1, 4, 3].Merge = true;
                    worksheet.Cells[4, 1].Value = $"负责人：{item.responsible_employee}";
                    worksheet.Cells[4, 4, 4, 6].Merge = true;
                    worksheet.Cells[4, 6].Value = $"创建时间：{item.create_time.Value.ToString("yyyy年MM月dd日")}";
                    worksheet.Cells[4, 7, 4, 9].Merge = true;
                    worksheet.Cells[4, 7].Value = $"创建人：{item.creater}";
                    worksheet.Cells[5, 1, 5, 9].Merge = true;
                    worksheet.Cells[5, 1].Value = $"备注：{item.remark}";
                    //表头字段
                    for (int i = 0; i < headers.Count; i++)
                    {
                        worksheet.Cells[6, i + 1].Value = headers[i];
                    }
                    var row = 7;
                    var dataList = applyDetials.Where(w => w.no == item.no).ToList();
                    for (int i = 0; i < dataList.Count; i++)
                    {
                        worksheet.Cells[row, 1].Value = i + 1;
                        worksheet.Cells[row, 2].Value = dataList[i].name;
                        worksheet.Cells[row, 3].Value = dataList[i].spec;
                        worksheet.Cells[row, 4].Value = dataList[i].manufactor;
                        worksheet.Cells[row, 5].Value = dataList[i].unit;
                        worksheet.Cells[row, 6].Value = dataList[i].num;
                        worksheet.Cells[row, 7].Value = dataList[i].stocktaking_num;
                        worksheet.Cells[row, 8].Value = dataList[i].num_difference;
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
