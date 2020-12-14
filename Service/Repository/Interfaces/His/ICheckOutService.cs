using Models.DB;
using Models.View.His;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace Service.Repository.Interfaces.His
{
    /// <summary>
    /// 结账接口
    /// </summary>
    public interface ICheckOutService
    {
        /// <summary>
        /// 门诊收费结算单分页数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<Page<FalanceModel>> GetPageAsync(FalanceSearch entity);

       /// <summary>
       /// 结账分页数据
       /// </summary>
       /// <param name="entity"></param>
       /// <returns></returns>
        Task<Page<CheckOutModelPage>> GetCheckOutPage(CheckOutSearch entity);

        /// <summary>
        /// 根据结账id获取结算分页数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<Page<f_balance>> GetBalancePage(FalanceSearch entity);

        /// <summary>
        /// 收费结账
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> AccoutAsync(CheckOutModel entity);
    }
}
