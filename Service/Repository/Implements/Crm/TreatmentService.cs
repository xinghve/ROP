using Models.DB;
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
    /// 诊疗方式
    /// </summary>
    public class TreatmentService : DbContext, ITreatmentService
    {
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <returns></returns>
        public async Task<int> AddAsync(c_treatment treatment)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            //判断是否存在
            var isExisteName = await Db.Queryable<c_treatment>().WithCache().AnyAsync(a => a.org_id == userInfo.org_id && a.store_id == treatment.store_id && a.name == treatment.name);
            if (isExisteName)
            {
                throw new MessageException("已存在此诊疗方式");
            }
            treatment.spell = ToSpell.GetFirstPinyin(treatment.name);
            treatment.org_id = userInfo.org_id;
            //新增
            redisCache.RemoveAll<c_treatment>();
            return await Db.Insertable(treatment).ExecuteReturnIdentityAsync();
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <returns></returns>
        public async Task<int> DeleteAsync(List<int> vs)
        {
            return await Db.Deleteable<c_treatment>(vs).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
        }

        /// <summary>
        /// 数据列表
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="store_id">门店ID</param>
        /// <returns></returns>
        public async Task<List<c_treatment>> GetListAsync(string name, int store_id)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            return await Db.Queryable<c_treatment>()
               .Where(w => w.org_id == userInfo.org_id)
               .WhereIF(store_id == 0, w => w.store_id == 0)
               .WhereIF(store_id > 0, w => w.store_id == store_id || w.store_id == 0)
               .WhereIF(!string.IsNullOrEmpty(name), w => w.name.Contains(name) || w.spell.Contains(name))
               .WithCache()
               .ToListAsync();
        }

        /// <summary>
        /// 获得列表
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="store_id">门店ID</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        public async Task<Page<c_treatment>> GetPagesAsync(string name, int store_id, string order, int orderType, int limit, int page)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var orderTypeStr = " Asc";
            if (orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            return await Db.Queryable<c_treatment>()
                .Where(w => w.org_id == userInfo.org_id)
                .WhereIF(store_id == 0, w => w.store_id == 0)
                .WhereIF(store_id > 0, w => w.store_id == store_id || w.store_id == 0)
                .WhereIF(!string.IsNullOrEmpty(name), w => w.name.Contains(name) || w.spell.Contains(name))
                .OrderBy(order + orderTypeStr)
                .WithCache()
                .ToPageAsync(page, limit);
        }

        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <returns></returns>
        public async Task<int> ModifyAsync(c_treatment treatment)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            //判断是否存在
            var isExisteName = await Db.Queryable<c_treatment>().WithCache().AnyAsync(a => a.org_id == userInfo.org_id && a.store_id == treatment.store_id && a.name == treatment.name && a.id != treatment.id);
            if (isExisteName)
            {
                throw new MessageException("已存在此诊疗方式");
            }
            var spell = ToSpell.GetFirstPinyin(treatment.name);
            return await Db.Updateable<c_treatment>()
                .SetColumns(s => new c_treatment { name = treatment.name, spell = spell })
                .Where(w => w.id == treatment.id)
                .RemoveDataCache()
                .EnableDiffLogEvent()
                .ExecuteCommandAsync();
        }
    }
}
