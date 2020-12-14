using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 角色功能表
    /// </summary>
    public class p_role_action
    {
        /// <summary>
        /// 角色功能表
        /// </summary>
        public p_role_action()
        {
        }

        private System.Int32 _role_id;
        /// <summary>
        /// 角色Id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 role_id { get { return this._role_id; } set { this._role_id = value; } }

        private System.Int32 _action_id;
        /// <summary>
        /// 功能Id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 action_id { get { return this._action_id; } set { this._action_id = value; } }
    }
}