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
    /// 诊室
    /// </summary>
    public interface IConsRoomService
    {
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <returns></returns>
        Task<bool> AddAsync(ConsRoomDept entity);

        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <returns></returns>
        Task<bool> ModifyAsync(ConsRoomDept entity);

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <returns></returns>
        Task<int> DeleteAsync(int id);

        /// <summary>
        /// 获得分页列表
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="name">名称</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        Task<Page<ConsRoomDept>> GetPagesAsync(int store_id, string name, string order, int orderType, int limit, int page);

        /// <summary>
        /// 数据列表
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="store_id">门店ID</param>
        /// <param name="dept_id">部门ID</param>
        /// <returns></returns>
        Task<List<ConsRoomDept>> GetListAsync(string name, int store_id, int dept_id);
    }
}
