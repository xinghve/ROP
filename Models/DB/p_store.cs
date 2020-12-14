using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 门店表
    /// </summary>
    public class p_store
    {
        /// <summary>
        /// 门店表
        /// </summary>
        public p_store()
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

        private System.String _code;
        /// <summary>
        /// 编码
        /// </summary>
        public System.String code { get { return this._code; } set { this._code = value?.Trim(); } }

        private System.String _name;
        /// <summary>
        /// 名称
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.String _no;
        /// <summary>
        /// 门店编号
        /// </summary>
        public System.String no { get { return this._no; } set { this._no = value?.Trim(); } }

        private System.String _manager;
        /// <summary>
        /// 负责人
        /// </summary>
        public System.String manager { get { return this._manager; } set { this._manager = value?.Trim(); } }

        private System.String _phone_no;
        /// <summary>
        /// 负责人电话
        /// </summary>
        public System.String phone_no { get { return this._phone_no; } set { this._phone_no = value?.Trim(); } }

        private System.String _province_code;
        /// <summary>
        /// 省代码
        /// </summary>
        public System.String province_code { get { return this._province_code; } set { this._province_code = value?.Trim(); } }

        private System.String _province_name;
        /// <summary>
        /// 省名称
        /// </summary>
        public System.String province_name { get { return this._province_name; } set { this._province_name = value?.Trim(); } }

        private System.String _city_code;
        /// <summary>
        /// 市代码
        /// </summary>
        public System.String city_code { get { return this._city_code; } set { this._city_code = value?.Trim(); } }

        private System.String _city_name;
        /// <summary>
        /// 市名称
        /// </summary>
        public System.String city_name { get { return this._city_name; } set { this._city_name = value?.Trim(); } }

        private System.String _county_code;
        /// <summary>
        /// 区县代码
        /// </summary>
        public System.String county_code { get { return this._county_code; } set { this._county_code = value?.Trim(); } }

        private System.String _county_name;
        /// <summary>
        /// 区县名称
        /// </summary>
        public System.String county_name { get { return this._county_name; } set { this._county_name = value?.Trim(); } }

        private System.String _street_code;
        /// <summary>
        /// 街道代码
        /// </summary>
        public System.String street_code { get { return this._street_code; } set { this._street_code = value?.Trim(); } }

        private System.String _street_name;
        /// <summary>
        /// 街道名称
        /// </summary>
        public System.String street_name { get { return this._street_name; } set { this._street_name = value?.Trim(); } }

        private System.String _address;
        /// <summary>
        /// 详细地址
        /// </summary>
        public System.String address { get { return this._address; } set { this._address = value?.Trim(); } }

        private System.String _license_no;
        /// <summary>
        /// 执照号
        /// </summary>
        public System.String license_no { get { return this._license_no; } set { this._license_no = value?.Trim(); } }

        private System.String _license_path;
        /// <summary>
        /// 执照图片路径
        /// </summary>
        public System.String license_path { get { return this._license_path; } set { this._license_path = value?.Trim(); } }

        private System.String _cert_no;
        /// <summary>
        /// 执业资格证书号
        /// </summary>
        public System.String cert_no { get { return this._cert_no; } set { this._cert_no = value?.Trim(); } }

        private System.String _cert_path;
        /// <summary>
        /// 执业资格证书路径
        /// </summary>
        public System.String cert_path { get { return this._cert_path; } set { this._cert_path = value?.Trim(); } }

        private System.Int16 _status;
        /// <summary>
        /// 状态： 0-待审 1-试用 2-正常
        /// </summary>
        public System.Int16 status { get { return this._status; } set { this._status = value; } }

        private System.Int16 _cert_status;
        /// <summary>
        /// 实名认证状态：0-未认证 1-未通过 2-认证申请 3-已通过
        /// </summary>
        public System.Int16 cert_status { get { return this._cert_status; } set { this._cert_status = value; } }

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

        private System.Int16 _use_status;
        /// <summary>
        /// 状态： -1-删除  0-停用 1-正常
        /// </summary>
        public System.Int16 use_status { get { return this._use_status; } set { this._use_status = value; } }

        private System.String _introduce;
        /// <summary>
        /// 简介
        /// </summary>
        public System.String introduce { get { return this._introduce; } set { this._introduce = value?.Trim(); } }

        private System.String _tel;
        /// <summary>
        /// 座机
        /// </summary>
        public System.String tel { get { return this._tel; } set { this._tel = value?.Trim(); } }
    }
}