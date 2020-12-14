using Models.DB;
using Models.View.His;
using Newtonsoft.Json;
using Service.Repository.Interfaces.His;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tools;
using Tools.Filter;

namespace Service.Repository.Implements.His
{
    /// <summary>
    /// 排班
    /// </summary>
    public class EmpscheduleService : DbContext, IEmpscheduleService
    {
        //获取用户信息
        Tools.IdentityModels.GetUser.UserInfo userInfo = new Tools.IdentityModels.GetUser().userInfo;

        /// <summary>
        /// 获取排班
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="deptid">部门ID</param>
        /// <param name="orderbegin">开始时间</param>
        /// <param name="orderend">结束时间</param>
        /// <returns></returns>
        public async Task<Empschedule> GetAsync(int store_id, int deptid, DateTime orderbegin, DateTime orderend)
        {
            //获取排班信息
            var empschedules = await Db.Queryable<his_empschedule>().Where(w => w.orgid == userInfo.org_id && w.store_id == store_id && w.deptid == deptid && w.orderbegin == orderbegin && w.orderend == orderend).WithCache().ToListAsync();
            if (empschedules.Count == 0)
            {
                throw new MessageException("此时间段选择部门无排班");
            }
            //获取排班ID集合
            var scheduleids = empschedules.Select(s => s.scheduleid).ToList();

            //获取排班ID集合对应的排班时间段信息
            var scheduletimes = await Db.Queryable<his_scheduletimes>().Where(w => scheduleids.Contains(w.scheduleid)).WithCache().ToListAsync();

            //定义返回对象Empschedule
            var empschedule = new Empschedule();

            #region Empschedule基本信息
            empschedule.deptid = deptid;
            empschedule.deptname = empschedules.First().deptname;
            empschedule.orderbegin = orderbegin;
            empschedule.orderend = orderend;
            empschedule.store_id = store_id;

            #region 医生列表doctors
            //定义医生列表doctors
            var doctors = new List<Doctor>();

            foreach (var item in empschedules)
            {
                //定义医生信息doctor
                var doctor = new Doctor { callflag = item.callflag, empid = item.empid, empname = item.empname, friday = item.friday, limitnumbers = item.limitnumbers, limitorders = item.limitorders, monday = item.monday, orderflag = item.orderflag, roomid = item.roomid, roomname = item.roomname, saturday = item.saturday, sunday = item.sunday, thursday = item.thursday, tuesday = item.tuesday, typeid = item.typeid, wednesday = item.wednesday };
                doctor.scheduletimes = scheduletimes.Where(w => w.scheduleid == item.scheduleid).ToList();
                doctors.Add(doctor);
            }

            empschedule.doctors = doctors;
            #endregion

            #endregion

            return empschedule;
        }

        /// <summary>
        /// 获取排班时间段
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="deptid">部门ID</param>
        /// <param name="year">年份</param>
        /// <returns></returns>
        public async Task<List<EmpscheduleDate>> GetDateAsync(int store_id, int deptid, int year)
        {
            return await Db.Queryable<his_empschedule>().Where(w => SqlFunc.DateValue(w.orderbegin, DateType.Year) == year && w.orgid == userInfo.org_id && w.store_id == store_id && w.deptid == deptid).GroupBy(s => new { s.orderbegin, s.orderend }).OrderBy(o => o.orderbegin, OrderByType.Desc).Select(s => new EmpscheduleDate { orderbegin = s.orderbegin, orderend = s.orderend }).WithCache().ToListAsync();
        }

        /// <summary>
        /// 获取存在排班的年份
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="deptid">部门ID</param>
        /// <returns></returns>
        public async Task<List<int>> GetYearAsync(int store_id, int deptid)
        {
            return await Db.Queryable<his_empschedule>().Where(w => w.orgid == userInfo.org_id && w.store_id == store_id && w.deptid == deptid).GroupBy(g => SqlFunc.DateValue(g.orderbegin, DateType.Year)).OrderBy(o => SqlFunc.DateValue(o.orderbegin, DateType.Year), OrderByType.Desc).Select(s => SqlFunc.DateValue(s.orderbegin, DateType.Year)).WithCache().ToListAsync();
        }

        /// <summary>
        /// 设置排班
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<string> Set(Empschedule entity)
        {
            if (entity.orderbegin > entity.orderend)
            {
                throw new MessageException("排班结束时间必须大于或等于开始时间");
            }
            if (entity.doctors.Count == 0)
            {
                throw new MessageException("未设置医生排班信息");
            }
            //获取最大有效挂号或预约日期
            var date_max = await Db.Queryable<his_register>().Where(w => w.orgid == userInfo.org_id && w.store_id == entity.store_id && w.deptid == entity.deptid && w.canceled == 3).OrderBy(o => o.regdate, OrderByType.Desc).WithCache().FirstAsync();
            if (date_max != null)
            {
                if (date_max.regdate.Date >= entity.orderbegin.Date)
                {
                    throw new MessageException($"{date_max.regdate.Date.ToString("yyyy-MM-dd")}存在挂号或预约，开始时间必须大于{date_max.regdate.Date.ToString("yyyy-MM-dd")}");
                }
            }
            //医生列表doctors
            var doctors = entity.doctors;
            foreach (var doctor in doctors)
            {
                if (doctor.limitnumbers < doctor.limitorders)
                {
                    throw new MessageException("限约数不能小于限号数");
                }
                //医生排班时间段
                var scheduletimes = doctor.scheduletimes;

                //星期一排班时间段
                doctor.monday = SetWeek(scheduletimes, 1);

                //星期二排班时间段
                doctor.tuesday = SetWeek(scheduletimes, 2);

                //星期三排班时间段
                doctor.wednesday = SetWeek(scheduletimes, 3);

                //星期四排班时间段
                doctor.thursday = SetWeek(scheduletimes, 4);

                //星期五排班时间段
                doctor.friday = SetWeek(scheduletimes, 5);

                //星期六排班时间段
                doctor.saturday = SetWeek(scheduletimes, 6);

                //星期天排班时间段
                doctor.sunday = SetWeek(scheduletimes, 0);
            }

            var v_org_id = new SugarParameter("v_org_id", userInfo.org_id);
            var v_store_id = new SugarParameter("v_store_id", entity.store_id);
            var v_deptid = new SugarParameter("v_deptid", entity.deptid);
            var v_deptname = new SugarParameter("v_deptname", entity.deptname);
            var v_orderbegin = new SugarParameter("v_orderbegin", entity.orderbegin);
            var v_orderend = new SugarParameter("v_orderend", entity.orderend);
            var v_doctors = new SugarParameter("v_doctors", JsonConvert.SerializeObject(entity.doctors));
            redisCache.RemoveAll<his_empschedule>();
            redisCache.RemoveAll<his_scheduletimes>();
            var result = await Db.Ado.UseStoredProcedure().GetStringAsync("f_his_empschedule", v_org_id, v_store_id, v_deptid, v_deptname, v_orderbegin, v_orderend, v_doctors);
            if (result != "成功")
            {
                throw new MessageException(result);
            }
            return result;
        }

        /// <summary>
        /// 设置Week
        /// </summary>
        /// <param name="scheduletimes">时间段信息</param>
        /// <param name="days">星期（1,2,3,4...）</param>
        /// <returns></returns>
        private static string SetWeek(List<his_scheduletimes> scheduletimes, short days)
        {
            var showTxt = "";
            var list = scheduletimes.Where(w => w.days == days).OrderBy(o => o.begintime).ToList();
            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var scheduletime = list[i];
                    //设置时间段固定属性值
                    scheduletime.replacedocid = 0;
                    scheduletime.replacedocname = "";
                    scheduletime.replaceid = 0;
                    scheduletime.stateid = 5;
                    scheduletime.statename = "正常";
                    scheduletime.no = short.Parse((i + 1).ToString());
                    //生成显示值
                    showTxt += $"{scheduletime.begintime} - {scheduletime.endtime} |";
                }
            }

            return showTxt.TrimEnd('|');
        }
    }
}
