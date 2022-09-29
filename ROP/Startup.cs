using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ROP.Controllers.CustomMessage;
using Senparc.CO2NET;
using Senparc.CO2NET.RegisterServices;
using Senparc.Weixin;
using Senparc.Weixin.Entities;
using Senparc.Weixin.MP;
using Senparc.Weixin.RegisterServices;
using Senparc.Weixin.TenPay;
using Service.Repository;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Tools.Filter;
using Tools.WebSocket;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using static IdentityModel.ClaimComparer;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Senparc.Weixin.Cache.CsRedis;
using Tools;
using Microsoft.OpenApi.Models;

namespace ROP
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

            RedisHelper.Initialization(new CSRedis.CSRedisClient(ConfigExtensions.Configuration.GetSection("ConnectionStrings:RedisConnection").Value));

            //配置identityServer授权
            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = Configuration.GetConnectionString("IdentityConnection");
                    options.RequireHttpsMetadata = false;
                    options.JwtValidationClockSkew = TimeSpan.FromMinutes(2);
                    options.ApiName = "api";
                });

            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(HttpGlobalExceptionFilter)); // by type
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;//PascalCase
                options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);  //防止中文出现乱码
            });

            services.AddScoped(typeof(IBaseServer<>), typeof(BaseServer<>));

            services.AddHttpContextAccessor();

            services.AddSwaggerGen(options =>
            {
                //options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "ROP API",
                    Description = "数字化康复运营平台 HTTP API."
                });

                var filePath = Path.Combine(AppContext.BaseDirectory, "ROP.xml");
                options.IncludeXmlComments(filePath);
                filePath = Path.Combine(AppContext.BaseDirectory, "Models.xml");
                options.IncludeXmlComments(filePath);
                filePath = Path.Combine(AppContext.BaseDirectory, "Tools.xml");
                options.IncludeXmlComments(filePath);
                filePath = Path.Combine(AppContext.BaseDirectory, "Service.xml");
                options.IncludeXmlComments(filePath);
            });

            //自定注册
            AddAssembly(services, "Service");

            services.AddSenparcGlobalServices(Configuration)//Senparc.CO2NET 全局注册
                    .AddSenparcWeixinServices(Configuration);//Senparc.Weixin 注册
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IOptions<SenparcSetting> senparcSetting, IOptions<SenparcWeixinSetting> senparcWeixinSetting)//, ILoggerFactory loggerFactory
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //使用跨域
            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            app.UseWebSockets();
            app.UseMiddleware<ChatWebSocketMiddleware>();

            app.UseHttpsRedirection();

            ////添加NLog  
            //loggerFactory.AddNLog();
            ////读取Nlog配置文件 
            //env.ConfigureNLog("nlog.config");

            app.UseAuthentication();
            app.UseMvc();

            DefaultFilesOptions defaultFilesOptions = new DefaultFilesOptions();
            defaultFilesOptions.DefaultFileNames.Clear();
            defaultFilesOptions.DefaultFileNames.Add("index.html");
            app.UseDefaultFiles(defaultFilesOptions);

            app.UseStaticFiles();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ROP API V1");
            });

            // 启动 CO2NET 全局注册，必须！
            IRegisterService register = RegisterService.Start(senparcSetting.Value).UseSenparcGlobal();

            #region CO2NET 全局配置

            #region 全局缓存配置（按需）

            //当同一个分布式缓存同时服务于多个网站（应用程序池）时，可以使用命名空间将其隔离（非必须）
            register.ChangeDefaultCacheNamespace("DefaultCO2NETCache");

            #region 配置和使用 Redis          -- DPBMARK Redis

            //配置全局使用Redis缓存（按需，独立）
            var redisConfigurationStr = senparcSetting.Value.Cache_Redis_Configuration;
            var useRedis = !string.IsNullOrEmpty(redisConfigurationStr) && redisConfigurationStr != "#{Cache_Redis_Configuration}#"/*默认值，不启用*/;
            if (useRedis)//这里为了方便不同环境的开发者进行配置，做成了判断的方式，实际开发环境一般是确定的，这里的if条件可以忽略
            {
                /* 说明：
                 * 1、Redis 的连接字符串信息会从 Config.SenparcSetting.Cache_Redis_Configuration 自动获取并注册，如不需要修改，下方方法可以忽略
                /* 2、如需手动修改，可以通过下方 SetConfigurationOption 方法手动设置 Redis 链接信息（仅修改配置，不立即启用）
                 */
                Senparc.CO2NET.Cache.CsRedis.Register.SetConfigurationOption(redisConfigurationStr);

                //以下会立即将全局缓存设置为 Redis
                Senparc.CO2NET.Cache.CsRedis.Register.UseKeyValueRedisNow();//键值对缓存策略（推荐）
                //Senparc.CO2NET.Cache.Redis.Register.UseHashRedisNow();//HashSet储存格式的缓存策略

                //也可以通过以下方式自定义当前需要启用的缓存策略
                //CacheStrategyFactory.RegisterObjectCacheStrategy(() => RedisObjectCacheStrategy.Instance);//键值对
                //CacheStrategyFactory.RegisterObjectCacheStrategy(() => RedisHashSetObjectCacheStrategy.Instance);//HashSet
            }
            //如果这里不进行Redis缓存启用，则目前还是默认使用内存缓存 

            #endregion                        // DPBMARK_END

            #endregion

            #region 注册日志（按需，建议）

            register.RegisterTraceLog(ConfigTraceLog);//配置TraceLog

            #endregion

            Senparc.CO2NET.APM.Config.DataExpire = TimeSpan.FromMinutes(60);//测试APM缓存过期时间（默认情况下可以不用设置）

            #endregion

            #region 微信相关配置


            /* 微信配置开始
             * 
             * 建议按照以下顺序进行注册，尤其须将缓存放在第一位！
             */

            //注册开始



            //开始注册微信信息，必须！
            register.UseSenparcWeixin(senparcWeixinSetting.Value, (weixinRegister, weixinSetting) => {

                #region 微信缓存

                //微信的 Redis 缓存，如果不使用则注释掉（开启前必须保证配置有效，否则会抛错）         -- DPBMARK Redis
                if (useRedis)
                {
                    weixinRegister.UseSenparcWeixinCacheCsRedis();
                }                                                                                        // DPBMARK_END

                #endregion

                #region 注册公众号或小程序（按需）

                //注册公众号（可注册多个）                                                -- DPBMARK MP
                weixinRegister.RegisterMpAccount(senparcWeixinSetting.Value, "scyykj");// DPBMARK_END

                //除此以外，仍然可以在程序任意地方注册公众号或小程序：
                //AccessTokenContainer.Register(appId, appSecret, name);//命名空间：Senparc.Weixin.MP.Containers
                #endregion


                #region 注册微信支付（按需）        -- DPBMARK TenPay

                //注册最新微信支付版本（V3）（可注册多个）
                weixinRegister.RegisterTenpayV3(senparcWeixinSetting.Value, "scyykj");//记录到同一个 SenparcWeixinSettingItem 对象中

                #endregion                          // DPBMARK_END
            ;

            });
                //注意：上一行没有 ; 下面可接着写 .RegisterXX()

            

            /* 微信配置结束 */

            #endregion
        }

        /// <summary>  
        /// 自动注册服务——获取程序集中的实现类对应的多个接口
        /// </summary>
        /// <param name="services">服务集合</param>  
        /// <param name="assemblyName">程序集名称</param>
        public void AddAssembly(IServiceCollection services, string assemblyName)
        {
            if (!String.IsNullOrEmpty(assemblyName))
            {
                Assembly assembly = Assembly.Load(assemblyName);
                List<Type> ts = assembly.GetTypes().Where(u => u.IsClass && !u.IsAbstract && !u.IsGenericType).ToList();
                foreach (var item in ts.Where(s => !s.IsInterface))
                {
                    var interfaceType = item.GetInterfaces();
                    if (interfaceType.Length == 1)
                    {
                        services.AddTransient(interfaceType[0], item);
                    }
                    if (interfaceType.Length > 1)
                    {
                        services.AddTransient(interfaceType[1], item);
                    }
                }
            }
        }

        /// <summary>
        /// 配置微信跟踪日志
        /// </summary>
        private void ConfigTraceLog()
        {
            //这里设为Debug状态时，/App_Data/WeixinTraceLog/目录下会生成日志文件记录所有的API请求日志，正式发布版本建议关闭

            //如果全局的IsDebug（Senparc.CO2NET.Config.IsDebug）为false，此处可以单独设置true，否则自动为true
            Senparc.CO2NET.Trace.SenparcTrace.SendCustomLog("系统日志", "系统启动");//只在Senparc.Weixin.Config.IsDebug = true的情况下生效

            //全局自定义日志记录回调
            Senparc.CO2NET.Trace.SenparcTrace.OnLogFunc = () =>
            {
                //加入每次触发Log后需要执行的代码
            };

            //当发生基于WeixinException的异常时触发
            WeixinTrace.OnWeixinExceptionFunc = async ex =>
            {
                //加入每次触发WeixinExceptionLog后需要执行的代码

                //发送模板消息给管理员                                   -- DPBMARK Redis
                var eventService = new EventService();
                await eventService.ConfigOnWeixinExceptionFunc(ex);      // DPBMARK_END
            };
        }
    }
}
