using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static ServiceStack.Diagnostics.Events;
using Tools;

namespace IdS4
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // 使用内存存储，密钥，客户端和资源来配置身份服务器。
            services.AddIdentityServer()
                .AddSigningCredential(new X509Certificate2("idsrv4.pfx",
                "newcbsoft"))
                .AddInMemoryIdentityResources(Config.GetIdentityResourceResources())
                .AddInMemoryApiResources(Config.GetApiResources())//添加api资源
                .AddInMemoryClients(Config.GetClients())//添加客户端
                                                        //.AddTestUsers(Config.GetUsers()) //添加测试用户
                .AddProfileService<ProfileService>()
                .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseIdentityServer();

            app.UseMvc();
        }
    }
}
