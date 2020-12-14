using Models.DB;
using Models.View.Crm;
using Service.Extensions;
using Service.Repository.Interfaces.Crm;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Tools;
using Tools.Filter;

namespace Service.Repository.Implements.Crm
{
    /// <summary>
    /// 优惠券业务处理
    /// </summary>
     public class CouponService:DbContext,ICouponService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<Page<c_coupon>> GetPageAsync(SearchMl model)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var couponList = Db.Queryable<c_coupon>()
                   .Where(w => w.org_id == userInfo.org_id)
                   .WhereIF(!string.IsNullOrEmpty(model.name), w => w.name.Contains(model.name))
                   .WhereIF(model.startTime != null, w => w.create_date >= model.startTime)
                   .WhereIF(model.endTime != null, w => w.create_date <= model.endTime)
                   .WhereIF(model.storeId >= 0, w => w.store_id == model.storeId);
            var orderTypeStr = " Asc";
            if (model.orderType == 1)
            {
                orderTypeStr = " Desc";
            }

            couponList = couponList.OrderBy(model.order + orderTypeStr);
            return await couponList.WithCache().ToPageAsync(model.page, model.limit);

        }

        /// <summary>
        /// 添加优惠券
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> Add(c_coupon entity)
        {
            if (entity.deduction_money >= entity.use_money)
            {
                throw new MessageException("抵扣金额不能大于使用金额！");
            }
            if (entity.deduction_money<0||entity.use_money<0)
            {
                throw new MessageException("金额填写不正确！");
            }
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            //查询是否存在相同优惠券
            var isExtist =await Db.Queryable<c_coupon>().WithCache().AnyAsync(a => a.org_id == userInfo.org_id && a.store_id == entity.store_id&&a.use_money==entity.use_money&&a.name==entity.name&&a.deduction_money==entity.deduction_money);
            if (isExtist)
            {
                throw new MessageException("此类型优惠券已存在");
            }
            var couponEntity = new c_coupon();
            couponEntity = entity;
            couponEntity.org_id = userInfo.org_id;
            couponEntity.creater_id = userInfo.id;
            couponEntity.creater = userInfo.name;
            couponEntity.create_date = DateTime.Now;
            couponEntity.status = 1;

            var isSuccess= await Db.Insertable(couponEntity).RemoveDataCache().ExecuteCommandAsync();
            if (isSuccess <= 0)
            {
                throw new MessageException("添加失败！");
            }
            return isSuccess;
        }

        /// <summary>
        /// 启用、禁用优惠券
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> Modify(c_coupon entity)
        {
           
            //查询优惠券是否在使用
            var is_use = await Db.Queryable<c_activity_coupon>().WithCache().AnyAsync(w => w.coupon_id == entity.id );
            if (is_use)
            {
                throw new MessageException("当前优惠券被使用中，不能操作！");
            }
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            var isSuccess= await Db.Updateable(entity).SetColumns(s=>new c_coupon { status=entity.status }).Where(s=>s.id==entity.id&&s.org_id==userInfo.org_id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
            if (isSuccess <= 0)
            {
                throw new MessageException("操作失败！");
            }
            return isSuccess;
        }

        /// <summary>
        /// 删除优惠券
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> Delete(int id)
        {
            //查询优惠券是否在使用
            var is_use = await Db.Queryable<c_activity_coupon>().WithCache().AnyAsync(w => w.coupon_id == id);
            if (is_use)
            {
                throw new MessageException("当前优惠券被使用中，不能删除！");
            }
            var isSuccess = await Db.Deleteable<c_coupon>().Where(w => w.id == id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
            if (isSuccess<=0)
            {
                throw new MessageException("删除失败！");
            }
            return isSuccess;
        }

        /// <summary>
        /// 获取优惠券列表
        /// </summary>
        /// <param name="store_id"></param>
        /// <returns></returns>
        public async Task<List<c_coupon>> GetList( int store_id)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            return await Db.Queryable<c_coupon>()
                .Where(w => w.org_id == userInfo.org_id && w.store_id == store_id&&w.status==1)
                .WithCache().ToListAsync();
        }
    }
}
