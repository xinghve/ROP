using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 
    /// </summary>
    public class bus_assets
    {
        /// <summary>
        /// 
        /// </summary>
        public bus_assets()
        {
        }

        private System.Int32 _id;
        /// <summary>
        /// 主键ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public System.Int32 id { get { return this._id; } set { this._id = value; } }

        private System.Int32 _org_id;
        /// <summary>
        /// 机构ID
        /// </summary>
        public System.Int32 org_id { get { return this._org_id; } set { this._org_id = value; } }

        private System.Int32? _storage_id;
        /// <summary>
        /// 库存id
        /// </summary>
        public System.Int32? storage_id { get { return this._storage_id; } set { this._storage_id = value ?? default(System.Int32); } }

        private System.String _storage_no;
        /// <summary>
        /// 库存明细编号
        /// </summary>
        public System.String storage_no { get { return this._storage_no; } set { this._storage_no = value?.Trim(); } }

        private System.String _no;
        /// <summary>
        /// 编号
        /// </summary>
        public System.String no { get { return this._no; } set { this._no = value?.Trim(); } }

        private System.Int32 _std_item_id;
        /// <summary>
        /// 物资基础项目ID
        /// </summary>
        public System.Int32 std_item_id { get { return this._std_item_id; } set { this._std_item_id = value; } }

        private System.String _name;
        /// <summary>
        /// 名称
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.String _spec;
        /// <summary>
        /// 规格
        /// </summary>
        public System.String spec { get { return this._spec; } set { this._spec = value?.Trim(); } }

        private System.Int16 _state;
        /// <summary>
        /// 状态
        /// </summary>
        public System.Int16 state { get { return this._state; } set { this._state = value; } }

        private System.Int16 _type_id;
        /// <summary>
        /// 类型ID
        /// </summary>
        public System.Int16 type_id { get { return this._type_id; } set { this._type_id = value; } }

        private System.String _type;
        /// <summary>
        /// 类型
        /// </summary>
        public System.String type { get { return this._type; } set { this._type = value?.Trim(); } }

        private System.Int32 _manufactor_id;
        /// <summary>
        /// 厂家ID
        /// </summary>
        public System.Int32 manufactor_id { get { return this._manufactor_id; } set { this._manufactor_id = value; } }

        private System.String _manufactor;
        /// <summary>
        /// 厂家
        /// </summary>
        public System.String manufactor { get { return this._manufactor; } set { this._manufactor = value?.Trim(); } }

        private System.String _unit;
        /// <summary>
        /// 单位
        /// </summary>
        public System.String unit { get { return this._unit; } set { this._unit = value?.Trim(); } }

        private System.Decimal? _buy_price;
        /// <summary>
        /// 原价
        /// </summary>
        public System.Decimal? buy_price { get { return this._buy_price; } set { this._buy_price = value ?? default(System.Decimal); } }

        private System.DateTime _buy_date;
        /// <summary>
        /// 购置日期
        /// </summary>
        public System.DateTime buy_date { get { return this._buy_date; } set { this._buy_date = value; } }

        private System.Decimal? _price;
        /// <summary>
        /// 价格
        /// </summary>
        public System.Decimal? price { get { return this._price; } set { this._price = value ?? default(System.Decimal); } }

        private System.Int16? _year_num;
        /// <summary>
        /// 使用年限
        /// </summary>
        public System.Int16? year_num { get { return this._year_num; } set { this._year_num = value ?? default(System.Int16); } }

        private System.Int32? _store_id;
        /// <summary>
        /// 使用门店ID
        /// </summary>
        public System.Int32? store_id { get { return this._store_id; } set { this._store_id = value ?? default(System.Int32); } }

        private System.String _store;
        /// <summary>
        /// 使用门店
        /// </summary>
        public System.String store { get { return this._store; } set { this._store = value?.Trim(); } }

        private System.Int32? _dept_id;
        /// <summary>
        /// 使用部门ID
        /// </summary>
        public System.Int32? dept_id { get { return this._dept_id; } set { this._dept_id = value ?? default(System.Int32); } }

        private System.String _dept;
        /// <summary>
        /// 使用部门
        /// </summary>
        public System.String dept { get { return this._dept; } set { this._dept = value?.Trim(); } }

        private System.Int32? _use_employee_id;
        /// <summary>
        /// 使用人ID
        /// </summary>
        public System.Int32? use_employee_id { get { return this._use_employee_id; } set { this._use_employee_id = value ?? default(System.Int32); } }

        private System.String _use_employee;
        /// <summary>
        /// 使用人
        /// </summary>
        public System.String use_employee { get { return this._use_employee; } set { this._use_employee = value?.Trim(); } }

        private System.String _address;
        /// <summary>
        /// 位置
        /// </summary>
        public System.String address { get { return this._address; } set { this._address = value?.Trim(); } }

        private System.Int32? _responsible_employee_id;
        /// <summary>
        /// 负责人ID
        /// </summary>
        public System.Int32? responsible_employee_id { get { return this._responsible_employee_id; } set { this._responsible_employee_id = value ?? default(System.Int32); } }

        private System.String _responsible_employee;
        /// <summary>
        /// 负责人
        /// </summary>
        public System.String responsible_employee { get { return this._responsible_employee; } set { this._responsible_employee = value?.Trim(); } }

        private System.Decimal _net_salvage_rate;
        /// <summary>
        /// 净残值率
        /// </summary>
        public System.Decimal net_salvage_rate { get { return this._net_salvage_rate; } set { this._net_salvage_rate = value; } }

        private System.String _bill_no;
        /// <summary>
        /// 入库单号
        /// </summary>
        public System.String bill_no { get { return this._bill_no; } set { this._bill_no = value?.Trim(); } }

        private System.Decimal _net_salvage;
        /// <summary>
        /// 净残值
        /// </summary>
        public System.Decimal net_salvage { get { return this._net_salvage; } set { this._net_salvage = value; } }

        private System.Decimal _total_depreciation;
        /// <summary>
        /// 累计折旧额
        /// </summary>
        public System.Decimal total_depreciation { get { return this._total_depreciation; } set { this._total_depreciation = value; } }

        private System.Decimal _depreciation;
        /// <summary>
        /// 可提折旧额
        /// </summary>
        public System.Decimal depreciation { get { return this._depreciation; } set { this._depreciation = value; } }

        private System.Decimal _remaining_depreciation;
        /// <summary>
        /// 剩余折旧额
        /// </summary>
        public System.Decimal remaining_depreciation { get { return this._remaining_depreciation; } set { this._remaining_depreciation = value; } }

        private System.Decimal _month_depreciation;
        /// <summary>
        /// 每月折旧额
        /// </summary>
        public System.Decimal month_depreciation { get { return this._month_depreciation; } set { this._month_depreciation = value; } }

        private System.Decimal _net_residual;
        /// <summary>
        /// 净剩值
        /// </summary>
        public System.Decimal net_residual { get { return this._net_residual; } set { this._net_residual = value; } }
    }
}