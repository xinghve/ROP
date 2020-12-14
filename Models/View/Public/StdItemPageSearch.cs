using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Public
{
    /// <summary>
    /// 基础分页查询
    /// </summary>
    public class StdItemPageSearch : Search
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }

        private short _state = -1;
        /// <summary>
        /// 状态
        /// </summary>
        public short state { get { return this._state; } set { this._state = value; } }

        private short _type_id = -1;
        /// <summary>
        /// 类别ID
        /// </summary>
        public short type_id { get { return this._type_id; } set { this._type_id = value; } }

        private int _manufactor_id = -1;
        /// <summary>
        /// 厂家ID
        /// </summary>
        public int manufactor_id { get { return this._manufactor_id; } set { this._manufactor_id = value; } }

        private short _catalog_type = 0;
        /// <summary>
        /// 目录类型(0=所有；1=目录；2=类型)
        /// </summary>
        public short catalog_type { get { return this._catalog_type; } set { this._catalog_type = value; } }
    }
}
