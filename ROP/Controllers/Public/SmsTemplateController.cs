using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Service.Repository.Interfaces.Public;
using Tools;

namespace ROP.Controllers.Public
{
    /// <summary>
    /// 短信模板
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SmsTemplateController : ControllerBase
    {
        private readonly ISmsTemplateService _smsTemplateService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="smsTemplateService"></param>
        public SmsTemplateController(ISmsTemplateService smsTemplateService)
        {
            _smsTemplateService = smsTemplateService;
        }

        /// <summary>
        /// 添加短信模板
        /// </summary>
        /// <param name="entity">短信模板信息</param>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<int> Add([FromBody]p_sms_template entity)
        {
            return await _smsTemplateService.AddAsync(entity);
        }

        /// <summary>
        /// 修改短信模板
        /// </summary>
        /// <param name="entity">短信模板信息</param>
        /// <returns></returns>
        [HttpPut("Modify")]
        public async Task<int> Modify([FromBody]p_sms_template entity)
        {
            return await _smsTemplateService.ModifyAsync(entity);
        }

        /// <summary>
        /// 删除短信模板
        /// </summary>
        /// <param name="list">短信模板ID</param>
        /// <returns></returns>
        [HttpDelete("Delete")]
        public async Task<int> Delete([FromBody]List<int> list)
        {
            return await _smsTemplateService.DeleteAsync(list);
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="name">模板名称</param>
        /// <param name="isSelect">是否可选（1=可选，0=不可选，-1=所有）</param>
        /// <returns></returns>
        [HttpGet("GetList")]
        public async Task<List<p_sms_template>> GetListAsync([FromQuery] string name, [FromQuery] int isSelect = -1)
        {
            return await _smsTemplateService.GetListAsync(name, isSelect);
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="name">模板名称</param>
        /// <param name="isSelect">是否可选（1=可选，0=不可选，-1=所有）</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        [HttpGet("GetPages")]
        public async Task<Page<p_sms_template>> GetPagesAsync([FromQuery] string name, [FromQuery] int isSelect, [FromQuery] string order, [FromQuery] int orderType, [FromQuery] int limit, [FromQuery] int page)
        {
            return await _smsTemplateService.GetPagesAsync(name, isSelect, order, orderType, limit, page);
        }
    }
}