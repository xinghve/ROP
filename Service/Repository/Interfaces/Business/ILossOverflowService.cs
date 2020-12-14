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
    /// 报损报溢
    /// </summary>
    public interface ILossOverflowService
    {
        /// <summary>
        /// 新增报损报溢
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> AddAsync(LossOverflow entity);

        /// <summary>
        /// 撤销报损报溢
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> CancelAsync(bus_loss_overflow entity);

        /// <summary>
        /// 报损报溢分页
        /// </summary>
        /// <returns></returns>
        Task<Page<LossOverflow>> GetPageAsync(LossOverflowPageSearch entity);

        /// <summary>
        /// 获取报损报溢详情
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        Task<LossOverflow> GetLossDetail(string no);

        /// <summary>
        /// 导出表格
        /// </summary>
        /// <param name="list_no"></param>
        /// <returns></returns>
        Task<string> ExportAsync(List<string> list_no);
    }
}
