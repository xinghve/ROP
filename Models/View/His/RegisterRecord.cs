using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.His
{
    /// <summary>
    /// 挂号记录
    /// </summary>
    public class RegisterRecord : his_register
    {

        private System.String _name;
        /// <summary>
        /// 姓名
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.String _phone;
        /// <summary>
        /// 手机号
        /// </summary>
        public System.String phone { get { return this._phone; } set { this._phone = value?.Trim(); } }
    }
}
