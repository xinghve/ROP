using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 医疗室
    /// </summary>
    public class p_room
    {
        /// <summary>
        /// 医疗室
        /// </summary>
        public p_room()
        {
        }

        private System.Int32 _id;
        /// <summary>
        /// ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public System.Int32 id { get { return this._id; } set { this._id = value; } }

        private System.Int32? _org_id;
        /// <summary>
        /// 集团ID
        /// </summary>
        public System.Int32? org_id { get { return this._org_id; } set { this._org_id = value ?? default(System.Int32); } }

        private System.Int32? _store_id;
        /// <summary>
        /// 门店ID
        /// </summary>
        public System.Int32? store_id { get { return this._store_id; } set { this._store_id = value ?? default(System.Int32); } }

        private System.String _store_name;
        /// <summary>
        /// 门店名称
        /// </summary>
        public System.String store_name { get { return this._store_name; } set { this._store_name = value?.Trim(); } }

        private System.String _name;
        /// <summary>
        /// 名称
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.Int16? _state;
        /// <summary>
        /// 状态
        /// </summary>
        public System.Int16? state { get { return this._state; } set { this._state = value ?? default(System.Int16); } }

        private System.Int16? _equipment;
        /// <summary>
        /// 是否存放设备
        /// </summary>
        public System.Int16? equipment { get { return this._equipment; } set { this._equipment = value ?? default(System.Int16); } }

        private System.String _position;
        /// <summary>
        /// 位置
        /// </summary>
        public System.String position { get { return this._position; } set { this._position = value?.Trim(); } }

        private System.Int32? _creater_id;
        /// <summary>
        /// 创建人ID
        /// </summary>
        public System.Int32? creater_id { get { return this._creater_id; } set { this._creater_id = value ?? default(System.Int32); } }

        private System.String _creater;
        /// <summary>
        /// 创建人
        /// </summary>
        public System.String creater { get { return this._creater; } set { this._creater = value?.Trim(); } }

        private System.DateTime? _create_date;
        /// <summary>
        /// 创建时间
        /// </summary>
        public System.DateTime? create_date { get { return this._create_date; } set { this._create_date = value ?? default(System.DateTime); } }

        private System.Int16? _wait_times;
        /// <summary>
        /// 等待时长（x分/次）
        /// </summary>
        public System.Int16? wait_times { get { return this._wait_times; } set { this._wait_times = value ?? default(System.Int16); } }

        private System.Int16? _work_times;
        /// <summary>
        /// 工作时长（x分/天）
        /// </summary>
        public System.Int16? work_times { get { return this._work_times; } set { this._work_times = value ?? default(System.Int16); } }
    }
}