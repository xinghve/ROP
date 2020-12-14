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
    /// 物品领用
    /// </summary>
    public interface IRequisitionService
    {
        /// <summary>
        /// 添加物品领用
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> AddRequisition(RequisitionAddModel entity);

        /// <summary>
        /// 取消申请
        /// </summary>
        /// <returns></returns>
        Task<int> CancelRequisition(CancelRequisitionModel entity);

        /// <summary>
        /// 个人固资领用
        /// </summary>
        /// <returns></returns>
        Task<Page<bus_grant_detail>> GetOwnRequisition(GrantSearch entity);
        /// <summary>
        /// 个人申领记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<Page<RequisitionPageModel>> GetOwnRequisitionApply(RequisitionRecordSearch entity);

        /// <summary>
        /// 获取领用详情
        /// </summary>
        /// <param name="apply_no"></param>
        /// <returns></returns>
        Task<RequisitionDetail> GetRequisitionDetail(string apply_no);

        /// <summary>
        /// 获取审核通过的物资申领
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<Page<RequisitionPageModel>> GetRequisitionRecord(RequisitionRecordSearch entity);

        /// <summary>
        /// 发放物资
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> GrantRequisition(RequisitionAddModel entity);

        /// <summary>
        /// 所有发放记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<Page<AllGrantModel>> AllGrantRecord(GrantSearch entity);

        /// <summary>
        /// 所有固定资产记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<Page<AllGrantModel>> AllAssentGrantRecord(GrantSearch entity);

        /// <summary>
        /// 获取供应商
        /// </summary>
        /// <param name="std_item_id"></param>
        /// <param name="spec"></param>
        /// <param name="store_id"></param>
        /// <returns></returns>
        Task<List<GetManufactor>> GetManufactors(string name,int std_item_id,string spec,int store_id);

        /// <summary>
        /// 归还
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> ReturnRequisition(ReturnModel entity);

    }
}
