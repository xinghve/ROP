using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 客户关系维护
    /// </summary>
    public class c_maintain
    {
        /// <summary>
        /// 客户关系维护
        /// </summary>
        public c_maintain()
        {
        }

        private System.Int32 _id;
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public System.Int32 id { get { return this._id; } set { this._id = value; } }

        private System.Int32? _archives_id;
        /// <summary>
        /// 档案ID
        /// </summary>
        public System.Int32? archives_id { get { return this._archives_id; } set { this._archives_id = value ?? default(System.Int32); } }

        private System.String _archives;
        /// <summary>
        /// 档案姓名
        /// </summary>
        public System.String archives { get { return this._archives; } set { this._archives = value?.Trim(); } }

        private System.String _archives_phone;
        /// <summary>
        /// 档案手机
        /// </summary>
        public System.String archives_phone { get { return this._archives_phone; } set { this._archives_phone = value?.Trim(); } }

        private System.Int32? _from_director_id;
        /// <summary>
        /// 前负责人ID
        /// </summary>
        public System.Int32? from_director_id { get { return this._from_director_id; } set { this._from_director_id = value ?? default(System.Int32); } }

        private System.String _from_director;
        /// <summary>
        /// 前负责人
        /// </summary>
        public System.String from_director { get { return this._from_director; } set { this._from_director = value?.Trim(); } }

        private System.Int32? _to_director_id;
        /// <summary>
        /// 现负责人ID
        /// </summary>
        public System.Int32? to_director_id { get { return this._to_director_id; } set { this._to_director_id = value ?? default(System.Int32); } }

        private System.String _to_director;
        /// <summary>
        /// 现负责人
        /// </summary>
        public System.String to_director { get { return this._to_director; } set { this._to_director = value?.Trim(); } }

        private System.String _cause;
        /// <summary>
        /// 负责人转换原因
        /// </summary>
        public System.String cause { get { return this._cause; } set { this._cause = value?.Trim(); } }

        private System.Int32? _creater_id;
        /// <summary>
        /// 创建人ID
        /// </summary>
        public System.Int32? creater_id { get { return this._creater_id; } set { this._creater_id = value ?? default(System.Int32); } }

        private System.String _creater;
        /// <summary>
        /// 创建人
        /// </summary>
        public System.String creater { get { return this._creater; } set { this._creater = value?.Trim(); } }

        private System.DateTime? _create_date;
        /// <summary>
        /// 创建时间
        /// </summary>
        public System.DateTime? create_date { get { return this._create_date; } set { this._create_date = value ?? default(System.DateTime); } }

        private System.Int32? _from_store_id;
        /// <summary>
        /// 前门店id
        /// </summary>
        public System.Int32? from_store_id { get { return this._from_store_id; } set { this._from_store_id = value ?? default(System.Int32); } }

        private System.Int32 _org_id;
        /// <summary>
        /// 机构id
        /// </summary>
        public System.Int32 org_id { get { return this._org_id; } set { this._org_id = value; } }

        private System.Int32? _to_store_id;
        /// <summary>
        /// 现门店id
        /// </summary>
        public System.Int32? to_store_id { get { return this._to_store_id; } set { this._to_store_id = value ?? default(System.Int32); } }

        private System.String _from_store_name;
        /// <summary>
        /// 前门店名称
        /// </summary>
        public System.String from_store_name { get { return this._from_store_name; } set { this._from_store_name = value?.Trim(); } }

        private System.String _to_store_name;
        /// <summary>
        /// 现门店名称
        /// </summary>
        public System.String to_store_name { get { return this._to_store_name; } set { this._to_store_name = value?.Trim(); } }

        private System.String _id_card;
        /// <summary>
        /// 身份证号
        /// </summary>
        public System.String id_card { get { return this._id_card; } set { this._id_card = value?.Trim(); } }
    }
}