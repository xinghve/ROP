using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 调拨单
    /// </summary>
    public class bus_transfer_bill
    {
        /// <summary>
        /// 调拨单
        /// </summary>
        public bus_transfer_bill()
        {
        }

        private System.String _bill_no;
        /// <summary>
        /// 调拨单号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String bill_no { get { return this._bill_no; } set { this._bill_no = value?.Trim(); } }

        private System.Int32 _org_id;
        /// <summary>
        /// 机构ID
        /// </summary>
        public System.Int32 org_id { get { return this._org_id; } set { this._org_id = value; } }

        private System.String _apply_no;
        /// <summary>
        /// 申请单号
        /// </summary>
        public System.String apply_no { get { return this._apply_no; } set { this._apply_no = value?.Trim(); } }

        private System.DateTime? _transfer_time;
        /// <summary>
        /// 调拨时间
        /// </summary>
        public System.DateTime? transfer_time { get { return this._transfer_time; } set { this._transfer_time = value; } }

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

        private System.Int16 _state;
        /// <summary>
        /// 状态
        /// </summary>
        public System.Int16 state { get { return this._state; } set { this._state = value; } }

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

        private System.Int32 _out_store_id;
        /// <summary>
        /// 调出门店ID
        /// </summary>
        public System.Int32 out_store_id { get { return this._out_store_id; } set { this._out_store_id = value; } }

        private System.String _out_store_name;
        /// <summary>
        /// 调出门店
        /// </summary>
        public System.String out_store_name { get { return this._out_store_name; } set { this._out_store_name = value?.Trim(); } }

        private System.Int32? _out_employee_id;
        /// <summary>
        /// 调出确认人ID
        /// </summary>
        public System.Int32? out_employee_id { get { return this._out_employee_id; } set { this._out_employee_id = value ?? default(System.Int32); } }

        private System.String _out_employee_name;
        /// <summary>
        /// 调出确认人
        /// </summary>
        public System.String out_employee_name { get { return this._out_employee_name; } set { this._out_employee_name = value?.Trim(); } }

        private System.DateTime? _out_time;
        /// <summary>
        /// 调出确认时间
        /// </summary>
        public System.DateTime? out_time { get { return this._out_time; } set { this._out_time = value; } }

        private System.Int32 _in_store_id;
        /// <summary>
        /// 调入门店ID
        /// </summary>
        public System.Int32 in_store_id { get { return this._in_store_id; } set { this._in_store_id = value; } }

        private System.String _in_store_name;
        /// <summary>
        /// 调入门店
        /// </summary>
        public System.String in_store_name { get { return this._in_store_name; } set { this._in_store_name = value?.Trim(); } }

        private System.Int32? _in_employee_id;
        /// <summary>
        /// 调入确认人ID
        /// </summary>
        public System.Int32? in_employee_id { get { return this._in_employee_id; } set { this._in_employee_id = value ?? default(System.Int32); } }

        private System.String _in_employee_name;
        /// <summary>
        /// 调入确认人
        /// </summary>
        public System.String in_employee_name { get { return this._in_employee_name; } set { this._in_employee_name = value?.Trim(); } }

        private System.DateTime? _in_time;
        /// <summary>
        /// 调入确认时间
        /// </summary>
        public System.DateTime? in_time { get { return this._in_time; } set { this._in_time = value; } }

        private System.String _remark;
        /// <summary>
        /// 备注
        /// </summary>
        public System.String remark { get { return this._remark; } set { this._remark = value?.Trim(); } }

        private System.Int32 _creater_id;
        /// <summary>
        /// 创建人ID
        /// </summary>
        public System.Int32 creater_id { get { return this._creater_id; } set { this._creater_id = value; } }

        private System.String _creater;
        /// <summary>
        /// 创建人
        /// </summary>
        public System.String creater { get { return this._creater; } set { this._creater = value?.Trim(); } }

        private System.DateTime _create_time;
        /// <summary>
        /// 创建时间
        /// </summary>
        public System.DateTime create_time { get { return this._create_time; } set { this._create_time = value; } }

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

        private System.DateTime? _apply_time;
        /// <summary>
        /// 申请时间
        /// </summary>
        public System.DateTime? apply_time { get { return this._apply_time; } set { this._apply_time = value; } }
    }
}