using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.His
{
    /// <summary>
    /// 康复预约分页model
    /// </summary>
    public class RecoverRegisterPage
    {
        /// <summary>
        /// 申请id
        /// </summary>
        public int applyid { get; set; }
        /// <summary>
        /// 用户
        /// </summary>
        public string archives { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public int centerid { get; set; }
        /// <summary>
        /// 用户手机号
        /// </summary>
        public string phone { get; set; }

        /// <summary>
        /// 负责人
        /// </summary>
        public string to_director { get; set; }

        /// <summary>
        /// 就诊id
        /// </summary>
        public int clinicid { get; set; }

        /// <summary>
        /// 单据类型
        /// </summary>
        public string typename { get; set; }

        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime applydate { get; set; }
        /// <summary>
        /// 医生姓名
        /// </summary>
        public string doctorname { get; set; }
        /// <summary>
        /// 科室名称
        /// </summary>
        public string deptname { get; set; }
        /// <summary>
        /// 缴费标志
        /// </summary>
        public int issettle { get; set; }
        /// <summary>
        /// 应收金额
        /// </summary>
        public decimal shouldamount { get; set; }
        /// <summary>
        /// 实收金额
        /// </summary>
        public decimal actualamount { get; set; }

        /// <summary>
        /// 预约项目
        /// </summary>
        public List<RecoverItemList> RecoverItem { get; set; }
    }

    /// <summary>
    /// 康复预约项目
    /// </summary>
    public class RecoverItemList
    {
        /// <summary>
        /// 申请id
        /// </summary>
        public int applyid { get; set; }
        /// <summary>
        /// 规格id 
        /// </summary>
        public int specid { get; set; }
        /// <summary>
        /// 规格名称
        /// </summary>
        public string specname { get; set; }
        /// <summary>
        /// 项目id
        /// </summary>
        public int item_id { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string item_name { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public int orderid { get; set; }
        /// <summary>
        /// 预约数量
        /// </summary>
        public int quantity { get; set; }
        /// <summary>
        /// 已使用数量
        /// </summary>
        public int use_quantity { get; set; }
    }

    /// <summary>
    /// 康复分页查询
    /// </summary>
    public class RecoverRegisterSearch
    {
        /// <summary>
        /// 搜索手机号/用户/负责人
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 门店id
        /// </summary>
        public int store_id { get; set; }

        private System.String _order = "centerid";
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

        private bool _is_me = false;
        /// <summary>
        /// 自己客户
        /// </summary>
        public bool is_me { get { return this._is_me; } set { this._is_me = value; } }
    }

    /// <summary>
    /// 康复是否可预约
    /// </summary>
    public class RecoverIfOrder
    {

        /// <summary>
        /// 项目id
        /// </summary>
        public int item_id { get; set; }
        /// <summary>
        /// 规格id
        /// </summary>
        public int specid { get; set; }

        /// <summary>
        /// 规格名称
        /// </summary>
        public string specname { get; set; }
        /// <summary>
        /// 门店id
        /// </summary>
        public int store_id { get; set; }
        /// <summary>
        /// 预约时间
        /// </summary>
        public DateTime regdate { get; set; }
    }

    /// <summary>
    /// 康复预约
    /// </summary>
    public class RecoreOrderAdd
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public int centerid { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public int orderid { get; set; }
        /// <summary>
        /// 项目id
        /// </summary>
        public int item_id { get; set; }
        /// <summary>
        /// 规格id
        /// </summary>
        public int specid { get; set; }
        /// <summary>
        /// 规格名称
        /// </summary>
        public string specname { get; set; }
        /// <summary>
        /// 门店id
        /// </summary>
        public int store_id { get; set; }
        /// <summary>
        /// 预约时间
        /// </summary>
        public DateTime regdate { get; set; }
        /// <summary>
        /// 申请id
        /// </summary>
        public int applyid { get; set; }
    }
    /// <summary>
    /// 是否可预约
    /// </summary>
    public class RecordIfUse
    {
        /// <summary>
        /// 是否可预约
        /// </summary>
        public bool state { get; set; }
        /// <summary>
        /// 规格名
        /// </summary>
        public string specname { get; set; }
    }

    /// <summary>
    /// 康复预约记录
    /// </summary>
    public class RecoverRecordPage : his_recover
    {
        /// <summary>
        /// 用户
        /// </summary>
        public string archives { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string phone { get; set; }

        /// <summary>
        /// 负责人
        /// </summary>
        public string to_director { get; set; }
        /// <summary>
        /// 规格名
        /// </summary>
        public string specname { get; set; }

        /// <summary>
        /// 项目名
        /// </summary>
        public string item_name { get; set; }

    }

    /// <summary>
    /// 康复记录搜索
    /// </summary>
    public class RecoverRecordSearch : RecoverRegisterSearch
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public string startTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public string endTime { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int stateid { get; set; }
    }

    /// <summary>
    /// 改期
    /// </summary>
    public class ModifyModel
    {
        /// <summary>
        /// 康复记录id
        /// </summary>
        public int recoverid { get; set; }

        /// <summary>
        /// 改期时间
        /// </summary>
        public DateTime recoverTime { get; set; }
    }

    /// <summary>
    /// 取消预约
    /// </summary>
    public class CancelModel
    {
        /// <summary>
        /// 康复记录id
        /// </summary>
        public int recoverid { get; set; }
    }

    /// <summary>
    /// 康复记录（客户端）
    /// </summary>
    public class RecoverRecordPC
    {
        /// <summary>
        /// 门店名
        /// </summary>
        public string strore_name { get; set; }
        /// <summary>
        /// 项目名
        /// </summary>
        public string item_name { get; set; }
        /// <summary>
        /// 预约时间
        /// </summary>
        public DateTime regdate { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string state_name { get; set; }
        /// <summary>
        /// 操作人
        /// </summary>
        public string operator_name { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime recorddate { get; set; }
        /// <summary>
        /// 康复id
        /// </summary>
        public int recoverid { get; set; }
    }

    /// <summary>
    /// 康复（客户端）
    /// </summary>
    public class RecoverPCSearch
    {

        private System.String _order = "recoverid";
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
    /// 获取康复记录列表（客户端）
    /// </summary>
    public class RecoverList
    {
        /// <summary>
        /// 预约日期（开方时间）
        /// </summary>
        public DateTime regdate { get; set; }
        /// <summary>
        /// 科室
        /// </summary>
        public string dept { get; set; }
        /// <summary>
        /// 医生
        /// </summary>
        public string doctor { get; set; }
        /// <summary>
        /// 申请id
        /// </summary>
        public int applyid { get; set; }
    }

    /// <summary>
    /// 康复详情（客户端）
    /// </summary>
    public class RecoverDetail : RecoverList
    {

        /// <summary>
        /// 负责人
        /// </summary>
        public string to_director { get; set; }
        /// <summary>
        /// 负责人电话
        /// </summary>
        public string phone { get; set; }
        /// <summary>
        /// 项目list
        /// </summary>
        public List<RecoverDetailList> recoverList { get; set; }

        /// <summary>
        /// 最后一次完成时间
        /// </summary>
        public DateTime last_time { get; set; }

    }

    /// <summary>
    /// 开单康复List(客户端)
    /// </summary>
    public class RecoverDetailList
    {
        /// <summary>
        /// 项目名
        /// </summary>
        public string item_name { get; set; }
        /// <summary>
        /// 总次数
        /// </summary>
        public int total_count { get; set; }
        /// <summary>
        /// 已使用次数
        /// </summary>
        public int use_count { get; set; }

    }
}
