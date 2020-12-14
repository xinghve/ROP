using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 客户所属分销人员
    /// </summary>
    public class c_distributor
    {
        /// <summary>
        /// 客户所属分销人员
        /// </summary>
        public c_distributor()
        {
        }

        private System.Int32 _distributor_id;
        /// <summary>
        /// 分销人员ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 distributor_id { get { return this._distributor_id; } set { this._distributor_id = value; } }

        private System.Int32 _archives_id;
        /// <summary>
        /// 客户ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 archives_id { get { return this._archives_id; } set { this._archives_id = value; } }
    }
}