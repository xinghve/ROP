using Models.DB;
using Models.View.Public;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tools;

namespace Service.Repository.Interfaces.Public
{
    /// <summary>
    /// 门店接口
    /// </summary>
    public interface IStoreService
    {
        /// <summary>
        /// 根据门店ID获取门店信息
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <returns></returns>
        Task<p_store> GetAsync(int store_id);

        /// <summary>
        /// 添加门店
        /// </summary>
        /// <param name="entity">门店实体</param>
        /// <returns></returns>
        Task<bool> AddAsync(p_store entity);

        /// <summary>
        /// 编辑门店
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> ModifyAsync(p_store entity);

        /// <summary>
        /// 获得列表
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        Task<Page<p_store>> GetPageAsync(string name, string order, int orderType, int limit, int page);

        /// <summary>
        /// 修改状态（状态（0=停用，1=正常））
        /// </summary>
        /// <returns></returns>
        Task<int> ModifyUseStatusdAsync(p_store entity);

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <returns></returns>
        Task<int> DeleteAsync(List<int> vs);

        /// <summary>
        /// 获取门店列表
        /// </summary>
        /// <returns></returns>
        Task<List<Store>> GetListAsync(bool contain_org);

        /// <summary>
        /// 获取客户端门店列表
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        Task<List<Store>> GetStoreList(int orgId);

        /// <summary>
        /// 获取门店简介
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        Task<StoreIntroduceModel> GetStoreIntroduce(int storeId);
    }
}
