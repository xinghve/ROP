using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Business
{
    /// <summary>
    /// 审核实体
    /// </summary>
    public class ApprovalModel
    {
        /// <summary>
        /// 发起人
        /// </summary>
        public string apply_name { get; set; }

        /// <summary>
        /// 审核类型
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 审核类型id
        /// </summary>
        public int type_id { get; set; }

        /// <summary>
        /// 待审核人
        /// </summary>
        public int await_verifier_id { get; set; }

        /// <summary>
        /// 申请单号
        /// </summary>
        public string apply_no { get; set; }

        /// <summary>
        /// 发起人id
        /// </summary>
        public int apply_id { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string phone { get; set; }

        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime apply_time { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public short state { get; set; }

        /// <summary>
        /// 待审核角色id
        /// </summary>
        public int role_id { get; set; }
        /// <summary>
        /// 请假类型id
        /// </summary>
        public int leave_type_id { get; set; }

        /// <summary>
        /// 天数
        /// </summary>
        public decimal duration { get; set; }

        /// <summary>
        /// 门店名
        /// </summary>
        public string store { get; set; }
        /// <summary>
        /// 流程名
        /// </summary>
        public string process_name { get; set; }
    }

    /// <summary>
    /// 查询条件
    /// </summary>
    public class ApprovalSearchModel
    {
        /// <summary>
        /// 门店id
        /// </summary>
        public int store_id { get; set; }

        /// <summary>
        /// 选择状态
        /// </summary>
        public int state { get; set; }

        /// <summary>
        /// 自己的状态
        /// </summary>
        public int own_state { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public int type_id { get; set; }

        /// <summary>
        /// 请假单号\发起人
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 申请区间
        /// </summary>
        public string apply_start_time { get; set; }
        /// <summary>
        /// 申请区间
        /// </summary>
        public string apply_end_time { get; set; }


        private System.String _order = "apply_no";
        /// <summary>
        /// 排序字段
        /// </summary>
        public System.String order { get { return this._order; } set { this._order = value?.Trim(); } }

        private System.Int32 _orderType = 0;
        /// <summary>
        /// 排序方式
        /// </summary>
        public System.Int32 orderType { get { return this._orderType; } set { this._orderType = value; } }

        private System.Int32 _limit = 10;
        /// <summary>
        /// 单页数据条数
        /// </summary>
        public System.Int32 limit { get { return this._limit; } set { this._limit = value; } }

        private System.Int32 _page = 1;
        /// <summary>
        /// 查询第几页
        /// </summary>
        public System.Int32 page { get { return this._page; } set { this._page = value; } }
    }

    /// <summary>
    /// 审核按钮共用
    /// </summary>
    public class ApprovalLeaveModel
    {
        /// <summary>
        /// 门店id
        /// </summary>
        public int store_id { get; set; }
        /// <summary>
        /// 单号
        /// </summary>
        public string apply_no { get; set; }

        /// <summary>
        /// 审核状态：28不通过34通过
        /// </summary>
        public short approval_state { get; set; }

        /// <summary>
        /// 审核说明
        /// </summary>
        public string verify_remark { get; set; }
    }

    /// <summary>
    /// 当前流程信息
    /// </summary>
    public class process_currentMesModel
    {
        /// <summary>
        /// 部门id
        /// </summary>
        public int dept_id { get; set; }
        /// <summary>
        /// 角色id
        /// </summary>
        public int role_id { get; set; }
        /// <summary>
        /// 人员id
        /// </summary>
        public int employee_id { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string dept_name { get; set; }
        /// <summary>
        /// 角色名
        /// </summary>
        public string role_name { get; set; }
        /// <summary>
        /// 流程id
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 流程名
        /// </summary>
        public string name { get; set; } 
        /// <summary>
        /// 类型id
        /// </summary>
        public short leave_type_id { get; set; } 
        /// <summary>
        /// 请假类型
        /// </summary>
        public string leave_type { get; set; }
    }

    /// <summary>
    /// 共同model
    /// </summary>
    public class CommonModel
    {
        /// <summary>
        /// 总等级
        /// </summary>
        public short total_level { get; set; }
        /// <summary>
        /// 下一级
        /// </summary>
        public int level { get; set; }

        /// <summary>
        /// 门店id
        /// </summary>
        public int store_id { get; set; }
        /// <summary>
        /// 门店名
        /// </summary>
        public string store { get; set; }

        /// <summary>
        /// 申请id
        /// </summary>
        public int applicant_id { get; set; }

        /// <summary>
        /// 申请人
        /// </summary>
        public string applicant { get; set; }

        /// <summary>
        /// 流程类型
        /// </summary>
        public short process_type_id { get; set; }

        /// <summary>
        /// 流程类型
        /// </summary>
        public string process_type { get; set; }

        /// <summary>
        /// 流程id
        /// </summary>
        public int org_process_id { get; set; }
    }
    /// <summary>
    /// 待审核model
    /// </summary>
    public class NewProcessModel
    {
        /// <summary>
        /// 流程详情
        /// </summary>
        public p_process_detials p_Process_Detials { get;set;}

        /// <summary>
        /// 流程等级
        /// </summary>
        public short processleave { get; set; }
    }
}
