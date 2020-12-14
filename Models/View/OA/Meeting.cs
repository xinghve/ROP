using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.OA
{
    /// <summary>
    /// 会议
    /// </summary>
    public class Meeting : oa_meeting
    {
        /// <summary>
        /// 参会人员
        /// </summary>
        public List<oa_meeting_employee> meeting_Employees { get; set; }

        /// <summary>
        /// 会议附件
        /// </summary>
        public List<oa_meeting_accessories> meeting_Accessories { get; set; }

        /// <summary>
        /// 会议发言
        /// </summary>
        public List<oa_meeting_speak> meeting_Speaks { get; set; }

        /// <summary>
        /// 会议决议
        /// </summary>
        public List<oa_meeting_resolution> meeting_Resolutions { get; set; }
    }
}
