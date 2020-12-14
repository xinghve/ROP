using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.View.Public;
using Service.Repository.Interfaces.Public;

namespace ROP.Controllers.Public
{
    /// <summary>
    /// 集团权限
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class OrgActionController : ControllerBase
    {
        private readonly IOrgActionService _orgActionService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orgActionService"></param>
        public OrgActionController(IOrgActionService orgActionService)
        {
            _orgActionService = orgActionService;
        }

        /// <summary>
        /// 获取集团权限
        /// </summary>
        /// <param name="org_id">集团Id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> Get([FromQuery]int org_id)
        {
            return await _orgActionService.GetAsync(org_id);
        }

        /// <summary>
        /// 设置集团权限
        /// </summary>
        /// <param name="item">集团权限</param>
        /// <returns>成功/失败</returns>
        [HttpPost]
        public bool Post([FromBody] OrgAction item)
        {
            return _orgActionService.Set(item);
        }
    }
}