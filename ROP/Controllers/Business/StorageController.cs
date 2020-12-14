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
    /// 库存
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class StorageController : ControllerBase
    {
        private readonly IStorageService _storageService;
        /// <summary>
        /// 库存构造
        /// </summary>
        /// <param name="storageService"></param>
        public StorageController(IStorageService storageService)
        {
            _storageService = storageService;
        }

        /// <summary>
        /// 获得分页列表（明细）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetDetialsPage")]
        public async Task<Page<bus_storage_detials>> GetDetialsPageAsync([FromQuery]StorageDetialsPageSearch entity)
        {
            return await _storageService.GetDetialsPageAsync(entity);
        }

        /// <summary>
        /// 获得分页列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetPage")]
        public async Task<Page<bus_storage>> GetPageAsync([FromQuery]StoragePageSearch entity)
        {
            return await _storageService.GetPageAsync(entity);
        }

        /// <summary>
        /// 获取集团所有库存明细（根据项目、规格、厂家）
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPut("DetialsList")]
        public async Task<List<StorageDetials>> GetDetialsListAsync([FromBody]List<StorageDetials> list)
        {
            return await _storageService.GetDetialsListAsync(list);
        }

        /// <summary>
        /// 获取库存明细（根据项目、规格、厂家）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetDetials")]
        public async Task<StorageDetials> GetDetialsAsync([FromQuery]StorageDetials entity)
        {
            return await _storageService.GetDetialsAsync(entity);
        }
    }
}