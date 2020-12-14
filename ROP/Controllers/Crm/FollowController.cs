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
    /// 随访计划
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class FollowController : ControllerBase
    {
        private readonly IFollowService _followService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="followService"></param>
        public FollowController(IFollowService followService)
        {
            _followService = followService;
        }

        /// <summary>
        /// 添加随访计划
        /// </summary>
        /// <param name="entity">随访计划信息</param>
        /// <returns></returns>
        //[HttpPost("Add")]
        //public async Task<int> Add([FromBody]r_follow_up entity)
        //{
        //    return await _followService.AddAsync(entity);
        //}

        /// <summary>
        /// 修改随访计划
        /// </summary>
        /// <param name="entity">随访计划信息</param>
        /// <returns></returns>
        [HttpPut("Modify")]
        public async Task<int> Modify([FromBody]r_follow_up entity)
        {
            return await _followService.ModifyAsync(entity);
        }

        /// <summary>
        /// 委托随访计划
        /// </summary>
        /// <param name="entity">随访计划信息</param>
        /// <returns></returns>
        [HttpPut("Client")]
        public async Task<int> Client([FromBody]r_follow_up entity)
        {
            return await _followService.ClientAsync(entity);
        }

        /// <summary>
        /// 执行随访计划
        /// </summary>
        /// <param name="entity">随访计划信息</param>
        /// <returns></returns>
        [HttpPut("Execute")]
        public async Task<int> Execute([FromBody]FollowExecute entity)
        {
            return await _followService.ExecuteAsync(entity);
        }

        /// <summary>
        /// 删除随访计划
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        //[HttpDelete("Delete")]
        //public async Task<int> Delete([FromBody]List<int> list)
        //{
        //    return await _followService.DeleteAsync(list);
        //}

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetPages")]
        public async Task<Page<FollowUpModel>> GetPagesAsync([FromQuery] FollowUpSearch entity)
        {
            return await _followService.GetPagesAsync(entity);
        }
    }
}