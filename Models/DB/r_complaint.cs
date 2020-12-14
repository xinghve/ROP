using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 
    /// </summary>
    public class r_complaint
    {
        /// <summary>
        /// 
        /// </summary>
        public r_complaint()
        {
        }

        private System.Int32 _id;
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public System.Int32 id { get { return this._id; } set { this._id = value; } }

        private System.Int32? _archives_id;
        /// <summary>
        /// 档案ID
        /// </summary>
        public System.Int32? archives_id { get { return this._archives_id; } set { this._archives_id = value ?? default(System.Int32); } }

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

        private System.String _content;
        /// <summary>
        /// 内容
        /// </summary>
        public System.String content { get { return this._content; } set { this._content = value?.Trim(); } }

        private System.DateTime? _date;
        /// <summary>
        /// 时间
        /// </summary>
        public System.DateTime? date { get { return this._date; } set { this._date = value ?? default(System.DateTime); } }

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

        private System.Int16? _state;
        /// <summary>
        /// 状态
        /// </summary>
        public System.Int16? state { get { return this._state; } set { this._state = value ?? default(System.Int16); } }

        private System.DateTime? _deal_time;
        /// <summary>
        /// 处理时间
        /// </summary>
        public System.DateTime? deal_time { get { return this._deal_time; } set { this._deal_time = value ?? default(System.DateTime); } }

        private System.Int32? _dealer_id;
        /// <summary>
        /// 处理人ID
        /// </summary>
        public System.Int32? dealer_id { get { return this._dealer_id; } set { this._dealer_id = value ?? default(System.Int32); } }

        private System.String _dealer;
        /// <summary>
        /// 处理人
        /// </summary>
        public System.String dealer { get { return this._dealer; } set { this._dealer = value?.Trim(); } }

        private System.String _result;
        /// <summary>
        /// 处理结果
        /// </summary>
        public System.String result { get { return this._result; } set { this._result = value?.Trim(); } }
    }
}