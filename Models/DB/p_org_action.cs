using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 集团功能表
    /// </summary>
    public class p_org_action
    {
        /// <summary>
        /// 集团功能表
        /// </summary>
        public p_org_action()
        {
        }

        private System.Int32 _org_id;
        /// <summary>
        /// 集团Id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 org_id { get { return this._org_id; } set { this._org_id = value; } }

        private System.Int32 _action_id;
        /// <summary>
        /// 功能Id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 action_id { get { return this._action_id; } set { this._action_id = value; } }

        private System.Int16 _action_type;
        /// <summary>
        /// 菜单类型
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int16 action_type { get { return this._action_type; } set { this._action_type = value; } }
    }
}