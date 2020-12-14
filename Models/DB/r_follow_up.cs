using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 随访计划
    /// </summary>
    public class r_follow_up
    {
        /// <summary>
        /// 随访计划
        /// </summary>
        public r_follow_up()
        {
        }

        private System.Int32 _id;
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public System.Int32 id { get { return this._id; } set { this._id = value; } }

        private System.Int32? _archives_id;
        /// <summary>
        /// 档案ID
        /// </summary>
        public System.Int32? archives_id { get { return this._archives_id; } set { this._archives_id = value ?? default(System.Int32); } }

        private System.String _archives;
        /// <summary>
        /// 档案姓名
        /// </summary>
        public System.String archives { get { return this._archives; } set { this._archives = value?.Trim(); } }

        private System.String _archives_phone;
        /// <summary>
        /// 档案手机
        /// </summary>
        public System.String archives_phone { get { return this._archives_phone; } set { this._archives_phone = value?.Trim(); } }

        private System.String _content;
        /// <summary>
        /// 随访内容
        /// </summary>
        public System.String content { get { return this._content; } set { this._content = value?.Trim(); } }

        private System.DateTime? _task_date;
        /// <summary>
        /// 待执行任务时间
        /// </summary>
        public System.DateTime? task_date { get { return this._task_date; } set { this._task_date = value ?? default(System.DateTime); } }

        private System.Int16? _state;
        /// <summary>
        /// 状态（16=待执行，17=已执行）
        /// </summary>
        public System.Int16? state { get { return this._state; } set { this._state = value ?? default(System.Int16); } }

        private System.Int32? _executor_id;
        /// <summary>
        /// 执行人ID
        /// </summary>
        public System.Int32? executor_id { get { return this._executor_id; } set { this._executor_id = value ?? default(System.Int32); } }

        private System.String _executor;
        /// <summary>
        /// 执行人
        /// </summary>
        public System.String executor { get { return this._executor; } set { this._executor = value?.Trim(); } }

        private System.DateTime? _execute_date;
        /// <summary>
        /// 执行时间
        /// </summary>
        public System.DateTime? execute_date { get { return this._execute_date; } set { this._execute_date = value ?? default(System.DateTime); } }

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

        private System.Int32? _store_id;
        /// <summary>
        /// 门店id
        /// </summary>
        public System.Int32? store_id { get { return this._store_id; } set { this._store_id = value ?? default(System.Int32); } }

        private System.Int32 _org_id;
        /// <summary>
        /// 机构id
        /// </summary>
        public System.Int32 org_id { get { return this._org_id; } set { this._org_id = value; } }

        private System.Int32? _client_id;
        /// <summary>
        /// 委托人ID
        /// </summary>
        public System.Int32? client_id { get { return this._client_id; } set { this._client_id = value ?? default(System.Int32); } }

        private System.String _client;
        /// <summary>
        /// 委托人
        /// </summary>
        public System.String client { get { return this._client; } set { this._client = value?.Trim(); } }

        private System.DateTime? _client_date;
        /// <summary>
        /// 委托时间
        /// </summary>
        public System.DateTime? client_date { get { return this._client_date; } set { this._client_date = value ?? default(System.DateTime); } }

        private System.String _id_card;
        /// <summary>
        /// 身份证号
        /// </summary>
        public System.String id_card { get { return this._id_card; } set { this._id_card = value?.Trim(); } }

        private System.String _answer;
        /// <summary>
        /// 答复内容
        /// </summary>
        public System.String answer { get { return this._answer; } set { this._answer = value?.Trim(); } }
    }
}