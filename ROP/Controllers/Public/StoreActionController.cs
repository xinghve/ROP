using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.View.Public;
using Service.Repository.Interfaces.Public;

namespace ROP.Controllers.Public
{
    /// <summary>
    /// 门店权限
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class StoreActionController : ControllerBase
    {
        private readonly IStoreActionService _storeActionService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storeActionService"></param>
        public StoreActionController(IStoreActionService storeActionService)
        {
            _storeActionService = storeActionService;
        }

        /// <summary>
        /// 获取门店权限
        /// </summary>
        /// <param name="store_id">门店Id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> Get([FromQuery]int store_id)
        {
            return await _storeActionService.GetAsync(store_id);
        }

        /// <summary>
        /// 设置门店权限
        /// </summary>
        /// <param name="item">角色权限</param>
        /// <returns>成功/失败</returns>
        [HttpPost]
        public bool Post([FromBody] StoreAction item)
        {
            return _storeActionService.Set(item);
        }
    }
}