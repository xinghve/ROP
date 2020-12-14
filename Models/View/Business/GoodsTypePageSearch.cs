using Models.View.Public;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Business
{
    /// <summary>
    /// 物资分类分页查询
    /// </summary>
    public class GoodsTypePageSearch : Search
    {
        /// <summary>
        /// 物资分类名称
        /// </summary>
        public string text { get; set; }

        /// <summary>
        /// 目录ID
        /// </summary>
        public short catalog_id { get; set; }
    }
}
