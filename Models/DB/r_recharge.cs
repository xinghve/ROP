using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 充值记录
    /// </summary>
    public class r_recharge
    {
        /// <summary>
        /// 充值记录
        /// </summary>
        public r_recharge()
        {
        }

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

        private System.DateTime _recharge_date;
        /// <summary>
        /// 充值时间
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.DateTime recharge_date { get { return this._recharge_date; } set { this._recharge_date = value; } }

        private System.Decimal _recharge_money;
        /// <summary>
        /// 充值金额
        /// </summary>
        public System.Decimal recharge_money { get { return this._recharge_money; } set { this._recharge_money = value; } }

        private System.Int16? _recharge_integral;
        /// <summary>
        /// 增加积分
        /// </summary>
        public System.Int16? recharge_integral { get { return this._recharge_integral; } set { this._recharge_integral = value ?? default(System.Int16); } }

        private System.Decimal? _total_money;
        /// <summary>
        /// 总金额
        /// </summary>
        public System.Decimal? total_money { get { return this._total_money; } set { this._total_money = value ?? default(System.Decimal); } }

        private System.Int32? _total_integral;
        /// <summary>
        /// 总积分
        /// </summary>
        public System.Int32? total_integral { get { return this._total_integral; } set { this._total_integral = value ?? default(System.Int32); } }

        private System.String _way_code;
        /// <summary>
        /// 支付方式编码
        /// </summary>
        public System.String way_code { get { return this._way_code; } set { this._way_code = value?.Trim(); } }

        private System.String _way;
        /// <summary>
        /// 支付方式
        /// </summary>
        public System.String way { get { return this._way; } set { this._way = value?.Trim(); } }

        private System.String _bill_no;
        /// <summary>
        /// 流水号
        /// </summary>
        public System.String bill_no { get { return this._bill_no; } set { this._bill_no = value?.Trim(); } }

        private System.DateTime _occurrence_date;
        /// <summary>
        /// 发生时间
        /// </summary>
        public System.DateTime occurrence_date { get { return this._occurrence_date; } set { this._occurrence_date = value; } }

        private System.String _level;
        /// <summary>
        /// 会员等级
        /// </summary>
        public System.String level { get { return this._level; } set { this._level = value?.Trim(); } }

        private System.Int16? _money_integral;
        /// <summary>
        /// 现金积分（x元=1积分）
        /// </summary>
        public System.Int16? money_integral { get { return this._money_integral; } set { this._money_integral = value ?? default(System.Int16); } }

        private System.Int32? _org_id;
        /// <summary>
        /// 机构id
        /// </summary>
        public System.Int32? org_id { get { return this._org_id; } set { this._org_id = value ?? default(System.Int32); } }

        private System.Int32? _store_id;
        /// <summary>
        /// 门店id
        /// </summary>
        public System.Int32? store_id { get { return this._store_id; } set { this._store_id = value ?? default(System.Int32); } }

        private System.Decimal? _balance;
        /// <summary>
        /// 余额
        /// </summary>
        public System.Decimal? balance { get { return this._balance; } set { this._balance = value ?? default(System.Decimal); } }

        private System.Int32? _integral;
        /// <summary>
        /// 剩余积分
        /// </summary>
        public System.Int32? integral { get { return this._integral; } set { this._integral = value ?? default(System.Int32); } }

        private System.Decimal? _give_total_money;
        /// <summary>
        /// 赠送总金额
        /// </summary>
        public System.Decimal? give_total_money { get { return this._give_total_money; } set { this._give_total_money = value ?? default(System.Decimal); } }

        private System.Decimal? _give_balance;
        /// <summary>
        /// 赠送余额
        /// </summary>
        public System.Decimal? give_balance { get { return this._give_balance; } set { this._give_balance = value ?? default(System.Decimal); } }

        private System.Decimal? _give_money;
        /// <summary>
        /// 赠送金额
        /// </summary>
        public System.Decimal? give_money { get { return this._give_money; } set { this._give_money = value ?? default(System.Decimal); } }

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

        private System.Int32? _consultation_id;
        /// <summary>
        /// 就诊id
        /// </summary>
        public System.Int32? consultation_id { get { return this._consultation_id; } set { this._consultation_id = value ?? default(System.Int32); } }

        private System.String _no;
        /// <summary>
        /// 单据号
        /// </summary>
        public System.String no { get { return this._no; } set { this._no = value?.Trim(); } }

        private System.String _bank;
        /// <summary>
        /// 银行
        /// </summary>
        public System.String bank { get { return this._bank; } set { this._bank = value?.Trim(); } }

        private System.Int32? _check_id;
        /// <summary>
        /// 结账id
        /// </summary>
        public System.Int32? check_id { get { return this._check_id; } set { this._check_id = value ?? default(System.Int32); } }

        private System.Int16? _state_id;
        /// <summary>
        /// 状态id
        /// </summary>
        public System.Int16? state_id { get { return this._state_id; } set { this._state_id = value ?? default(System.Int16); } }

        private System.String _state;
        /// <summary>
        /// 状态
        /// </summary>
        public System.String state { get { return this._state; } set { this._state = value?.Trim(); } }

        private System.String _remark;
        /// <summary>
        /// 摘要
        /// </summary>
        public System.String remark { get { return this._remark; } set { this._remark = value?.Trim(); } }

        private System.Int16? _categroy_id;
        /// <summary>
        /// 分类id
        /// </summary>
        public System.Int16? categroy_id { get { return this._categroy_id; } set { this._categroy_id = value ?? default(System.Int16); } }

        private System.Int32? _to_director_id;
        /// <summary>
        /// 现负责人ID
        /// </summary>
        public System.Int32? to_director_id { get { return this._to_director_id; } set { this._to_director_id = value ?? default(System.Int32); } }

        private System.String _to_director;
        /// <summary>
        /// 现负责人
        /// </summary>
        public System.String to_director { get { return this._to_director; } set { this._to_director = value?.Trim(); } }

        private System.String _coupon_no;
        /// <summary>
        /// 券码
        /// </summary>
        public System.String coupon_no { get { return this._coupon_no; } set { this._coupon_no = value?.Trim(); } }

        private System.Decimal _discount_rate;
        /// <summary>
        /// 折扣率
        /// </summary>
        public System.Decimal discount_rate { get { return this._discount_rate; } set { this._discount_rate = value; } }
    }
}