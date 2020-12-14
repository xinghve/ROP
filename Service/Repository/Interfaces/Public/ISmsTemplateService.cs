using Models.DB;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tools;

namespace Service.Repository.Interfaces.Public
{
    /// <summary>
    /// 短信模板
    /// </summary>
    public interface ISmsTemplateService
    {
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <returns></returns>
        Task<int> AddAsync(p_sms_template entity);

        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <returns></returns>
        Task<int> ModifyAsync(p_sms_template entity);

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <returns></returns>
        Task<int> DeleteAsync(List<int> vs);

        /// <summary>
        /// 获得列表
        /// </summary>
        /// <param name="name">模版名称</param>
        /// <param name="is_select">是否可选（1=可选，0=不可选，-1=所有）</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        Task<Page<p_sms_template>> GetPagesAsync(string name, int is_select, string order, int orderType, int limit, int page);

        /// <summary>
        /// 数据列表
        /// </summary>
        /// <param name="name">模版名称</param>
        /// <param name="is_select">是否可选（1=可选，0=不可选，-1=所有）</param>
        /// <returns></returns>
        Task<List<p_sms_template>> GetListAsync(string name, int is_select);
    }
}
