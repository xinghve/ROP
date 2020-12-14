using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 采购单
    /// </summary>
    public class bus_buy_bill
    {
        /// <summary>
        /// 采购单
        /// </summary>
        public bus_buy_bill()
        {
        }

        private System.String _bill_no;
        /// <summary>
        /// 采购单号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String bill_no { get { return this._bill_no; } set { this._bill_no = value?.Trim(); } }

        private System.Int32? _org_id;
        /// <summary>
        /// 机构ID
        /// </summary>
        public System.Int32? org_id { get { return this._org_id; } set { this._org_id = value ?? default(System.Int32); } }

        private System.Int16? _state;
        /// <summary>
        /// 状态
        /// </summary>
        public System.Int16? state { get { return this._state; } set { this._state = value ?? default(System.Int16); } }

        private System.Int32? _manufactor_id;
        /// <summary>
        /// 厂家ID
        /// </summary>
        public System.Int32? manufactor_id { get { return this._manufactor_id; } set { this._manufactor_id = value ?? default(System.Int32); } }

        private System.String _manufactor;
        /// <summary>
        /// 厂家
        /// </summary>
        public System.String manufactor { get { return this._manufactor; } set { this._manufactor = value?.Trim(); } }

        private System.String _remark;
        /// <summary>
        /// 备注
        /// </summary>
        public System.String remark { get { return this._remark; } set { this._remark = value?.Trim(); } }

        private System.Int32? _creater_id;
        /// <summary>
        /// 创建人ID
        /// </summary>
        public System.Int32? creater_id { get { return this._creater_id; } set { this._creater_id = value ?? default(System.Int32); } }

        private System.String _creater;
        /// <summary>
        /// 创建人
        /// </summary>
        public System.String creater { get { return this._creater; } set { this._creater = value?.Trim(); } }

        private System.DateTime? _create_time;
        /// <summary>
        /// 创建时间
        /// </summary>
        public System.DateTime? create_time { get { return this._create_time; } set { this._create_time = value ?? default(System.DateTime); } }

        private System.DateTime? _buy_time;
        /// <summary>
        /// 采购时间
        /// </summary>
        public System.DateTime? buy_time { get { return this._buy_time; } set { this._buy_time = value ?? default(System.DateTime); } }

        private System.String _delete_no;
        /// <summary>
        /// 作废对应单号
        /// </summary>
        public System.String delete_no { get { return this._delete_no; } set { this._delete_no = value?.Trim(); } }
    }
}