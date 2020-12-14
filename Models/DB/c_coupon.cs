using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 优惠券
    /// </summary>
    public class c_coupon
    {
        /// <summary>
        /// 优惠券
        /// </summary>
        public c_coupon()
        {
        }

        private System.Int32 _id;
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public System.Int32 id { get { return this._id; } set { this._id = value; } }

        private System.Int32 _org_id;
        /// <summary>
        /// 机构id
        /// </summary>
        public System.Int32 org_id { get { return this._org_id; } set { this._org_id = value; } }

        private System.String _name;
        /// <summary>
        /// 名称
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.Decimal? _use_money;
        /// <summary>
        /// 使用金额
        /// </summary>
        public System.Decimal? use_money { get { return this._use_money; } set { this._use_money = value ?? default(System.Decimal); } }

        private System.Decimal? _deduction_money;
        /// <summary>
        /// 抵扣金额
        /// </summary>
        public System.Decimal? deduction_money { get { return this._deduction_money; } set { this._deduction_money = value ?? default(System.Decimal); } }

        private System.Int32? _creater_id;
        /// <summary>
        /// 创建人ID
        /// </summary>
        public System.Int32? creater_id { get { return this._creater_id; } set { this._creater_id = value ?? default(System.Int32); } }

        private System.String _creater;
        /// <summary>
        /// 创建人
        /// </summary>
        public System.String creater { get { return this._creater; } set { this._creater = value?.Trim(); } }

        private System.DateTime? _create_date;
        /// <summary>
        /// 创建时间
        /// </summary>
        public System.DateTime? create_date { get { return this._create_date; } set { this._create_date = value ?? default(System.DateTime); } }

        private System.String _remarks;
        /// <summary>
        /// 备注
        /// </summary>
        public System.String remarks { get { return this._remarks; } set { this._remarks = value?.Trim(); } }

        private System.Int32 _store_id;
        /// <summary>
        /// 门店id
        /// </summary>
        public System.Int32 store_id { get { return this._store_id; } set { this._store_id = value; } }

        private System.Int16? _status;
        /// <summary>
        /// 1:启用0：禁用
        /// </summary>
        public System.Int16? status { get { return this._status; } set { this._status = value ?? default(System.Int16); } }
    }
}