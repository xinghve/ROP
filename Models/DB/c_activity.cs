using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 活动
    /// </summary>
    public class c_activity
    {
        /// <summary>
        /// 活动
        /// </summary>
        public c_activity()
        {
        }

        private System.Int32 _id;
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public System.Int32 id { get { return this._id; } set { this._id = value; } }

        private System.Int32 _org_id;
        /// <summary>
        /// 机构id
        /// </summary>
        public System.Int32 org_id { get { return this._org_id; } set { this._org_id = value; } }

        private System.String _name;
        /// <summary>
        /// 名称
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.DateTime? _start_date;
        /// <summary>
        /// 开始时间
        /// </summary>
        public System.DateTime? start_date { get { return this._start_date; } set { this._start_date = value ?? default(System.DateTime); } }

        private System.DateTime? _end_date;
        /// <summary>
        /// 结束时间
        /// </summary>
        public System.DateTime? end_date { get { return this._end_date; } set { this._end_date = value ?? default(System.DateTime); } }

        private System.String _content;
        /// <summary>
        /// 内容
        /// </summary>
        public System.String content { get { return this._content; } set { this._content = value?.Trim(); } }

        private System.Int16? _state;
        /// <summary>
        /// 状态（1=正常，0=暂停）
        /// </summary>
        public System.Int16? state { get { return this._state; } set { this._state = value ?? default(System.Int16); } }

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

        private System.Int16? _overlay;
        /// <summary>
        /// 叠加
        /// </summary>
        public System.Int16? overlay { get { return this._overlay; } set { this._overlay = value ?? default(System.Int16); } }

        private System.String _address;
        /// <summary>
        /// 地址
        /// </summary>
        public System.String address { get { return this._address; } set { this._address = value?.Trim(); } }
    }
}