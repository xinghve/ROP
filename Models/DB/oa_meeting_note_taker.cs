using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 会议记录人
    /// </summary>
    public class oa_meeting_note_taker
    {
        /// <summary>
        /// 会议记录人
        /// </summary>
        public oa_meeting_note_taker()
        {
        }

        private System.Int32 _id;
        /// <summary>
        /// 主键ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 id { get { return this._id; } set { this._id = value; } }

        private System.Int32 _note_taker_id;
        /// <summary>
        /// 记录人ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 note_taker_id { get { return this._note_taker_id; } set { this._note_taker_id = value; } }

        private System.String _note_taker;
        /// <summary>
        /// 记录人
        /// </summary>
        public System.String note_taker { get { return this._note_taker; } set { this._note_taker = value?.Trim(); } }
    }
}