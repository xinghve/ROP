using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 
    /// </summary>
    public class bus_buy_apply_detials
    {
        /// <summary>
        /// 
        /// </summary>
        public bus_buy_apply_detials()
        {
        }

        private System.String _apply_no;
        /// <summary>
        /// 申请单号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String apply_no { get { return this._apply_no; } set { this._apply_no = value?.Trim(); } }

        private System.Int32 _std_item_id;
        /// <summary>
        /// 物资基础项目ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 std_item_id { get { return this._std_item_id; } set { this._std_item_id = value; } }

        private System.Int32 _manufactor_id;
        /// <summary>
        /// 厂家ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 manufactor_id { get { return this._manufactor_id; } set { this._manufactor_id = value; } }

        private System.String _manufactor;
        /// <summary>
        /// 厂家
        /// </summary>
        public System.String manufactor { get { return this._manufactor; } set { this._manufactor = value?.Trim(); } }

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

        private System.Int16? _state;
        /// <summary>
        /// 状态
        /// </summary>
        public System.Int16? state { get { return this._state; } set { this._state = value ?? default(System.Int16); } }

        private System.Int16? _dispose_num;
        /// <summary>
        /// 处理数量
        /// </summary>
        public System.Int16? dispose_num { get { return this._dispose_num; } set { this._dispose_num = value ?? default(System.Int16); } }

        private System.Int16? _buy_num;
        /// <summary>
        /// 采购数量
        /// </summary>
        public System.Int16? buy_num { get { return this._buy_num; } set { this._buy_num = value ?? default(System.Int16); } }

        private System.Decimal? _transfer_num;
        /// <summary>
        /// 调拨数量
        /// </summary>
        public System.Decimal? transfer_num { get { return this._transfer_num; } set { this._transfer_num = value ?? default(System.Decimal); } }

        private System.String _remark;
        /// <summary>
        /// 备注
        /// </summary>
        public System.String remark { get { return this._remark; } set { this._remark = value?.Trim(); } }

        private System.Decimal? _price;
        /// <summary>
        /// 单价
        /// </summary>
        public System.Decimal? price { get { return this._price; } set { this._price = value ?? default(System.Decimal); } }

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

        private System.String _spec;
        /// <summary>
        /// 规格
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String spec { get { return this._spec; } set { this._spec = value?.Trim(); } }
    }
}