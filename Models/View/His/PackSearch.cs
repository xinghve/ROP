using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.His
{
    /// <summary>
    /// 套餐分页
    /// </summary>
    public class PackSearch
    {
        /// <summary>
        /// 套餐类型2项目5检查6检验
        /// </summary>
        public int typeId { get; set; }

        private System.Int32 _storeId;
        /// <summary>
        /// 门店id
        /// </summary>
        public System.Int32 storeId { get { return this._storeId; } set { this._storeId = value; } }

        private System.String _name;
        /// <summary>
        /// 搜索名
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.String _order = "packid";
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

    /// <summary>
    /// 套餐项目
    /// </summary>
    public class PackItemSearch
    {
        /// <summary>
        /// 申请id
        /// </summary>
        public int applyid { get; set; }

        private System.String _packId;
        /// <summary>
        /// 包ID
        /// </summary>
        public System.String packId { get { return this._packId; } set { this._packId = value?.Trim(); } }


        private System.Int32 _storeId;
        /// <summary>
        /// 门店id
        /// </summary>
        public System.Int32 storeId { get { return this._storeId; } set { this._storeId = value; } }

        private System.String _name;
        /// <summary>
        /// 搜索名
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.String _order = "packid";
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
