using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Public
{
    /// <summary>
    /// 器械设备分页数据
    /// </summary>
    public class EquipmentPages : p_equipment
    {
        /// <summary>
        /// 设备对应项目规格
        /// </summary>
        public List<p_equipment_itemspec> equipment_Itemspecs { get; set; }
    }
}
