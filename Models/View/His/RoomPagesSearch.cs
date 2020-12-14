using Models.View.Public;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.His
{
    /// <summary>
    /// 医疗室分页查询
    /// </summary>
    public class RoomPagesSearch : Search
    {
        /// <summary>
        /// 门店ID
        /// </summary>
        public int store_id { get; set; }

        private System.String _name;
        /// <summary>
        /// 名称
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }
    }
}
