using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 优惠券（新）
    /// </summary>
    public class c_new_coupon
    {
        /// <summary>
        /// 优惠券（新）
        /// </summary>
        public c_new_coupon()
        {
        }

        private System.Int32 _id;
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
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

        private System.String _head;
        /// <summary>
        /// 券码头
        /// </summary>
        public System.String head { get { return this._head; } set { this._head = value?.Trim(); } }

        private System.Int16? _num;
        /// <summary>
        /// 号数
        /// </summary>
        public System.Int16? num { get { return this._num; } set { this._num = value ?? default(System.Int16); } }

        private System.Int16? _overlay;
        /// <summary>
        /// 叠加
        /// </summary>
        public System.Int16? overlay { get { return this._overlay; } set { this._overlay = value ?? default(System.Int16); } }

        private System.String _remarks;
        /// <summary>
        /// 备注
        /// </summary>
        public System.String remarks { get { return this._remarks; } set { this._remarks = value?.Trim(); } }

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
    }
}