using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Public
{
    /// <summary>
    /// 排班时间段
    /// </summary>
    public class SchedulingTimes
    {
        /// <summary>
        /// 门店ID
        /// </summary>
        public int store_id { get; set; }

        /// <summary>
        /// 时间段
        /// </summary>
        public List<Time> times { get; set; }
    }

    /// <summary>
    /// 时间段
    /// </summary>
    public class Time
    {
        private System.TimeSpan? _start;
        /// <summary>
        /// 开始时间
        /// </summary>
        public System.TimeSpan? start { get { return this._start; } set { this._start = value ?? default(System.TimeSpan); } }

        private System.TimeSpan? _end;
        /// <summary>
        /// 结束时间
        /// </summary>
        public System.TimeSpan? end { get { return this._end; } set { this._end = value ?? default(System.TimeSpan); } }
    }
}
