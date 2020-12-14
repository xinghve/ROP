using Microsoft.AspNetCore.Http;
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
    /// 活动接口
    /// </summary>
    public interface IActivityService
    {
        /// <summary>
        /// 活动分页数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<Page<ActivityPageModel>> GetPageAsync(ActivitySearchModel model);

        /// <summary>
        /// 添加活动、优惠券
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> Add(ActivityCouponModel entity);

        /// <summary>
        /// 编辑活动、优惠券
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> Modify(ActivityCouponModel entity);

        /// <summary>
        /// 启用、禁用活动
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> ModifyEnable(c_activity entity);

        /// <summary>
        /// 删除活动
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> Delete(int id);

        /// <summary>
        /// 领取优惠券
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> CouponCollection(ArchivesActivityCouponModel entity);

        /// <summary>
        /// 根据优惠券返回用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<List<int>> ReturnArchives(ReturnArchivesModel model);

        /// <summary>
        /// 设置图片
        /// </summary>
        /// <param name="image"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<bool> SetAsync(ActivityIMGModel image, dynamic type);

        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="activity_id">活动id</param>
        /// <returns></returns>
        Task<List<r_activity_img>> GetAsync(int activity_id);

        /// <summary>
        /// 获取图片（客户端）
        /// </summary>
        /// <returns></returns>
        Task<dynamic> GetByNowAsync();

        /// <summary>
        /// 获取活动详情（客户端）
        /// </summary>
        /// <param name="activity_id">活动id</param>
        /// <returns></returns>
        Task<dynamic> GetDetialsAsync(int activity_id);

        /// <summary>
        /// 优惠券领取记录
        /// </summary>
        /// <returns></returns>
        Task<Page<ReceiveCouponModel>> GetRecivePage(ReceiveCouponSearch entity);
    }
}
