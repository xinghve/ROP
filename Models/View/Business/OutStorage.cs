using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Business
{
    /// <summary>
    /// 出库
    /// </summary>
    public class OutStorage : bus_out_storage
    {
        /// <summary>
        /// 出库明细
        /// </summary>
        public List<bus_out_storage_detials> out_Storage_Detials { get; set; }

        /// <summary>
        /// 出库明细固定资产
        /// </summary>
        public List<bus_out_storage_detials_assets> out_Storage_Detials_Assets { get; set; }
    }

    /// <summary>
    /// 出库固定资产
    /// </summary>
    public class bus_out_storage_detials_assets : bus_out_storage_detials
    {
        /// <summary>
        /// 固定资产
        /// </summary>
        public List<bus_assets> bus_Assets { get; set; }
    }
}
