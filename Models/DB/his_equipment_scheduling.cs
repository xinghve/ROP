using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 
    /// </summary>
    public class his_equipment_scheduling
    {
        /// <summary>
        /// 
        /// </summary>
        public his_equipment_scheduling()
        {
        }

        private System.String _id;
        /// <summary>
        /// id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String id { get { return this._id; } set { this._id = value?.Trim(); } }

        private System.Int32 _org_id;
        /// <summary>
        /// 机构id
        /// </summary>
        public System.Int32 org_id { get { return this._org_id; } set { this._org_id = value; } }

        private System.Int32 _store_id;
        /// <summary>
        /// 门店id
        /// </summary>
        public System.Int32 store_id { get { return this._store_id; } set { this._store_id = value; } }

        private System.Int32 _equipment_id;
        /// <summary>
        /// 设备ID
        /// </summary>
        public System.Int32 equipment_id { get { return this._equipment_id; } set { this._equipment_id = value; } }

        private System.String _no;
        /// <summary>
        /// 编号
        /// </summary>
        public System.String no { get { return this._no; } set { this._no = value?.Trim(); } }

        private System.String _equipment_address;
        /// <summary>
        /// 存放位置
        /// </summary>
        public System.String equipment_address { get { return this._equipment_address; } set { this._equipment_address = value?.Trim(); } }

        private System.DateTime _work_date;
        /// <summary>
        /// 工作日期
        /// </summary>
        public System.DateTime work_date { get { return this._work_date; } set { this._work_date = value; } }

        private System.String _work_time_start;
        /// <summary>
        /// 开始工作时间
        /// </summary>
        public System.String work_time_start { get { return this._work_time_start; } set { this._work_time_start = value?.Trim(); } }

        private System.String _work_time_end;
        /// <summary>
        /// 结束工作时间
        /// </summary>
        public System.String work_time_end { get { return this._work_time_end; } set { this._work_time_end = value?.Trim(); } }

        private System.Int32 _itemid;
        /// <summary>
        /// 项目ID
        /// </summary>
        public System.Int32 itemid { get { return this._itemid; } set { this._itemid = value; } }

        private System.String _tradename;
        /// <summary>
        /// 项目商品名
        /// </summary>
        public System.String tradename { get { return this._tradename; } set { this._tradename = value?.Trim(); } }

        private System.Int32 _specid;
        /// <summary>
        /// 规格ID
        /// </summary>
        [SugarColumn(IsIdentity = true)]
        public System.Int32 specid { get { return this._specid; } set { this._specid = value; } }

        private System.String _specname;
        /// <summary>
        /// 规格
        /// </summary>
        public System.String specname { get { return this._specname; } set { this._specname = value?.Trim(); } }

        private System.Int16? _work_times;
        /// <summary>
        /// 工作时长（分钟）
        /// </summary>
        public System.Int16? work_times { get { return this._work_times; } set { this._work_times = value ?? default(System.Int16); } }

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

        private System.String _age;
        /// <summary>
        /// 年龄
        /// </summary>
        public System.String age { get { return this._age; } set { this._age = value?.Trim(); } }

        private System.String _address;
        /// <summary>
        /// 联系地址
        /// </summary>
        public System.String address { get { return this._address; } set { this._address = value?.Trim(); } }

        private System.String _contactno;
        /// <summary>
        /// 联系电话
        /// </summary>
        public System.String contactno { get { return this._contactno; } set { this._contactno = value?.Trim(); } }

        private System.String _emergencyno;
        /// <summary>
        /// 应急电话
        /// </summary>
        public System.String emergencyno { get { return this._emergencyno; } set { this._emergencyno = value?.Trim(); } }

        private System.String _contancts;
        /// <summary>
        /// 联系人
        /// </summary>
        public System.String contancts { get { return this._contancts; } set { this._contancts = value?.Trim(); } }

        private System.String _contactsno;
        /// <summary>
        /// 联系人电话
        /// </summary>
        public System.String contactsno { get { return this._contactsno; } set { this._contactsno = value?.Trim(); } }

        private System.String _relationid;
        /// <summary>
        /// 联系人关系ID
        /// </summary>
        public System.String relationid { get { return this._relationid; } set { this._relationid = value?.Trim(); } }

        private System.String _relation;
        /// <summary>
        /// 联系人关系
        /// </summary>
        public System.String relation { get { return this._relation; } set { this._relation = value?.Trim(); } }

        private System.Int32 _clinicid;
        /// <summary>
        /// 就诊ID
        /// </summary>
        public System.Int32 clinicid { get { return this._clinicid; } set { this._clinicid = value; } }

        private System.Int16 _stateid;
        /// <summary>
        /// 状态ID
        /// </summary>
        public System.Int16 stateid { get { return this._stateid; } set { this._stateid = value; } }

        private System.String _summary;
        /// <summary>
        /// 摘要
        /// </summary>
        public System.String summary { get { return this._summary; } set { this._summary = value?.Trim(); } }

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

        private System.DateTime _create_date;
        /// <summary>
        /// 创建时间
        /// </summary>
        public System.DateTime create_date { get { return this._create_date; } set { this._create_date = value; } }

        private System.DateTime? _sign_time;
        /// <summary>
        /// 签到时间
        /// </summary>
        public System.DateTime? sign_time { get { return this._sign_time; } set { this._sign_time = value ?? default(System.DateTime); } }

        private System.Int32? _room_id;
        /// <summary>
        /// 医疗室ID
        /// </summary>
        public System.Int32? room_id { get { return this._room_id; } set { this._room_id = value ?? default(System.Int32); } }

        private System.Int16? _order_num;
        /// <summary>
        /// 号序
        /// </summary>
        public System.Int16? order_num { get { return this._order_num; } set { this._order_num = value ?? default(System.Int16); } }

        private System.Int16? _wait_times;
        /// <summary>
        /// 等待时长（x分/次）
        /// </summary>
        public System.Int16? wait_times { get { return this._wait_times; } set { this._wait_times = value ?? default(System.Int16); } }

        private System.Int32 _recoverid;
        /// <summary>
        /// 康复预约ID
        /// </summary>
        public System.Int32 recoverid { get { return this._recoverid; } set { this._recoverid = value; } }

        private System.String _room_name;
        /// <summary>
        /// 医疗室名称
        /// </summary>
        public System.String room_name { get { return this._room_name; } set { this._room_name = value?.Trim(); } }
    }
}