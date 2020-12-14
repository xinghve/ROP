using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Crm
{
    /// <summary>
    /// 充值记录实体
    /// </summary>
     public class RJLModel
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

        private System.DateTime? _startTime;
        /// <summary>
        /// 开始时间
        /// </summary>
        public System.DateTime? startTime { get { return this._startTime; } set { this._startTime = value; } }

        /// <summary>
        /// 结束时间
        /// </summary>
        public string endTime { get; set; }

        private System.String _level;
        /// <summary>
        /// 会员等级
        /// </summary>
        public System.String level { get { return this._level; } set { this._level = value?.Trim(); } }

        private System.String _phone;
        /// <summary>
        /// 手机号
        /// </summary>
        public System.String phone { get { return this._phone; } set { this._phone = value?.Trim(); } }

        private System.String _card_no;
        /// <summary>
        /// 卡号
        /// </summary>
        public System.String card_no { get { return this._card_no; } set { this._card_no = value?.Trim(); } }

        private System.String _way_code;
        /// <summary>
        /// 支付方式编码
        /// </summary>
        public System.String way_code { get { return this._way_code; } set { this._way_code = value?.Trim(); } }

        private System.String _bill_no;
        /// <summary>
        /// 流水号
        /// </summary>
        public System.String bill_no { get { return this._bill_no; } set { this._bill_no = value?.Trim(); } }
    }
}
