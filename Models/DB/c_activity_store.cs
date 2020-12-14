using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 活动门店
    /// </summary>
    public class c_activity_store
    {
        /// <summary>
        /// 活动门店
        /// </summary>
        public c_activity_store()
        {
        }

        private System.Int32 _activity_id;
        /// <summary>
        /// 活动ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 activity_id { get { return this._activity_id; } set { this._activity_id = value; } }

        private System.Int32 _store_id;
        /// <summary>
        /// 门店id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 store_id { get { return this._store_id; } set { this._store_id = value; } }
    }
}