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
    /// 康复预约接口
    /// </summary>
    public interface IRecoverRegisterService
    {
        /// <summary>
        /// 康复预约分页数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<Page<RecoverRegisterPage>> GetRecoverPageAsync(RecoverRegisterSearch entity);
        /// <summary>
        /// 查询是否可预约
        /// </summary>
        /// <returns></returns>
        Task<RecordIfUse> GetIfOrder(RecoverIfOrder entity);
        /// <summary>
        /// 添加康复预约记录
        /// </summary>
        /// <returns></returns>
        Task<bool> AddOrder(List<RecoreOrderAdd> entity);

        /// <summary>
        /// 康复预约记录分页
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<Page<RecoverRecordPage>> GetRecordPageAsync(RecoverRecordSearch entity);
        /// <summary>
        /// 康复预约改期
        /// </summary>
        /// <returns></returns>
        Task<bool> ModifyRecoverTime(ModifyModel entity);
        /// <summary>
        /// 取消预约
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> CancelRecoverTime(CancelModel entity);
        /// <summary>
        /// 康复分页数据（客户端）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<Page<RecoverRecordPC>> GetRecordPCAsync(RecoverPCSearch entity);
        /// <summary>
        /// 获取康复记录列表（客户端）
        /// </summary>
        /// <param name="store_id"></param>
        /// <param name="record_type">0进行中 1已完成</param>
        /// <returns></returns>
        Task<List<RecoverList>> GetRecoverList(int store_id, int record_type);

        /// <summary>
        /// 获取康复记录详情
        /// </summary>
        /// <param name="applyid"></param>
        /// <returns></returns>
        Task<RecoverDetail> GetRecoverListDetail(int applyid, int record_type);
    }
}
