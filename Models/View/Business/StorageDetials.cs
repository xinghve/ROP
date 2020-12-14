using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Business
{
    /// <summary>
    /// 库存明细
    /// </summary>
    public class StorageDetials
    {
        /// <summary>
        /// 物资基础项目ID
        /// </summary>
        public int std_item_id { get; set; }

        /// <summary>
        /// 厂家ID
        /// </summary>
        public int manufactor_id { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        public string spec { get; set; }

        /// <summary>
        /// 门店ID
        /// </summary>
        public int store_id { get; set; }

        private System.Int32 _use_num;
        /// <summary>
        /// 可用数量
        /// </summary>
        public System.Int32 use_num { get { return this._use_num; } set { this._use_num = value; } }
    }
}
