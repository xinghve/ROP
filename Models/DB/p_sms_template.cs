using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 短信模板表
    /// </summary>
    public class p_sms_template
    {
        /// <summary>
        /// 短信模板表
        /// </summary>
        public p_sms_template()
        {
        }

        private System.Int32 _id;
        /// <summary>
        /// 自增主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public System.Int32 id { get { return this._id; } set { this._id = value; } }

        private System.String _name;
        /// <summary>
        /// 模版名称
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.String _code;
        /// <summary>
        /// 模版CODE
        /// </summary>
        public System.String code { get { return this._code; } set { this._code = value?.Trim(); } }

        private System.String _parameter;
        /// <summary>
        /// 参数
        /// </summary>
        public System.String parameter { get { return this._parameter; } set { this._parameter = value?.Trim(); } }

        private System.DateTime _create_time;
        /// <summary>
        /// 创建时间
        /// </summary>
        public System.DateTime create_time { get { return this._create_time; } set { this._create_time = value; } }

        private System.Int32? _org_id;
        /// <summary>
        /// 集团ID
        /// </summary>
        public System.Int32? org_id { get { return this._org_id; } set { this._org_id = value ?? default(System.Int32); } }

        private System.String _content;
        /// <summary>
        /// 模版内容
        /// </summary>
        public System.String content { get { return this._content; } set { this._content = value?.Trim(); } }

        private System.Int16? _is_select;
        /// <summary>
        /// 是否可选（1=可选，0=不可选）
        /// </summary>
        public System.Int16? is_select { get { return this._is_select; } set { this._is_select = value ?? default(System.Int16); } }
    }
}