using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 消费习惯
    /// </summary>
    public class c_habit
    {
        /// <summary>
        /// 消费习惯
        /// </summary>
        public c_habit()
        {
        }

        private System.Int32 _id;
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public System.Int32 id { get { return this._id; } set { this._id = value; } }

        private System.Int32? _org_id;
        /// <summary>
        /// 集团ID
        /// </summary>
        public System.Int32? org_id { get { return this._org_id; } set { this._org_id = value ?? default(System.Int32); } }

        private System.Int32? _store_id;
        /// <summary>
        /// 门店ID
        /// </summary>
        public System.Int32? store_id { get { return this._store_id; } set { this._store_id = value ?? default(System.Int32); } }

        private System.String _code;
        /// <summary>
        /// 消费习惯编码
        /// </summary>
        public System.String code { get { return this._code; } set { this._code = value?.Trim(); } }

        private System.String _name;
        /// <summary>
        /// 消费习惯
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.String _spell;
        /// <summary>
        /// 拼音码（首字母）
        /// </summary>
        public System.String spell { get { return this._spell; } set { this._spell = value?.Trim(); } }
    }
}