using Models.View.Public;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Business
{
    /// <summary>
    /// 报损报溢分页查询
    /// </summary>
    public class LossOverflowPageSearch : Search
    {
        private System.String _no;
        /// <summary>
        /// 单号
        /// </summary>
        public System.String no { get { return this._no; } set { this._no = value?.Trim(); } }

        private System.String _realted_no;
        /// <summary>
        /// 关联单号
        /// </summary>
        public System.String realted_no { get { return this._realted_no; } set { this._realted_no = value?.Trim(); } }

        private System.Int32 _store_id;
        /// <summary>
        /// 门店ID
        /// </summary>
        public System.Int32 store_id { get { return this._store_id; } set { this._store_id = value; } }

        private System.Int16? _state;
        /// <summary>
        /// 状态(26=待审核；36=审核中；28=未通过；15=已完成；-1=所有)
        /// </summary>
        public System.Int16? state { get { return this._state; } set { this._state = value ?? default(System.Int16); } }

        /// <summary>
        /// 盘点年月（开始）
        /// </summary>
        public DateTime? start_date { get; set; }

        /// <summary>
        /// 盘点年月（结束）
        /// </summary>
        public DateTime? end_date { get; set; }

        private System.Int16? _type_id;
        /// <summary>
        /// 单据类型（1=报溢；2=报损；）
        /// </summary>
        public System.Int16? type_id { get { return this._type_id; } set { this._type_id = value ?? default(System.Int16); } }
    }
}
