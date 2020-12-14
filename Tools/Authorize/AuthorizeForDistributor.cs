using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Tools.Cache;
using Tools.Filter;

namespace Tools.Authorize
{
    /// <summary>
    /// 客户认证
    /// </summary>
    public class AuthorizeForDistributor : TypeFilterAttribute
    {
        #region 字段

        private readonly bool _ignoreFilter;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ignore">是否忽略过滤。默认为false</param>
        public AuthorizeForDistributor(bool ignore = false) : base(typeof(AuthorizeDistributorFilter))
        {
            this._ignoreFilter = ignore;
            this.Arguments = new object[] { ignore };
        }

        #endregion

        #region 属性

        /// <summary>
        /// 获取是否忽略过滤？
        /// </summary>
        public bool IgnoreFilter => _ignoreFilter;

        #endregion


        #region 内部过滤器

        /// <summary>
        /// 管理员授权过滤器
        /// </summary>
        private class AuthorizeDistributorFilter : IAuthorizationFilter
        {
            #region 字段

            private readonly bool _ignoreFilter;
            //private readonly IPermissionService _permissionService; 
            //假设这个 IPermissionService 是我们业务上需要访问数据库获取用户是否有权限访问的类。

            #endregion

            #region 构造函数

            public AuthorizeDistributorFilter(bool ignoreFilter /*, IPermissionService permissionService*/ )
            {
                this._ignoreFilter = ignoreFilter;
                //this._permissionService = permissionService;
            }

            #endregion

            #region 方法

            public void OnAuthorization(AuthorizationFilterContext filterContext)
            {
                if (filterContext == null)
                    throw new ArgumentNullException(nameof(filterContext));

                //检查是否已经被 Action 方法重写了
                var actionFilter = filterContext.ActionDescriptor.FilterDescriptors
                    .Where(filterDescriptor => filterDescriptor.Scope == FilterScope.Action)
                    .Select(filterDescriptor => filterDescriptor.Filter).OfType<AuthorizeForDistributor>().FirstOrDefault();


                if (actionFilter?.IgnoreFilter ?? _ignoreFilter)
                    return;

                if (filterContext.Filters.Any(filter => filter is AuthorizeDistributorFilter))
                {
                    //下面是访问自定义的服务，获取当前登录用户是否有权限访问
                    //bool hasPermission =  _permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel);
                    var redisAuthorize = filterContext.HttpContext.Request.Headers["Authorization"].ToString();
                    if (string.IsNullOrEmpty(redisAuthorize))
                    {
                        filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.NonAuthoritativeInformation;
                        filterContext.Result = new StatusCodeResult(203);
                    }
                    //bool hasPermission = new Random().Next(1, 11) > 5 ? true : false;
                    var redisCache = new RedisCache(ConfigExtensions.Configuration["ConnectionStrings:RedisConnection"]);
                    if (!redisCache.ContainsKey<p_distributor>(redisAuthorize))
                    {
                        //filterContext.Result = new ChallengeResult();
                        filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.NonAuthoritativeInformation;
                        filterContext.Result = new StatusCodeResult(203);
                    }
                }
            }

            #endregion
        }

        #endregion
    }
}
