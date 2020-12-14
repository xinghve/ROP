using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 固定资产出库
    /// </summary>
    public class bus_out_assets
    {
        /// <summary>
        /// 固定资产出库
        /// </summary>
        public bus_out_assets()
        {
        }

        private System.String _bill_no;
        /// <summary>
        /// 出库单号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String bill_no { get { return this._bill_no; } set { this._bill_no = value?.Trim(); } }

        private System.Int32 _assets_id;
        /// <summary>
        /// 固定资产ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 assets_id { get { return this._assets_id; } set { this._assets_id = value; } }

        private System.Int32 _std_item_id;
        /// <summary>
        /// 物资基础项目ID
        /// </summary>
        public System.Int32 std_item_id { get { return this._std_item_id; } set { this._std_item_id = value; } }

        private System.String _spec;
        /// <summary>
        /// 规格
        /// </summary>
        public System.String spec { get { return this._spec; } set { this._spec = value?.Trim(); } }

        private System.Int32 _manufactor_id;
        /// <summary>
        /// 厂家ID
        /// </summary>
        public System.Int32 manufactor_id { get { return this._manufactor_id; } set { this._manufactor_id = value; } }
    }
}