using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 领取活动优惠券
    /// </summary>
    public class c_archives_activity_coupon
    {
        /// <summary>
        /// 领取活动优惠券
        /// </summary>
        public c_archives_activity_coupon()
        {
        }

        private System.Int32 _archives_id;
        /// <summary>
        /// 档案ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 archives_id { get { return this._archives_id; } set { this._archives_id = value; } }

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

        private System.String _coupon_no;
        /// <summary>
        /// 券码
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String coupon_no { get { return this._coupon_no; } set { this._coupon_no = value?.Trim(); } }

        private System.Int16? _state;
        /// <summary>
        /// 状态（2=已使用，3=未使用）
        /// </summary>
        public System.Int16? state { get { return this._state; } set { this._state = value ?? default(System.Int16); } }

        private System.DateTime? _use_date;
        /// <summary>
        /// 使用开始时间
        /// </summary>
        public System.DateTime? use_date { get { return this._use_date; } set { this._use_date = value ?? default(System.DateTime); } }


        private System.String _remarks;
        /// <summary>
        /// 备注
        /// </summary>
        public System.String remarks { get { return this._remarks; } set { this._remarks = value?.Trim(); } }

        private System.Int32 _org_id;
        /// <summary>
        /// 机构id
        /// </summary>
        public System.Int32 org_id { get { return this._org_id; } set { this._org_id = value; } }

        private System.Int32 _store_id;
        /// <summary>
        /// 门店id
        /// </summary>
        public System.Int32 store_id { get { return this._store_id; } set { this._store_id = value; } }
    }
}