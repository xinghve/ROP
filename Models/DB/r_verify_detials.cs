using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 审核记录明细
    /// </summary>
    public class r_verify_detials
    {
        /// <summary>
        /// 审核记录明细
        /// </summary>
        public r_verify_detials()
        {
        }

        private System.String _realted_no;
        /// <summary>
        /// 关联单号
        /// </summary>
        public System.String realted_no { get { return this._realted_no; } set { this._realted_no = value?.Trim(); } }

        private System.Int32 _process_id;
        /// <summary>
        /// 流程ID
        /// </summary>
        public System.Int32 process_id { get { return this._process_id; } set { this._process_id = value; } }

        private System.String _process_name;
        /// <summary>
        /// 流程名
        /// </summary>
        public System.String process_name { get { return this._process_name; } set { this._process_name = value?.Trim(); } }

        private System.Int16 _level_type_id;
        /// <summary>
        /// 请假类型ID
        /// </summary>
        public System.Int16 level_type_id { get { return this._level_type_id; } set { this._level_type_id = value; } }

        private System.String _level_type;
        /// <summary>
        /// 请假类型
        /// </summary>
        public System.String level_type { get { return this._level_type; } set { this._level_type = value?.Trim(); } }

        private System.Int16 _process_total_level;
        /// <summary>
        /// 当前流程总层级
        /// </summary>
        public System.Int16 process_total_level { get { return this._process_total_level; } set { this._process_total_level = value; } }

        private System.Int16 _process_level;
        /// <summary>
        /// 当前流程层级
        /// </summary>
        public System.Int16 process_level { get { return this._process_level; } set { this._process_level = value; } }

        private System.Int16 _state;
        /// <summary>
        /// 状态
        /// </summary>
        public System.Int16 state { get { return this._state; } set { this._state = value; } }

        private System.Int32 _verifier_id;
        /// <summary>
        /// 审核人ID
        /// </summary>
        public System.Int32 verifier_id { get { return this._verifier_id; } set { this._verifier_id = value; } }

        private System.String _verifier;
        /// <summary>
        /// 审核人
        /// </summary>
        public System.String verifier { get { return this._verifier; } set { this._verifier = value?.Trim(); } }

        private System.DateTime _verify_time;
        /// <summary>
        /// 审核时间
        /// </summary>
        public System.DateTime verify_time { get { return this._verify_time; } set { this._verify_time = value; } }

        private System.String _verify_remark;
        /// <summary>
        /// 审核说明
        /// </summary>
        public System.String verify_remark { get { return this._verify_remark; } set { this._verify_remark = value?.Trim(); } }

        private System.Int32 _role_id;
        /// <summary>
        /// 申请角色ID
        /// </summary>
        public System.Int32 role_id { get { return this._role_id; } set { this._role_id = value; } }

        private System.String _role_name;
        /// <summary>
        /// 角色
        /// </summary>
        public System.String role_name { get { return this._role_name; } set { this._role_name = value?.Trim(); } }

        private System.Int32 _employee_id;
        /// <summary>
        /// 申请人员ID
        /// </summary>
        public System.Int32 employee_id { get { return this._employee_id; } set { this._employee_id = value; } }

        private System.String _employee;
        /// <summary>
        /// 人员
        /// </summary>
        public System.String employee { get { return this._employee; } set { this._employee = value?.Trim(); } }

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
    }
}