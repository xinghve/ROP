using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Models.View.Business;
using Service.Repository.Interfaces.Business;
using Tools;

namespace ROP.Controllers.Business
{
    /// <summary>
    /// 物品领用
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RequisitionController : ControllerBase
    {
        private readonly IRequisitionService _requistionService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requistionService"></param>
        public RequisitionController(IRequisitionService requistionService)
        {
            _requistionService = requistionService;
        }

        /// <summary>
        /// 添加领用
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("AddRequisition")]
        public async Task<bool> AddRequisition([FromBody]RequisitionAddModel entity)
        {
            return await _requistionService.AddRequisition(entity);
        }

        /// <summary>
        /// 取消领用
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("CancelRequisition")]
        public async Task<int> CancelRequisition([FromBody]CancelRequisitionModel entity)
        {
            return await _requistionService.CancelRequisition(entity);
        }

        /// <summary>
        /// 个人固资领用记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetOwnRequisition")]
        public async Task<Page<bus_grant_detail>> GetOwnRequisition([FromQuery]GrantSearch entity)
        {
            return await _requistionService.GetOwnRequisition(entity);
        }

        /// <summary>
        /// 个人申领记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetOwnRequisitionApply")]
        public async Task<Page<RequisitionPageModel>> GetOwnRequisitionApply([FromQuery]RequisitionRecordSearch entity)
        {
            return await _requistionService.GetOwnRequisitionApply(entity);
        }

        /// <summary>
        /// 获取领用详情
        /// </summary>
        /// <param name="apply_no"></param>
        /// <returns></returns>
        [HttpGet("GetRequisitionDetail")]
        public async Task<RequisitionDetail> GetRequisitionDetail([FromQuery]string apply_no)
        {
            return await _requistionService.GetRequisitionDetail(apply_no);
        }

        /// <summary>
        /// 获取审核通过领用数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetRequisitionRecord")]
        public async Task<Page<RequisitionPageModel>> GetRequisitionRecord([FromQuery]RequisitionRecordSearch entity)
        {
            return await _requistionService.GetRequisitionRecord(entity);
        }

        /// <summary>
        /// 物资领用发放
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("GrantRequisition")]
        public async Task<bool> GrantRequisition([FromBody]RequisitionAddModel entity)
        {
            return await _requistionService.GrantRequisition(entity);
        }

        /// <summary>
        /// 所有发放记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("AllGrantRecord")]
        public async Task<Page<AllGrantModel>> AllGrantRecord([FromQuery]GrantSearch entity)
        {
            return await _requistionService.AllGrantRecord(entity);
        }

        /// <summary>
        /// 所有固定资产记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("AllAssentGrantRecord")]
        public async Task<Page<AllGrantModel>> AllAssentGrantRecord([FromQuery]GrantSearch entity)
        {
            return await _requistionService.AllAssentGrantRecord(entity);
        }

        /// <summary>
        /// 获取供应商
        /// </summary>
        /// <param name="std_item_id"></param>
        /// <param name="spec"></param>
        /// <param name="store_id"></param>
        /// <returns></returns>
        [HttpGet("GetManufactors")]
        public async Task<List<GetManufactor>> GetManufactors([FromQuery]string name,[FromQuery]int std_item_id, [FromQuery]string spec, [FromQuery]int store_id)
        {
            return await _requistionService.GetManufactors(name,std_item_id, spec,store_id);
        }

        /// <summary>
        /// 物资归还
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("ReturnRequisition")]
        public async Task<bool> ReturnRequisition([FromBody]ReturnModel entity)
        {
            return await _requistionService.ReturnRequisition(entity);
        }
    }
}