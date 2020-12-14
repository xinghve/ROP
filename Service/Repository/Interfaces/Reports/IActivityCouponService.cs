using Models.View.Reports;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace Service.Repository.Interfaces.Reports
{
    /// <summary>
    /// 活动优惠券领取情况
    /// </summary>
    public interface IActivityCouponService
    {
        /// <summary>
        /// 获取活动优惠券领取情况
        /// </summary>
        /// <returns></returns>
        Task<Page<AcCouponModel>> GetACouponPage(AcCouponSearch entity);
    }
}
