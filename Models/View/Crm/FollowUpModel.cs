using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Crm
{
    /// <summary>
    /// 随访分页model
    /// </summary>
    public class FollowUpModel: r_follow_up
    {

        /// <summary>
        /// 门店名
        /// </summary>
        public string storeName { get; set; }

        /// <summary>
        /// 待执行人
        /// </summary>
        public string wait_dir { get; set; }
    }

    /// <summary>
    /// 随访查询
    /// </summary>
    public class FollowUpSearch
    {
        private System.String _name;
        /// <summary>
        /// 搜索 名称/手机号/身份证
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.Int32 _state = 0;
        /// <summary>
        /// 状态（-1=所有，16=待执行，17=已执行）
        /// </summary>
        public System.Int32 state { get { return this._state; } set { this._state = value; } }

        private System.Int32 _all = 0;
        /// <summary>
        /// 所有-1
        /// </summary>
        public System.Int32 all { get { return this._all; } set { this._all = value; } }

        private System.Int32 _storeID = 0;
        /// <summary>
        /// 门店id
        /// </summary>
        public System.Int32 storeID { get { return this._storeID; } set { this._storeID = value; } }

        private System.String _order = "archives_id";
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

        private System.String _dateTimeStart;
        /// <summary>
        /// 计划日期（开始）
        /// </summary>
        public System.String dateTimeStart { get { return this._dateTimeStart; } set { this._dateTimeStart = value?.Trim(); } }

        private System.String _dateTimeEnd;
        /// <summary>
        /// 计划日期（结束）
        /// </summary>
        public System.String dateTimeEnd { get { return this._dateTimeEnd; } set { this._dateTimeEnd = value?.Trim(); } }
    }
}
