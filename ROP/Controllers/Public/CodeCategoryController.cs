using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Service.Repository;
using SqlSugar;
using Tools;

namespace ROP.Controllers.Public
{
    /// <summary>
    /// 编码目录
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CodeCategoryController : ControllerBase
    {
        private readonly IBaseServer<b_code_category> _baseServer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseServer"></param>
        public CodeCategoryController(IBaseServer<b_code_category> baseServer)
        {
            _baseServer = baseServer;
        }

        /// <summary>
        /// 添加编码目录
        /// </summary>
        /// <param name="entity">编码目录信息</param>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<int> Add([FromBody]b_code_category entity)
        {
            entity.pinyin = ToSpell.GetFirstPinyin(entity.name);
            return await _baseServer.AddAsync(entity);
        }

        /// <summary>
        /// 修改编码目录
        /// </summary>
        /// <param name="entity">编码目录信息</param>
        /// <returns></returns>
        [HttpPut("Modify")]
        public async Task<int> Modify([FromBody]b_code_category entity)
        {
            entity.pinyin = ToSpell.GetFirstPinyin(entity.name);
            return await _baseServer.UpdateAsync(entity);
        }

        /// <summary>
        /// 删除编码目录
        /// </summary>
        /// <param name="list">编码目录ID</param>
        /// <returns></returns>
        [HttpDelete("Delete")]
        public async Task<int> Delete([FromBody]List<int> list)
        {
            return await _baseServer.DeleteAsync(d => list.Contains(d.id));
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        [HttpGet("GetPages")]
        public async Task<Page<b_code_category>> GetPagesAsync([FromQuery] string name, [FromQuery] string order, [FromQuery] int orderType, [FromQuery] int limit, [FromQuery] int page)
        {
            return await _baseServer.GetPagesAsync(new Service.DtoModel.PageParm { limit = limit, page = page }, order, orderType, true, !string.IsNullOrEmpty(name), w => w.name.Contains(name));
        }
    }
}