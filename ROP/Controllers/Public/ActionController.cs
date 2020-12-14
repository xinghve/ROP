using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Service.Repository.Interfaces.Public;
using Tools;

namespace ROP.Controllers.Public
{
    /// <summary>
    /// 菜单功能
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class ActionController : ControllerBase
    {
        private readonly IActionService _actionService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionService"></param>
        public ActionController(IActionService actionService)
        {
            _actionService = actionService;
        }

        /// <summary>
        /// 添加菜单功能
        /// </summary>
        /// <param name="entity">菜单功能信息</param>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<int> Add([FromBody]p_action entity)
        {
            return await _actionService.AddAsync(entity);
        }

        /// <summary>
        /// 修改菜单功能
        /// </summary>
        /// <param name="entity">菜单功能信息</param>
        /// <returns></returns>
        [HttpPut("Modify")]
        public async Task<int> Modify([FromBody]p_action entity)
        {
            return await _actionService.ModifyAsync(entity);
        }

        /// <summary>
        /// 删除菜单功能
        /// </summary>
        /// <param name="list">菜单功能ID</param>
        /// <returns></returns>
        [HttpDelete("Delete")]
        public async Task<int> Delete([FromBody]List<int> list)
        {
            return await _actionService.DeleteAsync(list);
        }

        /// <summary>
        /// 获取菜单树
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetMenuTree")]
        public async Task<object> GetMenuTreeAsync([FromQuery] int storeId = 0)
        {
            return await _actionService.GetMenuTree(storeId);
        }

        /// <summary>
        /// 获取功能表
        /// </summary>
        /// <param name="storeId">门店ID</param>
        /// <param name="setType">类型（-1=所有，0=平台，1=集团，2=门店）</param>
        /// <returns></returns>
        [HttpGet("GetActionTree")]
        public async Task<object> GetActionTreeAsync([FromQuery] int setType = -1, [FromQuery]int storeId = 0)
        {
            return await _actionService.GetActionTree(storeId, setType);
        }

        /// <summary>
        /// 分页方式获取功能表
        /// </summary>
        /// <param name="parentId">上级Id</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        [HttpGet("GetPages")]
        public async Task<Page<p_action>> GetPagesAsync([FromQuery] int parentId, [FromQuery] string order, [FromQuery] int orderType, [FromQuery] int limit, [FromQuery] int page)
        {
            return await _actionService.GetPagesAsync(parentId, order, orderType, limit, page);
        }
    }
}