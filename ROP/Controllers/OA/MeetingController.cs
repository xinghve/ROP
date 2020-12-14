using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Models.View.OA;
using Service.Repository.Interfaces.OA;
using Tools;

namespace ROP.Controllers.OA
{
    /// <summary>
    /// 会议
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MeetingController : ControllerBase
    {
        private readonly IMeetingService _meetingService;
        /// <summary>
        /// 文件管理构造
        /// </summary>
        /// <param name="meetingService"></param>
        public MeetingController(IMeetingService meetingService)
        {
            _meetingService = meetingService;
        }

        /// <summary>
        /// 添加会议
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("AddMeeting")]
        public async Task<bool> AddMeetingAsync([FromBody]Meeting entity)
        {
            return await _meetingService.AddMeetingAsync(entity);
        }

        /// <summary>
        /// 取消会议
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpDelete("CancelMeeting")]
        public async Task<int> CancelMeetingAsync([FromBody]oa_meeting entity)
        {
            return await _meetingService.CancelMeetingAsync(entity);
        }

        /// <summary>
        /// 延期会议
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("DelayMeeting")]
        public async Task<int> DelayMeetingAsync([FromBody]oa_meeting entity)
        {
            return await _meetingService.DelayMeetingAsync(entity);
        }

        /// <summary>
        /// 获得分页列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetPage")]
        public async Task<Page<Meeting>> GetPageAsync([FromQuery]MeetingPageSearch entity)
        {
            return await _meetingService.GetPageAsync(entity);
        }

        /// <summary>
        /// 保存决议
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("SaveResolution")]
        public async Task<bool> SaveResolutionAsync([FromBody]ResolutionList entity)
        {
            return await _meetingService.SaveResolutionAsync(entity);
        }

        /// <summary>
        /// 保存发言
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("SaveSpeak")]
        public async Task<bool> SaveSpeakAsync([FromBody]SpeakList entity)
        {
            return await _meetingService.SaveSpeakAsync(entity);
        }

        /// <summary>
        /// 获取会议记录
        /// </summary>
        /// <param name="id">会议ID</param>
        /// <returns></returns>
        [HttpGet("GetRecord")]
        public async Task<string> GetRecordAsync([FromQuery]int id)
        {
            return await _meetingService.GetRecordAsync(id);
        }

        /// <summary>
        /// 修改会议记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("ModifyRecord")]
        public async Task<bool> ModifyRecordAsync([FromBody]oa_meeting_employee entity)
        {
            return await _meetingService.ModifyRecordAsync(entity);
        }
    }
}