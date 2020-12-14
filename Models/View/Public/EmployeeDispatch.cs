using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Public
{
    /// <summary>
    /// 人员调度
    /// </summary>
    public class EmployeeDispatch
    {
        /// <summary>
        /// 人员ID
        /// </summary>
        public int employeeID { get; set; }

        /// <summary>
        /// 调出门店
        /// </summary>
        public int fromStoreID { get; set; }
        /// <summary>
        /// 调入门店
        /// </summary>
        public int toStoreID { get; set; }

        /// <summary>
        /// 部门角色
        /// </summary>
      //  public List<deptRole> deptRoles { get; set; }
    }

    /// <summary>
    /// 部门角色
    /// </summary>
    public class deptRole {
        /// <summary>
        /// 部门id
        /// </summary>
        public int dept_id { get; set; }
        /// <summary>
        /// 角色id
        /// </summary>
        public int role_id { get; set; }
    }
}
