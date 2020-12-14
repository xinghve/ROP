using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 医疗室工作安排
    /// </summary>
    public class his_room_work
    {
        /// <summary>
        /// 医疗室工作安排
        /// </summary>
        public his_room_work()
        {
        }

        private System.Int32 _room_id;
        /// <summary>
        /// 医疗室ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 room_id { get { return this._room_id; } set { this._room_id = value; } }

        private System.DateTime _work_date;
        /// <summary>
        /// 工作日期
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.DateTime work_date { get { return this._work_date; } set { this._work_date = value; } }

        private System.Int16 _total_times;
        /// <summary>
        /// 工作总时长（分钟）
        /// </summary>
        public System.Int16 total_times { get { return this._total_times; } set { this._total_times = value; } }

        private System.Int16 _work_times;
        /// <summary>
        /// 已工作时长（分钟）
        /// </summary>
        public System.Int16 work_times { get { return this._work_times; } set { this._work_times = value; } }
    }
}