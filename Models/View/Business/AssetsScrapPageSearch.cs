using Models.View.Public;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Business
{
    /// <summary>
    /// 固定资产报废
    /// </summary>
    public class AssetsScrapPageSearch : Search
    {
        /// <summary>
        /// 资产报废单号
        /// </summary>
        public string no { get; set; }

        /// <summary>
        /// 资产名称、规格或编号
        /// </summary>
        public string str { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public short state { get; set; }

        /// <summary>
        /// 门店ID
        /// </summary>
        public int store_id { get; set; }
    }
}
