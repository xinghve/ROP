using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 优惠券明细（新）
    /// </summary>
    public class c_new_coupon_detials
    {
        /// <summary>
        /// 优惠券明细（新）
        /// </summary>
        public c_new_coupon_detials()
        {
        }

        private System.Int32 _id;
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 id { get { return this._id; } set { this._id = value; } }

        private System.Int32 _activity_id;
        /// <summary>
        /// 活动ID
        /// </summary>
        public System.Int32 activity_id { get { return this._activity_id; } set { this._activity_id = value; } }

        private System.Int32 _org_id;
        /// <summary>
        /// 机构id
        /// </summary>
        public System.Int32 org_id { get { return this._org_id; } set { this._org_id = value; } }

        private System.Decimal? _money;
        /// <summary>
        /// 使用金额
        /// </summary>
        public System.Decimal? money { get { return this._money; } set { this._money = value ?? default(System.Decimal); } }

        private System.String _no;
        /// <summary>
        /// 券码
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String no { get { return this._no; } set { this._no = value?.Trim(); } }

        private System.Int16? _overlay;
        /// <summary>
        /// 叠加
        /// </summary>
        public System.Int16? overlay { get { return this._overlay; } set { this._overlay = value ?? default(System.Int16); } }

        private System.String _address;
        /// <summary>
        /// 地址
        /// </summary>
        public System.String address { get { return this._address; } set { this._address = value?.Trim(); } }

        private System.String _remarks;
        /// <summary>
        /// 备注
        /// </summary>
        public System.String remarks { get { return this._remarks; } set { this._remarks = value?.Trim(); } }

        private System.Int16? _use_state;
        /// <summary>
        /// 是否已使用
        /// </summary>
        public System.Int16? use_state { get { return this._use_state; } set { this._use_state = value ?? default(System.Int16); } }
    }
}