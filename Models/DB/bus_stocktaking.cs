using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 库存盘点
    /// </summary>
    public class bus_stocktaking
    {
        /// <summary>
        /// 库存盘点
        /// </summary>
        public bus_stocktaking()
        {
        }

        private System.String _no;
        /// <summary>
        /// 单号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String no { get { return this._no; } set { this._no = value?.Trim(); } }

        private System.Int32 _org_id;
        /// <summary>
        /// 机构ID
        /// </summary>
        public System.Int32 org_id { get { return this._org_id; } set { this._org_id = value; } }

        private System.Int32 _store_id;
        /// <summary>
        /// 门店ID
        /// </summary>
        public System.Int32 store_id { get { return this._store_id; } set { this._store_id = value; } }

        private System.String _store_name;
        /// <summary>
        /// 门店名称
        /// </summary>
        public System.String store_name { get { return this._store_name; } set { this._store_name = value?.Trim(); } }

        private System.DateTime? _date;
        /// <summary>
        /// 日期
        /// </summary>
        public System.DateTime? date { get { return this._date; } set { this._date = value ?? default(System.DateTime); } }

        private System.Int16? _state;
        /// <summary>
        /// 状态
        /// </summary>
        public System.Int16? state { get { return this._state; } set { this._state = value ?? default(System.Int16); } }

        private System.Int32? _responsible_employee_id;
        /// <summary>
        /// 负责人ID
        /// </summary>
        public System.Int32? responsible_employee_id { get { return this._responsible_employee_id; } set { this._responsible_employee_id = value ?? default(System.Int32); } }

        private System.String _responsible_employee;
        /// <summary>
        /// 负责人
        /// </summary>
        public System.String responsible_employee { get { return this._responsible_employee; } set { this._responsible_employee = value?.Trim(); } }

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
    }
}