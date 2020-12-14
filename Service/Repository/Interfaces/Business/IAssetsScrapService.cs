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
    /// 固定资产报废
    /// </summary>
    public interface IAssetsScrapService
    {
        /// <summary>
        /// 新增资产报废
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> AddAsync(r_assets_scrap entity);

        /// <summary>
        /// 取消资产报废
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> CancelAsync(r_assets_scrap entity);

        /// <summary>
        /// 资产报废单分页
        /// </summary>
        /// <returns></returns>
        Task<Page<AssetsScrap>> GetPageAsync(AssetsScrapPageSearch entity);

        /// <summary>
        /// 获取报废单信息
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        Task<AssetsScrap> GetAssetsScrapAsync(string no);

        /// <summary>
        /// 导出表格
        /// </summary>
        /// <param name="list_no"></param>
        /// <returns></returns>
        Task<string> ExportAsync(List<string> list_no);
    }
}
