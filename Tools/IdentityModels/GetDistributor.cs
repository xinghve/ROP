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
    /// 获取分销人员信息
    /// </summary>
    public class GetDistributor
    {
        /// <summary>
        /// 请求头参数
        /// </summary>
        public readonly IHttpContextAccessor httpContextAccessor = new HttpContextAccessor();

        /// <summary>
        /// 
        /// </summary>
        public p_distributor Distributor = new p_distributor();

        /// <summary>
        /// 
        /// </summary>
        public GetDistributor()
        {
            var redisAuthorize = httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
            var redisCache = new RedisCache(ConfigExtensions.Configuration["ConnectionStrings:RedisConnection"]);
            if (redisCache.ContainsKey<p_distributor>(redisAuthorize))
            {
                Distributor = redisCache.Get<p_distributor>(redisAuthorize);
            }
            else
            {
                throw new IdentityException("未获取到用户信息");
            }
        }
    }
}
