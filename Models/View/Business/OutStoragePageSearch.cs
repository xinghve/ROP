using Models.View.Public;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Business
{
    /// <summary>
    /// 出库分页查询
    /// </summary>
    public class OutStoragePageSearch : Search
    {
        /// <summary>
        /// 出库单号
        /// </summary>
        public string bill_no { get; set; }

        /// <summary>
        /// 类型ID（-1=所有）
        /// </summary>
        public int type_id { get; set; }

        /// <summary>
        /// 门店ID（-1=所有）
        /// </summary>
        public int store_id { get; set; }

        /// <summary>
        /// 状态（-1=所有； 7=已取消； 42=已出库）
        /// </summary>
        public short state { get; set; }
    }
}
