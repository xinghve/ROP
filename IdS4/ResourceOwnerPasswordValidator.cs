using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Models.DB;
using SqlSugar;
using System;
using System.Data;
using System.Security.Claims;
using System.Threading.Tasks;
using Tools;

namespace IdS4
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        public ResourceOwnerPasswordValidator()
        {

        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            SqlSugarClient Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = ConfigExtensions.Configuration["DbConnection:PostgreSqlConnectionString"],
                DbType = SqlSugar.DbType.PostgreSQL,
                InitKeyType = InitKeyType.Attribute,//从特性读取主键和自增列信息
                IsAutoCloseConnection = true,

            });

            var thisUser = new p_employee();
            if (context.UserName == ConfigExtensions.Configuration["User:UserName"])
            {

                if (context.Password == MetarnetRegex.Encrypt(ConfigExtensions.Configuration["User:Password"]))
                {
                    thisUser.id = 0;
                    thisUser.name = context.UserName;
                    thisUser.phone_no = "";
                    thisUser.org_id = 0;
                    thisUser.is_admin = true;
                }
            }
            else
            {
                //获取用户登录信息
                thisUser = await Db.Queryable<p_employee>().Where(a => a.phone_no == context.UserName && a.password == context.Password && a.expire_time >= DateTime.Now).OrderBy(o => o.expire_time, SqlSugar.OrderByType.Desc).FirstAsync();
                if (thisUser == null)
                {
                    //验证失败
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, $"无效凭据（{context.UserName}；{context.Password}）");
                }
            }
            //根据context.UserName和context.Password与数据库的数据做校验，判断是否合法
            if (thisUser != null)
            {
                context.Result = new GrantValidationResult(
                 subject: context.UserName,
                 authenticationMethod: "custom",
                 claims: GetUserClaims(thisUser));
            }
            else
            {
                //验证失败
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, $"无效凭据（{context.UserName}；{context.Password}）");
            }
        }
        //可以根据需要设置相应的Claim
        private Claim[] GetUserClaims(p_employee user)
        {
            return new Claim[]
            {
            new Claim("id", user.id.ToString()),
            new Claim(JwtClaimTypes.Name,string.IsNullOrEmpty(user.name)?"":user.name),
            new Claim("phone_no", string.IsNullOrEmpty(user.phone_no)?"":user.phone_no),
            new Claim("org_id",user.org_id.ToString()),
            new Claim("is_admin",user.is_admin.ToString())
            };
        }
    }
}
