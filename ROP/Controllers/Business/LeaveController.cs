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
    /// 请假流程
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveController : ControllerBase
    {
        private readonly ILeaveService _leaveService;

        /// <summary>
        /// 请假构造
        /// </summary>
        /// <param name="leaveService"></param>
        public LeaveController(ILeaveService leaveService)
        {
            _leaveService = leaveService;
        }

        /// <summary>
        /// 获取请假流程
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetVerifyProcess")]
        public async Task<List<VerifyProcess>> GetVerifyProcess([FromQuery]VerifyProcessSearch entity)
        {
            entity.whichuser = 1;
            return await _leaveService.GetVerifyProcess(entity);
        }

        /// <summary>
        /// 请假申请
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("AddLeavel")]
        public async Task<bool> AddLeavel([FromBody]AddLeavelModel entity)
        {
            dynamic type = (new Program()).GetType();
            return await _leaveService.AddLeavel(entity,type);
        }

      
        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="leave_no"></param>
        /// <returns></returns>
        [HttpGet("GetAsync")]
        public async Task<List<oa_leave_image>> GetAsync([FromQuery]string leave_no)
        {
            return await _leaveService.GetAsync(leave_no);
        }

        /// <summary>
        /// 请假记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetLeaveRecord")]
        public async Task<Page<LeaveRecord>> GetLeaveRecord([FromQuery]LeaveRecordSearch entity)
        {
            return await _leaveService.GetLeaveRecord(entity);
        }
    }
}