using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 提成记录
    /// </summary>
    public class r_amount
    {
        /// <summary>
        /// 提成记录
        /// </summary>
        public r_amount()
        {
        }

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

        private System.Int32 _archives_id;
        /// <summary>
        /// 档案ID
        /// </summary>
        public System.Int32 archives_id { get { return this._archives_id; } set { this._archives_id = value; } }

        private System.String _card_no;
        /// <summary>
        /// 卡号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String card_no { get { return this._card_no; } set { this._card_no = value?.Trim(); } }

        private System.String _archives;
        /// <summary>
        /// 档案姓名
        /// </summary>
        public System.String archives { get { return this._archives; } set { this._archives = value?.Trim(); } }

        private System.String _archives_phone;
        /// <summary>
        /// 档案手机
        /// </summary>
        public System.String archives_phone { get { return this._archives_phone; } set { this._archives_phone = value?.Trim(); } }

        private System.DateTime _amount_date;
        /// <summary>
        /// 提成时间
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.DateTime amount_date { get { return this._amount_date; } set { this._amount_date = value; } }

        private System.Decimal? _money;
        /// <summary>
        /// 提成金额
        /// </summary>
        public System.Decimal? money { get { return this._money; } set { this._money = value ?? default(System.Decimal); } }

        private System.Int32? _distributor_id;
        /// <summary>
        /// 分销员ID
        /// </summary>
        public System.Int32? distributor_id { get { return this._distributor_id; } set { this._distributor_id = value ?? default(System.Int32); } }

        private System.String _distributor;
        /// <summary>
        /// 分销员
        /// </summary>
        public System.String distributor { get { return this._distributor; } set { this._distributor = value?.Trim(); } }

        private System.Int32? _balance_id;
        /// <summary>
        /// 结算ID
        /// </summary>
        public System.Int32? balance_id { get { return this._balance_id; } set { this._balance_id = value ?? default(System.Int32); } }
    }
}