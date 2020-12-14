using Models.DB;
using Models.View.Public;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace Service.Repository.Interfaces.Public
{
    /// <summary>
    /// 基础物资
    /// </summary>
    public interface IStdItemService
    {
        /// <summary>
        /// 根据基础物资ID获取基础物资信息
        /// </summary>
        /// <param name="id">基础物资ID</param>
        /// <returns></returns>
        Task<StdItem> GetAsync(int id);

        /// <summary>
        /// 添加基础物资
        /// </summary>
        /// <param name="entity">基础物资实体</param>
        /// <returns></returns>
        Task<bool> AddAsync(StdItem entity);

        /// <summary>
        /// 编辑基础物资
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> ModifyAsync(StdItem entity);

        /// <summary>
        /// 获得分页列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<Page<StdItem>> GetPageAsync(StdItemPageSearch entity);

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <returns></returns>
        Task<int> ModifyStatusAsync(p_std_item entity);

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <returns></returns>
        Task<int> DeleteAsync(List<int> vs);

        /// <summary>
        /// 获取基础物资列表
        /// </summary>
        /// <returns></returns>
        Task<List<StdItem>> GetListAsync(string name, short type_id);

        /// <summary>
        /// 获取物资明细列表
        /// </summary>
        Task<List<p_std_item_detials>> GetDetialListAsync(int id);

        /// <summary>
        /// 获取物资明细（规格对应明细）
        /// </summary>
        /// <param name="id">物资基础ID</param>
        /// <returns></returns>
        Task<List<StdItemDetials>> GetSpecDetialsAsync(int id);

        /// <summary>
        /// 根据分类获取物资
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<List<p_std_item>> GetStdByClass(string name,int id);

        /// <summary>
        /// 获取规格明细加可用数量
        /// </summary>
        /// <param name="id">基础id</param>
        /// <param name="store_id"></param>
        /// <returns></returns>
        Task<List<StdItemDetials>> GetSpecDetialsNumAsync(int id,int store_id);
    }
}
