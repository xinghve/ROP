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
    /// 请假接口
    /// </summary>
    public interface ILeaveService
    {

        /// <summary>
        /// 获取审核流程
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<List<VerifyProcess>> GetVerifyProcess(VerifyProcessSearch entity);

        /// <summary>
        /// 请假流程
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> AddLeavel(AddLeavelModel entity,dynamic type);


        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="leave_no">请假单号</param>
        /// <returns></returns>
       Task<List<oa_leave_image>> GetAsync(string leave_no);

        /// <summary>
        /// 请假记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<Page<LeaveRecord>> GetLeaveRecord(LeaveRecordSearch entity);

    }
}
