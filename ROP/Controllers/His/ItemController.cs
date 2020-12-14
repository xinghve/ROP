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
    /// 项目--ty
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IItemService _itemService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemService"></param>
        public ItemController(IItemService itemService)
        {
            _itemService = itemService;
        }

        /// <summary>
        /// 获取项目分页数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetItemAsync")]
        public async Task<Page<h_item>> GetItemAsync([FromQuery]ItemSearch entity)
        {
            return await _itemService.GetItemAsync(entity);
        }

        /// <summary>
        /// 获取费别下拉数据
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>
        [HttpGet("GetFeeList")]
        public async Task<object> GetFeeList([FromQuery]int typeId)
        {
            return await _itemService.GetFeeList(typeId);
        }

        /// <summary>
        /// 添加项目
        /// </summary>
        /// <param name="entity">项目信息</param>
        /// <returns></returns>
        [HttpPost("AddItem")]
        public async Task<int> AddItem([FromBody]ItemModel entity)
        {
            return await _itemService.AddItemAsync(entity);
        }

        /// <summary>
        /// 编辑项目
        /// </summary>
        /// <param name="entity">项目信息</param>
        /// <returns></returns>
        [HttpPut("ModifyItem")]
        public async Task<bool> ModifyItem([FromBody]ItemModel entity)
        {
            return await _itemService.ModifyItemAsync(entity);
        }

        /// <summary>
        /// 获取项目规格List
        /// </summary>
        /// <param name="feeType">费别id</param>
        /// <param name="name">项目名称</param>
        /// <returns></returns>
        [HttpGet("GetItemSpecAsync")]
        public async Task<List<ItemSpecModel>> GetItemSpecAsync([FromQuery]int feeType, [FromQuery]string name = "")
        {
            return await _itemService.GetItemSpecAsync(feeType, name);
        }

        /// <summary>
        /// 删除项目
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpDelete("DeleteAsync")]
        public async Task<bool> DeleteAsync([FromBody]h_item entity)
        {
            return await _itemService.DeleteAsync(entity.item_id);
        }

        /// <summary>
        /// 获取开方项目分页
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetItemPageAsync")]
        public async Task<Page<h_item>> GetItemPageAsync([FromQuery]ItemPage entity)
        {
            return await _itemService.GetItemPageAsync(entity);
        }

        /// <summary>
        /// 获取设备项目
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetEquipmentList")]
        public async Task<Page<EquipmentItem>> GetEquipmentList([FromQuery]ItemPage entity)
        {
            return await _itemService.GetEquipmentList(entity);
        }
    }
}