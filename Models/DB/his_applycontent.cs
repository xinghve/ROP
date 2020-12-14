using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 
    /// </summary>
    public class his_applycontent
    {
        /// <summary>
        /// 
        /// </summary>
        public his_applycontent()
        {
        }

        private System.Int32 _applyid;
        /// <summary>
        /// 申请ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 applyid { get { return this._applyid; } set { this._applyid = value; } }

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

        private System.String _packid;
        /// <summary>
        /// 包ID
        /// </summary>
        public System.String packid { get { return this._packid; } set { this._packid = value?.Trim(); } }

        private System.String _content;
        /// <summary>
        /// 内容
        /// </summary>
        public System.String content { get { return this._content; } set { this._content = value?.Trim(); } }

        private System.Int16 _quantity;
        /// <summary>
        /// 数量
        /// </summary>
        public System.Int16 quantity { get { return this._quantity; } set { this._quantity = value; } }

        private System.Decimal _price;
        /// <summary>
        /// 售价
        /// </summary>
        public System.Decimal price { get { return this._price; } set { this._price = value; } }

        private System.Decimal _cost;
        /// <summary>
        /// 成本价
        /// </summary>
        public System.Decimal cost { get { return this._cost; } set { this._cost = value; } }

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

        private System.Int16? _usageid;
        /// <summary>
        /// 用法ID
        /// </summary>
        public System.Int16? usageid { get { return this._usageid; } set { this._usageid = value ?? default(System.Int16); } }

        private System.String _usagename;
        /// <summary>
        /// 用法
        /// </summary>
        public System.String usagename { get { return this._usagename; } set { this._usagename = value?.Trim(); } }

        private System.Int16? _frequecyid = 1;
        /// <summary>
        /// 频率ID
        /// </summary>
        public System.Int16? frequecyid { get { return this._frequecyid; } set { this._frequecyid = value ?? default(System.Int16); } }

        private System.String _frequecyname;
        /// <summary>
        /// 频率
        /// </summary>
        public System.String frequecyname { get { return this._frequecyname; } set { this._frequecyname = value?.Trim(); } }

        private System.Decimal? _sigle = 1;
        /// <summary>
        /// 单量
        /// </summary>
        public System.Decimal? sigle { get { return this._sigle; } set { this._sigle = value ?? default(System.Decimal); } }

        private System.String _dosageunit;
        /// <summary>
        /// 剂量单位
        /// </summary>
        public System.String dosageunit { get { return this._dosageunit; } set { this._dosageunit = value?.Trim(); } }

        private System.Int32 _execdeptid;
        /// <summary>
        /// 执行科室ID
        /// </summary>
        public System.Int32 execdeptid { get { return this._execdeptid; } set { this._execdeptid = value; } }

        private System.String _execdept;
        /// <summary>
        /// 执行科室
        /// </summary>
        public System.String execdept { get { return this._execdept; } set { this._execdept = value?.Trim(); } }

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

        private System.String _unitname;
        /// <summary>
        /// 单位
        /// </summary>
        public System.String unitname { get { return this._unitname; } set { this._unitname = value?.Trim(); } }

        private System.Decimal _modulus;
        /// <summary>
        /// 系数
        /// </summary>
        public System.Decimal modulus { get { return this._modulus; } set { this._modulus = value; } }

        private System.Int32? _groupid;
        /// <summary>
        /// 组ID
        /// </summary>
        public System.Int32? groupid { get { return this._groupid; } set { this._groupid = value ?? default(System.Int32); } }

        private System.Int32? _org_id;
        /// <summary>
        /// 机构id
        /// </summary>
        public System.Int32? org_id { get { return this._org_id; } set { this._org_id = value ?? default(System.Int32); } }

        private System.Int32? _store_id;
        /// <summary>
        /// 门店id
        /// </summary>
        public System.Int32? store_id { get { return this._store_id; } set { this._store_id = value ?? default(System.Int32); } }

        private System.Int32? _type_id;
        /// <summary>
        /// 套餐类型id
        /// </summary>
        public System.Int32? type_id { get { return this._type_id; } set { this._type_id = value ?? default(System.Int32); } }

        private System.String _specname;
        /// <summary>
        /// 规格
        /// </summary>
        public System.String specname { get { return this._specname; } set { this._specname = value?.Trim(); } }

        private System.Int32? _item_id;
        /// <summary>
        /// 项目ID
        /// </summary>
        public System.Int32? item_id { get { return this._item_id; } set { this._item_id = value ?? default(System.Int32); } }

        private System.String _item_name;
        /// <summary>
        /// 项目名
        /// </summary>
        public System.String item_name { get { return this._item_name; } set { this._item_name = value?.Trim(); } }

        private System.Int16 _use_quantity;
        /// <summary>
        /// 已使用数量
        /// </summary>
        public System.Int16 use_quantity { get { return this._use_quantity; } set { this._use_quantity = value; } }
    }
}