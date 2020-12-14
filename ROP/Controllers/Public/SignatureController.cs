using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Senparc.Weixin.MP;
using Senparc.Weixin;
using Senparc.Weixin.MP.MvcExtension;
using Senparc.Weixin.MP.Entities.Request;
using System.Threading;
using ROP.Controllers.CustomMessage;
using Senparc.CO2NET.HttpUtility;
using Senparc.NeuChar.MessageHandlers;

namespace ROP.Controllers.Public
{
    /// <summary>
    /// 微信Signature
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SignatureController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        public SignatureController()
        {
            
        }

        /// <summary>
        /// 微信测试token验证地址
        /// </summary>
        /// <param name="echostr">echostr</param>
        /// <param name="signature">signature</param>
        /// <param name="timestamp">timestamp</param>
        /// <param name="nonce">nonce</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Get(string echostr, string signature, string timestamp, string nonce)
        {
            if (CheckSignature.Check(signature, timestamp, nonce, Config.SenparcWeixinSetting.Token))
            {
                return Content(echostr); //返回随机字符串则表示验证通过
            }
            else
            {
                return Content("failed:" + signature + "," + timestamp + "," + nonce + "," + echostr + "," + Config.SenparcWeixinSetting.Token + "," + CheckSignature.GetSignature(timestamp, nonce, Config.SenparcWeixinSetting.Token));
            }
        }

        /// <summary>
        ///                              
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="timestamp"></param>
        /// <param name="nonce"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Post(string signature, string timestamp, string nonce)
        {
            if (!CheckSignature.Check(signature, timestamp, nonce, Config.SenparcWeixinSetting.Token))
            {
                return new WeixinResult("参数错误！");
            }

            PostModel postModel = new PostModel();

            postModel.Token = Config.SenparcWeixinSetting.Token;
            postModel.EncodingAESKey = Config.SenparcWeixinSetting.EncodingAESKey; //根据自己后台的设置保持一致
            postModel.AppId = Config.SenparcWeixinSetting.WeixinAppId; //根据自己后台的设置保持一致

            var cancellationToken = new CancellationToken();//给异步方法使用

            var messageHandler = new CustomMessageHandlers(Request.GetRequestMemoryStream(), postModel, 10);

            messageHandler.DefaultMessageHandlerAsyncEvent = DefaultMessageHandlerAsyncEvent.SelfSynicMethod;

            messageHandler.OmitRepeatedMessage = true;//默认已经开启，此处仅作为演示，也可以设置为false在本次请求中停用此功能

            messageHandler.SaveRequestMessageLog();//记录 Request 日志（可选）

            await messageHandler.ExecuteAsync(cancellationToken); //执行微信处理过程（关键）

            messageHandler.SaveResponseMessageLog();//记录 Response 日志（可选）

            return new FixWeixinBugWeixinResult(messageHandler);
        }
    }
}