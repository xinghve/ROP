using IdentityModel.Client;
using IdentityServer4.Models;
using Models.DB;
using Models.View.Public;
using Newtonsoft.Json;
using Senparc.Weixin;
using Senparc.Weixin.Entities;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.MP.Entities.Menu;
using Service.Repository.Interfaces.Public;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Tools;
using Tools.Filter;

namespace Service.Repository.Implements.Public
{
    /// <summary>
    /// 登录
    /// </summary>
    public class LoginService : DbContext, ILoginService
    {
        private static string IdentityConnection;

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="code">短信验证码</param>
        /// <returns></returns>
        public async Task<LoginInfo> Login(string userName, string password, string code)
        {
            var user_name = "超级管理员";
            var id = 0;//用户id
            var org_id = 0;
            bool is_admin = true;//是否管理员
            var day = 0;//剩余天数
            p_sms_send_record getCode = new p_sms_send_record();
            //验证码登录
            if (!String.IsNullOrEmpty(code))
            {
                //查询短信验证码
                getCode = await Db.Queryable<p_sms_send_record>().Where(a => a.phone_no == userName && a.scene == "登录确认验证码" && a.expire_time > DateTime.Now).OrderBy(a => a.expire_time, SqlSugar.OrderByType.Desc).FirstAsync();

                if (getCode == null)
                {
                    throw new MessageException("验证码已过期,请重新获取!");
                }

                //手机验证码
                if (code != getCode.code)
                {
                    throw new MessageException("验证码错误!");
                }
            }

            var user = new p_employee();

            //请求令牌
            TokenResponse tokenResponse;
            //多门店的标志
            var stroeMark = new List<LoginStoreList>();

            //超管
            if (userName == "administrator")
            {
                if (string.IsNullOrEmpty(code) && password != MetarnetRegex.Encrypt("abc123"))
                {
                    throw new MessageException("密码错误!");
                }
                password = MetarnetRegex.Encrypt("abc123");
            }
            else
            {
                //获取用户登录信息
                user = await Db.Queryable<p_employee>().Where(a => a.phone_no == userName && a.expire_time >= DateTime.Now).OrderBy(o => o.expire_time, SqlSugar.OrderByType.Desc).FirstAsync();

                if (user == null)
                {
                    throw new MessageException("用户不存在!");
                }

                if (string.IsNullOrEmpty(code) && password != user.password)
                {
                    throw new MessageException("密码错误!");
                }
                password = user.password;
                user_name = user.name;
                id = user.id;
                is_admin = user.is_admin;
                org_id = user.org_id;
            }

            if (id > 0)
            {

                if (org_id > 0)
                {
                    // 查询机构是否在有效期内
                    var org = await Db.Queryable<p_org>().Where(a => a.id == org_id).Select(a => new { a.expire_time, a.status }).FirstAsync();
                    if (org.status == 0)
                    {
                        throw new MessageException("机构待审核，暂时不能登录!");
                    }
                    if (DateTime.Now > org.expire_time)
                    {
                        throw new MessageException("机构已过有效期!");
                    }
                    //到期天数
                    day = (org.expire_time - DateTime.Now).Days;
                }

            }

            //获取token
            tokenResponse = await GetToKen(userName, password);
            //请求令牌失败
            if (tokenResponse.IsError)
            {
                throw new IdentityException("身份验证失败！" + JsonConvert.SerializeObject(tokenResponse));
            }
            //获取令牌中的access_token       
            var Result = JsonConvert.DeserializeObject<IdentityModels>(tokenResponse.Json.ToString()).access_token;

            if (id > 0)
            {
                var orgName = await Db.Queryable<p_org>().Where(a => a.id == org_id).Select(a => new { a.name }).FirstAsync();
                if (orgName == null)
                {
                    orgName = new { name = "管理平台" };
                }
                //查询是否有多门店
                var stroeList = await Db.Queryable<p_employee_role, p_store>((er, st) => new object[] { SqlSugar.JoinType.Left, er.store_id == st.id }).Where((er, st) => er.employee_id == user.id && er.org_id == org_id && (st.use_status == 1 || st.use_status == null)).Select((er, st) => new LoginStoreList { id = er.store_id, is_admin = er.is_admin, name = SqlFunc.IIF(string.IsNullOrEmpty(st.name), orgName.name, st.name) }).ToListAsync();//.GroupBy((er, st) => new { er.store_id, er.is_admin, st.name })
                var stroeLists = stroeList.Select(s => new { s.id, s.name }).Distinct().ToList();
                stroeMark = stroeLists.Select(s => new LoginStoreList { id = s.id, name = s.name, is_admin = stroeList.Where(w => w.id == s.id && w.name == s.name && w.is_admin == true).FirstOrDefault() == null ? false : true }).ToList(); //.Select(s => new LoginStoreList { name = string.IsNullOrEmpty(s.name) ? orgName.name : s.name, id = s.id, is_admin = s.is_admin }).Distinct().ToList();
            }

            if (!string.IsNullOrEmpty(code))
            {
                //修改验证码时间
                getCode.expire_time = DateTime.Now;
                await Db.Updateable(getCode).Where(w => w.phone_no == getCode.phone_no && w.send_time == getCode.send_time).RemoveDataCache().EnableDiffLogEvent(true).ExecuteCommandAsync();
            }

            var log = new r_logs { code = DateTime.Now.ToString("yyyyMMddHHmmssffff"), content = "", date = DateTime.Now, ip = Utils.GetIp(), parameter = "", state = 1, url = Utils.GetUrl(), creater = user_name, creater_id = id, after_data = "", before_data = "", type = "登录" };

            await Db.Insertable(log).ExecuteCommandAsync();
            redisCache.RemoveAll<r_logs>();
            //人员性质
            var nature_list = await Db.Queryable<p_employee_nature>().Where(w => w.employee_id == id && w.org_id == org_id).Select(s => s.nature_id).ToListAsync();
            //医生科室

            var dept_list = await Db.Queryable<p_employee, p_employee_role, p_employee_nature, p_dept, p_dept_nature>((e, er, en, d, dn) => new object[] { JoinType.Left, er.employee_id == e.id, JoinType.Left, en.employee_id == er.employee_id, JoinType.Left, er.dept_id == d.id, JoinType.Left, dn.dept_id == d.id })
                                   .Where((e, er, en, d, dn) => e.id == id && en.nature_id == 1 && e.expire_time >= DateTime.Now && e.org_id == org_id && d.state == 1 && dn.nature_id == 3)
                                   .Select((e, er, en, d, dn) => new Dept { dept_id = er.dept_id, name = d.name })
                                   .WithCache().ToListAsync();

            return new LoginInfo { id = id, access_token = Result, is_admin = is_admin, user_name = user_name, stroeMark = stroeMark, day = day, org_id = org_id, nature_list = nature_list, dept_list = dept_list };
        }

        #region 获取用户名密码请求的access_token
        /// <summary>
        /// 获取用户名密码请求的access_token
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>返回请求的令牌</returns>
        public async static Task<TokenResponse> GetToKen(string username, string password)
        {
            IdentityConnection = ConfigExtensions.Configuration["ConnectionStrings:IdentityConnection"];

            #region IdentityModel自带请求方式

            // 从元数据中发现客户端
            //var disco = DiscoveryClient.GetAsync("http://192.168.20.124:8001/").Result;

            var client = new HttpClient();

            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = IdentityConnection,
                Policy =
                {
                    RequireHttps = false,
                    ValidateIssuerName = false
                }
            });


            // 请求令牌
            var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "ro.client",
                ClientSecret = "secret",
                Scope = "api",
                GrantType = GrantType.ResourceOwnerPassword,
                Password = password,
                UserName = username
            });

            return tokenResponse;
            #endregion
        }

        #endregion

        /// <summary>
        /// 忘记密码
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> ModifyPassword(LoginPassword entity)
        {
            //查询短信验证码
            var getCode = await Db.Queryable<p_sms_send_record>().Where(a => a.phone_no == entity.phone_no && a.scene == "修改密码验证码" && a.expire_time > DateTime.Now).OrderBy(a => a.expire_time, SqlSugar.OrderByType.Desc).FirstAsync();

            if (getCode == null)
            {
                throw new MessageException("请重新获取验证码!");
            }

            //手机验证码
            if (entity.code != getCode.code && !string.IsNullOrEmpty(entity.code))
            {
                throw new MessageException("验证码错误!");
            }
            if (entity.passwd != entity.passwd2)
            {
                throw new MessageException("两次输入密码不一致!");
            }
            //验证用户
            await GetUser(entity.phone_no);

            //修改密码
            var pModel = new p_employee();
            pModel.password = entity.passwd2;
            pModel.phone_no = entity.phone_no;
            //获取修改返回条数
            var result = await Db.Updateable<p_employee>(pModel).UpdateColumns(s => new { s.password }).Where(it => it.phone_no == entity.phone_no).RemoveDataCache().EnableDiffLogEvent(true).ExecuteCommandAsync();
            if (result <= 0)
            {
                throw new MessageException("密码修改失败!");
            }
            //修改验证码时间
            await Db.Updateable<p_sms_send_record>().SetColumns(s => new p_sms_send_record { expire_time = DateTime.Now }).Where(w => w.code == entity.code && w.phone_no == entity.phone_no && w.send_time == getCode.send_time).RemoveDataCache().EnableDiffLogEvent(true).ExecuteCommandAsync();

            return result;
        }


        /// <summary>
        /// 获取用户，是否存在
        /// </summary>
        /// <param name="phone">电话号码</param>
        /// <returns></returns>
        public async Task<bool> GetUser(string phone)
        {
            if (String.IsNullOrEmpty(phone))
            {
                throw new MessageException("手机号不能为空!");
            }
            var isExit = await Db.Queryable<p_employee>().WithCache().AnyAsync(a => a.phone_no == phone);
            if (!isExit)
            {
                throw new MessageException("用户不存在!");
            }
            return isExit;
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> UpdatePassword(LoginPassword entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            if (userInfo == null)
            {
                throw new MessageException("用户信息无效!");
            }

            if (entity == null)
            {
                throw new MessageException("传值错误!");
            }
            //验证原密码是否正确
            var isPwd = await Db.Queryable<p_employee>().Where(a => a.id == userInfo.id && a.org_id == userInfo.org_id).Select(a => a.password).WithCache().FirstAsync();
            if (entity.pwd != isPwd)
            {
                throw new MessageException("原密码错误!");
            }

            //修改密码
            var pModel = new p_employee();
            pModel.password = entity.passwd;
            pModel.id = userInfo.id;
            //获取修改返回条数
            var result = await Db.Updateable<p_employee>(pModel).UpdateColumns(s => new { s.password }).Where(it => it.id == userInfo.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
            if (result <= 0)
            {
                throw new MessageException("密码修改失败!");
            }
            return result;
        }


        /// <summary>
        /// Identity令牌实体
        /// </summary>
        public class IdentityModels
        {
            /// <summary>
            /// 存取令牌
            /// </summary>
            public string access_token { get; set; }
            /// <summary>
            /// 生命周期
            /// </summary>
            public string expires_in { get; set; }
            /// <summary>
            /// 令牌类型
            /// </summary>
            public string token_type { get; set; }
        }

        /// <summary>
        /// 获取OpenID
        /// </summary>
        /// <param name="code">code</param>
        /// <returns></returns>
        public async Task<string> GetOpenID(string code)
        {
            OAuthAccessTokenResult result = await OAuthApi.GetAccessTokenAsync(Config.SenparcWeixinSetting.WeixinAppId, Config.SenparcWeixinSetting.WeixinAppSecret, code);
            if (result.errcode.ToString() == "请求成功")
            {
                return result.openid;
            }
            else
            {
                throw new MessageException("获取OpenID失败 || " + Config.SenparcWeixinSetting.WeixinAppId + "||" + Config.SenparcWeixinSetting.WeixinAppSecret);
            }
        }

        /// <summary>
        /// 创建自定义菜单
        /// </summary>
        /// <param name="resultFull">Menu</param>
        /// <returns></returns>
        public async Task<string> CreateMenu(GetMenuResultFull resultFull)
        {
            //根据appId、appSecret获取
            string access_token = await AccessTokenContainer.TryGetAccessTokenAsync(Config.SenparcWeixinSetting.WeixinAppId, Config.SenparcWeixinSetting.WeixinAppSecret);
            try
            {
                //重新整理按钮信息
                WxJsonResult result = null;
                IButtonGroupBase buttonGroup = null;

                buttonGroup = Senparc.Weixin.MP.CommonAPIs.CommonApi.GetMenuFromJsonResult(resultFull, new ButtonGroup()).menu;
                result = Senparc.Weixin.MP.CommonAPIs.CommonApi.CreateMenu(access_token, buttonGroup);

                if (result.errmsg == "ok")
                {
                    return "创建自定义菜单成功";
                }
                else
                {
                    throw new MessageException("创建自定义菜单失败");
                }
            }
            catch (Exception ex)
            {
                throw new MessageException(ex.Message);
            }
        }
    }
}
