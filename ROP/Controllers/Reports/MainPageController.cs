using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.View.Reports;
using Service.Repository.Interfaces.Reports;

namespace ROP.Controllers.Reports
{
    /// <summary>
    /// 主页报表
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MainPageController : ControllerBase
    {
        private readonly IMainPageService _mainPageService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mainPageService"></param>
        public MainPageController(IMainPageService mainPageService)
        {
            _mainPageService = mainPageService;
        }

        /// <summary>
        /// 主页报表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetMainPage")]
        public async Task<MainPageModel> GetMainPage([FromQuery]MainPageSearch entity)
        {
            return await _mainPageService.GetMainPage(entity);
        }

        /// <summary>
        /// 业绩
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetRecharge")]
        public async Task<rechargeList> GetRecharge([FromQuery]MainPageSearch entity)
        {
            return await _mainPageService.GetRecharge(entity);
        }

        /// <summary>
        /// 整体情况
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetTotal")]
        public async Task<totalList> GetTotal([FromQuery]MainPageSearch entity)
        {
            return await _mainPageService.GetTotal(entity);
        }
    }
}