using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Models.View.Public;
using Service.Repository.Interfaces.Public;
using Tools;

namespace ROP.Controllers.Public
{
    /// <summary>
    /// 提现管理
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CashController : ControllerBase
    {
        private readonly ICashService _cashService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cashService"></param>
        public CashController(ICashService cashService)
        {
            _cashService = cashService;
        }

        /// <summary>
        /// 提现分页数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetPageAsync")]
        public async Task<Page<CashExtent>> GetPageAsync([FromQuery]CashSearch entity)
        {
            return await _cashService.GetPageAsync(entity);
        }

        /// <summary>
        /// 提现审核
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("Check")]
        public async Task<int> Check([FromBody]r_cash_withdrawal entity)
        {
            return await _cashService.Check(entity);
        }

        /// <summary>
        /// 提现转账
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("Transfer")]
        public async Task<int> Transfer([FromBody]r_cash_withdrawal entity)
        {
            return await _cashService.Transfer(entity);
        }

        /// <summary>
        /// 设置提现上下限
        /// </summary>
        /// <returns></returns>
        [HttpPut("ModifyLowerUper")]
        public async Task<int> ModifyLowerUper(CashLower entity)
        {
            return await _cashService.ModifyLowerUper(entity);
        }

        /// <summary>
        /// 获取提现上下限
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetLowerUper")]
        public async Task<object> GetLowerUper()
        {
            return await _cashService.GetLowerUper();
        }
    }
}