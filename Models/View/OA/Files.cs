using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.OA
{
    /// <summary>
    /// 文件
    /// </summary>
    public class Files
    {
        /// <summary>
        /// 文件夹路径
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// 门店ID
        /// </summary>
        public int store_id { get; set; }

        /// <summary>
        /// 门店名称
        /// </summary>
        public string store_name { get; set; }
    }
}
