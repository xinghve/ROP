using Models.DB;
using Models.View.Public;
using Service.Repository.Interfaces.Public;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;
using Tools.Filter;

namespace Service.Repository.Implements.Public
{
    /// <summary>
    /// 排班时间段
    /// </summary>
    public class SchedulingTimeService : DbContext, ISchedulingTimeService
    {
        /// <summary>
        /// 设置排班时间段
        /// </summary>
        /// <returns></returns>
        public bool SetAsync(SchedulingTimes entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            var list = new List<p_scheduling_time>();

            foreach (var item in entity.times)
            {
                if (item.start >= item.end)
                {
                    throw new MessageException("开始时间必须小于结束时间");
                }
                //判断是否存在
                var isExisteName = list.Any(a => item.start >= a.start && item.start <= a.end || item.end >= a.start && item.end <= a.end || a.start >= item.start && a.start <= item.end || a.end >= item.start && a.end <= item.end);
                if (isExisteName)
                {
                    throw new MessageException("时间段不允许存在交集");
                }
                list.Add(new p_scheduling_time { org_id = userInfo.org_id, store_id = entity.store_id, start = item.start, end = item.end });
            }

            //事务创建
            var result = Db.Ado.UseTran(() =>
            {
                var ret = -1;
                //删除门店排班时间段
                ret = Db.Deleteable<p_scheduling_time>().Where(w => w.org_id == userInfo.org_id && w.store_id == entity.store_id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                if (ret != -1)
                {
                    //新增
                    if (list.Count == 1)
                    {
                        Db.Ado.ExecuteCommand($"insert into p_scheduling_time values(@org_id,@store_id,'{list[0].start}','{list[0].end}')", new { list[0].org_id, list[0].store_id });
                    }
                    else if (list.Count > 1)
                    {
                        Db.Insertable(list).ExecuteCommand();
                    }
                    redisCache.RemoveAll<p_scheduling_time>();
                }
            });
            return result.IsSuccess;
        }

        /// <summary>
        /// 获取排班时间段
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <returns></returns>
        public async Task<List<Time>> GetListAsync(int store_id)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var list = await Db.Queryable<p_scheduling_time>().Where(w => w.org_id == userInfo.org_id && w.store_id == store_id).Select(s => new Time { start = s.start, end = s.end }).WithCache().ToListAsync();
            if (list.Count == 0)
            {
                list = await Db.Queryable<p_scheduling_time>().Where(w => w.org_id == 0 && w.store_id == 0).Select(s => new Time { start = s.start, end = s.end }).WithCache().ToListAsync();
            }
            return list;
        }
    }
}
