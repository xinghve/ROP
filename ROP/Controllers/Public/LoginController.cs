using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Models.View.Public;
using Senparc.CO2NET.Extensions;
using Senparc.Weixin.MP;
using Service.Repository.Interfaces.Public;
using Tools;

namespace ROP.Controllers.Public
{
    /// <summary>
    /// 登录--ty
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]

    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginServer;
        /// <summary>
        /// 登录构造
        /// </summary>
        /// <param name="loginServer"></param>
        public LoginController(ILoginService loginServer)
        {
            _loginServer = loginServer;
        }

        /// <summary>
        /// 登录验证
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="code">验证码</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<LoginInfo> Login([FromQuery]string username, [FromQuery]string password, [FromQuery]string code)
        {
            return await _loginServer.Login(username, password, code);
            //var result = await _loginServer.Login(username, password, code);
            //return Utils.ToBase64String(result.ToJson(), true, "Login");
        }

        /// <summary>
        /// 忘记密码
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("Modify")]
        public async Task<int> Modify([FromBody]LoginPassword entity)
        {
            return await _loginServer.ModifyPassword(entity);
            //var result = await _loginServer.ModifyPassword(entity);
            //return Utils.ToBase64String(result.ToJson(), true, "Login/Modify");
        }

        /// <summary>
        /// 获取用户是否存在
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <returns></returns>
        [HttpGet("GetUser")]
        public async Task<bool> GetUser([FromQuery]string phone)
        {
            return await _loginServer.GetUser(phone);
            //var result = await _loginServer.GetUser(phone);
            //return Utils.ToBase64String(result.ToJson(), true, "Login/GetUser");
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("UpdatePassword")]
        public async Task<int> UpdatePassword([FromBody]LoginPassword entity)
        {
            return await _loginServer.UpdatePassword(entity);
            //var result = await _loginServer.UpdatePassword(entity);
            //return Utils.ToBase64String(result.ToJson(), true, "Login/UpdatePassword");
        }

        /// <summary>
        /// 获取OpenID
        /// </summary>
        /// <param name="code">code</param>
        /// <returns></returns>
        [HttpGet("GetOpenID")]
        public async Task<string> GetOpenID(string code)
        {
            return await _loginServer.GetOpenID(code);
            //var result = await _loginServer.GetOpenID(code);
            //return Utils.ToBase64String(result.ToJson(), true, "Login/GetOpenID");
        }

        /// <summary>
        /// 创建自定义菜单
        /// </summary>
        /// <param name="resultFull">Menu</param>
        /// <returns></returns>
        [HttpPost("CreateMenu")]
        public async Task<string> CreateMenu(GetMenuResultFull resultFull)
        {
            return await _loginServer.CreateMenu(resultFull);
            //var result = await _loginServer.CreateMenu(resultFull);
            //return Utils.ToBase64String(result.ToJson(), true, "Login/CreateMenu");
        }
    }
}