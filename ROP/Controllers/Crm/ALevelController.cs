using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Models.View.Crm;
using Service.Repository.Interfaces.Crm;
using Tools;

namespace ROP.Controllers.Crm
{
    /// <summary>
    /// 会员等级
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ALevelController : ControllerBase
    {
        private readonly IALevelService _aLevelService;
        /// <summary>
        /// 会员等级构造
        /// </summary>
        /// <param name="aLevelService"></param>
        public ALevelController(IALevelService aLevelService)
        {
            _aLevelService = aLevelService;
        }

        /// <summary>
        /// 会员等级分页
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetPageAsync")]
        public async Task<Page<c_archives_level>> GetPageAsync([FromQuery]SearchMl entity)
        {
            return await _aLevelService.GetPageAsync(entity);
        }

        /// <summary>
        /// 会员等级列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetList")]
        public async Task<List<c_archives_level>> GetList([FromQuery]int store_id)
        {
            return await _aLevelService.GetList( store_id);
        }
        /// <summary>
        /// 添加会员等级
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<int> Add([FromBody]c_archives_level entity)
        {
            return await _aLevelService.Add(entity);
        }

        /// <summary>
        /// 编辑会员等级
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("Modify")]
        public async Task<int> Modify([FromBody]c_archives_level entity)
        {
            return await _aLevelService.Modify(entity);
        }


        /// <summary>
        /// 删除会员等级信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpDelete("Delete")]
        public async Task<int> Delete([FromBody]c_archives_level entity)
        {
            return await _aLevelService.Delete(entity.id);
        }
    }
}