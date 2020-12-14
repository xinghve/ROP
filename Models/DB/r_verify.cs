using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 
    /// </summary>
    public class r_verify
    {
        /// <summary>
        /// 
        /// </summary>
        public r_verify()
        {
        }

        private System.String _realted_no;
        /// <summary>
        /// 关联单号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String realted_no { get { return this._realted_no; } set { this._realted_no = value?.Trim(); } }

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

        private System.Int16 _state;
        /// <summary>
        /// 状态
        /// </summary>
        public System.Int16 state { get { return this._state; } set { this._state = value; } }

        private System.Int16 _process_type_id;
        /// <summary>
        /// 流程类型ID
        /// </summary>
        public System.Int16 process_type_id { get { return this._process_type_id; } set { this._process_type_id = value; } }

        private System.String _process_type;
        /// <summary>
        /// 流程类型
        /// </summary>
        public System.String process_type { get { return this._process_type; } set { this._process_type = value?.Trim(); } }

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

        private System.Int32 _process_id;
        /// <summary>
        /// 流程ID
        /// </summary>
        public System.Int32 process_id { get { return this._process_id; } set { this._process_id = value; } }
    }
}