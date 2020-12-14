using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 活动优惠券关联
    /// </summary>
    public class c_activity_coupon
    {
        /// <summary>
        /// 活动优惠券关联
        /// </summary>
        public c_activity_coupon()
        {
        }

        private System.Int32 _activity_id;
        /// <summary>
        /// 活动ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 activity_id { get { return this._activity_id; } set { this._activity_id = value; } }

        private System.Int32 _coupon_id;
        /// <summary>
        /// 优惠券ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 coupon_id { get { return this._coupon_id; } set { this._coupon_id = value; } }

        private System.DateTime? _start_date;
        /// <summary>
        /// 使用开始时间
        /// </summary>
        public System.DateTime? start_date { get { return this._start_date; } set { this._start_date = value ?? default(System.DateTime); } }

        private System.DateTime? _end_date;
        /// <summary>
        /// 使用结束时间
        /// </summary>
        public System.DateTime? end_date { get { return this._end_date; } set { this._end_date = value ?? default(System.DateTime); } }

        private System.String _remarks;
        /// <summary>
        /// 备注
        /// </summary>
        public System.String remarks { get { return this._remarks; } set { this._remarks = value?.Trim(); } }
    }
}