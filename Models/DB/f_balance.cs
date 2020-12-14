using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 门诊收费结算单
    /// </summary>
    public class f_balance
    {
        /// <summary>
        /// 门诊收费结算单
        /// </summary>
        public f_balance()
        {
        }

        private System.Int32 _balanceid;
        /// <summary>
        /// 结算ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public System.Int32 balanceid { get { return this._balanceid; } set { this._balanceid = value; } }

        private System.Int16 _typeid;
        /// <summary>
        /// 结算类型ID
        /// </summary>
        public System.Int16 typeid { get { return this._typeid; } set { this._typeid = value; } }

        private System.Int32 _clinicid;
        /// <summary>
        /// 就诊ID
        /// </summary>
        public System.Int32 clinicid { get { return this._clinicid; } set { this._clinicid = value; } }

        private System.Int32 _centerid;
        /// <summary>
        /// 档案ID
        /// </summary>
        public System.Int32 centerid { get { return this._centerid; } set { this._centerid = value; } }

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

        private System.Int32 _doctorid;
        /// <summary>
        /// 开单医生ID
        /// </summary>
        public System.Int32 doctorid { get { return this._doctorid; } set { this._doctorid = value; } }

        private System.String _doctorname;
        /// <summary>
        /// 开单医生
        /// </summary>
        public System.String doctorname { get { return this._doctorname; } set { this._doctorname = value?.Trim(); } }

        private System.Int32 _deptid;
        /// <summary>
        /// 开单科室ID
        /// </summary>
        public System.Int32 deptid { get { return this._deptid; } set { this._deptid = value; } }

        private System.String _deptname;
        /// <summary>
        /// 开单科室
        /// </summary>
        public System.String deptname { get { return this._deptname; } set { this._deptname = value?.Trim(); } }

        private System.DateTime _recorddate;
        /// <summary>
        /// 记录时间
        /// </summary>
        public System.DateTime recorddate { get { return this._recorddate; } set { this._recorddate = value; } }

        private System.DateTime? _balancedate;
        /// <summary>
        /// 结算时间
        /// </summary>
        public System.DateTime? balancedate { get { return this._balancedate; } set { this._balancedate = value; } }

        private System.Int32? _operatorid;
        /// <summary>
        /// 结算人ID
        /// </summary>
        public System.Int32? operatorid { get { return this._operatorid; } set { this._operatorid = value; } }

        private System.String _operator_name;
        /// <summary>
        /// 结算人
        /// </summary>
        public System.String operator_name { get { return this._operator_name; } set { this._operator_name = value?.Trim(); } }

        private System.Int16 _insuranceid;
        /// <summary>
        /// 险种ID
        /// </summary>
        public System.Int16 insuranceid { get { return this._insuranceid; } set { this._insuranceid = value; } }

        private System.Int16? _sourceid;
        /// <summary>
        /// 来源ID
        /// </summary>
        public System.Int16? sourceid { get { return this._sourceid; } set { this._sourceid = value; } }

        private System.String _source;
        /// <summary>
        /// 来源
        /// </summary>
        public System.String source { get { return this._source; } set { this._source = value?.Trim(); } }

        private System.Int16 _issettle;
        /// <summary>
        /// 结算标志
        /// </summary>
        public System.Int16 issettle { get { return this._issettle; } set { this._issettle = value; } }

        private System.Decimal _shouldamount;
        /// <summary>
        /// 应收金额
        /// </summary>
        public System.Decimal shouldamount { get { return this._shouldamount; } set { this._shouldamount = value; } }

        private System.Decimal _actualamount;
        /// <summary>
        /// 实收金额
        /// </summary>
        public System.Decimal actualamount { get { return this._actualamount; } set { this._actualamount = value; } }

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

        private System.Decimal _cashpay;
        /// <summary>
        /// 现金支付
        /// </summary>
        public System.Decimal cashpay { get { return this._cashpay; } set { this._cashpay = value; } }

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

        private System.Int32 _checkoutid;
        /// <summary>
        /// 结账ID
        /// </summary>
        public System.Int32 checkoutid { get { return this._checkoutid; } set { this._checkoutid = value; } }

        private System.Int16 _stateid;
        /// <summary>
        /// 状态ID
        /// </summary>
        public System.Int16 stateid { get { return this._stateid; } set { this._stateid = value; } }

        private System.Int32 _returnid;
        /// <summary>
        /// 退单ID
        /// </summary>
        public System.Int32 returnid { get { return this._returnid; } set { this._returnid = value; } }

        private System.String _summay;
        /// <summary>
        /// 摘要
        /// </summary>
        public System.String summay { get { return this._summay; } set { this._summay = value?.Trim(); } }

        private System.Int16? _is_register;
        /// <summary>
        /// 是否挂号
        /// </summary>
        public System.Int16? is_register { get { return this._is_register; } set { this._is_register = value; } }

        private System.Decimal _couponpay;
        /// <summary>
        /// 优惠金额支付
        /// </summary>
        public System.Decimal couponpay { get { return this._couponpay; } set { this._couponpay = value; } }
    }
}