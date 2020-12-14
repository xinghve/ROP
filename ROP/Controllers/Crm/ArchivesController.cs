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
    /// 档案信息
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ArchivesController : ControllerBase
    {
        private readonly IArchivesService _archivesService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="archivesService"></param>
        public ArchivesController(IArchivesService archivesService)
        {
            _archivesService = archivesService;
        }

        /// <summary>
        /// 添加档案信息
        /// </summary>
        /// <param name="entity">档案信息信息</param>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<bool> Add([FromBody]Archives entity)
        {
            return await _archivesService.AddAsync(entity);
        }

        /// <summary>
        /// 修改档案信息
        /// </summary>
        /// <param name="entity">档案信息信息</param>
        /// <returns></returns>
        [HttpPut("Modify")]
        public async Task<bool> Modify([FromBody]Archives entity)
        {
            return await _archivesService.ModifyAsync(entity);
        }

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <returns></returns>
        [HttpPut("ModifyDisabled")]
        public async Task<int> ModifyDisabled([FromBody]c_archives entity)
        {
            return await _archivesService.ModifyDisabledAsync(entity);
        }

        /// <summary>
        /// 根据档案ID获取信息
        /// </summary>
        /// <param name="archives_id">档案ID</param>
        /// <returns></returns>
        [HttpGet("GetArchives")]
        public async Task<dynamic> GetArchivesAsync([FromQuery] int archives_id)
        {
            return await _archivesService.GetArchivesAsync(archives_id);
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="storeID">门店ID</param>
        /// <param name="isAll">是否所有</param>
        /// <param name="name">姓名/手机号</param>
        /// <returns></returns>
        [HttpGet("GetList")]
        public async Task<dynamic> GetListAsync([FromQuery] int storeID, [FromQuery] bool isAll, [FromQuery] string name)
        {
            return await _archivesService.GetListAsync(storeID, isAll, name);
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("GetPages")]
        public async Task<Page<ArchivesExtend>> GetPagesAsync([FromQuery] ArchivesSearch entity)
        {
            return await _archivesService.GetPagesAsync(entity);
        }

        /// <summary>
        /// 获取券码是否输入正确
        /// </summary>
        /// <param name="coupon_no"></param>
        /// <returns></returns>
        [HttpGet("GetCoupon")]
        public async Task<object> GetCoupon([FromQuery]string coupon_no)
        {
            return await _archivesService.GetCoupon(coupon_no);
        }

        /// <summary>
        /// 档案导入
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("ArchivesImport")]
        public async Task<bool> ArchivesImportAsync([FromBody]ArchivesImport entity)
        {
            return await _archivesService.ArchivesImportAsync(entity);
        }

        /// <summary>
        /// 设置会员等级
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("SetLevel")]
        public async Task<int> SetLevelAsync([FromBody]c_archives entity)
        {
            return await _archivesService.SetLevelAsync(entity);
        }
    }
}