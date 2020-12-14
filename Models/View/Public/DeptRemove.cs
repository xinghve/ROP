using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Public
{
    /// <summary>
    /// 移除部门
    /// </summary>
    public class DeptRemove
    {
        /// <summary>
        /// 门店ID
        /// </summary>
        public int store_id { get; set; }

        /// <summary>
        /// 部门ID
        /// </summary>
        public int dept_id { get; set; }
    }
}
