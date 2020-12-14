using Microsoft.AspNetCore.Http;
using Models.DB;
using Models.View.Crm;
using Models.View.Public;
using Service.Extensions;
using Service.Repository.Interfaces.Crm;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Tools;
using Tools.Cache;
using Tools.Filter;

namespace Service.Repository.Implements.Crm
{
    /// <summary>
    /// 活动业务
    /// </summary>
    public class ActivityService : DbContext, IActivityService
    {
        /// <summary>
        /// 活动分页数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<Page<ActivityPageModel>> GetPageAsync(ActivitySearchModel model)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var activityList = Db.Queryable<c_activity,c_new_coupon_detials, c_activity_store>((ca,cncd, cas) => new object[] { JoinType.Left, cncd.activity_id == ca.id, JoinType.Left, ca.id == cas.activity_id })
                .Where((ca, cncd, cas) => ca.org_id == userInfo.org_id)
                .WhereIF(model.storeId > 0, (ca, cncd, cas) => cas.store_id == model.storeId || cas.store_id == null)
                .WhereIF(model.state >= 0, (ca, cncd, cas) => ca.state == model.state)
                .WhereIF(!string.IsNullOrEmpty(model.name), (ca, cncd, cas) => ca.name.Contains(model.name))
                .WhereIF(model.start_date != null, (ca, cncd, cas) => ca.start_date >= model.start_date)
                .WhereIF(model.end_date != null, (ca, cncd, cas) => ca.end_date <= model.end_date);

            var orderTypeStr = " Asc";
            if (model.orderType == 1)
            {
                orderTypeStr = " Desc";
            }

            var list = await activityList.GroupBy((ca, cncd, cas) => new { ca.id, ca.name, ca.start_date, ca.end_date, ca.content, ca.state, ca.creater, cncd.overlay, ca.address }).Select((ca, cncd, cas) => new ActivityPageModel { id = ca.id, name = ca.name, start_date = ca.start_date, end_date = ca.end_date, content = ca.content, state = ca.state, creater = ca.creater, address = ca.address, overlay = cncd.overlay }).OrderBy(model.order + orderTypeStr).WithCache().ToPageAsync(model.page, model.limit);

            var activity_ids = list.Items.Select(s => s.id).ToList();

            //查询活动关联优惠券
            var couponList = await Db.Queryable<c_new_coupon>().Where(w => activity_ids.Contains(w.activity_id)).WithCache().ToListAsync();

            //查询活动关联门店
            var activity_store_list = await Db.Queryable<c_activity_store, p_store>((cas, ps) => new object[] { JoinType.Left, cas.store_id == ps.id }).Where((cas, ps) => activity_ids.Contains(cas.activity_id)).Select((cas, ps) => new { cas.activity_id, ps.name, ps.id }).WithCache().ToListAsync();

            ////查询活动积分规则
            //var acList = await Db.Queryable<c_activity_level>().WithCache().ToListAsync();

            //返回用户，优惠券集合
            var newList = list.Items.Select(s => new ActivityPageModel { overlay = s.overlay, address = s.address, id = s.id, name = s.name, start_date = s.start_date, end_date = s.end_date, content = s.content, state = s.state, creater = s.creater, aCModel = couponList.Where(w => w.activity_id == s.id).ToList(), store = GetStore(activity_store_list.Where(w => w.activity_id == s.id).Select(ss => ss.name).ToList()), storeList = activity_store_list.Where(w => w.activity_id == s.id).Select(ss => new Store { id = ss.id, name = ss.name }).ToList(), isStart = GetStart(s.start_date) }).ToList();//, s.end_date
            list.Items = newList;
            return list;
        }

        /// <summary>
        /// 活动是否已开始
        /// </summary>
        /// <returns></returns>
        public int GetStart(DateTime? start)//, DateTime? end
        {
            if (start <= DateTime.Now.Date) // && DateTime.Now.Date <= end
            {
                return 2;
            }
            return 3;
        }

        /// <summary>
        /// 活动优惠券明细分页数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<dynamic> GetPageDetialsAsync(ActivityDetialsPageSearch entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var activityList = Db.Queryable<c_new_coupon_detials, c_activity, c_activity_store>((cncd, ca, cas) => new object[] { JoinType.Left, cncd.activity_id == ca.id, JoinType.Left, ca.id == cas.activity_id })
                .Where((cncd, ca, cas) => cncd.org_id == userInfo.org_id)
                .WhereIF(entity.store_id > 0, (cncd, ca, cas) => cas.store_id == entity.store_id || cas.store_id == null)
                .WhereIF(entity.state >= 0, (cncd, ca, cas) => ca.state == entity.state)
                .WhereIF(!string.IsNullOrEmpty(entity.name), (cncd, ca, cas) => ca.name.Contains(entity.name))
                .WhereIF(entity.start_date != null, (cncd, ca, cas) => ca.start_date >= entity.start_date)
                .WhereIF(entity.end_date != null, (cncd, ca, cas) => ca.end_date <= entity.end_date);

            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }

            var list = await activityList.GroupBy((cncd, ca, cas) => new { cncd.id, cncd.activity_id, ca.name, ca.start_date, ca.end_date, ca.content, ca.state, ca.creater, cncd.money, cncd.no, cncd.overlay, cncd.address, cncd.remarks })
                .Select((cncd, ca, cas) => new { cncd.id, cncd.activity_id, ca.name, ca.start_date, ca.end_date, ca.content, ca.state, ca.creater, cncd.money, cncd.no, cncd.overlay, cncd.address, cncd.remarks, store = "" })
                .OrderBy(entity.order + orderTypeStr)
                .WithCache()
                .ToPageAsync(entity.page, entity.limit);

            var activity_ids = list.Items.Select(s => s.id).ToList();

            //查询活动关联门店
            var activity_store_list = await Db.Queryable<c_activity_store, p_store>((cas, ps) => new object[] { JoinType.Left, cas.store_id == ps.id }).Where((cas, ps) => activity_ids.Contains(cas.activity_id)).Select((cas, ps) => new { cas.activity_id, ps.name }).WithCache().ToListAsync();

            var newList = list.Items.Select(s => new { s.id, s.activity_id, s.name, s.start_date, s.end_date, s.content, s.state, s.creater, s.money, s.no, s.overlay, s.address, s.remarks, store = GetStore(activity_store_list.Where(w => w.activity_id == s.activity_id).Select(ss => ss.name).ToList()) }).ToList();
            list.Items = newList;
            return list;
        }

        private string GetStore(List<string> list)
        {
            var txt = "";
            list.ForEach(c =>
            {
                txt += c + "/";
            });
            return txt.TrimEnd('/');
        }

        /// <summary>
        /// 添加活动、优惠券
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> Add(ActivityCouponModel entity)
        {
            if (entity.activityModel.start_date < DateTime.Now.Date)
            {
                throw new MessageException("开始时间不能小于当前时间！");
            }
            if (entity.activityModel.start_date > entity.activityModel.end_date)
            {
                throw new MessageException("开始时间不能大于结束时间！");
            }
            //查询用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            //查询是否存在此活动
            var isExtist = await Db.Queryable<c_activity>().WithCache().AnyAsync(s => s.org_id == userInfo.org_id && s.name == entity.activityModel.name && s.start_date == entity.activityModel.start_date);
            if (isExtist)
            {
                throw new MessageException("此活动已存在！");
            }

            var result = await Db.Ado.UseTranAsync(() =>
            {
                var activityEntity = new c_activity();
                activityEntity.org_id = userInfo.org_id;
                activityEntity.name = entity.activityModel.name;
                activityEntity.start_date = entity.activityModel.start_date;
                activityEntity.end_date = entity.activityModel.end_date;
                activityEntity.content = entity.activityModel.content;
                activityEntity.state = entity.activityModel.state;
                activityEntity.creater_id = userInfo.id;
                activityEntity.creater = userInfo.name;
                activityEntity.create_date = DateTime.Now;
                activityEntity.address = entity.activityModel.address;
                activityEntity.overlay = entity.activityModel.overlay;

                //返回活动
                var isAIdentity = Db.Insertable(activityEntity).ExecuteReturnIdentity();
                redisCache.RemoveAll<c_activity>();
                if (isAIdentity <= 0)
                {
                    throw new MessageException("活动未添加成功！");
                }
                //添加优惠券
                if (entity.aCModel.Count > 0)
                {
                    //添加活动优惠券明细
                    var coupon_detials_list = new List<c_new_coupon_detials>();
                    entity.aCModel.ForEach(c =>
                    {
                        var new_coupon = new c_new_coupon { org_id = userInfo.org_id, money = c.money, remarks = c.remarks, creater = userInfo.name, creater_id = userInfo.id, create_date = DateTime.Now, activity_id = isAIdentity, head = c.head, num = c.num, overlay = c.overlay };
                        var new_coupon_id = Db.Insertable(new_coupon).ExecuteReturnIdentity();
                        redisCache.RemoveAll<c_new_coupon>();
                        for (int i = 1; i <= c.num; i++)
                        {
                            coupon_detials_list.Add(new c_new_coupon_detials { activity_id = isAIdentity, address = activityEntity.address, overlay = activityEntity.overlay, money = c.money, no = c.head + i.ToString().PadLeft(8, '0'), org_id = userInfo.org_id, remarks = c.remarks, id = new_coupon_id, use_state = 3 });
                        }
                    });
                    Db.Insertable(coupon_detials_list).ExecuteCommand();
                    redisCache.RemoveAll<c_new_coupon_detials>();
                }

                //添加活动对应门店
                if (entity.store_id_list.Count > 0)
                {
                    var store_id_list = entity.store_id_list.Select(s => new c_activity_store { activity_id = isAIdentity, store_id = s }).ToList();
                    Db.Insertable(store_id_list).ExecuteCommand();
                    redisCache.RemoveAll<c_activity_store>();
                }

                ////添加活动积分规则
                //if (entity.activityLevelModel.Count > 0)
                //{
                //    foreach (var item in entity.activityLevelModel)
                //    {
                //        if (item.money_integral <= 0)
                //        {
                //            throw new MessageException($"等级:{item.level_name}现金积分不能<0!");
                //        }
                //        if (item.integral_money <= 0)
                //        {
                //            throw new MessageException($"等级:{item.level_name}积分兑换不能<0!");
                //        }
                //        if (item.discount_rate < 0 || item.discount_rate > 100)
                //        {
                //            throw new MessageException($"等级:{item.level_name}折扣率应在0-100之间!");
                //        }
                //    }

                //    //添加活动积分
                //    var allist = entity.activityLevelModel.Select(s => new c_activity_level { activity_id = isAIdentity, level_id = s.level_id, org_id = userInfo.org_id, store_id = entity.activityModel.store_id, start_date = Convert.ToDateTime(entity.activityModel.start_date), end_date = Convert.ToDateTime(entity.activityModel.end_date), money_integral = s.money_integral, integral_money = s.integral_money, discount_rate = s.discount_rate, creater_id = userInfo.id, creater = userInfo.name, create_date = DateTime.Now, actity_name = entity.activityModel.name, level_name = s.level_name, min_money = s.min_money, max_money = s.max_money }).ToList();

                //    Db.Insertable(allist).ExecuteCommand();
                //    redisCache.RemoveAll<c_activity_level>();
                //}
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 编辑活动、添加优惠券
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> Modify(ActivityCouponModel entity)
        {
            if (entity.activityModel.start_date > entity.activityModel.end_date)
            {
                throw new MessageException("开始时间不能大于结束时间!");
            }
            //查询用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            //查询当前活动
            var hd = await Db.Queryable<c_activity>().Where(s => s.id == entity.activityModel.id).WithCache().FirstAsync();
            if (hd == null)
            {
                throw new MessageException("未查询到活动!");
            }
            if (hd.start_date <= DateTime.Now)
            {
                throw new MessageException("此活动已开始，不能编辑!");
            }
            //查询是否存在此活动
            var isExtist = await Db.Queryable<c_activity>().WithCache().AnyAsync(s => s.org_id == userInfo.org_id && s.name == entity.activityModel.name && s.start_date == entity.activityModel.start_date && s.id != entity.activityModel.id);
            if (isExtist)
            {
                throw new MessageException("此活动已存在！");
            }

            var result = await Db.Ado.UseTranAsync(() =>
            {
                var activityEntity = new c_activity();
                activityEntity.name = entity.activityModel.name;
                activityEntity.start_date = entity.activityModel.start_date;
                activityEntity.end_date = entity.activityModel.end_date;
                activityEntity.content = entity.activityModel.content;
                activityEntity.address = entity.activityModel.address;
                activityEntity.overlay = entity.activityModel.overlay;

                //返回活动
                var isSuccess = Db.Updateable(activityEntity).IgnoreColumns(it => new { it.org_id, it.state, it.creater_id, it.creater, it.create_date }).Where(w => w.id == entity.activityModel.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                if (isSuccess <= 0)
                {
                    throw new MessageException("活动未编辑成功！");
                }

                //删除之前优惠券
                Db.Deleteable<c_new_coupon>().Where(w => w.activity_id == entity.activityModel.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                //添加优惠券
                if (entity.aCModel.Count > 0)
                {
                    //添加活动优惠券明细
                    var coupon_detials_list = new List<c_new_coupon_detials>();
                    entity.aCModel.ForEach(c =>
                    {
                        var new_coupon = new c_new_coupon { org_id = userInfo.org_id, money = c.money, remarks = c.remarks, creater = userInfo.name, creater_id = userInfo.id, create_date = DateTime.Now, activity_id = entity.activityModel.id, head = c.head, num = c.num, overlay = c.overlay };
                        var new_coupon_id = Db.Insertable(new_coupon).ExecuteReturnIdentity();
                        redisCache.RemoveAll<c_new_coupon>();
                        for (int i = 1; i <= c.num; i++)
                        {
                            coupon_detials_list.Add(new c_new_coupon_detials { activity_id = entity.activityModel.id, address = activityEntity.address, overlay = activityEntity.overlay, money = c.money, no = c.head + i.ToString().PadLeft(8, '0'), org_id = userInfo.org_id, remarks = c.remarks, id = new_coupon_id, use_state = 3 });
                        }
                    });
                    Db.Insertable(coupon_detials_list).ExecuteCommand();
                    redisCache.RemoveAll<c_new_coupon_detials>();
                }

                //删除之前活动对应门店
                Db.Deleteable<c_activity_store>().Where(w => w.activity_id == entity.activityModel.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                //添加活动对应门店
                if (entity.store_id_list.Count > 0)
                {
                    var store_id_list = entity.store_id_list.Select(s => new c_activity_store { activity_id = entity.activityModel.id, store_id = s }).ToList();
                    Db.Insertable(store_id_list).ExecuteCommand();
                    redisCache.RemoveAll<c_activity_store>();
                }

                //    // 删除活动等级积分
                //    Db.Deleteable<c_activity_level>().Where(w => w.activity_id == entity.activityModel.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                //    //编辑活动积分规则
                //    if (entity.activityLevelModel.Count > 0)
                //    {
                //        foreach (var item in entity.activityLevelModel)
                //        {
                //            if (item.money_integral <= 0)
                //            {
                //                throw new MessageException($"等级:{item.level_name}现金积分不能<0!");
                //            }
                //            if (item.integral_money <= 0)
                //            {
                //                throw new MessageException($"等级:{item.level_name}积分兑换不能<0!");
                //            }
                //            if (item.discount_rate < 0 || item.discount_rate > 100)
                //            {
                //                throw new MessageException($"等级:{item.level_name}折扣率应在0-100之间!");
                //            }
                //        }

                //        //添加活动等级积分
                //        var allist = entity.activityLevelModel.Select(s => new c_activity_level { activity_id = entity.activityModel.id, level_id = s.level_id, org_id = userInfo.org_id, store_id = entity.activityModel.store_id, start_date = Convert.ToDateTime(entity.activityModel.start_date), end_date = Convert.ToDateTime(entity.activityModel.end_date), money_integral = s.money_integral, integral_money = s.integral_money, discount_rate = s.discount_rate, creater_id = userInfo.id, creater = userInfo.name, create_date = DateTime.Now, actity_name = entity.activityModel.name, level_name = s.level_name, min_money = s.min_money, max_money = s.max_money }).ToList();

                //        Db.Insertable(allist).ExecuteCommand();
                //        redisCache.RemoveAll<c_activity_level>();
                //    }
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 启用、禁用活动
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> ModifyEnable(c_activity entity)
        {
            ////查询活动是否在使用
            //var is_use = await Db.Queryable<c_archives_activity_coupon>().WithCache().AnyAsync(w => w.activity_id == entity.id);
            //if (entity.start_date <= DateTime.Now.Date && is_use)
            //{
            //    throw new MessageException("活动已开始不能操作!");
            //}

            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var isS = await Db.Updateable(entity).SetColumns(w => new c_activity { state = entity.state }).Where(w => w.id == entity.id && w.org_id == userInfo.org_id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
            if (isS <= 0)
            {
                throw new MessageException("未操作成功!");
            }
            return isS;
        }

        /// <summary>
        /// 删除活动
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> Delete(int id)
        {
            //查询活动是否在使用
            var is_use = await Db.Queryable<c_activity>().WithCache().AnyAsync(w => w.id == id && w.start_date <= DateTime.Now);
            if (is_use)
            {
                throw new MessageException("当前活动正在进行中，不能删除!");
            }
            var result = await Db.Ado.UseTranAsync(() =>
            {
                ////删除优惠券
                //Db.Deleteable<c_activity_coupon>().Where(w => w.activity_id == id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                ////删除活动等级规则
                //Db.Deleteable<c_activity_level>().Where(w => w.activity_id == id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                //删除活动
                Db.Deleteable<c_activity>().Where(w => w.id == id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

            });
            return result.IsSuccess;
        }

        /// <summary>
        /// 领取优惠券
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> CouponCollection(ArchivesActivityCouponModel entity)
        {
            if (entity.id <= 0 || entity.aCModel == null)
            {
                throw new MessageException("没有优惠券可领取!");
            }
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            var list = new List<c_archives_activity_coupon>();

            var result = await Db.Ado.UseTranAsync(() =>
            {
                //删除此用户领取活动的其他优惠券
                Db.Deleteable<c_archives_activity_coupon>().Where(s => entity.aArray.Contains(s.archives_id) && s.activity_id == entity.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                foreach (var item in entity.aCModel)
                {
                    foreach (var item1 in entity.aArray)
                    {
                        var aac = new c_archives_activity_coupon();
                        aac.activity_id = entity.id;
                        aac.coupon_no ="";
                        aac.org_id = userInfo.org_id;
                        aac.store_id = entity.store_id;
                        aac.coupon_id = item.id;
                        aac.state = 0;
                        aac.use_date = DateTime.Now;
                        aac.remarks = item.remarks;
                        aac.archives_id = item1;
                        list.Add(aac);
                    }

                }
                var isS = Db.Insertable(list).ExecuteCommand();
                redisCache.RemoveAll<c_archives_activity_coupon>();
                if (isS <= 0)
                {
                    throw new MessageException("未领取成功!");
                }
            });

            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;

        }


        /// <summary>
        /// 根据活动优惠券返回档案用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<List<int>> ReturnArchives(ReturnArchivesModel model)
        {
            if (model.activity_id <= 0 || string.IsNullOrEmpty(model.coupList))
            {
                throw new MessageException("没有获取到活动信息!");
            }
            //将优惠券字符串分隔成数组
            var couList = new List<int>();
            couList = model.coupList.Split(',').Select(s => int.Parse(s)).ToList();

            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            //获取会员领取优惠券数据
            var getList = await Db.Queryable<c_archives_activity_coupon>().Where(w => w.store_id == model.store_id && w.org_id == userInfo.org_id && w.activity_id == model.activity_id && couList.Contains(w.coupon_id)).GroupBy(w => new { w.archives_id }).Having(h => SqlFunc.AggregateCount(h.archives_id) == couList.Count).Select(s => s.archives_id).WithCache().ToListAsync();

            return getList;


        }

        /// <summary>
        /// 设置图片
        /// </summary>
        /// <param name="image"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<bool> SetAsync(ActivityIMGModel image, dynamic type)
        {
            if (image.activity_id <= 0)
            {
                throw new MessageException("活动id不正确!");
            }

            string currentDirectory = Path.GetDirectoryName(type.Assembly.Location);

            var newVs = image.vs;//传过来的图片
            var list = Db.Queryable<r_activity_img>().Where(w => w.activity_id == image.activity_id && !newVs.Contains(w.img_url)).WithCache().ToList();//需要删除的
            foreach (var item in list)
            {
                var url = currentDirectory + "/wwwroot/" + item.img_url.Trim();
                if (System.IO.File.Exists(url))
                {
                    System.IO.File.Delete(url);//删除无用文件
                }
            }
            var newImage = new List<r_activity_img>();
            for (int i = 0; i < newVs.Count; i++)
            {
                //新增
                newImage.Add(new r_activity_img { activity_id = image.activity_id, img_url = newVs[i] });
            }
            var ret = -1;
            ret = await Db.Deleteable<r_activity_img>().Where(w => w.activity_id == image.activity_id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
            if (ret != -1)
            {
                await Db.Insertable(newImage).ExecuteCommandAsync();
                redisCache.RemoveAll<r_activity_img>();
            }
            return true;
        }

        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="activity_id">活动id</param>
        /// <returns></returns>
        public async Task<List<r_activity_img>> GetAsync(int activity_id)
        {
            return await Db.Queryable<r_activity_img>().Where(w => w.activity_id == activity_id).WithCache().ToListAsync();
        }

        /// <summary>
        /// 获取图片（客户端）
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> GetByNowAsync()
        {
            //获取客户信息
            var archivesInfo = new Tools.IdentityModels.GetArchives().archives;

            var list = await Db.Queryable<r_activity_img, c_activity>((ai, a) => new object[] { JoinType.Left, ai.activity_id == a.id }).Where((ai, a) => a.org_id == archivesInfo.org_id && SqlFunc.Between(DateTime.Now.Date, a.start_date, a.end_date) && a.state == 1).Select((ai, a) => new { id = ai.activity_id, url = ai.img_url, type = 1 }).WithCache().ToListAsync();
            if (list.Count == 0)
            {
                list = await Db.Queryable<p_image, p_org>((i, o) => new object[] { JoinType.Left, i.relation_code == o.code }).Where((i, o) => o.id == archivesInfo.org_id).Select((i, o) => new { o.id, i.url, type = 2 }).WithCache().ToListAsync();
            }
            return list;
        }

        /// <summary>
        /// 获取活动详情（客户端）
        /// </summary>
        /// <param name="activity_id">活动id</param>
        /// <returns></returns>
        public async Task<dynamic> GetDetialsAsync(int activity_id)
        {
            return await Db.Queryable<c_activity>().Where(w => w.id == activity_id).Select(s => new { s.name, s.start_date, s.end_date, s.content }).WithCache().FirstAsync();
        }

        /// <summary>
        /// 获取优惠券领取分页记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Page<ReceiveCouponModel>> GetRecivePage(ReceiveCouponSearch entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }

            return await Db.Queryable<c_archives_activity_coupon,c_archives,c_activity>((aac,a,c)=>new object[] {JoinType.Left,aac.archives_id==a.id,JoinType.Left,aac.activity_id==c.id })
                            .Where((aac,a,c)=>aac.org_id==userInfo.org_id)
                            .WhereIF(!string.IsNullOrEmpty(entity.name),(aac,a,c)=>aac.coupon_no.Contains(entity.name)||a.name.Contains(entity.name)||c.name.Contains(entity.name))
                            .WhereIF(entity.startTime != null, (aac, a, c) => aac.use_date >= entity.startTime)
                            .WhereIF(entity.endTime != null, (aac, a, c) => aac.use_date <= entity.endTime)
                            .Select((aac, a, c)=>new ReceiveCouponModel { activity_id=aac.activity_id, org_id=aac.org_id, activity_name=c.name, archives_id=aac.archives_id, archives_name=a.name, coupon_no=aac.coupon_no,  remarks=aac.remarks, use_date=aac.use_date, state=aac.state, store_id=aac.store_id, coupon_id=aac.coupon_id, archives_phone=a.phone })
                            .OrderBy(entity.order + orderTypeStr)
                            .WithCache()
                            .ToPageAsync(entity.page, entity.limit);
        }
    }
}
