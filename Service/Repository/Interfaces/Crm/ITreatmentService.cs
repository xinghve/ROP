using Models.DB;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tools;

namespace Service.Repository.Interfaces.Crm
{
    /// <summary>
    /// 诊疗方式
    /// </summary>
    public interface ITreatmentService
    {
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <returns></returns>
        Task<int> AddAsync(c_treatment treatment);

        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <returns></returns>
        Task<int> ModifyAsync(c_treatment treatment);

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <returns></returns>
        Task<int> DeleteAsync(List<int> vs);

        /// <summary>
        /// 获得列表
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="store_id">门店ID</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        Task<Page<c_treatment>> GetPagesAsync(string name, int store_id, string order, int orderType, int limit, int page);

        /// <summary>
        /// 数据列表
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="store_id">门店ID</param>
        /// <returns></returns>
        Task<List<c_treatment>> GetListAsync(string name, int store_id);
    }
}
