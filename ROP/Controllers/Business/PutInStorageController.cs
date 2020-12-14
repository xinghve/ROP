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
    /// 入库
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PutInStorageController : ControllerBase
    {
        private readonly IPutInStorageService _putInStorageService;
        /// <summary>
        /// 入库构造
        /// </summary>
        /// <param name="putInStorageService"></param>
        public PutInStorageController(IPutInStorageService putInStorageService)
        {
            _putInStorageService = putInStorageService;
        }

        /// <summary>
        /// 添加入库
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<bool> AddAsync([FromBody] PutInStorage entity)
        {
            return await _putInStorageService.AddAsync(entity);
        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpDelete("Cancel")]
        public async Task<bool> CancelAsync([FromBody]bus_put_in_storage entity)
        {
            return await _putInStorageService.CancelAsync(entity);
        }

        /// <summary>
        /// 根据入库单号获取入库信息
        /// </summary>
        /// <param name="bill_no">入库单号</param>
        /// <returns></returns>
        [HttpGet("GetPutIn")]
        public async Task<PutInStorage> GetPutInAsync([FromQuery]string bill_no)
        {
            return await _putInStorageService.GetPutInAsync(bill_no);
        }

        /// <summary>
        /// 获得分页列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetPage")]
        public async Task<Page<PutInStorage>> GetPageAsync([FromQuery]PutInStoragePageSearch entity)
        {
            return await _putInStorageService.GetPageAsync(entity);
        }

        /// <summary>
        /// 导出到Excel
        /// </summary>
        /// <returns></returns>
        [HttpGet("Export")]
        public async Task<IActionResult> ExportExcel([FromQuery]string No)
        {
            var excelFilePath = await _putInStorageService.ExportAsync(No);

            //下边这个是返回文件需要前端处理   
            return File(new FileStream(excelFilePath, FileMode.Open), "application/octet-stream", "入库单.xlsx");
            //下边这种是返回下载连接,可通过浏览器直接访问下载
            //return excelFilePath;

        }
    }
}