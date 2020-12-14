using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.View.His;
using Service.Repository.Interfaces.His;

namespace ROP.Controllers.Mobile
{
    /// <summary>
    /// 就诊
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ClinicRecordController : ControllerBase
    {
        private readonly IClinicRecordService _clinicRecordService;

        /// <summary>
        /// 
        /// </summary>
        public ClinicRecordController(IClinicRecordService clinicRecordService)
        {
            _clinicRecordService = clinicRecordService;
        }

        /// <summary>
        /// 获取负责人客户签到列表
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="state_id">状态（1=已签到，0=未签到）</param>
        /// <returns></returns>
        [HttpGet("GetSignList")]
        public async Task<dynamic> GetSignListAsync([FromQuery]int store_id, [FromQuery]short state_id = 0)
        {
            var list = await _clinicRecordService.GetSignListAsync(store_id, state_id);
            return new { num = list.Count, list };
        }
    }
}