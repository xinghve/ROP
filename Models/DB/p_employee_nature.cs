using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 员工性质
    /// </summary>
    public class p_employee_nature
    {
        /// <summary>
        /// 员工性质
        /// </summary>
        public p_employee_nature()
        {
        }

        private System.Int32 _org_id;
        /// <summary>
        /// 组织机构Id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 org_id { get { return this._org_id; } set { this._org_id = value; } }

        private System.Int32 _employee_id;
        /// <summary>
        /// 用户Id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 employee_id { get { return this._employee_id; } set { this._employee_id = value; } }

        private System.Int16 _nature_id;
        /// <summary>
        /// 性质ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int16 nature_id { get { return this._nature_id; } set { this._nature_id = value; } }

        private System.String _nature;
        /// <summary>
        /// 性质
        /// </summary>
        public System.String nature { get { return this._nature; } set { this._nature = value?.Trim(); } }
    }
}