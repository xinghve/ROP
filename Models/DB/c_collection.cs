using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 收藏医生
    /// </summary>
    public class c_collection
    {
        /// <summary>
        /// 收藏医生
        /// </summary>
        public c_collection()
        {
        }

        private System.Int32 _archives_id;
        /// <summary>
        /// 客户id
        /// </summary>
        public System.Int32 archives_id { get { return this._archives_id; } set { this._archives_id = value; } }

        private System.Int32 _doctor_id;
        /// <summary>
        /// 医生id
        /// </summary>
        public System.Int32 doctor_id { get { return this._doctor_id; } set { this._doctor_id = value; } }

        private System.Int32 _org_id;
        /// <summary>
        /// 机构id
        /// </summary>
        public System.Int32 org_id { get { return this._org_id; } set { this._org_id = value; } }

        private System.Int32 _store_id;
        /// <summary>
        /// 门店id
        /// </summary>
        public System.Int32 store_id { get { return this._store_id; } set { this._store_id = value; } }

        private System.Int32 _dept_id;
        /// <summary>
        /// 科室id
        /// </summary>
        public System.Int32 dept_id { get { return this._dept_id; } set { this._dept_id = value; } }
    }
}