using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.View.Public;
using Service.Repository.Interfaces.Public;
using Tools;

namespace ROP.Controllers.Public
{
    /// <summary>
    /// 提成记录
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AmountController : ControllerBase
    {
        private readonly IAmountService _amountService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amountService"></param>
        public AmountController(IAmountService amountService)
        {
            _amountService = amountService;
        }

        /// <summary>
        /// 提成分页数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetPageAsync")]
        public async Task<Page<AmountExpent>> GetPageAsync([FromQuery]AmountSearch entity)
        {
            return await _amountService.GetPageAsync(entity);
        }

        /// <summary>
        /// 分销人员提成比例设置
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("SetRate")]
        public async Task<int> SetRate([FromBody]AmountRate entity)
        {
            return await _amountService.SetRate(entity);
        }
        /// <summary>
        /// 获取提成比例
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetRate")]
        public async Task<object> GetRate()
        {
            return await _amountService.GetRate();
        }

    }
}