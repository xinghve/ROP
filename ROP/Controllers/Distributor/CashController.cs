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
using Tools.Authorize;

namespace ROP.Controllers.Distributor
{
    /// <summary>
    /// 提现管理
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeForDistributor]
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
        /// 提现分页记录(分销人员端)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetPagesByDistributor")]
        public async Task<dynamic> GetPagesByDistributorAsync([FromQuery]Search entity)
        {
            return await _cashService.GetPagesByDistributorAsync(entity);
        }

        /// <summary>
        /// 提现（分销人员端）
        /// </summary>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<bool> AddAsync([FromBody]r_cash_withdrawal entity)
        {
            return await _cashService.AddAsync(entity);
        }
    }
}