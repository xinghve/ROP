using Models.DB;
using Models.View.His;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace Service.Repository.Interfaces.His
{
    /// <summary>
    /// 项目规格
    /// </summary>
    public interface IItemSpecService
    {
        /// <summary>
        /// 获取项目规格分页数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<Page<h_itemspec>> GetPageAsync(ItemSpecSearch model);
        /// <summary>
        /// 添加项目规格
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> AddAsync(h_itemspec entity);

        /// <summary>
        /// 编辑项目规格
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> ModifyAsync(h_itemspec entity);

        /// <summary>
        /// 删除规格
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<int> DeleteAsync(int id);

        /// <summary>
        /// 数据列表
        /// </summary>
        /// <param name="item_id">项目ID</param>
        /// <returns></returns>
        Task<dynamic> GetListAsync(int item_id);
    }
}
