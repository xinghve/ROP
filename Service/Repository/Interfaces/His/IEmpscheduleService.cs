using Models.View.His;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.Repository.Interfaces.His
{
    /// <summary>
    /// 排班
    /// </summary>
    public interface IEmpscheduleService
    {
        /// <summary>
        /// 设置排班
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<string> Set(Empschedule entity);

        /// <summary>
        /// 获取排班
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="deptid">部门ID</param>
        /// <param name="orderbegin">开始时间</param>
        /// <param name="orderend">结束时间</param>
        /// <returns></returns>
        Task<Empschedule> GetAsync(int store_id, int deptid, DateTime orderbegin, DateTime orderend);

        /// <summary>
        /// 获取存在排班的年份
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="deptid">部门ID</param>
        /// <returns></returns>
        Task<List<int>> GetYearAsync(int store_id, int deptid);

        /// <summary>
        /// 获取排班时间段
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="deptid">部门ID</param>
        /// <param name="year">年份</param>
        /// <returns></returns>
        Task<List<EmpscheduleDate>> GetDateAsync(int store_id, int deptid, int year);
    }
}
