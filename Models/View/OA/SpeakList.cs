using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.OA
{
    /// <summary>
    /// 发言列表
    /// </summary>
    public class SpeakList
    {
        /// <summary>
        /// 会议ID
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<Speak> speaks { get; set; }
    }

    /// <summary>
    /// 发言
    /// </summary>
    public class Speak
    {
        private System.Int32 _spokesman_id;
        /// <summary>
        /// 发言人ID
        /// </summary>
        public System.Int32 spokesman_id { get { return this._spokesman_id; } set { this._spokesman_id = value; } }

        private System.String _spokesman;
        /// <summary>
        /// 发言人
        /// </summary>
        public System.String spokesman { get { return this._spokesman; } set { this._spokesman = value?.Trim(); } }

        private System.String _point;
        /// <summary>
        /// 要点
        /// </summary>
        public System.String point { get { return this._point; } set { this._point = value?.Trim(); } }
    }
}
