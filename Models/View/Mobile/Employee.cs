using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Mobile
{
    /// <summary>
    /// 人员
    /// </summary>
    public class Employee
    {
        /// <summary>
        /// ID
        /// </summary>
        public int employee_id { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 身份证
        /// </summary>
        public string id_no { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string address { get; set; }
    }
}
