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
    /// 收费员交账单--ty
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CheckOutController : ControllerBase
    {
        private readonly ICheckOutService _checkOutService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkOutService"></param>
        public CheckOutController(ICheckOutService checkOutService)
        {
            _checkOutService = checkOutService;
        }
        /// <summary>
        /// 获取门诊收费结算单分页数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetPageAsync")]
        public async Task<Page<FalanceModel>> GetPageAsync([FromQuery]FalanceSearch entity)
        {
            return await _checkOutService.GetPageAsync(entity);
        }

        /// <summary>
        /// 获取收账单分页数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetCheckOutPage")]
        public async Task<Page<CheckOutModelPage>> GetCheckOutPage([FromQuery]CheckOutSearch entity)
        {
            return await _checkOutService.GetCheckOutPage(entity);
        }

        /// <summary>
        /// 根据结账id获取结算分页数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetBalancePage")]
        public async Task<Page<f_balance>> GetBalancePage([FromQuery] FalanceSearch entity)
        {
            return await _checkOutService.GetBalancePage(entity);
        }

        /// <summary>
        /// 交账
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("AccoutAsync")]
        public async Task<bool> AccoutAsync([FromBody]CheckOutModel entity)
        {
            return await _checkOutService.AccoutAsync(entity);
        }
    }
}