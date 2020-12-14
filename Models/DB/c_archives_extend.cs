using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 档案信息扩展表
    /// </summary>
    public class c_archives_extend
    {
        /// <summary>
        /// 档案信息扩展表
        /// </summary>
        public c_archives_extend()
        {
        }

        private System.Int32 _id;
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 id { get { return this._id; } set { this._id = value; } }

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

        private System.Decimal? _income;
        /// <summary>
        /// 收入
        /// </summary>
        public System.Decimal? income { get { return this._income; } set { this._income = value ?? default(System.Decimal); } }

        private System.String _source_code;
        /// <summary>
        /// 来源编码
        /// </summary>
        public System.String source_code { get { return this._source_code; } set { this._source_code = value?.Trim(); } }

        private System.String _source;
        /// <summary>
        /// 来源
        /// </summary>
        public System.String source { get { return this._source; } set { this._source = value?.Trim(); } }

        private System.String _hobby;
        /// <summary>
        /// 爱好
        /// </summary>
        public System.String hobby { get { return this._hobby; } set { this._hobby = value?.Trim(); } }

        private System.String _favour_way_code;
        /// <summary>
        /// 偏爱诊疗方式编码
        /// </summary>
        public System.String favour_way_code { get { return this._favour_way_code; } set { this._favour_way_code = value?.Trim(); } }

        private System.String _favour_way;
        /// <summary>
        /// 偏爱诊疗方式
        /// </summary>
        public System.String favour_way { get { return this._favour_way; } set { this._favour_way = value?.Trim(); } }

        private System.String _consume_habit_code;
        /// <summary>
        /// 消费习惯编码
        /// </summary>
        public System.String consume_habit_code { get { return this._consume_habit_code; } set { this._consume_habit_code = value?.Trim(); } }

        private System.String _consume_habit;
        /// <summary>
        /// 消费习惯
        /// </summary>
        public System.String consume_habit { get { return this._consume_habit; } set { this._consume_habit = value?.Trim(); } }

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

        private System.String _blood_type_code;
        /// <summary>
        /// 血型编码
        /// </summary>
        public System.String blood_type_code { get { return this._blood_type_code; } set { this._blood_type_code = value?.Trim(); } }

        private System.String _blood_type;
        /// <summary>
        /// 血型
        /// </summary>
        public System.String blood_type { get { return this._blood_type; } set { this._blood_type = value?.Trim(); } }

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

        private System.String _address;
        /// <summary>
        /// 详细住址
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

        private System.String _remarks;
        /// <summary>
        /// 备注
        /// </summary>
        public System.String remarks { get { return this._remarks; } set { this._remarks = value?.Trim(); } }

        private System.String _coupon_no;
        /// <summary>
        /// 券码
        /// </summary>
        public System.String coupon_no { get { return this._coupon_no; } set { this._coupon_no = value?.Trim(); } }
    }
}