using Models.View.Public;
using System.Threading.Tasks;

namespace Service.Repository.Interfaces.Public
{
    /// <summary>
    /// 门店权限
    /// </summary>
    public interface IStoreActionService
    {
        /// <summary>
        /// 设置门店权限
        /// </summary>
        /// <param name="storeAction"></param>
        /// <returns></returns>
        bool Set(StoreAction storeAction);

        /// <summary>
        /// 获取选中门店权限
        /// </summary>
        /// <param name="store_id">门店id</param>
        /// <returns></returns>
        Task<object> GetAsync(int store_id);
    }
}
