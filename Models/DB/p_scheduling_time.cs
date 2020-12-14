using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 基础排班时间段
    /// </summary>
    public class p_scheduling_time
    {
        /// <summary>
        /// 基础排班时间段
        /// </summary>
        public p_scheduling_time()
        {
        }

        private System.Int32 _org_id;
        /// <summary>
        /// 集团ID
        /// </summary>
        public System.Int32 org_id { get { return this._org_id; } set { this._org_id = value; } }

        private System.Int32 _store_id;
        /// <summary>
        /// 门店ID
        /// </summary>
        public System.Int32 store_id { get { return this._store_id; } set { this._store_id = value; } }

        private System.TimeSpan? _start;
        /// <summary>
        /// 开始时间
        /// </summary>
        public System.TimeSpan? start { get { return this._start; } set { this._start = value ?? default(System.TimeSpan); } }

        private System.TimeSpan? _end;
        /// <summary>
        /// 结束时间
        /// </summary>
        public System.TimeSpan? end { get { return this._end; } set { this._end = value ?? default(System.TimeSpan); } }
    }
}