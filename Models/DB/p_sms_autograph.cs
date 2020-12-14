using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 签名表
    /// </summary>
    public class p_sms_autograph
    {
        /// <summary>
        /// 签名表
        /// </summary>
        public p_sms_autograph()
        {
        }

        private System.Int32 _id;
        /// <summary>
        /// 自增主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public System.Int32 id { get { return this._id; } set { this._id = value; } }

        private System.String _end_point;
        /// <summary>
        /// 端点
        /// </summary>
        public System.String end_point { get { return this._end_point; } set { this._end_point = value?.Trim(); } }

        private System.String _access_key_id;
        /// <summary>
        /// 访问密钥
        /// </summary>
        public System.String access_key_id { get { return this._access_key_id; } set { this._access_key_id = value?.Trim(); } }

        private System.String _secret_access_key;
        /// <summary>
        /// 密钥存取密钥
        /// </summary>
        public System.String secret_access_key { get { return this._secret_access_key; } set { this._secret_access_key = value?.Trim(); } }

        private System.String _topic_name;
        /// <summary>
        /// 主题名称
        /// </summary>
        public System.String topic_name { get { return this._topic_name; } set { this._topic_name = value?.Trim(); } }

        private System.String _free_sign_name;
        /// <summary>
        /// 自由名称
        /// </summary>
        public System.String free_sign_name { get { return this._free_sign_name; } set { this._free_sign_name = value?.Trim(); } }

        private System.Int32? _org_id;
        /// <summary>
        /// 集团ID
        /// </summary>
        public System.Int32? org_id { get { return this._org_id; } set { this._org_id = value ?? default(System.Int32); } }
    }
}