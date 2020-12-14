using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Models.View.Public;
using Service.Repository.Interfaces.Public;
using Tools;

namespace ROP.Controllers.Public
{
    /// <summary>
    /// 门店--ty
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly IStoreService _storeServer;

        /// <summary>
        /// 门店
        /// </summary>
        /// <param name="storeServer"></param>
        public StoreController(IStoreService storeServer)
        {
            _storeServer = storeServer;
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        [HttpGet("GetPages")]
        public async Task<Page<p_store>> GetPagesAsync([FromQuery]string name, [FromQuery] string order, [FromQuery] int orderType, [FromQuery] int limit, [FromQuery] int page)
        {
            return await _storeServer.GetPageAsync(name, order, orderType, limit, page);
        }

        /// <summary>
        /// 添加门店
        /// </summary>
        /// <param name="entity">门店实体</param>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<bool> Add([FromBody]p_store entity)
        {
            return await _storeServer.AddAsync(entity);
        }

        /// <summary>
        /// 编辑门店
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("Modify")]
        public async Task<bool> Modify([FromBody]p_store entity)
        {
            return await _storeServer.ModifyAsync(entity);
        }


        /// <summary>
        /// 修改状态（状态（0=停用，1=启用））
        /// </summary>
        /// <returns></returns>
        [HttpPut("ModifyDisabled")]
        public async Task<int> ModifyDisabled([FromBody]p_store entity)
        {
            return await _storeServer.ModifyUseStatusdAsync(entity);
        }

        /// <summary>
        /// 删除门店
        /// </summary>
        /// <param name="list">门店ID</param>
        /// <returns></returns>
        [HttpDelete("Delete")]
        public async Task<int> Delete([FromBody]List<int> list)
        {
            return await _storeServer.DeleteAsync(list);
        }

        /// <summary>
        /// 获取门店信息
        /// </summary>
        /// <param name="storeID">门店ID</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<p_store> GetAsync([FromQuery] int storeID)
        {
            return await _storeServer.GetAsync(storeID);
        }

        /// <summary>
        /// 获取门店列表
        /// </summary>
        /// <param name="containOrg">是否包含集团</param>
        /// <returns></returns>
        [HttpGet("GetList")]
        public async Task<List<Store>> GetListAsync([FromQuery]bool containOrg = true)
        {
            return await _storeServer.GetListAsync(containOrg);
        }

        /// <summary>
        /// 获取客户端门店列表
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        [HttpGet("GetStoreList")]
        public async Task<List<Store>> GetStoreList([FromQuery]int orgId)
        {
            return await _storeServer.GetStoreList(orgId);
        }

        /// <summary>
        /// 获取门店简介
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        [HttpGet("GetStoreIntroduce")]
        public async Task<StoreIntroduceModel> GetStoreIntroduce([FromQuery]int storeId)
        {
            return await _storeServer.GetStoreIntroduce(storeId);
        }
    }
}