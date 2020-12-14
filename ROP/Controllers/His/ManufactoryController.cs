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
    /// 供应商--ty
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ManufactoryController : ControllerBase
    {
        private readonly IManufactoryService _manufactoryService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="manufactoryService"></param>
        public ManufactoryController(IManufactoryService manufactoryService)
        {
            _manufactoryService = manufactoryService;
        }

        /// <summary>
        /// 查询供应商分页数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetPageAsync")]
        public async Task<Page<ManufactoryModel>> GetPageAsync([FromQuery]ManufactorySearch entity)
        {
            return await _manufactoryService.GetPageAsync(entity);
        }
        /// <summary>
        /// 获取供货商List
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("GetManufactoryList")]
        public async Task<List<ManufactoryList>> GetManufactoryList([FromQuery]string name, [FromQuery]int type_id = -1)
        {
            return await _manufactoryService.GetManufactoryList(name, type_id);
        }

        /// <summary>
        /// 添加供应商
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("AddAsync")]
        public async Task<bool> AddAsync([FromBody]ManufactoryModel entity)
        {
            return await _manufactoryService.AddAsync(entity);
        }

        /// <summary>
        /// 编辑供应商
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("ModifyAsync")]
        public async Task<bool> ModifyAsync([FromBody]ManufactoryModel entity)
        {
            return await _manufactoryService.ModifyAsync(entity);

        }

        /// <summary>
        /// 删除供应商
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpDelete("DeleteAsync")]
        public async Task<bool> DeleteAsync([FromBody]h_manufactor entity)
        {
            return await _manufactoryService.DeleteAsync(entity);
        }

        /// <summary>
        /// 设置供应商分类
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPost("SetManufactorClass")]
        public async Task<int> SetManufactorClass([FromBody]ManufactorClassModel list)
        {
            return await _manufactoryService.SetManufactorClass(list);
        }

        /// <summary>
        /// 修改供应商状态
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("SetState")]
        public async Task<int> SetState([FromBody]ManufactorSetState entity)
        {
            return await _manufactoryService.SetState(entity);
        }
    }
}