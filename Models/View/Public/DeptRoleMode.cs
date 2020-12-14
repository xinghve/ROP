using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Public
{
    /// <summary>
    /// 人员部门角色
    /// </summary>
    public class DeptRoleMode
    {
        private System.String _name;
        /// <summary>
        /// 部门名
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.String _roleName;
        /// <summary>
        /// 角色名
        /// </summary>
        public System.String roleName { get { return this._roleName; } set { this._roleName = value?.Trim(); } }

        private System.Int32 _id;
        /// <summary>
        /// 部门id
        /// </summary>
        public System.Int32 id { get { return this._id; } set { this._id = value; } }

        private System.Int32 _roleId;
        /// <summary>
        /// 角色id
        /// </summary>
        public System.Int32 roleId { get { return this._roleId; } set { this._roleId = value; } }


    }
}
