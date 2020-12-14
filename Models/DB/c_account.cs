using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 
    /// </summary>
    public class c_account
    {
        /// <summary>
        /// 
        /// </summary>
        public c_account()
        {
        }

        private System.Int32 _archives_id;
        /// <summary>
        /// 档案ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 archives_id { get { return this._archives_id; } set { this._archives_id = value; } }

        private System.Decimal _coupon;
        /// <summary>
        /// 优惠券
        /// </summary>
        public System.Decimal coupon { get { return this._coupon; } set { this._coupon = value; } }

        private System.Decimal _recharge;
        /// <summary>
        /// 充值金额
        /// </summary>
        public System.Decimal recharge { get { return this._recharge; } set { this._recharge = value; } }

        private System.Decimal _consume;
        /// <summary>
        /// 本次消费
        /// </summary>
        public System.Decimal consume { get { return this._consume; } set { this._consume = value; } }

        private System.Decimal _balance;
        /// <summary>
        /// 余额
        /// </summary>
        public System.Decimal balance { get { return this._balance; } set { this._balance = value; } }

        private System.Decimal _integral;
        /// <summary>
        /// 积分
        /// </summary>
        public System.Decimal integral { get { return this._integral; } set { this._integral = value; } }

        private System.String _password;
        /// <summary>
        /// 密码
        /// </summary>
        public System.String password { get { return this._password; } set { this._password = value?.Trim(); } }

        private System.Decimal _salseamount;
        /// <summary>
        /// 销售金额
        /// </summary>
        public System.Decimal salseamount { get { return this._salseamount; } set { this._salseamount = value; } }

        private System.Decimal _rate;
        /// <summary>
        /// 提成比例
        /// </summary>
        public System.Decimal rate { get { return this._rate; } set { this._rate = value; } }

        private System.Decimal _settleamount;
        /// <summary>
        /// 已结提成
        /// </summary>
        public System.Decimal settleamount { get { return this._settleamount; } set { this._settleamount = value; } }

        private System.Decimal _noneamount;
        /// <summary>
        /// 未结提成
        /// </summary>
        public System.Decimal noneamount { get { return this._noneamount; } set { this._noneamount = value; } }

        private System.String _code;
        /// <summary>
        /// 验证码
        /// </summary>
        public System.String code { get { return this._code; } set { this._code = value?.Trim(); } }

        private System.Decimal? _amount;
        /// <summary>
        /// 提成总金额
        /// </summary>
        public System.Decimal? amount { get { return this._amount; } set { this._amount = value ?? default(System.Decimal); } }

        private System.Int32? _total_integral;
        /// <summary>
        /// 总积分
        /// </summary>
        public System.Int32? total_integral { get { return this._total_integral; } set { this._total_integral = value ?? default(System.Int32); } }

        private System.Decimal? _total_coupon;
        /// <summary>
        /// 总优惠金额
        /// </summary>
        public System.Decimal? total_coupon { get { return this._total_coupon; } set { this._total_coupon = value ?? default(System.Decimal); } }
    }
}