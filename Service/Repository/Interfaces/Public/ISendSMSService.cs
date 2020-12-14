using Models.DB;
using Models.View.Public;
using System.Threading.Tasks;
using Tools;

namespace Service.Repository.Interfaces.Public
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISendSMSService
    {
        /// <summary>
        /// 短信发送
        /// </summary>
        /// <param name="phone_no">手机号</param>
        /// <param name="template_id">模板ID</param>
        /// <param name="toValues">模板ID</param>
        /// <param name="content">内容</param>
        /// <param name="type">类型</param>
        /// <param name="org_id">集团ID</param>
        Task<bool> SendSmsAsync(string phone_no, int template_id, string toValues, string content, int type = 0, int org_id = 0);

        /// <summary>
        /// 短信批量发送
        /// </summary>
        /// <param name="sendSMSGroup"></param>
        /// <returns></returns>
        Task<bool> SendBatchSmsAsync(SendSMSGroup sendSMSGroup);

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
        Task<Page<p_sms_send_record>> GetPageAsync(int store_id, int template_id, string phone, int status, string code, bool is_all, string order, int orderType, int limit, int page);
    }
}
