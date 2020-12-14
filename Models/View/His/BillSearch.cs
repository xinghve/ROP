using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.His
{
    /// <summary>
    /// 单据查询
    /// </summary>
    public class BillSearch
    {
        private System.String _name;
        /// <summary>
        /// 搜索名
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.String _order = "bill_id";
        /// <summary>
        /// 排序字段
        /// </summary>
        public System.String order { get { return this._order; } set { this._order = value?.Trim(); } }

        private System.Int32 _orderType = 0;
        /// <summary>
        /// 排序方式
        /// </summary>
        public System.Int32 orderType { get { return this._orderType; } set { this._orderType = value; } }

        private System.Int32 _limit = 10;
        /// <summary>
        /// 单页数据条数
        /// </summary>
        public System.Int32 limit { get { return this._limit; } set { this._limit = value; } }

        private System.Int32 _page = 1;
        /// <summary>
        /// 查询第几页
        /// </summary>
        public System.Int32 page { get { return this._page; } set { this._page = value; } }
    }
}
