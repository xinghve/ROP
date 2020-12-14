using System;
using System.Collections.Generic;
using System.IO;
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
    /// 报损报溢
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class LossOverflowController : ControllerBase
    {
        private readonly ILossOverflowService _lossOverflowService;
        /// <summary>
        /// 报损报溢构造
        /// </summary>
        /// <param name="lossOverflowService"></param>
        public LossOverflowController(ILossOverflowService lossOverflowService)
        {
            _lossOverflowService = lossOverflowService;
        }

        /// <summary>
        /// 新增报损报溢
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<bool> AddAsync([FromBody]LossOverflow entity)
        {
            return await _lossOverflowService.AddAsync(entity);
        }

        /// <summary>
        /// 撤销报损报溢
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpDelete("Cancel")]
        public async Task<bool> CancelAsync([FromBody]bus_loss_overflow entity)
        {
            return await _lossOverflowService.CancelAsync(entity);
        }

        /// <summary>
        /// 报损报溢分页
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetPage")]
        public async Task<Page<LossOverflow>> GetPageAsync([FromQuery]LossOverflowPageSearch entity)
        {
            return await _lossOverflowService.GetPageAsync(entity);
        }

        /// <summary>
        /// 报损报溢详情
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        [HttpGet("GetLossDetail")]
        public async Task<LossOverflow> GetLossDetail([FromQuery]string no)
        {
            return await _lossOverflowService.GetLossDetail(no);
        }

        /// <summary>
        /// 导出表格
        /// </summary>
        /// <param name="list_no"></param>
        /// <returns></returns>
        [HttpGet("ExportAsync")]
        public async Task<IActionResult> ExportAsync([FromQuery]string list_no)
        {
            var excelFilePath = await _lossOverflowService.ExportAsync(list_no.Split(",").ToList());
            return File(new FileStream(excelFilePath, FileMode.Open), "application/octet-stream", "报损(溢)单.xlsx");
        }
    }
}