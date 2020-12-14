using Models.DB;
using Models.View.Reports;
using Service.Extensions;
using Service.Repository.Interfaces.Reports;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace Service.Repository.Implements.Repotrs
{
    /// <summary>
    /// 活动优惠券领取情况
    /// </summary>
    public class ActivityCouponService:DbContext,IActivityCouponService
    {
        /// <summary>
        /// 获取优惠券领取情况
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Page<AcCouponModel>> GetACouponPage(AcCouponSearch entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
           
            //活动详情
            var acList= await Db.Queryable<c_activity, c_archives_activity_coupon>((a, aac) => new object[] { JoinType.Left, aac.activity_id == a.id })
                                        .Where((a, aac) => a.org_id == userInfo.org_id)
                                        .WhereIF(!string.IsNullOrEmpty(entity.activity_name),(a,aac)=>a.name.Contains(entity.activity_name))
                                        .WhereIF(entity.startTime!=null, (a, aac)=>a.start_date >=entity.startTime )
                                        .WhereIF(entity.endTime!=null, (a, aac)=>a.end_date<= entity.endTime)
                                        .GroupBy((a, aac) => new { id = a.id })
                                        .Select((a, aac) => new AcCouponModel { activity_id = a.id, activity_name = a.name, start_time=a.start_date,end_time=a.end_date })
                                        .WithCache()
                                        .ToPageAsync(entity.page, entity.limit);

            //查询领用的情况
            var aacList = await Db.Queryable<c_archives_activity_coupon>()
                              .Where(w => w.org_id == userInfo.org_id)
                              .WithCache()
                              .ToListAsync();

            //此活动引入的新客户数
            var archivesCount = await Db.Queryable<c_archives, c_new_coupon_detials>((a, d) => new object[] { JoinType.Left, a.coupon_no == d.no })
                                        .Where((a, d) => a.coupon_no != null && d.org_id == userInfo.org_id)
                                        .Select((a, d)=>new { a.id,d.no,d.activity_id})
                                        .WithCache()
                                        .ToListAsync();

            var orderTypeCoupon = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeCoupon = " Desc";
            }

            //活动优惠券总数
            var couponTotal = await Db.Queryable<c_new_coupon>()
                                  .Where(w => w.org_id == userInfo.org_id)
                                  .GroupBy(w => new { w.activity_id })
                                  .Select(w => new CouponTot { activity_id = w.activity_id,  sum_coupon = SqlFunc.AggregateSum(w.num), total_money= SqlFunc.AggregateSum(w.num*w.money) })
                                  .WithCache()
                                  .ToListAsync();

            //查询活动下的优惠券信息
            var couponList = await Db.Queryable<c_new_coupon>()
                                   .Where(w => w.org_id == userInfo.org_id)
                                   .GroupBy(w => new { w.id })
                                   .OrderBy(entity.order_coupon + orderTypeCoupon)
                                   .Select(w=>new NewCoupon {id=w.id,  new_sum_coupon=w.num, activity_id=w.activity_id, head=w.head, money=w.money, remarks=w.remarks, coupon_total_money=w.num*w.money})
                                   .WithCache()
                                   .ToListAsync();


            var newList = acList.Items.Select(s => new AcCouponModel { start_time=s.start_time, end_time=s.end_time, activity_id = s.activity_id, activity_name = s.activity_name, coupon_total = couponTotal.Where(ct=>ct.activity_id==s.activity_id).Select(ct=>ct.sum_coupon).FirstOrDefault(), total_money=couponTotal.Where(ct => ct.activity_id == s.activity_id).Select(ct => ct.total_money).FirstOrDefault(),  archives_count=archivesCount.Where(c=>c.activity_id==s.activity_id).GroupBy(c=>c.id).Count(), couponList=couponList.Where(d=>d.activity_id==s.activity_id).Select(d=>new NewCoupon { id=d.id, new_sum_coupon= d.new_sum_coupon, activity_id=d.activity_id, use_total= aacList.Where(aac=>aac.coupon_id==d.id).GroupBy(aac=>aac.archives_id).Count(), remarks=d.remarks, money=d.money, head=d.head, coupon_total_money=d.coupon_total_money, use_total_money=( aacList.Where(aac => aac.coupon_id == d.id).GroupBy(aac => aac.archives_id).Count()*d.money).Value }).ToList(), use_rate = GetRate(s, couponTotal, aacList)}).ToList();

            if (entity.order.ToLower()== "archives_count")
            {
                //var list = from nnn in newList orderby "archives_count" descending select nnn;
                var list = newList.OrderByDescending(p => p.archives_count);
                acList.Items = list.ToList();
            }
            else if (entity.order.ToLower() == "coupon_total")
            {
                var list = newList.OrderByDescending(p => p.coupon_total);
                acList.Items = list.ToList();
            }
            else if (entity.order.ToLower() == "use_rate")
            {
                var list = newList.OrderByDescending(p => p.use_rate);
                acList.Items = list.ToList();
            }
            else if (entity.order.ToLower() == "start_time")
            {
                var list = newList.OrderByDescending(p => p.start_time);
                acList.Items = list.ToList();
            }
            else if (entity.order.ToLower() == "end_time")
            {
                var list = newList.OrderByDescending(p => p.end_time);
                acList.Items = list.ToList();
            }
            else
            {
                acList.Items = newList.ToList();
            }
            return acList;
        }

        public decimal GetRate(AcCouponModel s, List<CouponTot> coup, List<c_archives_activity_coupon> aacList)
        {

            decimal use = aacList.Where(w => w.activity_id == s.activity_id).GroupBy(w => w.archives_id).Count();
            var suncou = coup.Where(ct => ct.activity_id == s.activity_id).Select(ct => ct.sum_coupon).FirstOrDefault()??0;

            var useRate =suncou == 0 ? 0 : use / suncou;
            return useRate;
        }
    }
}
