using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 
    /// </summary>
    public class oa_meeting
    {
        /// <summary>
        /// 
        /// </summary>
        public oa_meeting()
        {
        }

        private System.Int32 _id;
        /// <summary>
        /// 主键ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public System.Int32 id { get { return this._id; } set { this._id = value; } }

        private System.Int32? _org_id;
        /// <summary>
        /// 机构ID
        /// </summary>
        public System.Int32? org_id { get { return this._org_id; } set { this._org_id = value ?? default(System.Int32); } }

        private System.Int32? _store_id;
        /// <summary>
        /// 门店ID
        /// </summary>
        public System.Int32? store_id { get { return this._store_id; } set { this._store_id = value ?? default(System.Int32); } }

        private System.String _store;
        /// <summary>
        /// 门店
        /// </summary>
        public System.String store { get { return this._store; } set { this._store = value?.Trim(); } }

        private System.Int32 _dept_id;
        /// <summary>
        /// 部门ID
        /// </summary>
        public System.Int32 dept_id { get { return this._dept_id; } set { this._dept_id = value; } }

        private System.String _dept_name;
        /// <summary>
        /// 部门名称
        /// </summary>
        public System.String dept_name { get { return this._dept_name; } set { this._dept_name = value?.Trim(); } }

        private System.String _no;
        /// <summary>
        /// 编号
        /// </summary>
        public System.String no { get { return this._no; } set { this._no = value?.Trim(); } }

        private System.Int16 _type_id;
        /// <summary>
        /// 类型ID
        /// </summary>
        public System.Int16 type_id { get { return this._type_id; } set { this._type_id = value; } }

        private System.String _type;
        /// <summary>
        /// 类型
        /// </summary>
        public System.String type { get { return this._type; } set { this._type = value?.Trim(); } }

        private System.Int16 _state;
        /// <summary>
        /// 状态
        /// </summary>
        public System.Int16 state { get { return this._state; } set { this._state = value; } }

        private System.Int32 _convenor_id;
        /// <summary>
        /// 召集人ID
        /// </summary>
        public System.Int32 convenor_id { get { return this._convenor_id; } set { this._convenor_id = value; } }

        private System.String _convenor;
        /// <summary>
        /// 召集人
        /// </summary>
        public System.String convenor { get { return this._convenor; } set { this._convenor = value?.Trim(); } }

        private System.String _start_time;
        /// <summary>
        /// 开始时间
        /// </summary>
        public System.String start_time { get { return this._start_time; } set { this._start_time = value?.Trim(); } }

        private System.Decimal _times;
        /// <summary>
        /// 时长
        /// </summary>
        public System.Decimal times { get { return this._times; } set { this._times = value; } }

        private System.String _address;
        /// <summary>
        /// 地址
        /// </summary>
        public System.String address { get { return this._address; } set { this._address = value?.Trim(); } }

        private System.Int32 _host_id;
        /// <summary>
        /// 主持人ID
        /// </summary>
        public System.Int32 host_id { get { return this._host_id; } set { this._host_id = value; } }

        private System.String _host;
        /// <summary>
        /// 主持人
        /// </summary>
        public System.String host { get { return this._host; } set { this._host = value?.Trim(); } }

        private System.String _theme;
        /// <summary>
        /// 主题
        /// </summary>
        public System.String theme { get { return this._theme; } set { this._theme = value?.Trim(); } }

        private System.String _issue;
        /// <summary>
        /// 议题
        /// </summary>
        public System.String issue { get { return this._issue; } set { this._issue = value?.Trim(); } }

        private System.String _data;
        /// <summary>
        /// 资料
        /// </summary>
        public System.String data { get { return this._data; } set { this._data = value?.Trim(); } }

        private System.String _content;
        /// <summary>
        /// 内容
        /// </summary>
        public System.String content { get { return this._content; } set { this._content = value?.Trim(); } }

        private System.Int32 _creater_id;
        /// <summary>
        /// 创建人ID
        /// </summary>
        public System.Int32 creater_id { get { return this._creater_id; } set { this._creater_id = value; } }

        private System.String _creater;
        /// <summary>
        /// 创建人
        /// </summary>
        public System.String creater { get { return this._creater; } set { this._creater = value?.Trim(); } }

        private System.DateTime _create_time;
        /// <summary>
        /// 创建时间
        /// </summary>
        public System.DateTime create_time { get { return this._create_time; } set { this._create_time = value; } }

        private System.DateTime _start_date;
        /// <summary>
        /// 开始日期
        /// </summary>
        public System.DateTime start_date { get { return this._start_date; } set { this._start_date = value; } }

        private System.String _remark;
        /// <summary>
        /// 备注
        /// </summary>
        public System.String remark { get { return this._remark; } set { this._remark = value?.Trim(); } }

        private System.Int32 _note_taker_id;
        /// <summary>
        /// 记录人ID
        /// </summary>
        public System.Int32 note_taker_id { get { return this._note_taker_id; } set { this._note_taker_id = value; } }

        private System.String _note_taker;
        /// <summary>
        /// 记录人
        /// </summary>
        public System.String note_taker { get { return this._note_taker; } set { this._note_taker = value?.Trim(); } }
    }
}