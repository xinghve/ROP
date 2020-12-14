using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Public
{
    /// <summary>
    /// 提现分页实体
    /// </summary>
    public class CashExtent: r_cash_withdrawal
    {
        /// <summary>
        /// 分销人员手机号
        /// </summary>
        public string distributor_phone { get; set; }
        /// <summary>
        /// 门店名
        /// </summary>
        public string store_name { get; set; }

        /// <summary>
        /// 分销人员
        /// </summary>
        public string distributor_name { get; set; }
    }

    /// <summary>
    /// 提现查询
    /// </summary>
    public class CashSearch : Search
    {
        /// <summary>
        /// 门店id
        /// </summary>
        public int store_id { get; set; }

        /// <summary>
        /// 查询条件（分销人员/分销手机/审核人）
        /// </summary>
        public string search_condition { get; set; }

        /// <summary>
        /// 审核状态
        /// </summary>
        public int state { get; set; }

        private System.DateTime? _startTime;
        /// <summary>
        /// 提现开始时间
        /// </summary>
        public System.DateTime? startTime { get { return this._startTime; } set { this._startTime = value ; } }

        private System.DateTime? _endTime;
        /// <summary>
        ///  提现结束时间
        /// </summary>
        public System.DateTime? endTime { get { return this._endTime; } set { this._endTime = value; } }

        private System.DateTime? _audit_startTime;
        /// <summary>
        /// 审核开始时间
        /// </summary>
        public System.DateTime? audit_startTime { get { return this._audit_startTime; } set { this._audit_startTime = value ; } }

        private System.DateTime? _audit_endTime;
        /// <summary>
        /// 审核结束时间
        /// </summary>
        public System.DateTime? audit_endTime { get { return this._audit_endTime; } set { this._audit_endTime = value; } }

        private System.DateTime? _finish_startTime;
        /// <summary>
        /// 到账开始时间
        /// </summary>
        public System.DateTime? finish_startTime { get { return this._finish_startTime; } set { this._finish_startTime = value; } }

        private System.DateTime? _finish_endTime;
        /// <summary>
        ///到账结束时间
        /// </summary>
        public System.DateTime? finish_endTime { get { return this._finish_endTime; } set { this._finish_endTime = value; } }

    }
}
