using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Crm
{
    /// <summary>
    /// 活动图片实体
    /// </summary>
    public class ActivityIMGModel
    {
        private System.Int32 _activity_id;
        /// <summary>
        /// 活动ID
        /// </summary>
        public System.Int32 activity_id { get { return this._activity_id; } set { this._activity_id = value; } }
        /// <summary>
        /// 图片路径列表
        /// </summary>
        public List<string> vs { get; set; }
    }
}
