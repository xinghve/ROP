using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.View.Business;
using Service.Repository.Interfaces.Business;

namespace ROP.Controllers.Business
{
    /// <summary>
    /// 流程管理--ty
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProcessController : ControllerBase
    {
        private readonly IProcessService _processService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processService"></param>
        public ProcessController(IProcessService processService)
        {
            _processService = processService;
        }

        /// <summary>
        /// 流程数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetProcessAsync")]
        public async Task<List<ProcessModel>> GetProcessAsync([FromQuery]ProcessSearchModel entity)
        {
            return await _processService.GetProcessAsync(entity);
        }
        /// <summary>
        /// 新增流程
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("AddProcess")]
        public async Task<bool> AddProcess([FromBody]AddProcessModel entity)
        {
            return await _processService.AddProcess(entity);
        }
        /// <summary>
        /// 编辑流程
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("ModifyProcess")]
        public async Task<bool> ModifyProcess([FromBody]ModifyProcessModel entity)
        {
            return await _processService.ModifyProcess(entity);
        }
        /// <summary>
        /// 删除流程
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpDelete("DeleteProcess")]
        public async Task<int> DeleteProcess([FromBody]DeleteProcessModel entity)
        {
            return await _processService.DeleteProcess(entity);
        }
        /// <summary>
        /// 启用禁用流程
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("ProcessEnable")]
        public async Task<bool> ProcessEnable([FromBody]EnableProcessModel entity)
        {
            return await _processService.ProcessEnable(entity);
        }
        /// <summary>
        /// 获取请假部门信息
        /// </summary>
        /// <param name="store_id"></param>
        /// <param name="is_org"></param>
        /// <returns></returns>
        [HttpGet("GetLeavelProcessAsync")]
        public async Task<List<LeavelProcessModel>> GetLeavelProcessAsync([FromQuery]int store_id,[FromQuery]bool is_org)
        {
            return await _processService.GetLeavelProcessAsync(store_id,is_org);
        }

        /// <summary>
        /// 根据部门返回请假流程
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetLeavelAsync")]
        public async Task<List<ProcessModel>> GetLeavelAsync([FromQuery]ProcessLeaveSearchModel entity)
        {
            return await _processService.GetLeavelAsync(entity);
        }
    }
}