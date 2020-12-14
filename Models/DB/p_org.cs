using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 
    /// </summary>
    public class p_org
    {
        /// <summary>
        /// 
        /// </summary>
        public p_org()
        {
        }

        private System.Int32 _id;
        /// <summary>
        /// 自增主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public System.Int32 id { get { return this._id; } set { this._id = value; } }

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

        private System.String _pinyin;
        /// <summary>
        /// 拼音码
        /// </summary>
        public System.String pinyin { get { return this._pinyin; } set { this._pinyin = value?.Trim(); } }

        private System.Int16 _status;
        /// <summary>
        /// 状态：0-待审 1-试用 2-正常
        /// </summary>
        public System.Int16 status { get { return this._status; } set { this._status = value; } }

        private System.Int16 _cert_status;
        /// <summary>
        /// 实名认证状态：0-未认证 1-未通过 2-认证申请 3-已通过
        /// </summary>
        public System.Int16 cert_status { get { return this._cert_status; } set { this._cert_status = value; } }

        private System.String _legal_manager;
        /// <summary>
        /// 法人
        /// </summary>
        public System.String legal_manager { get { return this._legal_manager; } set { this._legal_manager = value?.Trim(); } }

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

        private System.String _reg_address;
        /// <summary>
        /// 注册地址
        /// </summary>
        public System.String reg_address { get { return this._reg_address; } set { this._reg_address = value?.Trim(); } }

        private System.String _bank;
        /// <summary>
        /// 开户行
        /// </summary>
        public System.String bank { get { return this._bank; } set { this._bank = value?.Trim(); } }

        private System.String _bank_account;
        /// <summary>
        /// 银行账号
        /// </summary>
        public System.String bank_account { get { return this._bank_account; } set { this._bank_account = value?.Trim(); } }

        private System.String _license_no;
        /// <summary>
        /// 营业执照号
        /// </summary>
        public System.String license_no { get { return this._license_no; } set { this._license_no = value?.Trim(); } }

        private System.String _license_path;
        /// <summary>
        /// 营业执照图片路径
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

        private System.String _link_man;
        /// <summary>
        /// 联系人
        /// </summary>
        public System.String link_man { get { return this._link_man; } set { this._link_man = value?.Trim(); } }

        private System.String _phone_no;
        /// <summary>
        /// 联系人电话
        /// </summary>
        public System.String phone_no { get { return this._phone_no; } set { this._phone_no = value?.Trim(); } }

        private System.String _office_address;
        /// <summary>
        /// 办公地点
        /// </summary>
        public System.String office_address { get { return this._office_address; } set { this._office_address = value?.Trim(); } }

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

        private System.String _legal_phone_no;
        /// <summary>
        /// 法人手机号
        /// </summary>
        public System.String legal_phone_no { get { return this._legal_phone_no; } set { this._legal_phone_no = value?.Trim(); } }

        private System.Int16? _integral_card;
        /// <summary>
        /// 就诊卡是否可积分
        /// </summary>
        public System.Int16? integral_card { get { return this._integral_card; } set { this._integral_card = value ?? default(System.Int16); } }

        private System.Int16? _physical_card;
        /// <summary>
        /// 是否存在实体就诊卡
        /// </summary>
        public System.Int16? physical_card { get { return this._physical_card; } set { this._physical_card = value ?? default(System.Int16); } }

        private System.String _introduce;
        /// <summary>
        /// 简介
        /// </summary>
        public System.String introduce { get { return this._introduce; } set { this._introduce = value?.Trim(); } }

        private System.Decimal? _royalty_rate;
        /// <summary>
        /// 外部分销提成比例
        /// </summary>
        public System.Decimal? royalty_rate { get { return this._royalty_rate; } set { this._royalty_rate = value ?? default(System.Decimal); } }

        private System.Decimal? _cash_lower;
        /// <summary>
        /// 提现下限
        /// </summary>
        public System.Decimal? cash_lower { get { return this._cash_lower; } set { this._cash_lower = value ?? default(System.Decimal); } }

        private System.Decimal? _cash_uper;
        /// <summary>
        /// 提现上限
        /// </summary>
        public System.Decimal? cash_uper { get { return this._cash_uper; } set { this._cash_uper = value ?? default(System.Decimal); } }

        private System.String _distribution_policy;
        /// <summary>
        /// 分销政策
        /// </summary>
        public System.String distribution_policy { get { return this._distribution_policy; } set { this._distribution_policy = value?.Trim(); } }

        private System.String _withdrawal_rules;
        /// <summary>
        /// 提现规则
        /// </summary>
        public System.String withdrawal_rules { get { return this._withdrawal_rules; } set { this._withdrawal_rules = value?.Trim(); } }
    }
}