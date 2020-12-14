using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Models.View.Public;
using Service.Repository;
using Service.Repository.Interfaces.Public;
using Tools;

namespace ROP.Controllers.Public
{
    /// <summary>
    /// 机构--ty
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class OrgController : ControllerBase
    {
        private readonly IOrgService _orgService;
        private readonly IBaseServer<p_org> _baseServer;

        /// <summary>
        /// 机构构造函数
        /// </summary>
        /// <param name="orgService"></param>
        /// <param name="baseServer"></param>
        public OrgController(IOrgService orgService, IBaseServer<p_org> baseServer)
        {
            _orgService = orgService;
            _baseServer = baseServer;
        }

        /// <summary>
        /// 获取机构分页数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<Page<p_org>> GetPageAsync([FromQuery]SearchModel entity)
        {
            return await _orgService.GetPageAsync(entity);
        }

        /// <summary>
        /// 根据id获取机构详情    
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetById")]
        public async Task<dynamic> GetById([FromQuery]int id = 0)
        {
            return await _orgService.GetById(id);
        }

        /// <summary>
        /// 机构审核
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("ModifyCheck")]
        public async Task<bool> ModifyCheck([FromBody]p_org entity)
        {
            return await _orgService.ModifyCheck(entity.id);
        }

        /// <summary>
        /// 机构启用禁用
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("ModifyEnable")]
        public async Task<bool> ModifyEnable([FromBody]OrgModel entity)
        {
            return await _orgService.ModifyEnable(entity);
        }


        /// <summary>
        /// 编辑机构信息
        /// </summary>
        /// <param name="entity">机构实体</param>
        /// <returns></returns>
        [HttpPut("Modify")]
        public async Task<int> Modify([FromBody]p_org entity)
        {
            return await _orgService.Modify(entity);
        }

        /// <summary>
        /// 就诊卡是否可积分、是否存在实体就诊卡
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("CardState")]
        public async Task<int> CardState([FromBody]CardState entity)
        {
            return await _orgService.CardState(entity);
        }

        /// <summary>
        /// 获取就诊卡是否可积分、是否存在实体就诊卡    
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetCardState")]
        public async Task<dynamic> GetCardState()
        {
            return await _orgService.GetCardState();
        }

        /// <summary>
        /// 获取机构简介
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        [HttpGet("GetOrgIntroduce")]
        public async Task<OrgIntroduceModel> GetOrgIntroduce([FromQuery]int orgId)
        {
            return await _orgService.GetOrgIntroduce(orgId);
        }

        /// <summary>
        /// 编辑分销政策
        /// </summary>
        /// <returns></returns>
        [HttpPut("ModifyPolicy")]
        public async Task<int> ModifyPolicy([FromBody]PolicyModel policy)
        {
            return await _orgService.ModifyPolicy(policy.content);
        }


        /// <summary>
        /// 编辑提现规则
        /// </summary>
        /// <returns></returns>
        [HttpPut("ModifyRules")]
        public async Task<int> ModifyRules([FromBody]PolicyModel rules)
        {
            return await _orgService.ModifyRules(rules.content);
        }

        /// <summary>
        /// 获取分销政策
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetPolicy")]
        public async Task<PolicyDetailModel> GetPolicy()
        {
            return await _orgService.GetPolicy();
        }

        /// <summary>
        /// 获取提现规则
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetRules")]
        public async Task<PolicyDetailModel> GetRules()
        {
            return await _orgService.GetRules();
        }

       
    }
}