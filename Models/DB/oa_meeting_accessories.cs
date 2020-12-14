using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 会议附件
    /// </summary>
    public class oa_meeting_accessories
    {
        /// <summary>
        /// 会议附件
        /// </summary>
        public oa_meeting_accessories()
        {
        }

        private System.Int32 _id;
        /// <summary>
        /// 主键ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 id { get { return this._id; } set { this._id = value; } }

        private System.String _name;
        /// <summary>
        /// 名称
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.String _url;
        /// <summary>
        /// 路径
        /// </summary>
        public System.String url { get { return this._url; } set { this._url = value?.Trim(); } }
    }
}