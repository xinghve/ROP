using Models.DB;
using Models.View.Public;
using Service.Extensions;
using Service.Repository.Interfaces.Public;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;
using Tools.Filter;

namespace Service.Repository.Implements.Public
{
    /// <summary>
    /// 部门
    /// </summary>
    public class DeptService : DbContext, IDeptService
    {
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <returns></returns>
        public async Task<bool> AddAsync(DeptExtent entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            //判断是否存在
            var isExisteName = await Db.Queryable<p_dept>().WithCache().AnyAsync(a => a.org_id == userInfo.org_id && a.store_id == entity.dept.store_id && a.name == entity.dept.name);
            if (isExisteName)
            {
                throw new MessageException("当前集团或门店已存在此部门");
            }

            //新增
            var code = "";
            var result = await Db.Ado.UseTranAsync(() =>
            {
                entity.dept.org_id = userInfo.org_id;
                if (entity.dept.store_id != 0)
                {
                    var maxCode = Db.Queryable<p_dept>().Where(w => w.store_id == entity.dept.store_id).WithCache().Max(m => m.code);
                    if (maxCode == null)
                    {
                        var store = Db.Queryable<p_store>().Where(w => w.id == entity.dept.store_id).WithCache().First();
                        code = store.code + "00001";
                    }
                    else
                    {
                        code = (Int64.Parse(maxCode) + 1).ToString().PadLeft(15, '0');
                    }

                }
                else
                {
                    var maxCode = Db.Queryable<p_dept>().Where(w => w.org_id == userInfo.org_id && w.store_id == 0).WithCache().Max(m => m.code);
                    if (maxCode == null)
                    {
                        var org = Db.Queryable<p_org>().Where(w => w.id == userInfo.org_id).WithCache().First();
                        if (org == null)
                        {
                            code = "000000000000001";
                        }
                        else
                        {
                            code = org.code + "0000000001";
                        }
                    }
                    else
                    {
                        code = (Int64.Parse(maxCode) + 1).ToString().PadLeft(15, '0');
                    }
                }
                entity.dept.code = code;
                entity.dept.state = 1;
                entity.dept.pinyin = ToSpell.GetFirstPinyin(entity.dept.name);
                entity.dept.creater_id = userInfo.id;
                entity.dept.creater = userInfo.name;
                entity.dept.create_date = DateTime.Now;
                entity.dept.withdrawal_date = DateTime.Parse("3000-12-31 23:59:59");
                //添加部门返回部门id
                var deptId = Db.Insertable(entity.dept).ExecuteReturnIdentity();
                redisCache.RemoveAll<p_dept>();
                //部门性质
                if (entity.deptNature.Count <= 0)
                {
                    throw new MessageException("请选择部门性质!");
                }
                //添加部门性质
                var deptNature = entity.deptNature.Select(s => new p_dept_nature { dept_id = deptId, org_id = userInfo.org_id, nature_id = s.nature_id, nature = s.nature }).ToList();
                Db.Insertable(deptNature).ExecuteCommand();
                redisCache.RemoveAll<p_dept_nature>();
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(int id)
        {
            var is_use = await Db.Queryable<p_employee_role>().WithCache().AnyAsync(w => w.dept_id == id);
            if (is_use)
            {
                throw new MessageException("当前部门正在使用中，不能删除");
            }

            var result = await Db.Ado.UseTranAsync(() =>
            {
                //删除部门性质
                Db.Deleteable<p_dept_nature>().Where(w => w.dept_id == id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                //删除部门
                Db.Deleteable<p_dept>().Where(w => w.id == id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

            });
            return result.IsSuccess;
        }

        /// <summary>
        /// 数据列表
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="store_id">门店ID</param>
        /// <param name="disabled_code">状态（-1=所有，0=停用，1=启用）</param>
        /// <param name="nature">性质</param>
        /// <param name="sign">是否集团私有(3=否； 2=是； -1=所有)</param> 
        /// <param name="is_org">是否集团</param> 
        /// <returns></returns>
        public async Task<List<p_dept>> GetListAsync(string name, int store_id, int disabled_code, int nature, short sign = -1)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            return await Db.Queryable<p_dept, p_dept_nature>((d, dn) => new object[] { JoinType.Left, d.id == dn.dept_id })
                .Where((d, dn) => d.org_id == userInfo.org_id && d.store_id == store_id)
                .WhereIF(disabled_code != -1, (d, dn) => d.state == disabled_code)
                .WhereIF(nature != -1, (d, dn) => dn.nature_id == nature)
                .WhereIF(sign != -1, (d, dn) => d.sign == sign)
                .WhereIF(!string.IsNullOrEmpty(name), (d, dn) => d.name.Contains(name))
                .GroupBy((d, dn) => new { d.org_id, d.phone, d.pinyin, d.position, d.professional, d.professional_code, d.service_object, d.service_object_id, d.sex_restriction, d.sex_restriction_id, d.state, d.store_id, d.withdrawal, d.withdrawal_date, d.authorized_bed, d.code, d.creater_id, d.creater, d.create_date, d.dept_type_code, d.dept_type_name, d.execution_nature, d.execution_nature_id, d.introduce, d.name, d.id })
                .Select((d, dn) => d)
                .WithCache()
                .ToListAsync();
        }

        /// <summary>
        /// 数据列表
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <returns></returns>
        public async Task<List<p_dept>> GetDeptListAsync(int store_id)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            //查询门店已存在的部门
            var depts = await Db.Queryable<p_dept>().Where(w => w.store_id == store_id).Select(s => s.link_id).WithCache().ToListAsync();
            return await Db.Queryable<p_dept>().Where(w => w.org_id == userInfo.org_id && w.store_id == 0 && w.sign == 3 && !depts.Contains(w.id)).WithCache().ToListAsync();
        }

        /// <summary>
        /// 获得列表
        /// </summary>
        /// <param model="model">名称</param>
        /// <returns></returns>
        public async Task<Page<DeptModel>> GetPagesAsync(DeptSearchModel model)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var orderTypeStr = " Asc";
            if (model.orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            var list = await Db.Queryable<p_dept, p_dept_nature>((d, dn) => new object[] { JoinType.Left, d.id == dn.dept_id })
                .Where((d, dn) => d.org_id == userInfo.org_id && d.store_id == model.store_id)
                .WhereIF(!string.IsNullOrEmpty(model.name), (d, dn) => d.name.Contains(model.name))
                .GroupBy((d, dn) => new { d.org_id, d.phone, d.pinyin, d.position, d.professional, d.professional_code, d.service_object, d.service_object_id, d.sex_restriction, d.sex_restriction_id, d.state, d.store_id, d.withdrawal, d.withdrawal_date, d.authorized_bed, d.code, d.creater, d.create_date, d.dept_type_code, d.dept_type_name, d.execution_nature, d.execution_nature_id, d.introduce, d.name, d.id })
                .Select((d, dn) => new DeptModel { org_id = d.org_id, phone = d.phone, pinyin = d.pinyin, position = d.position, professional = d.professional, professional_code = d.professional_code, service_object = d.service_object, service_object_id = d.service_object_id, sex_restriction = d.sex_restriction, sex_restriction_id = d.sex_restriction_id, state = d.state, store_id = d.store_id, withdrawal = d.withdrawal, withdrawal_date = d.withdrawal_date, authorized_bed = d.authorized_bed, code = d.code, creater = d.creater, create_date = d.create_date, dept_type_code = d.dept_type_code, dept_type_name = d.dept_type_name, execution_nature = d.execution_nature, execution_nature_id = d.execution_nature_id, introduce = d.introduce, name = d.name, id = d.id,sign=d.sign.Value })
                .OrderBy(model.order + orderTypeStr)
                .WithCache()
                .ToPageAsync(model.page, model.limit);
            //获取部门性质
            var natureLiat = await Db.Queryable<p_dept, p_dept_nature>((d, dn) => new object[] { JoinType.Left, d.id == dn.dept_id }).Select((d, dn) => new { dn.dept_id, dn.nature, dn.nature_id }).WithCache().ToListAsync();

            var newList = list.Items.Select((d, dn) => new DeptModel {sign=d.sign, org_id = d.org_id, phone = d.phone, pinyin = d.pinyin, position = d.position, professional = d.professional, professional_code = d.professional_code, service_object = d.service_object, service_object_id = d.service_object_id, sex_restriction = d.sex_restriction, sex_restriction_id = d.sex_restriction_id, state = d.state, store_id = d.store_id, withdrawal = d.withdrawal, withdrawal_date = d.withdrawal_date, authorized_bed = d.authorized_bed, code = d.code, creater = d.creater, create_date = d.create_date, dept_type_code = d.dept_type_code, dept_type_name = d.dept_type_name, execution_nature = d.execution_nature, execution_nature_id = d.execution_nature_id, introduce = d.introduce, name = d.name, id = d.id, deptNature = natureLiat.Where(w => w.dept_id == d.id).Select(s => new p_dept_nature { nature = s.nature, nature_id = s.nature_id, dept_id = s.dept_id }).ToList() }).ToList();
            list.Items = newList;
            return list;
        }

        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ModifyAsync(DeptExtent entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            //判断是否存在
            var isExisteName = await Db.Queryable<p_dept>().WithCache().AnyAsync(a => a.org_id == userInfo.org_id && a.store_id == entity.dept.store_id && a.name == entity.dept.name && a.id != entity.dept.id);
            if (isExisteName)
            {
                throw new MessageException("当前集团或门店已存在此部门");
            }
            entity.dept.pinyin = ToSpell.GetFirstPinyin(entity.dept.name);
            var result = await Db.Ado.UseTranAsync(() =>
            {
                //编辑部门
                Db.Updateable(entity.dept)
                 .IgnoreColumns(it => new { it.org_id, it.store_id, it.code, it.creater_id, it.creater, it.create_date, it.withdrawal_date, it.state })
                 .Where(w => w.id == entity.dept.id)
                 .RemoveDataCache()
                 .EnableDiffLogEvent()
                 .ExecuteCommand();
                //部门性质
                if (entity.deptNature.Count <= 0)
                {
                    throw new MessageException("请选择部门性质!");
                }
                //删除之前部门性质
                Db.Deleteable<p_dept_nature>().Where(w => w.dept_id == entity.dept.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                //添加部门性质
                var deptNature = entity.deptNature.Select(s => new p_dept_nature { dept_id = entity.dept.id, org_id = userInfo.org_id, nature_id = s.nature_id, nature = s.nature }).ToList();
                Db.Insertable(deptNature).ExecuteCommand();
                redisCache.RemoveAll<p_dept_nature>();
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;

        }

        /// <summary>
        /// 修改状态（状态（0=停用，1=启用））
        /// </summary>
        /// <returns></returns>
        public async Task<int> ModifyDisabledAsync(p_dept dept)
        {
            return await Db.Updateable<p_dept>()
                .SetColumns(s => new p_dept { state = dept.state })
                .Where(w => w.id == dept.id)
                .RemoveDataCache()
                .EnableDiffLogEvent()
                .ExecuteCommandAsync();
        }

        /// <summary>
        /// 获取挂号科室
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public async Task<List<DeptCusModel>> GetCusList(int storeId)
        {
            if (storeId <= 0)
            {
                throw new MessageException("未获取到门店!");
            }
            return await Db.Queryable<p_dept, p_dept_nature>((d, dn) => new object[] { JoinType.Left, d.id == dn.dept_id })
                           .Where((d, dn) => d.store_id == storeId && dn.nature_id == 3 && d.state == 1)
                           .Select((d, dn) => new DeptCusModel { deptId = d.id, text = d.name })
                           .WithCache()
                           .ToListAsync();


        }

        /// <summary>
        /// 获取科室简介
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        public async Task<DeptCusDetail> GetDeptIntroduce(int deptId)
        {
            if (deptId <= 0)
            {
                throw new MessageException("未获取到科室!");
            }

            return await Db.Queryable<p_dept, p_dept_nature>((d, dn) => new object[] { JoinType.Left, d.id == dn.dept_id })
                           .Where((d, dn) => d.id == deptId && dn.nature_id == 3 && d.state == 1)
                           .Select((d, dn) => new DeptCusDetail { name = d.name, introduce = d.introduce, phone = d.phone, position = d.position })
                           .WithCache()
                           .FirstAsync();
        }

        /// <summary>
        /// 复制部门信息到门店
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> CopyAsync(DeptCopy entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            //获取部门信息
            var list = await Db.Queryable<p_dept>().Where(w => entity.dept_ids.Contains(w.id)).WithCache().ToListAsync();
            var nature_list = await Db.Queryable<p_dept_nature>().Where(w => entity.dept_ids.Contains(w.dept_id)).WithCache().ToListAsync();

            //新增
            var code = "";
            var result = await Db.Ado.UseTranAsync(() =>
            {
                foreach (var item in list)
                {
                    var maxCode = Db.Queryable<p_dept>().Where(w => w.store_id == entity.store_id).WithCache().Max(m => m.code);
                    if (maxCode == null)
                    {
                        var store = Db.Queryable<p_store>().Where(w => w.id == entity.store_id).WithCache().First();
                        code = store.code + "00001";
                    }
                    else
                    {
                        code = (Int64.Parse(maxCode) + 1).ToString().PadLeft(15, '0');
                    }
                    item.code = code;
                    item.state = 1;
                    item.creater_id = userInfo.id;
                    item.creater = userInfo.name;
                    item.create_date = DateTime.Now;
                    item.link_id = item.id;
                    item.sign = 3;
                    item.store_id = entity.store_id;
                    //添加部门返回部门id
                    var deptId = Db.Insertable(item).ExecuteReturnIdentity();
                    redisCache.RemoveAll<p_dept>();

                    //添加部门性质
                    var deptNature = nature_list.Where(w => w.dept_id == item.link_id).Select(s => new p_dept_nature { dept_id = deptId, org_id = userInfo.org_id, nature_id = s.nature_id, nature = s.nature }).ToList();
                    Db.Insertable(deptNature).ExecuteCommand();
                    redisCache.RemoveAll<p_dept_nature>();
                }
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 移除门店部门
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> RemoveAsync(DeptRemove entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            var result = await Db.Ado.UseTranAsync(() =>
            {
                //查询要移除的人员
                var list = Db.Queryable<p_employee_role>().Where(w => w.dept_id == entity.dept_id && w.store_id == entity.store_id && w.org_id == userInfo.org_id && w.is_admin == false).WithCache().ToList();

                //查询门店部门关联的集团部门
                var dept_ids = Db.Queryable<p_dept>().Where(w => list.Select(s => s.dept_id).ToList().Contains(w.id)).Select(s => new { s.id, s.link_id }).WithCache().ToList();

                //查询门店职位关联的集团职位
                var role_ids = Db.Queryable<p_role>().Where(w => list.Select(s => s.role_id).ToList().Contains(w.id)).Select(s => new { s.id, s.link_id }).WithCache().ToList();

                //移除人员
                Db.Deleteable<p_employee_role>().Where(w => w.dept_id == entity.dept_id && w.store_id == entity.store_id && w.org_id == userInfo.org_id && w.is_admin == false).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                //添加人员到集团
                list = list.Select(s => new p_employee_role { employee_id = s.employee_id, is_admin = false, org_id = s.org_id, store_id = 0, role_id = role_ids.Where(w => w.id == s.role_id).First().link_id.Value, dept_id = dept_ids.Where(w => w.id == s.dept_id).First().link_id.Value }).ToList();
                Db.Insertable(list).ExecuteCommand();
                redisCache.RemoveAll<p_employee_role>();

                //移除职位
                Db.Deleteable<p_role>().Where(w => w.dept_id == entity.dept_id && w.store_id == entity.store_id && w.org_id == userInfo.org_id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                //移除部门
                Db.Deleteable<p_dept>().Where(w => w.id == entity.dept_id && w.store_id == entity.store_id && w.org_id == userInfo.org_id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 获取当前登录人部门集合
        /// </summary>
        /// <param name="store_id"></param>
        /// <returns></returns>
        public async Task<List<p_dept>> GetCurrentDept(int store_id)
        {
            //获取当前用户信息
            var userinfo = new Tools.IdentityModels.GetUser().userInfo;

            return  await Db.Queryable<p_employee_role, p_dept>((e, d) => new object[] { JoinType.Left, e.dept_id == d.id })
                                 .Where((e, d) => e.org_id == userinfo.org_id && d.state == 1 && e.store_id == store_id&&e.dept_id>0&&e.employee_id==userinfo.id)
                                 .Select((e,d)=>new p_dept { id=d.id,name=d.name })
                                 .WithCache()
                                 .ToListAsync();
                                 
        }
    }
}
