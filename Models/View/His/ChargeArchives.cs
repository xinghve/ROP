using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.His
{
    /// <summary>
    /// 结算客户
    /// </summary>
    public class ChargeArchives
    {
        /// <summary>
        /// 档案ID
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 客户姓名
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 客户性别
        /// </summary>
        public string sex { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string phone { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public short? age { get; set; }

        /// <summary>
        /// 应收金额
        /// </summary>
        public decimal money { get; set; }

        private System.Decimal? _discount_rate;
        /// <summary>
        /// 折扣率
        /// </summary>
        public System.Decimal? discount_rate { get { return this._discount_rate; } set { this._discount_rate = value ?? default(System.Decimal); } }

        private System.String _doctorname;
        /// <summary>
        /// 开单医生
        /// </summary>
        public System.String doctorname { get { return this._doctorname; } set { this._doctorname = value?.Trim(); } }

        private System.String _deptname;
        /// <summary>
        /// 就诊科室
        /// </summary>
        public System.String deptname { get { return this._deptname; } set { this._deptname = value?.Trim(); } }
        /// <summary>
        /// 优惠券金额
        /// </summary>
        public decimal coupon { get; set; }
        /// <summary>
        /// 余额
        /// </summary>
        public decimal balance { get; set; }
    }
}
