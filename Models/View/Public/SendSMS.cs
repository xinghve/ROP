using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Public
{
    /// <summary>
    /// 短信发送
    /// </summary>
    public class SendSMS
    {
        private System.String _phone_no;
        /// <summary>
        /// 电话
        /// </summary>
        public System.String phone_no { get { return this._phone_no; } set { this._phone_no = value?.Trim(); } }

        private System.Int32 _org_id = 0;
        /// <summary>
        /// 机构Id
        /// </summary>
        public System.Int32 org_id { get { return this._org_id; } set { this._org_id = value; } }

        private System.Int32 _type = 0;
        /// <summary>
        /// 类型（0=用户（默认）；1=客户）
        /// </summary>
        public System.Int32 type { get { return this._type; } set { this._type = value; } }
    }
}
