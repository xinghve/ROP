using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Service.Repository.Interfaces.Public;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tools;

namespace ROP.Controllers.Public
{
    /// <summary>
    /// 区域
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AreaController : ControllerBase
    {
        private readonly IAreaService _areaService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="areaService"></param>
        public AreaController(IAreaService areaService)
        {
            _areaService = areaService;
        }

        /// <summary>
        /// 添加区域
        /// </summary>
        /// <param name="entity">区域信息</param>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<int> Add([FromBody]b_area entity)
        {
            return await _areaService.AddAsync(entity);
        }

        /// <summary>
        /// 修改区域
        /// </summary>
        /// <param name="entity">区域信息</param>
        /// <returns></returns>
        [HttpPut("Modify")]
        public async Task<int> Modify([FromBody]b_area entity)
        {
            return await _areaService.ModifyAsync(entity);
        }

        /// <summary>
        /// 删除区域
        /// </summary>
        /// <param name="list">区域ID</param>
        /// <returns></returns>
        [HttpDelete("Delete")]
        public async Task<int> Delete([FromBody]List<int> list)
        {
            return await _areaService.DeleteAsync(list);
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="parentNum">上级编号</param>
        /// <returns></returns>
        [HttpGet("GetList")]
        public async Task<List<b_area>> GetListAsync([FromQuery] string name, [FromQuery] int parentNum = 0)
        {
            return await _areaService.GetListAsync(name, parentNum);
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
        public async Task<Page<b_area>> GetPagesAsync([FromQuery] string name, [FromQuery] string order, [FromQuery] int orderType, [FromQuery] int limit, [FromQuery] int page)
        {
            return await _areaService.GetPagesAsync(name, order, orderType, limit, page);
        }
    }
}
