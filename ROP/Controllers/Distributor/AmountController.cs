using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.View.Public;
using Service.Repository.Interfaces.Public;
using Tools.Authorize;

namespace ROP.Controllers.Distributor
{
    /// <summary>
    /// 提成记录
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeForDistributor]
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
        /// 提成分页数据(分销人员端)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetPageByDistributor")]
        public async Task<dynamic> GetPageByDistributorAsync([FromQuery]Search entity)
        {
            return await _amountService.GetPageByDistributorAsync(entity);
        }
    
    }
}