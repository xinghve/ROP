using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Public
{
    /// <summary>
    /// 分销人员注册
    /// </summary>
    public class DistributorRegister
    {

        private System.Int32 _org_id;
        /// <summary>
        /// 组织机构Id
        /// </summary>
        public System.Int32 org_id { get { return this._org_id; } set { this._org_id = value; } }

        private System.Int32 _store_id;
        /// <summary>
        /// 门店Id
        /// </summary>
        public System.Int32 store_id { get { return this._store_id; } set { this._store_id = value; } }

        private System.String _name;
        /// <summary>
        /// 姓名
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.String _id_no;
        /// <summary>
        /// 身份证号
        /// </summary>
        public System.String id_no { get { return this._id_no; } set { this._id_no = value?.Trim(); } }

        private System.String _phone_no;
        /// <summary>
        /// 手机号，同时用于登录
        /// </summary>
        public System.String phone_no { get { return this._phone_no; } set { this._phone_no = value?.Trim(); } }

        private System.Int32 _director_id;
        /// <summary>
        /// 负责人ID
        /// </summary>
        public System.Int32 director_id { get { return this._director_id; } set { this._director_id = value; } }

        private System.String _code;
        /// <summary>
        /// 验证码
        /// </summary>
        public System.String code { get { return this._code; } set { this._code = value?.Trim(); } }
    }
}
