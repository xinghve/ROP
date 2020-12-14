using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Models.View.Crm;
using Service.Repository.Interfaces.Crm;
using Tools;

namespace ROP.Controllers.Crm
{
    /// <summary>
    /// 活动管理--ty
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly IActivityService _activityService;
        /// <summary>
        /// 活动构造
        /// </summary>
        /// <param name="activityService"></param>
        public ActivityController(IActivityService activityService)
        {
            _activityService = activityService;
        }

        /// <summary>
        /// 活动分页数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("GetPageAsync")]
        public Task<Page<ActivityPageModel>> GetPageAsync([FromQuery]ActivitySearchModel model)
        {
            return _activityService.GetPageAsync(model);
        }

        /// <summary>
        /// 添加活动、优惠券
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<bool> Add([FromBody]ActivityCouponModel entity)
        {
            return await _activityService.Add(entity);
        }

        /// <summary>
        /// 编辑活动、添加优惠券
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("Modify")]
        public async Task<bool> Modify([FromBody]ActivityCouponModel entity)
        {
            return await _activityService.Modify(entity);
        }

        /// <summary>
        /// 启用、禁用活动
        /// </summary>
        /// <param name="entity">state1正常0暂停</param>
        /// <returns></returns>
        [HttpPut("ModifyEnable")]
        public async Task<int> ModifyEnable([FromBody]c_activity entity)
        {
            return await _activityService.ModifyEnable(entity);
        }

        /// <summary>
        /// 删除活动
        /// </summary>
        /// <returns></returns>
        [HttpDelete("delete")]
        public async Task<bool> Delete([FromBody]c_activity entity)
        {
            return await _activityService.Delete(entity.id);
        }

        /// <summary>
        /// 领取优惠券
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("CouponCollection")]
        public async Task<bool> CouponCollection([FromBody]ArchivesActivityCouponModel entity)
        {
            return await _activityService.CouponCollection(entity);
        }

        /// <summary>
        /// 根据活动优惠券返回档案用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("ReturnArchives")]
        public async Task<List<int>> ReturnArchives([FromQuery]ReturnArchivesModel model)
        {
            return await _activityService.ReturnArchives(model);
        }

        /// <summary>
        /// 设置图片
        /// </summary>
        /// <param name="item">图片</param>
        /// <returns>成功/失败</returns>
        [HttpPost]
        public async Task<bool> Post([FromBody] ActivityIMGModel item)
        {
            dynamic type = (new Program()).GetType();
            return await _activityService.SetAsync(item, type);
        }

        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="activity_id">活动id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<r_activity_img>> Get([FromQuery]int activity_id)
        {
            return await _activityService.GetAsync(activity_id);
        }

        /// <summary>
        /// 优惠券领取分页记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetRecivePage")]
        public async Task<Page<ReceiveCouponModel>> GetRecivePage([FromQuery]ReceiveCouponSearch entity)
        {
            return await _activityService.GetRecivePage(entity);
        }
    }
}