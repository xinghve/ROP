using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 
    /// </summary>
    public class p_process
    {
        /// <summary>
        /// 
        /// </summary>
        public p_process()
        {
        }

        private System.Int32 _id;
        /// <summary>
        /// 主键ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public System.Int32 id { get { return this._id; } set { this._id = value; } }

        private System.String _name;
        /// <summary>
        /// 名称
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.String _spell;
        /// <summary>
        /// 拼音码
        /// </summary>
        public System.String spell { get { return this._spell; } set { this._spell = value?.Trim(); } }

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

        private System.Int32 _org_id;
        /// <summary>
        /// 集团ID
        /// </summary>
        public System.Int32 org_id { get { return this._org_id; } set { this._org_id = value; } }

        private System.Int32 _store_id;
        /// <summary>
        /// 门店ID
        /// </summary>
        public System.Int32 store_id { get { return this._store_id; } set { this._store_id = value; } }

        private System.String _store_name;
        /// <summary>
        /// 门店名称
        /// </summary>
        public System.String store_name { get { return this._store_name; } set { this._store_name = value?.Trim(); } }

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

        private System.Int16 _state;
        /// <summary>
        /// 状态
        /// </summary>
        public System.Int16 state { get { return this._state; } set { this._state = value; } }

        private System.Int16 _total_level;
        /// <summary>
        /// 总层级
        /// </summary>
        public System.Int16 total_level { get { return this._total_level; } set { this._total_level = value; } }

        private System.DateTime? _enable_time;
        /// <summary>
        /// 启用时间
        /// </summary>
        public System.DateTime? enable_time { get { return this._enable_time; } set { this._enable_time = value ?? default(System.DateTime); } }

        private System.Int32? _enabler_id;
        /// <summary>
        /// 启用人ID
        /// </summary>
        public System.Int32? enabler_id { get { return this._enabler_id; } set { this._enabler_id = value ?? default(System.Int32); } }

        private System.String _enabler;
        /// <summary>
        /// 启用人
        /// </summary>
        public System.String enabler { get { return this._enabler; } set { this._enabler = value?.Trim(); } }

        private System.DateTime _create_time;
        /// <summary>
        /// 创建时间
        /// </summary>
        public System.DateTime create_time { get { return this._create_time; } set { this._create_time = value; } }

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

        private System.String _remark;
        /// <summary>
        /// 备注
        /// </summary>
        public System.String remark { get { return this._remark; } set { this._remark = value?.Trim(); } }

        private System.Int16 _leave_type_id;
        /// <summary>
        /// 请假类型ID
        /// </summary>
        public System.Int16 leave_type_id { get { return this._leave_type_id; } set { this._leave_type_id = value; } }

        private System.String _leave_type;
        /// <summary>
        /// 请假类型
        /// </summary>
        public System.String leave_type { get { return this._leave_type; } set { this._leave_type = value?.Trim(); } }

        private System.Decimal? _duration;
        /// <summary>
        /// 时长
        /// </summary>
        public System.Decimal? duration { get { return this._duration; } set { this._duration = value ?? default(System.Decimal); } }

        private System.Decimal? _use_money=0;
        /// <summary>
        /// 金额限制
        /// </summary>
        public System.Decimal? use_money { get { return this._use_money; } set { this._use_money = value ?? default(System.Decimal); } }
    }
}