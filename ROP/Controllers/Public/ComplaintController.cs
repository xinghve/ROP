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
    /// 投诉
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ComplaintController : ControllerBase
    {
        private readonly IComplaintService _complaintServer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="complaintService"></param>
        public ComplaintController(IComplaintService complaintService)
        {
            _complaintServer = complaintService;
        }

        /// <summary>
        /// 处理
        /// </summary>
        /// <returns></returns>
        [HttpPut("Deal")]
        public async Task<int> DealAsync([FromBody]r_complaint entity)
        {
            return await _complaintServer.DealAsync(entity);
        }

        /// <summary>
        /// 获得分页列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetPages")]
        public async Task<Page<ComplaintPageModel>> GetPagesAsync([FromQuery]ComplaintSearch entity)
        {
            return await _complaintServer.GetPagesAsync(entity);
        }
    }
}