using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 
    /// </summary>
    public class r_logs
    {
        /// <summary>
        /// 
        /// </summary>
        public r_logs()
        {
        }

        private System.String _code;
        /// <summary>
        /// 编码
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String code { get { return this._code; } set { this._code = value?.Trim(); } }

        private System.String _type;
        /// <summary>
        /// 类型
        /// </summary>
        public System.String type { get { return this._type; } set { this._type = value?.Trim(); } }

        private System.DateTime _date;
        /// <summary>
        /// 发生时间
        /// </summary>
        public System.DateTime date { get { return this._date; } set { this._date = value; } }

        private System.String _content;
        /// <summary>
        /// 内容
        /// </summary>
        public System.String content { get { return this._content; } set { this._content = value?.Trim(); } }

        private System.Int16? _state;
        /// <summary>
        /// 状态（0=待处理，1=已处理）
        /// </summary>
        public System.Int16? state { get { return this._state; } set { this._state = value ?? default(System.Int16); } }

        private System.String _parameter;
        /// <summary>
        /// 参数
        /// </summary>
        public System.String parameter { get { return this._parameter; } set { this._parameter = value?.Trim(); } }

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

        private System.String _ip;
        /// <summary>
        /// 客户端IP
        /// </summary>
        public System.String ip { get { return this._ip; } set { this._ip = value?.Trim(); } }

        private System.String _url;
        /// <summary>
        /// 请求地址
        /// </summary>
        public System.String url { get { return this._url; } set { this._url = value?.Trim(); } }

        private System.String _before_data;
        /// <summary>
        /// 修改前数据
        /// </summary>
        public System.String before_data { get { return this._before_data; } set { this._before_data = value?.Trim(); } }

        private System.String _after_data;
        /// <summary>
        /// 修改后数据
        /// </summary>
        public System.String after_data { get { return this._after_data; } set { this._after_data = value?.Trim(); } }
    }
}