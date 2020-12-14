using Models.DB;
using System.Threading.Tasks;
using Tools;

namespace Service.Repository.Interfaces.Crm
{
    /// <summary>
    /// 客户关系维护
    /// </summary>
    public interface IMaintainService
    {
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <returns></returns>
        Task<bool> AddAsync(c_maintain maintain);

        /// <summary>
        /// 获得列表
        /// </summary>
        /// <param name="name">名称/手机号/身份证</param>
        /// <param name="store_id">门店ID</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        Task<Page<c_maintain>> GetPagesAsync(string name, int store_id, string order, int orderType, int limit, int page);
    }
}
