using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Public
{
    /// <summary>
    /// 器械设备详细分页查询
    /// </summary>
    public class EquipmentDetialsPagesSearch : Search
    {

        private System.Int32 _id;
        /// <summary>
        /// 设备ID
        /// </summary>
        public System.Int32 id { get { return this._id; } set { this._id = value; } }

        private System.String _no;
        /// <summary>
        /// 编号
        /// </summary>
        public System.String no { get { return this._no; } set { this._no = value?.Trim(); } }

        private System.Int32? _room_id;
        /// <summary>
        /// 医疗室ID
        /// </summary>
        public System.Int32? room_id { get { return this._room_id; } set { this._room_id = value ?? default(System.Int32); } }
    }
}
