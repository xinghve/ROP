using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Business
{
    /// <summary>
    /// 申请单
    /// </summary>
    public class ApplyBill : bus_buy_apply
    {
        /// <summary>
        /// 申请单明细
        /// </summary>
        public List<bus_buy_apply_detials> bus_Buy_Apply_Detials { get; set; }
    }
    
}
