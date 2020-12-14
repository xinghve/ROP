using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 编码目录表
    /// </summary>
    public class b_code_category
    {
        /// <summary>
        /// 编码目录表
        /// </summary>
        public b_code_category()
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
        /// 目录名称
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.String _pinyin;
        /// <summary>
        /// 拼音码
        /// </summary>
        public System.String pinyin { get { return this._pinyin; } set { this._pinyin = value?.Trim(); } }

        private System.Boolean _is_disabled;
        /// <summary>
        /// 是否禁用
        /// </summary>
        public System.Boolean is_disabled { get { return this._is_disabled; } set { this._is_disabled = value; } }
    }
}