using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 设备器械详细
    /// </summary>
    public class p_equipment_detials
    {
        /// <summary>
        /// 设备器械详细
        /// </summary>
        public p_equipment_detials()
        {
        }

        private System.Int32 _id;
        /// <summary>
        /// 设备ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 id { get { return this._id; } set { this._id = value; } }

        private System.String _no;
        /// <summary>
        /// 编号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String no { get { return this._no; } set { this._no = value?.Trim(); } }

        private System.String _serial_number;
        /// <summary>
        /// 产品序号
        /// </summary>
        public System.String serial_number { get { return this._serial_number; } set { this._serial_number = value?.Trim(); } }

        private System.String _batch_number;
        /// <summary>
        /// 批次号
        /// </summary>
        public System.String batch_number { get { return this._batch_number; } set { this._batch_number = value?.Trim(); } }

        private System.DateTime? _manufacture_date;
        /// <summary>
        /// 生产日期
        /// </summary>
        public System.DateTime? manufacture_date { get { return this._manufacture_date; } set { this._manufacture_date = value ?? default(System.DateTime); } }

        private System.DateTime? _buy_date;
        /// <summary>
        /// 购买日期
        /// </summary>
        public System.DateTime? buy_date { get { return this._buy_date; } set { this._buy_date = value ?? default(System.DateTime); } }

        private System.Int16? _state;
        /// <summary>
        /// 状态
        /// </summary>
        public System.Int16? state { get { return this._state; } set { this._state = value ?? default(System.Int16); } }

        private System.Int32? _administrator_id;
        /// <summary>
        /// 管理员ID
        /// </summary>
        public System.Int32? administrator_id { get { return this._administrator_id; } set { this._administrator_id = value ?? default(System.Int32); } }

        private System.String _administrator;
        /// <summary>
        /// 管理员
        /// </summary>
        public System.String administrator { get { return this._administrator; } set { this._administrator = value?.Trim(); } }

        private System.Int16? _work_times;
        /// <summary>
        /// 工作时长（x分/天）
        /// </summary>
        public System.Int16? work_times { get { return this._work_times; } set { this._work_times = value ?? default(System.Int16); } }

        private System.Int16? _wait_times;
        /// <summary>
        /// 等待时长（x分/次）
        /// </summary>
        public System.Int16? wait_times { get { return this._wait_times; } set { this._wait_times = value ?? default(System.Int16); } }

        private System.Int32? _room_id;
        /// <summary>
        /// 医疗室ID
        /// </summary>
        public System.Int32? room_id { get { return this._room_id; } set { this._room_id = value ?? default(System.Int32); } }
    }
}