using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 采购申请单
    /// </summary>
    public class bus_buy_apply
    {
        /// <summary>
        /// 采购申请单
        /// </summary>
        public bus_buy_apply()
        {
        }

        private System.String _apply_no;
        /// <summary>
        /// 主键ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String apply_no { get { return this._apply_no; } set { this._apply_no = value?.Trim(); } }

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

        private System.String _store;
        /// <summary>
        /// 门店
        /// </summary>
        public System.String store { get { return this._store; } set { this._store = value?.Trim(); } }

        private System.Int16? _state;
        /// <summary>
        /// 状态
        /// </summary>
        public System.Int16? state { get { return this._state; } set { this._state = value ?? default(System.Int16); } }

        private System.String _remark;
        /// <summary>
        /// 备注
        /// </summary>
        public System.String remark { get { return this._remark; } set { this._remark = value?.Trim(); } }

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

        private System.Int32? _org_process_id;
        /// <summary>
        /// 流程ID
        /// </summary>
        public System.Int32? org_process_id { get { return this._org_process_id; } set { this._org_process_id = value ?? default(System.Int32); } }

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
        public System.DateTime? verify_time { get { return this._verify_time; } set { this._verify_time = value; } }

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

        private System.Int32? _applicant_id;
        /// <summary>
        /// 申请人ID
        /// </summary>
        public System.Int32? applicant_id { get { return this._applicant_id; } set { this._applicant_id = value ?? default(System.Int32); } }

        private System.String _applicant;
        /// <summary>
        /// 申请人
        /// </summary>
        public System.String applicant { get { return this._applicant; } set { this._applicant = value?.Trim(); } }

        private System.DateTime? _apply_time;
        /// <summary>
        /// 申请时间
        /// </summary>
        public System.DateTime? apply_time { get { return this._apply_time; } set { this._apply_time = value; } }

        private System.DateTime? _create_time;
        /// <summary>
        /// 创建时间
        /// </summary>
        public System.DateTime? create_time { get { return this._create_time; } set { this._create_time = value; } }

        private System.Decimal? _total_price;
        /// <summary>
        /// 总价
        /// </summary>
        public System.Decimal? total_price { get { return this._total_price; } set { this._total_price = value ?? default(System.Decimal); } }

        private System.Boolean? _is_org;
        /// <summary>
        /// 是否机构
        /// </summary>
        public System.Boolean? is_org { get { return this._is_org; } set { this._is_org = value ?? default(System.Boolean); } }

        private System.String _delete_no;
        /// <summary>
        /// 作废对应单号
        /// </summary>
        public System.String delete_no { get { return this._delete_no; } set { this._delete_no = value?.Trim(); } }
    }
}