using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Reports
{
    /// <summary>
    /// 活动优惠券分页统计
    /// </summary>
    public class AcCouponModel
    {
        /// <summary>
        /// 活动id
        /// </summary>
        public int activity_id { get; set; }
        /// <summary>
        /// 活动名称
        /// </summary>
        public string activity_name { get; set; }
       
        /// <summary>
        /// 优惠券总数
        /// </summary>
        public int? coupon_total { get; set; }

        /// <summary>
        /// 使用率
        /// </summary>
        public decimal use_rate { get; set; }

        /// <summary>
        /// 活动新引入客户数
        /// </summary>
        public int archives_count { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? start_time { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? end_time { get; set; }

        /// <summary>
        /// 活动优惠总金额
        /// </summary>
        public decimal? total_money { get; set; }

        /// <summary>
        /// 优惠券List
        /// </summary>
        public List<NewCoupon> couponList { get; set; }
    }

    /// <summary>
    /// 活动优惠券总数 
    /// </summary>
    public class CouponTot
    {
        /// <summary>
        /// 活动id
        /// </summary>
        public int activity_id { get; set; }

        /// <summary>
        /// 总数
        /// </summary>
        public int? sum_coupon { get; set; }

        /// <summary>
        /// 活动优惠总金额
        /// </summary>
        public decimal? total_money { get; set; }
    }

    /// <summary>
    /// 优惠券分类
    /// </summary>
    public class NewCoupon
    {
        /// <summary>
        /// 优惠券id
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 活动id
        /// </summary>
        public int activity_id { get; set; }

        /// <summary>
        /// 优惠券总张数
        /// </summary>
        public int? new_sum_coupon { get; set; }

        /// <summary>
        /// 优惠券使用总张数
        /// </summary>
        public int use_total { get; set; }

        private System.String _head;
        /// <summary>
        /// 券码头
        /// </summary>
        public System.String head { get { return this._head; } set { this._head = value?.Trim(); } }

        private System.String _remarks;
        /// <summary>
        /// 备注
        /// </summary>
        public System.String remarks { get { return this._remarks; } set { this._remarks = value?.Trim(); } }

        /// <summary>
        /// 优惠券总金额
        /// </summary>
        public decimal? coupon_total_money { get; set; }

        private System.Decimal? _money;
        /// <summary>
        /// 优惠券面值金额
        /// </summary>
        public System.Decimal? money { get { return this._money; } set { this._money = value ?? default(System.Decimal); } }

        /// <summary>
        /// 优惠券使用总金额
        /// </summary>
        public decimal use_total_money { get; set; }


    }



}
