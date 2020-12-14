using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Business
{
    /// <summary>
    /// 报损报溢
    /// </summary>
    public class LossOverflow : bus_loss_overflow
    {
        /// <summary>
        /// 明细
        /// </summary>
        public List<bus_loss_overflow_detials> loss_Overflow_Detials { get; set; }

        /// <summary>
        /// 审核记录
        /// </summary>
        public List<verifyInfo> verifyInfos { get; set; }
    }
}
