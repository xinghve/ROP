using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 设备器械
    /// </summary>
    public class p_equipment
    {
        /// <summary>
        /// 设备器械
        /// </summary>
        public p_equipment()
        {
        }

        private System.Int32 _id;
        /// <summary>
        /// 设备ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public System.Int32 id { get { return this._id; } set { this._id = value; } }

        private System.Int32? _org_id;
        /// <summary>
        /// 机构ID
        /// </summary>
        public System.Int32? org_id { get { return this._org_id; } set { this._org_id = value ?? default(System.Int32); } }

        private System.Int32? _store_id;
        /// <summary>
        /// 门店ID
        /// </summary>
        public System.Int32? store_id { get { return this._store_id; } set { this._store_id = value ?? default(System.Int32); } }

        private System.String _store_name;
        /// <summary>
        /// 门店名称
        /// </summary>
        public System.String store_name { get { return this._store_name; } set { this._store_name = value?.Trim(); } }

        private System.String _name;
        /// <summary>
        /// 设备名称
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.String _model;
        /// <summary>
        /// 型号
        /// </summary>
        public System.String model { get { return this._model; } set { this._model = value?.Trim(); } }

        private System.Int32? _manufactor_id;
        /// <summary>
        /// 供货商ID
        /// </summary>
        public System.Int32? manufactor_id { get { return this._manufactor_id; } set { this._manufactor_id = value ?? default(System.Int32); } }

        private System.String _manufactor;
        /// <summary>
        /// 供货商
        /// </summary>
        public System.String manufactor { get { return this._manufactor; } set { this._manufactor = value?.Trim(); } }
    }
}