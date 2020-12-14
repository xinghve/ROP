using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.His
{
    /// <summary>
    /// 收费
    /// </summary>
    public class Charge
    {
        /// <summary>
        /// 客户ID
        /// </summary>
        public int archives_id { get; set; }

        /// <summary>
        /// 是否使用赠送金额3否 2是
        /// </summary>
        public int isUse { get; set; }
    }
}
