using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.His
{
    /// <summary>
    /// 医疗室
    /// </summary>
    public class Room : p_room
    {
        /// <summary>
        /// 医疗室对应项目规格
        /// </summary>
        public List<room_itemspec> room_itemspecs_list { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class room_itemspec : p_room_itemspec
    {
        /// <summary>
        /// 单位
        /// </summary>
        public string salseunit { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal sale_price { get; set; }
    }
}
