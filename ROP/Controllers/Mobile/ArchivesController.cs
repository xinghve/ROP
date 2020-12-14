using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.View.Crm;
using Models.View.Mobile;
using Service.Repository.Interfaces.Crm;
using Service.Repository.Interfaces.Public;
using Tools;

namespace ROP.Controllers.Mobile
{
    /// <summary>
    /// 客户
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ArchivesController : ControllerBase
    {
        private readonly IACardService _aCardService;
        private readonly IArchivesService _archivesService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="aCardService"></param>
        /// <param name="archivesService"></param>
        public ArchivesController(IACardService aCardService, IArchivesService archivesService)
        {
            _aCardService = aCardService;
            _archivesService = archivesService;
        }

        /// <summary>
        /// 获取充值业绩
        /// </summary>
        /// <param name="type">类型（1=年 2=月 3=日）</param>
        /// <returns></returns>
        [HttpGet("GetRecharge")]
        public async Task<dynamic> GetRechargeAsync([FromQuery] short type)
        {
            return await _aCardService.GetRechargeAsync(type);
        }

        /// <summary>
        /// 获取客户量
        /// </summary>
        /// <param name="type">类型（1=年 2=月 3=日）</param>
        /// <returns></returns>
        [HttpGet("GetArchivesCount")]
        public async Task<dynamic> GetArchivesCountAsync([FromQuery] short type)
        {
            return await _archivesService.GetArchivesCountAsync(type);
        }

        /// <summary>
        /// 获取充值业绩排行
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="type">类型（1=年 2=月 3=日）</param>
        /// <returns></returns>
        [HttpGet("GetRechargeOrder")]
        public async Task<dynamic> GetRechargeOrderAsync([FromQuery] int store_id, [FromQuery]  short type)
        {
            return await _aCardService.GetRechargeOrderAsync(store_id, type);
        }

        /// <summary>
        /// 获取充值业绩排行
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="type">类型（0=所有 1=年 2=月 3=日）</param>
        /// <returns></returns>
        [HttpGet("GetTotalRechargeOrder")]
        public async Task<dynamic> GetTotalRechargeOrderAsync([FromQuery] int store_id, [FromQuery]  short type)
        {
            return await _aCardService.GetTotalRechargeOrderAsync(store_id, type);
        }

        /// <summary>
        /// 添加档案信息
        /// </summary>
        /// <param name="entity">档案信息信息</param>
        /// <returns></returns>
        [HttpPost("MobileAdd")]
        public async Task<bool> MobileAdd([FromBody]MobileArchives entity)
        {
            return await _archivesService.MobileAddAsync(entity);
        }

        /// <summary>
        /// 获取会员分页记录
        /// </summary>
        /// <param name="str">姓名/手机号</param>
        /// <param name="storeID">门店ID</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        [HttpGet("GetMobileCardPages")]
        public async Task<Page<CardPageModel>> GetMobileCardPagesAsync([FromQuery]string str, [FromQuery] int storeID, [FromQuery] string order, [FromQuery] int orderType, [FromQuery] int limit, [FromQuery] int page)
        {
            var model = new ACardModel { name = str, phone = str, order = order, orderType = orderType, limit = limit, page = page, storeId = storeID, is_me = true };
            return await _aCardService.GetPageAsync(model, true);
        }

        /// <summary>
        /// 修改档案信息
        /// </summary>
        /// <param name="entity">档案信息信息</param>
        /// <returns></returns>
        [HttpPut("MobileModify")]
        public async Task<bool> MobileModifyAsync([FromBody]MobileArchives entity)
        {
            return await _archivesService.MobileModifyAsync(entity);
        }

        /// <summary>
        /// 根据Id获取档案账户
        /// </summary>
        /// <param name="id">档案ID</param>
        /// <returns></returns>
        [HttpGet("GetAccount")]
        public async Task<dynamic> GetAccountAsync([FromQuery]int id)
        {
            return await _aCardService.GetAccountAsync(id);
        }


    }
}