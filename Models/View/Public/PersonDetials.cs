using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Public
{
    /// <summary>
    /// 员工详情
    /// </summary>
    public class PersonDetials : p_employee
    {
        private System.String _no;
        /// <summary>
        /// 工号
        /// </summary>
        public System.String no { get { return this._no; } set { this._no = value?.Trim(); } }

        private System.String _email;
        /// <summary>
        /// 电子邮件地址
        /// </summary>
        public System.String email { get { return this._email; } set { this._email = value?.Trim(); } }

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

        private System.DateTime? _birthday;
        /// <summary>
        /// 出生日期
        /// </summary>
        public System.DateTime? birthday { get { return this._birthday; } set { this._birthday = value ?? default(System.DateTime); } }

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

        private System.String _description;
        /// <summary>
        /// 描述
        /// </summary>
        public System.String description { get { return this._description; } set { this._description = value?.Trim(); } }

        private System.String _address;
        /// <summary>
        /// 详细地址
        /// </summary>
        public System.String address { get { return this._address; } set { this._address = value?.Trim(); } }

        public List<DeptRoleMode> deptRoleModes { get; set; }

        public List<Store> stores { get; set; }

        public bool status { get; set; }

        public List<p_employee_nature> employeeNatures { get; set; }
    }
}
