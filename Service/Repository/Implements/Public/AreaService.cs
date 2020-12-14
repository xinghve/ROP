using Models.DB;
using Service.Extensions;
using Service.Repository.Interfaces.Public;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tools;
using Tools.Filter;

namespace Service.Repository.Implements.Public
{
    /// <summary>
    /// 区域
    /// </summary>
    public class AreaService : DbContext, IAreaService
    {
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        public async Task<int> AddAsync(b_area area)
        {
            //判断是否存在
            var isExisteName = await Db.Queryable<b_area>().WithCache().AnyAsync(a => a.parent_id == area.parent_id && a.name == area.name);
            if (isExisteName)
            {
                throw new MessageException("当前层级已存在此名称");
            }

            //新增
            redisCache.RemoveAll<b_area>();
            return await Db.Insertable(area).ExecuteReturnIdentityAsync();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="vs"></param>
        /// <returns></returns>
        public async Task<int> DeleteAsync(List<int> vs)
        {
            return await Db.Deleteable<b_area>(vs).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parentNum"></param>
        /// <returns></returns>
        public async Task<List<b_area>> GetListAsync(string name, int parentNum)
        {
            return await Db.Queryable<b_area>()
                .Where(w => w.parent_id == parentNum)
                .WhereIF(!string.IsNullOrEmpty(name), w => w.name.Contains(name))
                .WithCache()
                .ToListAsync();
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="name"></param>
        /// <param name="order"></param>
        /// <param name="orderType"></param>
        /// <param name="limit"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public async Task<Page<b_area>> GetPagesAsync(string name, string order, int orderType, int limit, int page)
        {
            var orderTypeStr = " Asc";
            if (orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            return await Db.Queryable<b_area>()
                .WhereIF(!string.IsNullOrEmpty(name), w => w.name.Contains(name))
                .OrderBy(order + orderTypeStr)
                .WithCache()
                .ToPageAsync(page, limit);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        public async Task<int> ModifyAsync(b_area area)
        {
            //判断是否存在
            var isExisteName = await Db.Queryable<b_area>().WithCache().AnyAsync(a => a.parent_id == area.parent_id && a.name == area.name && a.id != area.id);
            if (isExisteName)
            {
                throw new MessageException("当前层级已存在此名称");
            }
            return await Db.Updateable(area).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
        }
    }
}
