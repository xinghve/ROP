using Models.DB;
using Models.View.Crm;
using Service.Extensions;
using Service.Repository.Interfaces.Crm;
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
    /// 回访记录
    /// </summary>
    public class PracticeService : DbContext, IPracticeService
    {
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <returns></returns>
        public async Task<int> AddAsync(r_practice entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var archives = await Db.Queryable<c_archives>().Where(w => w.id == entity.archives_id).WithCache().FirstAsync();
            entity.archives = archives.name;
            entity.archives_phone = archives.phone;
            entity.creater = userInfo.name;
            entity.creater_id = userInfo.id;
            entity.create_date = DateTime.Now;
            entity.id_card = archives.id_card;
            entity.org_id = userInfo.org_id;
            entity.modify_date = DateTime.Now;
            entity.task_date = DateTime.Now;
            entity.execute_date = DateTime.Now;
            entity.executor = userInfo.name;
            entity.executor_id = userInfo.id;
            entity.state = 17;
            return await Db.Insertable(entity).RemoveDataCache().ExecuteCommandAsync();
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <returns></returns>
        //public async Task<int> DeleteAsync(List<int> vs)
        //{
        //    return await Db.Deleteable<r_practice>(vs).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
        //}

        /// <summary>
        /// 获得列表
        /// </summary>
        /// <param name="name">档案姓名/手机号/身份证</param>
        /// <param name="dateTimeStart">计划日期（开始）</param>
        /// <param name="dateTimeEnd">计划日期（结束）</param>
        /// <param name="store_id">门店ID</param>
        /// <param name="executor_id">执行人ID</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        public async Task<Page<r_practice>> GetPagesAsync(string name, string dateTimeStart, string dateTimeEnd, int store_id, int executor_id, string order, int orderType, int limit, int page)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var orderTypeStr = " Asc";
            if (orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            var start = DateTime.Parse("1949-01-01 00:00:00");
            if (!string.IsNullOrEmpty(dateTimeStart))
            {
                start = DateTime.Parse(dateTimeStart + " 00:00:00");
            }
            var end = DateTime.Parse("3000-12-31 23:59:59");
            if (!string.IsNullOrEmpty(dateTimeEnd))
            {
                end = DateTime.Parse(dateTimeEnd + " 23:59:59");
            }
            return await Db.Queryable<r_practice>()
                .Where(w => w.org_id == userInfo.org_id)
                //.Where(w => SqlSugar.SqlFunc.Between(w.execute_date, start, end))
                .WhereIF(!string.IsNullOrEmpty(dateTimeStart), w => w.execute_date >=start)
                .WhereIF(!string.IsNullOrEmpty(dateTimeEnd), w => w.execute_date <= end)
                .WhereIF(store_id > 0, w => w.store_id == store_id)
                .WhereIF(!string.IsNullOrEmpty(name), w => w.archives.Contains(name) || w.archives_phone.Contains(name) || w.id_card.Contains(name))
                .OrderBy(order + orderTypeStr)
                .WithCache()
                .ToPageAsync(page, limit);
        }

        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <returns></returns>
        public async Task<int> ModifyAsync(r_practice entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            //查询创建人跟状态
            var creator = await Db.Queryable<r_practice>().Where(s => s.id == entity.id).Select(s=>new { s.creater_id,s.state }).WithCache().FirstAsync();


            if (creator.creater_id!=userInfo.id)
            {
                throw new MessageException("不能修改他人创建的回访!");
            }
            if (creator.state==17)
            {
                throw new MessageException("已执行不能修改!");
            }

            return await Db.Updateable(entity).SetColumns(s => new r_practice { content = entity.content, answer = entity.answer,modify_date=DateTime.Now }).Where(w => w.id == entity.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <returns></returns>
        public async Task<int> ExecuteAsync(int id,string answer,string content)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
          
            //修改回访计划
            return await Db.Updateable<r_practice>().SetColumns(s => new r_practice { state = 17, executor_id = userInfo.id, execute_date = DateTime.Now, executor = userInfo.name, answer=answer, modify_date = DateTime.Now }).Where(w => w.id == id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync(); ;
        }
    }
}
