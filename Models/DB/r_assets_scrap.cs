using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 固定资产报废
    /// </summary>
    public class r_assets_scrap
    {
        /// <summary>
        /// 固定资产报废
        /// </summary>
        public r_assets_scrap()
        {
        }

        private System.Int32 _id;
        /// <summary>
        /// 主键ID
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

        private System.String _store;
        /// <summary>
        /// 门店
        /// </summary>
        public System.String store { get { return this._store; } set { this._store = value?.Trim(); } }

        private System.String _no;
        /// <summary>
        /// 单号
        /// </summary>
        public System.String no { get { return this._no; } set { this._no = value?.Trim(); } }

        private System.Int16? _state;
        /// <summary>
        /// 状态
        /// </summary>
        public System.Int16? state { get { return this._state; } set { this._state = value ?? default(System.Int16); } }

        private System.Int32 _assets_id;
        /// <summary>
        /// 固定资产ID
        /// </summary>
        public System.Int32 assets_id { get { return this._assets_id; } set { this._assets_id = value; } }

        private System.String _assets_no;
        /// <summary>
        /// 固定资产编号
        /// </summary>
        public System.String assets_no { get { return this._assets_no; } set { this._assets_no = value?.Trim(); } }

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

        private System.String _spec;
        /// <summary>
        /// 规格
        /// </summary>
        public System.String spec { get { return this._spec; } set { this._spec = value?.Trim(); } }

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

        private System.DateTime _buy_date;
        /// <summary>
        /// 购置日期
        /// </summary>
        public System.DateTime buy_date { get { return this._buy_date; } set { this._buy_date = value; } }

        private System.String _remark;
        /// <summary>
        /// 备注
        /// </summary>
        public System.String remark { get { return this._remark; } set { this._remark = value?.Trim(); } }

        private System.Int32? _process_id;
        /// <summary>
        /// 流程ID
        /// </summary>
        public System.Int32? process_id { get { return this._process_id; } set { this._process_id = value ?? default(System.Int32); } }

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

        private System.Boolean? _is_org;
        /// <summary>
        /// 是否机构
        /// </summary>
        public System.Boolean? is_org { get { return this._is_org; } set { this._is_org = value ?? default(System.Boolean); } }

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

        private System.Int32 _applicant_id;
        /// <summary>
        /// 申请人ID
        /// </summary>
        public System.Int32 applicant_id { get { return this._applicant_id; } set { this._applicant_id = value; } }

        private System.String _applicant;
        /// <summary>
        /// 申请人
        /// </summary>
        public System.String applicant { get { return this._applicant; } set { this._applicant = value?.Trim(); } }

        private System.DateTime _apply_time;
        /// <summary>
        /// 申请时间
        /// </summary>
        public System.DateTime apply_time { get { return this._apply_time; } set { this._apply_time = value; } }
    }
}