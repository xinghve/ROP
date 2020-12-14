using Models.DB;
using Models.View.Business;
using Senparc.Weixin.MP.Helpers;
using Service.Extensions;
using Service.Repository.Interfaces.Business;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;
using Tools.Filter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Service.Repository.Implements.Business
{
    /// <summary>
    /// 固定资产
    /// </summary>
    public class AssetsService : DbContext, IAssetsService
    {
        /// <summary>
        /// 获取固定资产分类列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<b_codebase>> GetTypeListAsync()
        {
            return await Db.Queryable<b_codebase>().Where(w => w.category_id == 16 && w.property_id == 1).WithCache().ToListAsync();
        }

        /// <summary>
        /// 获取基础物资列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<p_std_item>> GetStdListAsync(string name, short type_id)
        {
            //获取固定资产基础项目ID
            return await Db.Queryable<p_std_item, b_codebase>((si, cb) => new object[] { JoinType.Left, si.type_id == cb.id }).Where((si, cb) => cb.property_id == 1).WhereIF(!string.IsNullOrEmpty(name), (si, cb) => si.name.Contains(name)).WhereIF(type_id > 0, (si, cb) => si.type_id == type_id).WithCache().Select((si, cb) => si).ToListAsync();
        }

        /// <summary>
        /// 获得分页列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Page<bus_assets>> GetPageAsync(AssetsPageSearch entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            //获取分页列表
            return await Db.Queryable<bus_assets>()
                            .Where(w => w.org_id == userInfo.org_id)
                            .WhereIF(entity.type_id != -1, w => w.type_id == entity.type_id)
                            .WhereIF(entity.state > 0, w => w.state == entity.state)
                            .WhereIF(entity.store_id != -1, w => w.store_id == entity.store_id)
                            .WhereIF(entity.std_item_id > 0, w => w.std_item_id == entity.std_item_id)
                            .WhereIF(!string.IsNullOrEmpty(entity.no), w => w.no.Contains(entity.no))
                            .OrderBy(entity.order + orderTypeStr)
                            .WithCache()
                            .ToPageAsync(entity.page, entity.limit);
        }

        /// <summary>
        /// 固定资产流向
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<AssetsFlow>> GetAssetsFlowAsync(int id)
        {
            //调拨单
            var transfer_list = await Db.Queryable<bus_transfer_bill>().WithCache().ToListAsync();
            var list = new List<AssetsFlow>();
            //入库
            var put_in_list = await Db.Queryable<bus_put_in_assets, bus_put_in_storage>((pia, pis) => new object[] { JoinType.Left, pia.bill_no == pis.bill_no }).Where((pia, pis) => pia.assets_id == id && pis.state == 40).Select((pia, pis) => new { pia, pis }).WithCache().ToListAsync();

            put_in_list.ForEach(item =>
            {
                var type = item.pis.type_id;
                var transfer_type = "";
                if (type == 2)
                {
                    type = 3;
                    var transfer = transfer_list.Where(w => w.bill_no == item.pis.realted_no).FirstOrDefault();
                    if (string.IsNullOrEmpty(transfer.apply_no))
                    {
                        transfer_type = "申请调拨";
                    }
                    else
                    {
                        transfer_type = "采购转调拨";
                    }
                }
                else
                {
                    type = 1;
                }
                list.Add(new AssetsFlow { datetime = item.pis.put_in_time.Value.ToString("yyyy-MM-dd HH:mm:ss"), employee = item.pis.creater, realted_no = item.pis.bill_no, remark = item.pis.remark, store = item.pis.store_name, type = type, transfer_type = transfer_type });
            });

            //出库
            var out_list = await Db.Queryable<bus_out_assets, bus_out_storage>((oa, os) => new object[] { JoinType.Left, oa.bill_no == os.bill_no }).Where((oa, os) => oa.assets_id == id && os.state == 42).Select((oa, os) => new { oa, os }).WithCache().ToListAsync();

            out_list.ForEach(item =>
            {
                var transfer_type = "采购转调拨";
                var transfer = transfer_list.Where(w => w.bill_no == item.os.realted_no).FirstOrDefault();
                if (string.IsNullOrEmpty(transfer.apply_no))
                {
                    transfer_type = "申请调拨";
                }
                list.Add(new AssetsFlow { datetime = item.os.out_time.Value.ToString("yyyy-MM-dd HH:mm:ss"), employee = item.os.creater, realted_no = item.os.bill_no, remark = item.os.remark, store = item.os.store_name, type = 2, transfer_type = transfer_type });
            });

            //领用
            //var requisitions_list = await Db.Queryable<bus_requisitions_detail, bus_requisitions_bill>((rd, rb) => new object[] { JoinType.Left, rd.bill_no == rb.bill_no }).Where((rd, rb) => rd.relation_id == id && rb.state == 15).Select((rd, rb) => new { rd, rb }).WithCache().ToListAsync();

            //requisitions_list.ForEach(item =>
            //{
            ////    list.Add(new AssetsFlow { datetime = item.rb.provide_time.Value.ToString("yyyy-MM-dd HH:mm:ss"), employee = item.rb.provider, remark = item.rb.remark, store = item.rb.store_name, type = 4, dept = item.rb.dept_name, requisitions_employee = item.rb.creater });
            //});

            //归还

            //维修

            //报废

            list = list.OrderByDescending(o => o.datetime).ToList();
            return list;
        }

        /// <summary>
        /// 编辑固定资产
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> ModifyAsync(bus_assets entity)
        {
            if (entity.net_salvage_rate > 100)
            {
                throw new MessageException("净残值率不能大于100%");
            }
            if (entity.buy_price < 0)
            {
                throw new MessageException("原值不能小于0");
            }
            if (entity.year_num <= 0)
            {
                throw new MessageException("使用年限必须大于0");
            }
            var assets = await Db.Queryable<bus_assets>().Where(w => w.id == entity.id).WithCache().FirstAsync();
            entity.net_salvage = entity.buy_price.Value * entity.net_salvage_rate / 100;
            entity.depreciation = entity.buy_price.Value * (100 - entity.net_salvage_rate) / 100;
            entity.remaining_depreciation = entity.buy_price.Value * (100 - entity.net_salvage_rate) / 100 - assets.total_depreciation;
            entity.net_residual = entity.buy_price.Value - assets.total_depreciation;
            entity.month_depreciation = (100 - entity.net_salvage_rate) / entity.year_num.Value / 100 / 12 * entity.buy_price.Value;
            return await Db.Updateable<bus_assets>().SetColumns(s => new bus_assets { year_num = entity.year_num, net_salvage_rate = entity.net_salvage_rate, responsible_employee_id = entity.responsible_employee_id, responsible_employee = entity.responsible_employee, buy_price = entity.buy_price, depreciation = entity.depreciation, net_salvage = entity.net_salvage, remaining_depreciation = entity.remaining_depreciation, net_residual = entity.net_residual, month_depreciation = entity.month_depreciation }).Where(w => w.id == entity.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
        }

        /// <summary>
        /// 获取固定资产列表
        /// </summary>
        /// <param name="store_id">门店ID(-1=所有)</param>
        /// <param name="state">状态（30=未使用；31=使用中；44=维修中；32=已报废；41=调拨中；47=已报损；-1=所有）</param>
        /// <param name="std_item_id">基础项目ID</param>
        /// <param name="spec">规格</param>
        /// <param name="manufactor_id">厂家ID</param>
        /// <returns></returns>
        public async Task<List<bus_assets>> GetListAsync(int store_id, short state,int std_item_id, string spec, int manufactor_id)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            return await Db.Queryable<bus_assets>().Where(w => w.org_id == userInfo.org_id).WhereIF(store_id != -1, w => w.store_id == store_id).WhereIF(state != -1, w => w.state == state).WhereIF(std_item_id != -1, w => w.std_item_id == std_item_id).WhereIF(!string.IsNullOrEmpty(spec), w => w.spec == spec).WhereIF(manufactor_id != -1, w => w.manufactor_id == manufactor_id).WithCache().ToListAsync();
        }

        /// <summary>
        /// 获取固资二维码
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetAssetsQrCode(int id, dynamic type)
        {
            //获取用户信息
            //var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var code_url = ConfigExtensions.Configuration["BaseConfig:QrCodeUrl"];
            return Tools.Utils.GetQrCode(id.ToString(), type);
        }


        /// <summary>
        /// 返回微信扫一扫所需
        /// </summary>
        /// <returns></returns>
        public object ReturnQr(string type)
        {
            var appid = ConfigExtensions.Configuration["SenparcWeixinSetting:WeixinAppId"];
            var appSecret = ConfigExtensions.Configuration["SenparcWeixinSetting:WeixinAppSecret"];
            //签名
            var signature=  JSSDKHelper.GetJsSdkUiPackage(appid, appSecret,type);

            return  new { signature.AppId, signature.NonceStr, signature.Signature, signature.Timestamp };
        }

        /// <summary>
        /// 扫描二维码返回固资信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bus_assets> ReturnAssets(int id)
        {
            if (id<=0)
            {
                throw new MessageException("没有相关信息");
            }

            return await Db.Queryable<bus_assets>()
                           .Where(b => b.id == id)
                           .WithCache()
                           .FirstAsync();
        }

    }
}
