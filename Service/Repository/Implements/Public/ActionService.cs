using Microsoft.AspNetCore.Http;
using Models.DB;
using Newtonsoft.Json;
using Service.Extensions;
using Service.Repository.Interfaces.Public;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Tools;
using Tools.Filter;

namespace Service.Repository.Implements.Public
{
    /// <summary>
    /// 菜单功能
    /// </summary>
    public class ActionServer : DbContext, IActionService
    {
        /// <summary>
        /// 删除选中所有（异步）
        /// </summary>
        /// <param name="delList">需要删除的id集合</param>
        /// <returns></returns>
        public async Task<int> DeleteAsync(List<int> delList)
        {
            return await Db.Deleteable<p_action>(delList).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
        }

        /// <summary>
        /// 获取功能表（异步）
        /// </summary>
        /// <returns></returns>
        public async Task<object> GetActionTree(int store_id, int setType)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            var action_type = 0;
            if (userInfo.org_id > 0)
            {
                action_type = setType;
            }
            var jsonStr = "[";
            if (action_type == 0)
            {
                var list = await Db.Queryable<p_action>().OrderBy(o => o.level).WithCache().ToListAsync();

                if (setType == 0)
                {
                    jsonStr += GetJson(list, -3, 0);
                }
                else if (setType == 1)
                {
                    jsonStr += GetJson(list, -2, 1);
                }
                else if (setType == 2)
                {
                    Dictionary<int, string> dir = new Dictionary<int, string>();

                    dir.Add(1, "集团功能");
                    dir.Add(2, "门店功能");
                    foreach (KeyValuePair<int, string> kvp in dir)
                    {
                        jsonStr += "{";

                        jsonStr += "label: '" + kvp.Value + "',";
                        jsonStr += "id: " + (kvp.Key - 3) + ",";
                        jsonStr += "action_type: " + kvp.Key + ",";
                        jsonStr += "children: [";
                        jsonStr += GetJson(list, (kvp.Key - 3), kvp.Key);
                        jsonStr += "]";

                        jsonStr += "},";
                    }
                }
                else
                {
                    Dictionary<int, string> dir = new Dictionary<int, string>();

                    dir.Add(0, "平台功能");
                    dir.Add(1, "集团功能");
                    dir.Add(2, "门店功能");
                    foreach (KeyValuePair<int, string> kvp in dir)
                    {
                        jsonStr += "{";

                        jsonStr += "label: '" + kvp.Value + "',";
                        jsonStr += "id: " + (kvp.Key - 3) + ",";
                        jsonStr += "action_type: " + kvp.Key + ",";
                        jsonStr += "children: [";
                        jsonStr += GetJson(list, (kvp.Key - 3), kvp.Key);
                        jsonStr += "]";

                        jsonStr += "},";
                    }
                }
            }
            else
            {
                var list = new List<p_action>();
                if (store_id > 0)
                {
                    list = await Db.Queryable<p_store_action, p_action>((sa, a) => new object[] { JoinType.Left, sa.action_id == a.id })
                        .Where((sa, a) => sa.store_id == store_id)
                        .OrderBy((sa, a) => a.level)
                        .Select((sa, a) => a)
                        .WithCache()
                        .ToListAsync();
                    jsonStr += GetJson(list, -1, 2);
                }
                else
                {
                    list = await Db.Queryable<p_org_action, p_action>((oa, a) => new object[] { JoinType.Left, oa.action_id == a.id })
                        .Where((oa, a) => oa.org_id == userInfo.org_id && oa.action_type == action_type)
                        .OrderBy((oa, a) => a.level)
                        .Select((oa, a) => a)
                        .WithCache()
                        .ToListAsync();
                    jsonStr += GetJson(list, action_type - 3, action_type);
                }
            }
            jsonStr = jsonStr.TrimEnd(',');
            jsonStr += "]";
            return JsonConvert.DeserializeObject(jsonStr);
        }

        private string GetJson(IEnumerable<p_action> lst, int parent_id, int action_type)
        {
            var str = "";
            var newLst = lst.Where(s => s.action_type == action_type && s.parent_id == parent_id).ToList();
            if (newLst.Count > 0)
            {
                foreach (var item in newLst)
                {
                    str += "{";

                    str += "label: '" + item.name + "',";
                    str += "id: " + item.id + ",";
                    str += "action_type: " + item.action_type + ",";
                    str += "children: [";
                    str += GetJson(lst, item.id, item.action_type);
                    str += "]";

                    str += "},";
                }
                str = str.TrimEnd(',');
            }
            return str;
        }

        /// <summary>
        /// 获取菜单树（异步）
        /// </summary>
        /// <returns></returns>
        public async Task<object> GetMenuTree(int store_id)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            var action_type = 0;
            var is_admin = userInfo.is_admin;
            if (userInfo.org_id > 0)
            {
                action_type = 1;
            }
            if (store_id > 0)
            {
                action_type = 2;
            }
            var lst = new List<p_action>();
            if (action_type == 0)
            {
                lst = await Db.Queryable<p_action>().Where(w => w.action_type == action_type).OrderBy(o => o.level).WithCache().ToListAsync();
            }
            else
            {
                if (store_id > 0)
                {
                    var er = await Db.Queryable<p_employee_role>()
                        .Where(w => w.org_id == userInfo.org_id && w.store_id == store_id && w.employee_id == userInfo.id && w.is_admin == true)
                        .WithCache()
                        .FirstAsync();
                    if (er != null)
                    {
                        is_admin = true;
                    }
                    lst = await Db.Queryable<p_store_action, p_action>((sa, a) => new object[] { JoinType.Left, sa.action_id == a.id })
                        .Where((sa, a) => sa.store_id == store_id && a.action_type == action_type)
                        .OrderBy((sa, a) => a.level)
                        .Select((sa, a) => a)
                        .WithCache()
                        .ToListAsync();
                }
                else
                {
                    lst = await Db.Queryable<p_org_action, p_action>((oa, a) => new object[] { JoinType.Left, oa.action_id == a.id })
                        .Where((oa, a) => oa.org_id == userInfo.org_id && a.action_type == action_type)
                        .OrderBy((oa, a) => a.level)
                        .Select((oa, a) => a)
                        .WithCache()
                        .ToListAsync();
                }
            }
            if (!is_admin)
            {
                var actionIdList = await Db.Queryable<p_role_action, p_employee_role>((ra, er) => new object[] { JoinType.Left, ra.role_id == er.role_id })
                    .Where((ra, er) => er.employee_id == userInfo.id)
                    .Select((ra, er) => ra.action_id)
                    .WithCache()
                    .ToListAsync();
                lst = lst.Where(w => actionIdList.Contains(w.id)).ToList();
            }
            var jsonStr = "[";
            var first = lst.Where(s => s.parent_id <= 0);
            foreach (var item in first)
            {
                var next = lst.Where(s => s.parent_id == item.id);

                if (next.Count() > 0)
                {
                    jsonStr += "{";
                    jsonStr += "menuName: '" + item.name + "',";
                    jsonStr += "menuIcon: '" + item.icon + "',";
                    jsonStr += "menuItem: [";

                    foreach (var nextItem in next)
                    {
                        jsonStr += "{";
                        jsonStr += "name: '" + nextItem.name + "',";
                        jsonStr += "rout: '" + nextItem.uri + "'";
                        jsonStr += "},";
                    }
                    jsonStr = jsonStr.TrimEnd(',');
                    jsonStr += "]";
                    jsonStr += "},";
                }
            }
            jsonStr = jsonStr.TrimEnd(',');
            jsonStr += "]";
            return JsonConvert.DeserializeObject(jsonStr);
        }

        /// <summary>
        /// 分页方式获取功能表（异步）
        /// </summary>
        /// <param name="parentId">上级Id</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        public async Task<Page<p_action>> GetPagesAsync(int parentId, string order, int orderType, int limit, int page)
        {
            var orderTypeStr = " Asc";
            if (orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            return await Db.Queryable<p_action>()
            .WhereIF(parentId != 0, w => w.parent_id == parentId)
            .OrderBy(order + orderTypeStr)
            .WithCache()
            .ToPageAsync(page, limit);
        }

        /// <summary>
        /// 添加功能菜单（异步）
        /// </summary>
        /// <param name="action">功能菜单</param>
        /// <returns></returns>
        public async Task<int> AddAsync(p_action action)
        {
            //判断是否存在
            var isExisteName = await Db.Queryable<p_action>().WithCache().AnyAsync(a => a.parent_id == action.parent_id && a.name == action.name);
            if (isExisteName)
            {
                throw new MessageException("当前层级已存在此名称");
            }
            action.pinyin = ToSpell.GetFirstPinyin(action.name);
            //新增
            redisCache.RemoveAll<p_action>();
            return await Db.Insertable(action).ExecuteReturnIdentityAsync();
        }

        /// <summary>
        /// 修改功能菜单（异步）
        /// </summary>
        /// <param name="action">功能菜单</param>
        /// <returns></returns>
        public async Task<int> ModifyAsync(p_action action)
        {
            //判断是否存在
            var isExisteName = await Db.Queryable<p_action>().WithCache().AnyAsync(a => a.parent_id == action.parent_id && a.name == action.name && a.id != action.id);
            if (isExisteName)
            {
                throw new MessageException("当前层级已存在此名称");
            }
            action.pinyin = ToSpell.GetFirstPinyin(action.name);
            return await Db.Updateable(action).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
        }
    }
}
