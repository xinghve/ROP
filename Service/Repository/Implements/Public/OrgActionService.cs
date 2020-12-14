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
    /// 集团权限
    /// </summary>
    public class OrgActionService : DbContext, IOrgActionService
    {
        /// <summary>
        /// 获取选中集团权限
        /// </summary>
        /// <param name="org_id">集团id</param>
        /// <returns></returns>
        public async Task<object> GetAsync(int org_id)
        {
            //查询角色已有权限
            var lst = await Db.Queryable<p_org_action, p_action>((ra, a) => new object[] { JoinType.Left, ra.action_id == a.id })
                .Where((ra, a) => ra.org_id == org_id && a.is_action == 2)
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
        /// 设置集团权限
        /// </summary>
        /// <param name="orgAction"></param>
        /// <returns></returns>
        public bool Set(OrgAction orgAction)
        {
            //事务创建
            var result = Db.Ado.UseTran(() =>
            {
                var ret = -1;
                //删除角色已有权限
                ret = Db.Deleteable<p_org_action>().Where(w => w.org_id == orgAction.org_id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                if (ret != -1)
                {
                    //获取需要添加的权限
                    var addlst = orgAction.actions.Select(s => new p_org_action { action_id = s.id, org_id = orgAction.org_id, action_type = s.action_type }).ToList();

                    //添加角色权限
                    Db.Insertable(addlst).ExecuteCommand();
                    redisCache.RemoveAll<p_org_action>();
                }
            });
            return result.IsSuccess;
        }
    }
}
