using Models.DB;
using Models.View.Business;
using Models.View.His;
using Models.View.Mobile;
using Newtonsoft.Json;
using Service.Extensions;
using Service.Repository.Interfaces.His;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;
using Tools.Filter;
using Tools.WebSocket;
using static Tools.Utils;

namespace Service.Repository.Implements.His
{
    /// <summary>
    /// 预约挂号
    /// </summary>
    public class RegisterService : DbContext, IRegisterService
    {
        /// <summary>
        /// 预约挂号
        /// </summary>
        /// <returns></returns>
        public async Task<bool> AddAsync(HisRegister entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            if (entity.regdate.Date < DateTime.Now.Date)
            {
                throw new MessageException("请选择正确的挂号日期");
            }

            return await Register(entity, userInfo);
        }

        /// <summary>
        /// 预约挂号（客户端）
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CusAddAsync(CusRegister entity)
        {
            if (entity.regdate.Date < DateTime.Now.Date)
            {
                throw new MessageException("请选择正确的挂号日期");
            }
            //获取客户信息
            var archivesInfo = new Tools.IdentityModels.GetArchives().archives;
            var userInfo = new Tools.IdentityModels.GetUser.UserInfo { id = archivesInfo.id, is_admin = false, name = archivesInfo.name, org_id = archivesInfo.org_id, phone_no = archivesInfo.phone };
            var his_entity = new HisRegister { cardno = archivesInfo.card_no, centerid = archivesInfo.id, days = entity.days, deptid = entity.deptid, deptname = entity.deptname, doctorid = entity.doctorid, doctorname = entity.doctorname, no = entity.no, orderflag = entity.orderflag, regdate = entity.regdate, room = entity.room, roomid = entity.roomid, scheduleid = entity.scheduleid, shouldamount = entity.shouldamount, source = entity.source, sourceid = entity.sourceid, store_id = entity.store_id, summary = entity.summary, typeid = entity.typeid };
            return await Register(his_entity, userInfo);
        }

        /// <summary>
        /// 挂号
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        private async Task<bool> Register(HisRegister entity, Tools.IdentityModels.GetUser.UserInfo userInfo)
        {
            var scheduletimes = Db.Queryable<his_scheduletimes>().Where(w => w.scheduleid == entity.scheduleid && w.days == entity.days && w.no == entity.no).WithCache().First();
            var regdatestr = entity.regdate.ToString($"yyyy-MM-dd {scheduletimes.begintime}");
            //查询档案账户
            var accountMessage = Db.Queryable<c_account, c_archives, c_archives_level>((ac, a, al) => new object[] { JoinType.Left, ac.archives_id == a.id, JoinType.Left, al.id == a.grade_id })
                .Where((ac, a, al) => ac.archives_id == entity.centerid)
                .Select((ac, a, al) => new { account = ac, archives = a, level = al })
                .WithCache().First();
            if (accountMessage == null)
            {
                throw new MessageException("账户余额不足，请先充值");
            }
            //档案账户
            var account = accountMessage.account;
            //档案信息
            var archives = accountMessage.archives;
            //档案等级
            var level = accountMessage.level;

            var register = new his_register();

            var result = Db.Ado.UseTran(() =>
            {
                register = RegisterCommon(entity, userInfo, scheduletimes, regdatestr, account, archives, level);
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            //通知
            var notice = new Business.NoticeService();
            var noticeList = new List<AddNoticeModel>();
            var notice_content = "";
            var employeeSocket = new List<WebSocketModel>();

            if (archives.to_director_id>0)
            {
                //预约挂号通知
                notice_content = $"{{\"name\":\"{archives.name}\",\"date\":\"{regdatestr}\",\"msg\":\"预约挂号成功\",\"doctor\":\"{entity.doctorname}\"}}";

                employeeSocket.Add(new WebSocketModel { userId = archives.to_director_id.Value, content = notice_content });
                var employeenotice = new List<employeeMes>();
                employeenotice.Add(new employeeMes { employee_id = archives.to_director_id.Value, employee = archives.to_director });

                notice.NewMethod(register.regid.ToString(), archives, register.store_id, notice, noticeList, 7, 1, notice_content, archives.name, employeenotice );

                //新增通知
                notice.AddNotice(noticeList);
                //消息提醒
                ChatWebSocketMiddleware.SendListAsync(employeeSocket);

            }

            //发送短信
            var store_org = await Db.Queryable<p_store, p_org>((s, o) => new object[] { JoinType.Left, s.org_id == o.id }).Where((s, o) => s.id == entity.store_id).Select((s, o) => new { store_name = s.name, org_name = o.name }).WithCache().FirstAsync();
            var hospital = $"{store_org.store_name}（{store_org.org_name}）";
            var content = $"您在{hospital}预约挂号成功，预约日期：{regdatestr}，请及时取号就诊，若不能及时取号就诊，请提前取消预约。";
            Dictionary<string, string> values = new Dictionary<string, string>();
            values.Add("hospital", hospital);
            values.Add("time", regdatestr);
            var toValues = JsonConvert.SerializeObject(values);
            var sendSms = new Public.SendSMSService();
            //await sendSms.SendSmsAsync(archives.phone, 6, toValues, content, 1, archives.org_id);
            content = $"{archives.name}在{hospital}预约挂号成功！预约日期：{regdatestr}，医生：{register.doctorname}，诊室：{register.room}。";
            //await sendSms.SendSmsAsync(employee.phone_no, 6, toValues, content, 1, archives.org_id);

            return result.IsSuccess;
        }

        /// <summary>
        /// 挂号
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        private his_register RegisterCommon(HisRegister entity, Tools.IdentityModels.GetUser.UserInfo userInfo, his_scheduletimes scheduletimes, string regdatestr, c_account account, c_archives archives, c_archives_level level)
        {

            //折扣率
            decimal discountRate = 100;
            //实收金额
            decimal actual_amount = 0;
            //优惠券支付
            decimal coupon_money = 0;
            //账户支付
            decimal accountpay = 0;

            //通知
            var notice = new Business.NoticeService();
            var noticeList = new List<AddNoticeModel>();
            var notice_content = "";
            var employeeSocket = new List<WebSocketModel>();

            if ((account.balance + account.coupon) < entity.shouldamount)
            {
                throw new MessageException("账户余额不足，请先充值");
            }


            //获取挂号费信息
            var registerfee = Db.Queryable<his_registertype, his_registertypefee, h_itemspec, b_basecode, h_item>((rt, rtf, i, b, hi) => new object[] { JoinType.Left, rt.typeid == rtf.typeid, JoinType.Left, rtf.specid == i.specid, JoinType.Left, i.incomeid == b.valueid && b.baseid == 66, JoinType.Left, i.itemid == hi.item_id })
            .Where((rt, rtf, i, b, hi) => rt.typeid == entity.typeid)
            .WhereIF(entity.orderflag == 3, (rt, rtf, i, b, hi) => rtf.orderflag == 3)
            .Select((rt, rtf, i, b, hi) => new { shouldamount = i.sale_price, rtf.specid, rtf.quantiry, i.buy_price, i.incomeid, i.itemid, b.value, hi.trade_name, i.salsemodulus, i.specname, i.salseunit })
            .WithCache()
            .ToList();

            //判断金额是否正确
            if (registerfee.Sum(s => s.shouldamount * s.quantiry) != entity.shouldamount)
            {
                throw new MessageException("收费金额不正确");
            }

            //扣除账户余额与赠送余额
            if (account.balance >= entity.shouldamount)
            {

                discountRate = level.discount_rate.Value;
                actual_amount = entity.shouldamount * discountRate / 100;
                account.balance -= entity.shouldamount * discountRate / 100;
                accountpay = entity.shouldamount * discountRate / 100;
            }
            else if (account.coupon >= entity.shouldamount)
            {
                actual_amount = entity.shouldamount;
                account.coupon -= entity.shouldamount;
                coupon_money = entity.shouldamount;
            }
            else
            {

                coupon_money = account.coupon;
                actual_amount = entity.shouldamount;
                account.balance -= entity.shouldamount - account.coupon;
                accountpay = entity.shouldamount - account.coupon;
                account.coupon = 0;
            }

            var Encrypt = account.amount + account.archives_id + account.balance + account.consume + account.coupon + account.integral + account.noneamount + account.password + account.rate + account.recharge + account.salseamount + account.settleamount + account.total_coupon + account.total_integral;
            var code = MetarnetRegex.Encrypt(MetarnetRegex.SHA512Encrypt(Encrypt.ToString()));
            account.code = code;
            Db.Updateable(account).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

            //写入门诊收费结算单
            var balance = new f_balance { couponpay = coupon_money, accountpay = accountpay, actualamount = actual_amount, alipay = 0, bankpay = 0, cashpay = 0, centerid = entity.centerid, checkoutid = 0, clinicid = 0, deptid = entity.deptid, deptname = entity.deptname, doctorid = entity.doctorid, doctorname = entity.doctorname, insuranceid = 0, insurancepay = 0, issettle = 3, orgid = userInfo.org_id, otherpay = 0, recorddate = DateTime.Now, returnid = 0, shouldamount = entity.shouldamount, source = entity.source, sourceid = entity.sourceid, stateid = 19, store_id = entity.store_id, typeid = 3, wechatpay = 0, operatorid = userInfo.id, balancedate = DateTime.Now, operator_name = userInfo.name, summay = "挂号自动结算", is_register = 2 };
            var balance_id = Db.Insertable(balance).ExecuteReturnIdentity();
            redisCache.RemoveAll<f_balance>();

            //写入门诊收费清单
            var balancedetail_list = registerfee.Select((s, index) => new f_balancedetail { actualamount = s.shouldamount * discountRate / 100, shouldamount = s.shouldamount, specid = s.specid, balanceid = balance_id, cost = s.buy_price, execdeptid = entity.deptid, execdeptname = entity.deptname, execstate = "已执行", modulus = s.salsemodulus, execstateid = 17, incomeid = 0, invoicename = s.value, itemname = s.trade_name, numbers = 1, orderid = short.Parse(index.ToString()), price = s.shouldamount, quantity = 1, rebate = 1, receipeid = 0, sigledousage = 1, specname = s.specname, unitname = s.salseunit, usage = "其他", usageid = 9, frequencyid = 0, frequency = "" }).ToList();
            Db.Insertable(balancedetail_list).ExecuteCommand();
            redisCache.RemoveAll<f_balancedetail>();

            var register = new his_register();
            //默认挂号信息
            register.orgid = userInfo.org_id;
            register.store_id = entity.store_id;
            register.deptid = entity.deptid;
            register.deptname = entity.deptname;
            register.doctorid = entity.doctorid;
            register.doctorname = entity.doctorname;
            register.typeid = entity.typeid;
            register.centerid = entity.centerid;
            register.clinicid = 0;
            register.recorddate = DateTime.Now;

            register.regdate = DateTime.Parse(regdatestr);

            register.operatorid = userInfo.id;
            register.operator_name = userInfo.name;
            register.cardno = entity.cardno;
            register.shouldamount = entity.shouldamount;
            register.actualamount = actual_amount;
            register.paymentid = 5;
            register.payment = "账户";
            register.settleflag = 2;
            register.orderflag = entity.orderflag;
            register.roomid = entity.roomid;
            register.room = entity.room;
            register.canceled = 3;
            register.stateid = 11;
            register.checkoutid = balance_id;

            short numbers = 1;
            short orders = 0;
            register.billtypeid = 2;
            register.billtype = "挂号";
            if (register.orderflag == 2)
            {
                register.billtypeid = 1;
                register.billtype = "预约";
                orders = 1;
            }

            register.sourceid = entity.sourceid;
            register.source = entity.source;
            register.clinicstateid = 5;
            register.summary = entity.summary;

            //添加挂号记录
            var regid = Db.Insertable(register).ExecuteReturnIdentity();
            register.regid = regid;
            redisCache.RemoveAll<his_register>();

            //添加挂号费
            var registerfee_list = registerfee.Select(s => new his_registerfee { actualamount = entity.shouldamount, regid = regid, shouldamount = s.shouldamount, specid = s.specid }).ToList();
            Db.Insertable(registerfee_list).ExecuteCommand();
            redisCache.RemoveAll<his_registerfee>();

            //添加门诊号数
            var outpatient_number = Db.Queryable<his_outpatient_number>().Where(w => w.scheduleid == entity.scheduleid && w.days == entity.days && w.no == entity.no && w.register_date == entity.regdate.Date).WithCache().First();
            if (outpatient_number != null)
            {
                outpatient_number.orders += orders;
                outpatient_number.numbers += numbers;
                //查询排班信息
                var empschedule = Db.Queryable<his_empschedule>().Where(w => w.scheduleid == entity.scheduleid).WithCache().First();
                if (outpatient_number.orders > empschedule.limitorders || outpatient_number.numbers > empschedule.limitnumbers)
                {
                    throw new MessageException("当前时间段已挂满，不能进行预约挂号");
                }
                Db.Updateable(outpatient_number).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
            }
            else
            {
                outpatient_number = new his_outpatient_number { days = entity.days, no = entity.no, numbers = numbers, orders = orders, register_date = entity.regdate.Date, scheduleid = entity.scheduleid };
                Db.Insertable(outpatient_number).ExecuteCommand();
                redisCache.RemoveAll<his_outpatient_number>();
            }

            //添加门诊号
            var outpatient_no = new his_outpatient_no { days = entity.days, no = entity.no, order_no = 0, regid = regid, register_date = entity.regdate, scheduleid = entity.scheduleid };
            Db.Insertable(outpatient_no).ExecuteCommand();
            redisCache.RemoveAll<his_outpatient_no>();

            //增加消费记录
            var recharge = new r_recharge();
            recharge.state_id = 20;
            recharge.state = "已消费";
            recharge.archives_id = Convert.ToInt32(archives.id);
            recharge.card_no = archives.card_no;
            recharge.archives = archives.name;
            recharge.archives_phone = archives.phone;
            recharge.recharge_date = DateTime.Now;
            recharge.recharge_money = actual_amount;//实际充值金额
            recharge.recharge_integral = 0;
            recharge.money_integral = level.money_integral;
            recharge.total_money = account.recharge;
            recharge.total_integral = account.total_integral;
            recharge.way_code = "5";
            recharge.way = "账户";
            recharge.bill_no = archives.id + DateTime.Now.ToString("yyyyMMddhhmmssffffff");
            recharge.occurrence_date = DateTime.Now;
            recharge.level = level.name;
            recharge.org_id = userInfo.org_id;
            recharge.store_id = entity.store_id;
            recharge.balance = account.balance;
            recharge.integral = Convert.ToInt32(account.integral);
            recharge.give_total_money = account.total_coupon;
            recharge.give_balance = account.coupon;
            recharge.give_money = coupon_money;//使用的优惠金额
            recharge.creater = userInfo.name;
            recharge.creater_id = userInfo.id;
            recharge.consultation_id = 0;
            recharge.categroy_id = 2;
            recharge.no = archives.id + DateTime.Now.ToString("yyMMddhhmmssffff");
            recharge.to_director_id = archives.to_director_id;
            recharge.to_director = archives.to_director;
            recharge.check_id = balance_id;
            recharge.discount_rate = discountRate;

            Db.Insertable(recharge).ExecuteCommand();
            redisCache.RemoveAll<r_recharge>();

            //查询客户所属分销人员
            var distributor = Db.Queryable<c_distributor, p_distributor>((cd, pd) => new object[] { JoinType.Left, cd.distributor_id == pd.id }).Where((cd, pd) => cd.archives_id == archives.id).Select((cd, pd) => pd).WithCache().First();
            //查询分销人员提成比例
            var royalty_rate = Db.Queryable<p_org>().Where(w => w.id == userInfo.org_id).Select(s => s.royalty_rate).WithCache().First();
            if (distributor != null && royalty_rate > 0)
            {
                var sum = actual_amount * royalty_rate;
                //写入提成记录
                var amount = new r_amount { archives = archives.name, archives_id = archives.id, archives_phone = archives.phone, amount_date = DateTime.Now, balance_id = balance_id, card_no = archives.card_no, distributor = distributor.name, distributor_id = distributor.id, money = sum, org_id = userInfo.org_id, store_id = balance.store_id };
                Db.Insertable(amount).ExecuteCommand();
                redisCache.RemoveAll<r_amount>();
                //修改分销人员账户信息
                Db.Updateable<p_distributor>().SetColumns(s => new p_distributor { amount = s.amount + sum, noneamount = s.noneamount + sum }).Where(w => w.id == distributor.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
            }

            //查询客户余额下限
            var balance_lower = Db.Queryable<c_archives, c_account, c_archives_level>((a, ac, al) => new object[] { JoinType.Left, ac.archives_id == a.id, JoinType.Left, a.grade_id == al.id })
                             .Where((a, ac, al) => a.id == entity.centerid && a.org_id == userInfo.org_id && ac.balance <= al.balance_limit_lower)
                             .Select((a, ac, al) => new { a.name, a.id, a.to_director_id, a.image_url,ac.balance,al.balance_limit_lower })
                             .WithCache()
                             .First();
            if (balance_lower != null&&balance_lower?.to_director_id>0)
            {
                //发送通知
                var con = $"{{\"name\":\"{balance_lower.name}\",\"msg\":\"余额已达下限！\",\"img_url\":\"{balance_lower.image_url}\",\"type\":\"balance\"}}";
                employeeSocket.Add(new WebSocketModel { userId = balance_lower.to_director_id.Value, content = con });

                //调用通知
                notice_content = $"{{\"name\":\"{balance_lower.name}\",\"msg\":\" 余额已达下限！\",\"balance\":\"{balance_lower.balance}\",\"balance_limit_lower\":\"{balance_lower.balance_limit_lower}\"}}";
                var employeenotice = new List<employeeMes>();
                employeenotice.Add(new employeeMes { employee_id = archives.to_director_id.Value, employee = archives.to_director });

                notice.NewMethod(archives.id.ToString(),archives, entity.store_id, notice, noticeList, 8, 3, notice_content, balance_lower.name, employeenotice);
                //新增通知
                notice.AddNotice(noticeList);
                //消息提醒
                ChatWebSocketMiddleware.SendListAsync(employeeSocket);

            }

            return register;
        }

        /// <summary>
        /// 获得分页列表
        /// </summary>
        /// <param name="name_or_phone">姓名或手机号</param>
        /// <param name="store_id">门店ID</param>
        /// <param name="dept_id">科室ID</param>
        /// <param name="doctor_id">医生ID</param>
        /// <param name="type_id">类别ID</param>
        /// <param name="orderflag">预约标志</param>
        /// <param name="stateid">状态ID</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        public async Task<Page<RegisterRecord>> GetPagesAsync(string name_or_phone, int store_id, int dept_id, int doctor_id, int type_id, int orderflag, int stateid, string order, int orderType, int limit, int page)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var orderTypeStr = " Asc";
            if (orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            return await Db.Queryable<his_register, c_archives>((r, a) => new object[] { JoinType.Left, r.centerid == a.id })
                .Where((r, a) => r.orgid == userInfo.org_id)
                .WhereIF(!string.IsNullOrEmpty(name_or_phone), (r, a) => a.name.Contains(name_or_phone) || a.phone.Contains(name_or_phone))
                .WhereIF(store_id > 0, (r, a) => r.store_id == store_id)
                .WhereIF(dept_id > 0, (r, a) => r.deptid == dept_id)
                .WhereIF(type_id > 0, (r, a) => r.typeid == type_id)
                .WhereIF(doctor_id > 0, (r, a) => r.doctorid == doctor_id)
                .WhereIF(orderflag > 0, (r, a) => r.orderflag == orderflag)
                .WhereIF(stateid > 0, (r, a) => r.stateid == stateid)
                .OrderBy(order + orderTypeStr)
                .Select((r, a) => new RegisterRecord { actualamount = r.actualamount, billtype = r.billtype, billtypeid = r.billtypeid, canceled = r.canceled, cardno = r.cardno, centerid = r.centerid, checkoutid = r.checkoutid, clinicid = r.clinicid, clinicstateid = r.clinicstateid, deptid = r.deptid, deptname = r.deptname, doctorid = r.doctorid, doctorname = r.doctorname, name = a.name, phone = a.phone, operatorid = r.operatorid, operator_name = r.operator_name, orderflag = r.orderflag, orgid = r.orgid, payment = r.payment, paymentid = r.paymentid, recorddate = r.recorddate, regdate = r.regdate, regid = r.regid, room = r.room, roomid = r.roomid, settleflag = r.settleflag, shouldamount = r.shouldamount, source = r.source, sourceid = r.sourceid, stateid = r.stateid, store_id = r.store_id, summary = r.summary, typeid = r.typeid })
                .WithCache()
                .ToPageAsync(page, limit);
        }

        #region 查询可挂号或可预约信息
        /// <summary>
        /// 根据科室和日期获取医生排班
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="deptid">科室ID</param>
        /// <param name="date">指定日期</param>
        /// <returns></returns>
        public async Task<List<RegisterDoctor>> GetByDeptAndDateAsync(int store_id, int deptid, DateTime date)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var list = new List<RegisterDoctor>();

            if (date.Date < DateTime.Now.Date)
            {
                throw new MessageException($"请选择当天（{DateTime.Now.Date.ToString("yyyy-MM-dd")}）及以后的日期");
            }
            //获取星期
            var Week = Utils.GetWeek(date);
            var days = Week.WeekNo;
            var this_date = date.ToString("yyyy-MM-dd");
            //获取医生排班
            var empschedule = await Db.Queryable<his_empschedule, his_scheduletimes, his_registertype>((e, s, r) => new object[] { JoinType.Left, e.scheduleid == s.scheduleid, JoinType.Left, e.typeid == r.typeid })
                .Where((e, s, r) => e.orgid == userInfo.org_id && e.store_id == store_id && SqlFunc.Between(date, e.orderbegin, e.orderend) && s.days == days && e.deptid == deptid)
                //.WhereIF(date.Date == DateTime.Now.Date, (e, s, r) => s.endtime > nowDate)
                .WhereIF(date.Date != DateTime.Now.Date, (e, s, r) => e.orderflag == 2)
                .Select((e, s, r) => new EmpscheduleScheduletimes { begintime = s.begintime, callflag = e.callflag, days = s.days, deptid = e.deptid, deptname = e.deptname, empid = e.empid, empname = e.empname, endtime = s.endtime, friday = e.friday, limitnumbers = e.limitnumbers, limitorders = e.limitorders, monday = e.monday, no = s.no, orderbegin = e.orderbegin, orderend = e.orderend, orderflag = e.orderflag, orgid = e.orgid, replacedocid = s.replacedocid, replacedocname = s.replacedocname, replaceid = s.replaceid, roomid = e.roomid, roomname = e.roomname, saturday = e.saturday, scheduleid = e.scheduleid, stateid = s.stateid, statename = s.statename, store_id = e.store_id, sunday = e.sunday, thursday = e.thursday, tuesday = e.tuesday, typeid = e.typeid, wednesday = e.wednesday, typename = r.typename })
                .WithCache()
                .ToListAsync();

            //挂号还是预约
            bool is_order = false;
            var txt = "可挂号";
            if (date.Date != DateTime.Now.Date)
            {
                is_order = true;
                txt = "可预约";
                empschedule = empschedule.Where(w => w.orderflag == 2).ToList();
            }
            else
            {
                var nowDate = DateTime.Now.TimeOfDay;
                empschedule = empschedule.Where(w => w.endtime > nowDate).ToList();
            }

            if (empschedule.Count == 0)
            {
                throw new MessageException($"{date.Date.ToString("yyyy-MM-dd")}无{txt}医生");
            }

            //获取费别费用
            var registertypefee = await Db.Queryable<his_registertypefee, h_itemspec>((r, i) => new object[] { JoinType.Left, r.specid == i.specid }).Select((r, i) => new RegisterTypeFee { typeid = r.typeid, quantiry = r.quantiry, orderflag = r.orderflag, sale_price = i.sale_price }).WithCache().ToListAsync();

            //获取排班ID
            var scheduleids = empschedule.Select(s => s.scheduleid).ToList();
            //门诊号数表
            var outpatient_number = await Db.Queryable<his_outpatient_number>().Where(w => scheduleids.Contains(w.scheduleid)).WithCache().ToListAsync();

            //获取医生排班（正常）
            var empschedule_ok = empschedule.Where(w => w.stateid == 5).ToList();
            //返回正常坐诊医生排班
            var emp_list = empschedule_ok.Select(s => new { s.empid, s.empname }).Distinct().ToList();
            foreach (var item in emp_list)
            {
                //获取医生信息
                var registerDoctor = new RegisterDoctor { empid = item.empid, empname = item.empname, is_order = is_order, Week = Week.WeekTxt };
                //医生坐诊时间信息
                registerDoctor.list = empschedule_ok.Where(w => w.empid == item.empid).Select(ss => new Scheduletimes { deptname = ss.deptname, scheduleid = ss.scheduleid, begintime = ss.begintime, endtime = ss.endtime, no = ss.no, numbers = GetOutNum(outpatient_number, ss, date).numbers.Value, orders = GetOutNum(outpatient_number, ss, date).orders.Value, limitnumbers = ss.limitnumbers, limitorders = ss.limitorders, roomname = ss.roomname, typename = ss.typename, type_id = ss.typeid, room_id = ss.roomid, dept_id = ss.deptid, price = GetPrice(registertypefee, ss, is_order) }).ToList();
                //医生坐诊添加到列表
                list.Add(registerDoctor);
            }

            //获取医生排班（替诊）
            var empschedule_replace = empschedule.Where(w => w.stateid == 9).ToList();
            //返回替诊坐诊医生排班
            var emp_list_replace = empschedule_replace.Select(s => new { s.replacedocid, s.replacedocname }).Distinct().ToList();
            foreach (var item in emp_list_replace)
            {
                //返回排班时间段
                var scheduletimes_list = empschedule_replace.Where(w => w.replacedocid == item.replacedocid).Select(ss => new Scheduletimes { deptname = ss.deptname, scheduleid = ss.scheduleid, begintime = ss.begintime, endtime = ss.endtime, no = ss.no, numbers = GetOutNum(outpatient_number, ss, date).numbers.Value, orders = GetOutNum(outpatient_number, ss, date).orders.Value, limitnumbers = ss.limitnumbers, limitorders = ss.limitorders, roomname = ss.roomname, typename = ss.typename, type_id = ss.typeid, room_id = ss.roomid, dept_id = ss.deptid, price = GetPrice(registertypefee, ss, is_order) }).ToList();

                //医生是否在集合中已存在
                if (list.Any(a => a.empid == item.replacedocid))
                {
                    foreach (var scheduletimes in scheduletimes_list)
                    {
                        list.Where(w => w.empid == item.replacedocid).First().list.Add(scheduletimes);
                    }
                }
                else
                {
                    //创建医生信息
                    var registerDoctor = new RegisterDoctor { empid = item.replacedocid.Value, empname = item.replacedocname, is_order = is_order, Week = Week.WeekTxt };
                    //医生坐诊时间信息
                    registerDoctor.list = scheduletimes_list;
                    //医生坐诊添加到列表
                    list.Add(registerDoctor);
                }
            }

            return list;
        }

        /// <summary>
        /// 根据科室和日期、医生获取排班
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="deptid">科室ID</param>
        /// <param name="date">指定日期</param>
        /// <param name="doctor_id">医生ID</param>
        /// <returns></returns>
        public async Task<List<Scheduletimes>> GetByDeptAndDateAndDoctorAsync(int store_id, int deptid, DateTime date, int doctor_id)
        {
            //获取客户信息
            var archivesInfo = new Tools.IdentityModels.GetArchives().archives;

            if (date.Date < DateTime.Now.Date)
            {
                throw new MessageException($"请选择当天（{DateTime.Now.Date.ToString("yyyy-MM-dd")}）及以后的日期");
            }
            //获取星期
            var Week = Utils.GetWeek(date);
            var days = Week.WeekNo;
            //获取医生排班
            var empschedule = await Db.Queryable<his_empschedule, his_scheduletimes, his_registertype, p_cons_room>((e, s, r, cr) => new object[] { JoinType.Left, e.scheduleid == s.scheduleid, JoinType.Left, e.typeid == r.typeid, JoinType.Left, e.roomid == cr.id })
                .Where((e, s, r, cr) => e.orgid == archivesInfo.org_id && e.store_id == store_id && SqlFunc.Between(date, e.orderbegin, e.orderend) && s.days == days && e.deptid == deptid)
                .WhereIF(date.Date != DateTime.Now.Date, (e, s, r, cr) => e.orderflag == 2)
                .WhereIF(doctor_id > 0, (e, s, r, cr) => e.empid == doctor_id || s.replacedocid == doctor_id)
                .Select((e, s, r, cr) => new EmpscheduleScheduletimes { begintime = s.begintime, callflag = e.callflag, days = s.days, deptid = e.deptid, deptname = e.deptname, empid = e.empid, empname = e.empname, endtime = s.endtime, friday = e.friday, limitnumbers = e.limitnumbers, limitorders = e.limitorders, monday = e.monday, no = s.no, orderbegin = e.orderbegin, orderend = e.orderend, orderflag = e.orderflag, orgid = e.orgid, replacedocid = s.replacedocid, replacedocname = s.replacedocname, replaceid = s.replaceid, roomid = e.roomid, roomname = e.roomname, saturday = e.saturday, scheduleid = e.scheduleid, stateid = s.stateid, statename = s.statename, store_id = e.store_id, sunday = e.sunday, thursday = e.thursday, tuesday = e.tuesday, typeid = e.typeid, wednesday = e.wednesday, typename = r.typename, position = cr.position })
                .WithCache()
                .ToListAsync();

            //挂号还是预约
            bool is_order = false;
            if (date.Date != DateTime.Now.Date)
            {
                is_order = true;
                empschedule = empschedule.Where(w => w.orderflag == 2).ToList();
            }
            else
            {
                var nowDate = DateTime.Now.TimeOfDay;
                empschedule = empschedule.Where(w => w.endtime > nowDate).ToList();
            }

            if (empschedule.Count == 0)
            {
                throw new MessageException($"此医生{date.Date.ToString("yyyy-MM-dd")}无排班");
            }

            //获取费别费用
            var registertypefee = await Db.Queryable<his_registertypefee, h_itemspec>((r, i) => new object[] { JoinType.Left, r.specid == i.specid }).Select((r, i) => new RegisterTypeFee { typeid = r.typeid, quantiry = r.quantiry, orderflag = r.orderflag, sale_price = i.sale_price }).WithCache().ToListAsync();

            //获取排班ID
            var scheduleids = empschedule.Select(s => s.scheduleid).ToList();
            //门诊号数表
            var outpatient_number = await Db.Queryable<his_outpatient_number>().Where(w => scheduleids.Contains(w.scheduleid)).WithCache().ToListAsync();

            //医生坐诊时间信息
            var list = empschedule.Select(ss => new Scheduletimes { deptname = ss.deptname, scheduleid = ss.scheduleid, begintime = ss.begintime, endtime = ss.endtime, no = ss.no, numbers = GetOutNum(outpatient_number, ss, date).numbers.Value, orders = GetOutNum(outpatient_number, ss, date).orders.Value, limitnumbers = ss.limitnumbers, limitorders = ss.limitorders, roomname = ss.roomname, typename = ss.typename, type_id = ss.typeid, room_id = ss.roomid, dept_id = ss.deptid, price = GetPrice(registertypefee, ss, is_order), position = ss.position }).ToList();

            return list;
        }

        /// <summary>
        /// 根据医生获取最近一周排班
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="doctor_id">医生ID</param>
        /// <returns></returns>
        public async Task<List<RegisterDate>> GetByDoctorAsync(int store_id, int doctor_id)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var registerDates = new List<RegisterDate>();

            //获取医生排班
            var empschedule = await Db.Queryable<his_empschedule, his_scheduletimes, his_registertype>((e, s, r) => new object[] { JoinType.Left, e.scheduleid == s.scheduleid, JoinType.Left, e.typeid == r.typeid })
                .Where((e, s, r) => e.orgid == userInfo.org_id && e.store_id == store_id && (e.empid == doctor_id || s.replacedocid == doctor_id) && s.stateid != 8)
                .Select((e, s, r) => new EmpscheduleScheduletimes { begintime = s.begintime, callflag = e.callflag, days = s.days, deptid = e.deptid, deptname = e.deptname, empid = e.empid, empname = e.empname, endtime = s.endtime, friday = e.friday, limitnumbers = e.limitnumbers, limitorders = e.limitorders, monday = e.monday, no = s.no, orderbegin = e.orderbegin, orderend = e.orderend, orderflag = e.orderflag, orgid = e.orgid, replacedocid = s.replacedocid, replacedocname = s.replacedocname, replaceid = s.replaceid, roomid = e.roomid, roomname = e.roomname, saturday = e.saturday, scheduleid = e.scheduleid, stateid = s.stateid, statename = s.statename, store_id = e.store_id, sunday = e.sunday, thursday = e.thursday, tuesday = e.tuesday, typeid = e.typeid, wednesday = e.wednesday, typename = r.typename })
                .WithCache()
                .ToListAsync();
            if (empschedule.Count == 0)
            {
                throw new MessageException("此医生无排班信息");
            }

            //获取费别费用
            var registertypefee = await Db.Queryable<his_registertypefee, h_itemspec>((r, i) => new object[] { JoinType.Left, r.specid == i.specid }).Select((r, i) => new RegisterTypeFee { typeid = r.typeid, quantiry = r.quantiry, orderflag = r.orderflag, sale_price = i.sale_price }).WithCache().ToListAsync();

            //获取排班ID
            var scheduleids = empschedule.Select(s => s.scheduleid).ToList();
            //门诊号数表
            var outpatient_number = await Db.Queryable<his_outpatient_number>().Where(w => scheduleids.Contains(w.scheduleid)).WithCache().ToListAsync();

            var date = DateTime.Now.Date;

            for (int i = 0; i < 7; i++)
            {
                var day_empschedule = registerDateArray(registertypefee, date.AddDays(i), empschedule, outpatient_number);
                registerDates.Add(day_empschedule);
            }

            return registerDates;
        }

        private static RegisterDate registerDateArray(List<RegisterTypeFee> registertypefee, DateTime date, List<EmpscheduleScheduletimes> empschedule, List<his_outpatient_number> outpatient_number)
        {
            //获取星期
            var Week = Utils.GetWeek(date);
            var days = Week.WeekNo;
            var this_date = date.ToString("yyyy-MM-dd");

            var use_empschedule = empschedule.Where(w => w.endtime > DateTime.Now.TimeOfDay);

            //挂号还是预约
            bool is_order = false;
            if (DateTime.Now.Date != date)
            {
                is_order = true;
                use_empschedule = empschedule.Where(w => w.orderflag == 2);
            }

            var registerDate = new RegisterDate { Date = this_date, is_order = is_order, Week = Week.WeekTxt };
            registerDate.list = use_empschedule.Where(w => w.days == days && w.orderbegin <= date && w.orderend >= date).Select(ss => new Scheduletimes { scheduleid = ss.scheduleid, deptname = ss.deptname, begintime = ss.begintime, endtime = ss.endtime, no = ss.no, numbers = GetOutNum(outpatient_number, ss, date).numbers.Value, orders = GetOutNum(outpatient_number, ss, date).orders.Value, limitnumbers = ss.limitnumbers, limitorders = ss.limitorders, roomname = ss.roomname, typename = ss.typename, type_id = ss.typeid, dept_id = ss.deptid, room_id = ss.roomid, price = GetPrice(registertypefee, ss, is_order) }).ToList();
            return registerDate;
        }

        private static his_outpatient_number GetOutNum(List<his_outpatient_number> outpatient_number, EmpscheduleScheduletimes ss, DateTime date)
        {
            var OutNum = outpatient_number.Where(w => w.scheduleid == ss.scheduleid && w.days == ss.days && w.no == ss.no && w.register_date == date).FirstOrDefault();
            if (OutNum == null)
            {
                OutNum = new his_outpatient_number { numbers = 0, orders = 0 };
            }
            return OutNum;
        }

        private static decimal GetPrice(List<RegisterTypeFee> registertypefee, EmpscheduleScheduletimes ss, bool is_order)
        {
            var e_sum = registertypefee.Where(w => w.typeid == ss.typeid);
            if (!is_order)
            {
                e_sum = e_sum.Where(w => w.orderflag == 3);
            }
            return e_sum.Sum(sum => sum.sale_price * sum.quantiry);
        }

        #endregion


        public async Task<bool> ModifyDateAsync(ModifyDateModel entity)
        {
            var result = Db.Ado.UseTran(() =>
            {
                ModifyDate(entity);
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }
        /// <summary>
        /// 更改挂号日期
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public void ModifyDate(ModifyDateModel entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            if (entity.regdate.Date < DateTime.Now.Date)
            {
                throw new MessageException("请选择正确的挂号日期！");
            }

            if (entity.regid <= 0 || entity.centerid < 0)
            {
                throw new MessageException("挂号记录不完整！");
            }
            //根据当前挂号id查询此单是否可以改期
            var reg = Db.Queryable<his_register, f_balance, r_recharge>((r, b, re) => new object[] { JoinType.Left, r.checkoutid == b.balanceid, JoinType.Left, re.check_id == b.balanceid })
                                .Where((r, b, re) => r.regid == entity.regid && r.canceled == 3)
                                .Select((r, b, re) => new { r, b, re })
                                .WithCache()
                                .First();
            if (reg == null)
            {
                throw new MessageException("此挂号记录无效！");
            }
            var regDetial = reg.r;
            var reg_balance = reg.b;
            //消费记录
            var rechargeDetail = reg.re;
            if (!(regDetial.clinicstateid == 5))
            {
                throw new MessageException("此挂号不能延期！");
            }
            //查询档案账户
            var accountMessage = Db.Queryable<c_account, c_archives, c_archives_level>((ac, a, al) => new object[] { JoinType.Left, ac.archives_id == a.id, JoinType.Left, al.id == a.grade_id })
                .Where((ac, a, al) => ac.archives_id == regDetial.centerid)
                .Select((ac, a, al) => new { account = ac, archives = a, level = al })
                .WithCache().First();
            //档案账户
            var account = accountMessage.account;
            //档案信息
            var archives = accountMessage.archives;
            //档案等级
            var level = accountMessage.level;
            //消费记录
            var recharge = new r_recharge();

            if (entity.shouldamount > regDetial.actualamount && account.balance < (entity.shouldamount - regDetial.actualamount))
            {
                throw new MessageException("账户余额不足，请先充值!");
            }


            //修改充值记录状态
            Db.Updateable<r_recharge>().SetColumns(s => new r_recharge { state_id = 22, state = "已退回" }).Where(w => w.check_id == reg_balance.balanceid).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

            //获取挂号费信息
            var registerfee = Db.Queryable<his_registertype, his_registertypefee, h_itemspec, b_basecode, h_item>((rt, rtf, i, b, hi) => new object[] { JoinType.Left, rt.typeid == rtf.typeid, JoinType.Left, rtf.specid == i.specid, JoinType.Left, i.incomeid == b.valueid && b.baseid == 66, JoinType.Left, i.itemid == hi.item_id })
            .Where((rt, rtf, i, b, hi) => rt.typeid == entity.typeid)
            .WhereIF(entity.orderflag == 3, (rt, rtf, i, b, hi) => rtf.orderflag == 3)
            .Select((rt, rtf, i, b, hi) => new { shouldamount = i.sale_price, rtf.specid, rtf.quantiry, i.buy_price, i.incomeid, i.itemid, b.value, hi.trade_name, i.salsemodulus, i.specname, i.salseunit })
            .WithCache()
            .ToList();

            //判断金额是否正确
            if (registerfee.Sum(s => s.shouldamount * s.quantiry) != entity.shouldamount)
            {
                throw new MessageException("挂号费不明确！");
            }
            //如果是账户支付，退回账户余额
            if (rechargeDetail.give_money <= 0)
            {
                account.balance += regDetial.actualamount;
            }
            //退回优惠金额
            else
            {
                account.coupon += regDetial.actualamount;
            }

            //修改账户余额
            var Encrypt = account.amount + account.archives_id + account.balance + account.consume + account.coupon + account.integral + account.noneamount + account.password + account.rate + account.recharge + account.salseamount + account.settleamount + account.total_coupon + account.total_integral;
            var code = MetarnetRegex.Encrypt(MetarnetRegex.SHA512Encrypt(Encrypt.ToString()));
            account.code = code;
            Db.Updateable(account).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();


            //查询结算单id
            var date = regDetial.recorddate;
            var balanceDetail = Db.Queryable<f_balance>()
                                .Where(w => w.centerid == archives.id && w.stateid == 19 && w.issettle == 3 && w.store_id == entity.store_id)
                                 .Where(" to_char(recorddate, 'yyyy-MM-dd')=to_char(@date , 'yyyy-MM-dd')", new { date })
                                .First();
            if (balanceDetail == null || balanceDetail.balanceid <= 0)
            {
                throw new MessageException("未获取到结算单！");
            }
            //修改门诊收费结算单
            Db.Updateable<f_balance>().SetColumns(s => s.stateid == 21).Where(s => s.balanceid == balanceDetail.balanceid).EnableDiffLogEvent().RemoveDataCache().ExecuteCommand();

            //查询门诊号表，修改门诊号
            var outPatient = Db.Queryable<his_register, his_outpatient_no, his_outpatient_number>((r, ono, onn) => new object[] { JoinType.Left, ono.regid == r.regid, JoinType.Left, onn.scheduleid == ono.scheduleid })
                    .Where((r, ono, onn) => r.regid == entity.regid && ono.register_date == onn.register_date && ono.no == onn.no)
                    .Select((r, ono, onn) => new { onDetaile = onn })
                    .First();

            //门诊号数表
            var onDetail = outPatient.onDetaile;

            //修改挂号单状态
            Db.Updateable<his_register>().SetColumns(s => new his_register { canceled = 2, stateid = 21, clinicstateid = 21 }).Where(s => s.regid == entity.regid).EnableDiffLogEvent().RemoveDataCache().ExecuteCommand();

            //修改门诊号表
            if (regDetial.billtypeid == 2)
            {
                Db.Updateable<his_outpatient_number>()
                .SetColumns(s => s.numbers == s.numbers - 1)
                .Where(s => s.register_date == onDetail.register_date && s.scheduleid == onDetail.scheduleid && s.no == onDetail.no)
                .RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
            }
            else if (regDetial.billtypeid == 1)
            {
                var order = Convert.ToInt16(onDetail.orders - 1);
                var num = Convert.ToInt16(onDetail.numbers - 1);
                Db.Updateable<his_outpatient_number>().SetColumns(s => new his_outpatient_number { orders = order, numbers = num })
                .Where(s => s.register_date == onDetail.register_date && s.scheduleid == onDetail.scheduleid && s.no == onDetail.no)
                .RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
            }


            //新增挂号
            var hisRegister = new HisRegister();
            hisRegister.cardno = archives.card_no;
            hisRegister.centerid = archives.id;
            hisRegister.days = entity.days;
            hisRegister.deptid = entity.deptid;
            hisRegister.deptname = entity.deptname;
            hisRegister.doctorid = entity.doctorid;
            hisRegister.doctorname = entity.doctorname;
            hisRegister.no = entity.no;
            hisRegister.orderflag = entity.orderflag;
            hisRegister.regdate = entity.regdate;
            hisRegister.room = entity.room;
            hisRegister.roomid = entity.roomid;
            hisRegister.scheduleid = entity.scheduleid;
            hisRegister.shouldamount = entity.shouldamount;
            hisRegister.source = entity.source;
            hisRegister.sourceid = entity.sourceid;
            hisRegister.store_id = entity.store_id;
            hisRegister.summary = entity.summary;
            hisRegister.typeid = entity.typeid;

            var scheduletimes = Db.Queryable<his_scheduletimes>().Where(w => w.scheduleid == entity.scheduleid && w.days == entity.days && w.no == entity.no).WithCache().First();
            var regdatestr = entity.regdate.ToString($"yyyy-MM-dd {scheduletimes.begintime}");

            //调用挂号
            var register=RegisterCommon(hisRegister, userInfo, scheduletimes, regdatestr, account, archives, level);

            //通知
            var notice = new Business.NoticeService();
            var noticeList = new List<AddNoticeModel>();
            var notice_content = "";
            var employeeSocket = new List<WebSocketModel>();

            if (archives.to_director_id > 0)
            {
                //预约挂号通知
                notice_content = $"{{\"name\":\"{archives.name}\",\"date\":\"{regdatestr}\",\"msg\":\"预约挂号改期\",\"doctor\":\"{entity.doctorname}\"}}";
                employeeSocket.Add(new WebSocketModel { userId = archives.to_director_id.Value, content = notice_content });

                var employeenotice = new List<employeeMes>();
                employeenotice.Add(new employeeMes { employee_id = archives.to_director_id.Value, employee = archives.to_director });

                notice.NewMethod(register.regid.ToString(), archives, register.store_id, notice, noticeList, 7, 2, notice_content, archives.name, employeenotice);
                
                //新增通知
                notice.AddNotice(noticeList);
                //消息提醒
                ChatWebSocketMiddleware.SendListAsync(employeeSocket);

            }
        }

        /// <summary>
        /// 新增操作
        /// </summary>
        /// <param name="entity"></param>
        public int Create(ModifyDateModel entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var registerfee = Db.Queryable<his_registertype, his_registertypefee, h_itemspec, b_basecode, h_item>((rt, rtf, i, b, hi) => new object[] { JoinType.Left, rt.typeid == rtf.typeid, JoinType.Left, rtf.specid == i.specid, JoinType.Left, i.incomeid == b.valueid && b.baseid == 66, JoinType.Left, i.itemid == hi.item_id })
              .Where((rt, rtf, i, b, hi) => rt.typeid == entity.typeid)
              .WhereIF(entity.orderflag == 3, (rt, rtf, i, b, hi) => rtf.orderflag == 3)
              .Select((rt, rtf, i, b, hi) => new { shouldamount = i.sale_price, rtf.specid, rtf.quantiry, i.buy_price, i.incomeid, i.itemid, b.value, hi.trade_name, i.salsemodulus, i.specname, i.salseunit })
              .WithCache()
              .ToList();

            //写入门诊收费结算单
            var balance = new f_balance { accountpay = entity.shouldamount, actualamount = entity.shouldamount, alipay = 0, bankpay = 0, cashpay = 0, centerid = entity.centerid, checkoutid = 0, clinicid = 0, deptid = entity.deptid, deptname = entity.deptname, doctorid = entity.doctorid, doctorname = entity.doctorname, insuranceid = 0, insurancepay = 0, issettle = 3, orgid = userInfo.org_id, otherpay = 0, recorddate = DateTime.Now, returnid = 0, shouldamount = entity.shouldamount, source = entity.source, sourceid = entity.sourceid, stateid = 19, store_id = entity.store_id, typeid = 3, wechatpay = 0, operatorid = userInfo.id, balancedate = DateTime.Now, operator_name = userInfo.name, summay = "挂号自动结算", is_register = 2 };
            var balance_id = Db.Insertable(balance).ExecuteReturnIdentity();
            redisCache.RemoveAll<f_balance>();

            //写入门诊收费清单
            var balancedetail_list = registerfee.Select((s, index) => new f_balancedetail { actualamount = s.shouldamount, shouldamount = s.shouldamount, specid = s.specid, balanceid = balance_id, cost = s.buy_price, execdeptid = entity.deptid, execdeptname = entity.deptname, execstate = "已执行", modulus = s.salsemodulus, execstateid = 17, incomeid = 0, invoicename = s.value, itemname = s.trade_name, numbers = 1, orderid = short.Parse(index.ToString()), price = s.shouldamount, quantity = 1, rebate = 1, receipeid = 0, sigledousage = 1, specname = s.specname, unitname = s.salseunit, usage = "其他", usageid = 9, frequencyid = 0, frequency = "" }).ToList();
            Db.Insertable(balancedetail_list).ExecuteCommand();
            redisCache.RemoveAll<f_balancedetail>();

            var register = new his_register();
            //默认挂号信息
            register.orgid = userInfo.org_id;
            register.store_id = entity.store_id;
            register.deptid = entity.deptid;
            register.deptname = entity.deptname;
            register.doctorid = entity.doctorid;
            register.doctorname = entity.doctorname;
            register.typeid = entity.typeid;
            register.centerid = entity.centerid;
            register.clinicid = 0;
            register.recorddate = DateTime.Now;
            //排班时间
            var scheduletimes = Db.Queryable<his_scheduletimes>().Where(w => w.scheduleid == entity.scheduleid && w.days == entity.days && w.no == entity.no).WithCache().First();
            register.regdate = DateTime.Parse(entity.regdate.ToString($"yyyy-MM-dd {scheduletimes.begintime}"));

            register.operatorid = userInfo.id;
            register.operator_name = userInfo.name;
            register.cardno = entity.cardno;
            register.shouldamount = entity.shouldamount;
            register.actualamount = entity.shouldamount;
            register.paymentid = 5;
            register.payment = "账户";
            register.settleflag = 2;
            register.orderflag = entity.orderflag;
            register.roomid = entity.roomid;
            register.room = entity.room;
            register.canceled = 3;
            register.stateid = 11;
            register.checkoutid = balance_id;

            short numbers = 1;
            short orders = 0;
            register.billtypeid = 2;
            register.billtype = "挂号";
            if (register.orderflag == 2)
            {
                register.billtypeid = 1;
                register.billtype = "预约";
                orders = 1;
            }

            register.sourceid = entity.sourceid;
            register.source = entity.source;
            register.clinicstateid = 5;
            register.summary = entity.summary;

            //添加挂号记录
            var regid = Db.Insertable(register).ExecuteReturnIdentity();
            redisCache.RemoveAll<his_register>();

            //添加挂号费
            var registerfee_list = registerfee.Select(s => new his_registerfee { actualamount = entity.shouldamount, regid = regid, shouldamount = s.shouldamount, specid = s.specid }).ToList();
            Db.Insertable(registerfee_list).ExecuteCommand();
            redisCache.RemoveAll<his_registerfee>();

            //添加门诊号数
            var outpatient_number = Db.Queryable<his_outpatient_number>().Where(w => w.scheduleid == entity.scheduleid && w.days == entity.days && w.no == entity.no && w.register_date == entity.regdate.Date).WithCache().First();
            if (outpatient_number != null)
            {
                outpatient_number.orders += orders;
                outpatient_number.numbers += numbers;
                Db.Updateable(outpatient_number).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
            }
            else
            {
                outpatient_number = new his_outpatient_number { days = entity.days, no = entity.no, numbers = numbers, orders = orders, register_date = entity.regdate.Date, scheduleid = entity.scheduleid };
                Db.Insertable(outpatient_number).ExecuteCommand();
                redisCache.RemoveAll<his_outpatient_number>();
            }

            //添加门诊号
            var outpatient_no = new his_outpatient_no { days = entity.days, no = entity.no, order_no = 0, regid = regid, register_date = entity.regdate, scheduleid = entity.scheduleid };
            Db.Insertable(outpatient_no).ExecuteCommand();
            redisCache.RemoveAll<his_outpatient_no>();

            return balance_id;
        }

        /// <summary>
        /// 获得分页列表（客户端）
        /// </summary>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        public async Task<Page<ArcRegisterRecord>> GetPagesArcAsync(string order, int orderType, int limit, int page)
        {
            //获取客户信息
            var archivesInfo = new Tools.IdentityModels.GetArchives().archives;
            var orderTypeStr = " Asc";
            if (orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            return await Db.Queryable<his_register, c_archives, his_outpatient_no, his_scheduletimes, b_basecode>((r, a, hon, hs, b) => new object[] { JoinType.Left, r.centerid == a.id, JoinType.Left, r.regid == hon.regid, JoinType.Left, hon.scheduleid == hs.scheduleid && hon.days == hs.days && hon.no == hs.no, JoinType.Left, r.clinicstateid == b.valueid && b.baseid == 0 })
                .Where((r, a, hon, hs, b) => r.centerid == archivesInfo.id)
                .Select((r, a, hon, hs, b) => new ArcRegisterRecord { arc_name = a.name, create_datetime = r.recorddate, dept = r.deptname, doctor = r.doctorname, money = r.actualamount, phone = a.phone, reg_datetime = hon.register_date, begintime = hs.begintime, endtime = hs.endtime, state_id = b.valueid })
                .OrderBy(order + orderTypeStr)
                .WithCache()
                .ToPageAsync(page, limit);
        }

        /// <summary>
        /// 获取一周日期信息（日期、星期、号数）和当前客户是否收藏
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> GetWeeks(int doctor_id)
        {
            //获取客户信息
            var archivesInfo = new Tools.IdentityModels.GetArchives().archives;
            var collection = await Db.Queryable<c_collection>().AnyAsync(w => w.doctor_id == doctor_id && w.archives_id == archivesInfo.id);
            //获取一周日期信息（日期、星期、号数）
            var list = new List<Week>();
            for (int i = 0; i < 7; i++)
            {
                list.Add(GetWeek(DateTime.Now.AddDays(i)));
            }
            list = list.Select(s => new Week { Date = s.Date, day = s.day, WeekNo = s.WeekNo, WeekTxt = s.WeekTxt.Replace("星期", "") }).ToList();
            return new { collection, list };
        }
    }
}
