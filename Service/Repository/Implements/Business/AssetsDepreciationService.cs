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
    /// 固定资产折旧
    /// </summary>
    public class AssetsDepreciationService : DbContext, IAssetsDepreciationService
    {
        /// <summary>
        /// 新增资产折旧
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> AddAsync(AssetsDepreciation entity)
        {

            var result = await Db.Ado.UseTranAsync(() =>
            {
                if (entity.assets_Depreciation_Detials.Count == 0)
                {
                    throw new MessageException("请选择固定资产进行折旧");
                }

                var assets_ids = entity.assets_Depreciation_Detials.Select(s => s.assets_id).ToList();
                Depreciation(entity, assets_ids);
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        private void Depreciation(AssetsDepreciation entity, List<int> assets_ids)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            var assets_list = Db.Queryable<bus_assets>().Where(w => assets_ids.Contains(w.id)).WithCache().ToList();

            //折旧单
            var assets_depreciation = new bus_assets_depreciation();
            assets_depreciation.creater = userInfo.name;
            assets_depreciation.creater_id = userInfo.id;
            assets_depreciation.create_time = DateTime.Now;
            assets_depreciation.date = entity.date;
            assets_depreciation.org_id = userInfo.org_id;
            assets_depreciation.remark = entity.remark;
            assets_depreciation.store_id = entity.store_id;
            assets_depreciation.store_name = entity.store_name;
            assets_depreciation.no = "ZJ" + userInfo.org_id + DateTime.Now.ToString("yyMMdd");
            //查询最大单号
            var max_no = Db.Queryable<bus_assets_depreciation>().Where(w => w.no.StartsWith(assets_depreciation.no) && w.org_id == userInfo.org_id).OrderBy(o => o.no, OrderByType.Desc).Select(s => s.no).WithCache().First();
            if (max_no == null)
            {
                assets_depreciation.no += "0001";
            }
            else
            {
                max_no = max_no.Substring(assets_depreciation.no.Length);
                assets_depreciation.no += (int.Parse(max_no) + 1).ToString().PadLeft(4, '0');
            }
            Db.Insertable(assets_depreciation).ExecuteCommand();
            redisCache.RemoveAll<bus_assets_depreciation>();

            //折旧单明细
            var assets_depreciation_detials_list = new List<bus_assets_depreciation_detials>();
            entity.assets_Depreciation_Detials.ForEach(item =>
            {
                //修改固定资产折旧
                var assets = assets_list.Where(w => w.id == item.assets_id).First();
                assets.remaining_depreciation -= item.depreciation;
                assets.net_residual -= item.depreciation;
                assets.total_depreciation += item.depreciation;
                Db.Updateable<bus_assets>().SetColumns(s => new bus_assets { remaining_depreciation = assets.remaining_depreciation, net_residual = assets.net_residual, total_depreciation = assets.total_depreciation }).Where(w => w.id == item.assets_id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                //添加明细
                assets_depreciation_detials_list.Add(new bus_assets_depreciation_detials { assets_id = item.assets_id, assets_no = assets.no, depreciation = item.depreciation, manufactor = assets.manufactor, manufactor_id = assets.manufactor_id, name = assets.name, no = assets_depreciation.no, spec = assets.spec, type = assets.type, type_id = assets.type_id, buy_price = assets.buy_price, net_residual = assets.net_residual, remaining_depreciation = assets.remaining_depreciation, total_depreciation = assets.total_depreciation });
            });
            Db.Insertable(assets_depreciation_detials_list).ExecuteCommand();
            redisCache.RemoveAll<bus_assets_depreciation_detials>();
        }

        /// <summary>
        /// 删除资产折旧
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(bus_assets_depreciation entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            var result = await Db.Ado.UseTranAsync(() =>
            {
                //查询折旧单
                var assets_depreciation = Db.Queryable<bus_assets_depreciation>().Where(w => w.no == entity.no).WithCache().First();
                if (assets_depreciation.creater_id != userInfo.id)
                {
                    throw new MessageException("只能删除自己添加的折旧单");
                }

                //折旧单明细还原资产
                var old_assets_depreciation_detials_list = Db.Queryable<bus_assets_depreciation_detials>().Where(w => w.no == entity.no).WithCache().ToList();

                var old_assets_ids = old_assets_depreciation_detials_list.Select(s => s.assets_id).ToList();

                var old_assets_list = Db.Queryable<bus_assets>().Where(w => old_assets_ids.Contains(w.id)).WithCache().ToList();

                old_assets_depreciation_detials_list.ForEach(item =>
                {
                    //修改固定资产折旧
                    var assets = old_assets_list.Where(w => w.id == item.assets_id).First();
                    assets.remaining_depreciation += item.depreciation;
                    assets.net_residual += item.depreciation;
                    assets.total_depreciation -= item.depreciation;
                    Db.Updateable<bus_assets>().SetColumns(s => new bus_assets { remaining_depreciation = assets.remaining_depreciation, net_residual = assets.net_residual, total_depreciation = assets.total_depreciation }).Where(w => w.id == item.assets_id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                });

                //删除折旧单
                Db.Deleteable<bus_assets_depreciation>().Where(w => w.no == entity.no).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 资产折旧单分页
        /// </summary>
        /// <returns></returns>
        public async Task<Page<AssetsDepreciation>> GetPageAsync(AssetsDepreciationPageSearch entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            //查询集团下所有门店
            var stores = await Db.Queryable<p_store>().Where(w => w.org_id == userInfo.org_id).WithCache().ToListAsync();
            //查询集团下所有资产折旧
            var depreciations = await Db.Queryable<bus_assets_depreciation>().Where(w => w.org_id == userInfo.org_id).WithCache().ToListAsync();
            //获取折旧id
            var depreciations_nos = depreciations.Select(s => s.no).ToList();
            //查询集团下所有资产折旧明细
            var depreciations_detials = await Db.Queryable<bus_assets_depreciation_detials>().Where(w => depreciations_nos.Contains(w.no)).WithCache().ToListAsync();

            var result = await Db.Ado.UseTranAsync(() =>
            {
                stores.ForEach(item =>
                {
                    //查询当前门店是否在本月已折旧
                    var isExiste = depreciations.Any(w => w.org_id == userInfo.org_id && w.store_id == item.id && w.date >= DateTime.Now.Date.AddDays(1 - DateTime.Now.Date.Day));
                    if (!isExiste)
                    {
                        //获取门店折旧单
                        var depreciation = depreciations.Where(w => w.store_id == item.id).OrderByDescending(o => o.date).FirstOrDefault();
                        if (depreciation != null)
                        {
                            //获取门店折旧明细
                            var depreciation_detials = depreciations_detials.Where(w => w.no == depreciation.no).ToList();
                            //获取固定资产id
                            var assets_ids = depreciation_detials.Select(s => s.assets_id).ToList();
                            var assetsDepreciation = new AssetsDepreciation { date = DateTime.Now, remark = "自动折旧", store_id = item.id, store_name = item.name, assets_Depreciation_Detials = depreciation_detials };
                            Depreciation(assetsDepreciation, assets_ids);
                        }
                    }
                });
            });
            //if (!result.IsSuccess)
            //{
            //    throw new MessageException(result.ErrorMessage);
            //}

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
            var pages = await Db.Queryable<bus_assets_depreciation>()
                            .Where(w => w.org_id == userInfo.org_id)
                            .WhereIF(!string.IsNullOrEmpty(entity.no), w => w.no.Contains(entity.no))
                            .WhereIF(entity.store_id != -1, w => w.store_id == entity.store_id)
                            .WhereIF(entity.start_date != null, w => w.date >= entity.start_date)
                            .WhereIF(entity.end_date != null, w => w.date < entity.end_date)
                            .Select(s => new AssetsDepreciation { creater = s.creater, store_id = s.store_id, creater_id = s.creater_id, create_time = s.create_time, date = s.date, no = s.no, org_id = s.org_id, remark = s.remark, store_name = s.store_name })
                            .OrderBy(entity.order + orderTypeStr)
                            .WithCache()
                            .ToPageAsync(entity.page, entity.limit);

            //列表中的折旧单号
            var nos = pages.Items.Select(s => s.no).ToList();

            //查询折旧单明细
            var assets_depreciation_detials_list = await Db.Queryable<bus_assets_depreciation_detials>().Where(w => nos.Contains(w.no)).WithCache().ToListAsync();

            //赋值调拨明细
            pages.Items = pages.Items.Select(s => new AssetsDepreciation { creater = s.creater, store_id = s.store_id, creater_id = s.creater_id, create_time = s.create_time, date = s.date, no = s.no, org_id = s.org_id, remark = s.remark, store_name = s.store_name, assets_Depreciation_Detials = assets_depreciation_detials_list.Where(w => w.no == s.no).ToList() }).ToList();

            return pages;
        }

        /// <summary>
        /// 编辑资产折旧
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> ModifyAsync(AssetsDepreciation entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            var result = await Db.Ado.UseTranAsync(() =>
            {
                if (entity.assets_Depreciation_Detials.Count == 0)
                {
                    throw new MessageException("请选择固定资产进行折旧");
                }

                //查询折旧单
                var assets_depreciation = Db.Queryable<bus_assets_depreciation>().Where(w => w.no == entity.no).WithCache().First();
                if (assets_depreciation.creater_id != userInfo.id)
                {
                    throw new MessageException("只能编辑自己添加的折旧单");
                }

                //修改折旧单
                Db.Updateable<bus_assets_depreciation>().SetColumns(s => new bus_assets_depreciation { date = entity.date, remark = entity.remark }).Where(w => w.no == entity.no).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                //折旧单明细还原资产
                var old_assets_depreciation_detials_list = Db.Queryable<bus_assets_depreciation_detials>().Where(w => w.no == entity.no).WithCache().ToList();

                var old_assets_ids = old_assets_depreciation_detials_list.Select(s => s.assets_id).ToList();

                var old_assets_list = Db.Queryable<bus_assets>().Where(w => old_assets_ids.Contains(w.id)).WithCache().ToList();

                old_assets_depreciation_detials_list.ForEach(item =>
                {
                    //修改固定资产折旧
                    var assets = old_assets_list.Where(w => w.id == item.assets_id).First();
                    assets.remaining_depreciation += item.depreciation;
                    assets.net_residual += item.depreciation;
                    assets.total_depreciation -= item.depreciation;
                    Db.Updateable<bus_assets>().SetColumns(s => new bus_assets { remaining_depreciation = assets.remaining_depreciation, net_residual = assets.net_residual, total_depreciation = assets.total_depreciation }).Where(w => w.id == item.assets_id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                });
                //删除折旧单明细
                Db.Deleteable<bus_assets_depreciation_detials>().Where(w => w.no == entity.no).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();


                var assets_ids = entity.assets_Depreciation_Detials.Select(s => s.assets_id).ToList();

                var assets_list = Db.Queryable<bus_assets>().Where(w => assets_ids.Contains(w.id)).WithCache().ToList();

                var assets_depreciation_detials_list = new List<bus_assets_depreciation_detials>();
                entity.assets_Depreciation_Detials.ForEach(item =>
                {
                    //修改固定资产折旧
                    var assets = assets_list.Where(w => w.id == item.assets_id).First();
                    assets.remaining_depreciation -= item.depreciation;
                    assets.net_residual -= item.depreciation;
                    assets.total_depreciation += item.depreciation;
                    Db.Updateable<bus_assets>().SetColumns(s => new bus_assets { remaining_depreciation = assets.remaining_depreciation, net_residual = assets.net_residual, total_depreciation = assets.total_depreciation }).Where(w => w.id == item.assets_id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                    //添加明细
                    assets_depreciation_detials_list.Add(new bus_assets_depreciation_detials { assets_id = item.assets_id, assets_no = assets.no, depreciation = item.depreciation, manufactor = assets.manufactor, manufactor_id = assets.manufactor_id, name = assets.name, no = entity.no, spec = assets.spec, type = assets.type, type_id = assets.type_id });
                });
                Db.Insertable(assets_depreciation_detials_list).ExecuteCommand();
                redisCache.RemoveAll<bus_assets_depreciation_detials>();
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 导出折旧
        /// </summary>
        /// <param name="list_no"></param>
        /// <returns></returns>
        public async Task<string> ExportAsync(List<string> list_no)
        {
            if (list_no.Count == 0)
            {
                throw new MessageException("请选择折旧单进行导出");
            }


            var applys = await Db.Queryable<bus_assets_depreciation>()
                            .Where(w => list_no.Contains(w.no))
                            .WithCache().ToListAsync();

            //申请单号对应的所有明细
            var applyDetials = await Db.Queryable<bus_assets_depreciation_detials>()
                            .Where(w => list_no.Contains(w.no))
                            .WithCache().ToListAsync();

            //定义表头
            var headers = new List<string>() { "序号", "编号", "名称", "规格", "厂家", "资产类别", "折旧金额", "原值", "累计折旧", "剩余可折", "净剩值" };

            string sWebRootFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot\\tempExcel");//如果用浏览器url下载的方式  存放excel的文件夹一定要建在网站首页的同级目录下！！！
            if (!Directory.Exists(sWebRootFolder))
            {
                Directory.CreateDirectory(sWebRootFolder);
            }

            //查询用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            string sFileName = $@"depreciation_{userInfo.id}.xlsx";
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
                    worksheet.Cells[1, 1].Value = "固定资产折旧单";
                    worksheet.Cells[1, 1].Style.Font.Size = 20;
                    worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[3, 1, 3, 3].Merge = true;
                    worksheet.Cells[3, 1].Value = $"单号：{item.no}";
                    worksheet.Cells[3, 4, 3, 6].Merge = true;
                    worksheet.Cells[3, 4].Value = $"折旧月份：{item.date.Value.ToString("yyyy年MM月")}";
                    worksheet.Cells[3, 7, 3, 9].Merge = true;
                    worksheet.Cells[3, 7].Value = $"制单门店：{item.store_name}";
                    worksheet.Cells[4, 1, 4, 3].Merge = true;
                    worksheet.Cells[4, 1].Value = $"制单人：{item.creater}";
                    worksheet.Cells[4, 4, 4, 6].Merge = true;
                    worksheet.Cells[4, 6].Value = $"制单时间：{item.create_time.Value.ToString("yyyy年MM月dd日")}";
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
                        worksheet.Cells[row, 2].Value = dataList[i].assets_no;
                        worksheet.Cells[row, 3].Value = dataList[i].name;
                        worksheet.Cells[row, 4].Value = dataList[i].spec;
                        worksheet.Cells[row, 5].Value = dataList[i].manufactor;
                        worksheet.Cells[row, 6].Value = dataList[i].type;
                        worksheet.Cells[row, 7].Value = dataList[i].depreciation;
                        worksheet.Cells[row, 8].Value = dataList[i].buy_price;
                        worksheet.Cells[row, 9].Value = dataList[i].total_depreciation;
                        worksheet.Cells[row, 10].Value = dataList[i].remaining_depreciation;
                        worksheet.Cells[row, 11].Value = dataList[i].net_residual;
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
