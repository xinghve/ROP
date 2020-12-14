using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Public
{
    /// <summary>
    /// 基础项目
    /// </summary>
    public class StdItem : p_std_item
    {
        /// <summary>
        /// 基础项目明细
        /// </summary>
        public List<p_std_item_detials> std_Item_Detials { get; set; }
    }
}
