using Models.View.Public;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Reports
{
    /// <summary>
    ///  活动优惠券分页查询
    /// </summary>
    public class AcCouponSearch:Search
    {
        /// <summary>
        /// 活动名
        /// </summary>
        public string activity_name { get; set; }

        private System.String _order_coupon = "id";
        /// <summary>
        /// 优惠券排序字段
        /// </summary>
        public System.String order_coupon { get { return this._order_coupon; } set { this._order_coupon = value?.Trim(); } }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? startTime { get; set ; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? endTime { get; set;  }

    }
}
