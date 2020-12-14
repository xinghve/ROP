using Microsoft.AspNetCore.Http;
using Models.DB;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Linq;
using Tools.Cache;

namespace Tools
{
    /// <summary>
    /// 
    /// </summary>
    public class DbContext
    {
        /// <summary>
        /// 注意：不能写成静态的
        /// </summary>
        public SqlSugarClient Db;//用来处理事务多表查询和复杂的操作
        /// <summary>
        /// 处理redis
        /// </summary>
        public IRedisCache redisCache;

        /// <summary>
        /// 请求头参数
        /// </summary>
        public readonly IHttpContextAccessor httpContextAccessor = new HttpContextAccessor();

        /// <summary>
        /// 
        /// </summary>
        public DbContext()
        {
            redisCache = new RedisCache(ConfigExtensions.Configuration["ConnectionStrings:RedisConnection"]);//ICacheService
            Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = ConfigExtensions.Configuration["DbConnection:PostgreSqlConnectionString"],
                DbType = DbType.PostgreSQL,
                InitKeyType = InitKeyType.Attribute,//从特性读取主键和自增列信息
                IsAutoCloseConnection = true,//开启自动释放模式
                ConfigureExternalServices = new ConfigureExternalServices()
                {
                    DataInfoCacheService = redisCache //RedisCache是继承ICacheService自已实现的一个类
                }
            });
            //调式代码 用来打印SQL 
            Db.Aop.OnLogExecuted = (sql, pars) =>
            {
                if (sql.ToLower().Contains("insert") && !sql.ToLower().Contains("r_logs"))
                {
                    var diffLogModel = new DiffLogModel { DiffType = DiffType.insert, Parameters = pars, Sql = sql };
                    Logs(diffLogModel);
                }
            };
            Db.Aop.OnLogExecuting = (sql, pars) =>
             {
                 Console.WriteLine(sql + "\r\n" +
                     Db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
                 Console.WriteLine();
             };
            Db.Aop.OnDiffLogEvent = it =>
            {
                //写日志方法
                Logs(it);
            };
            //Db.Aop.OnError = (exp) =>//执行SQL 错误事件
            //{
            //    //exp.sql exp.parameters 可以拿到参数和错误Sql   
            //    Logs(exp.Sql, (SugarParameter[])exp.Parametres, 0);
            //};
        }

        private void Logs(DiffLogModel diffLogModel)
        {
            var beforeJson = JsonConvert.SerializeObject(diffLogModel.BeforeData);
            var afterJson = JsonConvert.SerializeObject(diffLogModel.AfterData);
            var sql = diffLogModel.Sql;
            var pars = diffLogModel.Parameters;
            var httpContext = httpContextAccessor.HttpContext;
            var diffType = diffLogModel.DiffType;//枚举值 insert 、update 和 delete 用来作业务区分

            //获取用户信息
            var userInfo = new IdentityModels.GetUser.UserInfo();
            var claims = httpContextAccessor.HttpContext.User.Claims;
            if (claims.Count() > 0)
            {
                //获取用户信息
                userInfo = new IdentityModels.GetUser().userInfo;
            }
            else
            {
                var redisAuthorize = httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
                var redisCache = new RedisCache(ConfigExtensions.Configuration["ConnectionStrings:RedisConnection"]);
                if (redisCache.ContainsKey<c_archives>(redisAuthorize))
                {
                    //获取客户信息
                    var archivesInfo = new Tools.IdentityModels.GetArchives().archives;

                    userInfo.org_id = archivesInfo.org_id;
                    userInfo.name = archivesInfo.name;
                    userInfo.id = archivesInfo.id;
                }
            }
            var log = new r_logs { code = DateTime.Now.ToString("yyyyMMddHHmmssffff"), content = sql, date = DateTime.Now, ip = Utils.GetIp(), parameter = Db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)), state = 1, url = Utils.GetUrl(), after_data = afterJson, before_data = beforeJson };

            if (diffLogModel.BusinessData != null)
            {
                var isLogin = (bool)diffLogModel.BusinessData;
                if (!isLogin)
                {
                    log.creater = userInfo.name;
                    log.creater_id = userInfo.id;
                }
            }
            else
            {
                log.creater = userInfo.name;
                log.creater_id = userInfo.id;
            }
            if ((int)diffType == 0)
            {
                log.type = "添加";
            }
            else if ((int)diffType == 1)
            {
                log.type = "修改";
            }
            else if ((int)diffType == 2)
            {
                log.type = "删除";
            }
            Db.Insertable(log).RemoveDataCache().ExecuteCommand();
            Console.WriteLine(sql + "\r\n" +
                Db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
            Console.WriteLine();
        }
    }
}
