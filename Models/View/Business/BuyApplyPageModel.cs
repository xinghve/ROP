using Models.DB;
using Models.View.Public;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Business
{
    /// <summary>
    /// 申请采购查询实体
    /// </summary>
    public class BuyApplySearch : Search
    {
        /// <summary>
        /// 状态
        /// </summary>
        public short state { get; set; }
        /// <summary>
        /// 单据号，门店，申请人
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 登录人门店
        /// </summary>
        public int store_id { get; set; }
        /// <summary>
        /// 下拉选择门店
        /// </summary>
        public int store_select_id { get; set; }

        private bool _pass = false;
        /// <summary>
        /// 是否所有已通过
        /// </summary>
        public bool pass { get => _pass; set => _pass = value; }
    }

    /// <summary>
    /// 采购申请明细查询
    /// </summary>
    public class BuyApplyDetailSearch : Search
    {
        /// <summary>
        /// 采购单号
        /// </summary>
        public string apply_no { get; set; }
        /// <summary>
        /// 供应商名
        /// </summary>
        public string name { get; set; }
    }

    /// <summary>
    /// 申请采购新增实体
    /// </summary>
    public class BuyApplyAddModel
    {
        /// <summary>
        /// 门店id
        /// </summary>
        public int store_id { get; set; }
        /// <summary>
        /// 备注  
        /// </summary>
        public string remark { get; set; }

        /// <summary>
        /// 总价
        /// </summary>
        public decimal total_price { get; set; }

        /// <summary>
        /// 申请单明细
        /// </summary>
        public List<bus_buy_apply_detials> applyDetailList { get; set; }
    }

    /// <summary>
    /// 申请采购编辑实体
    /// </summary>
    public class BuyApplyModifyModel
    {
        /// <summary>
        /// 申请单号  
        /// </summary>
        public string apply_no { get; set; }


        /// <summary>
        /// 备注  
        /// </summary>
        public string remark { get; set; }

        /// <summary>
        /// 申请单明细
        /// </summary>
        public List<bus_buy_apply_detials> applyDetailList { get; set; }
    }

    /// <summary>
    /// 提交申请单
    /// </summary>
    public class CommitModel
    {
        /// <summary>
        /// 申请单号
        /// </summary>
        public string apply_no { get; set; }
    }
}
