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
    /// 物资类型
    /// </summary>
    public interface IGoodsTypeService
    {
        /// <summary>
        /// 添加物资分类
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> AddAsync(b_codebase entity);

        /// <summary>
        /// 编辑物资分类
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> ModifyAsync(b_codebase entity);

        /// <summary>
        /// 删除物资分类
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> DeleteAsync(b_codebase entity);

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        Task<List<b_codebase>> GetListAsync(int catalog_id);

        /// <summary>
        /// 物资分类分页查询
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<Page<b_codebase>> GetPagesAsync(GoodsTypePageSearch entity);


        /// <summary>
        /// 添加物资目录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> AddCatalogAsync(b_catalog entity);

        /// <summary>
        /// 编辑物资目录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> ModifyCatalogAsync(b_catalog entity);

        /// <summary>
        /// 删除物资目录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> DeleteCatalogAsync(b_catalog entity);

        /// <summary>
        /// 获取目录列表
        /// </summary>
        /// <returns></returns>
        Task<dynamic> GetCatalogListAsync();

        /// <summary>
        /// 物资目录分页查询
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<Page<b_catalog>> GetCatalogPagesAsync(CatalogPageSearch entity);
    }
}
