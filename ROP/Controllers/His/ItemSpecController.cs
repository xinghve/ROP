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
    /// 项目规格--ty
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ItemSpecController : ControllerBase
    {
        private readonly IItemSpecService _itemSpecService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemSpecService"></param>
        public ItemSpecController(IItemSpecService itemSpecService)
        {
            _itemSpecService = itemSpecService;
        }

        /// <summary>
        /// 获取项目规格分页数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("GetPageAsync")]
        public async Task<Page<h_itemspec>> GetPageAsync([FromQuery]ItemSpecSearch model)
        {
            return await _itemSpecService.GetPageAsync(model);
        }

        /// <summary>
        /// 添加项目规格
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("AddAsync")]
        public async Task<int> AddAsync([FromBody]h_itemspec entity)
        {
            return await _itemSpecService.AddAsync(entity);
        }

        /// <summary>
        /// 编辑项目规格
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("ModifyAsync")]
        public async Task<int> ModifyAsync([FromBody]h_itemspec entity)
        {
            return await _itemSpecService.ModifyAsync(entity);
        }

        /// <summary>
        /// 删除规格
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpDelete("DeleteAsync")]
        public async Task<int> DeleteAsync([FromBody]h_itemspec entity)
        {
            return await _itemSpecService.DeleteAsync(entity.specid);
        }
        /// <summary>
        /// 数据列表
        /// </summary>
        /// <param name="item_id">项目ID</param>
        /// <returns></returns>
        [HttpGet("GetList")]
        public async Task<dynamic> GetListAsync([FromQuery] int item_id)
        {
            return await _itemSpecService.GetListAsync(item_id);
        }
    }
}