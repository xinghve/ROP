using Models.DB;
using Models.View.Public;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Business
{
    /// <summary>
    /// 添加物品领用
    /// </summary>
    public class RequisitionAddModel
    {
        /// <summary>
        /// 领用申请单
        /// </summary>
        public bus_requisitions_bill bill { get; set; }
        /// <summary>
        /// 领用明细
        /// </summary>
        public List<NewDetail> bill_details { get; set; }
       
    }

    /// <summary>
    /// 取消领用
    /// </summary>
    public class CancelRequisitionModel 
    {
        /// <summary>
        /// 领用单号
        /// </summary>
        public string apply_no { get; set; }
        /// <summary>
        /// 门店id
        /// </summary>
        public int store_id { get; set; }
    }

    /// <summary>
    /// 个人领用分页数据
    /// </summary>
    public class RequisitionPageModel: bus_requisitions_bill
    {
        /// <summary>
        /// 领用明细
        /// </summary>
        public List<NewDetail> bill_details { get; set; }

    }

    /// <summary>
    /// 新详情
    /// </summary>
    public class NewDetail : bus_requisitions_detail
    {
        /// <summary>
        /// 可用数量
        /// </summary>
        public int new_num { get; set; }

        /// <summary>
        /// 供应商id
        /// </summary>
        public int manufactor_id { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public string manufactor { get; set; }

        /// <summary>
        /// 固定资产编号
        /// </summary>
        public string no { get; set; }
    }

    /// <summary>
    /// 个人领用记录
    /// </summary>
    public class RequisitionSearch
    {

        private System.String _order = "bill_no";
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
    /// 领用详情
    /// </summary>
    public class RequisitionDetail
    {

        /// <summary>
        /// 分类属性
        /// </summary>
      public string type { get; set; }

        /// <summary>
        /// 是否部门
        /// </summary>
      public int is_dept { get; set; }
      /// <summary>
      /// 部门
      /// </summary>
       public string dept_name { get; set; }
        /// <summary>
        /// 申领详情
        /// </summary>
        public List<bus_requisitions_detail> re_list { get; set; }

        /// <summary>
        /// 流程
        /// </summary>
        public List<VerifyProcess> verifyProcess { get; set; }

    }

    /// <summary>
    /// 领用记录
    /// </summary>
    public class RequisitionRecordSearch: RequisitionSearch
    {
        /// <summary>
        /// 物资分类属性id
        /// </summary>
        public int type_id { get; set; }
        /// <summary>
        /// 门店id
        /// </summary>
        public int store_id { get; set; }
        /// <summary>
        /// 申请人
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 已发放
        /// </summary>
        public int state { get; set; }

        /// <summary>
        /// 个人
        /// </summary>
        public int own { get; set; }

    }

    /// <summary>
    /// 库存所需
    /// </summary>
    public class StorageRequisitionModel 
    { 
        /// <summary>
        /// 明细可用数量
        /// </summary>
        public int use_num_det { get; set; }
        
        /// <summary>
        /// 可用数量
        /// </summary>
        public int use_num { get; set; }

        /// <summary>
        /// 实际数量
        /// </summary>
        public int num { get; set; }

        /// <summary>
        /// 明细实际数量
        /// </summary>
        public int num_det { get; set; }

        /// <summary>
        /// 库存id
        /// </summary>
        public int id { get; set; }

    }

    /// <summary>
    /// 所有发放记录
    /// </summary>
    public class AllGrantModel:bus_grant_detail
    {
        /// <summary>
        /// 申领人
        /// </summary>
        public string re_name { get; set; }

        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime re_time { get; set; }

        /// <summary>
        /// 预计归还时间
        /// </summary>
        public string except_time { get; set; }

        /// <summary>
        /// 详情类型id
        /// </summary>
        public int type_id_det { get; set; }

        /// <summary>
        /// 详情类型
        /// </summary>
        public string type_det { get; set; }

        /// <summary>
        /// 门店名
        /// </summary>
        public string store_name { get; set; }
    }

    /// <summary>
    /// 所有发放记录
    /// </summary>
    public class GrantSearch : RequisitionSearch
    {
        /// <summary>
        /// 分类属性
        /// </summary>
        public int type_id { get; set; }
        /// <summary>
        /// 门店id
        /// </summary>
        public int store_id { get; set; }

        /// <summary>
        /// 门店下拉
        /// </summary>
        public int store_select_id { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int state { get; set; }

        /// <summary>
        /// 申领人\发放人
        /// </summary>
        public string name { get; set; }

        private System.DateTime? _start_time;
        /// <summary>
        /// 发放开始时间
        /// </summary>
        public System.DateTime? start_time { get { return this._start_time; } set { this._start_time = value; } }

        private System.DateTime? _end_time;
        /// <summary>
        /// 结束时间
        /// </summary>
        public System.DateTime? end_time { get { return this._end_time; } set { this._end_time = value; } }

        /// <summary>
        /// 固资
        /// </summary>
        public int asset { get; set; }
    }

    /// <summary>
    /// 获取对应供应商
    /// </summary>
    public class GetManufactor
    {
        /// <summary>
        /// 供应商id
        /// </summary>
        public int manufactor_id { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public string manufactor { get; set; }

        /// <summary>
        /// 设备编号
        /// </summary>
         public List<string> no { get; set; }
    }

    /// <summary>
    /// 归还实体
    /// </summary>

    public class ReturnModel
    {
        /// <summary>
        /// 发放单号
        /// </summary>
        public string bill_no { get; set; }

        /// <summary>
        /// 物资类型id
        /// </summary>
        public int std_item_id { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        public string spec { get; set; }

        /// <summary>
        /// 供应商id
        /// </summary>
        public int manufactor_id { get; set; }

        /// <summary>
        /// 设备编号
        /// </summary>
        public string no { get; set; }

        /// <summary>
        /// 归还备注
        /// </summary>
        public string remark { get; set; }
    }
}
