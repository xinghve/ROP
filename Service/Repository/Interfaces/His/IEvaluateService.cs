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
    /// 就诊评价
    /// </summary>
    public interface IEvaluateService
    {
        /// <summary>
        /// 获得列表（客户端）
        /// </summary>
        /// <param name="state">状态（-1=所有，1=已评价，0=待评价）</param>
        /// <returns></returns>
        Task<dynamic> GetListAsync(short state);

        /// <summary>
        /// 评价（客户端）
        /// </summary>
        /// <returns></returns>
        Task<int> EvaluateAsync(r_evaluate entity);

        /// <summary>
        /// 获取评价信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<r_evaluate> GetByIdAsync(string id);

        /// <summary>
        /// 评价分页
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<Page<EvaluatePageModel>> GetPageAsync(EvaluateSearch entity);
    }
}
