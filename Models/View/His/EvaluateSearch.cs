using Models.DB;
using Models.View.Public;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.His
{
    /// <summary>
    /// 评价查询
    /// </summary>
    public class EvaluateSearch: Search
    {
        /// <summary>
        /// 门店id
        /// </summary>
        public int storeId { get; set; }
        /// <summary>
        /// 就诊用户
        /// </summary>
        public string clinic_name { get; set; }

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

    /// <summary>
    /// 评价分页model
    /// </summary>
    public class EvaluatePageModel: r_evaluate
    {
        /// <summary>
        /// 门店名
        /// </summary>
        public string storeName {get;set; }
        /// <summary>
        /// 就诊用户
        /// </summary>
        public string clinic_name { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string phone { get; set; }
    }
}
