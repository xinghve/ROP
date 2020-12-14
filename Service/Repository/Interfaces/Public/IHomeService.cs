using Models.DB;
using Models.View.Crm;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Repository.Interfaces.Public
{
    /// <summary>
    /// 首页
    /// </summary>
    public interface IHomeService
    {
        /// <summary>
        /// 获取节日（包括生日档案）
        /// </summary>
        /// <returns></returns>
        Task<object> GetFestival();

        /// <summary>
        /// 获取随访计划
        /// </summary>
        /// <returns></returns>
        Task<object> GetFollow();

        /// <summary>
        /// 获取客户余额下限
        /// </summary>
        /// <returns></returns>
        Task<object> GetLowerBalance();

        /// <summary>
        /// 修改生日消息已读
        /// </summary>
        /// <returns></returns>
        Task UpdateBirthday(int id);

        /// <summary>
        /// 修改随访消息已读
        /// </summary>
        /// <returns></returns>
        Task UpdateFollowUp(int id);


        /// <summary>
        /// 修改余额下限消息已读
        /// </summary>
        /// <returns></returns>
        Task UpdateBalanceLower(int id);
    }
}
