using Models.View.Public;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Business
{
    /// <summary>
    /// 待盘点物资分页查询
    /// </summary>
    public class WaitStocktakingPageSearch: Search
    {
        private System.String _name;
        /// <summary>
        /// 名称
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.Int16 _type_id;
        /// <summary>
        /// 类型ID
        /// </summary>
        public System.Int16 type_id { get { return this._type_id; } set { this._type_id = value; } }

        private System.Int32 _manufactor_id;
        /// <summary>
        /// 厂家ID
        /// </summary>
        public System.Int32 manufactor_id { get { return this._manufactor_id; } set { this._manufactor_id = value; } }

        /// <summary>
        /// 门店ID
        /// </summary>
        public int store_id { get; set; }

        private short _catalog_type = 0;
        /// <summary>
        /// 目录类型(0=所有；1=目录；2=类型)
        /// </summary>
        public short catalog_type { get { return this._catalog_type; } set { this._catalog_type = value; } }
    }
}
