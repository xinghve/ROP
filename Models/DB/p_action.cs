using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 功能表
    /// </summary>
    public class p_action
    {
        /// <summary>
        /// 功能表
        /// </summary>
        public p_action()
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
        /// 功能名称
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.String _pinyin;
        /// <summary>
        /// 拼音码
        /// </summary>
        public System.String pinyin { get { return this._pinyin; } set { this._pinyin = value?.Trim(); } }

        private System.Int32 _parent_id;
        /// <summary>
        /// 上级Id
        /// </summary>
        public System.Int32 parent_id { get { return this._parent_id; } set { this._parent_id = value; } }

        private System.Int16 _level;
        /// <summary>
        /// 层级
        /// </summary>
        public System.Int16 level { get { return this._level; } set { this._level = value; } }

        private System.Int16 _is_action;
        /// <summary>
        /// 是否功能，否表示目录
        /// </summary>
        public System.Int16 is_action { get { return this._is_action; } set { this._is_action = value; } }

        private System.String _uri;
        /// <summary>
        /// 资源地址
        /// </summary>
        public System.String uri { get { return this._uri; } set { this._uri = value?.Trim(); } }

        private System.String _icon;
        /// <summary>
        /// 图标
        /// </summary>
        public System.String icon { get { return this._icon; } set { this._icon = value?.Trim(); } }

        private System.String _description;
        /// <summary>
        /// 功能描述
        /// </summary>
        public System.String description { get { return this._description; } set { this._description = value?.Trim(); } }

        private System.Int16 _action_type;
        /// <summary>
        /// 0-平台功能 1-集团功能 2-门店功能
        /// </summary>
        public System.Int16 action_type { get { return this._action_type; } set { this._action_type = value; } }
    }
}