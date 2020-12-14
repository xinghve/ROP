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
    /// 医疗室
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomService"></param>
        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        /// <summary>
        /// 添加医疗室
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<bool> AddAsync([FromBody]Room entity)
        {
            return await _roomService.AddAsync(entity);
        }

        /// <summary>
        /// 分页数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetPages")]
        public async Task<Page<Room>> GetPagesAsync([FromQuery]RoomPagesSearch entity)
        {
            return await _roomService.GetPagesAsync(entity);
        }

        /// <summary>
        /// 修改医疗室
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("Modify")]
        public async Task<bool> ModifyAsync([FromBody]Room entity)
        {
            return await _roomService.ModifyAsync(entity);
        }

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <returns></returns>
        [HttpPut("ModifyState")]
        public async Task<int> ModifyStateAsync([FromBody]p_room entity)
        {
            return await _roomService.ModifyStateAsync(entity);
        }

        /// <summary>
        /// 获取医疗室列表
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="state">状态(-1=所有；0=停用；1=启用；30=未使用；31=使用中)</param>
        /// <param name="equipment">是否存放设备</param>
        /// <returns></returns>
        [HttpGet("GetList")]
        public async Task<List<p_room>> GetListAsync([FromQuery]int store_id, short state, short equipment)
        {
            return await _roomService.GetListAsync(store_id, state, equipment);
        }

        /// <summary>
        /// 根据项目规格获取医疗室列表
        /// </summary>
        /// <param name="item_id">项目ID</param>
        /// <param name="spec_id">规格ID</param>
        /// <param name="dateTime">日期（默认当天）</param>
        /// <returns></returns>
        [HttpGet("GetListByItemSpec")]
        public async Task<dynamic> GetListByItemSpecAsync([FromQuery]int item_id, [FromQuery] int spec_id, [FromQuery] DateTime? dateTime)
        {
            return await _roomService.GetListByItemSpecAsync(item_id, spec_id, dateTime);
        }
    }
}