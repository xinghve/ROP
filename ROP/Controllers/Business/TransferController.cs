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
    /// 调拨
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TransferController : ControllerBase
    {
        private readonly ITransferService _transferService;
        /// <summary>
        /// 调拨构造
        /// </summary>
        /// <param name="transferService"></param>
        public TransferController(ITransferService transferService)
        {
            _transferService = transferService;
        }

        /// <summary>
        /// 添加调拨
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<bool> AddAsync([FromBody]Transfer entity)
        {
            return await _transferService.AddAsync(entity);
        }

        /// <summary>
        /// 添加调拨（草稿）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("AddDraft")]
        public async Task<bool> AddDraftAsync([FromBody]Transfer entity)
        {
            return await _transferService.AddDraftAsync(entity);
        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpDelete("Cancel")]
        public async Task<bool> CancelAsync([FromBody]bus_transfer_bill entity)
        {
            return await _transferService.CancelAsync(entity);
        }

        /// <summary>
        /// 获得分页列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetPage")]
        public async Task<Page<Transfer>> GetPageAsync([FromQuery]TransferPageSearch entity)
        {
            return await _transferService.GetPageAsync(entity);
        }

        /// <summary>
        /// 根据调拨单号获取调拨信息
        /// </summary>
        /// <param name="bill_no">调拨单号</param>
        /// <returns></returns>
        [HttpGet("GetTransfer")]
        public async Task<Transfer> GetTransferAsync([FromQuery]string bill_no)
        {
            return await _transferService.GetTransferAsync(bill_no);
        }

        /// <summary>
        /// 确认调入
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("VerifyIn")]
        public async Task<bool> VerifyInAsync([FromBody]bus_transfer_bill entity)
        {
            return await _transferService.VerifyInAsync(entity);
        }

        /// <summary>
        /// 确认调出
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("VerifyOut")]
        public async Task<bool> VerifyOutAsync([FromBody]Transfer entity)
        {
            return await _transferService.VerifyOutAsync(entity);
        }

        /// <summary>
        /// 编辑调拨单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("Modify")]
        public async Task<bool> ModifyAsync([FromBody]Transfer entity)
        {
            return await _transferService.ModifyAsync(entity);
        }

        /// <summary>
        /// 编辑调拨单（草稿）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("ModifyDraft")]
        public async Task<bool> ModifyDraftAsync([FromBody]Transfer entity)
        {
            return await _transferService.ModifyDraftAsync(entity);
        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("Commit")]
        public async Task<int> CommitAsync([FromBody]Transfer entity)
        {
            return await _transferService.CommitAsync(entity);
        }

        /// <summary>
        /// 删除草稿
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpDelete("Delete")]
        public async Task<bool> DeleteAsync([FromBody]bus_transfer_bill entity)
        {
            return await _transferService.DeleteAsync(entity);
        }

        /// <summary>
        /// 导出到Excel
        /// </summary>
        /// <returns></returns>
        [HttpGet("Export")]
        public async Task<IActionResult> ExportExcel([FromQuery]string No)
        {
            var excelFilePath = await _transferService.ExportAsync(No);

            //下边这个是返回文件需要前端处理   
            return File(new FileStream(excelFilePath, FileMode.Open), "application/octet-stream", "入库单.xlsx");
            //下边这种是返回下载连接,可通过浏览器直接访问下载
            //return excelFilePath;

        }
    }
}