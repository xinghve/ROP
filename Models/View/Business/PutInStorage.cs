using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Business
{
    /// <summary>
    /// 入库
    /// </summary>
    public class PutInStorage : bus_put_in_storage
    {
        /// <summary>
        /// 入库明细
        /// </summary>
        public List<bus_put_in_storage_detials> put_In_Storage_Detials { get; set; }

        /// <summary>
        /// 入库固定资产
        /// </summary>
        public List<bus_put_in_storage_detials_assets> put_In_Storage_Detials_Assets { get; set; }

        /// <summary>
        /// 入库固定资产
        /// </summary>
        public List<bus_put_in_assets> put_In_Assets { get; set; }
    }

    /// <summary>
    /// 入库明细固定资产
    /// </summary>
    public class bus_put_in_storage_detials_assets : bus_put_in_storage_detials
    {
        /// <summary>
        /// 固定资产
        /// </summary>
        public List<bus_assets> bus_Assets { get; set; }
    }
}
