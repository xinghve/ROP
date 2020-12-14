using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 调拨单明细对应库存明细
    /// </summary>
    public class bus_transfer_detials_storage
    {
        /// <summary>
        /// 调拨单明细对应库存明细
        /// </summary>
        public bus_transfer_detials_storage()
        {
        }

        private System.String _bill_no;
        /// <summary>
        /// 调拨单号
        /// </summary>
        public System.String bill_no { get { return this._bill_no; } set { this._bill_no = value?.Trim(); } }

        private System.Int32 _id;
        /// <summary>
        /// 库存id
        /// </summary>
        public System.Int32 id { get { return this._id; } set { this._id = value; } }

        private System.String _no;
        /// <summary>
        /// 库存明细编号
        /// </summary>
        public System.String no { get { return this._no; } set { this._no = value?.Trim(); } }

        private System.Int32 _std_item_id;
        /// <summary>
        /// 物资基础项目ID
        /// </summary>
        public System.Int32 std_item_id { get { return this._std_item_id; } set { this._std_item_id = value; } }

        private System.Int16 _num;
        /// <summary>
        /// 数量
        /// </summary>
        public System.Int16 num { get { return this._num; } set { this._num = value; } }

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

        private System.DateTime? _expire_date;
        /// <summary>
        /// 有效期
        /// </summary>
        public System.DateTime? expire_date { get { return this._expire_date; } set { this._expire_date = value; } }

        private System.DateTime? _buy_date;
        /// <summary>
        /// 购入日期
        /// </summary>
        public System.DateTime? buy_date { get { return this._buy_date; } set { this._buy_date = value; } }
    }
}