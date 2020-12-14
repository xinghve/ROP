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
    /// 项目接口
    /// </summary>
    public interface IItemService
    {
        /// <summary>
        /// 项目分页查询
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<Page<h_item>> GetItemAsync(ItemSearch model);

        /// <summary>
        /// 添加项目（异步）
        /// </summary>
        /// <param name="item">项目</param>
        /// <returns></returns>
        Task<int> AddItemAsync(ItemModel item);

        /// <summary>
        /// 编辑项目（异步）
        /// </summary>
        /// <param name="item">项目</param>
        /// <returns></returns>
        Task<bool> ModifyItemAsync(ItemModel item);

        /// <summary>
        /// 获取费别下拉数据
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>
        Task<object> GetFeeList(int typeId);

        /// <summary>
        /// 根据费别获取项目跟规格
        /// </summary>
        /// <param name="feeType">费别id</param>
        /// <param name="name">项目名称</param>
        /// <returns></returns>
        Task<List<ItemSpecModel>> GetItemSpecAsync(int feeType, string name);

        /// <summary>
        /// 删除项目
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync(int id);
        /// <summary>
        /// 开方项目分页
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<Page<h_item>> GetItemPageAsync(ItemPage entity);

        /// <summary>
        /// 获取设备项目
        /// </summary>
        /// <returns></returns>
        Task<Page<EquipmentItem>> GetEquipmentList(ItemPage entity);
    }
}
