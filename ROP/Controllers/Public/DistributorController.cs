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
    /// 分销PC端
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DistributorController : ControllerBase
    {
        private readonly IDistributorService _distributorService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="distributorService"></param>
        public DistributorController(IDistributorService distributorService)
        {
            _distributorService = distributorService;
        }

        /// <summary>
        /// 获取分销分页查询
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetPageAsync")]
        public async Task<Page<DistributorPageModel>> GetPageAsync([FromQuery]DistributorSearch entity)
        {
            return await _distributorService.GetPagesAsync(entity);
        }
        /// <summary>
        /// 启用禁用分销人员
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("ModifyStateAsync")]
        public async Task<int> ModifyStateAsync([FromBody]DistributorModify entity)
        {
            return await _distributorService.ModifyStateAsync(entity);
        }

    }
}