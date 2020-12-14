using Models.DB;
using Models.View.Business;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace Service.Repository.Interfaces.Business
{
    /// <summary>
    /// 库存
    /// </summary>
    public interface IStorageService
    {

        /// <summary>
        /// 获得分页列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<Page<bus_storage>> GetPageAsync(StoragePageSearch entity);

        /// <summary>
        /// 获得分页列表（明细）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<Page<bus_storage_detials>> GetDetialsPageAsync(StorageDetialsPageSearch entity);

        /// <summary>
        /// 获取集团所有库存明细（根据项目、规格、厂家）
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        Task<List<StorageDetials>> GetDetialsListAsync(List<StorageDetials> list);

        /// <summary>
        /// 获取库存明细（根据项目、规格、厂家）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<StorageDetials> GetDetialsAsync(StorageDetials entity);
    }
}
