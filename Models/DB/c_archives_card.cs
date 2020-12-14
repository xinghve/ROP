using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 会员卡
    /// </summary>
    public class c_archives_card
    {
        /// <summary>
        /// 会员卡
        /// </summary>
        public c_archives_card()
        {
        }

        private System.Int32 _id;
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public System.Int32 id { get { return this._id; } set { this._id = value; } }

        private System.Int32? _archives_id;
        /// <summary>
        /// 档案ID
        /// </summary>
        public System.Int32? archives_id { get { return this._archives_id; } set { this._archives_id = value ?? default(System.Int32); } }

        private System.String _archives;
        /// <summary>
        /// 档案姓名
        /// </summary>
        public System.String archives { get { return this._archives; } set { this._archives = value?.Trim(); } }

        private System.String _archives_phone;
        /// <summary>
        /// 档案手机
        /// </summary>
        public System.String archives_phone { get { return this._archives_phone; } set { this._archives_phone = value?.Trim(); } }

        private System.Int32? _level_id;
        /// <summary>
        /// 等级ID
        /// </summary>
        public System.Int32? level_id { get { return this._level_id; } set { this._level_id = value ?? default(System.Int32); } }

        private System.String _level;
        /// <summary>
        /// 等级
        /// </summary>
        public System.String level { get { return this._level; } set { this._level = value?.Trim(); } }

        private System.String _card_no;
        /// <summary>
        /// 卡号
        /// </summary>
        public System.String card_no { get { return this._card_no; } set { this._card_no = value?.Trim(); } }

        private System.String _password;
        /// <summary>
        /// 支付密码
        /// </summary>
        public System.String password { get { return this._password; } set { this._password = value?.Trim(); } }

        private System.Decimal? _total_money;
        /// <summary>
        /// 总金额
        /// </summary>
        public System.Decimal? total_money { get { return this._total_money; } set { this._total_money = value ?? default(System.Decimal); } }

        private System.Decimal? _balance;
        /// <summary>
        /// 余额
        /// </summary>
        public System.Decimal? balance { get { return this._balance; } set { this._balance = value ?? default(System.Decimal); } }

        private System.Int32? _total_integral;
        /// <summary>
        /// 总积分
        /// </summary>
        public System.Int32? total_integral { get { return this._total_integral; } set { this._total_integral = value ?? default(System.Int32); } }

        private System.Int32? _integral;
        /// <summary>
        /// 剩余积分
        /// </summary>
        public System.Int32? integral { get { return this._integral; } set { this._integral = value ?? default(System.Int32); } }

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

        private System.Int32 _store_id;
        /// <summary>
        /// 门店id
        /// </summary>
        public System.Int32 store_id { get { return this._store_id; } set { this._store_id = value; } }

        private System.Decimal? _give_total_money;
        /// <summary>
        /// 赠送总金额
        /// </summary>
        public System.Decimal? give_total_money { get { return this._give_total_money; } set { this._give_total_money = value ?? default(System.Decimal); } }

        private System.Decimal? _give_balance;
        /// <summary>
        /// 赠送余额
        /// </summary>
        public System.Decimal? give_balance { get { return this._give_balance; } set { this._give_balance = value ?? default(System.Decimal); } }
    }
}