using Models.DB;
using Models.View.Public;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Crm
{
    /// <summary>
    /// 活动优惠券
    /// </summary>
    public class ActivityCouponModel
    {
        /// <summary>
        /// 活动
        /// </summary>
        public c_activity activityModel { get; set; }

        /// <summary>
        /// 优惠券List
        /// </summary>
        public List<c_new_coupon> aCModel { get; set; }

        /// <summary>
        /// 门店ID
        /// </summary>
        public List<int> store_id_list { get; set; }

        ///// <summary>
        ///// 下拉优惠券array
        ///// </summary>
        //public List<c_activity_coupon> cArray { get; set; }

        ///// <summary>
        ///// 活动积分List
        ///// </summary>
        //public List<c_activity_level> activityLevelModel { get; set; }
    }

    /// <summary>
    /// 根据活动优惠券返回用户
    /// </summary>
    public class ReturnArchivesModel
    {
        private System.Int32 _activity_id;
        /// <summary>
        /// 活动id
        /// </summary>
        public System.Int32 activity_id { get { return this._activity_id; } set { this._activity_id = value; } }

        private System.Int32 _store_id;
        /// <summary>
        /// 门店id
        /// </summary>
        public System.Int32 store_id { get { return this._store_id; } set { this._store_id = value; } }

        /// <summary>
        /// 优惠券List
        /// </summary>
        public string coupList { get; set; }
    }

    /// <summary>
    /// 优惠券领取分页
    /// </summary>
    public class ReceiveCouponModel: c_archives_activity_coupon
    {
        /// <summary>
        /// 档案名称
        /// </summary>
        public string archives_name { get; set; }

        /// <summary>
        /// 用户手机号
        /// </summary>
        public string archives_phone { get; set; }

        /// <summary>
        /// 活动名称
        /// </summary>
        public string activity_name { get; set; }
    }

    /// <summary>
    /// 优惠券领取查询
    /// </summary>
    public class ReceiveCouponSearch  
    {
        /// <summary>
        /// 活动名称，客户名称，优惠券号
        /// </summary>
        public string name { get; set; }

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
        public System.DateTime? startTime { get; set; }

        private System.DateTime? _endTime;
        /// <summary>
        /// 结束时间
        /// </summary>
        public System.DateTime? endTime { get; set; }

    }




}
