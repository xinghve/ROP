using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 分销人员
    /// </summary>
    public class p_distributor
    {
        /// <summary>
        /// 分销人员
        /// </summary>
        public p_distributor()
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

        private System.Int32 _store_id;
        /// <summary>
        /// 门店Id
        /// </summary>
        public System.Int32 store_id { get { return this._store_id; } set { this._store_id = value; } }

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

        private System.String _sex_code;
        /// <summary>
        /// 性别代码
        /// </summary>
        public System.String sex_code { get { return this._sex_code; } set { this._sex_code = value?.Trim(); } }

        private System.String _sex_name;
        /// <summary>
        /// 性别名称
        /// </summary>
        public System.String sex_name { get { return this._sex_name; } set { this._sex_name = value?.Trim(); } }

        private System.String _phone_no;
        /// <summary>
        /// 手机号，同时用于登录
        /// </summary>
        public System.String phone_no { get { return this._phone_no; } set { this._phone_no = value?.Trim(); } }

        private System.String _open_id;
        /// <summary>
        /// OpenID
        /// </summary>
        public System.String open_id { get { return this._open_id; } set { this._open_id = value?.Trim(); } }

        private System.String _image_url;
        /// <summary>
        /// 头像图片地址
        /// </summary>
        public System.String image_url { get { return this._image_url; } set { this._image_url = value?.Trim(); } }

        private System.String _password;
        /// <summary>
        /// 登陆密码
        /// </summary>
        public System.String password { get { return this._password; } set { this._password = value?.Trim(); } }

        private System.Decimal? _amount;
        /// <summary>
        /// 提成总金额
        /// </summary>
        public System.Decimal? amount { get { return this._amount; } set { this._amount = value ?? default(System.Decimal); } }

        private System.Decimal? _settleamount;
        /// <summary>
        /// 已结提成
        /// </summary>
        public System.Decimal? settleamount { get { return this._settleamount; } set { this._settleamount = value ?? default(System.Decimal); } }

        private System.Decimal? _noneamount;
        /// <summary>
        /// 未结提成
        /// </summary>
        public System.Decimal? noneamount { get { return this._noneamount; } set { this._noneamount = value ?? default(System.Decimal); } }

        private System.Int32 _director_id;
        /// <summary>
        /// 负责人ID
        /// </summary>
        public System.Int32 director_id { get { return this._director_id; } set { this._director_id = value; } }

        private System.String _director;
        /// <summary>
        /// 负责人
        /// </summary>
        public System.String director { get { return this._director; } set { this._director = value?.Trim(); } }

        private System.String _address;
        /// <summary>
        /// 详细地址
        /// </summary>
        public System.String address { get { return this._address; } set { this._address = value?.Trim(); } }

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
    }
}