using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.His
{
    /// <summary>
    /// 排班时间段（包含挂号费、已挂号数、已预约数...）
    /// </summary>
    public class Scheduletimes
    {
        /// <summary>
        /// 排班ID
        /// </summary>
        public int scheduleid { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string deptname { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        public short no { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public TimeSpan begintime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public TimeSpan endtime { get; set; }

        /// <summary>
        /// 已挂号数
        /// </summary>
        public short numbers { get; set; }

        /// <summary>
        /// 已预约数
        /// </summary>
        public short orders { get; set; }

        /// <summary>
        /// 限号数
        /// </summary>
        public short limitnumbers { get; set; }

        /// <summary>
        /// 限约数
        /// </summary>
        public short limitorders { get; set; }

        /// <summary>
        /// 诊室
        /// </summary>
        public string roomname { get; set; }

        /// <summary>
        /// 类别名称
        /// </summary>
        public string typename { get; set; }

        /// <summary>
        /// 挂号费
        /// </summary>
        public decimal price { get; set; }

        /// <summary>
        /// 类别id
        /// </summary>
        public int type_id { get; set; }

        /// <summary>
        /// 诊室id
        /// </summary>
        public int room_id { get; set; }

        /// <summary>
        /// 部门id
        /// </summary>
        public int dept_id { get; set; }

        /// <summary>
        /// 诊室位置
        /// </summary>
        public string position { get; set; }
    }
}
