using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 库存盘点明细
    /// </summary>
    public class bus_stocktaking_detials
    {
        /// <summary>
        /// 库存盘点明细
        /// </summary>
        public bus_stocktaking_detials()
        {
        }

        private System.String _no;
        /// <summary>
        /// 单号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String no { get { return this._no; } set { this._no = value?.Trim(); } }

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
        [SugarColumn(IsPrimaryKey = true)]
        public System.String spec { get { return this._spec; } set { this._spec = value?.Trim(); } }

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

        private System.String _unit;
        /// <summary>
        /// 单位
        /// </summary>
        public System.String unit { get { return this._unit; } set { this._unit = value?.Trim(); } }

        private System.Int32 _num;
        /// <summary>
        /// 数量
        /// </summary>
        public System.Int32 num { get { return this._num; } set { this._num = value; } }

        private System.Int32? _stocktaking_num;
        /// <summary>
        /// 盘点数量
        /// </summary>
        public System.Int32? stocktaking_num { get { return this._stocktaking_num; } set { this._stocktaking_num = value ?? default(System.Int32); } }

        private System.Int32? _num_difference;
        /// <summary>
        /// 数量差
        /// </summary>
        public System.Int32? num_difference { get { return this._num_difference; } set { this._num_difference = value ?? default(System.Int32); } }

        private System.Int32? _report_num;
        /// <summary>
        /// 已报数量
        /// </summary>
        public System.Int32? report_num { get { return this._report_num; } set { this._report_num = value ?? default(System.Int32); } }

        private System.Int32? _not_report_num;
        /// <summary>
        /// 待报数量
        /// </summary>
        public System.Int32? not_report_num { get { return this._not_report_num; } set { this._not_report_num = value ?? default(System.Int32); } }
    }
}