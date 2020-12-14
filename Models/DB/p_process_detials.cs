using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 流程明细
    /// </summary>
    public class p_process_detials
    {
        /// <summary>
        /// 流程明细
        /// </summary>
        public p_process_detials()
        {
        }

        private System.Int32 _id;
        /// <summary>
        /// 流程ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 id { get { return this._id; } set { this._id = value; } }

        private System.Int16 _level;
        /// <summary>
        /// 层级
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int16 level { get { return this._level; } set { this._level = value; } }

        private System.Int32 _role_id;
        /// <summary>
        /// 角色ID
        /// </summary>
        public System.Int32 role_id { get { return this._role_id; } set { this._role_id = value; } }

        private System.String _role_name;
        /// <summary>
        /// 角色
        /// </summary>
        public System.String role_name { get { return this._role_name; } set { this._role_name = value?.Trim(); } }

        private System.Int32? _employee_id;
        /// <summary>
        /// 人员ID
        /// </summary>
        public System.Int32? employee_id { get { return this._employee_id; } set { this._employee_id = value ?? default(System.Int32); } }

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
        /// 部门
        /// </summary>
        public System.String dept_name { get { return this._dept_name; } set { this._dept_name = value?.Trim(); } }

        private System.Boolean? _is_org;
        /// <summary>
        /// 是否机构
        /// </summary>
        public System.Boolean? is_org { get { return this._is_org; } set { this._is_org = value ?? default(System.Boolean); } }
    }
}