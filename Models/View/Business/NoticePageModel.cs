using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Business
{
    /// <summary>
    /// 消息通知
    /// </summary>
    public class NoticePageModel
    {
        /// <summary>
        /// 通知类型id
        /// </summary>
        public short valueid { get; set; }

        /// <summary>
        /// 通知类型名称
        /// </summary>
        public string notice_type_name { get; set; }

        /// <summary>
        /// 通知属性id
        /// </summary>
        public short propertyid { get; set; }

        /// <summary>
        /// 通知属性名
        /// </summary>
        public string notice_property { get; set; }

        /// <summary>
        /// 通知内容
        /// </summary>
        public string notice_content { get; set; }

        /// <summary>
        /// 关联单号
        /// </summary>
        public string relation_no { get; set; }

        /// <summary>
        /// 服务对象id
        /// </summary>
        public int service_object_id { get; set; }

        /// <summary>
        /// 服务对象
        /// </summary>
        public string service_object { get; set; }
        /// <summary>
        /// 是否已读
        /// </summary>
        public short read_state { get; set; }

        /// <summary>
        /// 通知时间
        /// </summary>
        public DateTime notice_time { get; set; }

        /// <summary>
        /// 通知id
        /// </summary>
        public int id { get; set; }
    }

    /// <summary>
    /// 通知搜索
    /// </summary>
    public class NoticeSearchModel
    {
        /// <summary>
        /// 类型id
        /// </summary>
        public int valie_id { get; set; }
        /// <summary>
        /// 读取状态
        /// </summary>
        public int readstate { get; set; }

        private System.String _order = "create_date";
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
