using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 
    /// </summary>
    public class oa_leave
    {
        /// <summary>
        /// 
        /// </summary>
        public oa_leave()
        {
        }

        private System.String _leave_no;
        /// <summary>
        /// 请假单号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String leave_no { get { return this._leave_no; } set { this._leave_no = value?.Trim(); } }

        private System.Int32 _org_id;
        /// <summary>
        /// 机构ID
        /// </summary>
        public System.Int32 org_id { get { return this._org_id; } set { this._org_id = value; } }

        private System.Int32 _store_id;
        /// <summary>
        /// 门店ID
        /// </summary>
        public System.Int32 store_id { get { return this._store_id; } set { this._store_id = value; } }

        private System.String _store;
        /// <summary>
        /// 门店
        /// </summary>
        public System.String store { get { return this._store; } set { this._store = value?.Trim(); } }

        private System.Int16? _state;
        /// <summary>
        /// 状态
        /// </summary>
        public System.Int16? state { get { return this._state; } set { this._state = value ?? default(System.Int16); } }

        private System.String _leave_cause;
        /// <summary>
        /// 请假事由
        /// </summary>
        public System.String leave_cause { get { return this._leave_cause; } set { this._leave_cause = value?.Trim(); } }

        private System.Int16 _total_level;
        /// <summary>
        /// 总层级
        /// </summary>
        public System.Int16 total_level { get { return this._total_level; } set { this._total_level = value; } }

        private System.Int16 _level;
        /// <summary>
        /// 层级
        /// </summary>
        public System.Int16 level { get { return this._level; } set { this._level = value; } }

        private System.Int32? _org_process_id;
        /// <summary>
        /// 集团流程ID
        /// </summary>
        public System.Int32? org_process_id { get { return this._org_process_id; } set { this._org_process_id = value ?? default(System.Int32); } }

        private System.Int32? _verifier_id;
        /// <summary>
        /// 审核人ID
        /// </summary>
        public System.Int32? verifier_id { get { return this._verifier_id; } set { this._verifier_id = value ?? default(System.Int32); } }

        private System.String _verifier;
        /// <summary>
        /// 审核人
        /// </summary>
        public System.String verifier { get { return this._verifier; } set { this._verifier = value?.Trim(); } }

        private System.DateTime? _verify_time;
        /// <summary>
        /// 审核时间
        /// </summary>
        public System.DateTime? verify_time { get { return this._verify_time; } set { this._verify_time = value ?? default(System.DateTime); } }

        private System.String _verify_remark;
        /// <summary>
        /// 审核说明
        /// </summary>
        public System.String verify_remark { get { return this._verify_remark; } set { this._verify_remark = value?.Trim(); } }

        private System.Int32? _await_verifier_id;
        /// <summary>
        /// 待审核人ID
        /// </summary>
        public System.Int32? await_verifier_id { get { return this._await_verifier_id; } set { this._await_verifier_id = value ?? default(System.Int32); } }

        private System.String _await_verifier;
        /// <summary>
        /// 待审核人
        /// </summary>
        public System.String await_verifier { get { return this._await_verifier; } set { this._await_verifier = value?.Trim(); } }

        private System.Int32 _applicant_id;
        /// <summary>
        /// 申请人ID
        /// </summary>
        public System.Int32 applicant_id { get { return this._applicant_id; } set { this._applicant_id = value; } }

        private System.String _applicant;
        /// <summary>
        /// 申请人
        /// </summary>
        public System.String applicant { get { return this._applicant; } set { this._applicant = value?.Trim(); } }

        private System.DateTime _apply_time;
        /// <summary>
        /// 申请时间
        /// </summary>
        public System.DateTime apply_time { get { return this._apply_time; } set { this._apply_time = value; } }

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

        private System.Decimal _duration;
        /// <summary>
        /// 时长
        /// </summary>
        public System.Decimal duration { get { return this._duration; } set { this._duration = value; } }

        private System.DateTime _apply_start_time;
        /// <summary>
        /// 请假开始时间
        /// </summary>
        public System.DateTime apply_start_time { get { return this._apply_start_time; } set { this._apply_start_time = value; } }

        private System.DateTime _apply_end_time;
        /// <summary>
        /// 请假结束时间
        /// </summary>
        public System.DateTime apply_end_time { get { return this._apply_end_time; } set { this._apply_end_time = value; } }

        private System.Int32 _role_id;
        /// <summary>
        /// 待审核角色ID
        /// </summary>
        public System.Int32 role_id { get { return this._role_id; } set { this._role_id = value; } }

        private System.String _role_name;
        /// <summary>
        /// 待审核角色
        /// </summary>
        public System.String role_name { get { return this._role_name; } set { this._role_name = value?.Trim(); } }

        private System.Boolean? _is_org;
        /// <summary>
        /// 是否机构
        /// </summary>
        public System.Boolean? is_org { get { return this._is_org; } set { this._is_org = value ?? default(System.Boolean); } }
    }
}