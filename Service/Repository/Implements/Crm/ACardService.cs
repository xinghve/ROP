using Microsoft.AspNetCore.Http;
using Models.DB;
using Models.View.Crm;
using Models.View.Mobile;
using Models.View.Public;
using Service.Extensions;
using Service.Repository.Interfaces.Crm;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Tools;
using Tools.Filter;

namespace Service.Repository.Implements.Crm
{
    /// <summary>
    /// 会员卡业务操作
    /// </summary>
    public class ACardService : DbContext, IACardService
    {
        /// <summary>
        /// 会员卡分页查询
        /// </summary>
        /// <param name="model"></param>
        /// <param name="name_or_phone"></param>
        /// <returns></returns>
        public async Task<Page<CardPageModel>> GetPageAsync(ACardModel model, bool name_or_phone = false)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var orderTypeStr = " Asc";
            if (model.orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            if (model.order.ToLower() == "id" || model.order.ToLower() == "name")
            {
                model.order = "ar." + model.order;
            }
            var list = await Db.Queryable<c_archives, c_account, p_store, c_archives_level, c_archives_supplement>((ar, ac, s, al, asl) => new object[] { JoinType.Left, ar.id == ac.archives_id, JoinType.Left, ar.store_id == s.id, JoinType.Left, ar.grade_id == al.id, JoinType.Left, ar.id == asl.archives_id })
                   .Where((ar, ac, s, al, asl) => ar.org_id == userInfo.org_id)
                   .WhereIF(!string.IsNullOrEmpty(model.name) && !name_or_phone, (ar, ac, s, al, asl) => ar.name.Contains(model.name) || ar.card_no.Contains(model.name) || ar.virtual_no.Contains(model.name))
                   .WhereIF(!string.IsNullOrEmpty(model.phone) && !name_or_phone, (ar, ac, s, al, asl) => ar.phone.Contains(model.phone) || ar.emergencyno.Contains(model.phone))
                   .WhereIF(name_or_phone && (!string.IsNullOrEmpty(model.phone) || !string.IsNullOrEmpty(model.name)), (ar, ac, s, al, asl) => ar.phone.Contains(model.phone) || ar.emergencyno.Contains(model.phone) || ar.name.Contains(model.name) || ar.card_no.Contains(model.name))
                   .WhereIF(!string.IsNullOrEmpty(model.startintegral), (ar, ac, s, al, asl) => ac.integral >= Convert.ToDecimal(model.startintegral))
                   .WhereIF(!string.IsNullOrEmpty(model.endintegral), (ar, ac, s, al, asl) => ac.integral <= Convert.ToDecimal(model.endintegral))
                   .WhereIF(model.startTime != null, (ar, ac, s, al, asl) => ar.create_date >= model.startTime)
                   .WhereIF(model.endTime != null, (ar, ac, s, al, asl) => ar.create_date <= model.endTime)
                   .WhereIF(model.storeId > 0, (ar, ac, s, al, asl) => ar.store_id == model.storeId)
                   .WhereIF(model.levelId > 0, (ar, ac, s, al, asl) => ar.grade_id == model.levelId)
                   .WhereIF(model.is_me, (ar, ac, s, al, asl) => ar.to_director_id == userInfo.id)
                   .GroupBy((ar, ac, s, al, asl) => new { ar.id, ar.store_id, ar.name, ar.phone, ar.sex, ar.card_no, ar.virtual_no, level_id = ar.grade_id, level = al.name, ac.coupon, ac.recharge, ac.consume, ac.balance, ac.integral, ac.noneamount, ac.salseamount, ac.rate, ac.settleamount, ac.amount, ac.total_coupon, ac.total_integral, ar.creater, ar.create_date, store_name = s.name, al.balance_limit_lower, ar.image_url, asl.drink, asl.foods, asl.fruits, asl.habit, asl.hobby, asl.smoke, ar.id_card })
                   .Select((ar, ac, s, al, asl) => new CardPageModel { archives_id = ar.id, store_id = ar.store_id, archives = ar.name, archives_phone = ar.phone, sex = ar.sex, card_no = ar.card_no, virtual_no = ar.virtual_no, level_id = ar.grade_id, level = al.name, coupon = ac.coupon, recharge = ac.recharge, consume = ac.consume, balance = ac.balance, integral = ac.integral, noneamount = ac.noneamount, salseamount = ac.salseamount, rate = ac.rate, settleamount = ac.settleamount, amount = ac.amount, total_coupon = ac.total_coupon, total_integral = ac.total_integral, creater = ar.creater, create_date = ar.create_date, store_name = s.name, balance_limit_lower = al.balance_limit_lower, image_url = ar.image_url, drink = asl.drink, foods = asl.foods, fruits = asl.fruits, habit = asl.habit, hobby = asl.hobby, smoke = asl.smoke, id_card = ar.id_card })
                   .OrderBy(model.order + orderTypeStr)
                   .WithCache()
                   .ToPageAsync(model.page, model.limit);

            //已领取的优惠券
            var getList = await Db.Queryable<c_archives_activity_coupon, c_archives, c_new_coupon_detials, c_activity>((aac, ac, c, a) => new object[] { JoinType.Left, aac.archives_id == ac.id, JoinType.Left, aac.coupon_id == c.id, JoinType.Left, aac.activity_id == a.id }).Select((aac, ac, c, a) => new { activityName = a.name, c.id, aac.state, c.no, c.money, c.overlay, aac.archives_id, a.start_date, a.end_date }).WithCache().ToListAsync();

            //返回用户，优惠券集合
            var newList = list.Items.Select(s => new CardPageModel { balance_limit_lower = s.balance_limit_lower, archives_id = s.archives_id, store_id = s.store_id, archives = s.archives, archives_phone = s.archives_phone, sex = s.sex, card_no = s.card_no, virtual_no = s.virtual_no, level_id = s.level_id, level = s.level, coupon = s.coupon, recharge = s.recharge, consume = s.consume, balance = s.balance, integral = s.integral, noneamount = s.noneamount, salseamount = s.salseamount, rate = s.rate, settleamount = s.settleamount, amount = s.amount, total_coupon = s.total_coupon, total_integral = s.total_integral, creater = s.creater, create_date = s.create_date, store_name = s.store_name, image_url = Utils.GetImage_url(s.image_url, s.sex), drink = s.drink, foods = s.foods, fruits = s.fruits, habit = s.habit, hobby = s.hobby, smoke = s.smoke, id_card = s.id_card, couponList = getList.Where(w => w.archives_id == s.archives_id && DateTime.Now >= w.start_date && DateTime.Now <= w.end_date).Select(se => new ArchivesCardModel { activityname = se.activityName, id = se.id, use_money = se.money, no = se.no, state = se.state, start_date = se.start_date, end_date = se.end_date, overlay = se.overlay }).ToList() }).ToList();
            list.Items = newList;

            return list;


        }

        /// <summary>
        /// 会员充值
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Recharge(RechargeModel model)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var result = await Db.Ado.UseTranAsync(() =>
             {
                 //查询会员信息
                 var cardDetail = Db.Queryable<c_archives, c_account>((a, ac) => new object[] { JoinType.Left, a.id == ac.archives_id }).Where((a, ac) => a.id == model.archives_id && a.org_id == userInfo.org_id && a.state == 1).Select((a, ac) => new { a.id, ac.balance, ac.coupon, ac.integral, ac.recharge, ac.rate, ac.total_coupon, ac.total_integral, a.grade_id, a.to_director_id, a.to_director, ac.amount, ac.consume, ac.noneamount, ac.password, ac.salseamount, ac.settleamount }).WithCache().First();
                 if (cardDetail == null)
                 {
                     throw new MessageException("会员信息被禁用或未获取到!");
                 }

                 //查询积分规则，是否启用积分
                 var isIntegral = Db.Queryable<p_org>().Where(s => s.id == userInfo.org_id).Select(s => s.integral_card).WithCache().First();
                 //new 会员账户
                 var cCard = new c_account();
                 //查询会员等级
                 var alevelList = Db.Queryable<c_archives_level>().Where(w => w.org_id == userInfo.org_id).WithCache().ToList();
                 //查询活动会员等级
                 var activityLevelList = Db.Queryable<c_activity_level>().Where(w => w.org_id == userInfo.org_id && w.store_id == model.store_id).WithCache().ToList();
                 //定义现金积分
                 short? xj = 0;
                 //查询会员卡等级信息
                 var level = new c_archives_level();
                 if (model.level_id == 0)
                 {
                     level = alevelList.Where(w => w.min_money == 0&&w.special!=2).FirstOrDefault();
                 }
                 else
                 {
                     level = alevelList.Where(w => w.id == model.level_id).FirstOrDefault();
                 }
                 if (level == null)
                 {
                     throw new MessageException("请先增加会员等级信息!");
                 }
                 //启用积分
                 if (isIntegral == 2)
                 {

                     xj = level.money_integral;

                     //查询充值金额所在区间的积分规则
                     var amountInterval = alevelList.Where(w => w.min_money <= model.money).OrderByDescending(o => o.min_money).Select(s => new { s.max_money, s.min_money, s.money_integral }).FirstOrDefault();
                     xj = amountInterval.money_integral;

                     //查询会员卡所在区间积分规则
                     var accountList = alevelList.Where(s => s.id == cardDetail.grade_id && s.org_id == userInfo.org_id).Select(s => new { s.max_money, s.min_money, s.money_integral }).FirstOrDefault();
                     if (accountList != null && accountList.min_money > amountInterval.min_money)
                     {
                         xj = accountList.money_integral;
                     }

                     //查询活动等级数据
                     //var alList = Db.Queryable<c_activity_level, c_activity>((al, a) => new object[] { JoinType.Left, al.activity_id == a.id }).Where((al, a) => a.state == 1 && a.org_id == userInfo.org_id && a.store_id == model.store_id && SqlFunc.Between(DateTime.Now, al.start_date, al.end_date) && al.level_id == model.level_id).WithCache().First();

                     //if (alList != null)
                     //{
                     //    //查询活动表充值金额所在区间的积分规则
                     //    var amountInterval = activityLevelList.Where(w => w.min_money <= model.money).OrderByDescending(o => o.min_money).Select(s => new { s.max_money, s.min_money, s.money_integral }).FirstOrDefault();
                     //    xj = amountInterval.money_integral;

                     //    //查询活动表会员卡所在区间积分规则
                     //    var accountList = activityLevelList.Where(s => s.level_id == cardDetail.grade_id && s.org_id == userInfo.org_id).Select(s => new { s.max_money, s.min_money, s.money_integral }).First();
                     //    if (accountList != null && accountList.min_money > amountInterval.min_money)
                     //    {
                     //        xj = accountList.money_integral;
                     //    }

                     //}
                     //else
                     //{
                     //    //查询充值金额所在区间的积分规则
                     //    var amountInterval = alevelList.Where(w => w.min_money <= model.money).OrderByDescending(o => o.min_money).Select(s => new { s.max_money, s.min_money, s.money_integral }).FirstOrDefault();
                     //    xj = amountInterval.money_integral;

                     //    //查询会员卡所在区间积分规则
                     //    var accountList = alevelList.Where(s => s.id == cardDetail.grade_id && s.org_id == userInfo.org_id).Select(s => new { s.max_money, s.min_money, s.money_integral }).FirstOrDefault();
                     //    if (accountList != null && accountList.min_money > amountInterval.min_money)
                     //    {
                     //        xj = accountList.money_integral;
                     //    }
                     //}


                     //修改档案账户总金额，积分，赠送金额
                     cCard.recharge = Convert.ToDecimal(cardDetail.recharge + model.money);//充值总金额
                     cCard.balance = Convert.ToDecimal(cardDetail.balance + model.money);//余额
                     cCard.total_integral = Convert.ToInt32(cardDetail.total_integral + (model.money / xj));//总积分
                     cCard.integral = Convert.ToInt32(cardDetail.integral + (model.money / xj));//剩余积分
                     cCard.coupon = Convert.ToDecimal(cardDetail.coupon + model.give_money);//赠送金额
                     cCard.total_coupon = Convert.ToDecimal(cardDetail.total_coupon + model.give_money);//赠送总金额
                 }
                 //不积分
                 else
                 {
                     //修改档案账户总金额，积分，赠送金额
                     cCard.recharge = Convert.ToDecimal(cardDetail.recharge + model.money);//充值总金额
                     cCard.balance = Convert.ToDecimal(cardDetail.balance + model.money);//余额
                     cCard.total_integral = Convert.ToInt32(cardDetail.total_integral);//总积分
                     cCard.integral = Convert.ToInt32(cardDetail.integral);//剩余积分
                     cCard.coupon = Convert.ToDecimal(cardDetail.coupon + model.give_money);//赠送金额
                     cCard.total_coupon = Convert.ToDecimal(cardDetail.total_coupon + model.give_money);//赠送总金额

                 }
                 var rate = Convert.ToInt16(cardDetail.rate);
                 var grade = cardDetail.grade_id;
                 short? money_integral = 0;
                 money_integral = level.money_integral;
                 var special = level.special;
                 if (special == 3)
                 {
                     //返回会员现等级
                     var newLevel = alevelList.Where(w => w.min_money <= cCard.recharge && w.special == 3).OrderByDescending(o => o.min_money).First();
                     rate = Convert.ToInt16(newLevel.royalty_rate);
                     grade = newLevel.id;
                     money_integral = newLevel.money_integral;

                     //修改会员档案等级
                     Db.Updateable<c_archives>()
                       .SetColumns(it => new c_archives { grade_id = grade })
                       .Where(a => a.id == model.archives_id && a.org_id == userInfo.org_id).RemoveDataCache().EnableDiffLogEvent()
                       .ExecuteCommand();
                 }

                 var Encrypt = cardDetail.amount + cardDetail.id + cCard.balance + cardDetail.consume + cCard.coupon + cCard.integral + cardDetail.noneamount + cardDetail.password + rate + cCard.recharge + cardDetail.salseamount + cardDetail.settleamount + cCard.total_coupon + cCard.total_integral;
                 var code = MetarnetRegex.Encrypt(MetarnetRegex.SHA512Encrypt(Encrypt.ToString()));
                 //修改会员账户积分
                 var isS = Db.Updateable<c_account>()
                    .SetColumns(it => new c_account { coupon = cCard.coupon, recharge = cCard.recharge, balance = cCard.balance, total_integral = cCard.total_integral, integral = cCard.integral, rate = rate, total_coupon = cCard.total_coupon })
                    .Where(a => a.archives_id == model.archives_id).RemoveDataCache().EnableDiffLogEvent()
                    .ExecuteCommand();


                 if (isS <= 0)
                 {
                     throw new MessageException("会员信息未充值成功!");
                 }

                 //查询券码是否存在
                 var isExist = Db.Queryable<c_new_coupon_detials, c_activity>((n, a) => new object[] { JoinType.Left, n.activity_id == a.id })
                       .Where((n, a) => n.org_id == userInfo.org_id && n.use_state == 3 && a.start_date <= DateTime.Now.Date && a.end_date >= DateTime.Now.Date && n.no == model.couponCode.Trim() && a.state == 1)
                       .Select((n, a) => new { n.money, n.no, a.name, a.id, coupon_id = n.id, a.start_date, a.end_date })
                       .WithCache()
                       .First();


                 //增加充值记录
                 var recharge = new r_recharge();
                 recharge.archives_id = Convert.ToInt32(model.archives_id);
                 recharge.card_no = model.card_no;
                 recharge.archives = model.archives;
                 recharge.archives_phone = model.archives_phone;
                 recharge.recharge_date = DateTime.Now;
                 recharge.recharge_money = model.money;
                 if (xj == 0)
                 {
                     recharge.recharge_integral = 0;
                     recharge.money_integral = money_integral;
                 }
                 else
                 {
                     recharge.recharge_integral = Convert.ToInt16(model.money / xj);
                     recharge.money_integral = xj;
                 }
                 recharge.total_money = cCard.recharge;
                 recharge.total_integral = cCard.total_integral;
                 recharge.way_code = model.way_code;
                 recharge.way = model.way;
                 recharge.bill_no = model.archives_id + DateTime.Now.ToString("yyyyMMddhhmmssffffff");
                 recharge.occurrence_date = DateTime.Now;
                 recharge.level = model.level_id == 0 ? level.name : model.level;
                 recharge.org_id = userInfo.org_id;
                 recharge.store_id = model.store_id;
                 recharge.balance = cCard.balance;
                 recharge.integral = Convert.ToInt32(cCard.integral);
                 recharge.give_total_money = cCard.total_coupon;
                 recharge.give_balance = cCard.coupon;
                 recharge.give_money = isExist == null ? model.give_money : isExist.money;
                 recharge.creater = userInfo.name;
                 recharge.creater_id = userInfo.id;
                 recharge.consultation_id = 0;
                 recharge.categroy_id = 1;
                 if (model.no != null)
                 {
                     recharge.no = model.no;
                 }
                 else
                 {
                     recharge.no = model.archives_id + DateTime.Now.ToString("yyMMddhhmmssffff");
                 }
                 recharge.state_id = 6;
                 recharge.state = "已充值";
                 recharge.to_director_id = cardDetail.to_director_id;
                 recharge.to_director = cardDetail.to_director;
                 recharge.coupon_no = isExist == null ? "" : isExist.no;

                 Db.Insertable(recharge).ExecuteCommand();
                 redisCache.RemoveAll<r_recharge>();

                 //修改优惠券的状态
                 if (!string.IsNullOrEmpty(model.couponCode))
                 {
                     if (isExist == null)
                     {
                         throw new MessageException("未获取到活动！");
                     }

                     var isTrue = Db.Updateable<c_new_coupon_detials>()
                       .SetColumns(w => w.use_state == 2)
                       .Where(w => w.org_id == userInfo.org_id && w.activity_id == isExist.id && w.id == isExist.coupon_id && w.no == model.couponCode)
                       .RemoveDataCache()
                       .EnableDiffLogEvent()
                       .ExecuteCommand();
                     if (isTrue <= 0)
                     {
                         throw new MessageException("优惠券未使用成功！");
                     }

                     //添加到会员领取表
                     var archives_coupon = new c_archives_activity_coupon();
                     archives_coupon.archives_id = model.archives_id.Value;
                     archives_coupon.activity_id = model.activity_id;
                     archives_coupon.coupon_id = model.coupon_id;
                     archives_coupon.state = 2;
                     archives_coupon.org_id = userInfo.org_id;
                     archives_coupon.store_id = model.store_id;
                     archives_coupon.use_date = DateTime.Now;
                     archives_coupon.coupon_no = isExist.no;

                     Db.Insertable(archives_coupon).ExecuteCommand();
                     redisCache.RemoveAll<c_archives_activity_coupon>();
                 }

             });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 充值记录分页查询
        /// </summary>
        /// <param name="model"></param>
        /// <param name="name_or_phone"></param>
        /// <param name="is_me"></param>
        /// <returns></returns>
        public async Task<Page<Recharge>> GetRechargePageAsync(RJLModel model, bool name_or_phone = false, bool is_me = false)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            DateTime endTime = DateTime.Parse("3000-12-31 23:59:59");
            if (model.endTime != null)
            {
                endTime = DateTime.Parse(model.endTime + " 23:59:59");
            }

            var rechargeList = Db.Queryable<r_recharge, c_archives>((w, a) => new object[] { JoinType.Left, w.archives_id == a.id })
                   .Where((w, a) => w.org_id == userInfo.org_id && w.categroy_id == 1)
                   .WhereIF(!string.IsNullOrEmpty(model.name) && !name_or_phone, (w, a) => w.archives.Contains(model.name) || w.card_no.Contains(model.name) || a.virtual_no.Contains(model.name))
                   .WhereIF(!string.IsNullOrEmpty(model.phone) && !name_or_phone, (w, a) => w.archives_phone.Contains(model.phone))
                   .WhereIF(name_or_phone, (w, a) => w.archives.Contains(model.name) || w.card_no.Contains(model.name) || w.archives_phone.Contains(model.phone))
                   .WhereIF(is_me, (w, a) => w.to_director_id == userInfo.id)
                   .WhereIF(!string.IsNullOrEmpty(model.bill_no), (w, a) => w.bill_no.Contains(model.bill_no))
                   .WhereIF(!string.IsNullOrEmpty(model.level), (w, a) => w.level.Contains(model.level))
                   .WhereIF(!string.IsNullOrEmpty(model.way_code), (w, a) => w.way_code.Contains(model.way_code))
                   .WhereIF(model.startTime != null, (w, a) => w.recharge_date >= model.startTime)
                   .WhereIF(model.endTime != null, (w, a) => w.recharge_date <= endTime)
                   .WhereIF(model.storeId > 0, (w, a) => w.store_id == model.storeId)
                   .Select((w, a) => new Recharge { archives = w.archives, archives_id = w.archives_id, virtual_no = a.virtual_no, archives_phone = w.archives_phone, balance = w.balance, bank = w.bank, bill_no = w.bill_no, card_no = w.card_no, categroy_id = w.categroy_id, check_id = w.check_id, consultation_id = w.consultation_id, coupon_no = w.coupon_no, creater = w.creater, creater_id = w.creater_id, give_balance = w.give_balance, give_money = w.give_money, give_total_money = w.give_total_money, integral = w.integral, level = w.level, money_integral = w.money_integral, no = w.no, occurrence_date = w.occurrence_date, org_id = w.org_id, recharge_date = w.recharge_date, recharge_integral = w.recharge_integral, recharge_money = w.recharge_money, remark = w.remark, state = w.state, state_id = w.state_id, store_id = w.store_id, total_integral = w.total_integral, total_money = w.total_money, to_director = w.to_director, to_director_id = w.to_director_id, way = w.way, way_code = w.way_code });
            var orderTypeStr = " Asc";
            if (model.orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            rechargeList = rechargeList.OrderBy(model.order + orderTypeStr);
            return await rechargeList.WithCache().ToPageAsync(model.page, model.limit);
        }

        /// <summary>
        /// 获取充值业绩
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<dynamic> GetRechargeAsync(short type)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            //获取负责人所有充值
            var recharge = await Db.Queryable<r_recharge>().Where(w => w.creater_id == userInfo.id && w.categroy_id == 1).WithCache().ToListAsync();
            //总绩效
            var sum = recharge.Sum(s => s.recharge_money);
            decimal? type_sum = 0;
            if (type == 1)//年
            {
                type_sum = recharge.Where(w => w.recharge_date.Year == DateTime.Now.Year).Sum(s => s.recharge_money);
            }
            else if (type == 2)//月
            {
                type_sum = recharge.Where(w => w.recharge_date.Year == DateTime.Now.Year && w.recharge_date.Month == DateTime.Now.Month).Sum(s => s.recharge_money);
            }
            else//日
            {
                type_sum = recharge.Where(w => w.recharge_date.Year == DateTime.Now.Year && w.recharge_date.Month == DateTime.Now.Month && w.recharge_date.Day == DateTime.Now.Day).Sum(s => s.recharge_money);
            }
            return new { sum, type_sum };
        }

        /// <summary>
        /// 获取充值业绩排行
        /// </summary>
        /// <param name="store_id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<dynamic> GetRechargeOrderAsync(int store_id, short type)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            //获取所有充值
            var recharge = await Db.Queryable<r_recharge>().Where(w => w.categroy_id == 1 && w.org_id == userInfo.org_id && w.store_id == store_id).WithCache().ToListAsync();
            //总绩效
            var order_list = recharge.Select(s => new { s.creater_id, sum = recharge.Sum(sum => sum.recharge_money) }).Distinct().OrderByDescending(o => o.sum).ToList();
            //获取登录人的业绩
            var item = order_list.Where(w => w.creater_id == userInfo.id).FirstOrDefault();
            //获得排行
            var order = 0;
            if (item != null)
            {
                order = order_list.IndexOf(item) + 1;
            }
            else
            {
                order = order_list.Count + 1;
            }
            if (type == 1)//年
            {
                order_list = recharge.Where(w => w.recharge_date.Year == DateTime.Now.Year).Select(s => new { s.creater_id, sum = recharge.Sum(sum => sum.recharge_money) }).Distinct().OrderByDescending(o => o.sum).ToList();
            }
            else if (type == 2)//月
            {
                order_list = recharge.Where(w => w.recharge_date.Year == DateTime.Now.Year && w.recharge_date.Month == DateTime.Now.Month).Select(s => new { s.creater_id, sum = recharge.Sum(sum => sum.recharge_money) }).Distinct().OrderByDescending(o => o.sum).ToList();
            }
            else//日
            {
                order_list = recharge.Where(w => w.recharge_date.Year == DateTime.Now.Year && w.recharge_date.Month == DateTime.Now.Month && w.recharge_date.Day == DateTime.Now.Day).Select(s => new { s.creater_id, sum = recharge.Sum(sum => sum.recharge_money) }).Distinct().OrderByDescending(o => o.sum).ToList();
            }
            //获取登录人对应类型的业绩
            item = order_list.Where(w => w.creater_id == userInfo.id).FirstOrDefault();
            //获得对应类型排行
            var type_order = 0;
            if (item != null)
            {
                type_order = order_list.IndexOf(item) + 1;
            }
            else
            {
                type_order = order_list.Count + 1;
            }
            return new { order, type_order };
        }

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<List<CardPageModel>> GetPrintAsync(PrintModel model)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            var list = await Db.Queryable<c_archives, c_account, p_store, c_archives_level>((ar, ac, s, al) => new object[] { JoinType.Left, ar.id == ac.archives_id, JoinType.Left, ar.store_id == s.id, JoinType.Left, ar.grade_id == al.id })
                   .Where((ar, ac, s, al) => ar.org_id == userInfo.org_id)
                   .WhereIF(!string.IsNullOrEmpty(model.name), (ar, ac, s, al) => ar.name.Contains(model.name) || ar.card_no.Contains(model.name))
                   .WhereIF(!string.IsNullOrEmpty(model.phone), (ar, ac, s, al) => ar.phone.Contains(model.phone) || ar.emergencyno.Contains(model.phone))
                   .WhereIF(model.startTime != null, (ar, ac, s, al) => ar.create_date >= model.startTime)
                   .WhereIF(model.endTime != null, (ar, ac, s, al) => ar.create_date <= model.endTime)
                   .WhereIF(model.storeId > 0, (ar, ac, s, al) => ar.store_id == model.storeId)
                   .WhereIF(model.levelId > 0, (ar, ac, s, al) => ar.grade_id == model.levelId)
                   .GroupBy((ar, ac, s, al) => new { ar.id, ar.store_id, ar.name, ar.phone, ar.sex, ar.card_no, ar.virtual_no, level_id = ar.grade_id, level = al.name, ac.coupon, ac.recharge, ac.consume, ac.balance, ac.integral, ac.noneamount, ac.salseamount, ac.rate, ac.settleamount, ac.amount, ac.total_coupon, ac.total_integral, ar.creater, ar.create_date, store_name = s.name, al.balance_limit_lower })
                   .OrderBy((ar, ac, s, al) => ar.name)
                   .Select((ar, ac, s, al) => new CardPageModel { archives_id = ar.id, store_id = ar.store_id, archives = ar.name, archives_phone = ar.phone, sex = ar.sex, card_no = ar.card_no, virtual_no = ar.virtual_no, level_id = ar.grade_id, level = al.name, coupon = ac.coupon, recharge = ac.recharge, consume = ac.consume, balance = ac.balance, integral = ac.integral, noneamount = ac.noneamount, salseamount = ac.salseamount, rate = ac.rate, settleamount = ac.settleamount, amount = ac.amount, total_coupon = ac.total_coupon, total_integral = ac.total_integral, creater = ar.creater, create_date = ar.create_date, store_name = s.name, balance_limit_lower = al.balance_limit_lower })
                   .WithCache()
                   .ToListAsync();

            return list;
        }

        /// <summary>
        /// 获取账户信息（移动端）
        /// </summary>
        /// <param name="archives_id"></param>
        /// <returns></returns>
        public async Task<dynamic> GetAccountAsync(int archives_id)
        {
            return await Db.Queryable<c_account>().Where(w => w.archives_id == archives_id).Select(s => new { s.balance, s.coupon, s.integral }).WithCache().FirstAsync();
        }

        /// <summary>
        /// 获取档案账户信息（客户端）
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> GetArcAccountAsync()
        {
            //获客户信息
            var archivesInfo = new Tools.IdentityModels.GetArchives().archives;

            var archives = await Db.Queryable<c_archives, c_account, c_archives_level>((ar, ac, al) => new object[] { JoinType.Left, ar.id == ac.archives_id, JoinType.Left, ar.grade_id == al.id }).Where((ar, ac, al) => ar.id == archivesInfo.id).Select((ar, ac, al) => new { ac.balance, ac.coupon, ac.integral, ar.id, ar.name, ar.phone, ar.image_url, level = al.name, ar.id_card, ar.address, ar.sex }).WithCache().FirstAsync();
            var new_archives = new { image_url = Utils.GetImage_url(archives.image_url, archives.sex), archives.balance, archives.coupon, archives.integral, archives.id, archives.name, archives.phone, archives.level, archives.id_card, archives.address, archives.sex };
            return new_archives;
        }

        /// <summary>
        /// 获取门店总充值业绩排行
        /// </summary>
        /// <param name="store_id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<dynamic> GetTotalRechargeOrderAsync(int store_id, short type)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            //获取所有充值
            var recharge = Db.Queryable<r_recharge, p_employee, p_employee_profile, p_employee_nature>((r, e, ep, en) => new object[] { JoinType.Left, r.creater_id == e.id, JoinType.Left, e.id == ep.id, JoinType.Left, r.creater_id == en.employee_id }).Where((r, e, ep, en) => r.categroy_id == 1 && r.org_id == userInfo.org_id && r.store_id == store_id && (en.nature_id == 4 || en.nature_id == 9));
            if (type == 1)//年
            {
                recharge.Where((r, e, ep, en) => r.recharge_date.Year == DateTime.Now.Year);
            }
            else if (type == 2)//月
            {
                recharge.Where((r, e, ep, en) => r.recharge_date.Year == DateTime.Now.Year && r.recharge_date.Month == DateTime.Now.Month);
            }
            else if (type == 3)//日
            {
                recharge.Where((r, e, ep, en) => r.recharge_date.Year == DateTime.Now.Year && r.recharge_date.Month == DateTime.Now.Month && r.recharge_date.Day == DateTime.Now.Day);
            }
            //获取所有充值记录列表
            var recharge_list = await recharge.GroupBy((r, e, ep, en) => new { r.creater, e.phone_no, e.image_url, r.creater_id, ep.sex_name }).OrderBy((r, e, ep, en) => SqlFunc.AggregateSum(r.recharge_money), OrderByType.Desc).Select((r, e, ep, en) => new { r.creater, e.phone_no, e.image_url, r.creater_id, ep.sex_name, sum = SqlFunc.AggregateSum(r.recharge_money) }).Take(10).WithCache().ToListAsync();
            if (recharge_list.Count < 10)//充值记录列表总条数小于10
            {
                var ids = recharge_list.Select(s => s.creater_id).ToList();//已存在用户
                //查询存在用户之外的用户
                var add_list = await Db.Queryable<p_employee, p_employee_profile, p_employee_nature>((e, ep, en) => new object[] { JoinType.Left, e.id == ep.id, JoinType.Left, e.id == en.employee_id }).Where((e, ep, en) => !ids.Contains(e.id) && (en.nature_id == 4 || en.nature_id == 9)).GroupBy((e, ep, en) => new { e.id, e.image_url, e.name, e.phone_no, ep.sex_name }).Select((e, ep, en) => new { e.id, e.image_url, e.name, e.phone_no, ep.sex_name }).Take(10 - recharge_list.Count).ToListAsync();
                //合并处理
                add_list.ForEach(c =>
                {
                    decimal sum = 0;
                    recharge_list.Add(new { creater = c.name, c.phone_no, c.image_url, creater_id = c.id, c.sex_name, sum });
                });
            }
            //获得排行信息
            var list = recharge_list.Select((s, index) => new { s.creater, s.phone_no, s.sum, image_url = Utils.GetImage_url(s.image_url, s.sex_name), index = index + 1 }).ToList();
            return list;
        }


        /// <summary>
        /// 获取充值记录（客户端）
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> GetRechargeListAsync()
        {
            // 获取用户信息
            var user = new Tools.IdentityModels.GetArchives().archives;
            return await Db.Queryable<r_recharge>().Where(w => w.archives_id == user.id && w.categroy_id == 1).Select(s => new { s.bill_no, s.recharge_money, s.give_money, s.way, s.creater, s.recharge_date }).WithCache().ToListAsync();
        }

        /// <summary>
        /// 消费记录分页查询（客户端）
        /// </summary>
        /// <returns></returns>
        public async Task<Page<Spend>> GetSpendPagesAsync(Search entity)
        {
            // 获取用户信息
            var user = new Tools.IdentityModels.GetArchives().archives;
            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            return await Db.Queryable<r_recharge, p_store, f_balance>((r, s, b) => new object[] { JoinType.Left, r.store_id == s.id, JoinType.Left, r.check_id == b.balanceid })
                    .Where((r, s, b) => r.archives_id == user.id && r.categroy_id == 2)
                    .Select((r, s, b) => new Spend { date = r.recharge_date, money = r.recharge_money, store = s.name, balanceid = r.check_id, is_register = b.is_register, balance = r.balance })
                    .OrderBy(entity.order + orderTypeStr)
                    .WithCache()
                    .ToPageAsync(entity.page, entity.limit);
        }

        /// <summary>
        /// 获取优惠券金额
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="no"></param>
        /// <returns></returns>
        public async Task<object> GetCouponMoney(int storeId, string no)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            if (storeId <= 0)
            {

                return new { ret = false, message = "未获取到门店！" };
            }

            //查询优惠券是否存在
            var now = DateTime.Now.ToString("yyyy-MM-dd");

            var coupon = await Db.Queryable<c_new_coupon_detials, c_activity>((cd, a) => new object[] { JoinType.Left, cd.activity_id == a.id })
                               .Where((cd, a) => cd.org_id == userInfo.org_id && a.state == 1 && cd.no == no)
                               .Where(" @now BETWEEN to_char( a.start_date, 'yyyy-MM-dd') AND to_char(a.end_date, 'yyyy-MM-dd') ", new { now })
                               .Select((cd, a) => new { cd.use_state, cd.activity_id, cd.money, cd.no, a.start_date, a.end_date, cd.id })
                               .WithCache()
                               .FirstAsync();
            if (coupon == null)
            {
                return new { ret = false, message = "无此优惠券或未在活动时间范围！" };
            }
            if (coupon.use_state == 2)
            {
                return new { ret = false, message = "此优惠券已使用！" };
            }
            if (string.IsNullOrEmpty(coupon.no))
            {
                return new { ret = false, message = "优惠券券码无效！" };
            }
            if (DateTime.Now.Date < coupon.start_date)
            {
                return new { ret = false, message = "活动未开始！" };
            }
            if (DateTime.Now.Date > coupon.end_date)
            {
                return new { ret = false, message = "活动已结束！" };
            }

            //查询是否限制门店使用
            var isLimit = await Db.Queryable<c_activity_store>()
                                  .Where(w => w.activity_id == coupon.activity_id)
                                  .WithCache()
                                  .ToListAsync();

            if (isLimit.Count > 0)
            {
                var isExcist = isLimit.Select(w => w.store_id == storeId).ToList();
                if (isExcist.Count <= 0)
                {
                    throw new MessageException("此门店不能使用此优惠券！");
                }

            }

            return new { ret = true, money = coupon.money, activity_id = coupon.activity_id, coupon_id = coupon.id, coupon_no = coupon.no };

        }

        /// <summary>
        /// 消费记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Page<ConsumerList>> GetConsumerPagesAsync(ConsumerSearch entity)
        {
            // 获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            return await Db.Queryable<r_recharge, p_store, f_balance>((r, s, b) => new object[] { JoinType.Left, r.store_id == s.id, JoinType.Left, r.check_id == b.balanceid })
                    .Where((r, s, b) => r.org_id==userInfo.org_id && r.categroy_id == 2&&r.state_id==20)
                    .WhereIF(entity.store_id>0, (r,s,b)=>r.store_id==entity.store_id)
                    .WhereIF(entity.select_store_id > 0, (r,s,b)=>r.store_id==entity.select_store_id)
                    .WhereIF(!string.IsNullOrEmpty(entity.name),(r,s,b)=>r.archives.Contains(entity.name)||r.archives_phone.Contains(entity.name)||r.creater.Contains(entity.name))
                    .WhereIF(entity.start_time != null, (r, s, b) => r.recharge_date >= entity.start_time)
                    .WhereIF(entity.end_time != null, (r, s, b) => r.recharge_date <=entity.end_time)
                    .WhereIF(!string.IsNullOrEmpty(entity.way_code),(r,s,b)=>r.way_code==entity.way_code)
                    .Select((r, s, b) => new ConsumerList {store_name=s.name, archives=r.archives, archives_id=r.archives_id, archives_phone=r.archives_phone, balance=r.balance, bill_no=r.bill_no, card_no=r.card_no, check_id=r.check_id, consultation_id=r.consultation_id, coupon_no=r.coupon_no, creater=r.creater, creater_id=r.creater_id, discount_rate=r.discount_rate, level=r.level, recharge_date=r.recharge_date, recharge_money=r.recharge_money, remark=r.remark, state=r.state, state_id=r.state_id, store_id=r.store_id, to_director=r.to_director, to_director_id=r.to_director_id, way=r.way, way_code=r.way_code})
                    .OrderBy(entity.order + orderTypeStr)
                    .WithCache()
                    .ToPageAsync(entity.page, entity.limit);

        }

        /// <summary>
        /// 获取消费详情
        /// </summary>
        /// <param name="balance_id"></param>
        /// <returns></returns>
        public async Task<List<f_balancedetail>> GetConsumerDetail(int balance_id)
        {
            return await Db.Queryable<f_balancedetail>()
                           .Where(s => s.balanceid == balance_id)
                           .WithCache()
                           .ToListAsync();
        }
    }
}
