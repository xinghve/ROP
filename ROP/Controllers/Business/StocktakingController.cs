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
    /// 盘点
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class StocktakingController : ControllerBase
    {
        private readonly IStocktakingService _stocktakingService;
        /// <summary>
        /// 盘点构造
        /// </summary>
        /// <param name="stocktakingService"></param>
        public StocktakingController(IStocktakingService stocktakingService)
        {
            _stocktakingService = stocktakingService;
        }

        /// <summary>
        /// 新增盘点
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<bool> AddAsync([FromBody]Stocktaking entity)
        {
            return await _stocktakingService.AddAsync(entity);
        }

        /// <summary>
        /// 删除盘点
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpDelete("Delete")]
        public async Task<int> DeleteAsync([FromBody]bus_stocktaking entity)
        {
            return await _stocktakingService.DeleteAsync(entity);
        }

        /// <summary>
        /// 结束盘点
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("End")]
        public async Task<int> EndAsync([FromBody]bus_stocktaking entity)
        {
            return await _stocktakingService.EndAsync(entity);
        }

        /// <summary>
        /// 盘点单分页
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetPage")]
        public async Task<Page<Stocktaking>> GetPageAsync([FromQuery]StocktakingPageSearch entity)
        {
            return await _stocktakingService.GetPageAsync(entity);
        }

        /// <summary>
        /// 编辑盘点
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("Modify")]
        public async Task<bool> ModifyAsync([FromBody]Stocktaking entity)
        {
            return await _stocktakingService.ModifyAsync(entity);
        }

        /// <summary>
        /// 开始盘点
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("Start")]
        public async Task<int> StartAsync([FromBody]bus_stocktaking entity)
        {
            return await _stocktakingService.StartAsync(entity);
        }

        /// <summary>
        /// 盘点数量录入
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("StocktakingNum")]
        public async Task<int> StocktakingNumAsync([FromBody]bus_stocktaking_detials entity)
        {
            return await _stocktakingService.StocktakingNumAsync(entity);
        }

        /// <summary>
        /// 待盘点物资分页
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetWaitStocktakingPage")]
        public async Task<Page<WaitStocktaking>> GetWaitStocktakingPageAsync([FromQuery]WaitStocktakingPageSearch entity)
        {
            return await _stocktakingService.GetWaitStocktakingPageAsync(entity);
        }

        /// <summary>
        /// 根据单号获取明细
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        [HttpGet("GetStocktakingDetials")]
        public async Task<List<StocktakingDetials>> GetStocktakingDetialsAsync([FromQuery]string no)
        {
            return await _stocktakingService.GetStocktakingDetialsAsync(no);
        }

        /// <summary>
        /// 导出表格
        /// </summary>
        /// <param name="list_no"></param>
        /// <returns></returns>
        [HttpGet("ExportAsync")]
        public async Task<IActionResult> ExportAsync([FromQuery]string list_no)
        {
            var excelFilePath = await _stocktakingService.ExportAsync(list_no.Split(",").ToList());
            return File(new FileStream(excelFilePath, FileMode.Open), "application/octet-stream", "物品资产盘点单.xlsx");
        }
    }
}