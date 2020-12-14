using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 区域表
    /// </summary>
    public class b_area
    {
        /// <summary>
        /// 区域表
        /// </summary>
        public b_area()
        {
        }

        private System.Int32 _id;
        /// <summary>
        /// 自增主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public System.Int32 id { get { return this._id; } set { this._id = value; } }

        private System.Int32 _parent_id;
        /// <summary>
        /// 父级Id
        /// </summary>
        public System.Int32 parent_id { get { return this._parent_id; } set { this._parent_id = value; } }

        private System.String _code;
        /// <summary>
        /// 区域代码：街道为9位，其它为6位
        /// </summary>
        public System.String code { get { return this._code; } set { this._code = value?.Trim(); } }

        private System.String _name;
        /// <summary>
        /// 区域名称
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