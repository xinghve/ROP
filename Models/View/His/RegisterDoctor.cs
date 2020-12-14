using System.Collections.Generic;

namespace Models.View.His
{
    /// <summary>
    /// 挂号医生
    /// </summary>
    public class RegisterDoctor
    {
        /// <summary>
        /// 星期
        /// </summary>
        public string Week { get; set; }

        /// <summary>
        /// 医生ID
        /// </summary>
        public int empid { get; set; }
        /// <summary>
        /// 医生名称
        /// </summary>
        public string empname { get; set; }

        /// <summary>
        /// 是否预约（true=预约 false=挂号）
        /// </summary>
        public bool is_order { get; set; }

        /// <summary>
        /// 时间段
        /// </summary>
        public List<Scheduletimes> list { get; set; }
    }
}
