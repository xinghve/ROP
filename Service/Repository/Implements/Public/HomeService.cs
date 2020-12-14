using Models.DB;
using Models.View.Business;
using Models.View.Public;
using Service.Repository.Interfaces.Public;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Tools;
using Tools.WebSocket;
using static Tools.IdentityModels.GetUser;

namespace Service.Repository.Implements.Public
{
    /// <summary>
    /// 首页
    /// </summary>
    public class HomeService : DbContext, IHomeService
    {
        //获取用户信息
        private readonly UserInfo userInfo = new Tools.IdentityModels.GetUser().userInfo;
        /// <summary>
        /// 获取节日（包括生日档案）
        /// </summary>
        /// <returns></returns>
        public async Task<object> GetFestival()
            {
            //通知
            var notice = new Business.NoticeService();
            var noticeList = new List<AddNoticeModel>();
            var notice_content = "";
            var employeeSocket = new List<WebSocketModel>();

            var festival = Utils.GetFestival(DateTime.Now);
            //redis key
            string key = "birthday" + userInfo.id + DateTime.Now.ToString("yyyyMMdd");
            var isCache = redisCache.Get<List<HomeModel>>(key);

            //查询节日缓存
            var birthday_archives = new List<HomeModel>();
            if (isCache != null && isCache.Count > 0)
            {
                birthday_archives = isCache;
            }
            else
            {
                var now = DateTime.Now.ToString("MMdd");
                birthday_archives = await Db.Queryable<c_archives>()
                .Where(a => a.to_director_id == userInfo.id)
                .Where("to_char(birthday, 'MMdd')=@now", new { now })
                .Select(a => new HomeModel { to_director = a.to_director, to_director_id = a.to_director_id.Value, id = a.id, name = a.name, phone = a.phone, age = a.age, sex = a.sex, isRead = false, image_url = a.image_url })
                .ToListAsync();

                // redisCache.Add(key, birthday_archives, 86400);

                if (birthday_archives.Count > 0)
                {
                    var birthday = DateTime.Now.ToString("yyyy-MM-dd");
                    //查询消息中有则不需要通知
                    var isExist = await Db.Queryable<oa_notice, oa_notice_employee>((a, b) => new object[] { JoinType.Left, a.id == b.id })
                                        .Where((a, b) => a.org_id == userInfo.org_id && a.notice_type_id == 10 && a.notice_property_id == 2 && b.employee_id == userInfo.id&&a.name.Contains(birthday))
                                        .WithCache()
                                        .AnyAsync();

                    if (!isExist)
                    {
                        birthday_archives.ForEach(c =>
                        {
                            var archives = new c_archives();
                            archives.name = c.name;
                            archives.id = c.id;
                            archives.phone = c.phone;
                            //调用通知
                            notice_content = $"{{\"name\":\"{c.name}\",\"msg\":\"生日，快去送上祝福吧！\"}}";
                            employeeSocket.Add(new WebSocketModel { userId = c.to_director_id, content = notice_content });
                            var employeenotice = new List<employeeMes>();
                            employeenotice.Add(new employeeMes { employee_id = c.to_director_id, employee = c.to_director });

                            notice.NewMethod(archives.id.ToString(), archives, 0, notice, noticeList, 10, 2, notice_content, birthday, employeenotice);


                        });

                    }
                    else
                    {
                        birthday_archives = new List<HomeModel>();
                    }


                }

            }

            if (festival.Count > 0)
            {
                //查询消息中有则不需要通知
                var isExist = await Db.Queryable<oa_notice, oa_notice_employee>((a, b) => new object[] { JoinType.Left, a.id == b.id })
                                    .Where((a, b) => a.org_id == userInfo.org_id && a.notice_type_id == 10 && a.notice_property_id == 1 && b.employee_id == userInfo.id && festival.Contains(a.name))
                                    .WithCache()
                                    .AnyAsync();

                if (!isExist)
                {
                    festival.ForEach(d =>
                    {
                        var archives = new c_archives();
                        archives.name = " ";
                        archives.id = 0;
                        //调用通知
                        notice_content = $"{{\"name\":\"{d}\",\"msg\":\"祝您节日快乐！\"}}";
                        employeeSocket.Add(new WebSocketModel { userId = userInfo.id, content = notice_content });
                        var employeenotice = new List<employeeMes>();
                        employeenotice.Add(new employeeMes { employee_id = userInfo.id, employee = userInfo.name });

                        notice.NewMethod(archives.id.ToString(), archives, 0, notice, noticeList, 10, 1, notice_content, d, employeenotice);


                    });

                }
                else
                {
                    festival = new List<string>();
                }

            }
            //新增
            await notice.AddNoticeAsync(noticeList);
            //消息提醒
            ChatWebSocketMiddleware.SendListAsync(employeeSocket);


            return new { festival, birthday_archives };
        }

        /// <summary>
        /// 获取随访计划
        /// </summary>
        /// <returns></returns>
        public async Task<object> GetFollow()
        {
            //redis key
            string key = "followUp" + userInfo.id + DateTime.Now.ToString("yyyyMMdd");
            var isCache = redisCache.Get<List<FollowUpModel>>(key);
            //查询随访缓存
            var followup_archives = new List<FollowUpModel>();
            if (isCache != null && isCache.Count > 0)
            {
                followup_archives = isCache;
            }
            else
            {
                followup_archives = await Db.Queryable<r_follow_up, c_archives>((f, a) => new object[] { JoinType.Left, f.archives_id == a.id })
                .Where((f, a) => f.state == 16 && ((a.to_director_id == userInfo.id && f.client_id == null) || f.client_id == userInfo.id))
                .Where($"f.task_date='{DateTime.Now.ToString("yyyy-MM-dd")}'")
                .Select((f, a) => new FollowUpModel { id = f.id, name = f.archives, isRead = false, image_url = a.image_url })
                .ToListAsync();

                // redisCache.Add(key, followup_archives, 86400);
            }

            return followup_archives;
        }

        /// <summary>
        /// 获取余额下限的用户
        /// </summary>
        public async Task<object> GetLowerBalance()
        {
            //redis key
            string key = "balanceLower" + userInfo.id + DateTime.Now.ToString("yyyyMMdd");
            var isCache = redisCache.Get<List<BalanceLowerModel>>(key);
            //查询余额下限缓存
            var balance_archives = new List<BalanceLowerModel>();
            if (isCache != null && isCache.Count > 0)
            {
                balance_archives = isCache;
            }
            else
            {
                //查询客户余额下限
                balance_archives = await Db.Queryable<c_archives, c_account, c_archives_level>((a, ac, al) => new object[] { JoinType.Left, ac.archives_id == a.id, JoinType.Left, a.grade_id == al.id })
                             .Where((a, ac, al) => a.org_id == userInfo.org_id && ac.balance <= al.balance_limit_lower && a.to_director_id == userInfo.id)
                             .Select((a, ac, al) => new BalanceLowerModel { name = a.name, id = a.id, isRead = false, image_url = a.image_url })
                             .ToListAsync();

                // redisCache.Add(key, balance_archives, 86400);
            }

            return balance_archives;
        }

        /// <summary>
        /// 修改生日已读状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task UpdateBirthday(int id)
        {
            //redis key
            string key = "birthday" + userInfo.id + DateTime.Now.ToString("yyyyMMdd");
            var isCache = redisCache.Get<List<HomeModel>>(key);
            var newFestivals = isCache.Select(s => new HomeModel { id = s.id, age = s.age, name = s.name, phone = s.phone, sex = s.sex, isRead = s.id == id ? true : false }).ToList();

            redisCache.Remove<object>("birthday" + userInfo.id + DateTime.Now.ToString("yyyyMMdd"));

            redisCache.Add(key, newFestivals, 86400);
        }

        /// <summary>
        /// 修改随访状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task UpdateFollowUp(int id)
        {
            //redis key
            string key = "followUp" + userInfo.id + DateTime.Now.ToString("yyyyMMdd");
            var isCache = redisCache.Get<List<FollowUpModel>>(key);
            var newFollowUp = isCache.Select(s => new FollowUpModel { id = s.id, name = s.name, isRead = s.id == id ? true : false }).ToList();

            redisCache.Remove<object>("followUp" + userInfo.id + DateTime.Now.ToString("yyyyMMdd"));

            redisCache.Add(key, newFollowUp, 86400);
        }

        /// <summary>
        /// 修改余额下限状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task UpdateBalanceLower(int id)
        {
            //redis key
            string key = "balanceLower" + userInfo.id + DateTime.Now.ToString("yyyyMMdd");
            var isCache = redisCache.Get<List<BalanceLowerModel>>(key);
            var newFollowUp = isCache.Select(s => new BalanceLowerModel { id = s.id, name = s.name, isRead = s.id == id ? true : false }).ToList();

            redisCache.Remove<object>("balanceLower" + userInfo.id + DateTime.Now.ToString("yyyyMMdd"));

            redisCache.Add(key, newFollowUp, 86400);
        }
    }
}
