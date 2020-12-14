using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.His
{
    /// <summary>
    /// 结账搜索model
    /// </summary>
    public class CheckOutSearch
    {
        private System.Int32 _storeId = 0;
        /// <summary>
        /// 门店id（-1所有门店）
        /// </summary>
        public System.Int32 storeId { get { return this._storeId; } set { this._storeId = value; } }
        

        private System.String _order = "settle_accounts_id";
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


        private System.DateTime? _startTime;
        /// <summary>
        /// 结账开始时间
        /// </summary>
        public System.DateTime? startTime { get { return this._startTime; } set { this._startTime = value; } }

        private System.DateTime? _endTime;
        /// <summary>
        /// 结账结束时间
        /// </summary>
        public System.DateTime? endTime { get { return this._endTime; } set { this._endTime = value; } }
    }

    /// <summary>
    /// 结算分页查询
    /// </summary>
    public class FalanceSearch
    {
        private System.Int32 _deptId = 0;
        /// <summary>
        /// 科室id
        /// </summary>
        public System.Int32 deptId { get { return this._deptId; } set { this._deptId = value; } }

        private System.Int32 _doctorId = 0;
        /// <summary>
        /// 医生id
        /// </summary>
        public System.Int32 doctorId { get { return this._doctorId; } set { this._doctorId = value; } }

        private System.Int32 _storeId = 0;
        /// <summary>
        /// 门店id（-1所有门店）
        /// </summary>
        public System.Int32 storeId { get { return this._storeId; } set { this._storeId = value; } }


        private System.String _order = "balanceid";
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

        private System.DateTime? _startTime;
        /// <summary>
        /// 结算开始时间
        /// </summary>
        public System.DateTime? startTime { get { return this._startTime; } set { this._startTime = value; } }

        private System.DateTime? _endTime;
        /// <summary>
        /// 结算结束时间
        /// </summary>
        public System.DateTime? endTime { get { return this._endTime; } set { this._endTime = value; } }

        private System.Int32 _checkId = 0;
        /// <summary>
        /// 结账单id
        /// </summary>
        public System.Int32 checkId { get { return this._checkId; } set { this._checkId = value; } }
    }
}
