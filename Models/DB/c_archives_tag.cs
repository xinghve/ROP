using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 档案标签
    /// </summary>
    public class c_archives_tag
    {
        /// <summary>
        /// 档案标签
        /// </summary>
        public c_archives_tag()
        {
        }

        private System.Int32 _archives_id;
        /// <summary>
        /// 档案ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 archives_id { get { return this._archives_id; } set { this._archives_id = value; } }

        private System.Int32 _tag_id;
        /// <summary>
        /// 标签编码
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 tag_id { get { return this._tag_id; } set { this._tag_id = value; } }

        private System.String _tag;
        /// <summary>
        /// 标签
        /// </summary>
        public System.String tag { get { return this._tag; } set { this._tag = value?.Trim(); } }
    }
}