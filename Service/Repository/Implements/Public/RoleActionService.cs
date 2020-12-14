using Models.DB;
using Newtonsoft.Json;
using Service.Repository.Interfaces.Public;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tools;
using SqlSugar;
using System.Linq;
using Models.View.Public;

namespace Service.Repository.Implements.Public
{
    /// <summary>
    /// 角色权限
    /// </summary>
    public class RoleActionService : DbContext, IRoleActionService
    {
        /// <summary>
        /// 获取选中角色权限
        /// </summary>
        /// <param name="role_id">角色id</param>
        /// <returns></returns>
        public async Task<object> GetAsync(int role_id)
        {
            //查询角色已有权限
            var lst = await Db.Queryable<p_role_action, p_action>((ra, a) => new object[] { JoinType.Left, ra.action_id == a.id })
                .Where((ra, a) => ra.role_id == role_id && a.is_action == 2)
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
        /// <param name="roleAction"></param>
        /// <returns></returns>
        public bool Set(RoleAction roleAction)
        {
            //事务创建
            var result = Db.Ado.UseTran(() =>
            {
                var ret = -1;
                //删除角色已有权限
                ret = Db.Deleteable<p_role_action>().Where(w => w.role_id == roleAction.role_id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                if (ret != -1)
                {
                    //获取需要添加的权限
                    var addlst = roleAction.actions.Select(s => new p_role_action { action_id = s, role_id = roleAction.role_id }).ToList();
                    //添加角色权限
                    Db.Insertable(addlst).ExecuteCommand();
                    redisCache.RemoveAll<p_role_action>();
                }
            });
            return result.IsSuccess;
        }
    }
}
