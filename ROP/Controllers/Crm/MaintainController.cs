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
    /// 客户关系维护
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MaintainController : ControllerBase
    {
        private readonly IMaintainService _maintainService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maintainService"></param>
        public MaintainController(IMaintainService maintainService)
        {
            _maintainService = maintainService;
        }

        /// <summary>
        /// 添加客户关系
        /// </summary>
        /// <param name="entity">客户关系信息</param>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<bool> Add([FromBody]c_maintain entity)
        {
            return await _maintainService.AddAsync(entity);
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
        public async Task<Page<c_maintain>> GetPagesAsync([FromQuery] string name, [FromQuery] int storeID, [FromQuery] string order, [FromQuery] int orderType, [FromQuery] int limit, [FromQuery] int page)
        {
            return await _maintainService.GetPagesAsync(name, storeID, order, orderType, limit, page);
        }
    }
}