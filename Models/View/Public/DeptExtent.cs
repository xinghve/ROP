using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Public
{
    /// <summary>
    /// 部门扩展表
    /// </summary>
    public class DeptExtent
    {
        /// <summary>
        /// 部门表
        /// </summary>
        public p_dept dept {get;set; }

        /// <summary>
        /// 部门性质
        /// </summary>
        public List<p_dept_nature> deptNature { get; set; }
        
    }

    public class DeptModel
    {
        private System.Int32 _id;
        /// <summary>
        /// 自增主键
        /// </summary>
        public System.Int32 id { get { return this._id; } set { this._id = value; } }

        private System.Int32 _org_id;
        /// <summary>
        /// 集团Id
        /// </summary>
        public System.Int32 org_id { get { return this._org_id; } set { this._org_id = value; } }

        private System.Int32 _store_id;
        /// <summary>
        /// 门店Id
        /// </summary>
        public System.Int32 store_id { get { return this._store_id; } set { this._store_id = value; } }

        private System.String _name;
        /// <summary>
        /// 部门名称
        /// </summary>
        public System.String name { get { return this._name; } set { this._name = value?.Trim(); } }

        private System.String _code;
        /// <summary>
        /// 部门编码
        /// </summary>
        public System.String code { get { return this._code; } set { this._code = value?.Trim(); } }

        private System.String _pinyin;
        /// <summary>
        /// 拼音码
        /// </summary>
        public System.String pinyin { get { return this._pinyin; } set { this._pinyin = value?.Trim(); } }

        private System.String _introduce;
        /// <summary>
        /// 简介
        /// </summary>
        public System.String introduce { get { return this._introduce; } set { this._introduce = value?.Trim(); } }

        private System.Int16? _service_object_id;
        /// <summary>
        /// 服务对象id
        /// </summary>
        public System.Int16? service_object_id { get { return this._service_object_id; } set { this._service_object_id = value ?? default(System.Int16); } }

        private System.String _service_object;
        /// <summary>
        /// 服务对象
        /// </summary>
        public System.String service_object { get { return this._service_object; } set { this._service_object = value?.Trim(); } }

        private System.Int16? _sex_restriction_id;
        /// <summary>
        /// 性别限制id
        /// </summary>
        public System.Int16? sex_restriction_id { get { return this._sex_restriction_id; } set { this._sex_restriction_id = value ?? default(System.Int16); } }

        private System.String _sex_restriction;
        /// <summary>
        /// 性别限制
        /// </summary>
        public System.String sex_restriction { get { return this._sex_restriction; } set { this._sex_restriction = value?.Trim(); } }

        private System.String _professional_code;
        /// <summary>
        /// 专业编码
        /// </summary>
        public System.String professional_code { get { return this._professional_code; } set { this._professional_code = value?.Trim(); } }

        private System.String _professional;
        /// <summary>
        /// 专业
        /// </summary>
        public System.String professional { get { return this._professional; } set { this._professional = value?.Trim(); } }

        private System.Int16? _execution_nature_id;
        /// <summary>
        /// 执行性质id
        /// </summary>
        public System.Int16? execution_nature_id { get { return this._execution_nature_id; } set { this._execution_nature_id = value ?? default(System.Int16); } }

        private System.String _execution_nature;
        /// <summary>
        /// 执行性质
        /// </summary>
        public System.String execution_nature { get { return this._execution_nature; } set { this._execution_nature = value?.Trim(); } }

        private System.String _position;
        /// <summary>
        /// 位置
        /// </summary>
        public System.String position { get { return this._position; } set { this._position = value?.Trim(); } }

        private System.String _phone;
        /// <summary>
        /// 电话
        /// </summary>
        public System.String phone { get { return this._phone; } set { this._phone = value?.Trim(); } }

        private System.Int16? _authorized_bed;
        /// <summary>
        /// 编制床位
        /// </summary>
        public System.Int16? authorized_bed { get { return this._authorized_bed; } set { this._authorized_bed = value ?? default(System.Int16); } }

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

        private System.DateTime? _withdrawal_date;
        /// <summary>
        /// 撤档时间
        /// </summary>
        public System.DateTime? withdrawal_date { get { return this._withdrawal_date; } set { this._withdrawal_date = value ?? default(System.DateTime); } }

        private System.String _withdrawal;
        /// <summary>
        /// 撤档原因
        /// </summary>
        public System.String withdrawal { get { return this._withdrawal; } set { this._withdrawal = value?.Trim(); } }

        private System.String _dept_type_code;
        /// <summary>
        /// 部门分类编码
        /// </summary>
        public System.String dept_type_code { get { return this._dept_type_code; } set { this._dept_type_code = value?.Trim(); } }

        private System.String _dept_type_name;
        /// <summary>
        /// 部门分类名称
        /// </summary>
        public System.String dept_type_name { get { return this._dept_type_name; } set { this._dept_type_name = value?.Trim(); } }

        private System.Int16 _state;
        /// <summary>
        /// 状态
        /// </summary>
        public System.Int16 state { get { return this._state; } set { this._state = value; } }

        /// <summary>
        /// 是否私有
        /// </summary>
        public short sign { get; set; }

        /// <summary>
        /// 部门性质
        /// </summary>
        public List<p_dept_nature> deptNature { get; set; }
    }

    /// <summary>
    /// 客户端可挂号科室
    /// </summary>
    public class DeptCusModel 
    {
        /// <summary>
        /// 科室id
        /// </summary>
        public int deptId { get; set; }

        /// <summary>
        /// 科室名
        /// </summary>
        public string text { get; set; }
    }

    public class DeptCusDetail
    {
        /// <summary>
        /// 科室名
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 位置
        /// </summary>
        public string position { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string phone { get; set; }
        /// <summary>
        /// 简介
        /// </summary>
        public string introduce { get; set; }
    } 


}
