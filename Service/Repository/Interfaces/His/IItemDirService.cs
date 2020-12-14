using Models.DB;
using Models.View.His;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.Repository.Interfaces.His
{
    /// <summary>
    /// 项目目录接口
    /// </summary>
    public interface IItemDirService
    {
        /// <summary>
        /// 添加项目目录（异步）
        /// </summary>
        /// <param name="itemdir">项目目录</param>
        /// <returns></returns>
        Task<int> AddAsync(h_itemdir itemdir);

        /// <summary>
        /// 编辑项目目录（异步）
        /// </summary>
        /// <param name="itemdir">项目目录</param>
        /// <returns></returns>
        Task<int> ModifyAsync(h_itemdir itemdir);

        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// 获取目录树
        /// </summary>
        /// <returns></returns>
        Task<object> GetItemTree();

        /// <summary>
        /// 获取项目类别
        /// </summary>
        /// <param name="typeId">没有传-1</param>
        /// <param name="parentId">父级id</param>
        /// <returns></returns>
        Task<List<b_basecode>> GetItemType( int typeId,int parentId);

        /// <summary>
        /// 导入目录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> ImportDir(List<h_itemdir> entity);

        /// <summary>
        /// 导入所有项目
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> ImportAllItem(List<ItemAllModel> entity);

    }
}
