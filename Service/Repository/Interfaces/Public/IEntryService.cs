using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace Service.Repository.Interfaces.Public
{
    /// <summary>
    /// 词条
    /// </summary>
    public interface IEntryService
    {
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <returns></returns>
        Task<int> AddAsync(p_entry entry);

        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <returns></returns>
        Task<int> ModifyAsync(p_entry entry);

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <returns></returns>
        Task<int> DeleteAsync(List<string> vs);

        /// <summary>
        /// 获得列表
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        Task<Page<p_entry>> GetPagesAsync(string name, string order, int orderType, int limit, int page);

        /// <summary>
        /// 数据列表
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns></returns>
        Task<List<p_entry>> GetListAsync(string name);
    }
}
