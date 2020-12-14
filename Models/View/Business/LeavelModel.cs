using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Business
{
    class LeavelModel
    {
    }

    /// <summary>
    /// 新增请假实体
    /// </summary>
    public class AddLeavelModel
    {
        /// <summary>
        /// 门店id
        /// </summary>
        public int store_id { get; set; }

        /// <summary>
        /// 门店名
        /// </summary>
        public string store_name { get; set; }

        /// <summary>
        /// 请假开始时间
        /// </summary>
        public DateTime apply_start_time { get; set; }

        /// <summary>
        /// 请假结束时间
        /// </summary>
        public DateTime apply_end_time { get; set; }

        /// <summary>
        /// 请假事由
        /// </summary>
        public string leave_cause { get; set; }

        /// <summary>
        /// 时长
        /// </summary>
        public decimal duration { get; set; }

        /// <summary>
        /// 请假类型id
        /// </summary>
        public short leave_type_id { get; set; }
        /// <summary>
        /// 请假类型
        /// </summary>
        public string leave_type { get; set; }

        /// <summary>
        /// 请假流程id集合
        /// </summary>
        public List<int> leave_process_list { get; set; }

        /// <summary>
        /// 图片路径列表
        /// </summary>
        public List<string> leaveImg { get; set; }
    }

    /// <summary>
    ///请假图片
    /// </summary>
    public class LeaveImg
    {
        /// <summary>
        /// 图片路径列表
        /// </summary>
        public List<string> vs { get; set; }
    }

    /// <summary>
    /// 获取审核流程条件
    /// </summary>
    public class VerifyProcessSearch
    {
        /// <summary>
        /// 请假类型
        /// </summary>
        public int leave_type_id { get; set; }

        /// <summary>
        /// 时长
        /// </summary>
        public decimal duration { get; set; }
        /// <summary>
        /// 门店
        /// </summary>
        public int store_id { get; set; }

        /// <summary>
        /// 1:登录人 2：个人中心
        /// </summary>
        public short whichuser { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public int user_id { get; set; }
    }

    /// <summary>
    /// 审核流程
    /// </summary>
    public class VerifyProcess
    {
        /// <summary>
        /// 是否为机构
        /// </summary>
        public bool is_org { get; set; }
        /// <summary>
        /// 流程id
        /// </summary>
        public int process_id { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string dept_name { get; set; }
        /// <summary>
        /// 审核流程详情
        /// </summary>
        public List<VerifyProcessDetail> VerifyProcessDetail { get; set; }
       
    }
    /// <summary>
    /// 审核流程详情
    /// </summary>
    public class VerifyProcessDetail
    {
        /// <summary>
        /// 审核人员
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 层级
        /// </summary>
        public int level { get; set; }

        /// <summary>
        /// 职位
        /// </summary>
        public string role_name { get; set; }

        /// <summary>
        /// 审核状态
        /// </summary>
        public int state { get; set; }
        /// <summary>
        /// 审核原因
        /// </summary>
        public string reason { get; set; }

        /// <summary>
        /// 流程id
        /// </summary>
        public int no { get; set; }

        /// <summary>
        /// 是否机构
        /// </summary>
        public bool is_org { get; set; }
    }

    /// <summary>
    /// 请假记录
    /// </summary>
    public class LeaveRecord:oa_leave
    {
      
        /// <summary>
        /// 电话
        /// </summary>
        public string phone { get; set; }
        /// <summary>
        /// 请假图片
        /// </summary>
        public List<oa_leave_image> leaveImg { get; set; }

        /// <summary>
        /// 流程
        /// </summary>
        public List<VerifyProcess> verifyProcess { get; set; }


    }


    /// <summary>
    /// 请假记录搜索
    /// </summary>
    public class LeaveRecordSearch
    {
        /// <summary>
        /// 门店id
        /// </summary>
        public int store_id { get; set; }

        /// <summary>
        /// 选择门店id
        /// </summary>
        public int select_store_id { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int state { get; set; }

        /// <summary>
        /// 请假类型
        /// </summary>
        public int leave_type_id { get; set; }

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


        private System.String _order = "leave_no";
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
}
