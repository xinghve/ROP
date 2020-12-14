using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 偏好
    /// </summary>
    public class c_archives_preference
    {
        /// <summary>
        /// 偏好
        /// </summary>
        public c_archives_preference()
        {
        }

        private System.Int32 _archives_id;
        /// <summary>
        /// 档案ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 archives_id { get { return this._archives_id; } set { this._archives_id = value; } }

        private System.String _text;
        /// <summary>
        /// 偏好
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String text { get { return this._text; } set { this._text = value?.Trim(); } }
    }
}