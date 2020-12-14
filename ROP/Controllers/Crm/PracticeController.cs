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
    /// 回访计划
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PracticeController : ControllerBase
    {
        private readonly IPracticeService _practiceService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="practiceService"></param>
        public PracticeController(IPracticeService practiceService)
        {
            _practiceService = practiceService;
        }

        /// <summary>
        /// 添加回访记录
        /// </summary>
        /// <param name="entity">回访记录信息</param>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<int> Add([FromBody]r_practice entity)
        {
            return await _practiceService.AddAsync(entity);
        }

        /// <summary>
        /// 修改回访记录
        /// </summary>
        /// <param name="entity">回访记录信息</param>
        /// <returns></returns>
        [HttpPut("Modify")]
        public async Task<int> Modify([FromBody]r_practice entity)
        {
            return await _practiceService.ModifyAsync(entity);
        }

        /// <summary>
        /// 删除回访记录
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        //[HttpDelete("Delete")]
        //public async Task<int> Delete([FromBody]List<int> list)
        //{
        //    return await _practiceService.DeleteAsync(list);
        //}

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="dateTimeStart">计划日期（开始）</param>
        /// <param name="dateTimeEnd">计划日期（结束）</param>
        /// <param name="storeID">门店ID</param>
        /// <param name="executorId">执行人ID</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        [HttpGet("GetPages")]
        public async Task<Page<r_practice>> GetPagesAsync([FromQuery] string name, string dateTimeStart, string dateTimeEnd, [FromQuery] int storeID, [FromQuery] int executorId, [FromQuery] string order, [FromQuery] int orderType, [FromQuery] int limit, [FromQuery] int page)
        {
            return await _practiceService.GetPagesAsync(name, dateTimeStart, dateTimeEnd, storeID, executorId, order, orderType, limit, page);
        }

        /// <summary>
        /// 执行回访计划
        /// </summary>
        /// <param name="entity">回访计划信息</param>
        /// <returns></returns>
        [HttpPut("Execute")]
        public async Task<int> Execute([FromBody]repayModel entity)
        {
            return await _practiceService.ExecuteAsync(entity.id,entity.answer,entity.content);
        }
    }
}