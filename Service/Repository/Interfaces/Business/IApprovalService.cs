using Models.View.Business;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace Service.Repository.Interfaces.Business
{
    /// <summary>
    /// 审批接口
    /// </summary>
    public interface IApprovalService
    {
        /// <summary>
        /// 审核分页
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<Page<ApprovalModel>> GetApprovalPage(ApprovalSearchModel entity);

        /// <summary>
        /// 获取请假详情
        /// </summary>
        /// <param name="apply_no"></param>
        /// <returns></returns>
        Task<LeaveRecord> GetLeaveDetail(string apply_no);

        /// <summary>
        /// 撤销申请
        /// </summary>
        /// <param name="apply_no"></param>
        /// <returns></returns>
        Task<int> RevokeLeave(ApprovalLeaveModel apply_no);

        /// <summary>
        /// 请假审核
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> ApprovaLeave(ApprovalLeaveModel entity);

        /// <summary>
        /// 获取已审核接口
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<Page<ApprovalModel>> GetAudited(ApprovalSearchModel entity);

        /// <summary>
        /// 采购审核
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> ApprovaBuy(ApprovalLeaveModel entity);
        /// <summary>
        /// 调拨审核
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>

        Task<bool> ApprovaTransfer(ApprovalLeaveModel entity);

        /// <summary>
        /// 领用审核
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> ApprovaRequisition(ApprovalLeaveModel entity);
        /// <summary>
        /// 报废审核
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> ApprovaScrap(ApprovalLeaveModel entity);

        /// <summary>
        /// 报损报溢审核
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> ApprovaLossOver(ApprovalLeaveModel entity);
    }
}
