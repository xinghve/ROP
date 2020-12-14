using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.OA
{
    /// <summary>
    /// 文件信息
    /// </summary>
    public class FilesInfo
    {
        private System.String _name;
        /// <summary>
        /// 文件名
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.String _url;
        /// <summary>
        /// 路径
        /// </summary>
        public System.String url { get { return this._url; } set { this._url = value?.Trim(); } }

        private System.String _type;
        /// <summary>
        /// 类型
        /// </summary>
        public System.String type { get { return this._type; } set { this._type = value?.Trim(); } }

        private System.Int32 _size;
        /// <summary>
        /// 大小
        /// </summary>
        public System.Int32 size { get { return this._size; } set { this._size = value; } }

        private System.DateTime _create_time;
        /// <summary>
        /// 创建时间
        /// </summary>
        public System.DateTime create_time { get { return this._create_time; } set { this._create_time = value; } }
    }
}
