using Models.DB;
using Models.View.His;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace Service.Repository.Interfaces.His
{
    /// <summary>
    /// 套餐
    /// </summary>
    public interface IPackageService
    {
        /// <summary>
        /// 获取套餐分页数据
        /// </summary>
        /// <returns></returns>
        Task<Page<PackageModel>> GetPageAsync(PackSearch entity);

        /// <summary>
        /// 根据套餐获取项目
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<Page<his_applycontent>> GetItemPageAsync(PackItemSearch entity);

        /// <summary>
        /// 套餐列表
        /// </summary>
        /// <returns></returns>
        Task<List<PackageModel>> GetList(int typeId,int storeId);

        /// <summary>
        /// 添加套餐
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> AddAsync(PackageModel entity);

        /// <summary>
        /// 编辑套餐名
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> UpdateAsync(PackageUpdateModel entity);

        /// <summary>
        /// 编辑套餐
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> ModifyAsync(PackageEntity entity);

        /// <summary>
        /// 删除套餐
        /// </summary>
        /// <param name="packid"></param>
        /// <param name="storeId"></param>
        /// <param name="type_id"></param>
        /// <returns></returns>
        Task<int> DeleteAsync(string packid,int storeId,int type_id);
    }
}
