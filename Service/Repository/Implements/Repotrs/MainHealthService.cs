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
    /// 医疗板块报表
    /// </summary>
    public class MainHealthService:DbContext,IMainHealthService
    {
        private UserInfo userInfo = new Tools.IdentityModels.GetUser().userInfo;

        /// <summary>
        /// 获取医疗板块信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<MainHealthModel> GetHealthAsync(DoctorTopSearch entity)
        {
            if (userInfo.org_id<=0)
            {
                return null;
            }

            //获取秒数
            int seconds = GetSeconds();

            var emList = await Db.Queryable<p_employee>().WithCache().ToListAsync();
            //如果是超管,可以查看所有医生
            var isSuperManager = emList.Where(s => s.id == userInfo.id && s.org_id == userInfo.org_id).Select(s => s.is_admin).First();

            //如果是门店管理员，可以查看门店所有
            var isStroeManager = await Db.Queryable<p_employee_role, p_employee>((er, e) => new object[] { JoinType.Left, er.employee_id == e.id })
                                 .Where((er, e) => er.store_id == entity.store_id && er.org_id == userInfo.org_id && entity.store_id > 0 && er.role_id == entity.role_id)
                                 .Select((er, e) => er.is_admin)
                                 .WithCache().FirstAsync();

            if (isSuperManager || entity.nature_id ==1 || isStroeManager)
            {
                //部门性质
                var arr = new int[] { 3, 4, 5,6 };
                //总挂号金额
                Decimal? register_money = 0;
                //查询当日挂号数
                var his_registerCount = 0;
                //查询当日签到
                var signCount = 0;
                //当值医生人数
                var doctorDutyCount = 0;

                //查询当天挂号记录表
                var now = DateTime.Now.ToString("yyyy-MM-dd");
                var registerRecordList = await Db.Queryable<his_register>()
                                             .Where(s => s.orgid == userInfo.org_id && s.canceled == 3 && s.stateid == 11 )
                                             .Where(" to_char(regdate, 'yyyy-MM-dd')=@now", new { now })
                                             .WithCache(seconds).ToListAsync();

                var Week = Utils.GetWeek(DateTime.Now);
                var days = Week.WeekNo;
                //查询当值医生人数
                var doctorListName = await Db.Queryable<his_empschedule, p_employee_nature>((e, en) => new object[] { JoinType.Left, en.employee_id == e.empid })
                                .Where((e, en) => e.orgid == userInfo.org_id && en.nature_id == 1)
                                .WhereIF(entity.store_id>0, (e, en) => e.store_id == entity.store_id)
                                .WhereIF(days == 0, (e, en) => e.sunday != "")
                                .WhereIF(days == 1, (e, en) => e.monday != "")
                                .WhereIF(days == 2, (e, en) => e.tuesday != "")
                                .WhereIF(days == 3, (e, en) => e.wednesday != "")
                                .WhereIF(days == 4, (e, en) => e.thursday != "")
                                .WhereIF(days == 5, (e, en) => e.friday != "")
                                .WhereIF(days == 6, (e, en) => e.saturday != "")
                                .Where(" @now BETWEEN to_char(e.orderbegin, 'yyyy-MM-dd') AND to_char(e.orderend, 'yyyy-MM-dd') ", new { now })
                                .Select((e, en) => new { e.empid, e.orgid, e.store_id, e.deptid, e.deptname, e.empname })
                                .GroupBy(e => new { e.empid, e.orgid, e.store_id, e.deptid, e.deptname, e.empname })
                                .WithCache(seconds).ToListAsync();

                var doctor_name_list = doctorListName.Select(dnl => dnl.empname).ToList();

                doctorDutyCount = doctorListName.Count();
                //如果是医生
                if (entity.nature_id == 1 && !isStroeManager)
                {
                    //查询当日挂号总金额
                    register_money = registerRecordList
                                    .Where(s=>s.doctorid==userInfo.id&& s.store_id == entity.store_id &&s.deptid==entity.deptId)
                                    .Sum(s => s.shouldamount);
                    //查询当日挂号数
                    his_registerCount = registerRecordList
                                        .Where(s => s.doctorid == userInfo.id&& s.store_id == entity.store_id  && s.deptid == entity.deptId)
                                        .Count();
                    //查询当日签到数
                    signCount = registerRecordList
                               .Where(s => s.doctorid == userInfo.id&& 13 <= s.clinicstateid && s.clinicstateid <= 15&& s.store_id == entity.store_id && s.deptid == entity.deptId)
                               .Count();
                }
                //如果是超管或门店管理员
                else if (isSuperManager || isStroeManager)
                {
                     //门店管理员
                    if (isStroeManager)
                    {
                        //查询当日挂号总金额
                        register_money = registerRecordList
                                    .Where(s =>s.store_id==entity.store_id)
                                    .Sum(s => s.shouldamount);

                    }
                    else
                    {
                        //查询当日挂号总金额
                        register_money = registerRecordList
                                    .Sum(s => s.shouldamount);
                    }

                    
                }
                else
                {
                    return null;
                }

                return new MainHealthModel
                {
                    register_money = register_money,
                    his_registerCount = his_registerCount,
                    doctorDutyCount= doctorDutyCount,
                    sign_inCount = signCount,
                     doctor_name= doctor_name_list

                };
            }
            return null;
        }

        /// <summary>
        /// 获取医生排名
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
         public async Task<doctorList> GetTopList(DoctorTopSearch entity)
        {
            if (entity.dept_natureId!=3||entity.deptId<=0||userInfo.org_id<=0)
            {
                return null;
            }

            var emList = await Db.Queryable<p_employee>().WithCache().ToListAsync();
            //如果是超管,选择门店跟部门查看医生挂号排名
            var isSuperManager = emList.Where(s => s.id == userInfo.id && s.org_id == userInfo.org_id).Select(s => s.is_admin).First();
            //如果是门店管理员，选择部门查看医生挂号排名
            var isStroeManager = await Db.Queryable<p_employee_role, p_employee>((er, e) => new object[] { JoinType.Left, er.employee_id == e.id })
                                 .Where((er, e) => er.store_id == entity.store_id && er.org_id == userInfo.org_id && entity.store_id > 0 && er.role_id == entity.role_id)
                                 .Select((er, e) => er.is_admin)
                                 .WithCache().FirstAsync();

            //科室排名List
            var topList = new List<topList>();

            //根据科室创建日返回年月
           var yearList= await GetYear(entity.store_id,entity.storeId,entity.deptId, entity.nature_id,isStroeManager);

            //查询科室创建时间
            var deptTime = await Db.Queryable<p_dept>()
                                 .Where(s => s.id == entity.deptId && s.org_id == userInfo.org_id && s.state == 1)
                                 .Select(s=>s.create_date)
                                 .WithCache().FirstAsync();


            //根据科室年月返回医生
            var drList = await Db.Queryable<p_employee_nature, p_employee_role, p_employee>((en, er, e) => new object[] { JoinType.Left, en.employee_id == er.employee_id, JoinType.Left, er.employee_id == e.id })
                                  .Where((en, er, e) => er.org_id == userInfo.org_id && er.dept_id == entity.deptId && en.nature_id == 1&&e.expire_time>=DateTime.Now)
                                  //.WhereIF(deptTime!=null, (en, er, e)=>e.create_time>=deptTime)
                                  .WhereIF(entity.storeId > 0, (en, er, e) => er.store_id == entity.storeId)
                                  .WhereIF(entity.store_id > 0, (en, er, e) => er.store_id == entity.store_id)
                                  .Select((en, er, e) =>new drList{ doctorName= e.name,doctorId= e.id })
                                  .WithCache().ToListAsync();
                           


            //查看科室挂号排名
            if (entity.nature_id==1|| isSuperManager || isStroeManager)
            {
                if (entity.year>0)
                {
                    //按月
                    if (entity.month>0)
                    {

                        topList = await Db.Queryable<his_register>()
                               .Where(s => s.orgid == userInfo.org_id  && s.canceled == 3 && s.stateid == 11 && entity.dept_natureId == 3 && s.deptid == entity.deptId)
                               .WhereIF(entity.storeId>0,s=>s.store_id==entity.storeId)
                               .WhereIF(entity.store_id>0,s => s.store_id == entity.store_id)
                               .WhereIF(entity.month > 0, s => s.regdate.Month == entity.month)
                               .GroupBy("to_char(regdate, 'MM')")
                               .GroupBy(s => new { s.doctorid, s.doctorname, s.deptid })
                               .Having(s => SqlFunc.AggregateCount(s.regid) > 0)
                               .Select(s => new topList { doctorName = s.doctorname, doctorId = s.doctorid, deptId = s.deptid, registerCount = SqlFunc.AggregateCount(s.regid), value = s.regdate.Month })
                               .ToListAsync();
                    }
                    else
                    {
                        topList = await Db.Queryable<his_register>()
                               .Where(s => s.orgid == userInfo.org_id  && s.canceled == 3 && s.stateid == 11 && entity.dept_natureId == 3 && s.deptid == entity.deptId)
                               .WhereIF(entity.year > 0, s => s.regdate.Year == entity.year)
                               .WhereIF(entity.storeId > 0, s => s.store_id == entity.storeId)
                               .WhereIF(entity.store_id > 0, s => s.store_id == entity.store_id)
                               .GroupBy("to_char(regdate, 'YYYY')")
                               .GroupBy(s => new { s.doctorid, s.doctorname, s.deptid })
                               .Having(s => SqlFunc.AggregateCount(s.doctorid) > 0)
                               .Select(s => new topList { doctorName = s.doctorname, doctorId = s.doctorid, deptId = s.deptid, registerCount = SqlFunc.AggregateCount(s.regid), value = entity.year })
                               .ToListAsync();
                    }
                }

            }
            else
            {
               return  null;
            }
            


            return new doctorList {
                yearModel = yearList,
                drList= drList,
                topList=topList


            };
        }




        /// <summary>
        /// 根据科室获取下拉年月
        /// </summary>
        /// <param name="store_id"></param>
        /// <param name="storeId"></param>
        /// <param name="deptId"></param>
        /// <param name="nature_id"></param>
        /// <returns></returns>
        public async Task<List<yearModel>> GetYear(int store_id, int storeId, int deptId,int nature_id,bool isStroeManager)
        {
            if (deptId<0||(store_id<0&& storeId<0))
            {
                return null;
            }
            //年
            var yearList = new List<yearModel>();
            var year = 0;
            
            dynamic dept ;
            //医生注册年份,月份
            if (nature_id == 1 && !isStroeManager)
            {
                dept= await Db.Queryable<p_employee_role, p_employee>((er, e) => new object[] { JoinType.Left, er.employee_id == e.id })
                          .Where((er, e) => e.id == userInfo.id && e.org_id == userInfo.org_id&&er.store_id==store_id&&er.dept_id==deptId)
                          .Select((er, e) => new { year = SqlFunc.DateValue(e.create_time, DateType.Year), month = SqlFunc.DateValue(e.create_time, DateType.Month) })
                          .WithCache().FirstAsync();

                if (dept==null)
                {
                    throw new MessageException("未获取到年月！");
                }
            }
            //科室创建注册年份,月份
            else
            {
                dept = await Db.Queryable<p_dept>()
                         .Where(s => s.org_id == userInfo.org_id && s.id == deptId && s.state == 1)
                         .WhereIF(storeId > 0, s => s.store_id == storeId)
                         .WhereIF(store_id > 0, s => s.store_id == store_id)
                         .Select(s => new { year = SqlFunc.DateValue(s.create_date, DateType.Year), month = SqlFunc.DateValue(s.create_date, DateType.Month) })
                         .WithCache().FirstAsync();

                if (dept==null)
                {
                    throw new MessageException("未获取到年月！");
                }
            }

            var thisYear = DateTime.Now.Year;//当前年份
            var startMonth = new DateTime(thisYear, 1, 1).Month;//获取当前年的起始月
            var zcMonth = new DateTime(dept.year, 12, 31).Month;//获取注册年的终止月
            var Section = 0;//获得当前年份至想获取年份差
            var months = new List<int>();
            //注册年份小于当前年份
            if (thisYear != dept.year && thisYear > dept.year)
            {
                Section = thisYear - dept.year;
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
                    else if (year == dept.year)
                    {
                        //var thisMonth = DateTime.Now.Month;//当前月份
                        months = new List<int>();
                        var xc = zcMonth - dept.month;
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

                    yearList.Add(new yearModel { year = year, months = months });
                } // 遍历并添加年份到数组

            }
            //如果是注册年份=当前年份
            if (thisYear == dept.year)
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

                yearList.Add(new yearModel { year = year, months = months });
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
            var ts = Convert.ToInt32(ts2.Subtract(ts1).Ticks / 10000000);
            return ts;
        }
    }
}
