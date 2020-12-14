using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.View.Reports;
using Service.Repository.Interfaces.Reports;
using Tools;

namespace ROP.Controllers.Reports
{
    /// <summary>
    /// 活动优惠券领取详情
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityCouponController : ControllerBase
    {
        private readonly IActivityCouponService _activityCouponService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activityCouponService"></param>
        public ActivityCouponController(IActivityCouponService activityCouponService)
        {
            _activityCouponService = activityCouponService;
        }

        /// <summary>
        /// 获取活动优惠券分页数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetACouponPage")]
        public async Task<Page<AcCouponModel>> GetACouponPage([FromQuery]AcCouponSearch entity)
        {
            return await _activityCouponService.GetACouponPage(entity);
        }
    }
}