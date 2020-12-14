using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Models.View.Crm;
using Models.View.Mobile;
using Models.View.Public;
using Service.Repository.Interfaces.Crm;
using Tools;
using Tools.Authorize;

namespace ROP.Controllers.Cus
{
    /// <summary>
    /// 档案信息
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeForArchives]
    public class ArchivesController : ControllerBase
    {
        private readonly IArchivesService _archivesService;
        private readonly IACardService _aCardService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="archivesService"></param>
        /// <param name="aCardService"></param>
        public ArchivesController(IArchivesService archivesService, IACardService aCardService)
        {
            _archivesService = archivesService;
            _aCardService = aCardService;
        }

        /// <summary>
        /// 登录验证
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="org_id">集团ID</param>
        /// <param name="code">验证码</param>
        /// <returns></returns>
        [AuthorizeForArchives(true)]
        [HttpGet]
        public async Task<dynamic> Login([FromQuery]string username, [FromQuery]string password, [FromQuery]int org_id, [FromQuery]string code)
        {
            return await _archivesService.Login(username, password, org_id, code);
        }

        /// <summary>
        /// 获取档案头像
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetPhoto")]
        public async Task<string> GetPhotoAsync()
        {
            return await _archivesService.GetPhotoAsync();
        }

        /// <summary>
        /// 编辑档案头像
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("ModifyPhoto")]
        public async Task<int> ModifyPhotoByIdAsync([FromBody]ArcPhoto entity)
        {
            return await _archivesService.ModifyPhotoAsync(entity);
        }

        /// <summary>
        /// 忘记密码
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [AuthorizeForArchives(true)]
        [HttpPut("ModifyPassword")]
        public async Task<int> ModifyPassword([FromBody]ArcLoginPassword entity)
        {
            return await _archivesService.ModifyPassword(entity);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("UpdatePassword")]
        public async Task<int> UpdatePassword([FromBody]ArcLoginPassword entity)
        {
            return await _archivesService.UpdatePassword(entity);
        }

        /// <summary>
        /// 获取档案账户
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetArcAccount")]
        public async Task<dynamic> GetArcAccountAsync()
        {
            return await _aCardService.GetArcAccountAsync();
        }

        /// <summary>
        /// 修改档案（客户端）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("ArcModify")]
        public async Task<int> ArcModifyAsync([FromBody]ArcArchives entity)
        {
            return await _archivesService.ArcModifyAsync(entity);
        }

        /// <summary>
        /// 获取充值记录（客户端）
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetRechargeList")]
        public async Task<dynamic> GetRechargeListAsync()
        {
            return await _aCardService.GetRechargeListAsync();
        }

        /// <summary>
        /// 消费记录分页查询（客户端）
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetSpendPages")]
        public async Task<Page<Spend>> GetSpendPagesAsync([FromQuery]Search entity)
        {
            return await _aCardService.GetSpendPagesAsync(entity);
        }

        /// <summary>
        /// 自主注册
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [AuthorizeForArchives(true)]
        [HttpPost("ArcAdd")]
        public async Task<bool> ArcAddAsync([FromBody]RegisterArchives entity)
        {
            return await _archivesService.ArcAddAsync(entity);
        }

        
    }
}