using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Public
{
    /// <summary>
    /// 注册
    /// </summary>
    public class Register
    {
        private System.String _name;
        /// <summary>
        /// 名称
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.String _legal_manager;
        /// <summary>
        /// 法人
        /// </summary>
        public System.String legal_manager { get { return this._legal_manager; } set { this._legal_manager = value?.Trim(); } }

        private System.String _legal_phone_no;
        /// <summary>
        /// 法人手机号
        /// </summary>
        public System.String legal_phone_no { get { return this._legal_phone_no; } set { this._legal_phone_no = value?.Trim(); } }

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

        private System.String _code;
        /// <summary>
        /// 验证码
        /// </summary>
        public System.String code { get { return this._code; } set { this._code = value?.Trim(); } }
    }
}
