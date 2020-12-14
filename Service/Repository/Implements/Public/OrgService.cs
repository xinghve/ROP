using Models.DB;
using Models.View.Public;
using Service.Extensions;
using Service.Repository.Interfaces.Public;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Tools;
using Tools.Filter;
using SqlSugar;
using System.Web;

namespace Service.Repository.Implements.Public
{
    /// <summary>
    /// 机构
    /// </summary>
    public class OrgService : DbContext, IOrgService
    {
        /// <summary>
        /// 后台管理获取机构分页数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Page<p_org>> GetPageAsync(SearchModel entity)
        {
            var orgList = Db.Queryable<p_org>()
                .WhereIF(!string.IsNullOrEmpty(entity.name), w => w.name.Contains(entity.name))
                .WhereIF(entity.startTime != null, w => w.create_time >= entity.startTime)
                .WhereIF(entity.endTime != null, w => w.create_time <= entity.endTime);

            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            orgList = orgList.OrderBy(entity.order + orderTypeStr);
            return await orgList.WithCache().ToPageAsync(entity.page, entity.limit);
        }

        /// <summary>
        /// 根据id获取机构详情
        /// </summary>
        /// <param name="id">机构id</param>
        /// <returns></returns>
        public async Task<dynamic> GetById(int id)
        {
            var isAdmin = false;
            if (id == 0)
            { //获取用户信息
                var userInfo = new Tools.IdentityModels.GetUser().userInfo;
                id = userInfo.org_id;

                //查询是否管理员
                isAdmin = await Db.Queryable<p_employee>().Where(a => a.id == userInfo.id && a.org_id == userInfo.org_id).Select(a => a.is_admin).WithCache().FirstAsync();
            }
            var orgList = await Db.Queryable<p_org>().Where(a => a.id == id).WithCache().FirstAsync();
            return new { isAdmin, orgList };
        }

        /// <summary>
        /// 修改机构状态(审核)
        /// </summary>
        /// <param name="id">机构id</param>
        /// <returns></returns>
        public async Task<bool> ModifyCheck(int id)
        {
            if (id <= 0)
            {
                throw new MessageException("机构id不正确!");
            }
            var t = DateTime.Now.AddDays(int.Parse(ConfigExtensions.Configuration["BaseConfig:ExpireDay"]));
            var isResult = await Db.Ado.UseTranAsync(() => { 
                    Db.Updateable<p_store>().SetColumns(a => new p_store() { expire_time = t }).Where(a => a.org_id == id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                    Db.Updateable<p_employee>().SetColumns(a => new p_employee() { expire_time = t }).Where(a => a.org_id == id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                    var result =  Db.Updateable<p_org>().SetColumns(a => new p_org() { status = 1, expire_time = t }).Where(a => a.id == id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                    if (result <= 0)
                    {
                        throw new MessageException("未审核成功!");
                    }
                    
                });
              
            return isResult.IsSuccess;
        }

        /// <summary>
        /// 启用、禁用
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> ModifyEnable(OrgModel entity)
        {
            var isResult = await Db.Ado.UseTranAsync(() => {
                var result = 0;
                if (entity.state == 0)
                {
                     Db.Updateable<p_store>().SetColumns(a => new p_store() { expire_time = DateTime.Now }).Where(a => a.org_id == entity.orgId).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                     Db.Updateable<p_employee>().SetColumns(a => new p_employee() { expire_time = DateTime.Now }).Where(a => a.org_id == entity.orgId).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                    result =  Db.Updateable<p_org>().SetColumns(a => new p_org() { expire_time = DateTime.Now, status = 0 }).Where(a => a.id == entity.orgId).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                }
                else
                {
                    var expire_time = DateTime.Parse("3000-12-31 23:59:59");
                     Db.Updateable<p_store>().SetColumns(a => new p_store() { expire_time = expire_time }).Where(a => a.org_id == entity.orgId).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                     Db.Updateable<p_employee>().SetColumns(a => new p_employee() { expire_time = expire_time }).Where(a => a.org_id == entity.orgId).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                    result =  Db.Updateable<p_org>().SetColumns(a => new p_org() { expire_time = expire_time, status = 2 }).Where(a => a.id == entity.orgId).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                }
                if (result <= 0)
                {
                    throw new MessageException("操作失败!");
                }

            });
           
            return isResult.IsSuccess;
        }

        /// <summary>
        /// 编辑机构信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> Modify(p_org entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var result = 0;
            if (entity == null)
            {
                throw new MessageException("传值有误!");
            }
            //查询是否为管理员 
            var isAdmin = await Db.Queryable<p_employee>().Where(a => a.org_id == entity.id && a.id == userInfo.id).Select(a => a.is_admin).WithCache().FirstAsync();
            if (isAdmin == false)
            {
                throw new MessageException("非管理员不能操作!");
            }

            var isSuccess = await Db.Ado.UseTranAsync(() =>
            {
                result = Db.Updateable<p_org>().SetColumns(a => new p_org() { link_man = entity.link_man, phone_no = entity.phone_no, office_address = entity.office_address, introduce = entity.introduce
                }).Where(a => a.id == entity.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                if (result <= 0)
                {
                    throw new MessageException("修改失败!");
                }

                var employee_id = 0;
                //查询手机号在这个机构是否存在
                var employee = Db.Queryable<p_employee>().Where(a => a.phone_no == entity.phone_no && a.org_id == entity.id).WithCache().First();
                //修改其他人非管理员
                var count = Db.Updateable<p_employee>().SetColumns(a => new p_employee() { is_admin = false }).Where(a => a.org_id == entity.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                if (count <= 0)
                {
                    throw new MessageException("修改管理员失败!");
                }
                if (employee != null)
                {
                    employee_id = employee.id;
                    var uCount = Db.Updateable<p_employee>().SetColumns(a => new p_employee() { is_admin = true }).Where(a => a.org_id == entity.id && a.phone_no == entity.phone_no).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                    if (uCount <= 0)
                    {
                        throw new MessageException("管理员设置失败!");
                    }
                }
                else
                {
                    var personEntity = new p_employee();
                    personEntity.password = MetarnetRegex.Encrypt(entity.phone_no.Substring(entity.phone_no.Length - 6));
                    personEntity.name = entity.link_man;
                    personEntity.phone_no = entity.phone_no;
                    personEntity.org_id = entity.id;
                    personEntity.pinyin = ToSpell.GetFirstPinyin(entity.link_man);
                    personEntity.create_time = DateTime.Now;
                    personEntity.expire_time = DateTime.Now.AddDays(int.Parse(ConfigExtensions.Configuration["BaseConfig:ExpireDay"]));
                    personEntity.is_admin = true;
                    employee_id = Db.Insertable(personEntity).ExecuteReturnIdentity();
                    redisCache.RemoveAll<p_employee>();
                }

                //删除集团超级管理员
                Db.Deleteable<p_employee_role>().Where(w => w.org_id == userInfo.org_id && w.store_id == 0 && w.is_admin == true).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                #region 人员角色

                var p_employee_role = new p_employee_role()
                {
                    dept_id = 0,
                    employee_id = employee_id,
                    is_admin = true,
                    org_id = entity.id,
                    role_id = 0,
                    store_id = 0
                };

                Db.Insertable(p_employee_role).RemoveDataCache().ExecuteCommand();
                #endregion
            });
            if (!isSuccess.IsSuccess)
            {
                throw new MessageException(isSuccess.ErrorMessage);
            }
            return result;
        }

        /// <summary>
        /// 就诊卡是否可积分、是否存在实体就诊卡
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> CardState(CardState entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            return await Db.Updateable<p_org>().SetColumns(s => new p_org { integral_card = entity.integral_card, physical_card = entity.physical_card }).Where(w => w.id == userInfo.org_id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
        }

        /// <summary>
        /// 获取就诊卡是否可积分、是否存在实体就诊卡
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> GetCardState()
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            return await Db.Queryable<p_org>().Where(w => w.id == userInfo.org_id).Select(s => new { s.integral_card, s.physical_card }).WithCache().FirstAsync();
        }

        /// <summary>
        /// 获取机构简介
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public async Task<OrgIntroduceModel> GetOrgIntroduce(int orgId)
        {
            if (orgId<=0)
            {
                throw new MessageException("未获取到机构！");
            }
            //查询机构信息
            var orgDetail=await Db.Queryable<p_org>()
                           .Where(w => w.id == orgId)
                           .Select(w => new {w.introduce,w.code })
                           .WithCache()
                           .FirstAsync();

            //查询图片List
            var imageList = await Db.Queryable<p_image>()
                                  .Where(w => w.relation_code == orgDetail.code)
                                  .Select(w => w.url)
                                  .WithCache()
                                  .ToListAsync();

            return new OrgIntroduceModel { content=orgDetail.introduce, imgUrlList = imageList };
        }

        /// <summary>
        ///  编辑分销政策
        /// </summary>
        /// <returns></returns>
        public async Task<int> ModifyPolicy(string policy)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            return await Db.Updateable<p_org>()
                           .SetColumns(w=>w.distribution_policy== policy)
                           .Where(w => w.id == userInfo.org_id)
                           .EnableDiffLogEvent()
                           .RemoveDataCache()
                           .ExecuteCommandAsync();
        }

        /// <summary>
        ///  编辑提现规则
        /// </summary>
        /// <returns></returns>
        public async Task<int> ModifyRules(string rules)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            return await Db.Updateable<p_org>()
                           .SetColumns(w=>w.withdrawal_rules== rules)
                           .Where(w => w.id == userInfo.org_id)
                           .EnableDiffLogEvent()
                           .RemoveDataCache()
                           .ExecuteCommandAsync();
        }

        /// <summary>
        /// 获取分销政策
        /// </summary>
        /// <returns></returns>
        public async Task<PolicyDetailModel> GetPolicy()
        {
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            return await Db.Queryable<p_org>()
                           .Where(w => w.id == userInfo.org_id)
                           .Select(w => new PolicyDetailModel { content= w.distribution_policy })
                           .WithCache()
                           .FirstAsync();
        }

        /// <summary>
        /// 获取提现规则
        /// </summary>
        /// <returns></returns>
        public async Task<PolicyDetailModel> GetRules()
        {
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            return await Db.Queryable<p_org>()
                           .Where(w => w.id == userInfo.org_id)
                           .Select(w =>new PolicyDetailModel { content= w.withdrawal_rules })
                           .WithCache()
                           .FirstAsync();


        }
       
    }
}
