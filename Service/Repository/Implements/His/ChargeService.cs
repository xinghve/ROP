using Models.DB;
using Models.View.Business;
using Models.View.His;
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

namespace Service.Repository.Implements.His
{
    /// <summary>
    /// 收费
    /// </summary>
    public class ChargeService : DbContext, IChargeService
    {
        //账户余额
        decimal accountBalance = 0;
        //账户赠送金额
        decimal accountGive = 0;
        //待结算金额
        decimal sumAccount = 0;
        //无折扣待结算金额
        decimal noDiscount = 0;
        /// <summary>
        /// 收费
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ChargeAsync(Charge entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            //通知
            var notice = new Business.NoticeService();
            var noticeList = new List<AddNoticeModel>();
            var notice_content = "";
            var employeeSocket = new List<WebSocketModel>();

            var result = await Db.Ado.UseTranAsync(() =>
            {
                //查询档案账户
                var account_al = Db.Queryable<c_account, c_archives, c_archives_level>((a, ac, al) => new object[] { JoinType.Left, a.archives_id == ac.id, JoinType.Left, ac.grade_id == al.id }).Where((a, ac, al) => a.archives_id == entity.archives_id).Select((a, ac, al) => new { account = a, archives = ac, level = al }).WithCache().First();
                var archives = account_al.archives;
                var level = account_al.level;
                var account = account_al.account;
                //查询收费单信息
                var balances = Db.Queryable<f_balance>().Where(w => w.centerid == entity.archives_id && w.stateid == 18).WithCache().ToList();
                sumAccount = balances.Sum(s => s.shouldamount);
                //无折扣
                noDiscount = balances.Sum(s => s.shouldamount);
                //账户余额
                accountBalance = account.balance;
                //折扣率
                decimal discountRate = 100;
                //如果优先使用赠送金额
                if (entity.isUse == 2)
                {
                    if (account.coupon <= 0)
                    {
                        throw new MessageException("已无可使用的优惠金额,请重新选择！");
                    }
                    if ((account.balance + account.coupon) < noDiscount)
                    {
                        throw new MessageException("账户余额不足，请先充值");
                    }
                    //如果赠送余额充足，扣除赠送余额
                    if (noDiscount <= account.coupon && (account.coupon - noDiscount) >= 0)
                    {
                        accountGive = account.coupon;
                        account.coupon -= noDiscount;
                    }
                    else
                    {
                        account.balance -= noDiscount - account.coupon;
                        accountGive = account.coupon;
                        account.coupon = 0;
                        sumAccount = noDiscount - accountGive;
                    }
                }
                else if (entity.isUse == 3)
                {
                    discountRate = level.discount_rate.Value;
                    if (level.discount_rate > 0)
                    {
                        sumAccount = sumAccount * discountRate / 100;
                    }
                    if (account.balance < sumAccount)
                    {
                        throw new MessageException("账户余额不足，请先充值");
                    }
                    //扣除账户余额
                    account.balance -= sumAccount;
                }
                else
                {
                    throw new MessageException("未知情况！");
                }

                var Encrypt = account.amount + account.archives_id + account.balance + account.consume + account.coupon + account.integral + account.noneamount + account.password + account.rate + account.recharge + account.salseamount + account.settleamount + account.total_coupon + account.total_integral;
                var code = MetarnetRegex.Encrypt(MetarnetRegex.SHA512Encrypt(Encrypt.ToString()));
                account.code = code;
                Db.Updateable(account).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                //获取收费单id
                var balanceids = balances.Select(s => s.balanceid).ToList();
                //查询用户所属科室
                var depts = Db.Queryable<p_dept_nature, p_employee_role, p_dept>((dn, er, d) => new object[] { JoinType.Left, dn.dept_id == er.dept_id, JoinType.Left, er.dept_id == d.id })
                .Where((dn, er, d) => er.employee_id == userInfo.id)
                .GroupBy((dn, er, d) => new { d.id, d.dept_type_code, d.name })
                .Select((dn, er, d) => new { d.id, d.dept_type_code, d.name })
                .WithCache()
                .ToList();
                //获取人员财务科室信息
                var cwdept = depts.Where(w => w.dept_type_code == "07").FirstOrDefault();
                if (cwdept == null)
                {
                    cwdept = depts.FirstOrDefault();
                }

                //当前客户待缴费单据
                var applys = Db.Queryable<his_applybill>().Where(w => w.centerid == entity.archives_id && w.stateid == 15 && w.issettle == 3).WithCache().ToList();
                var applyids = applys.Select(s => s.applyid).ToList();


                var thisdiscountRate = entity.isUse == 2 ? 1 : discountRate / 100;
                //修改门诊收费
                Db.Updateable<his_applybill>().SetColumns(s => new his_applybill { actualamount = s.shouldamount * thisdiscountRate, issettle = 2 }).Where(w => applyids.Contains(w.applyid)).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();


                //修改门诊收费清单
                Db.Updateable<his_applycontent>().SetColumns(s => new his_applycontent { actualamount = s.shouldamount * thisdiscountRate }).Where(w => applyids.Contains(w.applyid)).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                //修改就诊记录总费用
                var clinics = balances.Select(s => s.clinicid).ToList();
                clinics.ForEach(c =>
                {
                    var clinicrecord_amount = Db.Queryable<his_clinicrecord>().Where(w => w.clinicid == c).First().amount;
                    var amount = Db.Queryable<f_balance>().Where(w => w.clinicid == c).Sum(sum => sum.actualamount);
                    clinicrecord_amount += amount;
                    Db.Updateable<his_clinicrecord>().SetColumns(s => new his_clinicrecord { amount = clinicrecord_amount }).Where(w => w.clinicid == c).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                });

                //修改结算单

                var use_balance = sumAccount;
                var use_give = accountGive;
                decimal sur = 0;
                balances = balances.Select(s => new f_balance { accountpay = SpendBalance(s, thisdiscountRate, ref sur, ref use_balance), actualamount = s.shouldamount * thisdiscountRate, alipay = s.alipay, balancedate = DateTime.Now, balanceid = s.balanceid, bankpay = s.bankpay, cashpay = s.cashpay, centerid = s.centerid, checkoutid = s.checkoutid, clinicid = s.clinicid, couponpay = sur, deptid = s.deptid, deptname = s.deptname, doctorid = s.doctorid, doctorname = s.doctorname, insuranceid = s.insuranceid, insurancepay = s.insurancepay, issettle = 2, is_register = s.is_register, operatorid = userInfo.id, operator_name = userInfo.name, orgid = s.orgid, otherpay = s.otherpay, recorddate = s.recorddate, returnid = s.returnid, shouldamount = s.shouldamount, source = "窗口结算", sourceid = 1, stateid = 19, store_id = s.store_id, summay = "结算", typeid = s.typeid, wechatpay = s.wechatpay }).ToList();
                balances.ForEach(c =>
                {
                    Db.Updateable<f_balance>().SetColumns(s => new f_balance { source = c.source, sourceid = c.sourceid, stateid = c.stateid, balancedate = c.balancedate, operatorid = c.operatorid, operator_name = c.operator_name, issettle = c.issettle, actualamount = c.actualamount, accountpay = c.accountpay, summay = c.summay, couponpay = c.couponpay }).Where(w => w.balanceid == c.balanceid).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                });

                //redisCache.RemoveAll<f_balance>();

                //修改结算明细
                Db.Updateable<f_balancedetail>().SetColumns(s => new f_balancedetail { actualamount = s.shouldamount * thisdiscountRate, rebate = level.discount_rate.Value }).Where(w => balanceids.Contains(w.balanceid)).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                //写入充值记录（消费）
                var bill_no = entity.archives_id + DateTime.Now.ToString("yyyyMMddhhmmssffffff");
                var recharge = balances.Select((s, index) => new r_recharge { archives = archives.name, archives_id = entity.archives_id, archives_phone = archives.phone, balance = GetBalance(s, level, ref accountBalance, entity.isUse, ref accountGive, ref sumAccount), bill_no = bill_no, card_no = archives.card_no, categroy_id = 2, check_id = s.balanceid, consultation_id = s.clinicid, creater = userInfo.name, creater_id = userInfo.id, level = level.name, give_balance = account.coupon, give_money = GiveMoney(accountBalance, s, ref accountGive, level, entity.isUse, ref sumAccount), give_total_money = account.total_coupon, integral = Convert.ToInt32(account.integral), money_integral = level.money_integral, no = archives.id + DateTime.Now.ToString("yyMMddhhmmssffff"), occurrence_date = DateTime.Now, org_id = userInfo.org_id, recharge_date = DateTime.Now.AddMinutes(index), recharge_integral = 0, recharge_money = s.shouldamount * thisdiscountRate, state = "已消费", state_id = 20, store_id = s.store_id, total_integral = account.total_integral, total_money = account.recharge, way_code = "5", way = "账户", to_director_id = archives.to_director_id, to_director = archives.to_director, discount_rate = discountRate }).ToList();
                Db.Insertable(recharge).ExecuteCommand();
                redisCache.RemoveAll<r_recharge>();

                //查询客户所属分销人员
                var distributor = Db.Queryable<c_distributor, p_distributor>((cd, pd) => new object[] { JoinType.Left, cd.distributor_id == pd.id }).Where((cd, pd) => cd.archives_id == archives.id).Select((cd, pd) => pd).WithCache().First();
                //查询分销人员提成比例
                var royalty_rate = Db.Queryable<p_org>().Where(w => w.id == userInfo.org_id).Select(s => s.royalty_rate).WithCache().First();
                if (distributor != null && royalty_rate > 0)
                {
                    //写入提成记录
                    var amount = balances.Select((s, index) => new r_amount { archives = archives.name, archives_id = entity.archives_id, archives_phone = archives.phone, amount_date = DateTime.Now, balance_id = s.balanceid, card_no = archives.card_no, distributor = distributor.name, distributor_id = distributor.id, money = entity.isUse == 2 ? s.shouldamount * royalty_rate / 100 : s.shouldamount * discountRate * royalty_rate / 100, org_id = userInfo.org_id, store_id = s.store_id }).Where(w => w.money > 0).ToList();
                    if (amount.Count > 0)
                    {
                        Db.Insertable(amount).ExecuteCommand();
                        redisCache.RemoveAll<r_amount>();
                        var sum = amount.Sum(s => s.money);
                        //修改分销人员账户信息
                        Db.Updateable<p_distributor>().SetColumns(s => new p_distributor { amount = s.amount + sum, noneamount = s.noneamount + sum }).Where(w => w.id == distributor.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                    }
                }

                //查询客户余额下限
                var balance_lower = Db.Queryable<c_archives, c_account, c_archives_level>((a, ac, al) => new object[] { JoinType.Left, ac.archives_id == a.id, JoinType.Left, a.grade_id == al.id })
                                 .Where((a, ac, al) => a.id == entity.archives_id && a.org_id == userInfo.org_id && ac.balance <= al.balance_limit_lower)
                                 .Select((a, ac, al) => new { a.name, a.id, a.to_director_id, a.image_url,al.balance_limit_lower })
                                 .WithCache()
                                 .First();
                if (balance_lower != null)
                {
                    if (balance_lower.to_director_id > 0)
                    {
                        var con = $"{{\"name\":\"{balance_lower.name}\",\"msg\":\"余额已达下限！\",\"img_url\":\"{balance_lower.image_url}\",\"type\":\"balance\"}}";
                        employeeSocket.Add(new WebSocketModel { userId = balance_lower.to_director_id.Value, content = con });

                        //余额下线通知
                        notice_content = $"{{\"name\":\"{archives.name}\",\"msg\":\" 余额已达下限,请及时充值\",\"balance\":\"{account.balance}\",\"balance_limit_lower\":\"{balance_lower.balance_limit_lower}\"}}";
                        var employeenotice = new List<employeeMes>();
                        employeenotice.Add(new employeeMes { employee_id = archives.to_director_id.Value, employee = archives.to_director });

                        notice.NewMethod(bill_no, archives, -1, notice, noticeList, 8, 3, notice_content, balance_lower.name, employeenotice);
                        notice.AddNotice(noticeList);
                        //消息提醒
                        ChatWebSocketMiddleware.SendListAsync(employeeSocket);


                    }

                }
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }


        /// <summary>
        /// 获取赠送
        /// </summary>
        /// <param name="accountBalance">账户余额</param>
        /// <param name="balance"></param>
        /// <param name="accountGive">赠送金额</param>
        /// <param name="level"></param>
        /// <param name="isUse"></param>
        /// <param name="sumAccount">需要消费的金额</param>
        /// <returns></returns>
        public decimal GiveMoney(decimal accountBalance, f_balance balance, ref decimal accountGive, c_archives_level level, int isUse, ref decimal sumAccount)
        {
            if (isUse == 3)
            {
                return 0;
            }
            else if (isUse == 2)
            {
                //如果赠送余额充足，扣除赠送余额
                if (noDiscount <= accountGive && (accountGive - noDiscount) > 0)
                {
                    accountGive -= noDiscount;
                    return balance.shouldamount;
                }
                else
                {
                    var je = accountGive;
                    accountGive -= accountGive;
                    noDiscount -= je;
                    return je;
                }

            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="balance"></param>
        /// <param name="use_balance">余额</param>
        /// <param name="sur">剩余</param>
        /// <param name="use_give">赠送金额</param>
        /// <returns></returns>
        public decimal SpendGive(f_balance balance, ref decimal use_balance, ref decimal sur, ref decimal use_give)
        {
            decimal give = 0;
            if (use_balance == 0)
            {
                var thisMoney = balance.shouldamount;
                if (sur > 0)
                {
                    give = sur;
                }
                else
                {
                    give = thisMoney - use_give;
                }
                use_give -= give;
            }

            return give;
        }

        /// <summary>
        /// 获取余额
        /// </summary>
        /// <param name="balance"></param>
        /// <param name="level"></param>
        /// <param name="accountBalance">账户余额</param>
        /// <param name="isUse"></param>
        /// <param name="accountGive">赠送金额</param>
        /// <param name="sumAccount">需要消费的金额</param>
        /// <returns></returns>
        public decimal GetBalance(f_balance balance, c_archives_level level, ref decimal accountBalance, int isUse, ref decimal accountGive, ref decimal sumAccount)
        {
            //应支付
            decimal shouldGive = (balance.shouldamount * level.discount_rate.Value / 100);
            if (isUse == 3)
            {
                accountBalance -= shouldGive;
                return accountBalance;
            }
            else if (isUse == 2)
            {
                //如果赠送余额充足，扣除赠送余额
                if (noDiscount <= accountGive && (accountGive - noDiscount) > 0)
                {
                    accountGive -= balance.shouldamount;
                    sumAccount -= balance.shouldamount;
                    return accountBalance;
                }
                else
                {
                    //赠送余额不充足，先扣赠送余额
                    accountBalance -= balance.shouldamount - accountGive;
                    if (accountGive > 0)
                    {
                        noDiscount -= balance.shouldamount - accountGive;
                    }
                    else
                    {
                        noDiscount -= balance.shouldamount;
                    }
                    return accountBalance;
                }
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="balance"></param>
        /// <param name="thisdiscountRate">折扣率</param>
        /// <param name="sur">剩余</param>
        /// <param name="use_balance">余额</param>
        /// <returns></returns>
        public decimal SpendBalance(f_balance balance, decimal thisdiscountRate, ref decimal sur, ref decimal use_balance)
        {
            decimal balance_money = 0;
            if (use_balance > 0)
            {
                var thisMoney = balance.shouldamount * thisdiscountRate;
                if (thisMoney < use_balance)
                {
                    balance_money = thisMoney;
                    use_balance -= thisMoney;
                }
                else
                {
                    balance_money = use_balance;
                    sur = thisMoney - balance_money;
                    use_balance = 0;
                }
            }

            return balance_money;
        }

        /// <summary>
        /// 获得分页列表（已缴费单明细）
        /// </summary>
        /// <param name="balanceid">申请ID</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        public async Task<Page<ChargePages>> GetBalanceDetailPagesAsync(int balanceid, string order, int orderType, int limit, int page)
        {
            var orderTypeStr = " Asc";
            if (orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            return await Db.Queryable<f_balancedetail, h_itemspec, h_item>((ac, isp, i) => new object[] { JoinType.Left, ac.specid == isp.specid, JoinType.Left, i.item_id == isp.itemid })
                .Where((ac, isp, i) => ac.balanceid == balanceid)
                .Select((ac, isp, i) => new ChargePages { frequecyname = ac.frequency, itemname = i.trade_name, modulus = ac.modulus, orderid = ac.orderid, price = ac.price, quantity = ac.quantity, shouldamount = ac.shouldamount, sigle = ac.sigledousage, specname = isp.specname, unitname = ac.unitname, discount_rate = ac.rebate })
                .OrderBy(order + orderTypeStr)
                .WithCache()
                .ToPageAsync(page, limit);
        }

        /// <summary>
        /// 获得列表（缴费单明细）
        /// </summary>
        /// <param name="balanceid">申请ID</param>
        /// <returns></returns>
        public async Task<dynamic> GetBalanceDetailListAsync(int balanceid)
        {
            var base_list = await Db.Queryable<f_balancedetail, h_itemspec, h_item>((ac, isp, i) => new object[] { JoinType.Left, ac.specid == isp.specid, JoinType.Left, i.item_id == isp.itemid })
                .Where((ac, isp, i) => ac.balanceid == balanceid)
                .Select((ac, isp, i) => new { itemname = i.trade_name, ac.orderid, ac.price, ac.quantity, ac.shouldamount, isp.specname, ac.unitname, ac.rebate, ac.actualamount, ac.invoicename })
                .WithCache()
                .ToListAsync();
            var type_list = base_list.Select(s => s.invoicename).Distinct().ToList();
            var list = type_list.Select(s => new { invoicename = s, shouldamount = base_list.Where(w => w.invoicename == s).Sum(sum => sum.shouldamount), actualamount = base_list.Where(w => w.invoicename == s).Sum(sum => sum.actualamount), list = base_list.Where(w => w.invoicename == s).ToList() }).ToList();
            return list;
        }

        /// <summary>
        /// 获得分页列表（已缴费单）
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="archives_id">客户ID</param>
        /// <param name="doctorid">医生ID</param>
        /// <param name="deptid">科室ID</param>
        /// <param name="startbalancedate">结算时间（开始）</param>
        /// <param name="endbalancedate">结算时间（结束）</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        public async Task<Page<FalanceModel>> GetBalancePagesAsync(int store_id, int archives_id, int doctorid, int deptid, DateTime? startbalancedate, DateTime? endbalancedate, string order, int orderType, int limit, int page)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var orderTypeStr = " Asc";
            if (orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            if (startbalancedate != null)
            {
                startbalancedate = startbalancedate.Value.Date;
            }
            if (endbalancedate != null)
            {
                endbalancedate = endbalancedate.Value.AddDays(1).Date;
            }
            return await Db.Queryable<f_balance, c_archives>((b, c) => new object[] { JoinType.Left, b.centerid == c.id })
                .Where((b, c) => b.orgid == userInfo.org_id && b.store_id == store_id && b.stateid == 19)
                .WhereIF(archives_id > 0, (b, c) => b.centerid == archives_id)
                .WhereIF(doctorid > 0, (b, c) => b.doctorid == doctorid)
                .WhereIF(deptid > 0, (b, c) => b.deptid == deptid)
                .WhereIF(startbalancedate != null, (b, c) => b.balancedate >= startbalancedate)
                .WhereIF(endbalancedate != null, (b, c) => b.balancedate < endbalancedate)
                .OrderBy(order + orderTypeStr)
                .Select((b, c) => new FalanceModel { balanceid = b.balanceid, accountpay = b.accountpay, actualamount = b.actualamount, alipay = b.alipay, balancedate = b.balancedate, bankpay = b.bankpay, cashpay = b.cashpay, checkoutid = b.checkoutid, deptname = b.deptname, doctorname = b.doctorname, source = b.source, operator_name = b.operator_name, shouldamount = b.shouldamount, summay = b.summay, customerName = c.name, couponpay = b.couponpay })
                .WithCache()
                .ToPageAsync(page, limit);
        }

        /// <summary>
        /// 门店待缴费
        /// </summary>
        /// <param name="store_id"></param>
        /// <returns></returns>
        public async Task<List<ChargeArchives>> GetListAsync(int store_id)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            return await Db.Queryable<f_balance, c_archives, c_archives_level, c_account>((b, a, al, ac) => new object[] { JoinType.Left, b.centerid == a.id, JoinType.Left, a.grade_id == al.id, JoinType.Left, ac.archives_id == a.id })
                .Where((b, a, al, ac) => a.org_id == userInfo.org_id && a.store_id == store_id && b.stateid == 18)
                .GroupBy((b, a, al, ac) => new { a.age, a.id, a.name, a.phone, a.sex, al.discount_rate, ac.coupon, ac.balance })
                .Select((b, a, al, ac) => new ChargeArchives { age = a.age, id = a.id, name = a.name, phone = a.phone, sex = a.sex, money = SqlFunc.AggregateSum(b.shouldamount), discount_rate = al.discount_rate, coupon = ac.coupon, balance = ac.balance })
                .ToListAsync();
        }

        /// <summary>
        /// 获得分页列表（待缴费）
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="archives_id">客户ID</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        public async Task<Page<ChargePages>> GetPagesAsync(int store_id, int archives_id, string order, int orderType, int limit, int page)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var orderTypeStr = " Asc";
            if (orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            return await Db.Queryable<f_balancedetail, f_balance, h_itemspec, h_item>((bd, b, isp, i) => new object[] { JoinType.Left, bd.balanceid == b.balanceid, JoinType.Left, isp.specid == bd.specid, JoinType.Left, i.item_id == isp.itemid })
                .Where((bd, b, isp, i) => b.orgid == userInfo.org_id && b.store_id == store_id && b.centerid == archives_id && b.stateid == 18)
                .Select((bd, b, isp, i) => new ChargePages { frequecyname = bd.frequency, itemname = i.trade_name, modulus = bd.modulus, orderid = bd.orderid, price = bd.price, quantity = bd.quantity, shouldamount = bd.shouldamount, sigle = bd.sigledousage, specname = isp.specname, unitname = bd.unitname, discount_rate = bd.rebate })
                .OrderBy(order + orderTypeStr)
                .WithCache()
                .ToPageAsync(page, limit);
        }
    }
}
