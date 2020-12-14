using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Reports
{
    /// <summary>
    /// 主页报表查询条件
    /// </summary>
    public class MainPageSearch
    {
        private System.Int32 _store_id;
        /// <summary>
        /// 门店id
        /// </summary>
        public System.Int32 store_id { get { return this._store_id; } set { this._store_id = value; } }

        private System.Int32 _storeId;
        /// <summary>
        /// 下拉选择门店id
        /// </summary>
        public System.Int32 storeId { get { return this._storeId; } set { this._storeId = value; } }

        private System.Int32 _year;
        /// <summary>
        /// 年
        /// </summary>
        public System.Int32 year { get { return this._year; } set { this._year = value; } }

        private System.Int32 _month;
        /// <summary>
        /// 月
        /// </summary>
        public System.Int32 month { get { return this._month; } set { this._month = value; } }

        private System.Int32 _nature_id;
        /// <summary>
        /// 登录人性质
        /// </summary>
        public System.Int32 nature_id { get { return this._nature_id; } set { this._nature_id = value; } }

        private System.Int32 _role_id;
        /// <summary>
        /// 角色id，门店管理员为0
        /// </summary>
        public System.Int32 role_id { get { return this._role_id; } set { this._role_id = value; } }
    }

    /// <summary>
    /// 医生挂号排名查询
    /// </summary>
    public class DoctorTopSearch
    {
        private System.Int32 _store_id;
        /// <summary>
        /// 门店id
        /// </summary>
        public System.Int32 store_id { get { return this._store_id; } set { this._store_id = value; } }

        private System.Int32 _storeId;
        /// <summary>
        /// 下拉选择门店id
        /// </summary>
        public System.Int32 storeId { get { return this._storeId; } set { this._storeId = value; } }

        private System.Int32 _deptId;
        /// <summary>
        /// 下拉选择科室id
        /// </summary>
        public System.Int32 deptId { get { return this._deptId; } set { this._deptId = value; } }

        private System.Int32 _year;
        /// <summary>
        /// 年
        /// </summary>
        public System.Int32 year { get { return this._year; } set { this._year = value; } }

        private System.Int32 _month;
        /// <summary>
        /// 月
        /// </summary>
        public System.Int32 month { get { return this._month; } set { this._month = value; } }

        private System.Int32 _dept_natureIdid;
        /// <summary>
        /// 部门性质
        /// </summary>
        public System.Int32 dept_natureId { get { return this._dept_natureIdid; } set { this._dept_natureIdid = value; } }

        private System.Int32 _nature_id;
        /// <summary>
        /// 登录人性质
        /// </summary>
        public System.Int32 nature_id { get { return this._nature_id; } set { this._nature_id = value; } }

        private System.Int32 _role_id;
        /// <summary>
        /// 角色id，门店管理员为0
        /// </summary>
        public System.Int32 role_id { get { return this._role_id; } set { this._role_id = value; } }
    }
}
