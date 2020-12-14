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
    /// 入库
    /// </summary>
    public interface IPutInStorageService
    {
        /// <summary>
        /// 入库
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> AddAsync(PutInStorage entity);

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> CancelAsync(bus_put_in_storage entity);

        /// <summary>
        /// 根据入库单号获取入库信息
        /// </summary>
        /// <param name="bill_no">入库单号</param>
        /// <returns></returns>
        Task<PutInStorage> GetPutInAsync(string bill_no);

        /// <summary>
        /// 获得分页列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<Page<PutInStorage>> GetPageAsync(PutInStoragePageSearch entity);

        /// <summary>
        /// 导出
        /// </summary>
        /// <returns></returns>
        Task<string> ExportAsync(string No);
    }
}
