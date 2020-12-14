using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.OA
{
    /// <summary>
    /// 决议列表
    /// </summary>
    public class ResolutionList
    {
        /// <summary>
        /// 会议ID
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<Resolution> resolutions { get; set; }
    }

    /// <summary>
    /// 决议
    /// </summary>
    public class Resolution
    {
        private System.String _matter;
        /// <summary>
        /// 事项
        /// </summary>
        public System.String matter { get { return this._matter; } set { this._matter = value?.Trim(); } }

        private System.String _target;
        /// <summary>
        /// 实施目标
        /// </summary>
        public System.String target { get { return this._target; } set { this._target = value?.Trim(); } }
    }
}
