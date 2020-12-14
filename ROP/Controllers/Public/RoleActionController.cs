using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.View.Public;
using Service.Repository.Interfaces.Public;
using Tools.Filter;

namespace ROP.Controllers.Public
{
    /// <summary>
    /// 角色权限
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RoleActionController : ControllerBase
    {
        private readonly IRoleActionService _roleActionService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleActionService"></param>
        public RoleActionController(IRoleActionService roleActionService)
        {
            _roleActionService = roleActionService;
        }

        /// <summary>
        /// 获取角色权限
        /// </summary>
        /// <param name="role_id">角色Id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> Get([FromQuery]int role_id)
        {
            return await _roleActionService.GetAsync(role_id);
        }

        /// <summary>
        /// 设置角色权限
        /// </summary>
        /// <param name="item">角色权限</param>
        /// <returns>成功/失败</returns>
        [HttpPost]
        public bool Post([FromBody] RoleAction item)
        {
            return _roleActionService.Set(item);
        }
    }
}