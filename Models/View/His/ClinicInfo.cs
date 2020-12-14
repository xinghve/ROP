using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.His
{
    /// <summary>
    /// 
    /// </summary>
    public class ClinicInfo
    {
        private System.Int32 _regid;
        /// <summary>
        /// 挂号ID
        /// </summary>
        public System.Int32 regid { get { return this._regid; } set { this._regid = value; } }

        private System.Int16 _order_no;
        /// <summary>
        /// 就诊号序
        /// </summary>
        public System.Int16 order_no { get { return this._order_no; } set { this._order_no = value; } }

        /// <summary>
        /// 时间段
        /// </summary>
        public string times { get; set; }

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
        /// 档案ID
        /// </summary>
        public int archives_id { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public short? age { get; set; }

        private System.Decimal? _discount_rate;
        /// <summary>
        /// 折扣率
        /// </summary>
        public System.Decimal? discount_rate { get { return this._discount_rate; } set { this._discount_rate = value ?? default(System.Decimal); } }

        /// <summary>
        /// 就诊ID
        /// </summary>
        public int clinicid { get; set; }

        private System.String _image_url;
        /// <summary>
        /// 头像图片地址
        /// </summary>
        public System.String image_url { get { return this._image_url; } set { this._image_url = value?.Trim(); } }

        /// <summary>
        /// 赠送余额
        /// </summary>
        public decimal coupon { get; set; }
        /// <summary>
        /// 余额
        /// </summary>
        public decimal balance { get; set; }
    }
}
