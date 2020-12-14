using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Service.Repository.Interfaces.Crm;
using Tools;

namespace ROP.Controllers.Crm
{
    /// <summary>
    /// 消费习惯
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class HabitController : ControllerBase
    {
        private readonly IHabitService _habitService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="habitService"></param>
        public HabitController(IHabitService habitService)
        {
            _habitService = habitService;
        }

        /// <summary>
        /// 添加消费习惯
        /// </summary>
        /// <param name="entity">消费习惯信息</param>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<int> Add([FromBody]c_habit entity)
        {
            return await _habitService.AddAsync(entity);
        }

        /// <summary>
        /// 修改消费习惯
        /// </summary>
        /// <param name="entity">消费习惯信息</param>
        /// <returns></returns>
        [HttpPut("Modify")]
        public async Task<int> Modify([FromBody]c_habit entity)
        {
            return await _habitService.ModifyAsync(entity);
        }

        /// <summary>
        /// 删除消费习惯
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpDelete("Delete")]
        public async Task<int> Delete([FromBody]List<int> list)
        {
            return await _habitService.DeleteAsync(list);
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="storeID">门店ID</param>
        /// <returns></returns>
        [HttpGet("GetList")]
        public async Task<List<c_habit>> GetListAsync([FromQuery] string name, [FromQuery] int storeID = 0)
        {
            return await _habitService.GetListAsync(name, storeID);
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="storeID">门店ID</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        [HttpGet("GetPages")]
        public async Task<Page<c_habit>> GetPagesAsync([FromQuery] string name, [FromQuery] int storeID, [FromQuery] string order, [FromQuery] int orderType, [FromQuery] int limit, [FromQuery] int page)
        {
            return await _habitService.GetPagesAsync(name, storeID, order, orderType, limit, page);
        }
    }
}