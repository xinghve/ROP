using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
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
    /// 固定资产
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AssetsController : ControllerBase
    {
        private readonly IAssetsService _assetsService;
        /// <summary>
        /// 固定资产构造
        /// </summary>
        /// <param name="assetsService"></param>
        public AssetsController(IAssetsService assetsService)
        {
            _assetsService = assetsService;
        }

        /// <summary>
        /// 获取固定资产分类列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetTypeList")]
        public async Task<List<b_codebase>> GetTypeListAsync()
        {
            return await _assetsService.GetTypeListAsync();
        }

        /// <summary>
        /// 获取基础物资列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetStdList")]
        public async Task<List<p_std_item>> GetStdListAsync([FromQuery]string name, [FromQuery]short type_id)
        {
            return await _assetsService.GetStdListAsync(name, type_id);
        }

        /// <summary>
        /// 获得分页列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetPage")]
        public async Task<Page<bus_assets>> GetPageAsync([FromQuery]AssetsPageSearch entity)
        {
            return await _assetsService.GetPageAsync(entity);
        }

        /// <summary>
        /// 固定资产流向
        /// </summary>
        /// <param name="id">固定资产ID</param>
        /// <returns></returns>
        [HttpGet("GetAssetsFlow")]
        public async Task<List<AssetsFlow>> GetAssetsFlowAsync([FromQuery]int id)
        {
            return await _assetsService.GetAssetsFlowAsync(id);
        }

        /// <summary>
        /// 编辑固定资产
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("Modify")]
        public async Task<int> ModifyAsync([FromBody]bus_assets entity)
        {
            return await _assetsService.ModifyAsync(entity);
        }

        /// <summary>
        /// 获取固定资产列表
        /// </summary>
        /// <param name="store_id">门店ID(-1=所有)</param>
        /// <param name="state">状态（30=未使用；31=使用中；44=维修中；32=已报废；41=调拨中；47=已报损；-1=所有）</param>
        /// <param name="std_item_id">基础项目ID</param>
        /// <param name="spec">规格</param>
        /// <param name="manufactor_id">厂家ID</param>
        /// <returns></returns>
        [HttpGet("GetList")]
        public async Task<List<bus_assets>> GetListAsync(int store_id, short state, int std_item_id, string spec, int manufactor_id)
        {
            return await _assetsService.GetListAsync(store_id, state, std_item_id, spec, manufactor_id);
        }

        /// <summary>
        /// 获取固资二维码
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetAssetsQrCode")]
        public string GetAssetsQrCode([FromQuery]int id)
        {
            dynamic type = (new Program()).GetType();
            return _assetsService.GetAssetsQrCode(id, type);
        }

        /// <summary>
        /// 返回微信扫一扫所需
        /// </summary>
        /// <returns></returns>
        [HttpGet("ReturnQr")]
        public object ReturnQr([FromQuery]string url)
        {
            
            var type = url;
            return _assetsService.ReturnQr(type);
        }

        /// <summary>
        /// 扫描二维码返回固资信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("ReturnAssets")]
        public async Task<bus_assets> ReturnAssets([FromQuery]int id)
        {
            return await _assetsService.ReturnAssets(id);
        }

    }
}