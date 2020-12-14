using Models.DB;
using Models.View.Public;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Crm
{
    /// <summary>
    /// 活动分页实体
    /// </summary>
     public class ActivityPageModel
    {
        private System.Int32 _id;
        /// <summary>
        /// 活动id
        /// </summary>
        public System.Int32 id { get { return this._id; } set { this._id = value; } }

        private System.String _name;
        /// <summary>
        /// 名称
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.DateTime? _start_date;
        /// <summary>
        /// 开始时间
        /// </summary>
        public System.DateTime? start_date { get { return this._start_date; } set { this._start_date = value ?? default(System.DateTime); } }

        private System.DateTime? _end_date;
        /// <summary>
        /// 结束时间
        /// </summary>
        public System.DateTime? end_date { get { return this._end_date; } set { this._end_date = value ?? default(System.DateTime); } }

        private System.String _content;
        /// <summary>
        /// 内容
        /// </summary>
        public System.String content { get { return this._content; } set { this._content = value?.Trim(); } }

        private System.Int16? _state;
        /// <summary>
        /// 状态（1=正常，0=暂停）
        /// </summary>
        public System.Int16? state { get { return this._state; } set { this._state = value ?? default(System.Int16); } }
        

        private System.String _creater;
        /// <summary>
        /// 创建人
        /// </summary>
        public System.String creater { get { return this._creater; } set { this._creater = value?.Trim(); } }

        /// <summary>
        /// 活动地址
        /// </summary>
        public string address { get; set; }

        /// <summary>
        /// 优惠券List
        /// </summary>
        public List<c_new_coupon> aCModel { get; set; }

        /// <summary>
        /// 门店
        /// </summary>
        public string store { get; set; }

        private System.Int16? _overlay;
        /// <summary>
        /// 叠加
        /// </summary>
        public System.Int16? overlay { get { return this._overlay; } set { this._overlay = value ?? default(System.Int16); } }

        /// <summary>
        /// 门店List
        /// </summary>
        public List<Store> storeList { get; set; }

        /// <summary>
        /// 活动是否已开始,2是3否
        /// </summary>
        public int isStart { get; set; }

        ///// <summary>
        ///// 活动积分List
        ///// </summary>
        //public List<c_activity_level> alModel { get; set; }

    }
}
