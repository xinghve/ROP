using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Mobile
{
    /// <summary>
    /// 挂号记录（客户端）
    /// </summary>
    public class ArcRegisterRecord
    {
        /// <summary>
        /// 挂号时间
        /// </summary>
        public DateTime reg_datetime { get; set; }

        /// <summary>
        /// 客户姓名
        /// </summary>
        public string arc_name { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string phone { get; set; }

        /// <summary>
        /// 科室
        /// </summary>
        public string dept { get; set; }

        /// <summary>
        /// 医生
        /// </summary>
        public string doctor { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime create_datetime { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal money { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public TimeSpan begintime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public TimeSpan endtime { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public short state_id { get; set; }
    }
}
