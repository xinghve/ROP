using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Mobile
{
    /// <summary>
    /// 消费
    /// </summary>
    public class Spend
    {
        /// <summary>
        /// 消费日期
        /// </summary>
        public DateTime date { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal money { get; set; }

        /// <summary>
        /// 余额
        /// </summary>
        public decimal? balance { get; set; }

        /// <summary>
        /// 门店
        /// </summary>
        public string store { get; set; }

        /// <summary>
        /// 对应结算ID
        /// </summary>
        public int? balanceid { get; set; }

        /// <summary>
        /// 是否挂号
        /// </summary>
        public short? is_register { get; set; }
    }
}
