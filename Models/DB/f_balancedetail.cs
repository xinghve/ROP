using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 门诊收费清单
    /// </summary>
    public class f_balancedetail
    {
        /// <summary>
        /// 门诊收费清单
        /// </summary>
        public f_balancedetail()
        {
        }

        private System.Int32 _balanceid;
        /// <summary>
        /// 结算ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 balanceid { get { return this._balanceid; } set { this._balanceid = value; } }

        private System.Int16 _orderid;
        /// <summary>
        /// 序号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int16 orderid { get { return this._orderid; } set { this._orderid = value; } }

        private System.Int32 _specid;
        /// <summary>
        /// 规格ID
        /// </summary>
        public System.Int32 specid { get { return this._specid; } set { this._specid = value; } }

        private System.String _itemname;
        /// <summary>
        /// 项目名称
        /// </summary>
        public System.String itemname { get { return this._itemname; } set { this._itemname = value?.Trim(); } }

        private System.String _specname;
        /// <summary>
        /// 规格
        /// </summary>
        public System.String specname { get { return this._specname; } set { this._specname = value?.Trim(); } }

        private System.Decimal _quantity;
        /// <summary>
        /// 数量
        /// </summary>
        public System.Decimal quantity { get { return this._quantity; } set { this._quantity = value; } }

        private System.Int16 _numbers;
        /// <summary>
        /// 付数
        /// </summary>
        public System.Int16 numbers { get { return this._numbers; } set { this._numbers = value; } }

        private System.Decimal _modulus;
        /// <summary>
        /// 系数
        /// </summary>
        public System.Decimal modulus { get { return this._modulus; } set { this._modulus = value; } }

        private System.String _unitname;
        /// <summary>
        /// 单位
        /// </summary>
        public System.String unitname { get { return this._unitname; } set { this._unitname = value?.Trim(); } }

        private System.Decimal _price;
        /// <summary>
        /// 零售价
        /// </summary>
        public System.Decimal price { get { return this._price; } set { this._price = value; } }

        private System.Decimal _cost;
        /// <summary>
        /// 成本价
        /// </summary>
        public System.Decimal cost { get { return this._cost; } set { this._cost = value; } }

        private System.Decimal _rebate;
        /// <summary>
        /// 折扣比
        /// </summary>
        public System.Decimal rebate { get { return this._rebate; } set { this._rebate = value; } }

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

        private System.Int32 _incomeid;
        /// <summary>
        /// 收入ID
        /// </summary>
        public System.Int32 incomeid { get { return this._incomeid; } set { this._incomeid = value; } }

        private System.String _invoicename;
        /// <summary>
        /// 票据项目
        /// </summary>
        public System.String invoicename { get { return this._invoicename; } set { this._invoicename = value?.Trim(); } }

        private System.Int32 _execdeptid;
        /// <summary>
        /// 执行科室ID
        /// </summary>
        public System.Int32 execdeptid { get { return this._execdeptid; } set { this._execdeptid = value; } }

        private System.String _execdeptname;
        /// <summary>
        /// 执行科室
        /// </summary>
        public System.String execdeptname { get { return this._execdeptname; } set { this._execdeptname = value?.Trim(); } }

        private System.Int16 _execstateid;
        /// <summary>
        /// 执行状态ID
        /// </summary>
        public System.Int16 execstateid { get { return this._execstateid; } set { this._execstateid = value; } }

        private System.String _execstate;
        /// <summary>
        /// 执行状态
        /// </summary>
        public System.String execstate { get { return this._execstate; } set { this._execstate = value?.Trim(); } }

        private System.Int32 _usageid;
        /// <summary>
        /// 给药方法ID
        /// </summary>
        public System.Int32 usageid { get { return this._usageid; } set { this._usageid = value; } }

        private System.String _usage;
        /// <summary>
        /// 给药方法
        /// </summary>
        public System.String usage { get { return this._usage; } set { this._usage = value?.Trim(); } }

        private System.Int32? _frequencyid;
        /// <summary>
        /// 频率ID
        /// </summary>
        public System.Int32? frequencyid { get { return this._frequencyid; } set { this._frequencyid = value ?? default(System.Int32); } }

        private System.String _frequency;
        /// <summary>
        /// 频率
        /// </summary>
        public System.String frequency { get { return this._frequency; } set { this._frequency = value?.Trim(); } }

        private System.Decimal? _sigledousage;
        /// <summary>
        /// 单量
        /// </summary>
        public System.Decimal? sigledousage { get { return this._sigledousage; } set { this._sigledousage = value ?? default(System.Decimal); } }

        private System.Int32 _receipeid;
        /// <summary>
        /// 处方ID
        /// </summary>
        public System.Int32 receipeid { get { return this._receipeid; } set { this._receipeid = value; } }
    }
}