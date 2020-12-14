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

namespace Service.Repository.Implements.Public
{
    /// <summary>
    /// 投诉
    /// </summary>
    public class ComplaintService : DbContext, IComplaintService
    {
        /// <summary>
        /// 添加一条数据（客户端）
        /// </summary>
        /// <returns></returns>
        public async Task<int> AddAsync(r_complaint entity)
        {
            //获客户信息
            var archivesInfo = new Tools.IdentityModels.GetArchives().archives;
            entity.archives = archivesInfo.name;
            entity.archives_id = archivesInfo.id;
            entity.archives_phone = archivesInfo.phone;
            entity.date = DateTime.Now;
            entity.org_id = archivesInfo.org_id;
            entity.state = 24;
            redisCache.RemoveAll<r_complaint>();
            return await Db.Insertable(entity).ExecuteReturnIdentityAsync();
        }

        /// <summary>
        /// 处理
        /// </summary>
        /// <returns></returns>
        public async Task<int> DealAsync(r_complaint entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            return await Db.Updateable<r_complaint>().SetColumns(s => new r_complaint { state = 25, dealer = userInfo.name, dealer_id = userInfo.id, deal_time = DateTime.Now, result = entity.result }).Where(w => w.id == entity.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
        }

        /// <summary>
        /// 数据列表（客户端）
        /// </summary>
        /// <param name="state">状态</param>
        /// <returns></returns>
        public async Task<List<r_complaint>> GetListAsync(int state)
        {
            //获客户信息
            var archivesInfo = new Tools.IdentityModels.GetArchives().archives;
            return await Db.Queryable<r_complaint>().Where(w => w.archives_id == archivesInfo.id).WhereIF(state > 0, w => w.state == state).WithCache().ToListAsync();
        }

        /// <summary>
        /// 获得分页列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Page<ComplaintPageModel>> GetPagesAsync(ComplaintSearch entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            return await Db.Queryable<r_complaint,p_store>((c,p)=>new object[] {JoinType.Left, c.store_id==p.id })
                .Where((c, p) => c.org_id == userInfo.org_id)
                .WhereIF(entity.store_id > 0, (c, p) => c.store_id == entity.store_id)
                .WhereIF(entity.state>0, (c, p) => c.state==entity.state)
                .WhereIF(!string.IsNullOrEmpty(entity.archives), (c, p) => c.archives.Contains(entity.archives) || c.archives_phone.Contains(entity.archives))
                 .WhereIF(entity.start_date != null, (c, p) => c.date >= entity.start_date)
                .WhereIF(entity.end_date != null, (c, p) => c.date <= entity.end_date)
                .Select((c, p) => new ComplaintPageModel { storeName=p.name, archives=c.archives, archives_id=c.archives_id, archives_phone=c.archives_phone , content=c.content, id=c.id, store_id=c.store_id, date=c.date, dealer=c.dealer ,dealer_id=c.dealer_id, deal_time=c.deal_time, org_id=c.org_id, result=c.result, state=c.state })
                .OrderBy(entity.order + orderTypeStr)
                .WithCache()
                .ToPageAsync(entity.page, entity.limit);
        }
    }
}
