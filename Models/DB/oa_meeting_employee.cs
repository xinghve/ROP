using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 会议人员
    /// </summary>
    public class oa_meeting_employee
    {
        /// <summary>
        /// 会议人员
        /// </summary>
        public oa_meeting_employee()
        {
        }

        private System.Int32 _id;
        /// <summary>
        /// 主键ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 id { get { return this._id; } set { this._id = value; } }

        private System.Int32 _employee_id;
        /// <summary>
        /// 人员ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 employee_id { get { return this._employee_id; } set { this._employee_id = value; } }

        private System.String _employee;
        /// <summary>
        /// 人员
        /// </summary>
        public System.String employee { get { return this._employee; } set { this._employee = value?.Trim(); } }

        private System.String _record;
        /// <summary>
        /// 会议记录
        /// </summary>
        public System.String record { get { return this._record; } set { this._record = value?.Trim(); } }
    }
}