using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 文件管理
    /// </summary>
    public class oa_files
    {
        /// <summary>
        /// 文件管理
        /// </summary>
        public oa_files()
        {
        }

        private System.String _name;
        /// <summary>
        /// 文件名
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.String _url;
        /// <summary>
        /// 路径
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String url { get { return this._url; } set { this._url = value?.Trim(); } }

        private System.String _type;
        /// <summary>
        /// 类型
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String type { get { return this._type; } set { this._type = value?.Trim(); } }

        private System.Int32 _size;
        /// <summary>
        /// 大小
        /// </summary>
        public System.Int32 size { get { return this._size; } set { this._size = value; } }

        private System.Int32 _org_id;
        /// <summary>
        /// 集团ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 org_id { get { return this._org_id; } set { this._org_id = value; } }

        private System.Int32 _store_id;
        /// <summary>
        /// 门店ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 store_id { get { return this._store_id; } set { this._store_id = value; } }

        private System.String _store_name;
        /// <summary>
        /// 门店名称
        /// </summary>
        public System.String store_name { get { return this._store_name; } set { this._store_name = value?.Trim(); } }

        private System.Int32 _creater_id;
        /// <summary>
        /// 创建人ID
        /// </summary>
        public System.Int32 creater_id { get { return this._creater_id; } set { this._creater_id = value; } }

        private System.String _creater;
        /// <summary>
        /// 创建人
        /// </summary>
        public System.String creater { get { return this._creater; } set { this._creater = value?.Trim(); } }

        private System.DateTime _create_time;
        /// <summary>
        /// 创建时间
        /// </summary>
        public System.DateTime create_time { get { return this._create_time; } set { this._create_time = value; } }
    }
}