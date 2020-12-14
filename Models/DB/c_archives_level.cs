using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 
    /// </summary>
    public class c_archives_level
    {
        /// <summary>
        /// 
        /// </summary>
        public c_archives_level()
        {
        }

        private System.Int32 _id;
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public System.Int32 id { get { return this._id; } set { this._id = value; } }

        private System.String _name;
        /// <summary>
        /// 名称
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.Int16? _money_integral;
        /// <summary>
        /// 现金积分（x元=1积分）
        /// </summary>
        public System.Int16? money_integral { get { return this._money_integral; } set { this._money_integral = value ?? default(System.Int16); } }

        private System.Int16? _integral_money;
        /// <summary>
        /// 积分兑现（x积分=1元）
        /// </summary>
        public System.Int16? integral_money { get { return this._integral_money; } set { this._integral_money = value ?? default(System.Int16); } }

        private System.Decimal? _discount_rate;
        /// <summary>
        /// 折扣率
        /// </summary>
        public System.Decimal? discount_rate { get { return this._discount_rate; } set { this._discount_rate = value ?? default(System.Decimal); } }

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

        private System.Int32 _org_id;
        /// <summary>
        /// 机构id
        /// </summary>
        public System.Int32 org_id { get { return this._org_id; } set { this._org_id = value; } }

        private System.Decimal? _min_money;
        /// <summary>
        /// 最小金额
        /// </summary>
        public System.Decimal? min_money { get { return this._min_money; } set { this._min_money = value ?? default(System.Decimal); } }

        private System.Decimal? _max_money;
        /// <summary>
        /// 最大金额
        /// </summary>
        public System.Decimal? max_money { get { return this._max_money; } set { this._max_money = value ?? default(System.Decimal); } }

        private System.Decimal? _royalty_rate;
        /// <summary>
        /// 提成比例
        /// </summary>
        public System.Decimal? royalty_rate { get { return this._royalty_rate; } set { this._royalty_rate = value ?? default(System.Decimal); } }

        private System.Decimal? _balance_limit_lower;
        /// <summary>
        /// 余额下限
        /// </summary>
        public System.Decimal? balance_limit_lower { get { return this._balance_limit_lower; } set { this._balance_limit_lower = value ?? default(System.Decimal); } }

        private System.Int16? _special;
        /// <summary>
        /// 是否特批
        /// </summary>
        public System.Int16? special { get { return this._special; } set { this._special = value ?? default(System.Int16); } }
    }
}