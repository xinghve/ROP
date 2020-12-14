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
    /// 部门
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DeptController : ControllerBase
    {
        private readonly IDeptService _deptService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deptService"></param>
        public DeptController(IDeptService deptService)
        {
            _deptService = deptService;
        }

        /// <summary>
        /// 添加部门
        /// </summary>
        /// <param name="entity">部门信息</param>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<bool> Add([FromBody]DeptExtent entity)
        {
            return await _deptService.AddAsync(entity);
        }

        /// <summary>
        /// 修改部门
        /// </summary>
        /// <param name="entity">部门信息</param>
        /// <returns></returns>
        [HttpPut("Modify")]
        public async Task<bool> Modify([FromBody]DeptExtent entity)
        {
            return await _deptService.ModifyAsync(entity);
        }

        /// <summary>
        /// 修改状态（状态（0=停用，1=启用））
        /// </summary>
        /// <returns></returns>
        [HttpPut("ModifyDisabled")]
        public async Task<int> ModifyDisabled([FromBody]p_dept entity)
        {
            return await _deptService.ModifyDisabledAsync(entity);
        }

        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="entity">部门</param>
        /// <returns></returns>
        [HttpDelete("Delete")]
        public async Task<bool> Delete([FromBody]p_dept entity)
        {
            return await _deptService.DeleteAsync(entity.id);
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="storeID">门店ID</param>
        /// <param name="disabledCode">状态（-1=所有，0=停用，1=启用）</param>
        /// <param name="nature">性质</param>
        /// <param name="sign">是否集团私有(3=否； 2=是； -1=所有)</param>
        /// <returns></returns>
        [HttpGet("GetList")]
        public async Task<List<p_dept>> GetListAsync([FromQuery] string name, [FromQuery] int storeID = 0, [FromQuery] int disabledCode = -1, [FromQuery] int nature = -1, [FromQuery] short sign = -1)
        {
            return await _deptService.GetListAsync(name, storeID, disabledCode, nature, sign);
        }

        /// <summary>
        /// 数据列表（门店选择复制部门）
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <returns></returns>
        [HttpGet("GetDeptList")]
        public async Task<List<p_dept>> GetDeptListAsync([FromQuery] int store_id)
        {
            return await _deptService.GetDeptListAsync(store_id);
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param model="model">名称</param>
        /// <returns></returns>
        [HttpGet("GetPages")]
        public async Task<Page<DeptModel>> GetPagesAsync([FromQuery]DeptSearchModel model)
        {
            return await _deptService.GetPagesAsync(model);
        }

        /// <summary>
        /// 获取挂号科室
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        [HttpGet("GetCusList")]
        public async Task<List<DeptCusModel>> GetCusList([FromQuery]int storeId)
        {
            return await _deptService.GetCusList(storeId);
        }

        /// <summary>
        /// 获取科室简介
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        [HttpGet("GetDeptIntroduce")]
        public async Task<DeptCusDetail> GetDeptIntroduce([FromQuery]int deptId)
        {
            return await _deptService.GetDeptIntroduce(deptId);
        }

        /// <summary>
        /// 复制部门信息到门店
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("Copy")]
        public async Task<bool> CopyAsync(DeptCopy entity)
        {
            return await _deptService.CopyAsync(entity);
        }

        /// <summary>
        /// 移除门店部门
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpDelete("Remove")]
        public async Task<bool> RemoveAsync(DeptRemove entity)
        {
            return await _deptService.RemoveAsync(entity);
        }

        /// <summary>
        /// 获取当前登录人所有部门
        /// </summary>
        /// <param name="store_id"></param>
        /// <returns></returns>
        [HttpGet("GetCurrentDept")]
        public async Task<List<p_dept>> GetCurrentDept([FromQuery]int store_id)
        {
            return await _deptService.GetCurrentDept(store_id);
        }
    }
}