using Models.View.Public;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Crm
{
    /// <summary>
    /// 活动优惠券明细
    /// </summary>
    public class ActivityDetialsPageSearch : Search
    {

        private System.String _name;
        /// <summary>
        /// 名称
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.DateTime? _start_date;
        /// <summary>
        /// 开始时间
        /// </summary>
        public System.DateTime? start_date { get { return this._start_date; } set { this._start_date = value ?? default(System.DateTime); } }

        private System.DateTime? _end_date;
        /// <summary>
        /// 结束时间
        /// </summary>
        public System.DateTime? end_date { get { return this._end_date; } set { this._end_date = value ?? default(System.DateTime); } }

        private System.Int32 _store_id;
        /// <summary>
        /// 门店id
        /// </summary>
        public System.Int32 store_id { get { return this._store_id; } set { this._store_id = value; } }

        private System.Int16? _state;
        /// <summary>
        /// 状态（1=正常，0=暂停）
        /// </summary>
        public System.Int16? state { get { return this._state; } set { this._state = value ?? default(System.Int16); } }

    }
}
