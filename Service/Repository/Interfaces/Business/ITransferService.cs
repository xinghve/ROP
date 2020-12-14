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
    /// 调拨
    /// </summary>
    public interface ITransferService
    {
        /// <summary>
        /// 添加调拨
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> AddAsync(Transfer entity);

        /// <summary>
        /// 添加调拨（草稿）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> AddDraftAsync(Transfer entity);

        /// <summary>
        /// 编辑调拨单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> ModifyAsync(Transfer entity);

        /// <summary>
        /// 编辑调拨单（草稿）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> ModifyDraftAsync(Transfer entity);

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> CommitAsync(Transfer entity);

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> CancelAsync(bus_transfer_bill entity);

        /// <summary>
        /// 根据调拨单号获取调拨信息
        /// </summary>
        /// <param name="bill_no">调拨单号</param>
        /// <returns></returns>
        Task<Transfer> GetTransferAsync(string bill_no);

        /// <summary>
        /// 获得分页列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<Page<Transfer>> GetPageAsync(TransferPageSearch entity);

        /// <summary>
        /// 确认调入
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> VerifyInAsync(bus_transfer_bill entity);

        /// <summary>
        /// 确认调出
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> VerifyOutAsync(Transfer entity);

        /// <summary>
        /// 删除草稿
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync(bus_transfer_bill entity);

        /// <summary>
        /// 导出
        /// </summary>
        /// <returns></returns>
        Task<string> ExportAsync(string No);
    }
}
