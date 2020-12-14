using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Public
{
    /// <summary>
    /// 诊室所属科室
    /// </summary>
    public class ConsRoomDept : p_cons_room
    {
        /// <summary>
        /// 科室ID
        /// </summary>
        public List<dept> depts { get; set; }
    }

    /// <summary>
    /// 科室
    /// </summary>
    public class dept
    {
        /// <summary>
        /// ID
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }
    }
}
