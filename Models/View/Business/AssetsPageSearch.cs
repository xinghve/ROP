using Models.View.Public;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Business
{
    /// <summary>
    /// 固定资产分页
    /// </summary>
    public class AssetsPageSearch: Search
    {
        private System.String _no;
        /// <summary>
        /// 编号
        /// </summary>
        public System.String no { get { return this._no; } set { this._no = value?.Trim(); } }

        private System.Int32 _std_item_id;
        /// <summary>
        /// 物资基础项目ID
        /// </summary>
        public System.Int32 std_item_id { get { return this._std_item_id; } set { this._std_item_id = value; } }

        private System.Int16 _state;
        /// <summary>
        /// 状态（30=未使用；31=使用中；44=维修中；32=已报废；41=调拨中；47=已报损；-1=所有）
        /// </summary>
        public System.Int16 state { get { return this._state; } set { this._state = value; } }

        private System.Int16 _type_id;
        /// <summary>
        /// 类型ID
        /// </summary>
        public System.Int16 type_id { get { return this._type_id; } set { this._type_id = value; } }

        private System.Int32? _store_id;
        /// <summary>
        /// 使用门店ID
        /// </summary>
        public System.Int32? store_id { get { return this._store_id; } set { this._store_id = value ?? default(System.Int32); } }
    }
}
