using Models.View.Business;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.Repository.Interfaces.Business
{
    /// <summary>
    /// 流程管理接口
    /// </summary>
    public interface IProcessService
    {
        /// <summary>
        /// 获取流程数据
        /// </summary>
        /// <returns></returns>
        Task<List<ProcessModel>> GetProcessAsync(ProcessSearchModel entity);

        /// <summary>
        /// 新增流程
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> AddProcess(AddProcessModel entity);
       /// <summary>
       /// 编辑流程
       /// </summary>
       /// <param name="entity"></param>
       /// <returns></returns>
        Task<bool> ModifyProcess(ModifyProcessModel entity);

        /// <summary>
        /// 删除流程
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> DeleteProcess(DeleteProcessModel entity);

        /// <summary>
        /// 启用禁用流程
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> ProcessEnable(EnableProcessModel entity);

        /// <summary>
        /// 获取请假类型部门信息
        /// </summary>
        /// <param name="store_id"></param>
        /// <param name="is_org"></param>
        /// <returns></returns>
        Task<List<LeavelProcessModel>> GetLeavelProcessAsync(int store_id,bool is_org);
        /// <summary>
        /// 根据部门获取请假流程
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<List<ProcessModel>> GetLeavelAsync(ProcessLeaveSearchModel entity);
    }
}
