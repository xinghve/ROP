using Models.View.Public;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Business
{
    /// <summary>
    /// 库存明细
    /// </summary>
    public class StorageDetialsPageSearch : Search
    {
        /// <summary>
        /// 库存ID
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 厂家ID
        /// </summary>
        public int manufactor_id { get; set; }
    }
}
