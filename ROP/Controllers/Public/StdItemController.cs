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
    /// 物资基础信息
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class StdItemController : ControllerBase
    {
        private readonly IStdItemService _stdItemService;
        /// <summary>
        /// 物资基础信息构造
        /// </summary>
        /// <param name="stdItemService"></param>
        public StdItemController(IStdItemService stdItemService)
        {
            _stdItemService = stdItemService;
        }

        /// <summary>
        /// 添加基础物资
        /// </summary>
        /// <param name="entity">基础物资实体</param>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<bool> AddAsync([FromBody] StdItem entity)
        {
            return await _stdItemService.AddAsync(entity);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <returns></returns>
        [HttpDelete("Delete")]
        public async Task<int> DeleteAsync([FromBody] List<int> vs)
        {
            return await _stdItemService.DeleteAsync(vs);
        }

        /// <summary>
        /// 根据基础物资ID获取基础物资信息
        /// </summary>
        /// <param name="id">基础物资ID</param>
        /// <returns></returns>
        [HttpGet("Get")]
        public async Task<StdItem> GetAsync([FromQuery] int id)
        {
            return await _stdItemService.GetAsync(id);
        }

        /// <summary>
        /// 获取基础物资列表
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="type_id">类别ID（-1=所有）</param>
        /// <returns></returns>
        [HttpGet("GetList")]
        public async Task<List<StdItem>> GetListAsync([FromQuery] string name, [FromQuery] short type_id)
        {
            return await _stdItemService.GetListAsync(name, type_id);
        }

        /// <summary>
        /// 获取物资明细列表
        /// </summary>
        /// <param name="id">基础物资ID</param>
        /// <returns></returns>
        [HttpGet("GetDetialList")]
        public async Task<List<p_std_item_detials>> GetDetialListAsync(int id)
        {
            return await _stdItemService.GetDetialListAsync(id);
        }

        /// <summary>
        /// 获取物资明细（规格对应明细）
        /// </summary>
        /// <param name="id">物资基础ID</param>
        /// <returns></returns>
        [HttpGet("GetSpecDetials")]
        public async Task<List<StdItemDetials>> GetSpecDetialsAsync(int id)
        {
            return await _stdItemService.GetSpecDetialsAsync(id);
        }

        /// <summary>
        /// 获得分页列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetPage")]
        public async Task<Page<StdItem>> GetPageAsync([FromQuery] StdItemPageSearch entity)
        {
            return await _stdItemService.GetPageAsync(entity);
        }

        /// <summary>
        /// 编辑基础物资
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("Modify")]
        public async Task<bool> ModifyAsync([FromBody] StdItem entity)
        {
            return await _stdItemService.ModifyAsync(entity);
        }

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <returns></returns>
        [HttpPut("ModifyStatus")]
        public async Task<int> ModifyStatusAsync([FromBody] p_std_item entity)
        {
            return await _stdItemService.ModifyStatusAsync(entity);
        }

        /// <summary>
        /// 根据分类获取物资
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetStdByClass")]
        public async Task<List<p_std_item>> GetStdByClass([FromQuery]string name, [FromQuery]int id)
        {
            return await _stdItemService.GetStdByClass(name, id);
        }

        /// <summary>
        ///  获取规格明细加可用数量
        /// </summary>
        /// <param name="id"></param>
        /// <param name="store_id"></param>
        /// <returns></returns>
        [HttpGet("GetSpecDetialsNumAsync")]
        public async Task<List<StdItemDetials>> GetSpecDetialsNumAsync([FromQuery]int id, [FromQuery]int store_id)
        {
            return await _stdItemService.GetSpecDetialsNumAsync(id, store_id);
        }
    }
}