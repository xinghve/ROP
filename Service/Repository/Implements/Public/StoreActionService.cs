using Models.DB;
using Models.View.Public;
using Newtonsoft.Json;
using Service.Repository.Interfaces.Public;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace Service.Repository.Implements.Public
{
    /// <summary>
    /// 门店权限
    /// </summary>
    public class StoreActionService : DbContext, IStoreActionService
    {
        /// <summary>
        /// 获取选中角色权限
        /// </summary>
        /// <param name="store_id">门店id</param>
        /// <returns></returns>
        public async Task<object> GetAsync(int store_id)
        {
            //查询角色已有权限
            var lst = await Db.Queryable<p_store_action, p_action>((ra, a) => new object[] { JoinType.Left, ra.action_id == a.id })
                .Where((ra, a) => ra.store_id == store_id && a.is_action == 2)
                .Select((ra, a) => new { a.id, a.name })
                .Distinct()
                .WithCache()
                .ToListAsync();
            var jsonStr = "[";
            if (lst.Count > 0)
            {
                foreach (var item in lst)
                {
                    jsonStr += "{";
                    jsonStr += "id: " + item.id + ",";
                    jsonStr += "label: '" + item.name + "'";
                    jsonStr += "},";
                }
                jsonStr = jsonStr.TrimEnd(',');
            }
            jsonStr = jsonStr.TrimEnd(',');
            jsonStr += "]";
            return JsonConvert.DeserializeObject(jsonStr);
        }

        /// <summary>
        /// 设置角色权限
        /// </summary>
        /// <param name="storeAction"></param>
        /// <returns></returns>
        public bool Set(StoreAction storeAction)
        {
            //事务创建
            var result = Db.Ado.UseTran(() =>
            {
                var ret = -1;
                //删除角色已有权限
                ret = Db.Deleteable<p_store_action>().Where(w => w.store_id == storeAction.store_id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                if (ret != -1)
                {
                    //获取需要添加的权限
                    var addlst = storeAction.actions.Select(s => new p_store_action { action_id = s, store_id = storeAction.store_id }).ToList();
                    //添加角色权限
                    Db.Insertable(addlst).ExecuteCommand();
                    redisCache.RemoveAll<p_store_action>();
                }
            });
            return result.IsSuccess;
        }
    }
}
