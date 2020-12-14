using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Service.Repository.Interfaces.Public;
using Tools.Authorize;

namespace ROP.Controllers.Cus
{
    /// <summary>
    /// 投诉
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeForArchives]
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
        /// 投诉
        /// </summary>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<int> AddAsync([FromBody]r_complaint entity)
        {
            return await _complaintServer.AddAsync(entity);
        }

        /// <summary>
        /// 数据列表
        /// </summary>
        /// <param name="state">状态</param>
        /// <returns></returns>
        [HttpGet("GetList")]
        public async Task<List<r_complaint>> GetListAsync([FromQuery]int state)
        {
            return await _complaintServer.GetListAsync(state);
        }
    }
}