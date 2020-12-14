using Models.DB;
using Models.View.Mobile;
using Models.View.Public;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace Service.Repository.Interfaces.Public
{
    /// <summary>
    /// 分销人员
    /// </summary>
    public interface IDistributorService
    {        
        /// <summary>
        /// 注册
        /// </summary>
        /// <returns></returns>
        Task<int> RegisterAsync(DistributorRegister entity);

        /// <summary>
        /// 根据分销人员ID获取信息
        /// </summary>
        /// <param name="distributor_id">分销人员ID</param>
        /// <returns></returns>
        Task<p_distributor> GetDistributorAsync(int distributor_id);

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="org_id"></param>
        /// <param name="code"></param>
        /// <param name="open_id">微信open_id</param>
        /// <returns></returns>
        Task<dynamic> Login(string userName, string password, int org_id, string code, string open_id);

        /// <summary>
        /// 获得分页列表
        /// </summary>
        /// <param name="entity">查询对象</param>
        /// <returns></returns>
        Task<Page<DistributorPageModel>> GetPagesAsync(DistributorSearch entity);

        /// <summary>
        /// 忘记密码
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> ModifyPassword(ArcLoginPassword entity);

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> UpdatePassword(ArcLoginPassword entity);

        /// <summary>
        /// 根据Id获取分销人员头像
        /// </summary>
        /// <returns></returns>
        Task<string> GetPhotoAsync();

        /// <summary>
        /// 根据Id修改分销人员头像
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> ModifyPhotoAsync(ArcPhoto entity);

        /// <summary>
        /// 修改一条数据（分销人员）
        /// </summary>
        /// <returns></returns>
        Task<int> ModifyAsync(p_distributor entity);

        /// <summary>
        /// 启用禁用分销人员
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> ModifyStateAsync(DistributorModify entity);

        
    }
}
