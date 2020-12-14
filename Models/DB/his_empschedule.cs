using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 人员排班表
    /// </summary>
    public class his_empschedule
    {
        /// <summary>
        /// 人员排班表
        /// </summary>
        public his_empschedule()
        {
        }

        private System.Int32 _scheduleid;
        /// <summary>
        /// 排班ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public System.Int32 scheduleid { get { return this._scheduleid; } set { this._scheduleid = value; } }

        private System.Int32 _orgid;
        /// <summary>
        /// 机构ID
        /// </summary>
        public System.Int32 orgid { get { return this._orgid; } set { this._orgid = value; } }

        private System.Int32 _store_id;
        /// <summary>
        /// 门店ID
        /// </summary>
        public System.Int32 store_id { get { return this._store_id; } set { this._store_id = value; } }

        private System.Int32 _typeid;
        /// <summary>
        /// 类别ID
        /// </summary>
        public System.Int32 typeid { get { return this._typeid; } set { this._typeid = value; } }

        private System.Int32 _deptid;
        /// <summary>
        /// 部门ID
        /// </summary>
        public System.Int32 deptid { get { return this._deptid; } set { this._deptid = value; } }

        private System.String _deptname;
        /// <summary>
        /// 部门名称
        /// </summary>
        public System.String deptname { get { return this._deptname; } set { this._deptname = value?.Trim(); } }

        private System.Int32 _empid;
        /// <summary>
        /// 职工ID
        /// </summary>
        public System.Int32 empid { get { return this._empid; } set { this._empid = value; } }

        private System.String _empname;
        /// <summary>
        /// 职工名称
        /// </summary>
        public System.String empname { get { return this._empname; } set { this._empname = value?.Trim(); } }

        private System.DateTime _orderbegin;
        /// <summary>
        /// 预约开始时间
        /// </summary>
        public System.DateTime orderbegin { get { return this._orderbegin; } set { this._orderbegin = value; } }

        private System.DateTime _orderend;
        /// <summary>
        /// 预约结束时间
        /// </summary>
        public System.DateTime orderend { get { return this._orderend; } set { this._orderend = value; } }

        private System.Int16 _orderflag;
        /// <summary>
        /// 预约标志
        /// </summary>
        public System.Int16 orderflag { get { return this._orderflag; } set { this._orderflag = value; } }

        private System.Int16 _limitnumbers;
        /// <summary>
        /// 限号数
        /// </summary>
        public System.Int16 limitnumbers { get { return this._limitnumbers; } set { this._limitnumbers = value; } }

        private System.Int16 _limitorders;
        /// <summary>
        /// 限约数
        /// </summary>
        public System.Int16 limitorders { get { return this._limitorders; } set { this._limitorders = value; } }

        private System.String _monday;
        /// <summary>
        /// 星期一
        /// </summary>
        public System.String monday { get { return this._monday; } set { this._monday = value?.Trim(); } }

        private System.String _tuesday;
        /// <summary>
        /// 星期二
        /// </summary>
        public System.String tuesday { get { return this._tuesday; } set { this._tuesday = value?.Trim(); } }

        private System.String _wednesday;
        /// <summary>
        /// 星期三
        /// </summary>
        public System.String wednesday { get { return this._wednesday; } set { this._wednesday = value?.Trim(); } }

        private System.String _thursday;
        /// <summary>
        /// 星期四
        /// </summary>
        public System.String thursday { get { return this._thursday; } set { this._thursday = value?.Trim(); } }

        private System.String _friday;
        /// <summary>
        /// 星期五
        /// </summary>
        public System.String friday { get { return this._friday; } set { this._friday = value?.Trim(); } }

        private System.String _saturday;
        /// <summary>
        /// 星期六
        /// </summary>
        public System.String saturday { get { return this._saturday; } set { this._saturday = value?.Trim(); } }

        private System.String _sunday;
        /// <summary>
        /// 星期天
        /// </summary>
        public System.String sunday { get { return this._sunday; } set { this._sunday = value?.Trim(); } }

        private System.Int32 _roomid;
        /// <summary>
        /// 诊室ID
        /// </summary>
        public System.Int32 roomid { get { return this._roomid; } set { this._roomid = value; } }

        private System.String _roomname;
        /// <summary>
        /// 诊室
        /// </summary>
        public System.String roomname { get { return this._roomname; } set { this._roomname = value?.Trim(); } }

        private System.Int16 _callflag;
        /// <summary>
        /// 叫号标志
        /// </summary>
        public System.Int16 callflag { get { return this._callflag; } set { this._callflag = value; } }
    }
}