using Models.DB;
using Models.View.Crm;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tools;

namespace Service.Repository.Interfaces.Crm
{
    /// <summary>
    /// 随访计划
    /// </summary>
    public interface IFollowService
    {
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <returns></returns>
        //Task<int> AddAsync(r_follow_up entity);

        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <returns></returns>
        Task<int> ModifyAsync(r_follow_up entity);

        /// <summary>
        /// 委托
        /// </summary>
        /// <returns></returns>
        Task<int> ClientAsync(r_follow_up entity);

        /// <summary>
        /// 执行
        /// </summary>
        /// <returns></returns>
        Task<int> ExecuteAsync(FollowExecute followExecute);

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <returns></returns>
        //Task<int> DeleteAsync(List<int> vs);

        /// <summary>
        /// 获得列表
        /// </summary>
        /// <returns></returns>
        Task<Page<FollowUpModel>> GetPagesAsync(FollowUpSearch entity);
    }
}
