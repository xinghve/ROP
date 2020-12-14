using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Public
{
    /// <summary>
    /// 角色
    /// </summary>
    public class Role : p_role
    {
        /// <summary>
        /// 部门名称
        /// </summary>
        public string dept_name { get; set; }
    }
}
