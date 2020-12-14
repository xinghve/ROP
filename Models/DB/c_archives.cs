using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 档案信息表
    /// </summary>
    public class c_archives
    {
        /// <summary>
        /// 档案信息表
        /// </summary>
        public c_archives()
        {
        }

        private System.Int32 _id;
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public System.Int32 id { get { return this._id; } set { this._id = value; } }

        private System.String _no;
        /// <summary>
        /// 档案号
        /// </summary>
        public System.String no { get { return this._no; } set { this._no = value?.Trim(); } }

        private System.Int32 _org_id;
        /// <summary>
        /// 机构id
        /// </summary>
        public System.Int32 org_id { get { return this._org_id; } set { this._org_id = value; } }

        private System.String _name;
        /// <summary>
        /// 姓名
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.String _spell;
        /// <summary>
        /// 拼音码（首字母）
        /// </summary>
        public System.String spell { get { return this._spell; } set { this._spell = value?.Trim(); } }

        private System.String _flagid;
        /// <summary>
        /// 个人标识ID
        /// </summary>
        public System.String flagid { get { return this._flagid; } set { this._flagid = value?.Trim(); } }

        private System.String _flag;
        /// <summary>
        /// 个人标识
        /// </summary>
        public System.String flag { get { return this._flag; } set { this._flag = value?.Trim(); } }

        private System.String _id_card;
        /// <summary>
        /// 证件号
        /// </summary>
        public System.String id_card { get { return this._id_card; } set { this._id_card = value?.Trim(); } }

        private System.Int16? _age;
        /// <summary>
        /// 年龄
        /// </summary>
        public System.Int16? age { get { return this._age; } set { this._age = value ?? default(System.Int16); } }

        private System.Int16? _month;
        /// <summary>
        /// 月
        /// </summary>
        public System.Int16? month { get { return this._month; } set { this._month = value ?? default(System.Int16); } }

        private System.Int16? _day;
        /// <summary>
        /// 日
        /// </summary>
        public System.Int16? day { get { return this._day; } set { this._day = value ?? default(System.Int16); } }

        private System.DateTime? _birthday;
        /// <summary>
        /// 生日
        /// </summary>
        public System.DateTime? birthday { get { return this._birthday; } set { this._birthday = value ?? default(System.DateTime); } }

        private System.String _phone;
        /// <summary>
        /// 联系电话
        /// </summary>
        public System.String phone { get { return this._phone; } set { this._phone = value?.Trim(); } }

        private System.String _emergencyno;
        /// <summary>
        /// 应急电话
        /// </summary>
        public System.String emergencyno { get { return this._emergencyno; } set { this._emergencyno = value?.Trim(); } }

        private System.String _password;
        /// <summary>
        /// 登录密码
        /// </summary>
        public System.String password { get { return this._password; } set { this._password = value?.Trim(); } }

        private System.String _sex_code;
        /// <summary>
        /// 性别代码
        /// </summary>
        public System.String sex_code { get { return this._sex_code; } set { this._sex_code = value?.Trim(); } }

        private System.String _sex;
        /// <summary>
        /// 性别
        /// </summary>
        public System.String sex { get { return this._sex; } set { this._sex = value?.Trim(); } }

        private System.String _type_code;
        /// <summary>
        /// 类型编码
        /// </summary>
        public System.String type_code { get { return this._type_code; } set { this._type_code = value?.Trim(); } }

        private System.String _type;
        /// <summary>
        /// 类型
        /// </summary>
        public System.String type { get { return this._type; } set { this._type = value?.Trim(); } }

        private System.Int16? _state;
        /// <summary>
        /// 状态
        /// </summary>
        public System.Int16? state { get { return this._state; } set { this._state = value ?? default(System.Int16); } }

        private System.Int32 _store_id;
        /// <summary>
        /// 门店id
        /// </summary>
        public System.Int32 store_id { get { return this._store_id; } set { this._store_id = value; } }

        private System.String _constellation_code;
        /// <summary>
        /// 星座编码
        /// </summary>
        public System.String constellation_code { get { return this._constellation_code; } set { this._constellation_code = value?.Trim(); } }

        private System.String _constellation;
        /// <summary>
        /// 星座
        /// </summary>
        public System.String constellation { get { return this._constellation; } set { this._constellation = value?.Trim(); } }

        private System.String _zodiac_code;
        /// <summary>
        /// 生肖编码
        /// </summary>
        public System.String zodiac_code { get { return this._zodiac_code; } set { this._zodiac_code = value?.Trim(); } }

        private System.String _zodiac;
        /// <summary>
        /// 生肖
        /// </summary>
        public System.String zodiac { get { return this._zodiac; } set { this._zodiac = value?.Trim(); } }

        private System.String _educationid;
        /// <summary>
        /// 文化程度ID
        /// </summary>
        public System.String educationid { get { return this._educationid; } set { this._educationid = value?.Trim(); } }

        private System.String _education;
        /// <summary>
        /// 文化程度
        /// </summary>
        public System.String education { get { return this._education; } set { this._education = value?.Trim(); } }

        private System.String _occupation_code;
        /// <summary>
        /// 职业编码
        /// </summary>
        public System.String occupation_code { get { return this._occupation_code; } set { this._occupation_code = value?.Trim(); } }

        private System.String _occupation;
        /// <summary>
        /// 职业
        /// </summary>
        public System.String occupation { get { return this._occupation; } set { this._occupation = value?.Trim(); } }

        private System.String _aboid;
        /// <summary>
        /// ABO血型ID
        /// </summary>
        public System.String aboid { get { return this._aboid; } set { this._aboid = value?.Trim(); } }

        private System.String _abo;
        /// <summary>
        /// ABO血型
        /// </summary>
        public System.String abo { get { return this._abo; } set { this._abo = value?.Trim(); } }

        private System.String _rhid;
        /// <summary>
        /// RH血型ID
        /// </summary>
        public System.String rhid { get { return this._rhid; } set { this._rhid = value?.Trim(); } }

        private System.String _rh;
        /// <summary>
        /// RH血型
        /// </summary>
        public System.String rh { get { return this._rh; } set { this._rh = value?.Trim(); } }

        private System.String _nation_code;
        /// <summary>
        /// 民族编码
        /// </summary>
        public System.String nation_code { get { return this._nation_code; } set { this._nation_code = value?.Trim(); } }

        private System.String _nation;
        /// <summary>
        /// 民族
        /// </summary>
        public System.String nation { get { return this._nation; } set { this._nation = value?.Trim(); } }

        private System.String _marital_status_code;
        /// <summary>
        /// 婚姻状况编码
        /// </summary>
        public System.String marital_status_code { get { return this._marital_status_code; } set { this._marital_status_code = value?.Trim(); } }

        private System.String _marital_status;
        /// <summary>
        /// 婚姻状况
        /// </summary>
        public System.String marital_status { get { return this._marital_status; } set { this._marital_status = value?.Trim(); } }

        private System.String _address;
        /// <summary>
        /// 联系地址
        /// </summary>
        public System.String address { get { return this._address; } set { this._address = value?.Trim(); } }

        private System.String _contacts;
        /// <summary>
        /// 联系人
        /// </summary>
        public System.String contacts { get { return this._contacts; } set { this._contacts = value?.Trim(); } }

        private System.String _contacts_phone;
        /// <summary>
        /// 联系人手机
        /// </summary>
        public System.String contacts_phone { get { return this._contacts_phone; } set { this._contacts_phone = value?.Trim(); } }

        private System.String _relationid;
        /// <summary>
        /// 联系人关系ID
        /// </summary>
        public System.String relationid { get { return this._relationid; } set { this._relationid = value?.Trim(); } }

        private System.String _relation;
        /// <summary>
        /// 联系人关系
        /// </summary>
        public System.String relation { get { return this._relation; } set { this._relation = value?.Trim(); } }

        private System.String _qq;
        /// <summary>
        /// QQ
        /// </summary>
        public System.String qq { get { return this._qq; } set { this._qq = value?.Trim(); } }

        private System.String _wechat;
        /// <summary>
        /// 微信
        /// </summary>
        public System.String wechat { get { return this._wechat; } set { this._wechat = value?.Trim(); } }

        private System.String _email;
        /// <summary>
        /// 邮箱
        /// </summary>
        public System.String email { get { return this._email; } set { this._email = value?.Trim(); } }

        private System.Decimal? _income;
        /// <summary>
        /// 收入
        /// </summary>
        public System.Decimal? income { get { return this._income; } set { this._income = value ?? default(System.Decimal); } }

        private System.String _province_code;
        /// <summary>
        /// 省编码
        /// </summary>
        public System.String province_code { get { return this._province_code; } set { this._province_code = value?.Trim(); } }

        private System.String _province;
        /// <summary>
        /// 省
        /// </summary>
        public System.String province { get { return this._province; } set { this._province = value?.Trim(); } }

        private System.String _city_code;
        /// <summary>
        /// 市编码
        /// </summary>
        public System.String city_code { get { return this._city_code; } set { this._city_code = value?.Trim(); } }

        private System.String _city;
        /// <summary>
        /// 市
        /// </summary>
        public System.String city { get { return this._city; } set { this._city = value?.Trim(); } }

        private System.String _county_code;
        /// <summary>
        /// 区编码
        /// </summary>
        public System.String county_code { get { return this._county_code; } set { this._county_code = value?.Trim(); } }

        private System.String _county;
        /// <summary>
        /// 区
        /// </summary>
        public System.String county { get { return this._county; } set { this._county = value?.Trim(); } }

        private System.String _street_code;
        /// <summary>
        /// 街道编码
        /// </summary>
        public System.String street_code { get { return this._street_code; } set { this._street_code = value?.Trim(); } }

        private System.String _street;
        /// <summary>
        /// 街道
        /// </summary>
        public System.String street { get { return this._street; } set { this._street = value?.Trim(); } }

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

        private System.String _carno;
        /// <summary>
        /// 车牌号
        /// </summary>
        public System.String carno { get { return this._carno; } set { this._carno = value?.Trim(); } }

        private System.String _card_no;
        /// <summary>
        /// 就诊卡
        /// </summary>
        public System.String card_no { get { return this._card_no; } set { this._card_no = value?.Trim(); } }

        private System.Int32 _grade_id;
        /// <summary>
        /// 等级ID
        /// </summary>
        public System.Int32 grade_id { get { return this._grade_id; } set { this._grade_id = value; } }

        private System.String _remarks;
        /// <summary>
        /// 备注
        /// </summary>
        public System.String remarks { get { return this._remarks; } set { this._remarks = value?.Trim(); } }

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

        private System.String _virtual_no;
        /// <summary>
        /// 虚拟就诊卡
        /// </summary>
        public System.String virtual_no { get { return this._virtual_no; } set { this._virtual_no = value?.Trim(); } }

        private System.String _archives_class;
        /// <summary>
        /// 客户分类
        /// </summary>
        public System.String archives_class { get { return this._archives_class; } set { this._archives_class = value?.Trim(); } }

        private System.String _class_code;
        /// <summary>
        /// 分类编码
        /// </summary>
        public System.String class_code { get { return this._class_code; } set { this._class_code = value?.Trim(); } }

        private System.String _source;
        /// <summary>
        /// 客户来源
        /// </summary>
        public System.String source { get { return this._source; } set { this._source = value?.Trim(); } }

        private System.String _image_url;
        /// <summary>
        /// 头像图片地址
        /// </summary>
        public System.String image_url { get { return this._image_url; } set { this._image_url = value?.Trim(); } }

        private System.String _coupon_no;
        /// <summary>
        /// 券码
        /// </summary>
        public System.String coupon_no { get { return this._coupon_no; } set { this._coupon_no = value?.Trim(); } }

        private System.String _source_code;
        /// <summary>
        /// 来源编码
        /// </summary>
        public System.String source_code { get { return this._source_code; } set { this._source_code = value?.Trim(); } }
    }
}