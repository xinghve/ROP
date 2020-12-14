using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Public
{
    /// <summary>
    /// 基础物资明细
    /// </summary>
    public class StdItemDetials
    {
        /// <summary>
        /// 规格
        /// </summary>
        public string spec { get; set; }

        /// <summary>
        /// 明细
        /// </summary>
        public List<p_std_item_detials> std_Item_Detials { get; set; }

        /// <summary>
        /// 可用数量
        /// </summary>
        public int num { get; set; }
    }
}
