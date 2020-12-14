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
    /// 固定资产折旧
    /// </summary>
    public interface IAssetsDepreciationService
    {
        /// <summary>
        /// 新增资产折旧
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> AddAsync(AssetsDepreciation entity);

        /// <summary>
        /// 编辑资产折旧
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> ModifyAsync(AssetsDepreciation entity);

        /// <summary>
        /// 删除资产折旧
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync(bus_assets_depreciation entity);

        /// <summary>
        /// 资产折旧单分页
        /// </summary>
        /// <returns></returns>
        Task<Page<AssetsDepreciation>> GetPageAsync(AssetsDepreciationPageSearch entity);

        /// <summary>
        /// 导出表格
        /// </summary>
        /// <param name="list_no"></param>
        /// <returns></returns>
        Task<string> ExportAsync(List<string> list_no);
    }
}
