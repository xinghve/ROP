using Models.View.Public;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.His
{
    /// <summary>
    /// 医疗室排班查询
    /// </summary>
    public class RoomSchedulingSearch : Search
    {
        private System.String _name;
        /// <summary>
        /// 客户姓名/手机号
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.DateTime? _startTime;
        /// <summary>
        /// 工作日期开始时间
        /// </summary>
        public System.DateTime? startTime { get { return this._startTime; } set { this._startTime = value; } }

        private System.DateTime? _endTime;
        /// <summary>
        /// 工作日期结束时间
        /// </summary>
        public System.DateTime? endTime { get { return this._endTime; } set { this._endTime = value; } }

        private short _state_id = 5;
        /// <summary>
        /// 状态（5=正常，16=待执行，17=已执行）
        /// </summary>
        public short state_id { get { return this._state_id; } set { this._state_id = value; } }

        private bool _is_me = false;
        /// <summary>
        /// 是否自己客户
        /// </summary>
        public bool is_me { get { return this._is_me; } set { this._is_me = value; } }

        private System.Int32 _store_id = 0;
        /// <summary>
        /// 门店id
        /// </summary>
        public System.Int32 store_id { get { return this._store_id; } set { this._store_id = value; } }

        private System.Int32 _room_id;
        /// <summary>
        /// 医疗室ID
        /// </summary>
        public System.Int32 room_id { get { return this._room_id; } set { this._room_id = value; } }
    }
}
