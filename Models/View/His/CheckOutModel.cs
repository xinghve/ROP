using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.His
{
    /// <summary>
    /// 收账单
    /// </summary>
    public class CheckOutModel
    {
        /// <summary>
        /// 结算单id数组
        /// </summary>
        public List<int> balanceArray { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime startTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime endTime { get; set; }
        /// <summary>
        /// 门店id
        /// </summary>
        public int storeId { get; set; }
    }

    /// <summary>
    /// 收账单分页
    /// </summary>
    public class CheckOutModelPage:f_checkout
    {
        /// <summary>
        /// 门店
        /// </summary>
        public string storeName { get; set; }
    }

    /// <summary>
    /// 待扎帐分页
    /// </summary>
    public class FalanceModel: f_balance
    {
        /// <summary>
        /// 门店
        /// </summary>
        public string storeName { get; set; }

        /// <summary>
        /// 客户名
        /// </summary>
        public string customerName { get; set; }

    }



}
