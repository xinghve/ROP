using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Business
{
    /// <summary>
    /// 固定资产报废
    /// </summary>
    public class AssetsScrap : r_assets_scrap
    {
        /// <summary>
        /// 审核记录
        /// </summary>
        public List<verifyInfo> verifyInfos { get; set; }
    }
}
