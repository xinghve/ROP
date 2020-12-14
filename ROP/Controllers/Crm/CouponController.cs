using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
    /// 优惠券--ty
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly ICouponService _couponService;
        
        /// <summary>
        /// 优惠券构造
        /// </summary>
        /// <param name="couponService"></param>
        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        /// <summary>
        /// 获取优惠券分页数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("GetPageAsync")]
        public async Task<Page<c_coupon>> GetPageAsync([FromQuery]SearchMl model)
        {
            return await _couponService.GetPageAsync(model);
        }

        /// <summary>
        /// 添加优惠券
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<int> Add([FromBody]c_coupon entity)
        {
            return await _couponService.Add(entity);
        }

        /// <summary>
        /// 启用、禁用优惠券
        /// </summary>
        /// <param name="entity">0禁用1启用</param>
        /// <returns></returns>
        [HttpPut("Modify")]
        public async Task<int> Modify([FromBody]c_coupon entity)
        {
            return await _couponService.Modify(entity);
        }

        /// <summary>
        /// 删除优惠券
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpDelete("Delete")]
        public async Task<int> Delete([FromBody]c_coupon entity)
        {
            return await _couponService.Delete(entity.id);
        }

        /// <summary>
        /// 获取优惠券列表
        /// </summary>
        /// <param name="store_id">门店id</param>
        /// <returns></returns>
        [HttpGet("GetList")]
        public async Task<List<c_coupon>> GetList([FromQuery]int store_id)
        {
            return await _couponService.GetList(store_id);
        }
    }
}