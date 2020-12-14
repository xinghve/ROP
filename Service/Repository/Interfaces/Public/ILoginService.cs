using Models.DB;
using Models.View.Public;
using Senparc.Weixin.MP;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service.Repository.Interfaces.Public
{
    /// <summary>
    /// 登录
    /// </summary>
    public interface ILoginService 
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<LoginInfo> Login(string userName, string password, string code);

        /// <summary>
        /// 忘记密码
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> ModifyPassword(LoginPassword entity);

        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        Task<bool> GetUser(string phone);

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> UpdatePassword(LoginPassword entity);

        /// <summary>
        /// 获取OpenID
        /// </summary>
        /// <param name="code">code</param>
        /// <returns></returns>
        Task<string> GetOpenID(string code);

        /// <summary>
        /// 创建自定义菜单
        /// </summary>
        /// <param name="resultFull">Menu</param>
        /// <returns></returns>
        Task<string> CreateMenu(GetMenuResultFull resultFull);
    }
}
