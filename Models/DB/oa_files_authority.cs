using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 文件权限
    /// </summary>
    public class oa_files_authority
    {
        /// <summary>
        /// 文件权限
        /// </summary>
        public oa_files_authority()
        {
        }

        private System.String _name;
        /// <summary>
        /// 文件名
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.String _url;
        /// <summary>
        /// 路径
        /// </summary>
        public System.String url { get { return this._url; } set { this._url = value?.Trim(); } }

        private System.String _type;
        /// <summary>
        /// 类型
        /// </summary>
        public System.String type { get { return this._type; } set { this._type = value?.Trim(); } }

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

        private System.Int32 _link_id_type;
        /// <summary>
        /// 关联ID类型
        /// </summary>
        public System.Int32 link_id_type { get { return this._link_id_type; } set { this._link_id_type = value; } }

        private System.Int32 _link_id;
        /// <summary>
        /// 关联ID
        /// </summary>
        public System.Int32 link_id { get { return this._link_id; } set { this._link_id = value; } }

        private System.String _link_name;
        /// <summary>
        /// 关联名称
        /// </summary>
        public System.String link_name { get { return this._link_name; } set { this._link_name = value?.Trim(); } }
    }
}