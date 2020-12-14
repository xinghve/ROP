using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 
    /// </summary>
    public class bus_loss_overflow
    {
        /// <summary>
        /// 
        /// </summary>
        public bus_loss_overflow()
        {
        }

        private System.String _no;
        /// <summary>
        /// 单号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String no { get { return this._no; } set { this._no = value?.Trim(); } }

        private System.String _realted_no;
        /// <summary>
        /// 关联单号
        /// </summary>
        public System.String realted_no { get { return this._realted_no; } set { this._realted_no = value?.Trim(); } }

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

        private System.Int16? _state;
        /// <summary>
        /// 状态
        /// </summary>
        public System.Int16? state { get { return this._state; } set { this._state = value ?? default(System.Int16); } }

        private System.Int16 _total_level;
        /// <summary>
        /// 总层级
        /// </summary>
        public System.Int16 total_level { get { return this._total_level; } set { this._total_level = value; } }

        private System.Int16 _level;
        /// <summary>
        /// 层级
        /// </summary>
        public System.Int16 level { get { return this._level; } set { this._level = value; } }

        private System.Int32? _verifier_id;
        /// <summary>
        /// 审核人ID
        /// </summary>
        public System.Int32? verifier_id { get { return this._verifier_id; } set { this._verifier_id = value ?? default(System.Int32); } }

        private System.String _verifier;
        /// <summary>
        /// 审核人
        /// </summary>
        public System.String verifier { get { return this._verifier; } set { this._verifier = value?.Trim(); } }

        private System.DateTime? _verify_time;
        /// <summary>
        /// 审核时间
        /// </summary>
        public System.DateTime? verify_time { get { return this._verify_time; } set { this._verify_time = value ?? default(System.DateTime); } }

        private System.String _verify_remark;
        /// <summary>
        /// 审核说明
        /// </summary>
        public System.String verify_remark { get { return this._verify_remark; } set { this._verify_remark = value?.Trim(); } }

        private System.Int32? _await_verifier_id;
        /// <summary>
        /// 待审核人ID
        /// </summary>
        public System.Int32? await_verifier_id { get { return this._await_verifier_id; } set { this._await_verifier_id = value ?? default(System.Int32); } }

        private System.String _await_verifier;
        /// <summary>
        /// 待审核人
        /// </summary>
        public System.String await_verifier { get { return this._await_verifier; } set { this._await_verifier = value?.Trim(); } }

        private System.String _remark;
        /// <summary>
        /// 备注
        /// </summary>
        public System.String remark { get { return this._remark; } set { this._remark = value?.Trim(); } }

        private System.Int32? _creater_id;
        /// <summary>
        /// 创建人ID
        /// </summary>
        public System.Int32? creater_id { get { return this._creater_id; } set { this._creater_id = value ?? default(System.Int32); } }

        private System.String _creater;
        /// <summary>
        /// 创建人
        /// </summary>
        public System.String creater { get { return this._creater; } set { this._creater = value?.Trim(); } }

        private System.DateTime? _create_time;
        /// <summary>
        /// 创建时间
        /// </summary>
        public System.DateTime? create_time { get { return this._create_time; } set { this._create_time = value ?? default(System.DateTime); } }

        private System.String _delete_no;
        /// <summary>
        /// 作废对应单号
        /// </summary>
        public System.String delete_no { get { return this._delete_no; } set { this._delete_no = value?.Trim(); } }

        private System.Int32? _process_id;
        /// <summary>
        /// 流程ID
        /// </summary>
        public System.Int32? process_id { get { return this._process_id; } set { this._process_id = value ?? default(System.Int32); } }

        private System.Boolean? _is_org;
        /// <summary>
        /// 是否机构
        /// </summary>
        public System.Boolean? is_org { get { return this._is_org; } set { this._is_org = value ?? default(System.Boolean); } }

        private System.Int16? _type_id;
        /// <summary>
        /// 单据类型（1=报溢；2=报损；）
        /// </summary>
        public System.Int16? type_id { get { return this._type_id; } set { this._type_id = value ?? default(System.Int16); } }

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

        private System.Int16 _item_type_id;
        /// <summary>
        /// 类型ID
        /// </summary>
        public System.Int16 item_type_id { get { return this._item_type_id; } set { this._item_type_id = value; } }

        private System.String _item_type;
        /// <summary>
        /// 类型
        /// </summary>
        public System.String item_type { get { return this._item_type; } set { this._item_type = value?.Trim(); } }

        private System.String _spec;
        /// <summary>
        /// 规格
        /// </summary>
        public System.String spec { get { return this._spec; } set { this._spec = value?.Trim(); } }

        private System.Int32 _manufactor_id;
        /// <summary>
        /// 厂家ID
        /// </summary>
        public System.Int32 manufactor_id { get { return this._manufactor_id; } set { this._manufactor_id = value; } }

        private System.String _manufactor;
        /// <summary>
        /// 厂家
        /// </summary>
        public System.String manufactor { get { return this._manufactor; } set { this._manufactor = value?.Trim(); } }

        private System.String _unit;
        /// <summary>
        /// 单位
        /// </summary>
        public System.String unit { get { return this._unit; } set { this._unit = value?.Trim(); } }

        private System.Int32 _num;
        /// <summary>
        /// 数量
        /// </summary>
        public System.Int32 num { get { return this._num; } set { this._num = value; } }

        private System.Int16 _property_id;
        /// <summary>
        /// 属性ID
        /// </summary>
        public System.Int16 property_id { get { return this._property_id; } set { this._property_id = value; } }
    }
}