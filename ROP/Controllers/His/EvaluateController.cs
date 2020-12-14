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
using Tools.Authorize;

namespace ROP.Controllers.His
{
    /// <summary>
    /// 就诊评价
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class EvaluateController : ControllerBase
    {
        private readonly IEvaluateService _evaluateService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="evaluateService"></param>
        public EvaluateController(IEvaluateService evaluateService)
        {
            _evaluateService = evaluateService;
        }

        /// <summary>
        /// 获得评价详情
        /// </summary>
        /// <param name="id">评价ID</param>
        /// <returns></returns>
        [HttpGet("GetById")]
        public async Task<r_evaluate> GetByIdAsync([FromQuery]string id)
        {
            return await _evaluateService.GetByIdAsync(id);
        }

        /// <summary>
        /// 评价分页数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetPageAsync")]
        public async Task<Page<EvaluatePageModel>> GetPageAsync([FromQuery]EvaluateSearch entity)
        {
            return await _evaluateService.GetPageAsync(entity);
        }
    }
}