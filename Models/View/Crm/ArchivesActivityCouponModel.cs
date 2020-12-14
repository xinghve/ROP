using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Crm
{
    /// <summary>
    /// 领取活动实体
    /// </summary>
    public class ArchivesActivityCouponModel
    {
        private System.Int32 _id;
        /// <summary>
        /// 活动id
        /// </summary>
        public System.Int32 id { get { return this._id; } set { this._id = value; } }

        private System.Int32 _store_id;
        /// <summary>
        /// 门店id
        /// </summary>
        public System.Int32 store_id { get { return this._store_id; } set { this._store_id = value; } }

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

        /// <summary>
        /// 优惠券List
        /// </summary>
        public List<c_coupon> aCModel { get; set; }

        /// <summary>
        /// 档案List
        /// </summary>
        public List<int> aArray { get; set; }
    }
}
