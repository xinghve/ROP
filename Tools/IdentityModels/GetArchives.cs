using Microsoft.AspNetCore.Http;
using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;
using Tools.Cache;
using Tools.Filter;

namespace Tools.IdentityModels
{
    /// <summary>
    /// 获取客户信息
    /// </summary>
    public class GetArchives
    {
        /// <summary>
        /// 请求头参数
        /// </summary>
        public readonly IHttpContextAccessor httpContextAccessor = new HttpContextAccessor();

        /// <summary>
        /// 
        /// </summary>
        public c_archives archives = new c_archives();

        /// <summary>
        /// 
        /// </summary>
        public GetArchives()
        {
            var redisAuthorize = httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
            var redisCache = new RedisCache(ConfigExtensions.Configuration["ConnectionStrings:RedisConnection"]);
            if (redisCache.ContainsKey<c_archives>(redisAuthorize))
            {
                archives = redisCache.Get<c_archives>(redisAuthorize);
            }
            else
            {
                throw new IdentityException("未获取到用户信息");
            }
        }
    }
}
