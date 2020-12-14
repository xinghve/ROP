using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Models.View.Crm;
using Service.Repository.Interfaces.Crm;
using Tools;

namespace ROP.Controllers.Crm
{
    /// <summary>
    /// 投诉反馈--ty
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class FeedBackController : ControllerBase
    {
        private readonly IFeedBackService _fdService;
        /// <summary>
        /// 投诉反馈构造
        /// </summary>
        public FeedBackController(IFeedBackService fdService)
        {
            _fdService = fdService;
        }

        /// <summary>
        /// 查询投诉反馈分页信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("GetPageAsync")]
        public async Task<Page<r_feedback>> GetPageAsync([FromQuery]FeedBackModel model)
        {
            return await _fdService.GetPageAsync(model);
        }

        /// <summary>
        /// 添加投诉反馈信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<int> Add([FromBody]r_feedback entity )
        {
            return await _fdService.Add(entity);
        }

        /// <summary>
        /// 编辑投诉反馈信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("Modify")]
        public async Task<int> Modify([FromBody]r_feedback entity)
        {
            return await _fdService.Modify(entity);
        }

        /// <summary>
        /// 批量删除投诉反馈信息
        /// </summary>
        /// <param name="delList"></param>
        /// <returns></returns>
        [HttpDelete("Delete")]
        public async Task<int> Delete([FromBody]List<int> delList)
        {
            return await _fdService.Delete(delList);
        }
    }
}