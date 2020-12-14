using Models.View.Public;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Business
{
    /// <summary>
    /// 资产折旧分页查询
    /// </summary>
    public class AssetsDepreciationPageSearch : Search
    {
        /// <summary>
        /// 资产折旧单号
        /// </summary>
        public string no { get; set; }

        /// <summary>
        /// 折旧年月（开始）
        /// </summary>
        public DateTime? start_date { get; set; }

        /// <summary>
        /// 折旧年月（结束）
        /// </summary>
        public DateTime? end_date { get; set; }

        /// <summary>
        /// 门店ID
        /// </summary>
        public int store_id { get; set; }
    }
}
