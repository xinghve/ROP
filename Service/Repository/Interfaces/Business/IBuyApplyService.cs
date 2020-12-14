using Models.DB;
using Models.View.Business;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace Service.Repository.Interfaces.Business
{
    /// <summary>
    /// 采购申请单
    /// </summary>
    public interface IBuyApplyService
    {
        /// <summary>
        /// 申请采购单分页
        /// </summary>
        /// <returns></returns>
        Task<Page<bus_buy_apply>> GetPageAsync(BuyApplySearch entity);

        /// <summary>
        /// 获取明细及审核信息
        /// </summary>
        /// <param name="apply_no">采购申请单号</param>
        /// <returns></returns>
        Task<BuyApplyDetials> GetDetialsAndVerifyAsync(string apply_no);

        /// <summary>
        /// 申请采购单明细分页
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<Page<bus_buy_apply_detials>> GetPageDetailAsync(BuyApplyDetailSearch entity);

        /// <summary>
        /// 新增采购单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> AddBuyApply(BuyApplyAddModel entity);

        /// <summary>
        /// 编辑采购单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> ModifyApply(BuyApplyModifyModel entity);

        /// <summary>
        /// 提交采购单
        /// </summary>
        /// <returns></returns>
        Task<bool> CommitApply(CommitModel entity);
        /// <summary>
        /// 作废采购单据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> DeleteApply(CommitModel entity);

        /// <summary>
        /// 获得分页列表（已审核）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<Page<ApplyBill>> GetApplyPageAsync(ApplyBillPageSearch entity);

        /// <summary>
        /// 立即提交采购申请
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> ImmediatelyCommit(BuyApplyAddModel entity);

        /// <summary>
        /// 根据申请单号集合获取对应明细
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        Task<List<bus_buy_apply_detials>> GetDetialsAsync(List<string> list);

        /// <summary>
        /// 删除草稿
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> DeleteDrafyAsync(CommitModel entity);

        /// <summary>
        /// 导出
        /// </summary>
        /// <returns></returns>
        Task<string> ExportAsync(string No);
    }
}
