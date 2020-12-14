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
    /// 会员卡--ty
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ACardController : ControllerBase
    {
        private readonly IACardService _aCardService;

        /// <summary>
        /// 会员构造
        /// </summary>
        /// <param name="aCardService"></param>
        public ACardController(IACardService aCardService)
        {
            _aCardService = aCardService;
        }

        /// <summary>
        /// 获取会员分页记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("GetPageAsync")]
        public async Task<Page<CardPageModel>> GetPageAsync([FromQuery]ACardModel model)
        {
            return await _aCardService.GetPageAsync(model);
        }

        /// <summary>
        /// 充值
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Recharge")]
        public async Task<bool> Recharge([FromBody]RechargeModel model)
        {
            return await _aCardService.Recharge(model);
        }

        /// <summary>
        /// 充值记录分页数据
        /// </summary>
        /// <param name="model"></param>
        /// <param name="name_or_phone"></param>
        /// <param name="is_me"></param>
        /// <returns></returns>
        [HttpGet("GetRechargePageAsync")]
        public async Task<Page<Recharge>> GetRechargePageAsync([FromQuery]RJLModel model, [FromQuery] bool name_or_phone = false, [FromQuery] bool is_me = false)
        {
            return await _aCardService.GetRechargePageAsync(model, name_or_phone, is_me);
        }

        /// <summary>
        /// 获取打印数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("GetPrintAsync")]
        public async Task<List<CardPageModel>> GetPrintAsync([FromQuery]PrintModel model)
        {
            return await _aCardService.GetPrintAsync(model);
        }

        /// <summary>
        /// 获取门店优惠券金额
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="no"></param>
        /// <returns></returns>
        [HttpGet("GetCouponMoney")]
        public async Task<object> GetCouponMoney([FromQuery]int storeId, [FromQuery] string no)
        {
            return await _aCardService.GetCouponMoney(storeId, no);
        }

        /// <summary>
        /// 消费记录（PC）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetConsumerPagesAsync")]
        public async Task<Page<ConsumerList>> GetConsumerPagesAsync([FromQuery] ConsumerSearch entity)
        {
            return await _aCardService.GetConsumerPagesAsync(entity);
        }

        /// <summary>
        /// 获取消费详情
        /// </summary>
        /// <param name="balance_id"></param>
        /// <returns></returns>
        [HttpGet("GetConsumerDetail")]
        public async Task<List<f_balancedetail>> GetConsumerDetail([FromQuery]int balance_id)
        {
            return await _aCardService.GetConsumerDetail(balance_id);
        }
    }
}