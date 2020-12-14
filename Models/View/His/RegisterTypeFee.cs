using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.His
{
    /// <summary>
    /// 挂号费用
    /// </summary>
    public class RegisterTypeFee
    {
        /// <summary>
        /// 类别ID
        /// </summary>
        public int typeid { get; set; }

        /// <summary>
        /// 次数
        /// </summary>
        public short quantiry { get; set; }

        /// <summary>
        /// 预约标志
        /// </summary>
        public short orderflag { get; set; }

        /// <summary>
        /// 售价
        /// </summary>
        public decimal sale_price { get; set; }
    }
}
