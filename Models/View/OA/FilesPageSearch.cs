using Models.View.Public;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.OA
{
    /// <summary>
    /// 文件分页
    /// </summary>
    public class FilesPageSearch : Search
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 门店ID
        /// </summary>
        public int store_id { get; set; }

        /// <summary>
        /// 路径
        /// </summary>
        public string url { get; set; }

        private bool Is_me = false;
        /// <summary>
        /// 
        /// </summary>
        public bool is_me { get => Is_me; set => Is_me = value; }
    }
}
