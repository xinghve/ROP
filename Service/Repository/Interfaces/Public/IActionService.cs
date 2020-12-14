using Models.DB;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tools;

namespace Service.Repository.Interfaces.Public
{
    /// <summary>
    /// 
    /// </summary>
    public interface IActionService
    {
        /// <summary>
        /// 添加功能菜单（异步）
        /// </summary>
        /// <param name="action">功能菜单</param>
        /// <returns></returns>
        Task<int> AddAsync(p_action action);

        /// <summary>
        /// 修改功能菜单（异步）
        /// </summary>
        /// <param name="action">功能菜单</param>
        /// <returns></returns>
        Task<int> ModifyAsync(p_action action);

        /// <summary>
        /// 删除选中所有（异步）
        /// </summary>
        /// <param name="delList">需要删除的id集合</param>
        /// <returns></returns>
        Task<int> DeleteAsync(List<int> delList);

        /// <summary>
        /// 获取菜单树（异步）
        /// </summary>
        /// <returns></returns>
        Task<object> GetMenuTree(int store_id);

        /// <summary>
        /// 获取功能表（异步）
        /// </summary>
        /// <returns></returns>
        Task<object> GetActionTree(int store_id, int setType);

        /// <summary>
        /// 分页方式获取功能表（异步）
        /// </summary>
        /// <param name="parentId">上级Id</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        Task<Page<p_action>> GetPagesAsync(int parentId, string order, int orderType, int limit, int page);
    }
}
