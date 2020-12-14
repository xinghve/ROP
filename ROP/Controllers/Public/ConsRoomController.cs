using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Models.View.Public;
using Service.Repository.Interfaces.Public;
using Tools;

namespace ROP.Controllers.Public
{
    /// <summary>
    /// 诊室
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ConsRoomController : ControllerBase
    {
        private readonly IConsRoomService _consRoomService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="consRoomService"></param>
        public ConsRoomController(IConsRoomService consRoomService)
        {
            _consRoomService = consRoomService;
        }

        /// <summary>
        /// 添加诊室
        /// </summary>
        /// <param name="entity">诊室信息</param>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<bool> Add([FromBody]ConsRoomDept entity)
        {
            return await _consRoomService.AddAsync(entity);
        }

        /// <summary>
        /// 修改诊室
        /// </summary>
        /// <param name="entity">诊室信息</param>
        /// <returns></returns>
        [HttpPut("Modify")]
        public async Task<bool> Modify([FromBody]ConsRoomDept entity)
        {
            return await _consRoomService.ModifyAsync(entity);
        }

        /// <summary>
        /// 删除诊室
        /// </summary>
        /// <param name="entity">诊室</param>
        /// <returns></returns>
        [HttpDelete("Delete")]
        public async Task<int> Delete([FromBody]p_cons_room entity)
        {
            return await _consRoomService.DeleteAsync(entity.id);
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="storeID">门店ID</param>
        /// <param name="deptID">部门ID</param>
        /// <returns></returns>
        [HttpGet("GetList")]
        public async Task<List<ConsRoomDept>> GetListAsync([FromQuery] string name, [FromQuery] int storeID = 0, [FromQuery] int deptID = 0)
        {
            return await _consRoomService.GetListAsync(name, storeID, deptID);
        }

        /// <summary>
        /// 获得分页列表
        /// </summary>
        /// <param name="storeId">门店ID</param>
        /// <param name="name">名称</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        [HttpGet("GetPages")]
        public async Task<Page<ConsRoomDept>> GetPagesAsync([FromQuery]int storeId, [FromQuery]string name, [FromQuery]string order, [FromQuery]int orderType, [FromQuery] int limit, [FromQuery] int page)
        {
            return await _consRoomService.GetPagesAsync(storeId, name, order, orderType, limit, page);
        }
    }
}