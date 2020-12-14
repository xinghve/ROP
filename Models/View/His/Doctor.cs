using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.His
{
    /// <summary>
    /// 医生信息
    /// </summary>
    public class Doctor
    {
        /// <summary>
        /// 类别ID
        /// </summary>
        public int typeid { get; set; }

        /// <summary>
        /// 职工ID
        /// </summary>
        public int empid { get; set; }

        /// <summary>
        /// 职工名称
        /// </summary>
        public string empname { get; set; }

        /// <summary>
        /// 预约标志
        /// </summary>
        public short orderflag { get; set; }

        /// <summary>
        /// 限号数
        /// </summary>
        public short limitnumbers { get; set; }

        /// <summary>
        /// 限约数
        /// </summary>
        public short limitorders { get; set; }

        /// <summary>
        /// 诊室ID
        /// </summary>
        public int roomid { get; set; }

        /// <summary>
        /// 诊室
        /// </summary>
        public string roomname { get; set; }

        /// <summary>
        /// 叫号标志
        /// </summary>
        public short callflag { get; set; }

        /// <summary>
        /// 星期一
        /// </summary>
        public string monday { get; set; }

        /// <summary>
        /// 星期二
        /// </summary>
        public string tuesday { get; set; }

        /// <summary>
        /// 星期三
        /// </summary>
        public string wednesday { get; set; }

        /// <summary>
        /// 星期四
        /// </summary>
        public string thursday { get; set; }

        /// <summary>
        /// 星期五
        /// </summary>
        public string friday { get; set; }

        /// <summary>
        /// 星期六
        /// </summary>
        public string saturday { get; set; }

        /// <summary>
        /// 星期天
        /// </summary>
        public string sunday { get; set; }

        /// <summary>
        /// 排班时间段
        /// </summary>
        public List<his_scheduletimes> scheduletimes { get; set; }
    }
}
