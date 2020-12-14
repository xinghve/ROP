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
    /// 固定资产报废
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AssetsScrapController : ControllerBase
    {
        private readonly IAssetsScrapService _assetsScrapService;
        /// <summary>
        /// 固定资产报废构造
        /// </summary>
        /// <param name="assetsScrapService"></param>
        public AssetsScrapController(IAssetsScrapService assetsScrapService)
        {
            _assetsScrapService = assetsScrapService;
        }

        /// <summary>
        /// 新增资产报废
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<bool> AddAsync([FromBody]r_assets_scrap entity)
        {
            return await _assetsScrapService.AddAsync(entity);
        }

        /// <summary>
        /// 取消资产报废
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpDelete("Cancel")]
        public async Task<int> CancelAsync([FromBody]r_assets_scrap entity)
        {
            return await _assetsScrapService.CancelAsync(entity);
        }

        /// <summary>
        /// 资产报废单分页
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetPage")]
        public async Task<Page<AssetsScrap>> GetPageAsync([FromQuery]AssetsScrapPageSearch entity)
        {
            return await _assetsScrapService.GetPageAsync(entity);
        }

        /// <summary>
        /// 获取报废单信息
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        [HttpGet("GetAssetsScrap")]
        public async Task<AssetsScrap> GetAssetsScrapAsync([FromQuery]string no)
        {
            return await _assetsScrapService.GetAssetsScrapAsync(no);
        }

        /// <summary>
        /// 导出表格
        /// </summary>
        /// <param name="list_no"></param>
        /// <returns></returns>
        [HttpGet("ExportAsync")]
        public async Task<IActionResult> ExportAsync([FromQuery]string list_no)
        {
            var excelFilePath = await _assetsScrapService.ExportAsync(list_no.Split(",").ToList());
            return File(new FileStream(excelFilePath, FileMode.Open), "application/octet-stream", "报废单.xlsx");
        }
    }
}