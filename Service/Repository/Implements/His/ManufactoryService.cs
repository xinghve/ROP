using Models.DB;
using Models.View.His;
using Service.Extensions;
using Service.Repository.Interfaces.His;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;
using Tools.Filter;
using static Tools.IdentityModels.GetUser;

namespace Service.Repository.Implements.His
{
    /// <summary>
    /// 供应商业务
    /// </summary>
    public class ManufactoryService : DbContext, IManufactoryService
    {
        private UserInfo userInfo = new Tools.IdentityModels.GetUser().userInfo;
        /// <summary>
        /// 供应商分页数据查询
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Page<ManufactoryModel>> GetPageAsync(ManufactorySearch entity)
        {
            var list = Db.Queryable<h_manufactor, h_manufactor_class, b_codebase>((m, mc, c) => new object[] { JoinType.Left, m.id == mc.manufactor_id, JoinType.Left, mc.class_id == c.id })
                .Where((m, mc, c) => m.org_id == userInfo.org_id && m.state_id != -1)
                .WhereIF(entity.state_id >= 0, (m, mc, c) => m.state_id == entity.state_id)
                .WhereIF(!string.IsNullOrEmpty(entity.name), (m, mc, c) => m.name.Contains(entity.name) || m.spell.Contains(entity.name))
                .WhereIF(entity.catalog_type == 0 && entity.class_id != -1, (m, mc, c) => mc.class_id == entity.class_id)
                .WhereIF(entity.catalog_type == 1, (m, mc, c) => c.catalog_id == entity.class_id)
                .WhereIF(entity.catalog_type == 2, (m, mc, c) => mc.class_id == entity.class_id);

            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }

            var manlist = await list.GroupBy((m, mc, c) => m.id).Select((m, mc, c) => new ManufactoryModel { address = m.address, creater = m.creater, creater_id = m.creater_id, create_date = m.create_date, id = m.id, link_man = m.link_man, modifier = m.modifier, modify_date = m.modify_date, modify_id = m.modify_id, name = m.name, org_id = m.org_id, permit_date = m.permit_date, permit_no = m.permit_no, phone_no = m.phone_no, remarks = m.remarks, spell = m.spell, state_id = m.state_id, tel = m.tel, bank = m.bank, bank_no = m.bank_no }).OrderBy(entity.order + orderTypeStr).WithCache().ToPageAsync(entity.page, entity.limit);

            //查询厂家分类
            var classList = await Db.Queryable<h_manufactor_class, h_manufactor, b_codebase>((mc, m, b) => new object[] { JoinType.Left, m.id == mc.manufactor_id, JoinType.Left, mc.class_id == b.id })
                .Where((mc, m, b) => m.org_id == userInfo.org_id).Select((mc, m, b) => new { mc.manufactor_id, mc.class_id, b.text }).WithCache().ToListAsync();

            var newList = manlist.Items.Select(s => new ManufactoryModel { address = s.address, org_id = s.org_id, creater = s.creater, creater_id = s.creater_id, create_date = s.create_date, id = s.id, link_man = s.link_man, modifier = s.modifier, modify_date = s.modify_date, modify_id = s.modify_id, name = s.name, permit_date = s.permit_date, permit_no = s.permit_no, phone_no = s.phone_no, remarks = s.remarks, spell = s.spell, state_id = s.state_id, tel = s.tel, bank = s.bank, bank_no = s.bank_no, classEntity = classList.Where(w => w.manufactor_id == s.id).Select(w => new ClassModel { class_id = w.class_id, class_name = w.text }).ToList() }).ToList();
            manlist.Items = newList;
            return manlist;
        }

        /// <summary>
        /// 获取供应商List
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type_id">分类ID</param>
        /// <returns></returns>
        public async Task<List<ManufactoryList>> GetManufactoryList(string name, int type_id)
        {
            return await Db.Queryable<h_manufactor, h_manufactor_class>((m, mc) => new object[] { JoinType.Left, m.id == mc.manufactor_id })
                         .Where((m, mc) => m.org_id == userInfo.org_id && m.state_id == 1)
                         .WhereIF(!string.IsNullOrEmpty(name), (m, mc) => m.name.Contains(name))
                         .WhereIF(type_id > 0, (m, mc) => mc.class_id == type_id)
                         .GroupBy((m, mc) => new { m.id, m.name })
                         .Select((m, mc) => new ManufactoryList { manufactor_id = m.id, manufactor = m.name })
                         .WithCache()
                         .ToListAsync();
        }

        /// <summary>
        /// 添加厂家
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> AddAsync(ManufactoryModel entity)
        {
            //查询是否存在相同厂家
            var isExtist = await Db.Queryable<h_manufactor>().WithCache().AnyAsync(s => s.org_id == userInfo.org_id && s.name == entity.name);
            if (isExtist)
            {
                throw new MessageException("已存在此供应商！");
            }

            var newEntity = new h_manufactor();
            newEntity.link_man = entity.link_man;
            newEntity.name = entity.name;
            newEntity.permit_date = entity.permit_date;
            newEntity.permit_no = entity.permit_no;
            newEntity.phone_no = entity.phone_no;
            newEntity.remarks = entity.remarks;
            newEntity.tel = entity.tel;
            newEntity.address = entity.address;
            newEntity.org_id = userInfo.org_id;
            newEntity.spell = ToSpell.GetFirstPinyin(entity.name);
            newEntity.state_id = entity.state_id;
            newEntity.creater = userInfo.name;
            newEntity.creater_id = userInfo.id;
            newEntity.create_date = DateTime.Now;
            newEntity.modify_date = DateTime.Parse("3000-12-31 23:59:59");
            newEntity.bank = entity.bank;
            newEntity.bank_no = entity.bank_no;

            var result = await Db.Ado.UseTranAsync(() =>
            {
                var manufactorId = Db.Insertable(newEntity).ExecuteReturnIdentity();
                redisCache.RemoveAll<h_manufactor>();

                if (entity.classEntity.Count > 0)
                {
                    entity.classEntity.ForEach(c =>
                    {
                        var classModel = new h_manufactor_class { class_id = c.class_id, manufactor_id = manufactorId };
                        Db.Insertable(classModel).ExecuteCommand();
                        redisCache.RemoveAll<h_manufactor_class>();
                    });
                }

            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 编辑厂家
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> ModifyAsync(ManufactoryModel entity)
        {
            if (entity.id <= 0)
            {
                throw new MessageException("请选择供应商！");
            }

            var result = await Db.Ado.UseTranAsync(() =>
            {
                //查询是否存在相同厂家
                var isExtist = Db.Queryable<h_manufactor>().WithCache().Any(s => s.org_id == userInfo.org_id && s.name == entity.name && s.id != entity.id);
                if (isExtist)
                {
                    throw new MessageException("已存在此供应商！");
                }

                //判断是否存在使用中的设备
                var isExiste = Db.Queryable<p_equipment>()
                    .WithCache()
                    .Any(ed => ed.manufactor_id == entity.id);
                if (isExiste && entity.state_id == 0)
                {
                    throw new MessageException("有使用中的设备关联供应商，不能禁用!");
                }


                var newEntity = new h_manufactor();
                newEntity.id = entity.id;
                newEntity.link_man = entity.link_man;
                newEntity.name = entity.name;
                newEntity.permit_date = entity.permit_date;
                newEntity.permit_no = entity.permit_no;
                newEntity.phone_no = entity.phone_no;
                newEntity.remarks = entity.remarks;
                newEntity.tel = entity.tel;
                newEntity.address = entity.address;
                newEntity.state_id = entity.state_id;
                newEntity.spell = ToSpell.GetFirstPinyin(entity.name);
                newEntity.modifier = userInfo.name;
                newEntity.modify_date = DateTime.Now;
                newEntity.modify_id = userInfo.id;
                newEntity.bank = entity.bank;
                newEntity.bank_no = entity.bank_no;

                //删除之前类型
                Db.Deleteable<h_manufactor_class>().Where(w => w.manufactor_id == entity.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                //修改
                Db.Updateable(newEntity).IgnoreColumns(it => new { it.id, it.org_id, it.creater, it.creater_id, it.create_date })
                    .Where(a => a.id == entity.id && a.org_id == userInfo.org_id)
                    .RemoveDataCache()
                    .EnableDiffLogEvent()
                    .ExecuteCommand();

                if (entity.classEntity.Count > 0)
                {
                    entity.classEntity.ForEach(c =>
                    {
                        var classModel = new h_manufactor_class { class_id = c.class_id, manufactor_id = entity.id };
                        Db.Insertable(classModel).ExecuteCommand();
                        redisCache.RemoveAll<h_manufactor_class>();
                    });
                }

            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 删除厂家
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(h_manufactor entity)
        {
            if (entity.id <= 0)
            {
                throw new MessageException("请选择供应商！");
            }
            var result = await Db.Ado.UseTranAsync(() =>
            {
                //查询厂家是否在使用中
                var isUse = Db.Queryable<h_itemspec>().WithCache().Any(s => s.factoryid == entity.id);
                if (isUse)
                {
                    throw new MessageException("此供应商使用中不能删除！");
                }

                //判断是否存在使用中的设备
                var isExiste = Db.Queryable<p_equipment>()
                    .WithCache()
                    .Any(ed => ed.manufactor_id == entity.id);
                if (isExiste)
                {
                    throw new MessageException("有使用中的设备关联供应商，不能删除!");
                }

                //删除之前类型
                Db.Deleteable<h_manufactor_class>().Where(w => w.manufactor_id == entity.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                //修改
                Db.Updateable(entity).SetColumns(it => new h_manufactor { state_id = -1 })
                    .Where(a => a.id == entity.id && a.org_id == userInfo.org_id)
                    .RemoveDataCache()
                    .EnableDiffLogEvent()
                    .ExecuteCommand();
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 设置供应商分类
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task<int> SetManufactorClass(ManufactorClassModel list)
        {
            if (list.class_id <= 0)
            {
                throw new MessageException("请选择分类！");
            }
            if (list.manufactoryList.Count <= 0)
            {
                throw new MessageException("请选择供应商！");
            }

            var entity = list.manufactoryList.Select(s => new h_manufactor_class { class_id = list.class_id, manufactor_id = s.manufactor_id }).ToList();

            var isSuccess = await Db.Insertable(entity).ExecuteCommandAsync();
            redisCache.RemoveAll<h_manufactor_class>();

            return isSuccess;
        }

        /// <summary>
        /// 设置状态
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> SetState(ManufactorSetState entity)
        {
            if (entity.id <= 0)
            {
                throw new MessageException("请选择供应商！");
            }

            //判断是否存在使用中的设备
            var isExiste = await Db.Queryable<p_equipment>()
                .WithCache()
                .AnyAsync(ed => ed.manufactor_id == entity.id);
            if (isExiste && entity.state_id == 0)
            {
                throw new MessageException("有使用中的设备关联供应商，不能禁用!");
            }


            return await Db.Updateable<h_manufactor>(entity).SetColumns(s => new h_manufactor { state_id = entity.state_id })
                           .Where(s => s.org_id == userInfo.org_id && s.id == entity.id)
                           .RemoveDataCache()
                           .EnableDiffLogEvent()
                           .ExecuteCommandAsync();
        }
    }
}
