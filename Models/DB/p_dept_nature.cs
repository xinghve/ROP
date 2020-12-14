using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 
    /// </summary>
    public class p_dept_nature
    {
        /// <summary>
        /// 
        /// </summary>
        public p_dept_nature()
        {
        }

        private System.Int32 _dept_id;
        /// <summary>
        /// 部门id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 dept_id { get { return this._dept_id; } set { this._dept_id = value; } }

        private System.Int32 _org_id;
        /// <summary>
        /// 集团id
        /// </summary>
        public System.Int32 org_id { get { return this._org_id; } set { this._org_id = value; } }

        private System.Int16 _nature_id;
        /// <summary>
        /// 性质id
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