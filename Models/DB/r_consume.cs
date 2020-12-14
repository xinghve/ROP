using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 消费记录
    /// </summary>
    public class r_consume
    {
        /// <summary>
        /// 消费记录
        /// </summary>
        public r_consume()
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

        private System.DateTime _consume_date;
        /// <summary>
        /// 消费时间
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.DateTime consume_date { get { return this._consume_date; } set { this._consume_date = value; } }

        private System.Decimal? _money;
        /// <summary>
        /// 消费金额
        /// </summary>
        public System.Decimal? money { get { return this._money; } set { this._money = value ?? default(System.Decimal); } }

        private System.Int16? _integral;
        /// <summary>
        /// 减少积分
        /// </summary>
        public System.Int16? integral { get { return this._integral; } set { this._integral = value ?? default(System.Int16); } }

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

        private System.Decimal? _discount_rate;
        /// <summary>
        /// 折扣率
        /// </summary>
        public System.Decimal? discount_rate { get { return this._discount_rate; } set { this._discount_rate = value ?? default(System.Decimal); } }

        private System.String _bill_no;
        /// <summary>
        /// 消费单
        /// </summary>
        public System.String bill_no { get { return this._bill_no; } set { this._bill_no = value?.Trim(); } }

        private System.Int16? _integral_money;
        /// <summary>
        /// 积分兑现（x积分=1元）
        /// </summary>
        public System.Int16? integral_money { get { return this._integral_money; } set { this._integral_money = value ?? default(System.Int16); } }

        private System.String _level;
        /// <summary>
        /// 会员等级
        /// </summary>
        public System.String level { get { return this._level; } set { this._level = value?.Trim(); } }

        private System.Int16? _way_code;
        /// <summary>
        /// 支付方式编码
        /// </summary>
        public System.Int16? way_code { get { return this._way_code; } set { this._way_code = value ?? default(System.Int16); } }

        private System.String _way;
        /// <summary>
        /// 支付方式
        /// </summary>
        public System.String way { get { return this._way; } set { this._way = value?.Trim(); } }

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
    }
}