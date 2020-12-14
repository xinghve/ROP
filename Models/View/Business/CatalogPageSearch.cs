using Models.View.Public;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Business
{
    /// <summary>
    /// 物资目录分页查询
    /// </summary>
    public class CatalogPageSearch : Search
    {
        /// <summary>
        /// 物资目录名称
        /// </summary>
        public string text { get; set; }

        /// <summary>
        /// 属性ID
        /// </summary>
        public short property_id { get; set; }
    }
}
