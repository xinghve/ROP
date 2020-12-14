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
    /// 厂家接口
    /// </summary>
    public interface IManufactoryService
    {
        /// <summary>
        /// 厂家分页数据查询
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<Page<ManufactoryModel>> GetPageAsync(ManufactorySearch entity);

        /// <summary>
        /// 获取供应商List
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type_id">分类ID</param>
        /// <returns></returns>
        Task<List<ManufactoryList>> GetManufactoryList(string name, int type_id);

        /// <summary>
        /// 添加供应商
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> AddAsync(ManufactoryModel entity);

        /// <summary>
        /// 编辑供应商
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> ModifyAsync(ManufactoryModel entity);

        /// <summary>
        /// 删除供应商
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync(h_manufactor entity);

        /// <summary>
        /// 选择分类给供应商设置分类
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        Task<int> SetManufactorClass(ManufactorClassModel list);
        /// <summary>
        /// 修改供应商状态
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> SetState(ManufactorSetState entity);
    }
}
