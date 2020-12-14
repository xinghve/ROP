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

namespace ROP.Controllers.Cus
{
    /// <summary>
    /// 就诊评价
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeForArchives]
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
        /// 评价（客户端）
        /// </summary>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<int> EvaluateAsync([FromBody]r_evaluate entity)
        {
            return await _evaluateService.EvaluateAsync(entity);
        }

        /// <summary>
        /// 获得列表（客户端）
        /// </summary>
        /// <param name="state">状态（-1=所有，1=已评价，0=待评价）</param>
        /// <returns></returns>
        [HttpGet("GetList")]
        public async Task<dynamic> GetListAsync([FromQuery]short state)
        {
            return await _evaluateService.GetListAsync(state);
        }

    }
}