using Models.DB;
using Service.Extensions;
using Service.Repository.Interfaces.Public;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace Service.Repository.Implements.Public
{
    /// <summary>
    /// 短信模板
    /// </summary>
    public class SmsTemplateService : DbContext, ISmsTemplateService
    {
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <returns></returns>
        public async Task<int> AddAsync(p_sms_template entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            entity.create_time = DateTime.Now;
            entity.org_id = userInfo.org_id;
            return await Db.Insertable(entity).RemoveDataCache().ExecuteCommandAsync();
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <returns></returns>
        public async Task<int> DeleteAsync(List<int> vs)
        {
            return await Db.Deleteable<p_sms_template>(vs).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
        }

        /// <summary>
        /// 数据列表
        /// </summary>
        /// <param name="name">模版名称</param>
        /// <param name="is_select">是否可选（1=可选，0=不可选，-1=所有）</param>
        /// <returns></returns>
        public async Task<List<p_sms_template>> GetListAsync(string name, int is_select)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            return await Db.Queryable<p_sms_template>()
                //.Where(w => w.org_id == userInfo.org_id)
                .WhereIF(is_select != -1, w => w.is_select == is_select)
                .WhereIF(!string.IsNullOrEmpty(name), w => w.name.Contains(name))
                .WithCache()
                .ToListAsync();
        }

        /// <summary>
        /// 获得列表
        /// </summary>
        /// <param name="name">模版名称</param>
        /// <param name="is_select">是否可选（1=可选，0=不可选，-1=所有）</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        public async Task<Page<p_sms_template>> GetPagesAsync(string name, int is_select, string order, int orderType, int limit, int page)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var orderTypeStr = " Asc";
            if (orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            return await Db.Queryable<p_sms_template>()
                //.Where(w => w.org_id == userInfo.org_id)
                .WhereIF(is_select != -1, w => w.is_select == is_select)
                .WhereIF(!string.IsNullOrEmpty(name), w => w.name.Contains(name))
                .OrderBy(order + orderTypeStr)
                .WithCache()
                .ToPageAsync(page, limit);
        }

        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <returns></returns>
        public async Task<int> ModifyAsync(p_sms_template entity)
        {
            return await Db.Updateable(entity).IgnoreColumns(it => new { it.create_time, it.org_id }).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
        }
    }
}
