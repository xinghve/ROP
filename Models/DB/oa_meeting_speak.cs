using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 会议发言
    /// </summary>
    public class oa_meeting_speak
    {
        /// <summary>
        /// 会议发言
        /// </summary>
        public oa_meeting_speak()
        {
        }

        private System.Int32 _id;
        /// <summary>
        /// 主键ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 id { get { return this._id; } set { this._id = value; } }

        private System.Int32 _spokesman_id;
        /// <summary>
        /// 发言人ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 spokesman_id { get { return this._spokesman_id; } set { this._spokesman_id = value; } }

        private System.String _spokesman;
        /// <summary>
        /// 发言人
        /// </summary>
        public System.String spokesman { get { return this._spokesman; } set { this._spokesman = value?.Trim(); } }

        private System.String _point;
        /// <summary>
        /// 要点
        /// </summary>
        public System.String point { get { return this._point; } set { this._point = value?.Trim(); } }
    }
}