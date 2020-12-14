using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 收费员交账单
    /// </summary>
    public class f_checkout
    {
        /// <summary>
        /// 收费员交账单
        /// </summary>
        public f_checkout()
        {
        }

        private System.Int32 _settle_accounts_id;
        /// <summary>
        /// 结账ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public System.Int32 settle_accounts_id { get { return this._settle_accounts_id; } set { this._settle_accounts_id = value; } }

        private System.Int32 _orgid;
        /// <summary>
        /// 机构ID
        /// </summary>
        public System.Int32 orgid { get { return this._orgid; } set { this._orgid = value; } }

        private System.Int32 _store_id;
        /// <summary>
        /// 门店ID
        /// </summary>
        public System.Int32 store_id { get { return this._store_id; } set { this._store_id = value; } }

        private System.DateTime? _recorddate;
        /// <summary>
        /// 记录时间
        /// </summary>
        public System.DateTime? recorddate { get { return this._recorddate; } set { this._recorddate = value ?? default(System.DateTime); } }

        private System.DateTime? _settle_date;
        /// <summary>
        /// 结账时间
        /// </summary>
        public System.DateTime? settle_date { get { return this._settle_date; } set { this._settle_date = value ?? default(System.DateTime); } }

        private System.Int32? _operatorid;
        /// <summary>
        /// 结算人ID
        /// </summary>
        public System.Int32? operatorid { get { return this._operatorid; } set { this._operatorid = value ?? default(System.Int32); } }

        private System.String _operator_name;
        /// <summary>
        /// 结算人
        /// </summary>
        public System.String operator_name { get { return this._operator_name; } set { this._operator_name = value?.Trim(); } }

        private System.Decimal? _total_money;
        /// <summary>
        /// 总金额
        /// </summary>
        public System.Decimal? total_money { get { return this._total_money; } set { this._total_money = value ?? default(System.Decimal); } }

        private System.Int16? _numbers;
        /// <summary>
        /// 票据张数
        /// </summary>
        public System.Int16? numbers { get { return this._numbers; } set { this._numbers = value ?? default(System.Int16); } }

        private System.Decimal? _total_cashpay;
        /// <summary>
        /// 现金合计
        /// </summary>
        public System.Decimal? total_cashpay { get { return this._total_cashpay; } set { this._total_cashpay = value ?? default(System.Decimal); } }

        private System.Decimal _accountpay;
        /// <summary>
        /// 账户支付
        /// </summary>
        public System.Decimal accountpay { get { return this._accountpay; } set { this._accountpay = value; } }

        private System.Decimal _insurancepay;
        /// <summary>
        /// 医保支付
        /// </summary>
        public System.Decimal insurancepay { get { return this._insurancepay; } set { this._insurancepay = value; } }

        private System.Decimal _wechatpay;
        /// <summary>
        /// 微信支付
        /// </summary>
        public System.Decimal wechatpay { get { return this._wechatpay; } set { this._wechatpay = value; } }

        private System.Decimal _alipay;
        /// <summary>
        /// 支付宝支付
        /// </summary>
        public System.Decimal alipay { get { return this._alipay; } set { this._alipay = value; } }

        private System.Decimal _bankpay;
        /// <summary>
        /// 银行卡支付
        /// </summary>
        public System.Decimal bankpay { get { return this._bankpay; } set { this._bankpay = value; } }

        private System.Decimal _otherpay;
        /// <summary>
        /// 其他支付
        /// </summary>
        public System.Decimal otherpay { get { return this._otherpay; } set { this._otherpay = value; } }

        private System.String _summay;
        /// <summary>
        /// 摘要
        /// </summary>
        public System.String summay { get { return this._summay; } set { this._summay = value?.Trim(); } }

        private System.Decimal _couponpay;
        /// <summary>
        /// 优惠金额支付
        /// </summary>
        public System.Decimal couponpay { get { return this._couponpay; } set { this._couponpay = value; } }
    }
}