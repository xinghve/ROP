using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 物资基础项目
    /// </summary>
    public class p_std_item
    {
        /// <summary>
        /// 物资基础项目
        /// </summary>
        public p_std_item()
        {
        }

        private System.Int32 _id;
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public System.Int32 id { get { return this._id; } set { this._id = value; } }

        private System.Int32 _org_id;
        /// <summary>
        /// 机构ID
        /// </summary>
        public System.Int32 org_id { get { return this._org_id; } set { this._org_id = value; } }

        private System.String _name;
        /// <summary>
        /// 名称
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.String _spell;
        /// <summary>
        /// 拼音
        /// </summary>
        public System.String spell { get { return this._spell; } set { this._spell = value?.Trim(); } }

        private System.String _code;
        /// <summary>
        /// 编码
        /// </summary>
        public System.String code { get { return this._code; } set { this._code = value?.Trim(); } }

        private System.Int16? _type_id;
        /// <summary>
        /// 类型ID
        /// </summary>
        public System.Int16? type_id { get { return this._type_id; } set { this._type_id = value ?? default(System.Int16); } }

        private System.String _type;
        /// <summary>
        /// 类型
        /// </summary>
        public System.String type { get { return this._type; } set { this._type = value?.Trim(); } }

        private System.Int16? _state;
        /// <summary>
        /// 状态
        /// </summary>
        public System.Int16? state { get { return this._state; } set { this._state = value ?? default(System.Int16); } }

        private System.Int32 _min_num;
        /// <summary>
        /// 最小库存数量
        /// </summary>
        public System.Int32 min_num { get { return this._min_num; } set { this._min_num = value; } }

        private System.String _unit;
        /// <summary>
        /// 单位
        /// </summary>
        public System.String unit { get { return this._unit; } set { this._unit = value?.Trim(); } }
    }
}