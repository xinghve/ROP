using Models.DB;
using Models.View.Public;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace Service.Repository.Interfaces.Public
{
    /// <summary>
    /// 机构接口
    /// </summary>
    public interface IOrgService
    {
        /// <summary>
        /// 获取机构分页数据
        /// </summary>
        /// <returns></returns>
        Task<Page<p_org>> GetPageAsync(SearchModel entity);

        /// <summary>
        /// 根据id获取详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<dynamic> GetById(int id);

        /// <summary>
        /// 修改机构状态
        /// </summary>
        /// <param name="id">机构id</param>
        /// <returns></returns>
        Task<bool> ModifyCheck(int id);

        /// <summary>
        /// 机构启用禁用
        /// </summary>
        /// <param name="entity">机构id</param>
        /// <returns></returns>
        Task<bool> ModifyEnable(OrgModel entity);

        /// <summary>
        /// 编辑机构信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> Modify(p_org entity);

        /// <summary>
        /// 就诊卡是否可积分、是否存在实体就诊卡
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> CardState(CardState entity);

        /// <summary>
        /// 获取就诊卡是否可积分、是否存在实体就诊卡
        /// </summary>
        /// <returns></returns>
        Task<dynamic> GetCardState();

        /// <summary>
        /// 获取机构简介
        /// </summary>
        /// <returns></returns>
       Task<OrgIntroduceModel> GetOrgIntroduce(int orgId);

        /// <summary>
        /// 编辑分销政策
        /// </summary>
        /// <returns></returns>
        Task<int> ModifyPolicy(string policy);

        /// <summary>
        /// 编辑提现规则
        /// </summary>
        /// <returns></returns>
        Task<int> ModifyRules(string rules);

        /// <summary>
        /// 获取分销政策
        /// </summary>
        /// <returns></returns>
        Task<PolicyDetailModel> GetPolicy();

        /// <summary>
        /// 获取提现规则
        /// </summary>
        /// <returns></returns>
        Task<PolicyDetailModel> GetRules();

       
    }
}
