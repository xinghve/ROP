using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Business
{
    /// <summary>
    /// 调拨
    /// </summary>
    public class Transfer : bus_transfer_bill
    {
        /// <summary>
        /// 调拨明细
        /// </summary>
        public List<bus_transfer_bill_detials> transfer_Bill_Detials { get; set; }

        /// <summary>
        /// 调拨固定资产明细
        /// </summary>
        public List<transfer_Bill_Detials_assets> transfer_Bill_Detials_assets { get; set; }

        /// <summary>
        /// 审核记录
        /// </summary>
        public List<verifyInfo> verifyInfos { get; set; }
    }

    /// <summary>
    /// 固定资产调拨
    /// </summary>
    public class transfer_Bill_Detials_assets: bus_transfer_bill_detials
    {
        /// <summary>
        /// 固定资产ID
        /// </summary>
        public List<int> assets_ids { get; set; }

        /// <summary>
        /// 固定资产
        /// </summary>
        public List<bus_assets> bus_Assets { get; set; }
    }
}
