using Models.View.Public;
using System.Threading.Tasks;

namespace Service.Repository.Interfaces.Public
{
    /// <summary>
    /// 角色权限
    /// </summary>
    public interface IRoleActionService
    {
        /// <summary>
        /// 设置角色权限
        /// </summary>
        /// <param name="roleAction"></param>
        /// <returns></returns>
        bool Set(RoleAction roleAction);

        /// <summary>
        /// 获取选中角色权限
        /// </summary>
        /// <param name="role_id">角色id</param>
        /// <returns></returns>
        Task<object> GetAsync(int role_id);
    }
}
