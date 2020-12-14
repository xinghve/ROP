using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.View.Business;
using Service.Repository.Interfaces.Business;
using Tools;

namespace ROP.Controllers.Business
{
    /// <summary>
    /// 审批
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ApprovalController : ControllerBase
    {
        private readonly IApprovalService _approvalService;

      
      /// <summary>
      /// 
      /// </summary>
      /// <param name="approvalService"></param>
        public ApprovalController(IApprovalService approvalService) 
        {

            _approvalService = approvalService;
        }

        /// <summary>
        /// 获取登录人审核分页数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetApprovalPage")]
        public async Task<Page<ApprovalModel>> GetApprovalPage([FromQuery]ApprovalSearchModel entity)
        {
            return await _approvalService.GetApprovalPage(entity);
        }

        /// <summary>
        /// 获取请假详情
        /// </summary>
        /// <param name="apply_no"></param>
        /// <returns></returns>
        [HttpGet("GetLeaveDetail")]
        public async Task<LeaveRecord> GetLeaveDetail([FromQuery]string apply_no)
        {
            return await _approvalService.GetLeaveDetail(apply_no);
        }

        /// <summary>
        /// 撤销申请
        /// </summary>
        /// <param name="apply_no"></param>
        /// <returns></returns>
        [HttpPut("RevokeLeave")]
        public async Task<int> RevokeLeave([FromBody]ApprovalLeaveModel apply_no)
        {
            return await _approvalService.RevokeLeave(apply_no);
        }

        /// <summary>
        /// 请假审核
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("ApprovaLeave")]
        public async Task<bool> ApprovaLeave([FromBody]ApprovalLeaveModel entity)
        {
            return await _approvalService.ApprovaLeave(entity);
        }

        /// <summary>
        /// 获取已审核数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetAudited")]
        public async Task<Page<ApprovalModel>> GetAudited([FromQuery]ApprovalSearchModel entity)
        {
            return await _approvalService.GetAudited(entity);
        }

        /// <summary>
        /// 采购审核
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("ApprovaBuy")]
        public async Task<bool> ApprovaBuy([FromBody]ApprovalLeaveModel entity)
        {
            return await _approvalService.ApprovaBuy(entity);
        }

        /// <summary>
        /// 调拨审核
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
         [HttpPost("ApprovaTransfer")]
        public async Task<bool> ApprovaTransfer([FromBody]ApprovalLeaveModel entity)
        {
            return await _approvalService.ApprovaTransfer(entity);
        }

        /// <summary>
        /// 领用审核
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("ApprovaRequisition")]
        public async Task<bool> ApprovaRequisition([FromBody]ApprovalLeaveModel entity)
        {
            return await _approvalService.ApprovaRequisition(entity);
        }

        /// <summary>
        /// 报废审核
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("ApprovaScrap")]
        public async Task<bool> ApprovaScrap([FromBody]ApprovalLeaveModel entity)
        {
            return await _approvalService.ApprovaScrap(entity);
         }


        /// <summary>
        /// 报损报溢审核
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("ApprovaLossOver")]
        public async Task<bool> ApprovaLossOver([FromBody]ApprovalLeaveModel entity)
        {
            return await _approvalService.ApprovaLossOver(entity);
        }
    }
}