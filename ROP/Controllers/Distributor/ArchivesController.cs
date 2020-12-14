using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.View.Crm;
using Models.View.Mobile;
using Models.View.Public;
using Service.Repository.Interfaces.Crm;
using Service.Repository.Interfaces.Public;
using Tools;
using Tools.Authorize;

namespace ROP.Controllers.Distributor
{
    /// <summary>
    /// 客户
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeForDistributor]
    public class ArchivesController : ControllerBase
    {
        private readonly IArchivesService _archivesService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="archivesService"></param>
        public ArchivesController(IArchivesService archivesService)
        {
            _archivesService = archivesService;
        }

        /// <summary>
        /// 添加档案信息（分销人员端）
        /// </summary>
        /// <param name="entity">档案信息信息</param>
        /// <returns></returns>
        [HttpPost("DistributorAdd")]
        public async Task<bool> DistributorAddAsync([FromBody]MobileArchives entity)
        {
            return await _archivesService.DistributorAddAsync(entity);
        }

        /// <summary>
        /// 获取档案分页列表（分销人员端）
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetPagesByDistributor")]
        public async Task<dynamic> GetPagesByDistributorAsync([FromQuery] Search entity)
        {
            return await _archivesService.GetPagesByDistributorAsync(entity);
        }
    }
}