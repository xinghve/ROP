using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Public
{
    /// <summary>
    /// 图片
    /// </summary>
    public class Image
    {

        private System.String _relation_code;
        /// <summary>
        /// 关联编码
        /// </summary>
        public System.String relation_code { get { return this._relation_code; } set { this._relation_code = value?.Trim(); } }

        /// <summary>
        /// 图片路径列表
        /// </summary>
        public List<string> vs { get; set; }
    }
}
