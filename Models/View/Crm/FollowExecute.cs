using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Crm
{
    /// <summary>
    /// 随访执行
    /// </summary>
    public class FollowExecute
    {
        /// <summary>
        /// 随访ID
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 答复内容
        /// </summary>
        public string answer { get; set; }
    }
    /// <summary>
    /// 回访记录
    /// </summary>

    public class repayModel
    {
        /// <summary>
        /// 回访记录
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 答复内容
        /// </summary>
        public string answer { get; set; }

        /// <summary>
        /// 实施内容
        /// </summary>
        public string content { get; set; }
    }
}
