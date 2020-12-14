using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 会议决议
    /// </summary>
    public class oa_meeting_resolution
    {
        /// <summary>
        /// 会议决议
        /// </summary>
        public oa_meeting_resolution()
        {
        }

        private System.Int32 _id;
        /// <summary>
        /// 主键ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 id { get { return this._id; } set { this._id = value; } }

        private System.String _no;
        /// <summary>
        /// 编号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String no { get { return this._no; } set { this._no = value?.Trim(); } }

        private System.String _matter;
        /// <summary>
        /// 事项
        /// </summary>
        public System.String matter { get { return this._matter; } set { this._matter = value?.Trim(); } }

        private System.String _target;
        /// <summary>
        /// 实施目标
        /// </summary>
        public System.String target { get { return this._target; } set { this._target = value?.Trim(); } }
    }
}