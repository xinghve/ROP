using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 规格
    /// </summary>
    public class h_itemspec
    {
        /// <summary>
        /// 规格
        /// </summary>
        public h_itemspec()
        {
        }

        private System.Int32 _specid;
        /// <summary>
        /// 规格ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public System.Int32 specid { get { return this._specid; } set { this._specid = value; } }

        private System.Int32 _itemid;
        /// <summary>
        /// 项目ID
        /// </summary>
        public System.Int32 itemid { get { return this._itemid; } set { this._itemid = value; } }

        private System.String _specname;
        /// <summary>
        /// 规格
        /// </summary>
        public System.String specname { get { return this._specname; } set { this._specname = value?.Trim(); } }

        private System.Int32? _factoryid;
        /// <summary>
        /// 厂家ID
        /// </summary>
        public System.Int32? factoryid { get { return this._factoryid; } set { this._factoryid = value ?? default(System.Int32); } }

        private System.String _factoryname;
        /// <summary>
        /// 厂家
        /// </summary>
        public System.String factoryname { get { return this._factoryname; } set { this._factoryname = value?.Trim(); } }

        private System.String _produced;
        /// <summary>
        /// 产地
        /// </summary>
        public System.String produced { get { return this._produced; } set { this._produced = value?.Trim(); } }

        private System.String _lotno;
        /// <summary>
        /// 批号
        /// </summary>
        public System.String lotno { get { return this._lotno; } set { this._lotno = value?.Trim(); } }

        private System.String _approvalno;
        /// <summary>
        /// 批准文号
        /// </summary>
        public System.String approvalno { get { return this._approvalno; } set { this._approvalno = value?.Trim(); } }

        private System.DateTime? _expiredate;
        /// <summary>
        /// 有效期
        /// </summary>
        public System.DateTime? expiredate { get { return this._expiredate; } set { this._expiredate = value ?? default(System.DateTime); } }

        private System.Decimal _mindosage;
        /// <summary>
        /// 最小剂量
        /// </summary>
        public System.Decimal mindosage { get { return this._mindosage; } set { this._mindosage = value; } }

        private System.String _dosageunit;
        /// <summary>
        /// 剂量单位
        /// </summary>
        public System.String dosageunit { get { return this._dosageunit; } set { this._dosageunit = value?.Trim(); } }

        private System.String _minunit;
        /// <summary>
        /// 最小单位
        /// </summary>
        public System.String minunit { get { return this._minunit; } set { this._minunit = value?.Trim(); } }

        private System.Int16 _stateid;
        /// <summary>
        /// 状态ID
        /// </summary>
        public System.Int16 stateid { get { return this._stateid; } set { this._stateid = value; } }

        private System.String _hint;
        /// <summary>
        /// 开方提示
        /// </summary>
        public System.String hint { get { return this._hint; } set { this._hint = value?.Trim(); } }

        private System.String _standardcode;
        /// <summary>
        /// 本位码
        /// </summary>
        public System.String standardcode { get { return this._standardcode; } set { this._standardcode = value?.Trim(); } }

        private System.String _feegrade;
        /// <summary>
        /// 费用等级
        /// </summary>
        public System.String feegrade { get { return this._feegrade; } set { this._feegrade = value?.Trim(); } }

        private System.String _dddunit;
        /// <summary>
        /// DDD单位
        /// </summary>
        public System.String dddunit { get { return this._dddunit; } set { this._dddunit = value?.Trim(); } }

        private System.Decimal? _dddvalue;
        /// <summary>
        /// DDD值
        /// </summary>
        public System.Decimal? dddvalue { get { return this._dddvalue; } set { this._dddvalue = value ?? default(System.Decimal); } }

        private System.Int16 _incomeid;
        /// <summary>
        /// 收入ID
        /// </summary>
        public System.Int16 incomeid { get { return this._incomeid; } set { this._incomeid = value; } }

        private System.String _salseunit;
        /// <summary>
        /// 售价单位
        /// </summary>
        public System.String salseunit { get { return this._salseunit; } set { this._salseunit = value?.Trim(); } }

        private System.Decimal _salsemodulus;
        /// <summary>
        /// 售价系数
        /// </summary>
        public System.Decimal salsemodulus { get { return this._salsemodulus; } set { this._salsemodulus = value; } }

        private System.String _packunit;
        /// <summary>
        /// 包装单位
        /// </summary>
        public System.String packunit { get { return this._packunit; } set { this._packunit = value?.Trim(); } }

        private System.Decimal _packmodulus;
        /// <summary>
        /// 包装系数
        /// </summary>
        public System.Decimal packmodulus { get { return this._packmodulus; } set { this._packmodulus = value; } }

        private System.Int32? _billid;
        /// <summary>
        /// 单据ID
        /// </summary>
        public System.Int32? billid { get { return this._billid; } set { this._billid = value ?? default(System.Int32); } }

        private System.Decimal _buy_price;
        /// <summary>
        /// 进价
        /// </summary>
        public System.Decimal buy_price { get { return this._buy_price; } set { this._buy_price = value; } }

        private System.Decimal _sale_price;
        /// <summary>
        /// 售价
        /// </summary>
        public System.Decimal sale_price { get { return this._sale_price; } set { this._sale_price = value; } }
    }
}