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
    /// 固定资产折旧
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AssetsDepreciationController : ControllerBase
    {
        private readonly IAssetsDepreciationService _assetsDepreciationService;
        /// <summary>
        /// 固定资产折旧构造
        /// </summary>
        /// <param name="assetsDepreciationService"></param>
        public AssetsDepreciationController(IAssetsDepreciationService assetsDepreciationService)
        {
            _assetsDepreciationService = assetsDepreciationService;
        }

        /// <summary>
        /// 新增资产折旧
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<bool> AddAsync([FromBody]AssetsDepreciation entity)
        {
            return await _assetsDepreciationService.AddAsync(entity);
        }

        /// <summary>
        /// 删除资产折旧
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpDelete("Delete")]
        public async Task<bool> DeleteAsync([FromBody]bus_assets_depreciation entity)
        {
            return await _assetsDepreciationService.DeleteAsync(entity);
        }

        /// <summary>
        /// 资产折旧单分页
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetPage")]
        public async Task<Page<AssetsDepreciation>> GetPageAsync([FromQuery]AssetsDepreciationPageSearch entity)
        {
            return await _assetsDepreciationService.GetPageAsync(entity);
        }

        /// <summary>
        /// 编辑资产折旧
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("Modify")]
        public async Task<bool> ModifyAsync([FromBody]AssetsDepreciation entity)
        {
            return await _assetsDepreciationService.ModifyAsync(entity);
        }

        /// <summary>
        /// 导出表格
        /// </summary>
        /// <param name="list_no"></param>
        /// <returns></returns>
        [HttpGet("ExportAsync")]
        public async Task<IActionResult> ExportAsync([FromQuery]string list_no)
        {
            var excelFilePath = await _assetsDepreciationService.ExportAsync(list_no.Split(",").ToList());
            return File(new FileStream(excelFilePath, FileMode.Open), "application/octet-stream", "固定资产折旧单.xlsx");
        }
    }
}