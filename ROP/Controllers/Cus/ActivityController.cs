using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Repository.Interfaces.Crm;
using Tools.Authorize;

namespace ROP.Controllers.Cus
{

    /// <summary>
    /// 活动
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeForArchives]
    public class ActivityController : ControllerBase
    {
        private readonly IActivityService _activityService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activityService"></param>
        public ActivityController(IActivityService activityService)
        {
            _activityService = activityService;
        }

        /// <summary>
        /// 获取图片（客户端）
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetByNow")]
        public async Task<dynamic> GetByNowAsync()
        {
            return await _activityService.GetByNowAsync();
        }

        /// <summary>
        /// 获取活动详情（客户端）
        /// </summary>
        /// <param name="activity_id">活动id</param>
        /// <returns></returns>
        [HttpGet("GetDetials")]
        public async Task<dynamic> GetDetialsAsync([FromQuery] int activity_id)
        {
            return await _activityService.GetDetialsAsync(activity_id);
        }
    }
}