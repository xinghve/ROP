using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 采购单对应申请单明细
    /// </summary>
    public class bus_buy_bill_to_apply_detials
    {
        /// <summary>
        /// 采购单对应申请单明细
        /// </summary>
        public bus_buy_bill_to_apply_detials()
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

        private System.String _apply_no;
        /// <summary>
        /// 申请单号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String apply_no { get { return this._apply_no; } set { this._apply_no = value?.Trim(); } }

        private System.Int16? _num;
        /// <summary>
        /// 数量
        /// </summary>
        public System.Int16? num { get { return this._num; } set { this._num = value ?? default(System.Int16); } }

        private System.String _no;
        /// <summary>
        /// 单号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String no { get { return this._no; } set { this._no = value?.Trim(); } }

        private System.String _spec;
        /// <summary>
        /// 规格
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String spec { get { return this._spec; } set { this._spec = value?.Trim(); } }
    }
}