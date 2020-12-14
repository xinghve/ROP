using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Tools.Cache;

namespace Tools.Filter
{
    /// <summary>
    /// 全局过滤器
    /// </summary>
    public class HttpGlobalExceptionFilter : DbContext, IExceptionFilter
    {
        //private readonly IHostingEnvironment env;
        ////private readonly ILogger<HttpGlobalExceptionFilter> logger;

        //public HttpGlobalExceptionFilter(IHostingEnvironment env, ILogger<HttpGlobalExceptionFilter> logger)
        //{
        //    this.env = env;
        //    //this.logger = logger;
        //}

        public void OnException(ExceptionContext context)
        {
            //logger.LogError(new EventId(context.Exception.HResult),
            //    context.Exception,
            //    context.Exception.Message);
            var log = new r_logs { code = DateTime.Now.ToString("yyyyMMddHHmmssffff"), content = context.Exception.Message, date = DateTime.Now, ip = Utils.GetIp(), parameter = "", url = Utils.GetUrl() };
            if (httpContextAccessor.HttpContext.User.Claims.Count() > 0)
            {
                //获取用户信息
                var userInfo = new IdentityModels.GetUser().userInfo;
                log.creater = userInfo.name;
                log.creater_id = userInfo.id;
            }
            else
            {
                var redisAuthorize = httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
                var redisCache = new RedisCache(ConfigExtensions.Configuration["ConnectionStrings:RedisConnection"]);
                if (redisCache.ContainsKey<c_archives>(redisAuthorize))
                {
                    //获取客户信息
                    var archivesInfo = new IdentityModels.GetArchives().archives;
                    log.creater = archivesInfo.name;
                    log.creater_id = archivesInfo.id;
                }
            }
            if (context.Exception.GetType() == typeof(IdentityException))
            {
                log.type = "认证";
                log.state = 1;
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.NonAuthoritativeInformation;
            }
            else if (context.Exception.GetType() == typeof(MessageException))
            {
                log.type = "提示";
                log.state = 1;
                context.Result = new AcceptedResult(context.HttpContext.Request.Path, context.Exception.Message.ToString());
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Accepted;
            }
            else
            {
                log.type = "错误";
                log.state = 0;
                var json = new JsonErrorResponse
                {
                    Messages = new[] { "发生错误" },
                };

                json.DeveloperMeesage = context.Exception;

                //if (env.IsDevelopment())
                //{
                //    json.DeveloperMeesage = context.Exception;
                //}

                context.Result = new InternalServerErrorObjectResult(json);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            context.ExceptionHandled = true;

            Db.Insertable(log).RemoveDataCache().ExecuteCommand();
        }

        private class JsonErrorResponse
        {
            public string[] Messages { get; set; }

            public object DeveloperMeesage { get; set; }
        }
    }
}
