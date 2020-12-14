using Models.DB;
using Models.View.Public;
using Service.Extensions;
using Service.Repository.Interfaces.Public;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tools;
using Tools.Filter;

namespace Service.Repository.Implements.Public
{
    /// <summary>
    /// 提成记录
    /// </summary>
    public class AmountService : DbContext, IAmountService
    {
        public async Task<Page<AmountExpent>> GetPageAsync(AmountSearch entity)
        {
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }

            return await Db.Queryable<r_amount, p_store, p_distributor>((amount, store, dis) => new object[] { JoinType.Left, amount.store_id == store.id, JoinType.Left, amount.distributor_id == dis.id })
                          .Where((amount, store, dis) => amount.org_id == userInfo.org_id)
                          .WhereIF(entity.store_id > 0, (amount, store, dis) => amount.store_id == entity.store_id)
                          .WhereIF(entity.startTime != null, (amount, store, dis) => amount.amount_date >= entity.startTime)
                          .WhereIF(entity.endTime != null, (amount, store, dis) => amount.amount_date <= entity.endTime)
                          .WhereIF(!string.IsNullOrEmpty(entity.search_condition), (amount, store, dis) => amount.card_no.Contains(entity.search_condition) || amount.archives.Contains(entity.search_condition) || amount.archives_phone.Contains(entity.search_condition) || amount.distributor.Contains(entity.search_condition) || dis.phone_no.Contains(entity.search_condition))
                          .Select((amount, store, dis) => new AmountExpent { amount_date = amount.amount_date, archives = amount.archives, archives_id = amount.archives_id, archives_phone = amount.archives_phone, card_no = amount.card_no, distributor = amount.distributor, distributor_phone = dis.phone_no, money = amount.money, store_id = amount.store_id, store_name = store.name })
                        .OrderBy(entity.order + orderTypeStr)
                        .WithCache()
                        .ToPageAsync(entity.page, entity.limit);
        }

        /// <summary>
        /// 提成记录分页(分销人员端)
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> GetPageByDistributorAsync(Search entity)
        {
            //获分销人员信息
            var distributorInfo = new Tools.IdentityModels.GetDistributor().Distributor;
            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            return await Db.Queryable<r_amount>()
                .Where(w => w.distributor_id == distributorInfo.id)
                .Select(s => new { s.amount_date, s.archives, s.money })
                .OrderBy(entity.order + orderTypeStr)
                .WithCache()
                .ToPageAsync(entity.page, entity.limit);
        }

        /// <summary>
        /// 分销人员提成比例设置
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> SetRate(AmountRate entity)
        {
            if (entity.royalty_rate<0||entity.royalty_rate>100)
            {
                throw new MessageException("提成比例不正确！");
            }
            //获取人员信息
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
            //修改提成比例
            return await Db.Updateable<p_org>()
                           .SetColumns(w => w.royalty_rate == entity.royalty_rate)
                           .Where(w => w.id == userInfo.org_id)
                           .RemoveDataCache()
                           .EnableDiffLogEvent()
                           .ExecuteCommandAsync();
            

        }

        /// <summary>
        /// 获取分销人员提成比例
        /// </summary>
        /// <returns></returns>
        public async Task<object> GetRate()
        {
            //获取人员信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var rate = await Db.Queryable<p_org>()
                             .Where(w => w.id == userInfo.org_id)
                             .Select(w=>w.royalty_rate)
                             .WithCache()
                             .FirstAsync();

            return new {rate};

        }
    }
}
