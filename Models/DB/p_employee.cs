using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 员工表
    /// </summary>
    public class p_employee
    {
        /// <summary>
        /// 员工表
        /// </summary>
        public p_employee()
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
        /// 组织机构Id
        /// </summary>
        public System.Int32 org_id { get { return this._org_id; } set { this._org_id = value; } }

        private System.String _name;
        /// <summary>
        /// 姓名
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.String _pinyin;
        /// <summary>
        /// 拼音码
        /// </summary>
        public System.String pinyin { get { return this._pinyin; } set { this._pinyin = value?.Trim(); } }

        private System.String _id_no;
        /// <summary>
        /// 身份证号
        /// </summary>
        public System.String id_no { get { return this._id_no; } set { this._id_no = value?.Trim(); } }

        private System.String _phone_no;
        /// <summary>
        /// 手机号，同时用于登录
        /// </summary>
        public System.String phone_no { get { return this._phone_no; } set { this._phone_no = value?.Trim(); } }

        private System.String _image_url;
        /// <summary>
        /// 头像图片地址
        /// </summary>
        public System.String image_url { get { return this._image_url; } set { this._image_url = value?.Trim(); } }

        private System.String _pro_duties_id;
        /// <summary>
        /// 专业职务ID
        /// </summary>
        public System.String pro_duties_id { get { return this._pro_duties_id; } set { this._pro_duties_id = value?.Trim(); } }

        private System.String _pro_duties;
        /// <summary>
        /// 专业职务
        /// </summary>
        public System.String pro_duties { get { return this._pro_duties; } set { this._pro_duties = value?.Trim(); } }

        private System.String _password;
        /// <summary>
        /// 登陆密码
        /// </summary>
        public System.String password { get { return this._password; } set { this._password = value?.Trim(); } }

        private System.Boolean _is_admin;
        /// <summary>
        /// 是否管理员
        /// </summary>
        public System.Boolean is_admin { get { return this._is_admin; } set { this._is_admin = value; } }

        private System.DateTime _create_time;
        /// <summary>
        /// 创建时间
        /// </summary>
        public System.DateTime create_time { get { return this._create_time; } set { this._create_time = value; } }

        private System.DateTime _expire_time;
        /// <summary>
        /// 失效时间
        /// </summary>
        public System.DateTime expire_time { get { return this._expire_time; } set { this._expire_time = value; } }

        private System.String _good_at;
        /// <summary>
        /// 擅长
        /// </summary>
        public System.String good_at { get { return this._good_at; } set { this._good_at = value?.Trim(); } }
    }
}