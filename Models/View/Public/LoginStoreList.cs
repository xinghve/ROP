using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Public
{
    /// <summary>
    /// 登录返回门店列表
    /// </summary>
    public class LoginStoreList
    {
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 是否门店超级管理员
        /// </summary>
        public bool is_admin { get; set; }
    }
}
