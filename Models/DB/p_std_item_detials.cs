using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 物资基础项目明细
    /// </summary>
    public class p_std_item_detials
    {
        /// <summary>
        /// 物资基础项目明细
        /// </summary>
        public p_std_item_detials()
        {
        }

        private System.Int32 _id;
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 id { get { return this._id; } set { this._id = value; } }

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

        private System.String _spec;
        /// <summary>
        /// 规格
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String spec { get { return this._spec; } set { this._spec = value?.Trim(); } }

        private System.Decimal _sale_price;
        /// <summary>
        /// 单价
        /// </summary>
        public System.Decimal price { get { return this._sale_price; } set { this._sale_price = value; } }

        private System.String _buy_unit;
        /// <summary>
        /// 采购单位
        /// </summary>
        public System.String buy_unit { get { return this._buy_unit; } set { this._buy_unit = value?.Trim(); } }

        private System.Int16 _buy_multiple;
        /// <summary>
        /// 采购单位倍率
        /// </summary>
        public System.Int16 buy_multiple { get { return this._buy_multiple; } set { this._buy_multiple = value; } }

        private System.Decimal _buy_price;
        /// <summary>
        /// 采购价
        /// </summary>
        public System.Decimal buy_price { get { return this._buy_price; } set { this._buy_price = value; } }

        private System.String _approval_no;
        /// <summary>
        /// 批准文号
        /// </summary>
        public System.String approval_no { get { return this._approval_no; } set { this._approval_no = value?.Trim(); } }
    }
}