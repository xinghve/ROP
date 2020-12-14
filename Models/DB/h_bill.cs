using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 
    /// </summary>
    public class h_bill
    {
        /// <summary>
        /// 
        /// </summary>
        public h_bill()
        {
        }

        private System.Int32 _bill_id;
        /// <summary>
        /// 单据ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public System.Int32 bill_id { get { return this._bill_id; } set { this._bill_id = value; } }

        private System.String _bill_name;
        /// <summary>
        /// 单据名称
        /// </summary>
        public System.String bill_name { get { return this._bill_name; } set { this._bill_name = value?.Trim(); } }

        private System.Int16 _property_id;
        /// <summary>
        /// 属性ID
        /// </summary>
        public System.Int16 property_id { get { return this._property_id; } set { this._property_id = value; } }

        private System.String _property;
        /// <summary>
        /// 属性
        /// </summary>
        public System.String property { get { return this._property; } set { this._property = value?.Trim(); } }

        private System.Int16 _state_id;
        /// <summary>
        /// 状态ID
        /// </summary>
        public System.Int16 state_id { get { return this._state_id; } set { this._state_id = value; } }

        private System.DateTime _create_date;
        /// <summary>
        /// 建档时间
        /// </summary>
        public System.DateTime create_date { get { return this._create_date; } set { this._create_date = value; } }

        private System.String _creater;
        /// <summary>
        /// 建档人
        /// </summary>
        public System.String creater { get { return this._creater; } set { this._creater = value?.Trim(); } }

        private System.Int32 _creater_id;
        /// <summary>
        /// 创建人ID
        /// </summary>
        public System.Int32 creater_id { get { return this._creater_id; } set { this._creater_id = value; } }

        private System.Int32? _org_id;
        /// <summary>
        /// 机构id
        /// </summary>
        public System.Int32? org_id { get { return this._org_id; } set { this._org_id = value ?? default(System.Int32); } }
    }
}