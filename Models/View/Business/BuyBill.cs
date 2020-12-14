using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Business
{
    /// <summary>
    /// 采购单
    /// </summary>
    public class BuyBill : bus_buy_bill
    {

        private System.Decimal? _total_price;
        /// <summary>
        /// 总价
        /// </summary>
        public System.Decimal? total_price { get { return this._total_price; } set { this._total_price = value ?? default(System.Decimal); } }

        /// <summary>
        /// 采购明细列表
        /// </summary>
        public List<bus_buy_bill_detials> bus_Buy_Bill_Detials { get; set; }

        /// <summary>
        /// 采购单对应申请单明细
        /// </summary>
        public List<bus_buy_bill_to_apply_detials> buy_Bill_To_Apply_Detials { get; set; }
    }
}
