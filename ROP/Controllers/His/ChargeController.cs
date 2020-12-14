using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Models.View.His;
using Service.Repository.Interfaces.His;
using Tools;

namespace ROP.Controllers.His
{
    /// <summary>
    /// 收费结算
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ChargeController : ControllerBase
    {
        private readonly IChargeService _chargeService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chargeService"></param>
        public ChargeController(IChargeService chargeService)
        {
            _chargeService = chargeService;
        }

        /// <summary>
        /// 收费
        /// </summary>
        /// <returns></returns>
        [HttpPost("Charge")]
        public async Task<bool> ChargeAsync([FromBody]Charge entity)
        {
            return await _chargeService.ChargeAsync(entity);
        }

        /// <summary>
        /// 门店待结算
        /// </summary>
        /// <param name="store_id"></param>
        /// <returns></returns>
        [HttpGet("GetList")]
        public async Task<List<ChargeArchives>> GetListAsync([FromQuery] int store_id)
        {
            return await _chargeService.GetListAsync(store_id);
        }

        /// <summary>
        /// 获得分页列表（待结算）
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="archives_id">客户ID</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        [HttpGet("GetPages")]
        public async Task<Page<ChargePages>> GetPagesAsync([FromQuery]int store_id, [FromQuery] short archives_id, [FromQuery]string order, [FromQuery]int orderType, [FromQuery] int limit, [FromQuery] int page)
        {
            return await _chargeService.GetPagesAsync(store_id, archives_id, order, orderType, limit, page);
        }

        /// <summary>
        /// 获得分页列表（已结算单）
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="archives_id">客户ID</param>
        /// <param name="doctorid">医生ID</param>
        /// <param name="deptid">科室ID</param>
        /// <param name="startbalancedate">结算时间（开始）</param>
        /// <param name="endbalancedate">结算时间（结束）</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        [HttpGet("GetBalancePages")]
        public async Task<Page<FalanceModel>> GetBalancePagesAsync([FromQuery]int store_id, [FromQuery]int archives_id, [FromQuery]int doctorid, [FromQuery] int deptid, DateTime? startbalancedate, [FromQuery] DateTime? endbalancedate, [FromQuery]string order, [FromQuery] int orderType, [FromQuery]int limit, [FromQuery] int page)
        {
            return await _chargeService.GetBalancePagesAsync(store_id, archives_id, doctorid, deptid, startbalancedate, endbalancedate, order, orderType, limit, page);
        }

        /// <summary>
        /// 获得分页列表（已结算单明细）
        /// </summary>
        /// <param name="balanceid">结算ID</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        [HttpGet("GetBalanceDetailPages")]
        public async Task<Page<ChargePages>> GetBalanceDetailPagesAsync([FromQuery]int balanceid, [FromQuery]string order, [FromQuery] int orderType, [FromQuery]int limit, [FromQuery] int page)
        {
            return await _chargeService.GetBalanceDetailPagesAsync(balanceid, order, orderType, limit, page);
        }

        /// <summary>
        /// 获得列表（已结算单明细）
        /// </summary>
        /// <param name="balanceid">结算ID</param>
        /// <returns></returns>
        [HttpGet("GetBalanceDetailList")]
        public async Task<dynamic> GetBalanceDetailListAsync([FromQuery]int balanceid)
        {
            return await _chargeService.GetBalanceDetailListAsync(balanceid);
        }
    }
}