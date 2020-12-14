using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 门诊号数表
    /// </summary>
    public class his_outpatient_number
    {
        /// <summary>
        /// 门诊号数表
        /// </summary>
        public his_outpatient_number()
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
        /// 序号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int16 no { get { return this._no; } set { this._no = value; } }

        private System.Int16? _numbers;
        /// <summary>
        /// 已挂号数
        /// </summary>
        public System.Int16? numbers { get { return this._numbers; } set { this._numbers = value ?? default(System.Int16); } }

        private System.Int16? _orders;
        /// <summary>
        /// 已预约数
        /// </summary>
        public System.Int16? orders { get { return this._orders; } set { this._orders = value ?? default(System.Int16); } }
    }
}