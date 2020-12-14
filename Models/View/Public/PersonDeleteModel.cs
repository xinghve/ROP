using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Public
{
    /// <summary>
    /// 删除人员所需
    /// </summary>
     public class PersonDeleteModel
    {
        /// <summary>
        /// 人员表
        /// </summary>
        public p_employee p_employee { get; set; }

        /// <summary>
        /// 人员idList
        /// </summary>
        public List<int> pList { get; set; }
    }
}
