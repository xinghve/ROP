using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Reports
{
    /// <summary>
    /// 医疗板块报表
    /// </summary>
    public class MainHealthModel
    {
        private System.Decimal? _register_money;
        /// <summary>
        /// 总挂号金额
        /// </summary>
        public System.Decimal? register_money { get { return this._register_money; } set { this._register_money = value ?? default(System.Decimal); } }

        private System.Int32 _his_register;
        /// <summary>
        ///挂号数
        /// </summary>
        public System.Int32 his_registerCount { get { return this._his_register; } set { this._his_register = value; } }

        private System.Int32 _sign_in;
        /// <summary>
        ///签到数
        /// </summary>
        public System.Int32 sign_inCount { get { return this._sign_in; } set { this._sign_in = value; } }

        private System.Int32 _doctorDutyCount;
        /// <summary>
        ///当值医生人数
        /// </summary>
        public System.Int32 doctorDutyCount { get { return this._doctorDutyCount; } set { this._doctorDutyCount = value; } }

        /// <summary>
        /// 当值医生
        /// </summary>
        public List<string> doctor_name { get; set; }
    }

    /// <summary>
    /// 医生排名
    /// </summary>
    public class doctorList
    {
        /// <summary>
        /// 医生List
        /// </summary>
        public List<drList> drList { get; set; }

        /// <summary>
        /// 排名Lsit
        /// </summary>
        public List<topList> topList { get; set; }

        /// <summary>
        /// 获取年List
        /// </summary>
        public List<yearModel> yearModel { get; set; }
    }

    /// <summary>
    /// 排名
    /// </summary>
    public class topList
    {
        /// <summary>
        /// 医生名称
        /// </summary>
        public string doctorName { get; set; }

        /// <summary>
        /// 科室id
        /// </summary>
        public int deptId { get; set; }

        /// <summary>
        /// 医生id
        /// </summary>
        public int doctorId { get; set; }

        /// <summary>
        /// 挂号总数
        /// </summary>
        public int registerCount { get; set; }

        /// <summary>
        /// 天或月
        /// </summary>
        public int value { get; set; }

    }
    /// <summary>
    /// 医生List
    /// </summary>
    public class drList
    {
        /// <summary>
        /// 医生名
        /// </summary>
        public string doctorName { get; set; }

        /// <summary>
        /// 医生id
        /// </summary>
        public int doctorId { get; set; }
    }

    /// <summary>
    /// 获取年月份List
    /// </summary>
    public class yearModel
    {
        public int year { get; set; }
        /// <summary>
        /// 获取月
        /// </summary>
        public List<int> months { get; set; }
    }
}
