using Models.DB;
using Models.View.Mobile;
using Models.View.Public;
using QRCoder;
using Service.Extensions;
using Service.Repository.Interfaces.Public;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Tools;
using Tools.Filter;

namespace Service.Repository.Implements.Public
{
    /// <summary>
    /// 人员管理
    /// </summary>
    public class PersonService : DbContext, IPersonService
    {
        /// <summary>
        /// 人员分页数据
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="status">状态（1=已启用 2=已停用 0=所有）</param>
        /// <returns></returns>
        public async Task<Page<PersonDetials>> GetPageAsync(SearchModel entity, int status)
        {
            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            //查询人员信息
            var list = await Db.Queryable<p_employee_role, p_employee, p_employee_profile, p_employee_nature>((er, p, pr, en) => new object[] { JoinType.Left, er.employee_id == p.id, JoinType.Left, p.id == pr.id, JoinType.Left, p.id == en.employee_id })
               .Where((er, p, pr, en) => p.org_id == userInfo.org_id)
               .WhereIF(entity.storeId > 0, (er, p, pr, en) => er.store_id == entity.storeId)
               .WhereIF(!string.IsNullOrEmpty(entity.name), (er, p, pr, en) => p.name.Contains(entity.name))
               .WhereIF(entity.startTime != null, (er, p, pr, en) => p.create_time >= entity.startTime)
               .WhereIF(entity.endTime != null, (er, p, pr, en) => p.create_time <= entity.endTime)
               .WhereIF(entity.roleId > 0, (er, p, pr, en) => er.role_id == entity.roleId)
               .WhereIF(entity.deptId > 0, (er, p, pr, en) => er.dept_id == entity.deptId)
               .WhereIF(status == 1, (er, p, pr, en) => p.expire_time > DateTime.Now)
               .WhereIF(!string.IsNullOrEmpty(entity.proDutiesId), (er, p, pr, en) => p.pro_duties_id.Contains(entity.proDutiesId))
               .WhereIF(entity.natureId > 0, (er, p, pr, en) => en.nature_id == entity.natureId)
               .WhereIF(status == 2, (er, p, pr, en) => p.expire_time < DateTime.Now)
               .GroupBy((er, p, pr, en) => new { p.id, pr.birthday, p.create_time, pr.description, p.name, p.phone_no, p.expire_time, p.id_no, p.image_url, pr.sex_name }).Select((er, p, pr, en) => new PersonDetials { id = p.id, birthday = pr.birthday, create_time = p.create_time, description = pr.description, name = p.name, phone_no = p.phone_no, expire_time = p.expire_time, id_no = p.id_no, image_url = p.image_url, sex_name = pr.sex_name, good_at = p.good_at }).OrderBy(entity.order + orderTypeStr).WithCache().ToPageAsync(entity.page, entity.limit);
            //查询人员门店信息
            var orgName = await Db.Queryable<p_org>().Where(a => a.id == userInfo.org_id).Select(a => new { a.name }).WithCache().FirstAsync();
            if (orgName == null)
            {
                orgName = new { name = "管理平台" };
            }
            var stroeList = await Db.Queryable<p_employee_role, p_store>((er, st) => new object[] { SqlSugar.JoinType.Left, er.store_id == st.id }).Where((er, st) => er.org_id == userInfo.org_id).GroupBy((er, st) => new { er.employee_id, er.store_id, st.name }).Select((er, st) => new { er.employee_id, id = er.store_id, name = SqlFunc.IIF(string.IsNullOrEmpty(st.name), orgName.name, st.name) }).WithCache().ToListAsync();
            //查询部门角色
            var drList = await Db.Queryable<p_employee_role, p_dept, p_role>((er, d, r) => new object[] { JoinType.Left, er.dept_id == d.id, JoinType.Left, er.role_id == r.id }).Where((er, d, r) => er.org_id == userInfo.org_id).WhereIF(entity.storeId > 0, (er, d, r) => er.store_id == entity.storeId).Select((er, d, r) => new { er.employee_id, er.dept_id, er.role_id, dept_name = d.name, role_name = r.name }).WithCache().ToListAsync();
            //查询人员性质
            var nature = await Db.Queryable<p_employee_nature>().Where(w => w.org_id == userInfo.org_id).WithCache().ToListAsync();
            //查询集团is_admin=true的人员
            var adminEmployeeIds = await Db.Queryable<p_employee>().Where(w => w.is_admin == true && w.org_id == userInfo.org_id).Select(s => s.id).WithCache().ToListAsync();
            //查询门店is_admin=true的人员
            var adminEmployeeIdDepts = await Db.Queryable<p_employee_role>().Where(w => w.is_admin == true && w.org_id == userInfo.org_id).Select(s => s.employee_id).WithCache().ToListAsync();
            //将部门角色添加到人员信息
            var newList = list.Items.Select(s => new PersonDetials { good_at = s.good_at, id = s.id, birthday = s.birthday, create_time = s.create_time, description = s.description, name = s.name, phone_no = s.phone_no, is_admin = adminEmployeeIds.Contains(s.id), expire_time = s.expire_time, status = (s.expire_time >= DateTime.Now ? true : false), id_no = s.id_no, image_url = s.image_url, sex_name = s.sex_name, deptRoleModes = drList.Where(w => w.employee_id == s.id).Select(se => new DeptRoleMode { id = se.dept_id, name = se.dept_name, roleId = se.role_id, roleName = se.role_name }).ToList(), stores = stroeList.Where(w => w.employee_id == s.id).Select(ss => new Store { id = ss.id, name = ss.name, is_admin = adminEmployeeIdDepts.Contains(ss.employee_id) }).ToList(), employeeNatures = nature.Where(w => w.employee_id == s.id).ToList() }).ToList();
            list.Items = newList;
            return list;
        }

        /// <summary>
        /// 根据id获取人员信息
        /// </summary>
        /// <param name="roleEntity">门店id</param>
        /// <returns></returns>
        public async Task<dynamic> GetPersonById(p_employee_role roleEntity)
        {
            if (roleEntity.employee_id <= 0)//|| roleEntity.store_id <= 0
            {
                throw new MessageException("未获取到人员信息！");
            }

            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            //人员信息
            var person = await Db.Queryable<p_employee, p_employee_profile>((e, ep) => new object[] { JoinType.Left, e.id == ep.id })
                .Where((e, ep) => e.id == roleEntity.employee_id)
                .Select((e, ep) => new { p_employee = e, p_detail = ep })
                .WithCache()
                .FirstAsync();

            //部门角色信息
            var ListDept = await Db.Queryable<p_employee_role, p_role, p_dept>((ed, role, e) => new object[] { JoinType.Left, ed.role_id == role.id, JoinType.Left, ed.dept_id == e.id })
                .Where((ed, role, e) => ed.employee_id == roleEntity.employee_id && ed.store_id == roleEntity.store_id && ed.org_id == userInfo.org_id)
                .WhereIF(roleEntity.dept_id > 0, (ed, role, e) => ed.dept_id == roleEntity.dept_id)
                .WhereIF(roleEntity.role_id > 0, (ed, role, e) => ed.role_id == roleEntity.role_id)
                .GroupBy((ed, role, e) => new { ed.employee_id, e.name, e.id, role_id = role.id, role_name = role.name })
                .Select((ed, role, e) => new { ed.employee_id, e.name, e.id, role_id = role.id, role_name = role.name })
                .WithCache()
                .ToListAsync();

            //性质
            var nature = await Db.Queryable<p_employee_nature>().Where(w => w.org_id == userInfo.org_id && w.employee_id == roleEntity.employee_id).WithCache().ToListAsync();

            var list = ListDept.Where(w => w.employee_id == person.p_employee.id).ToList();

            //部门跟人员信息
            var deptList = list.Select(s => new DeptRoleMode { id = s.id, name = s.name, roleId = s.role_id, roleName = s.role_name }).ToList();
            //person["deptList"] = deptList;
            //person.deptList = list.Select(s => new DeptRoleMode { id = s.id, name = s.name, roleId = s.role_id, roleName = s.role_name }).ToList();
            return new { person.p_employee, person.p_detail, deptList, nature };
        }

        /// <summary>
        /// 添加人员信息、详情、部门角色、性质
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> Add(PersonModel entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var exist = await Db.Queryable<p_employee>().Where(w => w.phone_no == entity.p_employee.phone_no && w.org_id == userInfo.org_id).WithCache().AnyAsync();
            if (exist)
            {
                throw new MessageException("此人员手机号已存在，请确认信息是否正确！");
            }

            var result = Db.Ado.UseTran(() =>
            {
                #region 人员信息
                var person = new p_employee();
                person = entity.p_employee;
                person.org_id = userInfo.org_id;
                person.pinyin = ToSpell.GetFirstPinyin(entity.p_employee.name);
                person.create_time = DateTime.Now;
                person.password = MetarnetRegex.Encrypt(entity.p_employee.phone_no.Substring(entity.p_employee.phone_no.Length - 6));
                if (userInfo.org_id == 0)
                {
                    person.expire_time = DateTime.Parse("3000-12-31 23:59:59");
                }
                else
                {
                    person.expire_time = DateTime.Now.AddDays(int.Parse(ConfigExtensions.Configuration["BaseConfig:ExpireDay"]));
                }
                //添加人员信息返回人员Id
                var personId = Db.Insertable(person).ExecuteReturnIdentity();
                redisCache.RemoveAll<p_employee>();
                if (entity.p_detail != null)
                {
                    //添加人员详情信息
                    entity.p_detail.id = personId;
                    var personDetail = Db.Insertable(entity.p_detail).RemoveDataCache().ExecuteCommand();
                }
                if (entity.pdList.Count > 0)
                {
                    //添加部门角色
                    var list = entity.pdList.Select(s => new p_employee_role { org_id = userInfo.org_id, store_id = entity.storeId, employee_id = personId, dept_id = s.dept_id, role_id = s.role_id, is_admin = false }).ToList();
                    Db.Insertable(list).ExecuteCommand();
                    redisCache.RemoveAll<p_employee_role>();
                }
                if (entity.employeeNatures.Count > 0)
                {
                    //添加性质
                    var list = entity.employeeNatures.Select(s => new p_employee_nature { org_id = userInfo.org_id, employee_id = personId, nature_id = s.nature_id, nature = s.nature }).ToList();
                    Db.Insertable(list).ExecuteCommand();
                    redisCache.RemoveAll<p_employee_nature>();
                }


                #endregion
            });
            return result.IsSuccess;
        }

        /// <summary>
        /// 编辑人员信息、详情、部门角色
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> Modify(PersonModel entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var exist = await Db.Queryable<p_employee>().Where(w => w.phone_no == entity.p_employee.phone_no && w.id != entity.p_employee.id && w.org_id == userInfo.org_id).WithCache().AnyAsync();
            if (exist)
            {
                throw new MessageException("此人员已存在，请确认信息是否正确！");
            }

            var result = Db.Ado.UseTran(() =>
            {
                //if (entity.pdList.Count > 0)
                //{
                //    entity.pdList.ForEach(c =>
                //    {
                //        //如果此人在门店已存在选择角色则不可增加此角色
                //        var isExist = Db.Queryable<p_employee_role, p_role>((er, r) => new object[] { er.role_id == r.link_id })
                //                            .Where((er, r) => er.org_id == userInfo.org_id && er.store_id > 0 && r.link_id == c.role_id)
                //                            .Select((er, r) => r.name)
                //                            .WithCache()
                //                            .First();

                //        if (isExist != null)
                //        {

                //            throw new MessageException($"此用户在门店已有“{isExist}”角色！");
                //        }


                //    });
                //}

                #region 人员信息
                var person = new p_employee();
                person = entity.p_employee;
                person.pinyin = ToSpell.GetFirstPinyin(entity.p_employee.name);
                if (entity.is_change_password)
                {
                    person.password = MetarnetRegex.Encrypt(entity.p_employee.phone_no.Substring(entity.p_employee.phone_no.Length - 6));
                    //编辑人员信息
                    Db.Updateable(person)
                    .IgnoreColumns(it => new { it.expire_time, it.org_id, it.create_time, it.is_admin })
                    .Where(a => a.id == entity.p_employee.id)
                    .RemoveDataCache()
                    .EnableDiffLogEvent()
                    .ExecuteCommand();
                }
                else
                {
                    //编辑人员信息
                    Db.Updateable(person)
                    .IgnoreColumns(it => new { it.password, it.expire_time, it.org_id, it.create_time, it.is_admin })
                    .Where(a => a.id == entity.p_employee.id)
                    .RemoveDataCache()
                    .EnableDiffLogEvent()
                    .ExecuteCommand();
                }
                //person.create_time = DateTime.Now;

                if (entity.p_detail != null)
                {
                    entity.p_detail.id = entity.p_employee.id;
                    //编辑人员详情信息
                    var personDetail = Db.Saveable(entity.p_detail).ExecuteCommand();
                }
                if (entity.pdList.Count > 0)
                {
                    //if (entity.storeId <= 0)
                    //{
                    //    throw new MessageException("未获取到门店！");
                    //}
                    //删除人员部门角色信息再添加
                    Db.Deleteable<p_employee_role>().Where(a => a.employee_id == entity.p_employee.id && a.store_id == entity.storeId && a.org_id == userInfo.org_id && a.is_admin == false).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                    //添加部门角色
                    var list = entity.pdList.Select(s => new p_employee_role { org_id = userInfo.org_id, store_id = entity.storeId, employee_id = entity.p_employee.id, dept_id = s.dept_id, role_id = s.role_id }).ToList();
                    Db.Insertable(list).ExecuteCommand();
                    redisCache.RemoveAll<p_employee_role>();
                }
                if (entity.employeeNatures.Count > 0)
                {
                    //删除性质
                    Db.Deleteable<p_employee_nature>().Where(a => a.employee_id == entity.p_employee.id && a.org_id == userInfo.org_id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                    //添加性质
                    var list = entity.employeeNatures.Select(s => new p_employee_nature { org_id = userInfo.org_id, employee_id = entity.p_employee.id, nature_id = s.nature_id, nature = s.nature }).ToList();
                    Db.Insertable(list).ExecuteCommand();
                    redisCache.RemoveAll<p_employee_nature>();
                }


                #endregion
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 批量删除人员信息
        /// </summary>
        /// <param name="delList"></param>
        /// <returns></returns>
        public async Task<int> DeleteAsync(List<int> delList)
        {
            var expire_time = DateTime.Now;
            return await Db.Updateable<p_employee>().SetColumns(a => new p_employee() { expire_time = expire_time }).Where(a => delList.Contains(a.id)).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
        }

        /// <summary>
        /// 批量启用人员
        /// </summary>
        /// <param name="useList"></param>
        /// <returns></returns>
        public async Task<int> UseAsync(List<int> useList)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var expire_time = DateTime.Parse("3000-12-31 23:59:59");
            if (userInfo.org_id > 0)
            {
                var org = await Db.Queryable<p_org>().Where(w => w.id == userInfo.org_id).WithCache().FirstAsync();
                expire_time = org.expire_time;
            }
            return await Db.Updateable<p_employee>().SetColumns(a => new p_employee() { expire_time = expire_time }).Where(a => useList.Contains(a.id)).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
        }

        /// <summary>
        /// 获取人员门店列表
        /// </summary>
        /// <param name="id">人员ID</param>
        /// <returns></returns>
        public async Task<List<Store>> GetListAsync(int id)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            var orgName = await Db.Queryable<p_org>().Where(a => a.id == userInfo.org_id).Select(a => new { a.name }).WithCache().FirstAsync();
            if (orgName == null)
            {
                orgName = new { name = "管理平台" };
            }
            return await Db.Queryable<p_employee_role, p_store>((e, s) => new object[] { JoinType.Left, e.store_id == s.id })
                .Where((e, s) => e.employee_id == id)
                .GroupBy((e, s) => new { s.id, s.name })
                .Select((e, s) => new Store { id = s.id, name = SqlFunc.IIF(string.IsNullOrEmpty(s.name), orgName.name, s.name) })
                .WithCache()
                .ToListAsync();
        }

        /// <summary>
        /// 人员调度（机构向门店）
        /// </summary>
        /// <param name="employeeDispatch">人员调度信息</param>
        /// <returns></returns>
        public async Task<bool> DispatchAsync(EmployeeDispatch employeeDispatch)
        {
            if (employeeDispatch.toStoreID <= 0 || employeeDispatch.employeeID <= 0)
            {
                throw new MessageException("未获取到信息！");

            }
            if (employeeDispatch.fromStoreID == employeeDispatch.toStoreID)
            {
                throw new MessageException("已存在此门店中！");
            }
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            //var result = Db.Ado.UseTran(() =>
            //{
            //    Db.Deleteable<p_employee_role>().Where(w => w.store_id == employeeDispatch.fromStoreID && w.employee_id == employeeDispatch.employeeID).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
            //    var list = employeeDispatch.deptRoles
            //    .Select(s => new p_employee_role
            //    {
            //        dept_id = s.dept_id,
            //        employee_id = employeeDispatch.employeeID,
            //        org_id = userInfo.org_id,
            //        role_id = s.role_id,
            //        store_id = employeeDispatch.toStoreID
            //    })
            //    .ToList();
            //    Db.Insertable(list).ExecuteCommand();
            //    redisCache.RemoveAll<p_employee_role>();
            //});
            var result = Db.Ado.UseTran(() =>
            {
                var nowRole = new List<p_employee_role>();
                if (employeeDispatch.fromStoreID > 0)
                {
                    //查询调出门店的部门角色
                    nowRole = Db.Queryable<p_employee_role>()
                                  .Where(p => p.employee_id == employeeDispatch.employeeID && p.org_id == userInfo.org_id && p.store_id == employeeDispatch.fromStoreID)
                                  .WithCache()
                                  .ToList();

                    var roleIds = nowRole.Select(s => s.role_id).ToList();

                    //查询总部角色
                    var roles = Db.Queryable<p_role>().Where(w => roleIds.Contains(w.id)).Select(s => s.link_id).WithCache().ToList();

                    //调入门店拥有的角色
                    var toRole = Db.Queryable<p_role>()
                                 .Where(tp => tp.org_id == userInfo.org_id && tp.store_id == employeeDispatch.toStoreID && roles.Contains(tp.link_id.Value))
                                 .WithCache()
                                 .ToList();

                    if (toRole.Count == 0)
                    {
                        throw new MessageException("调入门店没有此职位！");
                    }

                    if (toRole.Count > 0)
                    {
                        ////调入门店角色ids
                        //var toroleIds = toRole.Select(ss => ss.link_id).ToList();
                        //删除原机构角色
                        Db.Deleteable<p_employee_role>()
                          .Where(pr => pr.org_id == userInfo.org_id && roleIds.Contains(pr.role_id) && pr.store_id == employeeDispatch.fromStoreID && pr.employee_id == employeeDispatch.employeeID && pr.is_admin == false)
                          .RemoveDataCache()
                          .EnableDiffLogEvent()
                          .ExecuteCommand();

                        //门店新增
                        var nowERole = toRole.Select(tr => new p_employee_role { dept_id = tr.dept_id, store_id = employeeDispatch.toStoreID, employee_id = employeeDispatch.employeeID, is_admin = false, org_id = userInfo.org_id, role_id = tr.id }).ToList();
                        Db.Insertable(nowERole).ExecuteCommand();
                        redisCache.RemoveAll<p_employee_role>();
                    }

                }
                else
                {
                    //查询当前用户机构所属角色
                    nowRole = Db.Queryable<p_employee_role>()
                                       .Where(p => p.employee_id == employeeDispatch.employeeID && p.org_id == userInfo.org_id && p.store_id == 0)
                                       .WithCache()
                                       .ToList();

                    var roleIds = nowRole.Select(s => s.role_id).ToList();

                    //调入门店拥有的角色
                    var toRole = Db.Queryable<p_role>()
                                 .Where(tp => tp.org_id == userInfo.org_id && tp.store_id == employeeDispatch.toStoreID && roleIds.Contains(tp.link_id.Value))
                                 .WithCache()
                                 .ToList();

                    if (toRole.Count == 0)
                    {
                        throw new MessageException("调入门店没有此职位！");
                    }

                    if (toRole.Count > 0)
                    {
                        ////调入门店角色ids
                        //var toroleIds = toRole.Select(ss => ss.link_id).ToList();
                        //删除原机构角色
                        Db.Deleteable<p_employee_role>()
                          .Where(pr => pr.org_id == userInfo.org_id && roleIds.Contains(pr.role_id) && pr.store_id == 0 && pr.employee_id == employeeDispatch.employeeID && pr.is_admin == false)
                          .RemoveDataCache()
                          .EnableDiffLogEvent()
                          .ExecuteCommand();

                        //门店新增
                        var nowERole = toRole.Select(tr => new p_employee_role { dept_id = tr.dept_id, store_id = employeeDispatch.toStoreID, employee_id = employeeDispatch.employeeID, is_admin = false, org_id = userInfo.org_id, role_id = tr.id }).ToList();
                        Db.Insertable(nowERole).ExecuteCommand();
                        redisCache.RemoveAll<p_employee_role>();
                    }



                }



            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 获取人员列表
        /// </summary>
        /// <param name="name">姓名/拼音/手机号/身份证</param>
        /// <param name="nature_id">性质ID</param>
        /// <param name="store_id">门店ID</param>
        /// <param name="state">状态（-1=所有，0=已失效，1=正常）</param>
        /// <param name="dept_id">部门ID</param>
        /// <returns></returns>
        public async Task<List<p_employee>> GetEmployeeListAsync(string name, int nature_id = -1, int store_id = -1, int state = -1, int dept_id = -1)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            return await Db.Queryable<p_employee, p_employee_role, p_employee_nature>((e, er, en) => new object[] { JoinType.Left, e.id == er.employee_id, JoinType.Left, e.id == en.employee_id })
                .Where((e, er, en) => e.org_id == userInfo.org_id)
                .WhereIF(!string.IsNullOrEmpty(name), (e, er, en) => e.name.Contains(name) || e.pinyin.Contains(name) || e.phone_no.Contains(name) || e.id_no.Contains(name))
                .WhereIF(store_id != -1, (e, er, en) => er.store_id == store_id)
                .WhereIF(nature_id != -1, (e, er, en) => en.nature_id == nature_id)
                .WhereIF(dept_id != -1, (e, er, en) => er.dept_id == dept_id)
                .WhereIF(state == 0, (e, er, en) => e.expire_time <= DateTime.Now)
                .WhereIF(state == 1, (e, er, en) => e.expire_time >= DateTime.Now)
                .GroupBy((e, er, en) => new { e.create_time, e.expire_time, e.id, e.id_no, e.image_url, e.is_admin, e.name, e.org_id, e.password, e.phone_no, e.pinyin })
                .Select((e, er, en) => e)
                .WithCache()
                .ToListAsync();
        }

        /// <summary>
        /// 获取人员二维码
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="dept_type_code">部门分类编码</param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<dynamic> GetQrCodeAsync(int store_id, string dept_type_code, dynamic type)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var employeeList = await Db.Queryable<p_employee, p_employee_role, p_dept, b_basecode>((e, er, d, b) => new object[] { JoinType.Left, e.id == er.employee_id, JoinType.Left, er.dept_id == d.id, JoinType.Left, d.dept_type_code == b.valuecode })
                .Where((e, er, d, b) => e.org_id == userInfo.org_id)
                .WhereIF(store_id != -1, (e, er, d, b) => er.store_id == store_id)
                .WhereIF(!string.IsNullOrEmpty(dept_type_code), (e, er, d, b) => b.valuecode == dept_type_code)
                .GroupBy((e, er, d, b) => new { e.id, e.org_id, er.store_id, e.name, e.phone_no })
                .Select((e, er, d, b) => new { e.id, e.org_id, er.store_id, e.name, e.phone_no })
                .WithCache()
                .ToListAsync();
            var list = new List<object>();
            foreach (var item in employeeList)
            {
                var url = $"https://www.baidu.com?id={item.id}&orgId={item.org_id}&storeId={item.store_id}";
                var pic = Tools.Utils.GetQrCode(url, type);
                list.Add(new { item.name, item.phone_no, code = pic });
            }
            return list;
        }

        /// <summary>
        /// 根据Id获取人员头像
        /// </summary>
        /// <param name="id">人员ID</param>
        /// <returns></returns>
        public async Task<string> GetPhotoByIdAsync(int id)
        {
            var employee = await Db.Queryable<p_employee, p_employee_profile>((e, ep) => new object[] { JoinType.Left, e.id == ep.id }).Where((e, ep) => e.id == id).Select((e, ep) => new { e.image_url, ep.sex_name }).FirstAsync();
            var url = employee.image_url;
            return Utils.GetImage_url(url, employee.sex_name); ;
        }

        /// <summary>
        /// 编辑人员信息（移动端）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> Modify(Employee entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            var result = Db.Ado.UseTran(() =>
            {
                #region 人员信息

                Db.Updateable<p_employee>().SetColumns(s => new p_employee { name = entity.name, id_no = entity.id_no }).Where(w => w.id == entity.employee_id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                Db.Updateable<p_employee_profile>().SetColumns(s => new p_employee_profile { address = entity.address }).Where(w => w.id == entity.employee_id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                #endregion
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 根据Id修改人员头像
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> ModifyPhotoByIdAsync(Photo entity)
        {
            return await Db.Updateable<p_employee>().SetColumns(s => new p_employee { image_url = entity.url }).Where(w => w.id == entity.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
        }

        #region 获取分销二维码

        /// <summary>
        /// 获取分销二维码
        /// </summary>
        /// <param name="store_id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetDistributionQrCode(int store_id, dynamic type)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var code_url = ConfigExtensions.Configuration["BaseConfig:QrCodeUrl"];
            var url = $"http://{code_url}/#/register?director_id={userInfo.id}&orgId={userInfo.org_id}&storeId={store_id}";
            return Tools.Utils.GetQrCode(url, type);
        }

        #endregion

        /// <summary>
        /// 根据科室获取医生
        /// </summary>
        /// <param name="deptId"></param>
        /// <param name="storeId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<List<DoctorModel>> GetDoctorList(int deptId, int storeId, string name = "")
        {
            var list = await Db.Queryable<p_employee, p_employee_role, p_employee_nature, p_dept_nature, p_dept, p_employee_profile>((e, er, en, dn, d, ep) => new object[] { JoinType.Left, er.employee_id == e.id, JoinType.Left, en.employee_id == e.id, JoinType.Left, er.dept_id == dn.dept_id, JoinType.Left, er.dept_id == d.id, JoinType.Left, ep.id == e.id })
                .Where((e, er, en, dn, d, ep) => e.expire_time > DateTime.Now && er.store_id == storeId && en.nature_id == 1 && dn.nature_id == 3)
                .WhereIF(deptId > 0, (e, er, en, dn, d, ep) => dn.dept_id == deptId)
                .WhereIF(!string.IsNullOrEmpty(name), (e, er, en, dn, d, ep) => e.name.Contains(name))
                .Select((e, er, en, dn, d, ep) => new DoctorModel { doctorId = e.id, doctorName = e.name, pro_duties = e.pro_duties, dept_name = d.name, dept_id = d.id, good_at = e.good_at, image_url = e.image_url, sex = ep.sex_name, description = ep.description })
                .WithCache()
                .ToListAsync();

            list = list.Select(s => new DoctorModel { dept_id = s.dept_id, dept_name = s.dept_name, doctorId = s.doctorId, doctorName = s.doctorName, good_at = s.good_at, image_url = Utils.GetImage_url(s.image_url, s.sex), pro_duties = s.pro_duties, sex = s.sex, description = s.description }).ToList();

            return list;
        }

        /// <summary>
        /// 获取医生简介
        /// </summary>
        /// <param name="doctorId"></param>
        /// <param name="deptId"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public async Task<DoctorDetailModel> GetDoctorIntroduce(int doctorId, int deptId, int storeId)
        {
            if (doctorId <= 0)
            {
                throw new MessageException("未获取到医生！");
            }
            //获取用户信息
            var user = new Tools.IdentityModels.GetArchives().archives;

            //查询是否收藏
            var isCollection = await Db.Queryable<c_collection>()
                                   .Where(w => w.doctor_id == doctorId && w.dept_id == deptId && w.archives_id == user.id && w.org_id == user.org_id && w.store_id == storeId)
                                   .CountAsync();

            return await Db.Queryable<p_employee, p_employee_profile>((e, ep) => new object[] { JoinType.Left, ep.id == e.id })
                           .Where((e, ep) => e.id == doctorId)
                           .Select((e, ep) => new DoctorDetailModel { is_collection = isCollection, description = ep.description, doctorName = e.name, good_at = e.good_at, pro_duties = e.pro_duties, imgUrl = e.image_url })
                           .WithCache()
                           .FirstAsync();
        }

        /// <summary>
        /// 收藏医生
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> CollectionDoctor(c_collection entity)
        {
            //获取用户信息
            var user = new Tools.IdentityModels.GetArchives().archives;
            if (entity == null)
            {
                throw new MessageException("未获取到信息！");
            }
            entity.org_id = user.org_id;
            entity.archives_id = user.id;

            var isSuccess = await Db.Insertable(entity).ExecuteCommandAsync();
            redisCache.RemoveAll<c_collection>();
            return isSuccess;
        }

        /// <summary>
        /// 删除收藏
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> DeleteCollection(c_collection entity)
        {
            //获取用户信息
            var user = new Tools.IdentityModels.GetArchives().archives;

            if (entity.archives_id <= 0 || entity.dept_id <= 0 || entity.doctor_id <= 0 || entity.store_id <= 0)
            {
                throw new MessageException("未获取到信息！");
            }

            var isSuccess = await Db.Deleteable<c_collection>()
                .Where(w => w.org_id == user.org_id && w.store_id == entity.store_id && w.archives_id == user.id && w.dept_id == entity.dept_id && w.doctor_id == entity.doctor_id)
                .RemoveDataCache()
                .EnableDiffLogEvent()
                .ExecuteCommandAsync();
            return isSuccess;
        }

        /// <summary>
        /// 获取收藏医生
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> GetCollectionListAsync()
        {
            // 获取用户信息
            var user = new Tools.IdentityModels.GetArchives().archives;
            return await Db.Queryable<c_collection, p_employee, p_dept, p_employee_profile, p_store>((c, e, d, ep, s) => new object[] { JoinType.Left, c.doctor_id == e.id, JoinType.Left, c.dept_id == d.id, JoinType.Left, ep.id == e.id, JoinType.Left, c.store_id == s.id })
                .Where((c, e, d, ep, s) => c.archives_id == user.id)
                .Select((c, e, d, ep, s) => new DoctorModel { doctorId = e.id, doctorName = e.name, pro_duties = e.pro_duties, dept_name = d.name, dept_id = d.id, good_at = e.good_at, image_url = e.image_url, sex = ep.sex_name, description = ep.description, store_id = c.store_id, store_name = s.name })
                .WithCache()
                .ToListAsync();
        }

        /// <summary>
        /// 流程选定人员
        /// </summary>
        /// <param name="dept_id">部门id</param>
        /// <param name="role_id">角色id</param>
        /// <param name="searchVal">姓名或电话</param>
        /// <returns></returns>
        public async Task<List<PersonProcessModel>> GetProcessPerson(int dept_id, int role_id, string searchVal)
        {
            if (dept_id <= 0 || role_id <= 0)
            {
                throw new MessageException("请选择部门角色!");
            }

            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            return await Db.Queryable<p_employee, p_employee_role>((e, er) => new object[] { JoinType.Left, e.id == er.employee_id })
                           .Where((e, er) => er.org_id == userInfo.org_id && er.dept_id == dept_id && er.role_id == role_id && e.expire_time > DateTime.Now)
                           .WhereIF(!string.IsNullOrEmpty(searchVal), (e, er) => e.phone_no.Contains(searchVal) || e.name.Contains(searchVal))
                           .Select((e, er) => new PersonProcessModel { name = e.name, person_id = e.id, phone = e.phone_no })
                           .WithCache()
                           .ToListAsync();
        }

        /// <summary>
        /// 门店添加人员
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> StoreAddPerson(StoreAddPersonModel entity)
        {
            if (entity.employeeIds.Count == 0)
            {
                throw new MessageException("未选择人员!");
            }
            if (entity.store_id <= 0)
            {
                throw new MessageException("未获取到门店!");
            }
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            var result = Db.Ado.UseTran(() =>
            {
                //查询本门店关联角色
                var roleId = Db.Queryable<p_role>()
                                 .Where(r => r.org_id == userInfo.org_id && r.id == entity.role_id && r.store_id == entity.store_id)
                                 .Select(r => r.link_id)
                                 .WithCache()
                                 .First();

                //删除原机构角色
                Db.Deleteable<p_employee_role>()
                    .Where(s => s.org_id == userInfo.org_id && s.store_id == 0 && entity.employeeIds.Contains(s.employee_id) && s.role_id == roleId && s.is_admin == false)
                    .RemoveDataCache()
                    .EnableDiffLogEvent()
                    .ExecuteCommand();

                //新增角色
                var newRole = entity.employeeIds.Select(s => new p_employee_role { org_id = userInfo.org_id, dept_id = entity.dept_id, employee_id = s, is_admin = false, store_id = entity.store_id, role_id = entity.role_id }).ToList();
                Db.Insertable(newRole).ExecuteCommand();
                redisCache.RemoveAll<p_employee_role>();

            });
            return result.IsSuccess;
        }

        /// <summary>
        /// 门店移除角色
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> StoreRemovePerson(StoreRemovePersonModel entity)
        {
            if (entity.store_id <= 0 || entity.role_id <= 0 || entity.id <= 0)
            {
                throw new MessageException("未获取到用户信息!");
            }
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            //查询原角色
            var link_role = await Db.Queryable<p_role, p_role>((nowrole, beforerole) => new object[] { JoinType.Left, nowrole.link_id == beforerole.id })
                                  .Where((nowrole, beforerole) => nowrole.id == entity.role_id && nowrole.org_id == userInfo.org_id && nowrole.store_id == entity.store_id)
                                  .Select((nowrole, beforerole) => new { beforerole.dept_id, beforerole.id, nowrole.link_id })
                                  .WithCache()
                                  .FirstAsync();
            if (link_role?.link_id <= 0)
            {
                throw new MessageException("未获取到原角色!");
            }
            var result = Db.Ado.UseTran(() =>
            {
                //删除现角色
                Db.Deleteable<p_employee_role>()
                    .Where(s => s.org_id == userInfo.org_id && s.store_id == entity.store_id && s.employee_id == entity.id && s.role_id == entity.role_id)
                    .RemoveDataCache()
                    .EnableDiffLogEvent()
                    .ExecuteCommand();

                //新增机构角色

                var newRole = new p_employee_role { org_id = userInfo.org_id, dept_id = link_role.dept_id, employee_id = entity.id, is_admin = false, store_id = 0, role_id = link_role.id };
                Db.Insertable(newRole).ExecuteCommand();
                redisCache.RemoveAll<p_employee_role>();

            });
            return result.IsSuccess;

        }
        /// <summary>
        /// 获取机构下人员
        /// </summary>
        /// <param name="dept_id"></param>
        /// <param name="role_id"></param>
        /// <returns></returns>
        public async Task<List<PersonProcessModel>> GetOrgPerson(int dept_id, int role_id)
        {

            if (dept_id <= 0 || role_id <= 0)
            {
                throw new MessageException("请选择部门角色!");
            }

            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            //查询关联部门，角色
            var deptId = await Db.Queryable<p_dept>()
                               .Where(d => d.org_id == userInfo.org_id && d.id == dept_id)
                               .Select(d => d.link_id)
                               .WithCache()
                               .FirstAsync();

            var roleId = await Db.Queryable<p_role>()
                               .Where(r => r.org_id == userInfo.org_id && r.id == role_id)
                               .Select(r => r.link_id)
                               .WithCache()
                               .FirstAsync();

            return await Db.Queryable<p_employee, p_employee_role>((e, er) => new object[] { JoinType.Left, e.id == er.employee_id })
                           .Where((e, er) => er.org_id == userInfo.org_id && er.dept_id == deptId && er.role_id == roleId && e.expire_time > DateTime.Now)
                           .Select((e, er) => new PersonProcessModel { name = e.name, person_id = e.id, phone = e.phone_no })
                           .WithCache()
                           .ToListAsync();
        }

    }
}
