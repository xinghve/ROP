using Models.View.Reports;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.Repository.Interfaces.Reports
{
    /// <summary>
    /// 医疗板块报表
    /// </summary>
    public interface IMainHealthService
    {
        /// <summary>
        /// 获取医疗基本信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<MainHealthModel> GetHealthAsync(DoctorTopSearch model);

        /// <summary>
        /// 获取医生排班排名
        /// </summary>
        /// <returns></returns>
        Task<doctorList> GetTopList(DoctorTopSearch entity);
    }
}
