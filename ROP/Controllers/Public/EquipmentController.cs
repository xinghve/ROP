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
    /// 器械设备
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class EquipmentController : ControllerBase
    {
        private readonly IEquipmentService _equipmentService;

        /// <summary>
        /// 器械设备
        /// </summary>
        /// <param name="equipmentService"></param>
        public EquipmentController(IEquipmentService equipmentService)
        {
            _equipmentService = equipmentService;
        }

        /// <summary>
        /// 添加器械设备
        /// </summary>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<bool> AddAsync([FromBody]Equipment entity)
        {
            return await _equipmentService.AddAsync(entity);
        }

        /// <summary>
        /// 获得分页列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetPages")]
        public async Task<Page<EquipmentPages>> GetPagesAsync([FromQuery]EquipmentPagesSearch entity)
        {
            return await _equipmentService.GetPagesAsync(entity);
        }

        /// <summary>
        /// 修改器械设备
        /// </summary>
        /// <returns></returns>
        [HttpPut("Modify")]
        public async Task<bool> ModifyAsync([FromBody]Equipment entity)
        {
            return await _equipmentService.ModifyAsync(entity);
        }

        /// <summary>
        /// 添加器械设备详细
        /// </summary>
        /// <returns></returns>
        [HttpPost("AddDetials")]
        public async Task<int> AddDetialsAsync([FromBody]p_equipment_detials entity)
        {
            return await _equipmentService.AddDetialsAsync(entity);
        }

        /// <summary>
        /// 修改器械设备详细
        /// </summary>
        /// <returns></returns>
        [HttpPut("ModifyDetials")]
        public async Task<int> ModifyDetialsAsync([FromBody]p_equipment_detials entity)
        {
            return await _equipmentService.ModifyDetialsAsync(entity);
        }

        /// <summary>
        /// 修改器械设备详细状态
        /// </summary>
        /// <returns></returns>
        [HttpPut("ModifyDetialsState")]
        public async Task<int> ModifyDetialsStateAsync([FromBody]p_equipment_detials entity)
        {
            return await _equipmentService.ModifyDetialsStateAsync(entity);
        }

        /// <summary>
        /// 获得器械设备详细分页列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetDetialsPages")]
        public async Task<Page<p_equipment_detials>> GetDetialsPagesAsync([FromQuery]EquipmentDetialsPagesSearch entity)
        {
            return await _equipmentService.GetDetialsPagesAsync(entity);
        }

        /// <summary>
        /// 根据医疗室ID获取设备列表
        /// </summary>
        /// <param name="room_id">医疗室ID</param>
        /// <returns></returns>
        [HttpGet("GetList")]
        public async Task<dynamic> GetListAsync([FromQuery]int room_id)
        {
            return await _equipmentService.GetListAsync(room_id);
        }

        /// <summary>
        /// 根据项目规格获取设备列表
        /// </summary>
        /// <param name="item_id">项目ID</param>
        /// <param name="spec_id">规格ID</param>
        /// <param name="dateTime">日期（默认当天）</param>
        /// <returns></returns>
        [HttpGet("GetListByItemSpec")]
        public async Task<dynamic> GetListByItemSpecAsync([FromQuery]int item_id, [FromQuery] int spec_id, [FromQuery] DateTime? dateTime)
        {
            return await _equipmentService.GetListByItemSpecAsync(item_id, spec_id, dateTime);
        }
    }
}