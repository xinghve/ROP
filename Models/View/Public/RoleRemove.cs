using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Public
{
    /// <summary>
    /// 移除部门
    /// </summary>
    public class RoleRemove
    {
        /// <summary>
        /// 门店ID
        /// </summary>
        public int store_id { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        public int role_id { get; set; }
    }
}
