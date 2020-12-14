using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.His
{
    /// <summary>
    /// 挂号日期
    /// </summary>
    public class RegisterDate
    {
        /// <summary>
        /// 日期
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// 星期
        /// </summary>
        public string Week { get; set; }

        /// <summary>
        /// 是否预约（true=预约 false=挂号）
        /// </summary>
        public bool is_order { get; set; }

        /// <summary>
        /// 时间段
        /// </summary>
        public List<Scheduletimes> list { get; set; }
    }
}
