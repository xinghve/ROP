using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Models.View.Mobile;
using Models.View.Public;
using Service.Repository.Interfaces.Public;
using Tools.Authorize;

namespace ROP.Controllers.Distributor
{
    /// <summary>
    /// 分销人员
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeForDistributor]
    public class DistributorController : ControllerBase
    {
        private readonly IDistributorService _distributorService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="distributorService"></param>
        public DistributorController(IDistributorService distributorService)
        {
            _distributorService = distributorService;
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [AuthorizeForDistributor(true)]
        [HttpPost("Register")]
        public async Task<int> RegisterAsync([FromBody]DistributorRegister entity)
        {
            return await _distributorService.RegisterAsync(entity);
        }

        /// <summary>
        /// 登录验证
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="org_id">集团ID</param>
        /// <param name="code">验证码</param>
        /// <param name="open_id">微信open_id</param>
        /// <returns></returns>
        [AuthorizeForDistributor(true)]
        [HttpGet("Login")]
        public async Task<dynamic> Login([FromQuery]string username, [FromQuery]string password, [FromQuery]int org_id, [FromQuery]string code, [FromQuery]string open_id)
        {
            return await _distributorService.Login(username, password, org_id, code, open_id);
        }

        /// <summary>
        /// 获取分销人员头像
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetPhoto")]
        public async Task<string> GetPhotoAsync()
        {
            return await _distributorService.GetPhotoAsync();
        }

        /// <summary>
        /// 根据分销人员ID获取信息
        /// </summary>
        /// <param name="distributor_id">分销人员ID</param>
        /// <returns></returns>
        [HttpGet("GetDistributor")]
        public async Task<p_distributor> GetDistributorAsync([FromQuery]int distributor_id)
        {
            return await _distributorService.GetDistributorAsync(distributor_id);
        }

        /// <summary>
        /// 编辑分销人员头像
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("ModifyPhoto")]
        public async Task<int> ModifyPhotoByIdAsync([FromBody]ArcPhoto entity)
        {
            return await _distributorService.ModifyPhotoAsync(entity);
        }

        /// <summary>
        /// 忘记密码
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [AuthorizeForDistributor(true)]
        [HttpPut("ModifyPassword")]
        public async Task<int> ModifyPassword([FromBody]ArcLoginPassword entity)
        {
            return await _distributorService.ModifyPassword(entity);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("UpdatePassword")]
        public async Task<int> UpdatePassword([FromBody]ArcLoginPassword entity)
        {
            return await _distributorService.UpdatePassword(entity);
        }

        /// <summary>
        /// 修改分销人员（客户端）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("Modify")]
        public async Task<int> ModifyAsync([FromBody]p_distributor entity)
        {
            return await _distributorService.ModifyAsync(entity);
        }
    }
}