using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Business
{
    /// <summary>
    /// 资产折旧
    /// </summary>
    public class AssetsDepreciation : bus_assets_depreciation
    {
        /// <summary>
        /// 折旧单明细
        /// </summary>
        public List<bus_assets_depreciation_detials> assets_Depreciation_Detials { get; set; }
    }
}
