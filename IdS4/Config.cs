using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdS4
{
    public class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResourceResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(), //必须要添加，否则报无效的scope错误
                new IdentityResources.Profile()
            };
        }
        public static List<TestUser> GetUsers()
        {
            return null;
            //return new List<TestUser>
            //{
            //    new TestUser
            //    {
            //        SubjectId = "1",
            //        Username = "alice",
            //        Password = "password"
            //    },
            //    new TestUser
            //    {
            //        SubjectId = "2",
            //        Username = "bob",
            //        Password = "password"
            //    }
            //};
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "client",
                    // 没有交互性用户，使用 clientid/secret 实现认证。
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    //AccessTokenLifetime = 30,
                    AccessTokenLifetime = 3600,
                    // 用于认证的密码
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    //AccessTokenLifetime = setting.TokenLiftTime * 3600, //AccessToken的过期时间， in seconds 
                    // 客户端有权访问的范围（Scopes）
                    AllowedScopes = { "api",IdentityServerConstants.StandardScopes.OpenId,IdentityServerConstants.StandardScopes.Profile }
                    //必须要添加，否则报forbidden错误
                },
                // resource owner password grant client
                new Client
                {
                    ClientId = "ro.client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    //AccessTokenLifetime = 30,
                    AccessTokenLifetime = 3600,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "api",IdentityServerConstants.StandardScopes.OpenId, IdentityServerConstants.StandardScopes.Profile }
                }
            };
        }

        // scopes define the API resources in your system
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api", "MyAPI")
            };
        }
    }
}
