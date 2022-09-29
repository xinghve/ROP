using Models.DB;
using Models.View.Public;
using Service.Extensions;
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
    /// 角色
    /// </summary>
    public class RoleService : DbContext, IRoleService
    {
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <returns></returns>
        public async Task<int> AddAsync(p_role role)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            //判断是否存在
            var isExisteName = await Db.Queryable<p_role>().WithCache().AnyAsync(a => a.org_id == userInfo.org_id && a.store_id == role.store_id && a.dept_id == role.dept_id && a.name == role.name);
            if (isExisteName)
            {
                throw new MessageException("当前部门已存在此角色");
            }

            //新增
            role.org_id = userInfo.org_id;
            role.disabled = "启用";
            role.disabled_code = 1;
            role.pinyin = ToSpell.GetFirstPinyin(role.name);
            redisCache.RemoveAll<p_role>();
            return await Db.Insertable(role).ExecuteReturnIdentityAsync();
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <returns></returns>
        public async Task<int> DeleteAsync(int id)
        {
            var is_use = await Db.Queryable<p_employee_role>().WithCache().AnyAsync(w => w.role_id == id);
            if (is_use)
            {
                throw new MessageException("当前角色正在使用中，不能删除");
            }
            return await Db.Deleteable<p_role>().Where(w => w.id == id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
        }

        /// <summary>
        /// 数据列表
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="store_id">门店ID</param>
        /// <param name="dept_id">部门ID</param>
        /// <returns></returns>
        public async Task<List<p_role>> GetListAsync(string name, int store_id, int dept_id)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            return await Db.Queryable<p_role>()
                .Where(w => w.org_id == userInfo.org_id && w.store_id == store_id && w.dept_id == dept_id)
                .WhereIF(!string.IsNullOrEmpty(name), w => w.name.Contains(name))
                .WithCache()
                .ToListAsync();
        }

        /// <summary>
        /// 获得列表
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="store_id">门店ID</param>
        /// <param name="dept_id">部门ID</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        public async Task<Page<Role>> GetPagesAsync(string name, int store_id, int dept_id, string order, int orderType, int limit, int page)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var orderTypeStr = " Asc";
            if (orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            return await Db.Queryable<p_role, p_dept>((r, d) => new object[] { JoinType.Left, r.dept_id == d.id })
                .Where((r, d) => r.org_id == userInfo.org_id && r.store_id == store_id)
                .WhereIF(dept_id > 0, (r, d) => r.dept_id == dept_id)
                .WhereIF(!string.IsNullOrEmpty(name), (r, d) => r.name.Contains(name))
                .OrderBy(order + orderTypeStr)
                .Select((r, d) => new Role { disabled = r.disabled, disabled_code = r.disabled_code, id = r.id, introduce = r.introduce, name = r.name, pinyin = r.pinyin, dept_id = r.dept_id, dept_name = d.name, org_id = r.org_id, store_id = r.store_id })
                .WithCache()
                .ToPageAsync(page, limit);
        }

        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <returns></returns>
        public async Task<int> ModifyAsync(p_role role)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            //判断是否存在
            var isExisteName = await Db.Queryable<p_role>().WithCache().AnyAsync(a => a.org_id == userInfo.org_id && a.store_id == role.store_id && a.dept_id == role.dept_id && a.name == role.name && a.id != role.id);
            if (isExisteName)
            {
                throw new MessageException("当前部门已存在此角色");
            }
            var spell = ToSpell.GetFirstPinyin(role.name);
            return await Db.Updateable<p_role>()
                .SetColumns(s => new p_role { name = role.name, pinyin = spell, introduce = role.introduce })
                .Where(w => w.id == role.id)
                .RemoveDataCache()
                .EnableDiffLogEvent()
                .ExecuteCommandAsync();
        }

        /// <summary>
        /// 修改状态（状态（0=停用，1=启用））
        /// </summary>
        /// <returns></returns>
        public async Task<int> ModifyDisabledAsync(p_role role)
        {
            role.disabled = "停用";
            if (role.disabled_code == 1)
            {
                role.disabled = "启用";
            }
            return await Db.Updateable<p_role>()
                .SetColumns(s => new p_role { disabled = role.disabled, disabled_code = role.disabled_code })
                .Where(w => w.id == role.id)
                .RemoveDataCache()
                .EnableDiffLogEvent()
                .ExecuteCommandAsync();
        }

        /// <summary>
        /// 复制角色信息到门店
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> CopyAsync(RoleCopy entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            //获取角色信息
            var list = await Db.Queryable<p_role>().Where(w => entity.role_ids.Contains(w.id)).WithCache().ToListAsync();
            list = list.Select(s => new p_role { dept_id = entity.dept_id, disabled = "启用", disabled_code = 1, introduce = s.introduce, link_id = s.id, name = s.name, org_id = s.org_id, pinyin = s.pinyin, store_id = entity.store_id }).ToList();

            //新增
            redisCache.RemoveAll<p_role>();
            return await Db.Insertable(list).ExecuteCommandAsync();
        }

        /// <summary>
        /// 移除门店角色
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> RemoveAsync(RoleRemove entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            var result = Db.Ado.UseTran(() =>
            {
                //查询要移除的人员
                var list = Db.Queryable<p_employee_role>().Where(w => w.role_id == entity.role_id && w.store_id == entity.store_id && w.org_id == userInfo.org_id && w.is_admin == false).WithCache().ToList();

                //查询门店部门关联的集团部门
                var dept_ids = Db.Queryable<p_dept>().Where(w => list.Select(s => s.dept_id).ToList().Contains(w.id)).Select(s => new { s.id, s.link_id }).WithCache().ToList();

                //查询门店职位关联的集团职位
                var role_ids = Db.Queryable<p_role>().Where(w => list.Select(s => s.role_id).ToList().Contains(w.id)).Select(s => new { s.id, s.link_id }).WithCache().ToList();

                //移除人员
                Db.Deleteable<p_employee_role>().Where(w => w.role_id == entity.role_id && w.store_id == entity.store_id && w.org_id == userInfo.org_id && w.is_admin == false).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                //添加人员到集团
                list = list.Select(s => new p_employee_role { employee_id = s.employee_id, is_admin = false, org_id = s.org_id, store_id = 0, role_id = role_ids.Where(w => w.id == s.role_id).First().link_id.Value, dept_id = dept_ids.Where(w => w.id == s.dept_id).First().link_id.Value }).ToList();
                Db.Insertable(list).ExecuteCommand();
                redisCache.RemoveAll<p_employee_role>();

                //移除职位
                Db.Deleteable<p_role>().Where(w => w.id == entity.role_id && w.store_id == entity.store_id && w.org_id == userInfo.org_id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 数据列表（门店选择复制角色）
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="dept_id">部门ID</param>
        /// <returns></returns>
        public async Task<List<p_role>> GetRoleListAsync(int store_id, int dept_id)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            //查询集团部门ID对应的门店部门ID
            var deptid = await Db.Queryable<p_dept>().Where(w => w.link_id == dept_id).Select(s => s.id).WithCache().FirstAsync();
            //查询门店部门已存在的角色
            var roles = await Db.Queryable<p_role>().Where(w => w.store_id == store_id && w.dept_id == deptid).Select(s => s.link_id).WithCache().ToListAsync();
            return await Db.Queryable<p_role>().Where(w => w.org_id == userInfo.org_id && w.store_id == 0 && w.dept_id == dept_id && !roles.Contains(w.id)).WithCache().ToListAsync();
        }

        /// <summary>
        /// 流程管理用部门职位List
        /// </summary>
        /// <param name="is_org"></param>
        /// <param name="is_store"></param>
        /// <returns></returns>
        public async Task<List<ProcessDeptRoleModel>> GetDeptRole(bool is_org,bool is_store)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            
            if (is_org||!is_store)
            {
                //查询机构下部门职位
                var deptList = await Db.Queryable<p_dept>()
                                     .Where(d => d.org_id == userInfo.org_id && d.store_id == 0 && d.state == 1)
                                     .Select(d=>new ProcessDeptRoleModel { dept_name= d.name,dept_id= d.id })
                                     .WithCache()
                                     .ToListAsync();

                var deptIds = deptList.Select(ds => ds.dept_id).ToList();

                //查询部门下的职位
                var roleList = await Db.Queryable<p_role>()
                                     .Where(r => r.org_id == userInfo.org_id && r.store_id == 0 && deptIds.Contains(r.dept_id) && r.disabled_code == 1)
                                     .WithCache()
                                     .ToListAsync();
                var newList= deptList.Select(s => new ProcessDeptRoleModel { is_org= !is_store?true:false, dept_id = s.dept_id, dept_name = s.dept_name, roleList = roleList.Where(w => w.dept_id == s.dept_id).ToList() }).ToList();

                return newList;

            }
            else
            {
                //查询机构下部门职位
                var deptList = await Db.Queryable<p_dept>()
                                     .Where(d => d.org_id == userInfo.org_id && d.store_id == 0 && d.state == 1&&d.sign==3)
                                     .Select(d => new ProcessDeptRoleModel { dept_name = d.name, dept_id = d.id })
                                     .WithCache()
                                     .ToListAsync();

                var deptIds = deptList.Select(ds => ds.dept_id).ToList();

                //查询部门下的职位
                var roleList = await Db.Queryable<p_role>()
                                     .Where(r => r.org_id == userInfo.org_id && r.store_id == 0 && deptIds.Contains(r.dept_id) && r.disabled_code ==1)
                                     .WithCache()
                                     .ToListAsync();
                var newList = deptList.Select(s => new ProcessDeptRoleModel { is_org=false, dept_id = s.dept_id, dept_name = s.dept_name, roleList = roleList.Where(w => w.dept_id == s.dept_id).ToList() }).ToList();

                return newList;

            }
        }
    }
}
