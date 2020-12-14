using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Public
{
    /// <summary>
    /// 部门
    /// </summary>
    public class DeptCopy
    {
        /// <summary>
        /// 门店ID
        /// </summary>
        public int store_id { get; set; }

        /// <summary>
        /// 部门ID集合
        /// </summary>
        public List<int> dept_ids { get; set; }
    }
}
