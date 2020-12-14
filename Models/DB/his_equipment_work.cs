using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 设备器械工作
    /// </summary>
    public class his_equipment_work
    {
        /// <summary>
        /// 设备器械工作
        /// </summary>
        public his_equipment_work()
        {
        }

        private System.Int32 _equipment_id;
        /// <summary>
        /// 设备ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 equipment_id { get { return this._equipment_id; } set { this._equipment_id = value; } }

        private System.String _no;
        /// <summary>
        /// 编号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String no { get { return this._no; } set { this._no = value?.Trim(); } }

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