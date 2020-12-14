using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 门店功能表
    /// </summary>
    public class p_store_action
    {
        /// <summary>
        /// 门店功能表
        /// </summary>
        public p_store_action()
        {
        }

        private System.Int32 _store_id;
        /// <summary>
        /// 门店Id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 store_id { get { return this._store_id; } set { this._store_id = value; } }

        private System.Int32 _action_id;
        /// <summary>
        /// 功能Id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 action_id { get { return this._action_id; } set { this._action_id = value; } }
    }
}