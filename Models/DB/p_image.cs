using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 图片（集团、门店、部门）
    /// </summary>
    public class p_image
    {
        /// <summary>
        /// 图片（集团、门店、部门）
        /// </summary>
        public p_image()
        {
        }

        private System.String _url;
        /// <summary>
        /// 图片地址
        /// </summary>
        public System.String url { get { return this._url; } set { this._url = value?.Trim(); } }

        private System.String _relation_code;
        /// <summary>
        /// 关联编码
        /// </summary>
        public System.String relation_code { get { return this._relation_code; } set { this._relation_code = value?.Trim(); } }
    }
}