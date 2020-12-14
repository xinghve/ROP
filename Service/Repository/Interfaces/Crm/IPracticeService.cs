using Models.DB;
using Models.View.Crm;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tools;

namespace Service.Repository.Interfaces.Crm
{
    /// <summary>
    /// 随访记录
    /// </summary>
    public interface IPracticeService
    {
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <returns></returns>
        Task<int> AddAsync(r_practice entity);

        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <returns></returns>
        Task<int> ModifyAsync(r_practice entity);

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <returns></returns>
        //Task<int> DeleteAsync(List<int> vs);

        /// <summary>
        /// 获得列表
        /// </summary>
        /// <param name="name">名称/手机号/身份证</param>
        /// <param name="dateTimeStart">计划日期（开始）</param>
        /// <param name="dateTimeEnd">计划日期（结束）</param>
        /// <param name="store_id">门店ID</param>
        /// <param name="executor_id">执行人ID</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        Task<Page<r_practice>> GetPagesAsync(string name, string dateTimeStart, string dateTimeEnd, int store_id, int executor_id, string order, int orderType, int limit, int page);

       /// <summary>
       /// 执行
       /// </summary>
       /// <param name="id">回访id</param>
       /// <param name="answer">答复内容</param>
       /// <returns></returns>
        Task<int> ExecuteAsync(int id,string answer, string content);
    }
}
