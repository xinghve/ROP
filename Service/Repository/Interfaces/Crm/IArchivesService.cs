using Models.DB;
using Models.View.Crm;
using Models.View.Mobile;
using Models.View.Public;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tools;

namespace Service.Repository.Interfaces.Crm
{
    /// <summary>
    /// 档案
    /// </summary>
    public interface IArchivesService
    {
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <returns></returns>
        Task<bool> AddAsync(Archives archives);

        /// <summary>
        /// 添加一条数据（移动端）
        /// </summary>
        /// <returns></returns>
        Task<bool> MobileAddAsync(MobileArchives archives);

        /// <summary>
        /// 添加一条数据（客户端注册）
        /// </summary>
        /// <returns></returns>
        Task<bool> ArcAddAsync(RegisterArchives archives);

        /// <summary>
        /// 添加一条数据（分销人员端）
        /// </summary>
        /// <returns></returns>
        Task<bool> DistributorAddAsync(MobileArchives archives);

        /// <summary>
        /// 根据档案ID获取信息
        /// </summary>
        /// <param name="archives_id"></param>
        /// <returns></returns>
        Task<ArchivesExtend> GetArchivesAsync(int archives_id);

        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <returns></returns>
        Task<bool> ModifyAsync(Archives archives);

        /// <summary>
        /// 修改一条数据（移动端）
        /// </summary>
        /// <returns></returns>
        Task<bool> MobileModifyAsync(MobileArchives archives);

        /// <summary>
        /// 修改一条数据（客户端）
        /// </summary>
        /// <returns></returns>
        Task<int> ArcModifyAsync(ArcArchives archives);

        /// <summary>
        /// 修改状态（状态（1=正常，0=黑名单））
        /// </summary>
        /// <returns></returns>
        Task<int> ModifyDisabledAsync(c_archives archives);

        /// <summary>
        /// 获得列表
        /// </summary>
        /// <param name="entity">查询对象</param>
        /// <returns></returns>
        Task<Page<ArchivesExtend>> GetPagesAsync(ArchivesSearch entity);

        /// <summary>
        /// 获取档案列表
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="is_all">是否所有</param>
        /// <param name="name">姓名/手机号</param>
        /// <returns></returns>
        Task<dynamic> GetListAsync(int store_id, bool is_all, string name);

        /// <summary>
        /// 获取档案分页列表（分销人员端）
        /// </summary>
        /// <returns></returns>
        Task<dynamic> GetPagesByDistributorAsync(Search entity);

        /// <summary>
        /// 获取客户量
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<dynamic> GetArchivesCountAsync(short type);

        /// <summary>
        /// 根据Id获取客户头像
        /// </summary>
        /// <returns></returns>
        Task<string> GetPhotoAsync();

        /// <summary>
        /// 根据Id修改客户头像
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> ModifyPhotoAsync(ArcPhoto entity);

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="org_id"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<dynamic> Login(string userName, string password, int org_id, string code);

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
        /// 获取券码是否存在
        /// </summary>
        /// <param name="coupon_no"></param>
        /// <returns></returns>
        Task<object> GetCoupon(string coupon_no);

        /// <summary>
        /// 档案导入
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> ArchivesImportAsync(ArchivesImport entity);

        /// <summary>
        /// 设置会员等级
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> SetLevelAsync(c_archives entity);
    }
}
