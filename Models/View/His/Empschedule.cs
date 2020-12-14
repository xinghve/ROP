using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.His
{
    /// <summary>
    /// 排班
    /// </summary>
    public class Empschedule
    {
        /// <summary>
        /// 门店ID
        /// </summary>
        public int store_id { get; set; }

        /// <summary>
        /// 部门ID
        /// </summary>
        public int deptid { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string deptname { get; set; }

        /// <summary>
        /// 预约开始时间
        /// </summary>
        public DateTime orderbegin { get; set; }

        /// <summary>
        /// 预约结束时间
        /// </summary>
        public DateTime? orderend { get; set; }

        /// <summary>
        /// 医生列表
        /// </summary>
        public List<Doctor> doctors { get; set; }
    }
}
