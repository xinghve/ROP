using Models.DB;
using Models.View.Business;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace Service.Repository.Interfaces.Business
{
    /// <summary>
    /// 固定资产
    /// </summary>
    public interface IAssetsService
    {
        /// <summary>
        /// 编辑固定资产
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> ModifyAsync(bus_assets entity);

        /// <summary>
        /// 获取固定资产分类列表
        /// </summary>
        /// <returns></returns>
        Task<List<b_codebase>> GetTypeListAsync();

        /// <summary>
        /// 获取基础固定资产列表
        /// </summary>
        /// <returns></returns>
        Task<List<p_std_item>> GetStdListAsync(string name, short type_id);

        /// <summary>
        /// 获得分页列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<Page<bus_assets>> GetPageAsync(AssetsPageSearch entity);

        /// <summary>
        /// 固定资产流向
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<List<AssetsFlow>> GetAssetsFlowAsync(int id);

        /// <summary>
        /// 获取固定资产列表
        /// </summary>
        /// <param name="store_id">门店ID(-1=所有)</param>
        /// <param name="state">状态（30=未使用；31=使用中；44=维修中；32=已报废；41=调拨中；47=已报损；-1=所有）</param>
        /// <param name="std_item_id">基础项目ID</param>
        /// <param name="spec">规格</param>
        /// <param name="manufactor_id">厂家ID</param>
        /// <returns></returns>
        Task<List<bus_assets>> GetListAsync(int store_id, short state, int std_item_id, string spec, int manufactor_id);

        /// <summary>
        /// 获取固资二维码
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        string GetAssetsQrCode(int id, dynamic type);

        /// <summary>
        /// 返回微信
        /// </summary>
        /// <returns></returns>
        object ReturnQr(string type);

        /// <summary>
        /// 扫描二维码返回固资信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bus_assets> ReturnAssets(int id);

    }
}
