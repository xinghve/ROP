using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Public
{
    /// <summary>
    /// 角色
    /// </summary>
    public class RoleCopy
    {
        /// <summary>
        /// 门店ID
        /// </summary>
        public int store_id { get; set; }

        /// <summary>
        /// 部门ID
        /// </summary>
        public int dept_id { get; set; }

        /// <summary>
        /// 角色ID集合
        /// </summary>
        public List<int> role_ids { get; set; }
    }

    /// <summary>
    /// 部门角色List
    /// </summary>
    public class ProcessDeptRoleModel
    {
        /// <summary>
        /// 部门id
        /// </summary>
        public int dept_id { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string dept_name { get; set; }

        /// <summary>
        /// 是否机构流程
        /// </summary>
        public bool is_org { get; set; }

        /// <summary>
        /// 职位List
        /// </summary>
        public List<p_role> roleList { get; set; }
    }
}
