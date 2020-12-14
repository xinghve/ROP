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

namespace ROP.Controllers.His
{
    /// <summary>
    /// 挂号类别
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterTypeController : ControllerBase
    {
        private readonly IRegisterTypeService _registerTypeService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registerTypeService"></param>
        public RegisterTypeController(IRegisterTypeService registerTypeService)
        {
            _registerTypeService = registerTypeService;
        }

        /// <summary>
        /// 添加挂号类别
        /// </summary>
        /// <param name="entity">挂号类别信息</param>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<bool> Add([FromBody]RegisterType entity)
        {
            return await _registerTypeService.AddAsync(entity);
        }

        /// <summary>
        /// 修改挂号类别
        /// </summary>
        /// <param name="entity">挂号类别信息</param>
        /// <returns></returns>
        [HttpPut("Modify")]
        public async Task<bool> Modify([FromBody]RegisterType entity)
        {
            return await _registerTypeService.ModifyAsync(entity);
        }

        /// <summary>
        /// 删除挂号类别
        /// </summary>
        /// <param name="entity">挂号类别</param>
        /// <returns></returns>
        [HttpDelete("Delete")]
        public async Task<int> Delete([FromBody]his_registertype entity)
        {
            return await _registerTypeService.DeleteAsync(entity.typeid);
        }

        /// <summary>
        /// 数据列表
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="orderflag">预约标志</param>
        /// <param name="stateid">状态ID</param>
        /// <returns></returns>
        [HttpGet("GetList")]
        public async Task<List<RegisterType>> GetListAsync([FromQuery] string name, [FromQuery] int orderflag, [FromQuery] int stateid)
        {
            return await _registerTypeService.GetListAsync(name, orderflag, stateid);
        }

        /// <summary>
        /// 获得分页列表
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="orderflag">预约标志</param>
        /// <param name="stateid">状态ID</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        [HttpGet("GetPages")]
        public async Task<Page<RegisterType>> GetPagesAsync([FromQuery]string name, [FromQuery]int orderflag, [FromQuery]int stateid, [FromQuery]string order, [FromQuery]int orderType, [FromQuery] int limit, [FromQuery] int page)
        {
            return await _registerTypeService.GetPagesAsync(name, orderflag, stateid, order, orderType, limit, page);
        }
    }
}