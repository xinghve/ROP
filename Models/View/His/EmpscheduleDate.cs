using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.His
{
    /// <summary>
    /// 排班日期
    /// </summary>
    public class EmpscheduleDate
    {
        /// <summary>
        /// 预约开始时间
        /// </summary>
        public DateTime orderbegin { get; set; }

        /// <summary>
        /// 预约结束时间
        /// </summary>
        public DateTime orderend { get; set; }
    }
}
