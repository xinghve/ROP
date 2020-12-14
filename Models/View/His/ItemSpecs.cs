using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.His
{
    /// <summary>
    /// 项目规格
    /// </summary>
    public class ItemSpecs
    {
        /// <summary>
        /// 售价
        /// </summary>
        public decimal sale_price { get; set; }

        /// <summary>
        /// 售价单位
        /// </summary>
        public string salseunit { get; set; }

        /// <summary>
        /// 规格ID
        /// </summary>
        public int specid { get; set; }

        /// <summary>
        /// 规格名称
        /// </summary>
        public string specname { get; set; }
    }
}
