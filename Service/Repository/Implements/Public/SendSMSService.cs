using Models.DB;
using Models.View.Public;
using Newtonsoft.Json;
using Service.Extensions;
using Service.Repository.Interfaces.Public;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Tools;
using Tools.Aliyun;
using Tools.Filter;

namespace Service.Repository.Implements.Public
{
    /// <summary>
    /// 发送短信
    /// </summary>
    public class SendSMSService : DbContext, ISendSMSService
    {
        /// <summary>
        /// 获得列表
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="template_id">模板ID</param>
        /// <param name="phone">手机号</param>
        /// <param name="status">状态（1：成功；0：失败；-1：所有）</param>
        /// <param name="code">内容</param>
        /// <param name="is_all">是否所有</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        public async Task<Page<p_sms_send_record>> GetPageAsync(int store_id, int template_id, string phone, int status, string code, bool is_all, string order, int orderType, int limit, int page)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var orderTypeStr = " Asc";
            if (orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            //查询门店
            return await Db.Queryable<p_sms_send_record>()
                .Where(w => w.org_id == userInfo.org_id)
                .WhereIF(store_id != -1, w => w.store_id == store_id)
                .WhereIF(status != -1, w => w.status == status)
                .WhereIF(template_id > 0, w => w.template_id == template_id)
                .WhereIF(!is_all, w => w.sender_id == userInfo.id)
                .WhereIF(!string.IsNullOrEmpty(phone), w => w.phone_no.Contains(phone))
                .WhereIF(!string.IsNullOrEmpty(code), w => w.code.Contains(code))
                .OrderBy(order + orderTypeStr)
                .WithCache()
                .ToPageAsync(page, limit);
        }

        /// <summary>
        /// 短信发送
        /// </summary>
        /// <param name="phone_no">手机号</param>
        /// <param name="template_id">模板ID</param>
        /// <param name="toValues">模板ID</param>
        /// <param name="content">内容</param>
        /// <param name="type">类型</param>
        /// <param name="org_id">集团ID</param>
        public async Task<bool> SendSmsAsync(string phone_no, int template_id, string toValues, string content, int type = 0, int org_id = 0)
        {
            if (!string.IsNullOrEmpty(phone_no) && phone_no.Trim().Length == 11)
            {
                var userInfo = new Tools.IdentityModels.GetUser.UserInfo();
                if (type == 0)
                {
                    var claims = httpContextAccessor.HttpContext.User.Claims;
                    if (claims.Count() > 0)
                    {
                        //获取用户信息
                        userInfo = new Tools.IdentityModels.GetUser().userInfo;
                    }
                }
                var archives = await Db.Queryable<c_archives>().Where(w => w.phone == phone_no && w.org_id == userInfo.org_id).WithCache().FirstAsync();
                if (archives == null)
                {
                    //throw new MessageException("请输入正确的手机号");
                    archives = new c_archives { id = 0, phone = phone_no, name = "", id_card = "" };
                }
                if (type == 1)
                {
                    userInfo.org_id = org_id;
                    userInfo.name = archives.name;
                    userInfo.id = archives.id;
                }
                var result = await SendAsync(archives, template_id, toValues, content, userInfo, 0);
                if (!result)
                {
                    throw new MessageException("发送失败");
                }
                return result;
            }
            else
            {
                throw new MessageException("手机号格式不正确");
            }
        }

        /// <summary>
        /// 短信批量发送
        /// </summary>
        /// <param name="sendSMSGroup"></param>
        public async Task<bool> SendBatchSmsAsync(SendSMSGroup sendSMSGroup)
        {
            var userInfo = new Tools.IdentityModels.GetUser.UserInfo();
            var claims = httpContextAccessor.HttpContext.User.Claims;
            if (claims.Count() > 0)
            {
                //获取用户信息
                userInfo = new Tools.IdentityModels.GetUser().userInfo;
            }
            var phoneList = sendSMSGroup.phoneList;
            //查询所有档案手机号
            if (sendSMSGroup.isAll)
            {
                phoneList = await Db.Queryable<c_archives>().Where(w => w.store_id == sendSMSGroup.storeID && w.org_id == userInfo.org_id).GroupBy(g => new { g.phone }).Select(s => s.phone).WithCache().ToListAsync();
            }
            var archivesList = await Db.Queryable<c_archives>().Where(w => w.store_id == sendSMSGroup.storeID && w.org_id == userInfo.org_id && phoneList.Contains(w.phone)).WithCache().ToListAsync();
            foreach (var archives in archivesList)
            {
                if (archives.phone.Trim().Length == 11)
                {
                    await SendAsync(archives, sendSMSGroup.templateId, sendSMSGroup.toValues, sendSMSGroup.content, userInfo, sendSMSGroup.storeID);
                }
            }
            return true;
        }

        private async Task<bool> SendAsync(c_archives archives, int template_id, string toValues, string content, Tools.IdentityModels.GetUser.UserInfo userInfo, int store_id)
        {
            //获取模板
            var template = await Db.Queryable<p_sms_template>().Where(w => w.id == template_id).WithCache().FirstAsync();
            if (template == null)
            {
                throw new MessageException("请选择正确短信模板进行发送");
            }
            string message = string.Empty;
            //获取签名
            var autograph = await Db.Queryable<p_sms_autograph>().WithCache().FirstAsync();
            SMS.AccountEntity accountEntity = new SMS.AccountEntity { AccessKeyId = autograph.access_key_id, FreeSignName = autograph.free_sign_name, SecretAccessKey = autograph.secret_access_key };
            var ssr = new p_sms_send_record
            {
                autograph_id = autograph.id,
                code = content,
                error = "",
                org_id = userInfo.org_id,
                phone_no = archives.phone,
                scene = template.name,
                send_time = DateTime.Now,
                status = 1,
                template_id = template.id,
                store_id = store_id,
                expire_time = DateTime.Now.AddMinutes(int.Parse(ConfigExtensions.Configuration["BaseConfig:CodeExpireMinute"])),
                sender = userInfo.name,
                sender_id = userInfo.id,
                archives = archives.name,
                archives_id = archives.id,
                id_card = archives.id_card
            };
            if (SMS.sendSMS(accountEntity, template.code, toValues, archives.phone.Trim(), out message))
            {
                await Db.Insertable(ssr).RemoveDataCache().ExecuteCommandAsync();
                return true;
            }
            ssr.error = message;
            ssr.status = 0;
            await Db.Insertable(ssr).RemoveDataCache().ExecuteCommandAsync();
            return false;
        }
    }
}
