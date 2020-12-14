using Models.View.Public;
using System.Threading.Tasks;

namespace Service.Repository.Interfaces.Public
{
    /// <summary>
    /// 集团权限
    /// </summary>
    public interface IOrgActionService
    {
        /// <summary>
        /// 设置集团权限
        /// </summary>
        /// <param name="orgAction"></param>
        /// <returns></returns>
        bool Set(OrgAction orgAction);

        /// <summary>
        /// 获取选中集团权限
        /// </summary>
        /// <param name="org_id">集团id</param>
        /// <returns></returns>
        Task<object> GetAsync(int org_id);
    }
}
