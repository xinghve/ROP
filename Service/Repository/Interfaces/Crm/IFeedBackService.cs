using Models.DB;
using Models.View.Crm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace Service.Repository.Interfaces.Crm
{
    /// <summary>
    /// 反馈接口
    /// </summary>
    public interface IFeedBackService
    {
        /// <summary>
        /// 反馈分页信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<Page<r_feedback>> GetPageAsync(FeedBackModel entity);
        /// <summary>
        /// 反馈添加信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> Add(r_feedback entity);
        /// <summary>
        /// 编辑反馈信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> Modify(r_feedback entity);
        /// <summary>
        /// 批量删除反馈投诉信息
        /// </summary>
        /// <param name="delList"></param>
        /// <returns></returns>
        Task<int> Delete(List<int> delList);
    }
}
