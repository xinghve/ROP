using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Models.View.Business;
using Service.Repository.Interfaces.Business;
using Tools;

namespace ROP.Controllers.Business
{
    /// <summary>
    /// 物资分类
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class GoodsTypeController : ControllerBase
    {
        private readonly IGoodsTypeService _goodsType;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="goodsType"></param>
        public GoodsTypeController(IGoodsTypeService goodsType)
        {
            _goodsType = goodsType;
        }

        /// <summary>
        /// 添加物资分类
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<int> AddAsync([FromBody]b_codebase entity)
        {
            return await _goodsType.AddAsync(entity);
        }

        /// <summary>
        /// 删除物资分类
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpDelete("Delete")]
        public async Task<int> DeleteAsync([FromBody]b_codebase entity)
        {
            return await _goodsType.DeleteAsync(entity);
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetList")]
        public async Task<List<b_codebase>> GetListAsync([FromQuery]int catalog_id)
        {
            return await _goodsType.GetListAsync(catalog_id);
        }

        /// <summary>
        /// 物资分类分页查询
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetPages")]
        public async Task<Page<b_codebase>> GetPagesAsync([FromQuery]GoodsTypePageSearch entity)
        {
            return await _goodsType.GetPagesAsync(entity);
        }

        /// <summary>
        /// 编辑物资分类
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("Modify")]
        public async Task<int> ModifyAsync([FromBody]b_codebase entity)
        {
            return await _goodsType.ModifyAsync(entity);
        }

        /// <summary>
        /// 添加物资目录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("AddCatalog")]
        public async Task<int> AddCatalogAsync([FromBody]b_catalog entity)
        {
            return await _goodsType.AddCatalogAsync(entity);
        }

        /// <summary>
        /// 删除物资目录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpDelete("DeleteCatalog")]
        public async Task<int> DeleteCatalogAsync([FromBody]b_catalog entity)
        {
            return await _goodsType.DeleteCatalogAsync(entity);
        }

        /// <summary>
        /// 获取目录列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetCatalogList")]
        public async Task<dynamic> GetCatalogListAsync()
        {
            return await _goodsType.GetCatalogListAsync();
        }

        /// <summary>
        /// 物资目录分页查询
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetCatalogPages")]
        public async Task<Page<b_catalog>> GetCatalogPagesAsync([FromQuery]CatalogPageSearch entity)
        {
            return await _goodsType.GetCatalogPagesAsync(entity);
        }

        /// <summary>
        /// 编辑物资目录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("ModifyCatalog")]
        public async Task<int> ModifyCatalogAsync([FromBody]b_catalog entity)
        {
            return await _goodsType.ModifyCatalogAsync(entity);
        }
    }
}