using Models.View.Public;
using System.Threading.Tasks;

namespace Service.Repository.Interfaces.Public
{
    /// <summary>
    /// 注册
    /// </summary>
    public interface IRegisterService
    {
        /// <summary>
        /// 注册集团
        /// </summary>
        /// <returns></returns>
        Task<bool> RegisterAsync(Register register);
    }
}
