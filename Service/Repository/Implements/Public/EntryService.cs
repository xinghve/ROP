using Models.DB;
using Service.Extensions;
using Service.Repository.Interfaces.Public;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tools;
using Tools.Filter;

namespace Service.Repository.Implements.Public
{
    /// <summary>
    /// 词条
    /// </summary>
    public class EntryService : DbContext, IEntryService
    {
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <returns></returns>
        public async Task<int> AddAsync(p_entry entry)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            //判断是否存在
            var isExisteName = await Db.Queryable<p_entry>().WithCache().AnyAsync(a => a.employee_id == userInfo.id && a.text == entry.text);
            if (isExisteName)
            {
                throw new MessageException("已存在此词条");
            }
            entry.employee_id = userInfo.id;
            entry.code = DateTime.Now.ToString("yyyyMMddHHmmssffff");
            return await Db.Insertable(entry).RemoveDataCache().ExecuteCommandAsync();
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <returns></returns>
        public async Task<int> DeleteAsync(List<string> vs)
        {
            return await Db.Deleteable<p_entry>(vs).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
        }

        /// <summary>
        /// 数据列表
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public async Task<List<p_entry>> GetListAsync(string name)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            return await Db.Queryable<p_entry>()
                .Where(w => w.employee_id == userInfo.id)
                .WhereIF(!string.IsNullOrEmpty(name), w => w.text.Contains(name))
                .WithCache()
                .ToListAsync();
        }

        /// <summary>
        /// 获得列表
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        public async Task<Page<p_entry>> GetPagesAsync(string name, string order, int orderType, int limit, int page)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var orderTypeStr = " Asc";
            if (orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            return await Db.Queryable<p_entry>()
                .Where(w => w.employee_id == userInfo.id)
                .WhereIF(!string.IsNullOrEmpty(name), w => w.text.Contains(name))
                .OrderBy(order + orderTypeStr)
                .WithCache()
                .ToPageAsync(page, limit);
        }

        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <returns></returns>
        public async Task<int> ModifyAsync(p_entry entry)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            //判断是否存在
            var isExisteName = await Db.Queryable<p_entry>().WithCache().AnyAsync(a => a.employee_id == userInfo.id && a.text == entry.text && a.code != entry.code);
            if (isExisteName)
            {
                throw new MessageException("已存在此词条");
            }
            return await Db.Updateable<p_entry>()
                .SetColumns(s => new p_entry { text = entry.text })
                .Where(w => w.code == entry.code)
                .RemoveDataCache()
                .EnableDiffLogEvent()
                .ExecuteCommandAsync();
        }
    }
}
