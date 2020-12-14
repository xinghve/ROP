using Models.DB;
using Models.View.Public;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tools;

namespace Service.Repository.Interfaces.Public
{
    /// <summary>
    /// 部门
    /// </summary>
    public interface IDeptService
    {
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <returns></returns>
        Task<bool> AddAsync(DeptExtent entity);

        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <returns></returns>
        Task<bool> ModifyAsync(DeptExtent dept);

        /// <summary>
        /// 修改状态（状态（0=停用，1=启用））
        /// </summary>
        /// <returns></returns>
        Task<int> ModifyDisabledAsync(p_dept dept);

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <returns></returns>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// 复制部门信息到门店
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> CopyAsync(DeptCopy entity);

        /// <summary>
        /// 移除门店部门
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> RemoveAsync(DeptRemove entity);

        /// <summary>
        /// 获得列表
        /// </summary>
        /// <param name="model">名称</param>
        /// <returns></returns>
        Task<Page<DeptModel>> GetPagesAsync(DeptSearchModel model);

        /// <summary>
        /// 数据列表
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="store_id">门店ID</param>
        /// <param name="disabled_code">状态（-1=所有，0=停用，1=启用）</param>
        /// <param name="nature">性质</param>
        /// <param name="sign">是否集团私有(3=否； 2=是； -1=所有)</param>
        /// <returns></returns>
        Task<List<p_dept>> GetListAsync(string name, int store_id, int disabled_code, int nature, short sign = -1);

        /// <summary>
        /// 数据列表（门店选择复制部门）
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <returns></returns>
        Task<List<p_dept>> GetDeptListAsync(int store_id);

        /// <summary>
        /// 获取挂号科室列表
        /// </summary>
        /// <returns></returns>
        Task<List<DeptCusModel>> GetCusList(int storeId);
        /// <summary>
        /// 获取科室简介
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        Task<DeptCusDetail> GetDeptIntroduce(int deptId);

        /// <summary>
        /// 获取当前登录人所属部门集合
        /// </summary>
        /// <returns></returns>
        Task<List<p_dept>> GetCurrentDept(int store_id);
    }
}
