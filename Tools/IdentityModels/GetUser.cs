using Microsoft.AspNetCore.Http;
using Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Cache;
using Tools.Filter;

namespace Tools.IdentityModels
{
    /// <summary>
    /// 获取用户信息
    /// </summary>
    public class GetUser
    {

        /// <summary>
        /// 请求头参数
        /// </summary>
        public readonly IHttpContextAccessor httpContextAccessor = new HttpContextAccessor();

        /// <summary>
        /// 
        /// </summary>
        public UserInfo userInfo = new UserInfo();

        /// <summary>
        /// 
        /// </summary>
        public GetUser()
        {
            var loginUser = (from c in httpContextAccessor.HttpContext.User.Claims select new MyIdentityModel { Type = c.Type, Value = c.Value }).ToList();
            if (loginUser.Count <= 0)
            {
                throw new IdentityException("未获取到用户信息");
            }
            userInfo.id = Convert.ToInt32(loginUser.Where(s => s.Type == "id").First().Value);
            userInfo.name = loginUser.Where(s => s.Type == "name").First().Value;
            userInfo.phone_no = loginUser.Where(s => s.Type == "phone_no").First().Value;
            userInfo.org_id = Convert.ToInt32(loginUser.Where(s => s.Type == "org_id").First().Value);
            userInfo.is_admin = Convert.ToBoolean(loginUser.Where(s => s.Type == "is_admin").First().Value);
        }

        /// <summary>
        /// 
        /// </summary>
        public class MyIdentityModel
        {
            public string Type { get; set; }
            public string Value { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        public class UserInfo
        {
            /// <summary>
            /// ID
            /// </summary>
            public int id { get; set; }
            /// <summary>
            /// 姓名
            /// </summary>
            public string name { get; set; }
            /// <summary>
            /// 电话
            /// </summary>
            public string phone_no { get; set; }
            /// <summary>
            /// 集团id
            /// </summary>
            public int org_id { get; set; }
            /// <summary>
            /// 是否超级管理员
            /// </summary>
            public bool is_admin { get; set; }
        }
    }
}
