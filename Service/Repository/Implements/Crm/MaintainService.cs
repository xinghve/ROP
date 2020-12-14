using Models.DB;
using Models.View.Business;
using Service.Extensions;
using Service.Repository.Interfaces.Crm;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Tools;
using Tools.Filter;
using Tools.WebSocket;

namespace Service.Repository.Implements.Crm
{
    /// <summary>
    /// 客户关系维护
    /// </summary>
    public class MaintainService : DbContext, IMaintainService
    {
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <returns></returns>
        public async Task<bool> AddAsync(c_maintain maintain)
        {
            //通知
            var notice = new Business.NoticeService();
            var noticeList = new List<AddNoticeModel>();
            var notice_content = "";
            var employeeSocket = new List<WebSocketModel>();

            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var result = await Db.Ado.UseTranAsync(() =>
            {
                //获取档案信息
                var archives = Db.Queryable<c_archives>().Where(w => w.id == maintain.archives_id).WithCache().First();
                //获取最后一次转移记录
                var lastMaintain = Db.Queryable<c_maintain>().Where(w => w.archives_id == maintain.archives_id).OrderBy(o => o.id, SqlSugar.OrderByType.Desc).WithCache().First();
                maintain.archives = archives.name;
                maintain.archives_phone = archives.phone;
                maintain.creater = userInfo.name;
                maintain.creater_id = userInfo.id;
                maintain.create_date = DateTime.Now;
                maintain.from_director = lastMaintain.to_director;
                maintain.from_director_id = lastMaintain.to_director_id;
                maintain.from_store_id = lastMaintain.to_store_id;
                maintain.from_store_name = lastMaintain.to_store_name;
                maintain.id_card = archives.id_card;
                maintain.org_id = userInfo.org_id;
                Db.Insertable(maintain).RemoveDataCache().ExecuteCommand();//新增转移记录
                //修改档案门店
                Db.Updateable<c_archives>()
                .SetColumns(s => new c_archives { store_id = maintain.to_store_id.Value, to_director = maintain.to_director, to_director_id = maintain.to_director_id })
                .Where(w => w.id == maintain.archives_id)
                .RemoveDataCache()
                .EnableDiffLogEvent()
                .ExecuteCommand();              

                if (maintain.to_director_id>0)
                {
                    //发送通知
                    var con = $"{{\"name\":\"{archives.name}\",\"msg\":\"已转为您的客户！\",\"img_url\":\"{archives.image_url}\"}}";
                    employeeSocket.Add(new WebSocketModel { userId = maintain.to_director_id.Value, content = con });
                    
                    //客户转移通知
                    notice_content = $"{{\"name\":\"{archives.name}\",\"to_director\":\"{maintain.to_director}\",\"creater\":\"{maintain.creater}\",\"msg\":\" 客户已转移\"}}";
                    var employeenotice = new List<employeeMes>();
                    employeenotice.Add(new employeeMes { employee_id = maintain.to_director_id.Value, employee = maintain.to_director });

                    notice.NewMethod(maintain.archives_id.ToString(),archives, maintain.to_store_id.Value, notice, noticeList, 8, 1, notice_content, archives.name, employeenotice);
                    //新增
                    notice.AddNotice(noticeList);
                    //消息提醒
                    ChatWebSocketMiddleware.SendListAsync(employeeSocket);

                }

            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 获得列表
        /// </summary>
        /// <param name="name">名称/手机号/身份证</param>
        /// <param name="store_id">门店ID</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        public async Task<Page<c_maintain>> GetPagesAsync(string name, int store_id, string order, int orderType, int limit, int page)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var orderTypeStr = " Asc";
            if (orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            return await Db.Queryable<c_maintain>()
                .Where(w => w.org_id == userInfo.org_id)
                .WhereIF(!string.IsNullOrEmpty(name), w => w.archives.Contains(name) || w.archives_phone.Contains(name) || w.id_card.Contains(name))
                .OrderBy(order + orderTypeStr)
                .WithCache()
                .ToPageAsync(page, limit);
        }
    }
}
