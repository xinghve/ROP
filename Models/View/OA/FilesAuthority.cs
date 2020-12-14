using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.OA
{
    /// <summary>
    /// 文件权限
    /// </summary>
    public class FilesAuthority : oa_files
    {
        /// <summary>
        /// 权限数组
        /// </summary>
        public List<authority> authorities { get; set; }
    }

    /// <summary>
    /// 权限
    /// </summary>
    public class authority
    {
        /// <summary>
        /// 关联ID
        /// </summary>
        public int link_id { get; set; }
        /// <summary>
        /// 关联类型
        /// </summary>
        public int link_type { get; set; }
        /// <summary>
        /// 关联名称
        /// </summary>
        public string link_name { get; set; }
    }
}
