using Models.DB;
using Models.View.Business;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace Service.Repository.Interfaces.Business
{
    /// <summary>
    /// 通知接口
    /// </summary>
    public interface INoticeService
    {
        /// <summary>
        /// 添加通知
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> AddNoticeAsync(List<AddNoticeModel> entity);

        /// <summary>
        /// 获取通知分页数据
        /// </summary>
        /// <returns></returns>
        Task<object> GetNoticePage(NoticeSearchModel entity);

        /// <summary>
        /// 获取基础类型
        /// </summary>
        /// <param name="value_id"></param>
        /// <param name="property_id"></param>
        /// <returns></returns>
        b_basecode GetBaseDetail(short value_id, short property_id);

        /// <summary>
        /// 获取通知类型
        /// </summary>
        /// <returns></returns>
        List<b_basecode> GetBaseType();

        /// <summary>
        /// 标为已读
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> SetRead(NoticeRead entity);
    }
}
