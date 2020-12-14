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
    /// 投诉
    /// </summary>
    public interface IComplaintService
    {
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <returns></returns>
        Task<int> AddAsync(r_complaint entity);

        /// <summary>
        /// 处理
        /// </summary>
        /// <returns></returns>
        Task<int> DealAsync(r_complaint entity);

        /// <summary>
        /// 获得分页列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<Page<ComplaintPageModel>> GetPagesAsync(ComplaintSearch entity);

        /// <summary>
        /// 数据列表
        /// </summary>
        /// <param name="state">状态</param>
        /// <returns></returns>
        Task<List<r_complaint>> GetListAsync(int state);
    }
}
