using Models.DB;
using Models.View.Public;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Business
{
    /// <summary>
    /// 流程实体
    /// </summary>
    public class ProcessModel
    {
        /// <summary>
        /// 流程数据
        /// </summary>
        public p_process process { get; set; }
        /// <summary>
        /// 流程详情
        /// </summary>
        public List<p_process_detials> Details { get; set; }
    }

    /// <summary>
    /// 请假流程
    /// </summary>
    public class LeavelProcessModel
    {
        /// <summary>
        /// 请假类型id
        /// </summary>
        public int leave_type_id { get; set; }
        /// <summary>
        /// 请假类型
        /// </summary>
        public string leave_type { get; set; }

      /// <summary>
      /// 部门
      /// </summary>
        public List<DeptProcessModel> deptModel { get; set; }
    }

    /// <summary>
    /// 部门
    /// </summary>
    public class DeptProcessModel
    {
        /// <summary>
        /// 部门名称
        /// </summary>
        public string dept_name { get; set; }
        /// <summary>
        /// 部门id
        /// </summary>
        public int dept_id { get; set; }

        /// <summary>
        /// 请假类型id
        /// </summary>
        public int leave_type_id { get; set; }
    }

 

    /// <summary>
    /// 流程查询条件
    /// </summary>
    public class ProcessSearchModel
    {
        /// <summary>
        /// 流程类型
        /// </summary>
        public int type_id { get; set; }

        /// <summary>
        /// 门店id
        /// </summary>
        public int store_id { get; set; }
        /// <summary>
        /// 流程名
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 是否机构流程
        /// </summary>
        public bool is_org { get; set; }
    }

    /// <summary>
    /// 请假流程搜素实体
    /// </summary>
    public class ProcessLeaveSearchModel
    {
        /// <summary>
        /// 是否机构
        /// </summary>
        public bool is_org { get; set; }
        /// <summary>
        /// 流程类型
        /// </summary>
        public int leave_type_id { get; set; }

        /// <summary>
        /// 部门id
        /// </summary>
        public int dept_id { get; set; }

        /// <summary>
        /// 门店id
        /// </summary>
        public int store_id { get; set; }
        /// <summary>
        /// 流程名
        /// </summary>
        public string name { get; set; }


    }

    /// <summary>
    /// 流程新增
    /// </summary>
    public class AddProcessModel
    {
        /// <summary>
        /// 是否机构
        /// </summary>
        public bool is_org { get; set; }

        /// <summary>
        /// 流程名
        /// </summary>
        public string name { get; set; }

       
        private System.Int32 _store_id = 0;
        /// <summary>
        /// 门店id
        /// </summary>
        public System.Int32 store_id { get { return this._store_id; } set { this._store_id = value; } }
     

        private System.Int32 _dept_id = 0;
        /// <summary>
        /// 部门id
        /// </summary>
        public System.Int32 dept_id { get { return this._dept_id; } set { this._dept_id = value; } }

       
        private System.String _dept_name=" ";
        /// <summary>
        /// 部门
        /// </summary>
        /// 
        public System.String dept_name { get { return this._dept_name; } set { this._dept_name = value?.Trim(); } }

        /// <summary>
        /// 状态
        /// </summary>
        public short state { get; set; }

        /// <summary>
        /// 类型id
        /// </summary>
        public short type_id { get; set; }

        /// <summary>
        /// 类型名
        /// </summary>
        public string type_name { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string remark { get; set; }
        /// <summary>
        /// (请假、领用)类型List 
        /// </summary>
        public List<LeaveTypeList> leavel_type_List { get; set; }

        /// <summary>
        /// 请假时长
        /// </summary>
        public decimal duration { get; set; }

        /// <summary>
        /// 金额限制
        /// </summary>
        public decimal use_money { get; set; }
        /// <summary>
        /// 流程明细
        /// </summary>
        public List<p_process_detials> detailsList { get; set; }
    }

    /// <summary>
    /// 流程类型(领用，请假)
    /// </summary>
    public class LeaveTypeList
    {
       /// <summary>
       /// 类型id
       /// </summary>
        public short leave_type_id { get; set; }
        /// <summary>
        /// （请假、请假）类型
        /// </summary>
        public string leave_type { get; set; }
    }


    /// <summary>
    /// 流程编辑
    /// </summary>
    public class ModifyProcessModel
    {
        /// <summary>
        /// 是否机构
        /// </summary>
        public bool is_org { get; set; }

        /// <summary>
        /// 流程id
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 流程名
        /// </summary>
        public string name { get; set; }


        private System.Int32 _dept_id = 0;
        /// <summary>
        /// 部门id
        /// </summary>
        public System.Int32 dept_id { get { return this._dept_id; } set { this._dept_id = value; } }


        private System.String _dept_name = " ";
        /// <summary>
        /// 部门
        /// </summary>
        /// 
        public System.String dept_name { get { return this._dept_name; } set { this._dept_name = value?.Trim(); } }

        /// <summary>
        /// 状态
        /// </summary>
        public short state { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string remark { get; set; }

        /// <summary>
        /// 请假时长
        /// </summary>
        public decimal duration { get; set; }

        /// <summary>
        /// 金额限制
        /// </summary>
        public decimal use_money { get; set; }
        /// <summary>
        /// 流程明细
        /// </summary>
        public List<p_process_detials> detailsList { get; set; }
    }

    /// <summary>
    /// 删除流程
    /// </summary>
    public class DeleteProcessModel
    {
        /// <summary>
        /// 流程id
        /// </summary>
        public int id { get; set; }
    }

    /// <summary>
    /// 流程启用禁用
    /// </summary>
    public class EnableProcessModel
    {
        /// <summary>
        /// 流程id
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public short state { get; set; }
        /// <summary>
        /// 是否机构
        /// </summary>
        public bool is_org { get; set; }
    }
}
