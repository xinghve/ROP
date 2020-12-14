using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 商品出库明细
    /// </summary>
    public class bus_out_storage_detials
    {
        /// <summary>
        /// 商品出库明细
        /// </summary>
        public bus_out_storage_detials()
        {
        }

        private System.String _bill_no;
        /// <summary>
        /// 出库单号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String bill_no { get { return this._bill_no; } set { this._bill_no = value?.Trim(); } }

        private System.Int32 _storage_id;
        /// <summary>
        /// 库存id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 storage_id { get { return this._storage_id; } set { this._storage_id = value; } }

        private System.String _storage_no;
        /// <summary>
        /// 库存明细编号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String storage_no { get { return this._storage_no; } set { this._storage_no = value?.Trim(); } }

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

        private System.String _spec;
        /// <summary>
        /// 规格
        /// </summary>
        public System.String spec { get { return this._spec; } set { this._spec = value?.Trim(); } }

        private System.String _buy_unit;
        /// <summary>
        /// 采购单位
        /// </summary>
        public System.String buy_unit { get { return this._buy_unit; } set { this._buy_unit = value?.Trim(); } }

        private System.Int16? _buy_multiple;
        /// <summary>
        /// 采购单位倍率
        /// </summary>
        public System.Int16? buy_multiple { get { return this._buy_multiple; } set { this._buy_multiple = value ?? default(System.Int16); } }

        private System.Decimal? _buy_price;
        /// <summary>
        /// 采购价
        /// </summary>
        public System.Decimal? buy_price { get { return this._buy_price; } set { this._buy_price = value ?? default(System.Decimal); } }

        private System.Int32 _bill_num;
        /// <summary>
        /// 出库数量
        /// </summary>
        public System.Int32 bill_num { get { return this._bill_num; } set { this._bill_num = value; } }

        private System.String _unit;
        /// <summary>
        /// 单位
        /// </summary>
        public System.String unit { get { return this._unit; } set { this._unit = value?.Trim(); } }

        private System.Decimal _price;
        /// <summary>
        /// 单价
        /// </summary>
        public System.Decimal price { get { return this._price; } set { this._price = value; } }

        private System.String _approval_no;
        /// <summary>
        /// 批准文号
        /// </summary>
        public System.String approval_no { get { return this._approval_no; } set { this._approval_no = value?.Trim(); } }

        private System.DateTime? _expire_date;
        /// <summary>
        /// 有效期
        /// </summary>
        public System.DateTime? expire_date { get { return this._expire_date; } set { this._expire_date = value; } }

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

        private System.DateTime? _buy_date;
        /// <summary>
        /// 购入日期
        /// </summary>
        public System.DateTime? buy_date { get { return this._buy_date; } set { this._buy_date = value; } }
    }
}