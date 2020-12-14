using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Models.View.Business;
using Service.Repository.Interfaces.Business;
using Tools;

namespace ROP.Controllers.Business
{
    /// <summary>
    /// 通知
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class NoticeController : ControllerBase
    {
        private readonly INoticeService _noticeService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="noticeService"></param>
        public NoticeController(INoticeService noticeService)
        {
            _noticeService = noticeService;
        }

        /// <summary>
        /// 添加通知
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("AddNotice")]
        public async Task<bool> AddNotice([FromBody]List<AddNoticeModel> entity)
        {
            return await _noticeService.AddNoticeAsync(entity);
        }

        /// <summary>
        /// 获取基础类型
        /// </summary>
        /// <param name="value_id"></param>
        /// <param name="property_id"></param>
        /// <returns></returns>
        [HttpGet("GetBaseDetail")]
        public b_basecode GetBaseDetail([FromQuery]short value_id,[FromQuery] short property_id)
        {
            return _noticeService.GetBaseDetail(value_id,property_id);
        }

        /// <summary>
        /// 获取通知分页
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetNoticePage")]
        public async Task<object> GetNoticePage([FromQuery]NoticeSearchModel entity)
        {
            return await _noticeService.GetNoticePage(entity);
        }

        /// <summary>
        /// 获取通知类型
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetBaseType")]
        public List<b_basecode> GetBaseType()
        {
            return _noticeService.GetBaseType();
        }

        /// <summary>
        /// 标为已读
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("SetRead")]
        public async Task<int> SetRead([FromBody]NoticeRead entity)
        {
            return await _noticeService.SetRead(entity);
        }
    }
}