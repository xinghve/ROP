using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Crm
{
    /// <summary>
    /// 导入档案
    /// </summary>
    public class ArchivesImport
    {
        /// <summary>
        /// 门店ID
        /// </summary>
        public int store_id { get; set; }

        /// <summary>
        /// 档案信息列表
        /// </summary>
        public List<archives> archives {get;set;}
    }

    /// <summary>
    /// 档案信息
    /// </summary>
    public class archives
    {
        private System.String _name;
        /// <summary>
        /// 姓名
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.String _id_card;
        /// <summary>
        /// 证件号
        /// </summary>
        public System.String id_card { get { return this._id_card; } set { this._id_card = value?.Trim(); } }

        private System.String _phone;
        /// <summary>
        /// 联系电话
        /// </summary>
        public System.String phone { get { return this._phone; } set { this._phone = value?.Trim(); } }

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

        private System.String _to_director_phone;
        /// <summary>
        /// 现负责人手机号
        /// </summary>
        public System.String to_director_phone { get { return this._to_director_phone; } set { this._to_director_phone = value?.Trim(); } }

        private System.String _to_director;
        /// <summary>
        /// 现负责人
        /// </summary>
        public System.String to_director { get { return this._to_director; } set { this._to_director = value?.Trim(); } }

        private System.Int16? _age;
        /// <summary>
        /// 年龄
        /// </summary>
        public System.Int16? age { get { return this._age; } set { this._age = value ?? default(System.Int16); } }

        private System.DateTime? _birthday;
        /// <summary>
        /// 生日
        /// </summary>
        public System.DateTime? birthday { get { return this._birthday; } set { this._birthday = value ?? default(System.DateTime); } }

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

        private System.Decimal _balance;
        /// <summary>
        /// 余额
        /// </summary>
        public System.Decimal balance { get { return this._balance; } set { this._balance = value; } }
    }
}
