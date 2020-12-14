using Models.DB;
using Models.View.Public;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace Service.Repository.Interfaces.Public
{
    /// <summary>
    /// 设备器械
    /// </summary>
    public interface IEquipmentService
    {
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <returns></returns>
        Task<bool> AddAsync(Equipment entity);

        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <returns></returns>
        Task<bool> ModifyAsync(Equipment entity);

        /// <summary>
        /// 获得分页列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<Page<EquipmentPages>> GetPagesAsync(EquipmentPagesSearch entity);

        /// <summary>
        /// 添加一条详细数据
        /// </summary>
        /// <returns></returns>
        Task<int> AddDetialsAsync(p_equipment_detials entity);

        /// <summary>
        /// 修改一条详细数据
        /// </summary>
        /// <returns></returns>
        Task<int> ModifyDetialsAsync(p_equipment_detials entity);

        /// <summary>
        /// 修改一条详细数据状态
        /// </summary>
        /// <returns></returns>
        Task<int> ModifyDetialsStateAsync(p_equipment_detials entity);

        /// <summary>
        /// 获得详细分页列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<Page<p_equipment_detials>> GetDetialsPagesAsync(EquipmentDetialsPagesSearch entity);

        /// <summary>
        /// 根据医疗室ID获取设备列表
        /// </summary>
        /// <param name="room_id">医疗室ID</param>
        /// <returns></returns>
        Task<dynamic> GetListAsync(int room_id);

        /// <summary>
        /// 根据项目规格获取设备列表
        /// </summary>
        /// <param name="item_id">项目ID</param>
        /// <param name="spec_id">规格ID</param>
        /// <param name="dateTime">日期</param>
        /// <returns></returns>
        Task<dynamic> GetListByItemSpecAsync(int item_id, int spec_id, DateTime? dateTime);
    }
}
