using Models.DB;
using Models.View.Crm;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace Service.Repository.Interfaces.Crm
{
    /// <summary>
    /// 优惠券接口
    /// </summary>
    public interface ICouponService
    {
        /// <summary>
        /// 优惠券分页数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<Page<c_coupon>> GetPageAsync(SearchMl model);

        /// <summary>
        /// 添加优惠券
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> Add(c_coupon entity);
        /// <summary>
        /// 编辑优惠券
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> Modify(c_coupon entity);

        /// <summary>
        /// 删除优惠券
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<int> Delete(int id);

        /// <summary>
        /// 获取优惠券列表
        /// </summary>
        /// <param name="store_id"></param>
        /// <returns></returns>
        Task<List<c_coupon>> GetList( int store_id);
    }
}
