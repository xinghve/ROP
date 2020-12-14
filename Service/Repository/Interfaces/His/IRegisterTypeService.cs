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
    /// 挂号类别
    /// </summary>
    public interface IRegisterTypeService
    {
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <returns></returns>
        Task<bool> AddAsync(RegisterType entity);

        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <returns></returns>
        Task<bool> ModifyAsync(RegisterType entity);

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <returns></returns>
        Task<int> DeleteAsync(int id);

        /// <summary>
        /// 获得分页列表
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="orderflag">预约标志</param>
        /// <param name="stateid">状态ID</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        Task<Page<RegisterType>> GetPagesAsync(string name, int orderflag, int stateid,  string order, int orderType, int limit, int page);

        /// <summary>
        /// 数据列表
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="orderflag">预约标志</param>
        /// <param name="stateid">状态ID</param>
        /// <returns></returns>
        Task<List<RegisterType>> GetListAsync(string name, int orderflag, int stateid);
    }
}
