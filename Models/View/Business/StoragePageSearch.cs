using Models.View.Public;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Business
{
    /// <summary>
    /// 库存
    /// </summary>
    public class StoragePageSearch : Search
    {
        /// <summary>
        /// 门店ID
        /// </summary>
        public int store_id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 类型ID
        /// </summary>
        public short type_id { get; set; }

        private short _catalog_type = 0;
        /// <summary>
        /// 目录类型(0=所有；1=目录；2=类型)
        /// </summary>
        public short catalog_type { get { return this._catalog_type; } set { this._catalog_type = value; } }
    }
}
