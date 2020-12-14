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
    /// 医疗室排班
    /// </summary>
    public interface IRoomSchedulingService
    {
        /// <summary>
        /// 开始
        /// </summary>
        /// <returns></returns>
        Task<bool> StartAsync(Execute entity);

        /// <summary>
        /// 通知
        /// </summary>
        /// <returns></returns>
        Task<bool> NoticeAsync(Execute entity);

        /// <summary>
        /// 结束
        /// </summary>
        /// <returns></returns>
        Task<bool> EndAsync(Execute entity);

        /// <summary>
        /// 获得分页列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<dynamic> GetPagesAsync(RoomSchedulingSearch entity);
    }
}
