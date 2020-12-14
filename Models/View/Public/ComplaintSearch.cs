using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Public
{
    /// <summary>
    /// 投诉
    /// </summary>
    public class ComplaintSearch : Search
    {
        /// <summary>
        /// 门店ID
        /// </summary>
        public int store_id { get; set; }

        /// <summary>
        /// 24待处理 25已处理
        /// </summary>
        public int state { get; set; }

        /// <summary>
        /// 档案姓名/手机号
        /// </summary>
        public string archives { get; set; }

        private System.DateTime? _start_date;
        /// <summary>
        /// 开始时间
        /// </summary>
        public System.DateTime? start_date { get { return this._start_date; } set { this._start_date = value ?? default(System.DateTime); } }

        private System.DateTime? _end_date;
        /// <summary>
        /// 结束时间
        /// </summary>
        public System.DateTime? end_date { get { return this._end_date; } set { this._end_date = value; } }
    }

    /// <summary>
    /// 投诉分页实体
    /// </summary>
    public class ComplaintPageModel : r_complaint
    {
        /// <summary>
        /// 门店名
        /// </summary>
        public string storeName { get; set; }
    }

}
