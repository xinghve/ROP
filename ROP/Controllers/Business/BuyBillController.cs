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
    /// 采购单
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BuyBillController : ControllerBase
    {
        private readonly IBuyBillService _buyBillService;
        /// <summary>
        /// 采购单构造
        /// </summary>
        /// <param name="buyBillService"></param>
        public BuyBillController(IBuyBillService buyBillService)
        {
            _buyBillService = buyBillService;
        }
        /// <summary>
        /// 添加采购单
        /// </summary>
        /// <param name="buyBills">采购单实体</param>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<bool> AddAsync([FromBody] List<BuyBill> buyBills)
        {
            return await _buyBillService.AddAsync(buyBills);
        }

        /// <summary>
        /// 根据采购单ID获取采购单信息
        /// </summary>
        /// <param name="bill_no">采购单号</param>
        /// <returns></returns>
        [HttpGet("Get")]
        public async Task<BuyBill> GetAsync([FromQuery] string bill_no)
        {
            return await _buyBillService.GetAsync(bill_no);
        }

        /// <summary>
        /// 获得分页列表（采购单明细）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetDetialsPage")]
        public async Task<Page<bus_buy_bill_detials>> GetDetialsPageAsync([FromQuery] BuyBillDetialsPageSearch entity)
        {
            return await _buyBillService.GetDetialsPageAsync(entity);
        }

        /// <summary>
        /// 获得分页列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetPage")]
        public async Task<Page<BuyBill>> GetPageAsync([FromQuery] BuyBillPageSearch entity)
        {
            return await _buyBillService.GetPageAsync(entity);
        }

        /// <summary>
        /// 编辑采购单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("Modify")]
        public async Task<int> ModifyAsync([FromBody] bus_buy_bill entity)
        {
            return await _buyBillService.ModifyAsync(entity);
        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <returns></returns>
        [HttpDelete("Cancel")]
        public async Task<bool> CancelAsync([FromBody] List<string> list)
        {
            return await _buyBillService.CancelAsync(list);
        }

        /// <summary>
        /// 获取指定采购单号生成采购单时勾选的所有采购申请单生成的所有采购单号
        /// </summary>
        /// <param name="No"></param>
        /// <returns></returns>
        [HttpGet("GetLinkNo")]
        public async Task<List<string>> GetLinkNoAsync([FromQuery]string No)
        {
            return await _buyBillService.GetLinkNoAsync(No);
        }

        /// <summary>
        /// 完成
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("Finish")]
        public async Task<bool> FinishAsync([FromBody]bus_buy_bill entity)
        {
            return await _buyBillService.FinishAsync(entity);
        }

        /// <summary>
        /// 导出到Excel
        /// </summary>
        /// <returns></returns>
        [HttpGet("Export")]
        public async Task<IActionResult> ExportExcel([FromQuery]string No)
        {
            var excelFilePath = await _buyBillService.ExportAsync(No);

            //下边这个是返回文件需要前端处理   
            return File(new FileStream(excelFilePath, FileMode.Open), "application/octet-stream", "采购单.xlsx");
            //下边这种是返回下载连接,可通过浏览器直接访问下载
            //return excelFilePath;

        }
    }
}