using Models.View.Public;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.OA
{
    /// <summary>
    /// 会议分页
    /// </summary>
    public class MeetingPageSearch : Search
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
        /// 编号
        /// </summary>
        public string no { get; set; }

        /// <summary>
        /// 类型ID
        /// </summary>
        public int type_id { get; set; }

        /// <summary>
        /// 是否私有
        /// </summary>
        public bool is_me { get; set; }
    }
}
