using Models.DB;
using Models.View.Reports;
using Service.Repository.Interfaces.Reports;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;
using Tools.Filter;
using static Tools.IdentityModels.GetUser;

namespace Service.Repository.Implements.Repotrs
{
    /// <summary>
    /// 主页报表业务
    /// </summary>
    public class MainPageService : DbContext, IMainPageService
    {
        private UserInfo userInfo = new Tools.IdentityModels.GetUser().userInfo;
        /// <summary>
        /// 获取主页报表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<MainPageModel> GetMainPage(MainPageSearch entity)
        {
            if (userInfo.org_id <= 0)
            {
                return null;
            }
            //获取秒数
            int seconds = GetSeconds();

            var emList = await Db.Queryable<p_employee>().WithCache().ToListAsync();
            //如果是超管,可以查看所有
            var isSuperManager = emList.Where(s => s.id == userInfo.id && s.org_id == userInfo.org_id).Select(s => s.is_admin).First();
            //如果是门店管理员，可以查看门店所有
            var isStroeManager = await Db.Queryable<p_employee_role, p_employee>((er, e) => new object[] { JoinType.Left, er.employee_id == e.id })
                                 .Where((er, e) => er.store_id == entity.store_id && er.org_id == userInfo.org_id && entity.store_id > 0 && er.role_id == entity.role_id)
                                 .Select((er, e) => er.is_admin)
                                 .WithCache().FirstAsync();

            if (isSuperManager || entity.nature_id > 0 || isStroeManager)
            {
                //人员性质
                var arr = new int[] { 1, 2, 3, 4, 10 };
                //查询当日客户增长数
                var now = DateTime.Now.ToString("yyyy-MM-dd");
                var archivesCount = await Db.Queryable<c_archives>()
                    .Where(s => s.org_id == userInfo.org_id)
                    .WhereIF(isStroeManager == true || entity.nature_id == 9, s => s.store_id == entity.store_id)//门店管理员可以查看门店的//门店销售经理可见
                    .WhereIF(!isStroeManager && entity.nature_id == 4, s => s.to_director_id == userInfo.id)//如果是销售
                    .Where("to_char(create_date,'yyyy-MM-dd')=@now", new { now })
                    .Select(s => s.id)
                    .WithCache(seconds).CountAsync();
                //查询当日充值总金额
                // var rechargeList = await Db.Queryable<r_recharge>().WithCache().ToListAsync();
                Decimal? recharge_money = 0;
                //查询当日赠送总金额
                Decimal? give_total_money = 0;
                //查询当日销售成单数
                var orderCount = 0;
                //查询当日挂号数
                var his_registerCount = 0;
                //查询当日签到
                var signCount = 0;

                //如果是销售
                if (entity.nature_id == 4 && !isStroeManager)
                {
                    //查询关联表
                    var reList = await Db.Queryable<r_recharge, c_archives>((r, a) => new object[] { JoinType.Left, r.archives_id == a.id })
                                 .Where((r, a) => r.org_id == userInfo.org_id && r.categroy_id == 1 && r.state_id == 6 && a.to_director_id == userInfo.id && r.store_id == entity.store_id)
                                  .Where(" to_char(r.occurrence_date, 'yyyy-MM-dd')=@now", new { now })
                                 .WithCache(seconds).ToListAsync();

                    //查询当日充值总金额
                    recharge_money = reList.Sum(s => s.recharge_money);
                    //查询当日赠送总金额
                    give_total_money = reList.Sum(s => s.give_money);
                    //查询当日销售成单数
                    orderCount = reList.Count();

                    //挂号List
                    var ghList = await Db.Queryable<his_register, c_archives>((r, a) => new object[] { JoinType.Left, r.centerid == a.id })
                                 .Where((r, a) => r.orgid == userInfo.org_id && r.canceled == 3 && a.to_director_id == userInfo.id && r.store_id == entity.store_id)
                                  .Where(" to_char(r.regdate, 'yyyy-MM-dd')=@now", new { now })
                                  .WithCache(seconds).ToListAsync();
                    //查询当日挂号数
                    his_registerCount = ghList.Where(s => s.stateid == 11).Count();
                    //查询当日签到数
                    signCount = ghList.Where(s => 13 <= s.clinicstateid && s.clinicstateid <= 15 && s.stateid == 11).Count();


                }
                //如果是销售经理或超管
                else if (isSuperManager || isStroeManager || entity.nature_id == 9)
                {
                    var list = await Db.Queryable<r_recharge>()
                    .Where(s => s.org_id == userInfo.org_id && s.categroy_id == 1 && s.state_id == 6)
                    .Where(" to_char(occurrence_date, 'yyyy-MM-dd')=@now", new { now })
                    .WhereIF(isStroeManager == true || entity.nature_id == 9, s => s.store_id == entity.store_id)//门店管理员可以查看门店的//门店销售经理可见
                    .WithCache(seconds).ToListAsync();

                    //查询当日充值总金额
                    recharge_money = list.Sum(s => s.recharge_money);
                    //查询当日赠送总金额
                    give_total_money = list.Sum(s => s.give_money);
                    //查询当日销售成单数
                    orderCount = list.Count();
                    //挂号List
                    var ghList = await Db.Queryable<his_register>()
                         .Where(s => s.orgid == userInfo.org_id && s.canceled == 3)
                         .Where(" to_char(regdate , 'yyyy-MM-dd')=@now", new { now })
                         .WhereIF(isStroeManager == true || entity.nature_id == 9, s => s.store_id == entity.store_id)//门店管理员可以查看门店的//门店销售经理可见
                         .WithCache(seconds).ToListAsync();
                    //查询当日挂号数
                    his_registerCount = ghList.Where(s => s.stateid == 11).Count();
                    //查询当日签到数
                    signCount = ghList.Where(s => 13 <= s.clinicstateid && s.clinicstateid <= 15 && s.stateid == 11).Count();
                }
                else
                {
                    return null;
                }

                //查询门店销售比
                var storeList = new List<storeList>();
                if (isSuperManager)
                {
                    storeList = await Db.Queryable<p_store, r_recharge>((s, r) => new object[] { JoinType.Left, s.id == r.store_id })
                              .Where((s, r) => s.org_id == userInfo.org_id)
                              .GroupBy((s, r) => s.id)
                              .OrderBy((s, r) => s.id)
                              .Select((s, r) => new storeList { name = s.name, sale_money = SqlFunc.AggregateSum(r.recharge_money) })
                              .WithCache(seconds).ToListAsync();
                }

                return new MainPageModel
                {
                    archivesCount = archivesCount,
                    recharge_money = recharge_money,
                    give_total_money = give_total_money,
                    his_registerCount = his_registerCount,
                    orderCount = orderCount,
                    storeList = storeList,
                    sign_inCount = signCount
                };
            }

            return null;
        }

        /// <summary>
        /// 获取整体情况
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<totalList> GetTotal(MainPageSearch entity)
        {
            if (userInfo.org_id <= 0)
            {
                return null;
            }

            var emList = await Db.Queryable<p_employee>().WithCache().ToListAsync();
            //如果是超管,可以查看所有
            var isSuperManager = emList.Where(s => s.id == userInfo.id && s.org_id == userInfo.org_id).Select(s => s.is_admin).First();
            //如果是门店管理员，可以查看门店所有
            var isStroeManager = await Db.Queryable<p_employee_role, p_employee>((er, e) => new object[] { JoinType.Left, er.employee_id == e.id })
                                 .Where((er, e) => er.store_id == entity.store_id && er.org_id == userInfo.org_id && entity.store_id > 0 && er.role_id == entity.role_id)
                                 .Select((er, e) => er.is_admin)
                                 .WithCache().FirstAsync();

            if (isSuperManager || entity.nature_id > 0 || isStroeManager)
            {
                //客户List
                var archivesList = new List<archivesList>();

                var archivesCounts = 0;

                //成单数量List
                var orderList = new List<orderList>();
                //挂号数量List
                var his_registerList = new List<his_registerList>();
                //签到数量
                var signList = new List<signList>();

                //获取下拉年月
                var yearList = await GetYear(isSuperManager, entity.store_id);

                //获取客户List
                var arList = await Db.Queryable<c_archives>()
                     .Where(s => SqlFunc.DateValue(s.create_date.Value, DateType.Year) == entity.year && s.org_id == userInfo.org_id)
                     .WhereIF(entity.store_id > 0, s => s.store_id == entity.store_id)
                     .WhereIF(entity.nature_id == 4 && !isStroeManager, s => s.to_director_id == userInfo.id)//如果是销售
                     .WhereIF(isSuperManager == true && entity.storeId > 0, s => s.store_id == entity.storeId)//超管选择下拉门店
                     .WithCache().ToListAsync();

                //获取挂号List
                var registerList = await Db.Queryable<his_register, c_archives>((r, a) => new object[] { JoinType.Left, r.centerid == a.id })
                                 .Where((r, a) => SqlFunc.DateValue(r.regdate, DateType.Year) == entity.year && r.orgid == userInfo.org_id && a.to_director_id == userInfo.id && r.store_id == entity.store_id && r.canceled == 3)
                                  .WithCache().ToListAsync();

                //获取挂号List单表
                var registerListOnly = await Db.Queryable<his_register>()
                                 .Where(s => SqlFunc.DateValue(s.regdate, DateType.Year) == entity.year && s.orgid == userInfo.org_id && s.canceled == 3)
                                 .WhereIF(entity.store_id > 0, s => s.store_id == entity.store_id)
                                 .WhereIF(entity.storeId > 0, s => s.store_id == entity.storeId)//超管选择下拉门店
                                 .WithCache().ToListAsync();

                if (entity.year > 0)
                {

                    if (entity.month > 0)
                    {
                        //查询客户增长
                        archivesList = arList
                        .Where(s => s.create_date.Value.Month == entity.month)
                        .GroupBy(p => p.create_date.Value.Day)
                        .Select(a => new archivesList { archivesCount = a.Count(), month = a.First().create_date.Value.Day })
                        .ToList();

                        //客户月总数
                        if (archivesList.Count > 0)
                        {
                            foreach (var item in archivesList)
                            {
                                archivesCounts += item.archivesCount;
                            }
                        }

                    }
                    else
                    {
                        //查询客户增长
                        archivesList = arList
                            .GroupBy(p => p.create_date.Value.Month)
                            .Select(a => new archivesList { archivesCount = a.Count(), month = a.First().create_date.Value.Month })
                            .ToList();

                        if (archivesList.Count > 0)
                        {
                            foreach (var item in archivesList)
                            {
                                archivesCounts += item.archivesCount;
                            }
                        }
                    }
                }

                //如果是销售
                if (entity.nature_id == 4 && !isStroeManager)
                {
                    if (entity.year > 0)
                    {
                        //按月
                        if (entity.month > 0)
                        {

                            //查询成单数量
                            orderList = await Db.Queryable<r_recharge, c_archives>((r, a) => new object[] { JoinType.Left, r.archives_id == a.id })
                            .Where((r, a) => SqlFunc.DateValue(r.occurrence_date, DateType.Year) == entity.year && SqlFunc.DateValue(r.occurrence_date, DateType.Month) == entity.month && r.org_id == userInfo.org_id && r.categroy_id == 1 && r.state_id == 6 && a.to_director_id == userInfo.id && r.store_id == entity.store_id)
                            .GroupBy((r, a) => SqlFunc.DateValue(r.occurrence_date, DateType.Day))
                            .OrderBy((r, a) => SqlFunc.DateValue(r.occurrence_date, DateType.Day))
                            .Select((r, a) => new orderList { orderCount = SqlFunc.AggregateCount(r.archives_id), month = SqlFunc.DateValue(r.occurrence_date, DateType.Day) })
                            .WithCache().ToListAsync();

                            //查询挂号数量
                            his_registerList = registerList
                                .Where((r, a) => r.stateid == 11 && r.regdate.Month == entity.month)
                                .GroupBy(p => p.regdate.Day)
                                .Select(a => new his_registerList { his_register = a.Count(), month = a.First().regdate.Day })
                                .ToList();

                            //查询签单数量
                            signList = registerList
                                .Where((r, a) => 13 <= r.clinicstateid && r.clinicstateid <= 15 && r.stateid == 11 && r.regdate.Month == entity.month)
                                .GroupBy(p => p.regdate.Day)
                                .Select(a => new signList { sign = a.Count(), month = a.First().regdate.Day })
                                .ToList();


                        }
                        //按年
                        else
                        {
                            //查询成单数量
                            orderList = await Db.Queryable<r_recharge, c_archives>((r, a) => new object[] { JoinType.Left, r.archives_id == a.id })
                            .Where((r, a) => SqlFunc.DateValue(r.occurrence_date, DateType.Year) == entity.year && r.org_id == userInfo.org_id && r.categroy_id == 1 && r.state_id == 6 && a.to_director_id == userInfo.id && r.store_id == entity.store_id)
                            .GroupBy((r, a) => SqlFunc.DateValue(r.occurrence_date, DateType.Month))
                            .OrderBy((r, a) => SqlFunc.DateValue(r.occurrence_date, DateType.Month))
                            .Select((r, a) => new orderList { orderCount = SqlFunc.AggregateCount(r.archives_id), month = SqlFunc.DateValue(r.occurrence_date, DateType.Month) })
                            .WithCache().ToListAsync();

                            //查询挂号数量
                            his_registerList = registerList
                                .Where((r, a) => r.stateid == 11)
                                .GroupBy(p => p.regdate.Month)
                                .Select(a => new his_registerList { his_register = a.Count(), month = a.First().regdate.Month })
                                .ToList();

                            //查询签到数
                            signList = registerList
                                .Where((r, a) => 13 <= r.clinicstateid && r.clinicstateid <= 15 && r.stateid == 11)
                                .GroupBy(p => p.regdate.Month)
                                .Select(a => new signList { sign = a.Count(), month = a.First().regdate.Month })
                                .ToList();
                        }
                    }

                }

                //如果是销售经理或者超管
                else if (isSuperManager || isStroeManager || entity.nature_id == 9)
                {
                    if (entity.year > 0)
                    {
                        //按月
                        if (entity.month > 0)
                        {

                            //查询成单数量
                            orderList = await Db.Queryable<r_recharge>()
                            .Where(s => SqlFunc.DateValue(s.occurrence_date, DateType.Year) == entity.year && SqlFunc.DateValue(s.occurrence_date, DateType.Month) == entity.month && s.org_id == userInfo.org_id && s.categroy_id == 1 && s.state_id == 6)
                            .WhereIF(entity.store_id > 0, s => s.store_id == entity.store_id)
                            .WhereIF(entity.storeId > 0, s => s.store_id == entity.storeId)//超管选择下拉门店
                            .GroupBy(s => SqlFunc.DateValue(s.occurrence_date, DateType.Day))
                            .OrderBy(s => SqlFunc.DateValue(s.occurrence_date, DateType.Day))
                            .Select(s => new orderList { orderCount = SqlFunc.AggregateCount(s.archives_id), month = SqlFunc.DateValue(s.occurrence_date, DateType.Day) })
                            .WithCache().ToListAsync();

                            //查询挂号数量
                            his_registerList = registerListOnly
                                .Where(s => s.regdate.Month == entity.month && s.stateid == 11)
                                .GroupBy(p => p.regdate.Day)
                                .Select(a => new his_registerList { his_register = a.Count(), month = a.First().regdate.Day })
                                .ToList();

                            //查询签单数
                            signList = registerListOnly
                                .Where(s => s.regdate.Month == entity.month && 13 <= s.clinicstateid && s.clinicstateid <= 15 && s.stateid == 11)
                                .GroupBy(p => p.regdate.Day)
                                .Select(a => new signList { sign = a.Count(), month = a.First().regdate.Day })
                                .ToList();


                        }
                        //按年
                        else
                        {

                            //查询成单数量
                            orderList = await Db.Queryable<r_recharge>()
                            .Where(s => SqlFunc.DateValue(s.occurrence_date, DateType.Year) == entity.year && s.org_id == userInfo.org_id && s.categroy_id == 1 && s.state_id == 6)
                            .WhereIF(entity.store_id > 0, s => s.store_id == entity.store_id)
                            .WhereIF(entity.storeId > 0, s => s.store_id == entity.storeId)//超管选择下拉门店
                            .GroupBy(s => SqlFunc.DateValue(s.occurrence_date, DateType.Month))
                            .OrderBy(s => SqlFunc.DateValue(s.occurrence_date, DateType.Month))
                            .Select(s => new orderList { orderCount = SqlFunc.AggregateCount(s.archives_id), month = SqlFunc.DateValue(s.occurrence_date, DateType.Month) })
                            .WithCache().ToListAsync();

                            //查询挂号数量
                            his_registerList = registerListOnly
                                .Where(s => s.stateid == 11)
                                .GroupBy(p => p.regdate.Month)
                                .Select(a => new his_registerList { his_register = a.Count(), month = a.First().regdate.Month })
                                .ToList();

                            //查询签到数
                            signList = registerListOnly
                               .Where(s => 13 <= s.clinicstateid && s.clinicstateid <= 15 && s.stateid == 11)
                               .GroupBy(p => p.regdate.Month)
                               .Select(a => new signList { sign = a.Count(), month = a.First().regdate.Month })
                               .ToList();


                        }
                    }


                }
                else
                {
                    return null;
                }

                return new totalList
                {
                    archivesList = archivesList,
                    orderList = orderList,
                    his_registerList = his_registerList,
                    yearList = yearList,
                    signList = signList,
                    archivesCounts = archivesCounts
                };
            }
            return null;
        }

        /// <summary>
        /// 获取业绩
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<rechargeList> GetRecharge(MainPageSearch entity)
        {
            if (userInfo.org_id <= 0)
            {
                return null;
            }
            var emList = await Db.Queryable<p_employee>().WithCache().ToListAsync();
            //如果是超管,可以查看所有
            var isSuperManager = emList.Where(s => s.id == userInfo.id && s.org_id == userInfo.org_id).Select(s => s.is_admin).First();
            //如果是门店管理员，可以查看门店所有
            var isStroeManager = await Db.Queryable<p_employee_role, p_employee>((er, e) => new object[] { JoinType.Left, er.employee_id == e.id })
                                 .Where((er, e) => er.store_id == entity.store_id && er.org_id == userInfo.org_id && entity.store_id > 0 && er.role_id == entity.role_id)
                                 .Select((er, e) => er.is_admin)
                                 .WithCache().FirstAsync();

            Decimal? moneyTotal = 0;

            if (isSuperManager || entity.nature_id > 0 || isStroeManager)
            {
                //充值总金额List
                var rechargeMoneyList = new List<rechargeMoneyList>();

                //如果是销售
                if (entity.nature_id == 4 && !isStroeManager)
                {
                    if (entity.year > 0)
                    {
                        //按月
                        if (entity.month > 0)
                        {
                            //查询充值总金额
                            rechargeMoneyList = await Db.Queryable<r_recharge, c_archives>((r, a) => new object[] { JoinType.Left, r.archives_id == a.id })
                                    .Where((r, a) => SqlFunc.DateValue(r.occurrence_date, DateType.Year) == entity.year && SqlFunc.DateValue(r.occurrence_date, DateType.Month) == entity.month && r.org_id == userInfo.org_id && r.categroy_id == 1 && r.state_id == 6 && a.to_director_id == userInfo.id && r.store_id == entity.store_id)
                                    .GroupBy((r, a) => SqlFunc.DateValue(r.occurrence_date, DateType.Day))
                                    .OrderBy((r, a) => SqlFunc.DateValue(r.occurrence_date, DateType.Day))
                                    .Select((r, a) => new rechargeMoneyList { recharge_money = SqlFunc.AggregateSum(r.recharge_money), month = SqlFunc.DateValue(r.occurrence_date, DateType.Day) })
                                    .WithCache().ToListAsync();

                            moneyTotal = rechargeMoneyList.Sum(s => s.recharge_money);
                        }
                        //按年
                        else
                        {
                            //查询充值总金额
                            rechargeMoneyList = rechargeMoneyList = await Db.Queryable<r_recharge, c_archives>((r, a) => new object[] { JoinType.Left, r.archives_id == a.id })
                            .Where((r, a) => SqlFunc.DateValue(r.occurrence_date, DateType.Year) == entity.year && r.org_id == userInfo.org_id && r.categroy_id == 1 && r.state_id == 6 && a.to_director_id == userInfo.id && r.store_id == entity.store_id)
                            .GroupBy((r, a) => SqlFunc.DateValue(r.occurrence_date, DateType.Month))
                            .OrderBy((r, a) => SqlFunc.DateValue(r.occurrence_date, DateType.Month))
                            .Select((r, a) => new rechargeMoneyList { recharge_money = SqlFunc.AggregateSum(r.recharge_money), month = SqlFunc.DateValue(r.occurrence_date, DateType.Month) })
                            .WithCache().ToListAsync();

                            moneyTotal = rechargeMoneyList.Sum(s => s.recharge_money);
                        }
                    }


                }
                //如果是销售经理或超管
                else if (isSuperManager || isStroeManager || entity.nature_id == 9)
                {
                    if (entity.year > 0)
                    {
                        //按月
                        if (entity.month > 0)
                        {
                            //查询充值总金额
                            rechargeMoneyList = await Db.Queryable<r_recharge>()
                                .Where(s => SqlFunc.DateValue(s.occurrence_date, DateType.Year) == entity.year && SqlFunc.DateValue(s.occurrence_date, DateType.Month) == entity.month && s.org_id == userInfo.org_id && s.categroy_id == 1 && s.state_id == 6)
                                .WhereIF(entity.store_id > 0, s => s.store_id == entity.store_id)
                                .WhereIF(entity.storeId > 0, s => s.store_id == entity.storeId)//超管选择下拉门店
                                .GroupBy(s => SqlFunc.DateValue(s.occurrence_date, DateType.Day))
                                .OrderBy(s => SqlFunc.DateValue(s.occurrence_date, DateType.Day))
                                .Select(s => new rechargeMoneyList { recharge_money = SqlFunc.AggregateSum(s.recharge_money), month = SqlFunc.DateValue(s.occurrence_date, DateType.Day) })
                                .WithCache().ToListAsync();

                            moneyTotal = rechargeMoneyList.Sum(s => s.recharge_money);
                        }
                        //按年
                        else
                        {
                            //查询充值总金额
                            rechargeMoneyList = await Db.Queryable<r_recharge>()
                            .Where(s => SqlFunc.DateValue(s.occurrence_date, DateType.Year) == entity.year && s.org_id == userInfo.org_id && s.categroy_id == 1 && s.state_id == 6)
                            .WhereIF(entity.store_id > 0, s => s.store_id == entity.store_id)
                            .WhereIF(entity.storeId > 0, s => s.store_id == entity.storeId)//超管选择下拉门店
                            .GroupBy(s => SqlFunc.DateValue(s.occurrence_date, DateType.Month))
                            .OrderBy(s => SqlFunc.DateValue(s.occurrence_date, DateType.Month))
                            .Select(s => new rechargeMoneyList { recharge_money = SqlFunc.AggregateSum(s.recharge_money), month = SqlFunc.DateValue(s.occurrence_date, DateType.Month) })
                            .WithCache().ToListAsync();

                            moneyTotal = rechargeMoneyList.Sum(s => s.recharge_money);
                        }
                    }

                }
                else
                {
                    return null;
                }
                return new rechargeList { rechargeMoneyList = rechargeMoneyList, moneyTotal = moneyTotal };
            }

            return null;
        }


        /// <summary>
        /// 获取下拉年月
        /// </summary>
        /// <param name="store_id"></param>
        /// <returns></returns>
        public async Task<List<yearList>> GetYear(bool isSuperManager, int store_id)
        {
            //年
            var yearList = new List<yearList>();
            var year = 0;

            //注册年份,月份
            var user = await Db.Queryable<p_employee_role, p_employee>((er, e) => new object[] { JoinType.Left, er.employee_id == e.id })
                          .Where((er, e) => e.id == userInfo.id && e.org_id == userInfo.org_id)
                          .WhereIF(isSuperManager == false && store_id > 0, (er, e) => er.store_id == store_id)
                          .Select((er, e) => new { year = SqlFunc.DateValue(e.create_time, DateType.Year), month = SqlFunc.DateValue(e.create_time, DateType.Month) })
                          .WithCache().FirstAsync();

            if (user == null)
            {
                throw new MessageException("未获取到年月！");
            }


            var thisYear = DateTime.Now.Year;//当前年份
            var startMonth = new DateTime(thisYear, 1, 1).Month;//获取当前年的起始月
            var zcMonth = new DateTime(user.year, 12, 31).Month;//获取注册年的终止月
            var Section = 0;//获得当前年份至想获取年份差
            var months = new List<int>();
            //注册年份小于当前年份
            if (thisYear != user.year && thisYear > user.year)
            {
                Section = thisYear - user.year;
                //循环获取年份
                for (var i = 0; i <= Section; i++)
                {
                    year = thisYear--;

                    //如果是当前年
                    if (year == DateTime.Now.Year)
                    {
                        var thisMonth = DateTime.Now.Month;//当前月份
                        months = new List<int>();
                        var xc = thisMonth - startMonth;
                        //循环月份
                        for (var j = 0; j <= xc; j++)
                        {
                            months.Add(thisMonth--);

                        }
                    }//如果是注册年
                    else if (year == user.year)
                    {
                       // var thisMonth = DateTime.Now.Month;//当前月份
                        months = new List<int>();
                        var xc = zcMonth - user.month;
                        //循环月份
                        for (var j = 0; j <= xc; j++)
                        {
                            months.Add(zcMonth++);
                        }
                    }
                    else
                    {
                        int sz = 1;
                        months = new List<int>();
                        //循环月份
                        for (var j = 0; j < 12; j++)
                        {
                            months.Add(sz++);
                        }
                    }

                    yearList.Add(new yearList { year = year, months = months });
                } // 遍历并添加年份到数组

            }
            //如果是注册年份=当前年份
            if (thisYear == user.year)
            {
                var thisMonth = DateTime.Now.Month;//当前月份
                months = new List<int>();
                year = thisYear;
                //月份
                if (thisMonth > startMonth)
                {
                    //相差月份
                    var xc = thisMonth - startMonth;
                    //循环月份
                    for (var j = 0; j <= xc; j++)
                    {
                        months.Add(thisMonth--);

                    }
                }

                yearList.Add(new yearList { year = year, months = months });
            }

            return yearList;
        }

        /// <summary>
        /// 获取明天之前的秒数
        /// </summary>
        /// <returns></returns>
        public int GetSeconds()
            {
            //当前时间
            var nowTime = DateTime.Now;
            //第二天的0点00分00秒
            var tomorrowTime = DateTime.Now.AddDays(1).Date;

            //把2个时间转成TimeSpan,方便计算
            TimeSpan ts1 = new TimeSpan(nowTime.Ticks);
            TimeSpan ts2 = new TimeSpan(tomorrowTime.Ticks);
            //时间比较，得出差值
            var ts =Convert.ToInt32 (ts2.Subtract(ts1).Ticks / 10000000);
            return ts;
        }

    }
}
