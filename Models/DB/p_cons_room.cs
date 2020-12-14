using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 诊室
    /// </summary>
    public class p_cons_room
    {
        /// <summary>
        /// 诊室
        /// </summary>
        public p_cons_room()
        {
        }

        private System.Int32 _id;
        /// <summary>
        /// ID
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

        private System.String _name;
        /// <summary>
        /// 诊室名称
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.String _position;
        /// <summary>
        /// 位置
        /// </summary>
        public System.String position { get { return this._position; } set { this._position = value?.Trim(); } }

        private System.String _remarks;
        /// <summary>
        /// 备注
        /// </summary>
        public System.String remarks { get { return this._remarks; } set { this._remarks = value?.Trim(); } }
    }
}