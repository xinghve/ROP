using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Public
{
    /// <summary>
    /// 门店权限
    /// </summary>
    public class StoreAction
    {
        /// <summary>
        /// 门店id
        /// </summary>
        public int store_id { get; set; }
        /// <summary>
        /// 权限数组int[]
        /// </summary>
        public List<int> actions { get; set; }
    }
}
