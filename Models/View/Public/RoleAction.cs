using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Public
{
    /// <summary>
    /// 角色权限
    /// </summary>
    public class RoleAction
    {
        /// <summary>
        /// 角色id
        /// </summary>
        public int role_id { get; set; }
        /// <summary>
        /// 权限数组int[]
        /// </summary>
        public List<int> actions { get; set; }
    }
}
