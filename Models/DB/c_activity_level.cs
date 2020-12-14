using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 活动会员等级规则
    /// </summary>
    public class c_activity_level
    {
        /// <summary>
        /// 活动会员等级规则
        /// </summary>
        public c_activity_level()
        {
        }

        private System.Int32 _activity_id;
        /// <summary>
        /// 活动id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 activity_id { get { return this._activity_id; } set { this._activity_id = value; } }

        private System.Int32 _level_id;
        /// <summary>
        /// 等级id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 level_id { get { return this._level_id; } set { this._level_id = value; } }

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

        private System.DateTime _start_date;
        /// <summary>
        /// 开始时间
        /// </summary>
        public System.DateTime start_date { get { return this._start_date; } set { this._start_date = value; } }

        private System.DateTime _end_date;
        /// <summary>
        /// 结束时间
        /// </summary>
        public System.DateTime end_date { get { return this._end_date; } set { this._end_date = value; } }

        private System.Int16 _money_integral;
        /// <summary>
        /// 现金积分（x元=1积分）
        /// </summary>
        public System.Int16 money_integral { get { return this._money_integral; } set { this._money_integral = value; } }

        private System.Int16 _integral_money;
        /// <summary>
        /// 积分兑现（x积分=1元）
        /// </summary>
        public System.Int16 integral_money { get { return this._integral_money; } set { this._integral_money = value; } }

        private System.Decimal _discount_rate;
        /// <summary>
        /// 折扣率
        /// </summary>
        public System.Decimal discount_rate { get { return this._discount_rate; } set { this._discount_rate = value; } }

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

        private System.DateTime _create_date;
        /// <summary>
        /// 创建时间
        /// </summary>
        public System.DateTime create_date { get { return this._create_date; } set { this._create_date = value; } }

        private System.String _actity_name;
        /// <summary>
        /// 活动名称
        /// </summary>
        public System.String actity_name { get { return this._actity_name; } set { this._actity_name = value?.Trim(); } }

        private System.String _level_name;
        /// <summary>
        /// 等级名称
        /// </summary>
        public System.String level_name { get { return this._level_name; } set { this._level_name = value?.Trim(); } }

        private System.Decimal? _min_money;
        /// <summary>
        /// 最小金额
        /// </summary>
        public System.Decimal? min_money { get { return this._min_money; } set { this._min_money = value ?? default(System.Decimal); } }

        private System.Decimal? _max_money;
        /// <summary>
        /// 最大金额
        /// </summary>
        public System.Decimal? max_money { get { return this._max_money; } set { this._max_money = value ?? default(System.Decimal); } }
    }
}