using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Models.View.His;
using Service.Repository.Interfaces.His;
using Tools;

namespace ROP.Controllers.His
{
    /// <summary>
    /// 康复预约
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RecoverRegisterController : ControllerBase
    {
        private readonly IRecoverRegisterService _recoverRegisterService;
        /// <summary>
        /// 
        /// </summary>
        public RecoverRegisterController(IRecoverRegisterService recoverRegisterService)
        {
            _recoverRegisterService = recoverRegisterService;
        }
        /// <summary>
        /// 康复预约分页数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetRecoverPageAsync")]
        public async Task<Page<RecoverRegisterPage>> GetRecoverPageAsync([FromQuery]RecoverRegisterSearch entity)
        {
            return await _recoverRegisterService.GetRecoverPageAsync(entity);
        }
        /// <summary>
        /// 查询是否可预约
        /// </summary>
        /// <param name="enity"></param>
        /// <returns></returns>
        [HttpGet("GetIfOrder")]
        public async Task<RecordIfUse> GetIfOrder([FromQuery]RecoverIfOrder entity)
        {
            return await _recoverRegisterService.GetIfOrder(entity);
        }

        /// <summary>
        /// 添加预约记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("AddOrder")]
        public async Task<bool> AddOrder([FromBody]List<RecoreOrderAdd> entity)
        {
            return await _recoverRegisterService.AddOrder(entity);
        }

        /// <summary>
        /// 康复预约
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetRecordPageAsync")]
        public async Task<Page<RecoverRecordPage>> GetRecordPageAsync([FromQuery]RecoverRecordSearch entity)
        {
            return await _recoverRegisterService.GetRecordPageAsync(entity);
        }

        /// <summary>
        /// 康复预约改期
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("ModifyRecoverTime")]
        public async Task<bool> ModifyRecoverTime([FromBody]ModifyModel entity)
        {
            return await _recoverRegisterService.ModifyRecoverTime(entity);
        }
        /// <summary>
        /// 取消预约
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("CancelRecoverTime")]
        public async Task<bool> CancelRecoverTime([FromBody]CancelModel entity)
        {
            return await _recoverRegisterService.CancelRecoverTime(entity);
        }
    }
}