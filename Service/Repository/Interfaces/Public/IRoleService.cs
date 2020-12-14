using Models.DB;
using Models.View.Public;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tools;

namespace Service.Repository.Interfaces.Public
{
    /// <summary>
    /// 角色
    /// </summary>
    public interface IRoleService
    {
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <returns></returns>
        Task<int> AddAsync(p_role role);

        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <returns></returns>
        Task<int> ModifyAsync(p_role role);

        /// <summary>
        /// 修改状态（状态（0=停用，1=启用））
        /// </summary>
        /// <returns></returns>
        Task<int> ModifyDisabledAsync(p_role role);

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <returns></returns>
        Task<int> DeleteAsync(int id);

        /// <summary>
        /// 复制角色信息到门店
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> CopyAsync(RoleCopy entity);

        /// <summary>
        /// 移除门店角色
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> RemoveAsync(RoleRemove entity);

        /// <summary>
        /// 获得列表
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="store_id">门店ID</param>
        /// <param name="dept_id">部门ID</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        Task<Page<Role>> GetPagesAsync(string name, int store_id, int dept_id, string order, int orderType, int limit, int page);

        /// <summary>
        /// 数据列表
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="store_id">门店ID</param>
        /// <param name="dept_id">部门ID</param>
        /// <returns></returns>
        Task<List<p_role>> GetListAsync(string name, int store_id, int dept_id);

        /// <summary>
        /// 数据列表（门店选择复制角色）
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="dept_id">部门ID</param>
        /// <returns></returns>
        Task<List<p_role>> GetRoleListAsync(int store_id, int dept_id);

        /// <summary>
        /// 流程管理用部门职位List
        /// </summary>
        /// <param name="is_org"></param>
        /// <param name="is_store"></param>
        /// <returns></returns>
        Task<List<ProcessDeptRoleModel>> GetDeptRole(bool is_org,bool is_store);
    }
}
