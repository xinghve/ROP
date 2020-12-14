using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.View.His;
using Service.Repository.Interfaces.His;
using Tools;
using Tools.Authorize;

namespace ROP.Controllers.Cus
{
    /// <summary>
    /// 康复预约记录（客户端）
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeForArchives]
    public class RecoverController : ControllerBase
    {
        private readonly IRecoverRegisterService _recoverRegisterService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="recoverRegisterService"></param>
        public RecoverController(IRecoverRegisterService recoverRegisterService)
        {
            _recoverRegisterService = recoverRegisterService;
        }

        /// <summary>
        /// 获取康复记录（客户端）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetRecordPCAsync")]
        public async Task<Page<RecoverRecordPC>> GetRecordPCAsync([FromQuery]RecoverPCSearch entity)
        {
            return await _recoverRegisterService.GetRecordPCAsync(entity);
        }

        /// <summary>
        /// 获取康复列表（客户端）
        /// </summary>
        /// <param name="store_id"></param>
        /// <param name="record_type">0进行中 1已完成</param>
        /// <returns></returns>
        [HttpGet("GetRecoverList")]
        public async Task<List<RecoverList>> GetRecoverList([FromQuery]int store_id,[FromQuery]int record_type)
        {
            return await _recoverRegisterService.GetRecoverList(store_id,record_type);
        }

        /// <summary>
        /// 获取康复列表详情（客户端）
        /// </summary>
        /// <param name="applyid"></param>
        /// <returns></returns>
        [HttpGet("GetRecoverListDetail")]
        public async Task<RecoverDetail> GetRecoverListDetail([FromQuery]int applyid, [FromQuery]int record_type)
        {
            return await _recoverRegisterService.GetRecoverListDetail(applyid, record_type);
        }
    }
}