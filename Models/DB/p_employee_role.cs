using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 员工角色表
    /// </summary>
    public class p_employee_role
    {
        /// <summary>
        /// 员工角色表
        /// </summary>
        public p_employee_role()
        {
        }

        private System.Int32 _employee_id;
        /// <summary>
        /// 用户Id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 employee_id { get { return this._employee_id; } set { this._employee_id = value; } }

        private System.Int32 _role_id;
        /// <summary>
        /// 角色Id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 role_id { get { return this._role_id; } set { this._role_id = value; } }

        private System.Int32 _org_id;
        /// <summary>
        /// 集团Id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 org_id { get { return this._org_id; } set { this._org_id = value; } }

        private System.Int32 _store_id;
        /// <summary>
        /// 门店Id，同一用户可在多个门店执业
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 store_id { get { return this._store_id; } set { this._store_id = value; } }

        private System.Int32 _dept_id;
        /// <summary>
        /// 部门Id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 dept_id { get { return this._dept_id; } set { this._dept_id = value; } }

        private System.Boolean _is_admin;
        /// <summary>
        /// 是否管理员
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Boolean is_admin { get { return this._is_admin; } set { this._is_admin = value; } }
    }
}