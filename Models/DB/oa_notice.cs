using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 通知表
    /// </summary>
    public class oa_notice
    {
        /// <summary>
        /// 通知表
        /// </summary>
        public oa_notice()
        {
        }

        private System.Int32 _id;
        /// <summary>
        /// 自增主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public System.Int32 id { get { return this._id; } set { this._id = value; } }

        private System.Int32 _org_id;
        /// <summary>
        /// 机构id
        /// </summary>
        public System.Int32 org_id { get { return this._org_id; } set { this._org_id = value; } }

        private System.Int32? _store_id;
        /// <summary>
        /// 门店id
        /// </summary>
        public System.Int32? store_id { get { return this._store_id; } set { this._store_id = value ?? default(System.Int32); } }

        private System.Int16 _notice_type_id;
        /// <summary>
        /// 通知类型id
        /// </summary>
        public System.Int16 notice_type_id { get { return this._notice_type_id; } set { this._notice_type_id = value; } }

        private System.String _notice_type_code;
        /// <summary>
        /// 通知类型编码
        /// </summary>
        public System.String notice_type_code { get { return this._notice_type_code; } set { this._notice_type_code = value?.Trim(); } }

        private System.Int16 _notice_property_id;
        /// <summary>
        /// 通知属性
        /// </summary>
        public System.Int16 notice_property_id { get { return this._notice_property_id; } set { this._notice_property_id = value; } }

        private System.String _notice_type_name;
        /// <summary>
        /// 通知类型名
        /// </summary>
        public System.String notice_type_name { get { return this._notice_type_name; } set { this._notice_type_name = value?.Trim(); } }

        private System.String _notice_property;
        /// <summary>
        /// 通知属性
        /// </summary>
        public System.String notice_property { get { return this._notice_property; } set { this._notice_property = value?.Trim(); } }

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

        private System.DateTime _create_date;
        /// <summary>
        /// 建档时间
        /// </summary>
        public System.DateTime create_date { get { return this._create_date; } set { this._create_date = value; } }

        private System.String _notice_content;
        /// <summary>
        /// 通知内容
        /// </summary>
        public System.String notice_content { get { return this._notice_content; } set { this._notice_content = value?.Trim(); } }


        private System.String _relation_no;
        /// <summary>
        /// 关联单号
        /// </summary>
        public System.String relation_no { get { return this._relation_no; } set { this._relation_no = value?.Trim(); } }

        private System.String _name;
        /// <summary>
        /// 名称
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }
    }
}