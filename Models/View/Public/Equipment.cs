using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Public
{
    /// <summary>
    /// 器械设备
    /// </summary>
    public class Equipment
    {
        /// <summary>
        /// 主表
        /// </summary>
        public p_equipment equipment { get; set; }

        /// <summary>
        /// 设备对应项目规格
        /// </summary>
        public List<p_equipment_itemspec> equipment_Itemspecs { get; set; }
    }
}
