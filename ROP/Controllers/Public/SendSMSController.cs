using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Models.View.Public;
using Newtonsoft.Json;
using Service.Repository.Interfaces.Public;
using Tools;

namespace ROP.Controllers.Public
{
    /// <summary>
    /// 短信发送
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SendSMSController : ControllerBase
    {
        private readonly ISendSMSService _sendSMSService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sendSMSService"></param>
        public SendSMSController(ISendSMSService sendSMSService)
        {
            _sendSMSService = sendSMSService;
        }

        /// <summary>
        /// 发送短信（注册）
        /// </summary>
        /// <returns></returns>
        [HttpPost("SendRegister")]
        public async Task<bool> SendRegister([FromBody]SendSMS entity)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            Random ra = new Random();
            var raCode = ra.Next(100000, 1000000).ToString();
            values.Add("code", raCode);
            values.Add("product", "数字化康复运营平台");
            var toValues = JsonConvert.SerializeObject(values);
            return await _sendSMSService.SendSmsAsync(entity.phone_no, 1, toValues, raCode, entity.type, entity.org_id);
        }

        /// <summary>
        /// 发送短信（登录确认）
        /// </summary>
        /// <returns></returns>
        [HttpPost("SendLogin")]
        public async Task<bool> SendLogin([FromBody]SendSMS entity)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            Random ra = new Random();
            var raCode = ra.Next(100000, 1000000).ToString();
            values.Add("code", raCode);
            values.Add("product", "数字化康复运营平台");
            var toValues = JsonConvert.SerializeObject(values);
            return await _sendSMSService.SendSmsAsync(entity.phone_no, 2, toValues, raCode, entity.type, entity.org_id);
        }

        /// <summary>
        /// 发送短信（忘记密码）
        /// </summary>
        /// <returns></returns>
        [HttpPost("SendForgetPwd")]
        public async Task<bool> SendForgetPwd([FromBody]SendSMS entity)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            Random ra = new Random();
            var raCode = ra.Next(100000, 1000000).ToString();
            values.Add("code", raCode);
            values.Add("product", "数字化康复运营平台");
            var toValues = JsonConvert.SerializeObject(values);
            return await _sendSMSService.SendSmsAsync(entity.phone_no, 3, toValues, raCode, entity.type, entity.org_id);
        }

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <returns></returns>
        [HttpPost("SendSms")]
        public async Task<bool> SendSms([FromBody]SendSMSGroup entity)
        {
            return await _sendSMSService.SendBatchSmsAsync(entity);
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="storeId">门店ID</param>
        /// <param name="templateId">模板ID</param>
        /// <param name="phone">手机号</param>
        /// <param name="status">状态（1：成功；0：失败；-1：所有）</param>
        /// <param name="content">内容</param>
        /// <param name="is_all">是否所有</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        [HttpGet("GetPages")]
        public async Task<Page<p_sms_send_record>> GetPagesAsync([FromQuery]int storeId, [FromQuery] int templateId, [FromQuery] string phone, [FromQuery] int status, [FromQuery] string content, [FromQuery] bool is_all, [FromQuery] string order, [FromQuery] int orderType, [FromQuery] int limit, [FromQuery] int page)
        {
            return await _sendSMSService.GetPageAsync(storeId, templateId, phone, status, content, is_all, order, orderType, limit, page);
        }
    }
}