using Models.DB;
using Models.View.Public;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace Service.Repository.Interfaces.Public
{
    /// <summary>
    /// 提现接口
    /// </summary>
    public interface ICashService
    {
        /// <summary>
        /// 提现分页记录
        /// </summary>
        /// <returns></returns>
        Task<Page<CashExtent>> GetPageAsync(CashSearch entity);

        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <returns></returns>
        Task<bool> AddAsync(r_cash_withdrawal entity);

        /// <summary>
        /// 提现分页记录(分销人员端)
        /// </summary>
        /// <returns></returns>
        Task<dynamic> GetPagesByDistributorAsync(Search entity);
        /// <summary>
        /// 提现审核
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> Check(r_cash_withdrawal entity);

        /// <summary>
        /// 转账情况
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> Transfer(r_cash_withdrawal entity);

        /// <summary>
        /// 提现上下限
        /// </summary>
        /// <returns></returns>
        Task<int> ModifyLowerUper(CashLower entity);
        /// <summary>
        /// 获取提现上下限
        /// </summary>
        /// <returns></returns>
        Task<object> GetLowerUper();
    }
}
