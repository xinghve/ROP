using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Models.View.His;
using Service.Repository.Interfaces.His;

namespace ROP.Controllers.His
{
    /// <summary>
    /// 项目--ty
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ItemDirController : ControllerBase
    {
        private readonly IItemDirService _itemService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemService"></param>
        public ItemDirController(IItemDirService itemService)
        {
            _itemService = itemService;
        }
        /// <summary>
        /// 添加项目目录
        /// </summary>
        /// <param name="entity">项目目录信息</param>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<int> Add([FromBody]h_itemdir entity)
        {
            return await _itemService.AddAsync(entity);
        }

        /// <summary>
        /// 编辑项目目录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("Modify")]
        public async Task<int> Modify([FromBody]h_itemdir entity)
        {
            return await _itemService.ModifyAsync(entity);
        }

        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpDelete("Delete")]
        public async Task<bool> Delete([FromBody]h_itemdir entity)
        {
            return await  _itemService.DeleteAsync(entity.dirid);

        }

        /// <summary>
        /// 获取目录树
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetItemTree")]
        public async Task<object> GetItemTree()
        {
            return await _itemService.GetItemTree();
        }

        /// <summary>
        /// 获取项目类别
        /// </summary>
        /// <param name="typeId">没有传-1</param>
        /// <param name="parentId">目录id</param>
        /// <returns></returns>
        [HttpGet("GetItemType")]
        public async Task<List<b_basecode>> GetItemType( [FromQuery]int typeId,[FromQuery]int parentId)
        {
            return await _itemService.GetItemType(typeId, parentId);
        }

        /// <summary>
        /// 导入目录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("ImportDir")]
        public async Task<int> ImportDir([FromBody]List<h_itemdir> entity)
        {
            return await _itemService.ImportDir(entity);
        }

        /// <summary>
        /// 导入所有项目
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("ImportAllItem")]
        public async Task<bool> ImportAllItem([FromBody]List<ItemAllModel> entity)
        {
            return await _itemService.ImportAllItem(entity);
        }

    }
}