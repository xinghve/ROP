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
    /// 排班时间段
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SchedulingTimeController : ControllerBase
    {
        private readonly ISchedulingTimeService _schedulingTimeService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schedulingTimeService"></param>
        public SchedulingTimeController(ISchedulingTimeService schedulingTimeService)
        {
            _schedulingTimeService = schedulingTimeService;
        }

        /// <summary>
        /// 获取排班时间段
        /// </summary>
        /// <param name="storeId">门店Id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<Time>> Get([FromQuery]int storeId)
        {
            return await _schedulingTimeService.GetListAsync(storeId);
        }

        /// <summary>
        /// 设置排班时间段
        /// </summary>
        /// <param name="item">排班时间段</param>
        /// <returns>成功/失败</returns>
        [HttpPost]
        public bool Post([FromBody] SchedulingTimes item)
        {
            return _schedulingTimeService.SetAsync(item);
        }
    }
}