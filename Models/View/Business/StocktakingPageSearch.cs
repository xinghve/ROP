using Models.View.Public;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Business
{
    /// <summary>
    /// 盘点单分页查询
    /// </summary>
    public class StocktakingPageSearch: Search
    {
        /// <summary>
        /// 单号
        /// </summary>
        public string no { get; set; }

        /// <summary>
        /// 盘点年月（开始）
        /// </summary>
        public DateTime? start_date { get; set; }

        /// <summary>
        /// 盘点年月（结束）
        /// </summary>
        public DateTime? end_date { get; set; }

        /// <summary>
        /// 门店ID
        /// </summary>
        public int store_id { get; set; }

        /// <summary>
        /// 状态（45=待盘点；46=盘点中；24=待处理；33=部分处理；15=已完成；7=撤销；-1=所有）
        /// </summary>
        public short state { get; set; }
    }
}
