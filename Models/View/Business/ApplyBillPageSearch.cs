using Models.View.Public;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Business
{
    /// <summary>
    /// 申请单分页
    /// </summary>
    public class ApplyBillPageSearch : Search
    {
        /// <summary>
        /// 申请单号
        /// </summary>
        public string apply_no { get; set; }

        /// <summary>
        /// 门店ID
        /// </summary>
        public int store_id { get; set; }

        /// <summary>
        /// 状态（0=待处理，1=已处理）
        /// </summary>
        public short dispose_state { get; set; }

        private System.DateTime? _start_date;
        /// <summary>
        /// 申请开始时间
        /// </summary>
        public System.DateTime? start_date { get { return this._start_date; } set { this._start_date = value; } }

        private System.DateTime? _end_date;
        /// <summary>
        /// 申请结束时间
        /// </summary>
        public System.DateTime? end_date { get { return this._end_date; } set { this._end_date = value; } }
    }
}
