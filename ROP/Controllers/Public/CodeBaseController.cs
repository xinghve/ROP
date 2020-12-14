using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Service.Repository;
using Tools;

namespace ROP.Controllers.Public
{
    /// <summary>
    /// 基础编码
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CodeBaseController : ControllerBase
    {
        private readonly IBaseServer<b_codebase> _baseServer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseServer"></param>
        public CodeBaseController(IBaseServer<b_codebase> baseServer)
        {
            _baseServer = baseServer;
        }

        /// <summary>
        /// 添加基础编码
        /// </summary>
        /// <param name="entity">基础编码信息</param>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<int> Add([FromBody]b_codebase entity)
        {
            entity.pinyin = ToSpell.GetFirstPinyin(entity.text);
            return await _baseServer.AddAsync(entity);
        }

        /// <summary>
        /// 修改基础编码
        /// </summary>
        /// <param name="entity">基础编码信息</param>
        /// <returns></returns>
        [HttpPut("Modify")]
        public async Task<int> Modify([FromBody]b_codebase entity)
        {
            entity.pinyin = ToSpell.GetFirstPinyin(entity.text);
            return await _baseServer.UpdateAsync(entity);
        }

        /// <summary>
        /// 删除基础编码
        /// </summary>
        /// <param name="list">基础编码ID</param>
        /// <returns></returns>
        [HttpDelete("Delete")]
        public async Task<int> Delete([FromBody]List<int> list)
        {
            return await _baseServer.DeleteAsync(d => list.Contains(d.id));
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="categoryId">目录ID</param>
        /// <returns></returns>
        [HttpGet("GetList")]
        public async Task<List<b_codebase>> GetListAsync([FromQuery] int categoryId = 0)
        {
            return await _baseServer.GetListAsync(w => w.category_id == categoryId, o => o.id, 0);
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="text">名称</param>
        /// <param name="categoryId">目录ID</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        [HttpGet("GetPages")]
        public async Task<Page<b_codebase>> GetPagesAsync([FromQuery] string text, [FromQuery] int categoryId, [FromQuery] string order, [FromQuery] int orderType, [FromQuery] int limit, [FromQuery] int page)
        {
            return await _baseServer.GetPagesAsync(new Service.DtoModel.PageParm { limit = limit, page = page }, order, orderType, true, !string.IsNullOrEmpty(text), w => w.text.Contains(text), true, w => w.category_id == categoryId);
        }
    }
}