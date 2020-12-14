using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Public
{
    /// <summary>
    /// 状态
    /// </summary>
    public class CardState
    {
        /// <summary>
        /// 就诊卡是否可积分
        /// </summary>
        public short integral_card { get; set; }
        /// <summary>
        /// 是否存在实体就诊卡
        /// </summary>
        public short physical_card { get; set; }
    }
}
