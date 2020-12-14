using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Mobile
{
    /// <summary>
    /// 客户登录
    /// </summary>
    public class ArcLoginPassword
    {
        private System.String _pawd;
        /// <summary>
        /// 原密码
        /// </summary>
        public System.String pwd { get { return this._pawd; } set { this._pawd = value?.Trim(); } }

        private System.String _password;
        /// <summary>
        /// 现密码
        /// </summary>
        public System.String passwd { get { return this._password; } set { this._password = value?.Trim(); } }

        private System.String _password2;
        /// <summary>
        /// 现密码2
        /// </summary>
        public System.String passwd2 { get { return this._password2; } set { this._password2 = value?.Trim(); } }

        private System.String _code;
        /// <summary>
        /// 验证码
        /// </summary>
        public System.String code { get { return this._code; } set { this._code = value?.Trim(); } }

        private System.String _phone_no;
        /// <summary>
        /// 手机号
        /// </summary>
        public System.String phone_no { get { return this._phone_no; } set { this._phone_no = value?.Trim(); } }

        /// <summary>
        /// 集团ID
        /// </summary>
        public int org_id { get; set; }
    }
}
