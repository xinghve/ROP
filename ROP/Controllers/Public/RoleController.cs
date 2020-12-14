using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Models.View.Public;
using Service.Repository.Interfaces.Public;
using Tools;

namespace ROP.Controllers.Public
{
    /// <summary>
    /// 角色管理
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleService"></param>
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="entity">角色信息</param>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<int> Add([FromBody]p_role entity)
        {
            return await _roleService.AddAsync(entity);
        }

        /// <summary>
        /// 修改角色
        /// </summary>
        /// <param name="entity">角色信息</param>
        /// <returns></returns>
        [HttpPut("Modify")]
        public async Task<int> Modify([FromBody]p_role entity)
        {
            return await _roleService.ModifyAsync(entity);
        }

        /// <summary>
        /// 修改状态（状态（0=停用，1=启用））
        /// </summary>
        /// <returns></returns>
        [HttpPut("ModifyDisabled")]
        public async Task<int> ModifyDisabled([FromBody]p_role entity)
        {
            return await _roleService.ModifyDisabledAsync(entity);
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="entity">角色</param>
        /// <returns></returns>
        [HttpDelete("Delete")]
        public async Task<int> Delete([FromBody]p_role entity)
        {
            return await _roleService.DeleteAsync(entity.id);
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="storeID">门店ID</param>
        /// <param name="deptID">部门ID</param>
        /// <returns></returns>
        [HttpGet("GetList")]
        public async Task<List<p_role>> GetListAsync([FromQuery] string name, [FromQuery] int storeID = 0, [FromQuery] int deptID = 0)
        {
            return await _roleService.GetListAsync(name, storeID, deptID);
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="storeID">门店ID</param>
        /// <param name="deptID">部门ID</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        [HttpGet("GetPages")]
        public async Task<Page<Role>> GetPagesAsync([FromQuery] string name, [FromQuery] int storeID, [FromQuery] int deptID, [FromQuery] string order, [FromQuery] int orderType, [FromQuery] int limit, [FromQuery] int page)
        {
            return await _roleService.GetPagesAsync(name, storeID, deptID, order, orderType, limit, page);
        }

        /// <summary>
        /// 复制角色信息到门店
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("Copy")]
        public async Task<int> CopyAsync(RoleCopy entity)
        {
            return await _roleService.CopyAsync(entity);
        }

        /// <summary>
        /// 移除门店角色
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpDelete("Remove")]
        public async Task<bool> RemoveAsync(RoleRemove entity)
        {
            return await _roleService.RemoveAsync(entity);
        }

        /// <summary>
        /// 数据列表（门店选择复制角色）
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="dept_id">部门ID</param>
        /// <returns></returns>
        [HttpGet("GetRoleList")]
        public async Task<List<p_role>> GetRoleListAsync(int store_id, int dept_id)
        {
            return await _roleService.GetRoleListAsync(store_id, dept_id);
        }

        /// <summary>
        /// 流程管理用部门职位List
        /// </summary>
        /// <param name="is_org">是否机构</param>
        /// <param name="is_store">门店下判断 ：false机构 true门店</param>
        /// <returns></returns>
        [HttpGet("GetDeptRole")]
        public async Task<List<ProcessDeptRoleModel>> GetDeptRole([FromQuery]bool is_org,[FromQuery]bool is_store)
        {
            return await _roleService.GetDeptRole(is_org,is_store);
        }
    }
}