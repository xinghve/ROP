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
    /// 出库
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class OutStorageController : ControllerBase
    {
        private readonly IOutStorageService _outStorageService;
        /// <summary>
        /// 出库构造
        /// </summary>
        /// <param name="outStorageService"></param>
        public OutStorageController(IOutStorageService outStorageService)
        {
            _outStorageService = outStorageService;
        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpDelete("Cancel")]
        public async Task<bool> CancelAsync([FromBody]bus_out_storage entity)
        {
            return await _outStorageService.CancelAsync(entity);
        }

        /// <summary>
        /// 根据出库单号获取出库信息
        /// </summary>
        /// <param name="bill_no">出库单号</param>
        /// <returns></returns>
        [HttpGet("GetOut")]
        public async Task<OutStorage> GetOutAsync([FromQuery]string bill_no)
        {
            return await _outStorageService.GetOutAsync(bill_no);
        }

        /// <summary>
        /// 获得分页列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetPage")]
        public async Task<Page<OutStorage>> GetPageAsync([FromQuery]OutStoragePageSearch entity)
        {
            return await _outStorageService.GetPageAsync(entity);
        }

        /// <summary>
        /// 导出到Excel
        /// </summary>
        /// <returns></returns>
        [HttpGet("Export")]
        public async Task<IActionResult> ExportExcel([FromQuery]string No)
        {
            var excelFilePath = await _outStorageService.ExportAsync(No);

            //下边这个是返回文件需要前端处理   
            return File(new FileStream(excelFilePath, FileMode.Open), "application/octet-stream", "入库单.xlsx");
            //下边这种是返回下载连接,可通过浏览器直接访问下载
            //return excelFilePath;

        }
    }
}