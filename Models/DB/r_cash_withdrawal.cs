using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 提现记录
    /// </summary>
    public class r_cash_withdrawal
    {
        /// <summary>
        /// 提现记录
        /// </summary>
        public r_cash_withdrawal()
        {
        }

        private System.Int32 _distributor_id;
        /// <summary>
        /// 分销员ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 distributor_id { get { return this._distributor_id; } set { this._distributor_id = value; } }

        private System.DateTime _cash_withdrawal_date;
        /// <summary>
        /// 提现时间
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.DateTime cash_withdrawal_date { get { return this._cash_withdrawal_date; } set { this._cash_withdrawal_date = value; } }

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

        private System.Decimal? _money;
        /// <summary>
        /// 提现金额
        /// </summary>
        public System.Decimal? money { get { return this._money; } set { this._money = value ?? default(System.Decimal); } }

        private System.DateTime? _audit_date;
        /// <summary>
        /// 审核时间
        /// </summary>
        public System.DateTime? audit_date { get { return this._audit_date; } set { this._audit_date = value ?? default(System.DateTime); } }

        private System.Int32? _auditor_id;
        /// <summary>
        /// 审核人ID
        /// </summary>
        public System.Int32? auditor_id { get { return this._auditor_id; } set { this._auditor_id = value ?? default(System.Int32); } }

        private System.String _auditor;
        /// <summary>
        /// 审核人
        /// </summary>
        public System.String auditor { get { return this._auditor; } set { this._auditor = value?.Trim(); } }

        private System.Int16? _state;
        /// <summary>
        /// 状态
        /// </summary>
        public System.Int16? state { get { return this._state; } set { this._state = value ?? default(System.Int16); } }

        private System.String _remarks;
        /// <summary>
        /// 备注
        /// </summary>
        public System.String remarks { get { return this._remarks; } set { this._remarks = value?.Trim(); } }

        private System.DateTime? _finish_date;
        /// <summary>
        /// 到账时间
        /// </summary>
        public System.DateTime? finish_date { get { return this._finish_date; } set { this._finish_date = value ?? default(System.DateTime); } }

        private System.Int32? _transfer_id;
        /// <summary>
        /// 转账人ID
        /// </summary>
        public System.Int32? transfer_id { get { return this._transfer_id; } set { this._transfer_id = value ?? default(System.Int32); } }

        private System.String _transfer;
        /// <summary>
        ///  转账人
        /// </summary>
        public System.String transfer { get { return this._transfer; } set { this._transfer = value?.Trim(); } }
    }
}