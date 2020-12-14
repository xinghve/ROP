using Models.DB;
using Models.View.Crm;
using Service.Extensions;
using Service.Repository.Interfaces.Crm;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Tools;
using Tools.Filter;

namespace Service.Repository.Implements.Crm
{
    /// <summary>
    /// 随访计划
    /// </summary>
    public class FollowService : DbContext, IFollowService
    {
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <returns></returns>
        //public async Task<int> AddAsync(r_follow_up entity)
        //{
        //    //获取用户信息
        //    var userInfo = new Tools.IdentityModels.GetUser().userInfo;
        //    var archives = await Db.Queryable<c_archives>().Where(w => w.id == entity.archives_id).WithCache().FirstAsync();
        //    entity.archives = archives.name;
        //    entity.archives_phone = archives.phone;
        //    entity.creater = userInfo.name;
        //    entity.creater_id = userInfo.id;
        //    entity.create_date = DateTime.Now;
        //    entity.id_card = archives.id_card;
        //    entity.org_id = userInfo.org_id;
        //    entity.state = 0;
        //    return await Db.Insertable(entity).RemoveDataCache().ExecuteCommandAsync();
        //}

        /// <summary>
        /// 委托
        /// </summary>
        /// <returns></returns>
        public async Task<int> ClientAsync(r_follow_up entity)
        {
            return await Db.Updateable(entity).SetColumns(s => new r_follow_up { client_id = entity.client_id, client = entity.client, client_date = DateTime.Now }).Where(w => w.id == entity.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <returns></returns>
        //public async Task<int> DeleteAsync(List<int> vs)
        //{
        //    return await Db.Deleteable<r_follow_up>(vs).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
        //}

        /// <summary>
        /// 执行
        /// </summary>
        /// <returns></returns>
        public async Task<int> ExecuteAsync(FollowExecute followExecute)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            //查询客户负责人
            var isDirector = await Db.Queryable<c_archives,r_follow_up>((a,f)=>new object[] {JoinType.Left,f.archives_id==a.id })
                                 .Where((a, f) =>f.id==followExecute.id)
                                 .Select((a, f) => new { director=a.to_director_id, client=f.client_id })
                                 .WithCache()
                                 .FirstAsync();

            if (isDirector == null || (isDirector.director != userInfo.id&&isDirector.client!=userInfo.id))
            {
                throw new MessageException("不是该客户的负责人!");
            }

            //修改随访计划
            return await Db.Updateable<r_follow_up>().SetColumns(s => new r_follow_up { state = 17, executor_id = userInfo.id, execute_date = DateTime.Now, executor = userInfo.name, answer=followExecute.answer }).Where(w => w.id == followExecute.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
        }

        /// <summary>
        /// 获得列表
        /// </summary>
        /// <returns></returns>
        public async Task<Page<FollowUpModel>> GetPagesAsync(FollowUpSearch entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            var start = DateTime.Parse("1949-01-01 00:00:00");
            if (!string.IsNullOrEmpty(entity.dateTimeStart))
            {
                start = DateTime.Parse(entity.dateTimeStart + " 00:00:00");
            }
            var end = DateTime.Parse("3000-12-31 23:59:59");
            if (!string.IsNullOrEmpty(entity.dateTimeEnd))
            {
                end = DateTime.Parse(entity.dateTimeEnd + " 23:59:59");
            }

            return await Db.Queryable<r_follow_up,p_store,c_archives>((f,s,a)=>new object[] { JoinType.Left, s.id==f.store_id,JoinType.Left,a.id==f.archives_id})
                .Where((f, s, a) => f.org_id == userInfo.org_id)
                .Where((f, s, a) => SqlSugar.SqlFunc.Between(f.task_date, start, end))
                .WhereIF(entity.all !=-1, (f, s, a) => f.client_id == userInfo.id || f.creater_id == userInfo.id||a.to_director_id==userInfo.id)
                .WhereIF(entity.state>0, (f, s, a) => f.state == entity.state)
                .WhereIF(entity.storeID > 0, (f, s, a) => f.store_id == entity.storeID)
                .WhereIF(!string.IsNullOrEmpty(entity.name), (f, s, a) => f.archives.Contains(entity.name) || f.archives_phone.Contains(entity.name) || f.id_card.Contains(entity.name))
                .Select((f, s, a) => new FollowUpModel { wait_dir=f.client==null?a.to_director: f.client, storeName =s.name,  answer=f.answer, archives=f.archives, task_date=f.task_date, archives_id=f.archives_id, archives_phone=f.archives_phone, client=f.client, client_date=f.client_date , client_id=f.client_id, content=f.content, creater=f.creater , create_date=f.create_date, execute_date=f.execute_date , executor=f.executor, id=f.id, id_card=f.id_card , org_id=f.org_id, state=f.state , store_id=f.store_id, executor_id=f.executor_id , creater_id=f.creater_id})
                .OrderBy(entity.order + orderTypeStr)
                .WithCache()
                .ToPageAsync(entity.page, entity.limit);
        }

        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <returns></returns>
        public async Task<int> ModifyAsync(r_follow_up entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            //查询创建人
            var creator = await Db.Queryable<r_follow_up>().Where(s => s.id == entity.id).Select(s =>new { s.creater_id,s.state }).WithCache().FirstAsync();
            if (creator.creater_id != userInfo.id)
            {
                throw new MessageException("不能修改他人创建的随访!");
            }
            if (creator.state==17)
            {
                throw new MessageException("已执行不能修改!");
            }

            return await Db.Updateable(entity).SetColumns(s => new r_follow_up { content = entity.content, task_date = entity.task_date,answer=entity.answer }).Where(w => w.id == entity.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
        }
    }
}
