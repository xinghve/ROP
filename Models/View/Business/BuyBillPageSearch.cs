using Models.View.Public;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Business
{
    /// <summary>
    /// 采购单分页查询
    /// </summary>
    public class BuyBillPageSearch : Search
    {
        /// <summary>
        /// 采购单号
        /// </summary>
        public string bill_no { get; set; }

        /// <summary>
        /// 状态（-1=所有； 7=已取消； 15=已完成； 39=采购中）
        /// </summary>
        public short state { get; set; }

        /// <summary>
        /// 厂家ID（-1=所有）
        /// </summary>
        public int manufactor_id { get; set; }
    }
}
