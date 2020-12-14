using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 门诊号表
    /// </summary>
    public class his_outpatient_no
    {
        /// <summary>
        /// 门诊号表
        /// </summary>
        public his_outpatient_no()
        {
        }

        private System.Int32 _scheduleid;
        /// <summary>
        /// 排班ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 scheduleid { get { return this._scheduleid; } set { this._scheduleid = value; } }

        private System.DateTime _register_date;
        /// <summary>
        /// 挂号日期
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.DateTime register_date { get { return this._register_date; } set { this._register_date = value; } }

        private System.Int16 _days;
        /// <summary>
        /// 星期数
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int16 days { get { return this._days; } set { this._days = value; } }

        private System.Int16 _no;
        /// <summary>
        /// 排班时间段序号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int16 no { get { return this._no; } set { this._no = value; } }

        private System.Int32 _regid;
        /// <summary>
        /// 挂号ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 regid { get { return this._regid; } set { this._regid = value; } }

        private System.Int16 _order_no;
        /// <summary>
        /// 就诊号序
        /// </summary>
        public System.Int16 order_no { get { return this._order_no; } set { this._order_no = value; } }

        private System.DateTime? _order_time;
        /// <summary>
        /// 排号时间
        /// </summary>
        public System.DateTime? order_time { get { return this._order_time; } set { this._order_time = value ?? default(System.DateTime); } }
    }
}