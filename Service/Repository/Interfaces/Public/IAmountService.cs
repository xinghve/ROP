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
    /// 提成记录
    /// </summary>
    public interface IAmountService
    {
        /// <summary>
        /// 提成记录分页
        /// </summary>
        /// <returns></returns>
        Task<Page<AmountExpent>> GetPageAsync(AmountSearch entity);

        /// <summary>
        /// 提成记录分页(分销人员端)
        /// </summary>
        /// <returns></returns>
        Task<dynamic> GetPageByDistributorAsync(Search entity);

        /// <summary>
        /// 提成比例
        /// </summary>
        /// <returns></returns>
        Task<int> SetRate(AmountRate entity);

        /// <summary>
        /// 获取分销人员提成比例
        /// </summary>
        /// <returns></returns>
        Task<object> GetRate();
    }
}
