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
    /// 盘点
    /// </summary>
    public interface IStocktakingService
    {
        /// <summary>
        /// 新增盘点
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> AddAsync(Stocktaking entity);

        /// <summary>
        /// 编辑盘点
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> ModifyAsync(Stocktaking entity);

        /// <summary>
        /// 删除盘点
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> DeleteAsync(bus_stocktaking entity);

        /// <summary>
        /// 开始盘点
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> StartAsync(bus_stocktaking entity);

        /// <summary>
        /// 结束盘点
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> EndAsync(bus_stocktaking entity);

        /// <summary>
        /// 盘点数量录入
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> StocktakingNumAsync(bus_stocktaking_detials entity);

        /// <summary>
        /// 盘点单分页
        /// </summary>
        /// <returns></returns>
        Task<Page<Stocktaking>> GetPageAsync(StocktakingPageSearch entity);

        /// <summary>
        /// 待盘点物资分页
        /// </summary>
        /// <returns></returns>
        Task<Page<WaitStocktaking>> GetWaitStocktakingPageAsync(WaitStocktakingPageSearch entity);

        /// <summary>
        /// 根据单号获取明细
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        Task<List<StocktakingDetials>> GetStocktakingDetialsAsync(string no);

        /// <summary>
        /// 导出表格
        /// </summary>
        /// <param name="list_no"></param>
        /// <returns></returns>
        Task<string> ExportAsync(List<string> list_no);
    }
}
