using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Business
{
    /// <summary>
    /// 采购申请明细（含审核信息）
    /// </summary>
    public class BuyApplyDetials
    {
        /// <summary>
        /// 明细
        /// </summary>
        public List<buy_Apply_Detials_assets> buy_Apply_Detials_assets { get; set; }

        /// <summary>
        /// 明细
        /// </summary>
        public List<bus_buy_apply_detials> buy_Apply_Detials { get; set; }

        /// <summary>
        /// 审核信息
        /// </summary>
        public List<verifyInfo> verifyInfos { get; set; }
    }

    /// <summary>
    /// 固定资产
    /// </summary>
    public class buy_Apply_Detials_assets : bus_buy_apply_detials
    {
        /// <summary>
        /// 固定资产
        /// </summary>
        public List<bus_assets> bus_Assets { get; set; }
    }

    /// <summary>
    /// 审核信息
    /// </summary>
    public class verifyInfo
    {
        /// <summary>
        /// 审核时间
        /// </summary>
        public string verify_time { get; set; }

        /// <summary>
        /// 审核说明
        /// </summary>
        public string verify_remark { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        public string verifier { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public short state { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        public string dept { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public string role { get; set; }
        /// <summary>
        /// 是否机构
        /// </summary>
        public bool is_org { get; set; }
    }
}
