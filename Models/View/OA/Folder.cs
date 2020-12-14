using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.OA
{
    /// <summary>
    /// 文件夹
    /// </summary>
    public class Folder
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 路径
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
