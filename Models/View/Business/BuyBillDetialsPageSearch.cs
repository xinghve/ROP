using Models.View.Public;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Business
{
    /// <summary>
    /// 采购单分页查询
    /// </summary>
    public class BuyBillDetialsPageSearch : Search
    {
        /// <summary>
        /// 采购单号
        /// </summary>
        public string bill_no { get; set; }
    }
}
