using Models.DB;
using Models.View.Public;
using Service.Extensions;
using Service.Repository.Interfaces.Public;
using SqlSugar;
using System;
using System.Threading.Tasks;
using Tools;
using Tools.Filter;
using Senparc.Weixin.TenPay.V3;
using Senparc.Weixin.MP.Helpers;
using Microsoft.AspNetCore.Http;

namespace Service.Repository.Implements.Public
{
    /// <summary>
    /// 提现管理
    /// </summary>
    public class CashService:DbContext,ICashService
    {

        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <returns></returns>
        public async Task<bool> AddAsync(r_cash_withdrawal entity)
        {
            //获分销人员信息
            var distributorInfo = new Tools.IdentityModels.GetDistributor().Distributor;
            var result = Db.Ado.UseTran(() =>
            {
                //添加提现记录
                entity.cash_withdrawal_date = DateTime.Now;
                entity.distributor_id = distributorInfo.id;
                entity.org_id = distributorInfo.org_id;
                entity.state = 26;
                entity.store_id = distributorInfo.store_id;
                redisCache.RemoveAll<r_cash_withdrawal>();
                Db.Insertable(entity).ExecuteCommand();

                //修改分销人员账户信息
                Db.Updateable<p_distributor>().SetColumns(s => new p_distributor { settleamount = s.settleamount + entity.money, noneamount = s.noneamount - entity.money }).Where(w => w.id == distributorInfo.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 提现分页
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Page<CashExtent>> GetPageAsync(CashSearch entity)
        {
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }

            return await Db.Queryable<r_cash_withdrawal, p_store, p_distributor>((cash, store, dis) => new object[] { JoinType.Left, cash.store_id == store.id, JoinType.Left, cash.distributor_id == dis.id })
                          .Where((cash, store, dis) => cash.org_id == userInfo.org_id)
                          .WhereIF(entity.store_id > 0, (cash, store, dis) => cash.store_id == entity.store_id)
                          .WhereIF(entity.state>0, (cash, store, dis)=>cash.state==entity.state)
                          .WhereIF(entity.startTime != null, (cash, store, dis) => cash.cash_withdrawal_date >= entity.startTime)
                          .WhereIF(entity.endTime != null, (cash, store, dis) => cash.cash_withdrawal_date <= entity.endTime)
                          .WhereIF(entity.audit_startTime != null, (cash, store, dis) => cash.audit_date >= entity.audit_startTime)
                          .WhereIF(entity.audit_endTime != null, (cash, store, dis) => cash.audit_date <= entity.audit_endTime)
                          .WhereIF(entity.finish_startTime != null, (cash, store, dis) => cash.finish_date >= entity.finish_startTime)
                          .WhereIF(entity.finish_endTime != null, (cash, store, dis) => cash.finish_date <= entity.finish_endTime)
                          .WhereIF(!string.IsNullOrEmpty(entity.search_condition), (cash, store, dis) =>  cash.auditor.Contains(entity.search_condition) || dis.name.Contains(entity.search_condition) || dis.phone_no.Contains(entity.search_condition))
                          .Select((cash, store, dis) => new CashExtent { auditor=cash.auditor, auditor_id=cash.auditor_id, state=cash.state, audit_date=cash.audit_date, cash_withdrawal_date=cash.cash_withdrawal_date, distributor_id=cash.distributor_id, distributor_phone=dis.phone_no, money=cash.money, org_id=cash.org_id, remarks=cash.remarks, store_name=store.name, store_id=cash.store_id, distributor_name=dis.name  })
                        .OrderBy(entity.order + orderTypeStr)
                        .WithCache()
                        .ToPageAsync(entity.page, entity.limit);
        }

        /// <summary>
        /// 提现分页记录(分销人员端)
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> GetPagesByDistributorAsync(Search entity)
        {
            //获分销人员信息
            var distributorInfo = new Tools.IdentityModels.GetDistributor().Distributor;
            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            return await Db.Queryable<r_cash_withdrawal>()
                          .Where(w => w.distributor_id == distributorInfo.id)
                          .Select(s => new { s.cash_withdrawal_date, s.finish_date, s.money, s.remarks, s.state })
                        .OrderBy(entity.order + orderTypeStr)
                        .WithCache()
                        .ToPageAsync(entity.page, entity.limit);
        }

        /// <summary>
        /// 提现审核
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> Check(r_cash_withdrawal entity)
        {
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            if (entity==null||entity.store_id<=0||entity.distributor_id<=0||entity.cash_withdrawal_date==null)
            {
                throw new MessageException("传入数据不正确！");
            }

            return Db.Updateable<r_cash_withdrawal>()
                                      .SetColumns(w => new r_cash_withdrawal { state = 29, audit_date = DateTime.Now, auditor_id = userInfo.id, auditor = userInfo.name, remarks=entity.remarks })
                                      .Where(w => w.org_id == userInfo.org_id && w.distributor_id == entity.distributor_id && w.store_id == entity.store_id && w.cash_withdrawal_date == entity.cash_withdrawal_date)
                                      .RemoveDataCache()
                                      .EnableDiffLogEvent()
                                      .ExecuteCommand();

        }

        /// <summary>
        /// 转账
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> Transfer(r_cash_withdrawal entity)
        {
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            if (entity == null || entity.store_id <= 0 || entity.distributor_id <= 0 || entity.cash_withdrawal_date == null)
            {
                throw new MessageException("传入数据不正确！");
            }

            return Db.Updateable<r_cash_withdrawal>()
                                      .SetColumns(w => new r_cash_withdrawal { state = 27, finish_date = DateTime.Now, transfer_id = userInfo.id, transfer = userInfo.name })
                                      .Where(w => w.org_id == userInfo.org_id && w.distributor_id == entity.distributor_id && w.store_id == entity.store_id && w.cash_withdrawal_date == entity.cash_withdrawal_date)
                                      .RemoveDataCache()
                                      .EnableDiffLogEvent()
                                      .ExecuteCommand();

        }

        /// <summary>
        /// 编辑上下限
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> ModifyLowerUper(CashLower entity)
        {
            if (entity.cash_lower <= 0 || entity.cash_uper <= 0)
            {
                throw new MessageException("提现上下限设置不正确！");
            }
            if (entity.cash_lower >entity.cash_uper)
            {
                throw new MessageException("提现下限不能大于下限！");
            }
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            //看是否为机构管理员
            var isAdmin = await Db.Queryable<p_employee_role>()
                                .Where(w => w.org_id == userInfo.org_id && w.store_id == 0 && w.employee_id == userInfo.id)
                                .Select(w => w.is_admin)
                                .WithCache()
                                .FirstAsync();

            if (!isAdmin)
            {
                throw new MessageException("没有权限！");
            }

            return await Db.Updateable<p_org>()
                           .SetColumns(w => new p_org { cash_uper = entity.cash_uper, cash_lower = entity.cash_lower })
                           .Where(w => w.id == userInfo.org_id)
                           .RemoveDataCache()
                           .EnableDiffLogEvent()
                           .ExecuteCommandAsync();

        }
        /// <summary>
        /// 获取提现上下限
        /// </summary>
        /// <returns></returns>
        public async Task<object> GetLowerUper()
        {
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var cash = await Db.Queryable<p_org>()
                           .Where(w => w.id == userInfo.org_id)
                           .Select(w => new { lower = w.cash_lower, uper = w.cash_uper })
                           .WithCache()
                           .FirstAsync();

            return new { cash.lower,  cash.uper };
        }
    }
}
