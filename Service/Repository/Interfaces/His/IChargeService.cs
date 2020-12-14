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
    /// 收费
    /// </summary>
    public interface IChargeService
    {
        /// <summary>
        /// 获得分页列表（待缴费）
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="archives_id">客户ID</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        Task<Page<ChargePages>> GetPagesAsync(int store_id, int archives_id, string order, int orderType, int limit, int page);

        /// <summary>
        /// 门店缴费
        /// </summary>
        /// <param name="store_id"></param>
        /// <returns></returns>
        Task<List<ChargeArchives>> GetListAsync(int store_id);

        /// <summary>
        /// 收费
        /// </summary>
        /// <returns></returns>
        Task<bool> ChargeAsync(Charge entity);

        /// <summary>
        /// 获得分页列表（已缴费单）
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="archives_id">客户ID</param>
        /// <param name="doctorid">医生ID</param>
        /// <param name="deptid">科室ID</param>
        /// <param name="startapplydate">申请时间（开始）</param>
        /// <param name="endapplydate">申请时间（结束）</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        Task<Page<FalanceModel>> GetBalancePagesAsync(int store_id, int archives_id, int doctorid, int deptid, DateTime? startapplydate, DateTime? endapplydate, string order, int orderType, int limit, int page);

        /// <summary>
        /// 获得分页列表（已缴费单明细）
        /// </summary>
        /// <param name="balanceid">结算ID</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        Task<Page<ChargePages>> GetBalanceDetailPagesAsync(int balanceid, string order, int orderType, int limit, int page);

        /// <summary>
        /// 获得列表（缴费单明细）
        /// </summary>
        /// <param name="balanceid">申请ID</param>
        /// <returns></returns>
        Task<dynamic> GetBalanceDetailListAsync(int balanceid);
    }
}
