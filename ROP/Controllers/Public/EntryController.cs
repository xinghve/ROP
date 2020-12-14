using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Service.Repository.Interfaces.Public;
using Tools;

namespace ROP.Controllers.Public
{
    /// <summary>
    /// 词条
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class EntryController : ControllerBase
    {
        private readonly IEntryService _entryService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entryService"></param>
        public EntryController(IEntryService entryService)
        {
            _entryService = entryService;
        }

        /// <summary>
        /// 添加词条
        /// </summary>
        /// <param name="entity">词条信息</param>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<int> Add([FromBody]p_entry entity)
        {
            return await _entryService.AddAsync(entity);
        }

        /// <summary>
        /// 修改词条
        /// </summary>
        /// <param name="entity">词条信息</param>
        /// <returns></returns>
        [HttpPut("Modify")]
        public async Task<int> Modify([FromBody]p_entry entity)
        {
            return await _entryService.ModifyAsync(entity);
        }

        /// <summary>
        /// 删除词条
        /// </summary>
        /// <param name="list">词条</param>
        /// <returns></returns>
        [HttpDelete("Delete")]
        public async Task<int> Delete([FromBody]List<string> list)
        {
            return await _entryService.DeleteAsync(list);
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="name">词条内容</param>
        /// <returns></returns>
        [HttpGet("GetList")]
        public async Task<List<p_entry>> GetListAsync([FromQuery] string name)
        {
            return await _entryService.GetListAsync(name);
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="name">词条内容</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        [HttpGet("GetPages")]
        public async Task<Page<p_entry>> GetPagesAsync([FromQuery] string name, [FromQuery] string order, [FromQuery] int orderType, [FromQuery] int limit, [FromQuery] int page)
        {
            return await _entryService.GetPagesAsync(name, order, orderType, limit, page);
        }
    }
}