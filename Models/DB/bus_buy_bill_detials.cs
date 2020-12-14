using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 采购单明细
    /// </summary>
    public class bus_buy_bill_detials
    {
        /// <summary>
        /// 采购单明细
        /// </summary>
        public bus_buy_bill_detials()
        {
        }

        private System.String _bill_no;
        /// <summary>
        /// 采购单号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String bill_no { get { return this._bill_no; } set { this._bill_no = value?.Trim(); } }

        private System.Int32 _std_item_id;
        /// <summary>
        /// 物资基础项目ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 std_item_id { get { return this._std_item_id; } set { this._std_item_id = value; } }

        private System.String _name;
        /// <summary>
        /// 名称
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.Int16? _num;
        /// <summary>
        /// 数量
        /// </summary>
        public System.Int16? num { get { return this._num; } set { this._num = value ?? default(System.Int16); } }

        private System.String _unit;
        /// <summary>
        /// 单位
        /// </summary>
        public System.String unit { get { return this._unit; } set { this._unit = value?.Trim(); } }

        private System.Decimal? _aog_num;
        /// <summary>
        /// 到货数量
        /// </summary>
        public System.Decimal? aog_num { get { return this._aog_num; } set { this._aog_num = value ?? default(System.Int16); } }

        private System.Decimal? _price;
        /// <summary>
        /// 单价
        /// </summary>
        public System.Decimal? price { get { return this._price; } set { this._price = value ?? default(System.Decimal); } }

        private System.Decimal? _total_price;
        /// <summary>
        /// 总价
        /// </summary>
        public System.Decimal? total_price { get { return this._total_price; } set { this._total_price = value ?? default(System.Decimal); } }

        private System.String _remark;
        /// <summary>
        /// 备注
        /// </summary>
        public System.String remark { get { return this._remark; } set { this._remark = value?.Trim(); } }

        private System.String _spec;
        /// <summary>
        /// 规格
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
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

        private System.String _approval_no;
        /// <summary>
        /// 批准文号
        /// </summary>
        public System.String approval_no { get { return this._approval_no; } set { this._approval_no = value?.Trim(); } }
    }
}