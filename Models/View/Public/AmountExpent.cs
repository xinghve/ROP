using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Public
{
    /// <summary>
    /// 提成分页
    /// </summary>
    public class AmountExpent:r_amount
    {
        /// <summary>
        /// 分销人员手机号
        /// </summary>
        public string distributor_phone { get; set; }
        /// <summary>
        /// 门店名
        /// </summary>
        public string store_name { get; set; }
    }
    /// <summary>
    /// 提成搜索条件
    /// </summary>
    public class AmountSearch:Search
    {
        /// <summary>
        /// 门店id
        /// </summary>
        public int store_id { get; set; }

        /// <summary>
        /// 查询条件（用户/用户手机/分销人员/分销手机/卡号）
        /// </summary>
        public string search_condition { get; set; }

        private System.DateTime? _startTime;
        /// <summary>
        /// 提成开始时间
        /// </summary>
        public System.DateTime? startTime { get { return this._startTime; } set { this._startTime = value ?? default(System.DateTime); } }

        private System.DateTime? _endTime;
        /// <summary>
        /// 结束时间
        /// </summary>
        public System.DateTime? endTime { get { return this._endTime; } set { this._endTime = value; } }
    }

    /// <summary>
    /// 分销人员提成比例设置
    /// </summary>
    public class AmountRate
    {
        /// <summary>
        /// 提成比例设置
        /// </summary>
        public decimal royalty_rate { get; set; }
    }
}
