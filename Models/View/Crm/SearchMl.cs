using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Crm
{
    /// <summary>
    /// 分页搜索数据
    /// </summary>
    public class SearchMl
    {
        private System.String _name;
        /// <summary>
        /// 搜索名
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.Int32 _storeId = 0;
        /// <summary>
        /// 门店id
        /// </summary>
        public System.Int32 storeId { get { return this._storeId; } set { this._storeId = value; } }

        private System.Int32 _orgId = 0;
        /// <summary>
        /// 机构id
        /// </summary>
        public System.Int32 orgId { get { return this._orgId; } set { this._orgId = value; } }

        private System.String _order = "id";
        /// <summary>
        /// 排序字段
        /// </summary>
        public System.String order { get { return this._order; } set { this._order = value?.Trim(); } }

        private System.Int32 _orderType = 0;
        /// <summary>
        /// 排序方式
        /// </summary>
        public System.Int32 orderType { get { return this._orderType; } set { this._orderType = value; } }

        private System.Int32 _limit=10;
        /// <summary>
        /// 单页数据条数
        /// </summary>
        public System.Int32 limit { get { return this._limit; } set { this._limit = value; } }

        private System.Int32 _page=1;
        /// <summary>
        /// 查询第几页
        /// </summary>
        public System.Int32 page { get { return this._page; } set { this._page = value; } }

        private System.DateTime? _startTime;
        /// <summary>
        /// 开始时间
        /// </summary>
        public System.DateTime? startTime { get { return this._startTime; } set { this._startTime = value; } }

        private System.DateTime? _endTime;
        /// <summary>
        /// 结束时间
        /// </summary>
        public System.DateTime? endTime { get { return this._endTime; } set { this._endTime = value; } }
        
    }
}
