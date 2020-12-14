using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 商品库存
    /// </summary>
    public class bus_storage
    {
        /// <summary>
        /// 商品库存
        /// </summary>
        public bus_storage()
        {
        }

        private System.Int32 _id;
        /// <summary>
        /// 库存id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public System.Int32 id { get { return this._id; } set { this._id = value; } }

        private System.Int32 _org_id;
        /// <summary>
        /// 机构ID
        /// </summary>
        public System.Int32 org_id { get { return this._org_id; } set { this._org_id = value; } }

        private System.Int32 _store_id;
        /// <summary>
        /// 门店ID
        /// </summary>
        public System.Int32 store_id { get { return this._store_id; } set { this._store_id = value; } }

        private System.String _store_name;
        /// <summary>
        /// 门店名称
        /// </summary>
        public System.String store_name { get { return this._store_name; } set { this._store_name = value?.Trim(); } }

        private System.Int32 _std_item_id;
        /// <summary>
        /// 物资基础项目ID
        /// </summary>
        public System.Int32 std_item_id { get { return this._std_item_id; } set { this._std_item_id = value; } }

        private System.String _name;
        /// <summary>
        /// 名称
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.Int16 _type_id;
        /// <summary>
        /// 类型ID
        /// </summary>
        public System.Int16 type_id { get { return this._type_id; } set { this._type_id = value; } }

        private System.String _type;
        /// <summary>
        /// 类型
        /// </summary>
        public System.String type { get { return this._type; } set { this._type = value?.Trim(); } }

        private System.String _unit;
        /// <summary>
        /// 单位
        /// </summary>
        public System.String unit { get { return this._unit; } set { this._unit = value?.Trim(); } }

        private System.Int32 _use_num;
        /// <summary>
        /// 可用数量
        /// </summary>
        public System.Int32 use_num { get { return this._use_num; } set { this._use_num = value; } }

        private System.Int32 _num;
        /// <summary>
        /// 实际数量
        /// </summary>
        public System.Int32 num { get { return this._num; } set { this._num = value; } }

        private System.Int32 _min_num;
        /// <summary>
        /// 最小库存数量
        /// </summary>
        public System.Int32 min_num { get { return this._min_num; } set { this._min_num = value; } }
    }
}