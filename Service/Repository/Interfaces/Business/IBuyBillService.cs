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
    /// 采购单
    /// </summary>
    public interface IBuyBillService
    {
        /// <summary>
        /// 根据采购单号获取采购单信息
        /// </summary>
        /// <param name="bill_no">采购单号</param>
        /// <returns></returns>
        Task<BuyBill> GetAsync(string bill_no);

        /// <summary>
        /// 添加采购单
        /// </summary>
        /// <param name="buyBills">采购单实体</param>
        /// <returns></returns>
        Task<bool> AddAsync(List<BuyBill> buyBills);

        /// <summary>
        /// 编辑采购单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> ModifyAsync(bus_buy_bill entity);

        /// <summary>
        /// 获得分页列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<Page<BuyBill>> GetPageAsync(BuyBillPageSearch entity);

        /// <summary>
        /// 获得分页列表（采购单明细）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<Page<bus_buy_bill_detials>> GetDetialsPageAsync(BuyBillDetialsPageSearch entity);

        /// <summary>
        /// 取消
        /// </summary>
        /// <returns></returns>
        Task<bool> CancelAsync(List<string> list);

        /// <summary>
        /// 获取指定采购单号生成采购单时勾选的所有采购申请单生成的所有采购单号
        /// </summary>
        /// <param name="No"></param>
        /// <returns></returns>
        Task<List<string>> GetLinkNoAsync(string No);

        /// <summary>
        /// 完成
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> FinishAsync(bus_buy_bill entity);

        /// <summary>
        /// 导出
        /// </summary>
        /// <returns></returns>
        Task<string> ExportAsync(string No);
    }
}
