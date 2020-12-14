using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 微信档案关联
    /// </summary>
    public class c_wechat_archives
    {
        /// <summary>
        /// 微信档案关联
        /// </summary>
        public c_wechat_archives()
        {
        }

        private System.String _openid;
        /// <summary>
        /// 微信OpenID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String openid { get { return this._openid; } set { this._openid = value?.Trim(); } }

        private System.Int32 _archives_id;
        /// <summary>
        /// 档案ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 archives_id { get { return this._archives_id; } set { this._archives_id = value; } }
    }
}