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
    /// 套餐--ty
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PackageController : ControllerBase
    {
        private readonly IPackageService _packageService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="packageService"></param>
        public PackageController(IPackageService packageService)
        {
            _packageService = packageService;
        }


        /// <summary>
        /// 套餐分页
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetPageAsync")]
       public async Task<Page<PackageModel>> GetPageAsync([FromQuery]PackSearch entity)
        {
            return await _packageService.GetPageAsync(entity);
        }  
        
        /// <summary>
        /// 套餐项目分页
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetItemPageAsync")]
       public async Task<Page<his_applycontent>> GetItemPageAsync([FromQuery]PackItemSearch entity)
        {
            return await _packageService.GetItemPageAsync(entity);
        }
        
        /// <summary>
        /// 获取套餐列表
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        [HttpGet("GetList")]
        public async Task<List<PackageModel>> GetList([FromQuery]int typeId, [FromQuery]int storeId)
        {
            return await _packageService.GetList(typeId,storeId);
        }

        /// <summary>
        /// 添加套餐
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("AddAsync")]
        public async Task<int> AddAsync([FromBody]PackageModel entity)
        {
            return await _packageService.AddAsync(entity);
        }

        /// <summary>
        /// 编辑套餐名
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("UpdateAsync")]
        public async Task<int> UpdateAsync([FromBody]PackageUpdateModel entity)
        {
            return await _packageService.UpdateAsync(entity);
        }

        /// <summary>
        /// 编辑套餐
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("ModifyAsync")]
        public async Task<bool> ModifyAsync([FromBody]PackageEntity entity)
        {
            return await _packageService.ModifyAsync(entity);
        }


        /// <summary>
        /// 删除套餐
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpDelete("DeleteAsync")]
       public async Task<int> DeleteAsync([FromBody]PackageEntity entity)
        {
            return await _packageService.DeleteAsync(entity.packid,entity.store_id.Value,entity.type_id.Value );
        }
    }
}