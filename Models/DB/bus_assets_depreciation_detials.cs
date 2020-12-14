using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 固定资产折旧明细
    /// </summary>
    public class bus_assets_depreciation_detials
    {
        /// <summary>
        /// 固定资产折旧明细
        /// </summary>
        public bus_assets_depreciation_detials()
        {
        }

        private System.String _no;
        /// <summary>
        /// 单号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String no { get { return this._no; } set { this._no = value?.Trim(); } }

        private System.Int32 _assets_id;
        /// <summary>
        /// 固定资产ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 assets_id { get { return this._assets_id; } set { this._assets_id = value; } }

        private System.String _assets_no;
        /// <summary>
        /// 固定资产编号
        /// </summary>
        public System.String assets_no { get { return this._assets_no; } set { this._assets_no = value?.Trim(); } }

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

        private System.Decimal _depreciation;
        /// <summary>
        /// 折旧额
        /// </summary>
        public System.Decimal depreciation { get { return this._depreciation; } set { this._depreciation = value; } }

        private System.Decimal? _buy_price;
        /// <summary>
        /// 原价
        /// </summary>
        public System.Decimal? buy_price { get { return this._buy_price; } set { this._buy_price = value ?? default(System.Decimal); } }

        private System.Decimal _total_depreciation;
        /// <summary>
        /// 累计折旧额
        /// </summary>
        public System.Decimal total_depreciation { get { return this._total_depreciation; } set { this._total_depreciation = value; } }

        private System.Decimal _remaining_depreciation;
        /// <summary>
        /// 剩余折旧额
        /// </summary>
        public System.Decimal remaining_depreciation { get { return this._remaining_depreciation; } set { this._remaining_depreciation = value; } }

        private System.Decimal _net_residual;
        /// <summary>
        /// 净剩值
        /// </summary>
        public System.Decimal net_residual { get { return this._net_residual; } set { this._net_residual = value; } }
    }
}