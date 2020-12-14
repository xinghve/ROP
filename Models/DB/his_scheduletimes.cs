using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 排班时间段
    /// </summary>
    public class his_scheduletimes
    {
        /// <summary>
        /// 排班时间段
        /// </summary>
        public his_scheduletimes()
        {
        }

        private System.Int32 _scheduleid;
        /// <summary>
        /// 排班ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 scheduleid { get { return this._scheduleid; } set { this._scheduleid = value; } }

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

        private System.TimeSpan _begintime;
        /// <summary>
        /// 开始时间
        /// </summary>
        public System.TimeSpan begintime { get { return this._begintime; } set { this._begintime = value; } }

        private System.TimeSpan _endtime;
        /// <summary>
        /// 结束时间
        /// </summary>
        public System.TimeSpan endtime { get { return this._endtime; } set { this._endtime = value; } }

        private System.Int16 _stateid;
        /// <summary>
        /// 状态ID
        /// </summary>
        public System.Int16 stateid { get { return this._stateid; } set { this._stateid = value; } }

        private System.String _statename;
        /// <summary>
        /// 状态
        /// </summary>
        public System.String statename { get { return this._statename; } set { this._statename = value?.Trim(); } }

        private System.Int32? _replaceid;
        /// <summary>
        /// 替诊ID
        /// </summary>
        public System.Int32? replaceid { get { return this._replaceid; } set { this._replaceid = value ?? default(System.Int32); } }

        private System.Int32? _replacedocid;
        /// <summary>
        /// 替诊医生ID
        /// </summary>
        public System.Int32? replacedocid { get { return this._replacedocid; } set { this._replacedocid = value ?? default(System.Int32); } }

        private System.String _replacedocname;
        /// <summary>
        /// 替诊医生
        /// </summary>
        public System.String replacedocname { get { return this._replacedocname; } set { this._replacedocname = value?.Trim(); } }
    }
}