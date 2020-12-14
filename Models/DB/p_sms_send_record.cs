using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 发送记录
    /// </summary>
    public class p_sms_send_record
    {
        /// <summary>
        /// 发送记录
        /// </summary>
        public p_sms_send_record()
        {
        }

        private System.Int32 _autograph_id;
        /// <summary>
        /// 签名ID
        /// </summary>
        public System.Int32 autograph_id { get { return this._autograph_id; } set { this._autograph_id = value; } }

        private System.Int32 _template_id;
        /// <summary>
        /// 模板ID
        /// </summary>
        public System.Int32 template_id { get { return this._template_id; } set { this._template_id = value; } }

        private System.String _phone_no;
        /// <summary>
        /// 手机号
        /// </summary>
        public System.String phone_no { get { return this._phone_no; } set { this._phone_no = value?.Trim(); } }

        private System.Int16 _status;
        /// <summary>
        /// 状态（1：成功；0：失败）
        /// </summary>
        public System.Int16 status { get { return this._status; } set { this._status = value; } }

        private System.String _error;
        /// <summary>
        /// 错误信息
        /// </summary>
        public System.String error { get { return this._error; } set { this._error = value?.Trim(); } }

        private System.String _scene;
        /// <summary>
        /// 使用场景
        /// </summary>
        public System.String scene { get { return this._scene; } set { this._scene = value?.Trim(); } }

        private System.String _code;
        /// <summary>
        /// 验证码
        /// </summary>
        public System.String code { get { return this._code; } set { this._code = value?.Trim(); } }

        private System.Int32 _org_id;
        /// <summary>
        /// 机构ID
        /// </summary>
        public System.Int32 org_id { get { return this._org_id; } set { this._org_id = value; } }

        private System.DateTime _send_time;
        /// <summary>
        /// 发送时间
        /// </summary>
        public System.DateTime send_time { get { return this._send_time; } set { this._send_time = value; } }

        private System.Int32 _store_id;
        /// <summary>
        /// 门店ID
        /// </summary>
        public System.Int32 store_id { get { return this._store_id; } set { this._store_id = value; } }

        private System.DateTime? _expire_time;
        /// <summary>
        /// 失效时间
        /// </summary>
        public System.DateTime? expire_time { get { return this._expire_time; } set { this._expire_time = value ?? default(System.DateTime); } }

        private System.Int32? _sender_id;
        /// <summary>
        /// 发送人ID
        /// </summary>
        public System.Int32? sender_id { get { return this._sender_id; } set { this._sender_id = value ?? default(System.Int32); } }

        private System.String _sender;
        /// <summary>
        /// 发送人
        /// </summary>
        public System.String sender { get { return this._sender; } set { this._sender = value?.Trim(); } }

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

        private System.String _id_card;
        /// <summary>
        /// 身份证号
        /// </summary>
        public System.String id_card { get { return this._id_card; } set { this._id_card = value?.Trim(); } }
    }
}