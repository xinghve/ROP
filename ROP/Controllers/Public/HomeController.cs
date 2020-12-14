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
    /// 首页
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IHomeService _homeService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="homeService"></param>
        public HomeController(IHomeService homeService)
        {
            _homeService = homeService;
        }

        /// <summary>
        /// 获取节日（包括生日档案）
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetFestival")]
        public async Task<object> GetFestivalAsync()
        {
            return await _homeService.GetFestival();
        }

        /// <summary>
        /// 获取随访计划
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetFollow")]
        public async Task<object> GetFollowAsync()
        {
            return await _homeService.GetFollow();
        }

        /// <summary>
        /// 获取客户余额下限
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetLowerBalance")]
        public async Task<object> GetLowerBalance()
        {
            return await _homeService.GetLowerBalance();
        }

        /// <summary>
        /// 修改生日消息已读
        /// </summary>
        /// <returns></returns>
        [HttpPut("UpdateBirthday")]
        public async Task UpdateBirthday([FromBody]UpdateModel entity)
        {
             await _homeService.UpdateBirthday(entity.id);
        }

        /// <summary>
        /// 修改随访消息已读
        /// </summary>
        /// <returns></returns>
        [HttpPut("UpdateFollowUp")]
        public async Task UpdateFollowUp([FromBody]UpdateModel entity)
        {
            await _homeService.UpdateFollowUp(entity.id);
        }

        /// <summary>
        /// 修改余额下限消息已读
        /// </summary>
        /// <returns></returns>
        [HttpPut("UpdateBalanceLower")]
        public async Task UpdateBalanceLower([FromBody]UpdateModel entity)
        {
            await _homeService.UpdateBalanceLower(entity.id);
        }
    }
}