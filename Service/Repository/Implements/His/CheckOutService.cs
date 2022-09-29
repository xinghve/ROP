using Models.DB;
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
using static Tools.IdentityModels.GetUser;

namespace Service.Repository.Implements.His
{
    /// <summary>
    /// 收费交账单业务
    /// </summary>
    public class CheckOutService:DbContext,ICheckOutService
    {
        //获取用户信息
        private readonly UserInfo  userInfo = new Tools.IdentityModels.GetUser().userInfo;

        /// <summary>
        /// 门诊收费结算单分页数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Page<FalanceModel>> GetPageAsync(FalanceSearch entity)
        {
            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }

            return await Db.Queryable<f_balance,p_store>((b,s)=>new object[] {JoinType.Left,b.store_id==s.id })
                .Where((b, s) => b.orgid == userInfo.org_id && b.stateid == 19&& b.returnid==0&&b.checkoutid==0&&b.balancedate!=null)
                .WhereIF(entity.storeId>0, (b, s) => b.store_id==entity.storeId)
                .WhereIF(entity.deptId>0, (b, s) => b.deptid==entity.deptId)
                .WhereIF(entity.doctorId>0, (b, s) => b.doctorid==entity.doctorId)
                .WhereIF(entity.startTime != null, (b, s) => b.balancedate >= entity.startTime)
                .WhereIF(entity.endTime != null, (b, s) => b.balancedate <= entity.endTime)
                .Select((b, s)=>new FalanceModel{ balanceid=b.balanceid, accountpay =b.accountpay, actualamount=b.actualamount, alipay=b.alipay, balancedate=b.balancedate, bankpay=b.bankpay, cashpay=b.cashpay , checkoutid=b.checkoutid,  deptname=b.deptname,  doctorname=b.doctorname, source=b.source,  operator_name=b.operator_name, shouldamount=b.shouldamount, summay =b.summay, storeName=s.name, couponpay=b.couponpay })
                .OrderBy(entity.order + orderTypeStr)
                .WithCache()
                .ToPageAsync(entity.page, entity.limit);
        }

        /// <summary>
        /// 收账单分页数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Page<CheckOutModelPage>> GetCheckOutPage(CheckOutSearch entity)
        {
            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }

            var checkoutList = await Db.Queryable<f_checkout,p_store>((c,s)=>new object[] {JoinType.Left,c.store_id==s.id })
                .Where((c, s) => c.orgid == userInfo.org_id)
                .WhereIF(entity.storeId > 0, (c, s) => c.store_id == entity.storeId)
                .WhereIF(entity.startTime != null, (c, s) => c.settle_date >= entity.startTime)
                .WhereIF(entity.endTime != null, (c, s) => c.settle_date <= entity.endTime)
                .Select((c, s)=>new CheckOutModelPage { settle_accounts_id=c.settle_accounts_id, accountpay =c.accountpay, alipay=c.alipay, bankpay=c.bankpay, numbers=c.numbers, operator_name=c.operator_name, otherpay=c.otherpay, recorddate=c.recorddate ,summay=c.summay, storeName=s.name, total_cashpay=c.total_cashpay, total_money=c.total_money, wechatpay=c.wechatpay, orgid=c.orgid, settle_date=c.settle_date, couponpay=c.couponpay })
                .OrderBy(entity.order + orderTypeStr)
                .WithCache()
                .ToPageAsync(entity.page, entity.limit);

            return checkoutList;
        }

        /// <summary>
        /// 根据结账id获取结算分页数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Page<f_balance>> GetBalancePage(FalanceSearch entity)
        {
            if (entity.checkId<=0)
            {
                throw new MessageException("请选择结账单！");
            }
            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            return await Db.Queryable<f_balance>()
               .Where(w => w.orgid == userInfo.org_id && w.stateid == 19 && w.returnid == 0 && w.checkoutid > 0 && w.balancedate != null&&w.checkoutid==entity.checkId)
               .WhereIF(entity.storeId > 0, w => w.store_id == entity.storeId)
               .WhereIF(entity.deptId > 0, w => w.deptid == entity.deptId)
               .WhereIF(entity.doctorId > 0, w => w.doctorid == entity.doctorId)
               .WhereIF(entity.startTime != null, w => w.balancedate >= entity.startTime)
               .WhereIF(entity.endTime != null, w => w.balancedate <= entity.endTime)
               .OrderBy(entity.order + orderTypeStr)
               .WithCache()
               .ToPageAsync(entity.page, entity.limit);
        }

        /// <summary>
        /// 交账
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> AccoutAsync(CheckOutModel entity)
        {
            if (entity.storeId<=0)
            {
                throw new MessageException("未获取到门店！");
            }
            //结算单List
            var balanceList = new List<f_balance>();
            //如果勾选结算单
            if (entity.balanceArray.Count>0)
            {
                balanceList = await Db.Queryable<f_balance>()
                                       .Where(w => w.orgid == userInfo.org_id && w.stateid == 19 && w.returnid == 0 && w.checkoutid == 0&& entity.balanceArray.Contains(w.balanceid) && w.store_id == entity.storeId)
                                       .Select(w => new f_balance { balanceid = w.balanceid, cashpay = w.cashpay, accountpay = w.accountpay, insurancepay = w.insurancepay, wechatpay = w.wechatpay, alipay = w.alipay, bankpay = w.bankpay, otherpay = w.otherpay, store_id=w.store_id , couponpay=w.couponpay})
                                       .WithCache().ToListAsync();
            }
            else
            {
                //结算单为空则修改当前时间段结算单,结束时间不为空则以结束时间为准
                if (entity.endTime==null)
                {
                    entity.endTime = DateTime.Now;
                }
              
                //按门店、时间查询结算单
                 balanceList = await Db.Queryable<f_balance>()
                                        .Where(w => w.orgid == userInfo.org_id && w.stateid == 19 && w.returnid == 0 && w.checkoutid == 0 && w.balancedate != null && w.store_id == entity.storeId)
                                        .WhereIF(entity.startTime!=null,$" to_char( balancedate, 'yyyy-MM-dd')>='{entity.startTime.ToString("yyyy-MM-dd")}'")
                                        .WhereIF(entity.startTime != null, $" to_char( balancedate, 'yyyy-MM-dd')<='{entity.endTime.ToString("yyyy-MM-dd")}'")
                                        .WhereIF(entity.startTime == null&&entity.endTime != null, $" to_char( balancedate, 'yyyy-MM-dd')='{entity.endTime.ToString("yyyy-MM-dd")}'")
                                        .Select(w=>new f_balance{balanceid= w.balanceid,cashpay= w.cashpay, accountpay=w.accountpay, insurancepay=w.insurancepay, wechatpay=w.wechatpay, alipay=w.alipay, bankpay=w.bankpay, otherpay=w.otherpay, couponpay=w.couponpay })
                                        .WithCache().ToListAsync();
                
            }

            if (balanceList == null||balanceList.Count<=0)
            {
                throw new MessageException("无可收账结算单！");
            }
            entity.balanceArray =new List<int>();
            var checkoutEntity = new f_checkout();
            var i = 0;
            //获取结算单id
            foreach (var item in balanceList)
            {
                checkoutEntity.numbers =Convert.ToInt16( i+1);
                entity.balanceArray.Add(item.balanceid);
                checkoutEntity.total_cashpay += item.cashpay;
                checkoutEntity.accountpay += item.accountpay;
                checkoutEntity.insurancepay += item.insurancepay;
                checkoutEntity.wechatpay += item.wechatpay;
                checkoutEntity.alipay += item.alipay;
                checkoutEntity.bankpay += item.bankpay;
                checkoutEntity.otherpay += item.otherpay;
                checkoutEntity.couponpay += item.couponpay;
            }

            //新增结账单
            var result =Db.Ado.UseTran(()=> {
                
                checkoutEntity.orgid = userInfo.org_id;
                checkoutEntity.store_id = entity.storeId;
                checkoutEntity.recorddate = DateTime.Now;
                checkoutEntity.settle_date = DateTime.Now;
                checkoutEntity.operatorid = userInfo.id;
                checkoutEntity.operator_name = userInfo.name;
                checkoutEntity.total_money = checkoutEntity.total_cashpay + checkoutEntity.accountpay + checkoutEntity.insurancepay + checkoutEntity.wechatpay + checkoutEntity.alipay + checkoutEntity.bankpay + checkoutEntity.otherpay+checkoutEntity.couponpay;

                //新增结账单返回id
                var checkoutId = Db.Insertable(checkoutEntity).ExecuteReturnIdentity();
                redisCache.RemoveAll<f_checkout>();


                //修改结算单结账ID
                Db.Updateable<f_balance>().SetColumns(s => s.checkoutid == checkoutId).Where(s => entity.balanceArray.Contains(s.balanceid)).EnableDiffLogEvent().RemoveDataCache().ExecuteCommand();


            });

            return result.IsSuccess;
        }
    }
}
