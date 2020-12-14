using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Public
{
    /// <summary>
    /// 人员Model
    /// </summary>
    public class PersonModel
    {
        /// <summary>
        /// 人员表（必传）
        /// </summary>
        public p_employee p_employee { get; set; }

        /// <summary>
        /// 人员详情
        /// </summary>
        public p_employee_profile p_detail { get; set; }


        /// <summary>
        /// 部门角色List(添加、编辑),传部门id，角色id（必传）
        /// </summary>
        public List<p_employee_role> pdList { get; set; }

        /// <summary>
        /// 部门角色List(详情展示)
        /// </summary>
        public List<DeptRoleMode> deptList { get; set; }

        private System.Int32 _storeId = 0;
        /// <summary>
        /// 门店id(必传)
        /// </summary>
        public System.Int32 storeId { get { return this._storeId; } set { this._storeId = value; } }

        /// <summary>
        /// 是否修改密码
        /// </summary>
        public bool is_change_password { get; set; }

        /// <summary>
        /// 性质
        /// </summary>
        public List<employee_nature> employeeNatures { get; set; }
    }

    /// <summary>
    /// 性质
    /// </summary>
    public class employee_nature
    {
        /// <summary>
        /// 性质ID
        /// </summary>
        public short nature_id { get; set; }
        /// <summary>
        /// 性质
        /// </summary>
        public string nature { get; set; }
    }

    /// <summary>
    /// 医生model
    /// </summary>
    public class DoctorModel
    {
        /// <summary>
        /// 医生id
        /// </summary>
        public int doctorId { get; set; }

        /// <summary>
        /// 医生名
        /// </summary>
        public string doctorName { get; set; }

        /// <summary>
        /// 专业职务
        /// </summary>
        public string pro_duties { get; set; }

        /// <summary>
        /// 门店ID
        /// </summary>
        public int store_id { get; set; }

        /// <summary>
        /// 门店名称
        /// </summary>
        public string store_name { get; set; }

        /// <summary>
        /// 科室ID
        /// </summary>
        public int dept_id { get; set; }

        /// <summary>
        /// 科室
        /// </summary>
        public string dept_name { get; set; }

        /// <summary>
        /// 擅长
        /// </summary>
        public string good_at { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string image_url { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string sex { get; set; }
    }

    /// <summary>
    /// 医生详情
    /// </summary>
    public class DoctorDetailModel
    {

        /// <summary>
        /// 医生名
        /// </summary>
        public string doctorName { get; set; }

        /// <summary>
        /// 专业职务
        /// </summary>
        public string pro_duties { get; set; }

        /// <summary>
        /// 擅长
        /// </summary>
        public string good_at { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string imgUrl { get; set; }
        /// <summary>
        /// 收藏医生>0是
        /// </summary>
        public int is_collection { get; set; }
    }

    /// <summary>
    /// 流程指定人员
    /// </summary>
    public class PersonProcessModel
    {
        /// <summary>
        /// 人员名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 人员id
        /// </summary>
        public int person_id { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string phone { get; set; }
    }

    /// <summary>
    /// 门店新增人员
    /// </summary>
    public class StoreAddPersonModel
    {
        /// <summary>
        /// 门店id
        /// </summary>
        public int store_id { get; set; }
        /// <summary>
        /// 部门id
        /// </summary>
        public int dept_id { get; set; }
        /// <summary>
        /// 角色id
        /// </summary>
        public int role_id { get; set; }

        /// <summary>
        /// 关联角色id
        /// </summary>
        public int link_role_id { get; set; }
        /// <summary>
        /// 人员id List
        /// </summary>
        public List<int> employeeIds { get; set; }
    }

    /// <summary>
    /// 门店移除角色
    /// </summary>
    public class StoreRemovePersonModel
    {
        /// <summary>
        /// 门店id
        /// </summary>
        public int store_id { get; set; }
        /// <summary>
        /// 人员id
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 角色id
        /// </summary>
        public int role_id { get; set; }
    }

}
