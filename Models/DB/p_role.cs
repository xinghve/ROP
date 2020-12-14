using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 角色表
    /// </summary>
    public class p_role
    {
        /// <summary>
        /// 角色表
        /// </summary>
        public p_role()
        {
        }

        private System.Int32 _id;
        /// <summary>
        /// 自增主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public System.Int32 id { get { return this._id; } set { this._id = value; } }

        private System.String _name;
        /// <summary>
        /// 名称
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.Int32 _store_id;
        /// <summary>
        /// 门店Id
        /// </summary>
        public System.Int32 store_id { get { return this._store_id; } set { this._store_id = value; } }

        private System.Int32 _org_id;
        /// <summary>
        /// 机构Id
        /// </summary>
        public System.Int32 org_id { get { return this._org_id; } set { this._org_id = value; } }

        private System.Int32 _dept_id;
        /// <summary>
        /// 部门ID
        /// </summary>
        public System.Int32 dept_id { get { return this._dept_id; } set { this._dept_id = value; } }

        private System.String _pinyin;
        /// <summary>
        /// 拼音码
        /// </summary>
        public System.String pinyin { get { return this._pinyin; } set { this._pinyin = value?.Trim(); } }

        private System.String _disabled;
        /// <summary>
        /// 状态（0=停用，1=启用）
        /// </summary>
        public System.String disabled { get { return this._disabled; } set { this._disabled = value?.Trim(); } }

        private System.Int16? _disabled_code;
        /// <summary>
        /// 状态编码
        /// </summary>
        public System.Int16? disabled_code { get { return this._disabled_code; } set { this._disabled_code = value ?? default(System.Int16); } }

        private System.String _introduce;
        /// <summary>
        /// 简介
        /// </summary>
        public System.String introduce { get { return this._introduce; } set { this._introduce = value?.Trim(); } }

        private System.Int32? _link_id;
        /// <summary>
        /// 关联ID
        /// </summary>
        public System.Int32? link_id { get { return this._link_id; } set { this._link_id = value ?? default(System.Int32); } }
    }
}