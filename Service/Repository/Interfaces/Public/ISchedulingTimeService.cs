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
    /// 排班时间段
    /// </summary>
    public interface ISchedulingTimeService
    {
        /// <summary>
        /// 设置排班时间段
        /// </summary>
        /// <returns></returns>
        bool SetAsync(SchedulingTimes entity);

        /// <summary>
        /// 获取排班时间段
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <returns></returns>
        Task<List<Time>> GetListAsync(int store_id);
    }
}
