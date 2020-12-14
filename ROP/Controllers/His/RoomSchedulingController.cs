using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Models.View.His;
using Service.Repository.Interfaces.His;
using Tools;

namespace ROP.Controllers.His
{
    /// <summary>
    /// 医疗室安排
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RoomSchedulingController : ControllerBase
    {
        private readonly IRoomSchedulingService _roomSchedulingService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomSchedulingService"></param>
        public RoomSchedulingController(IRoomSchedulingService roomSchedulingService)
        {
            _roomSchedulingService = roomSchedulingService;
        }

        /// <summary>
        /// 开始
        /// </summary>
        /// <returns></returns>
        [HttpPost("Start")]
        public async Task<bool> StartAsync([FromBody]Execute entity)
        {
            return await _roomSchedulingService.StartAsync(entity);
        }

        /// <summary>
        /// 通知
        /// </summary>
        /// <returns></returns>
        [HttpPost("Notice")]
        public async Task<bool> NoticeAsync([FromBody]Execute entity)
        {
            return await _roomSchedulingService.NoticeAsync(entity);
        }

        /// <summary>
        /// 结束
        /// </summary>
        /// <returns></returns>
        [HttpPost("End")]
        public async Task<bool> EndAsync([FromBody]Execute entity)
        {
            return await _roomSchedulingService.EndAsync(entity);
        }

        /// <summary>
        /// 获得分页列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetPages")]
        public async Task<dynamic> GetPagesAsync([FromQuery]RoomSchedulingSearch entity)
        {
            return await _roomSchedulingService.GetPagesAsync(entity);
        }
    }
}