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
    /// 医疗室
    /// </summary>
    public interface IRoomService
    {
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <returns></returns>
        Task<bool> AddAsync(Room entity);

        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <returns></returns>
        Task<bool> ModifyAsync(Room entity);

        /// <summary>
        /// 获得分页列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<Page<Room>> GetPagesAsync(RoomPagesSearch entity);

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <returns></returns>
        Task<int> ModifyStateAsync(p_room entity);


        /// <summary>
        /// 获取医疗室列表
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="state">状态</param>
        /// <param name="equipment">是否存放设备</param>
        /// <returns></returns>
        Task<List<p_room>> GetListAsync(int store_id, short state, short equipment);

        /// <summary>
        /// 根据项目规格获取医疗室列表
        /// </summary>
        /// <param name="item_id">项目ID</param>
        /// <param name="spec_id">规格ID</param>
        /// <param name="dateTime">日期</param>
        /// <returns></returns>
        Task<dynamic> GetListByItemSpecAsync(int item_id, int spec_id, DateTime? dateTime);
    }
}
