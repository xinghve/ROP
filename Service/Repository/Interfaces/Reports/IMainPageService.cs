using Models.View.Reports;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.Repository.Interfaces.Reports
{
    /// <summary>
    /// 主页报表
    /// </summary>
    public interface IMainPageService
    {
        /// <summary>
        /// 主页报表
        /// </summary>
        /// <returns></returns>
        Task<MainPageModel> GetMainPage(MainPageSearch entity);

        /// <summary>
        /// 业绩
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<rechargeList> GetRecharge(MainPageSearch entity);

        /// <summary>
        /// 整体情况
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<totalList> GetTotal(MainPageSearch entity);

        
    }
}
