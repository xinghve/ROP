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
    /// 器械设备
    /// </summary>
    public class EquipmentService : DbContext, IEquipmentService
    {
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <returns></returns>
        public async Task<bool> AddAsync(Equipment entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            //判断是否存在
            var isExisteName = await Db.Queryable<p_equipment>().WithCache().AnyAsync(a => a.org_id == userInfo.org_id && a.store_id == entity.equipment.store_id && a.name == entity.equipment.name && a.model == entity.equipment.model && a.manufactor_id == entity.equipment.manufactor_id && a.manufactor == entity.equipment.manufactor);
            if (isExisteName)
            {
                throw new MessageException("当前集团或门店已存在此设备");
            }

            //查询门店名称
            var store_name = await Db.Queryable<p_store>().Where(w => w.id == entity.equipment.store_id).Select(s => s.name).WithCache().FirstAsync();

            //新增
            var result = Db.Ado.UseTran(() =>
            {
                entity.equipment.org_id = userInfo.org_id;
                entity.equipment.store_name = store_name;
                //添加设备返回设备id
                var equipmentId = Db.Insertable(entity.equipment).ExecuteReturnIdentity();
                redisCache.RemoveAll<p_equipment>();

                //添加设备对应项目规格
                if (entity.equipment_Itemspecs.Count > 0)
                {
                    var equipment_Itemspecs = entity.equipment_Itemspecs.Select(s => new p_equipment_itemspec { equipment_id = equipmentId, itemid = s.itemid, specid = s.specid, specname = s.specname, tradename = s.tradename, work_times = s.work_times }).ToList();
                    Db.Insertable(equipment_Itemspecs).ExecuteCommand();
                    redisCache.RemoveAll<p_equipment_itemspec>();
                }
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 获得分页列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Page<EquipmentPages>> GetPagesAsync(EquipmentPagesSearch entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            var list = await Db.Queryable<p_equipment,p_equipment_detials>((w,d)=>new object[] {JoinType.Left,w.id==d.id })
            .Where((w, d) => w.org_id == userInfo.org_id)
            .WhereIF(entity.store_id > 0, (w, d) => w.store_id == entity.store_id)
            .WhereIF(entity.state >= 0, (w, d) => d.state == entity.state||d.state==null)
            .WhereIF(!string.IsNullOrEmpty(entity.manufactor), (w, d) => w.manufactor.Contains(entity.manufactor))
            .WhereIF(!string.IsNullOrEmpty(entity.model), (w, d) => w.model.Contains(entity.model))
            .WhereIF(!string.IsNullOrEmpty(entity.name), (w, d) => w.name.Contains(entity.name))
            .GroupBy((w, d)=>new {w.id,w.manufactor,w.model,w.name,w.org_id,w.store_id,w.store_name,w.manufactor_id })
            .Select((w, d) => new EquipmentPages { id = w.id, manufactor = w.manufactor, model = w.model, name = w.name, org_id = w.org_id, store_id = w.store_id, store_name = w.store_name, manufactor_id = w.manufactor_id })
            .OrderBy(entity.order + orderTypeStr)
            .WithCache()
            .ToPageAsync(entity.page, entity.limit);
            var equipment_ids = list.Items.Select(s => s.id).ToList();
            var equipment_itemspec_list = await Db.Queryable<p_equipment_itemspec>().Where(w => equipment_ids.Contains(w.equipment_id)).ToListAsync();
            list.Items = list.Items.Select(s => new EquipmentPages { id = s.id, manufactor = s.manufactor, model = s.model, name = s.name, org_id = s.org_id, store_id = s.store_id, store_name = s.store_name, manufactor_id = s.manufactor_id, equipment_Itemspecs = equipment_itemspec_list.Where(w => w.equipment_id == s.id).ToList() }).ToList();
            return list;
        }

        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ModifyAsync(Equipment entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            //判断是否存在
            var isExisteName = await Db.Queryable<p_equipment>().WithCache().AnyAsync(a => a.org_id == userInfo.org_id && a.store_id == entity.equipment.store_id && a.name == entity.equipment.name && a.model == entity.equipment.model && a.manufactor_id == entity.equipment.manufactor_id && a.manufactor == entity.equipment.manufactor && a.id != entity.equipment.id);
            if (isExisteName)
            {
                throw new MessageException("当前集团或门店已存在此设备");
            }

            //新增
            var result = Db.Ado.UseTran(() =>
            {
                //修改设备
                Db.Updateable<p_equipment>().SetColumns(s => new p_equipment { manufactor = entity.equipment.manufactor, model = entity.equipment.model, name = entity.equipment.name, manufactor_id = entity.equipment.manufactor_id }).Where(w => w.id == entity.equipment.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                //删除设备对应项目规格
                Db.Deleteable<p_equipment_itemspec>().Where(w => w.equipment_id == entity.equipment.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                //添加设备对应项目规格
                if (entity.equipment_Itemspecs.Count > 0)
                {
                    var equipment_Itemspecs = entity.equipment_Itemspecs.Select(s => new p_equipment_itemspec { equipment_id = entity.equipment.id, itemid = s.itemid, specid = s.specid, specname = s.specname, tradename = s.tradename, work_times = s.work_times }).ToList();
                    Db.Insertable(equipment_Itemspecs).ExecuteCommand();
                    redisCache.RemoveAll<p_equipment_itemspec>();
                }
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 添加一条详细数据
        /// </summary>
        /// <returns></returns>
        public async Task<int> AddDetialsAsync(p_equipment_detials entity)
        {
            //判断是否存在
            var isExisteName = await Db.Queryable<p_equipment_detials>().WithCache().AnyAsync(a => a.id == entity.id && a.no == entity.no);
            if (isExisteName)
            {
                throw new MessageException("当前集团或门店已存在此编号设备");
            }
            var max_times = await Db.Queryable<p_equipment_itemspec>().Where(w => w.equipment_id == entity.id).WithCache().MaxAsync(m => m.work_times);
            if (max_times * 3 > entity.work_times)
            {
                throw new MessageException($"此设备的工作时长必须大于{max_times * 3}分/天");
            }
            redisCache.RemoveAll<p_equipment_detials>();
            return await Db.Insertable(entity).ExecuteCommandAsync();
        }

        /// <summary>
        /// 修改一条详细数据
        /// </summary>
        /// <returns></returns>
        public async Task<int> ModifyDetialsAsync(p_equipment_detials entity)
        {
            //判断是否存在
            var isExisteName = await Db.Queryable<p_equipment_detials>().WithCache()
                                       .Where(e=> e.id == entity.id && e.no == entity.no)
                                       .FirstAsync();
            if (isExisteName==null)
            {
                throw new MessageException("当前集团或门店不存在此编号设备");
            }

            var now = DateTime.Now.ToString("yyyy-MM-dd");
            //使用中不可以修改
            var isExisteEqu = await Db.Queryable<his_equipment_scheduling>()
                                      .Where(" ( to_char(work_date,'yyyy-MM-dd')>=@now ) ", new { now })
                                      .WithCache()
                                      .AnyAsync(a => (a.stateid==16||a.stateid==17)&&a.equipment_id ==entity.id&&a.no==entity.no);
            if (isExisteEqu&&(entity.state==0|| isExisteName.room_id!=entity.room_id))
            {
                throw new MessageException("此编号设备已在预约治疗，无法编辑治疗室跟状态");
            }

            return await Db.Updateable(entity).IgnoreColumns(s => new { s.id, s.no }).Where(w => w.id == entity.id && w.no == entity.no).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
        }

        /// <summary>
        /// 修改一条详细数据状态
        /// </summary>
        /// <returns></returns>
        public async Task<int> ModifyDetialsStateAsync(p_equipment_detials entity)
        {
            //判断是否存在
            var isExiste = await Db.Queryable<p_equipment_detials>()
                .Where(a => a.id == entity.id && a.no == entity.no)
                .WithCache().FirstAsync();
            if (isExiste==null)
            {
                throw new MessageException("当前集团或门店不存在此编号设备");
            }
            if (isExiste?.state == 31)
            {
                throw new MessageException("此设备使用中，无法修改");
            }
            var now = DateTime.Now.ToString("yyyy-MM-dd");
            //使用中不可以修改状态
            var isExisteEqu = await Db.Queryable<his_equipment_scheduling>()
                                      .Where(" ( to_char(work_date,'yyyy-MM-dd')>=@now ) ", new { now })
                                      .WithCache()
                                      .AnyAsync(a => (a.stateid == 16 || a.stateid == 17) && a.equipment_id == entity.id && a.no == entity.no);
            if (isExisteEqu && entity.state == 0 )
            {
                throw new MessageException("此编号设备已在预约治疗，无法编辑状态");
            }



            return await Db.Updateable<p_equipment_detials>().SetColumns(s => new p_equipment_detials { state = entity.state }).Where(w => w.id == entity.id && w.no == entity.no).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
        }

        /// <summary>
        /// 获得详细分页列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Page<p_equipment_detials>> GetDetialsPagesAsync(EquipmentDetialsPagesSearch entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            return await Db.Queryable<p_equipment_detials>()
            .Where(w => w.id == entity.id)
            .WhereIF(!string.IsNullOrEmpty(entity.no), w => w.no.Contains(entity.no))
            .WhereIF(entity.room_id > 0, w => w.room_id == entity.room_id)
            .OrderBy(entity.order + orderTypeStr)
            .WithCache()
            .ToPageAsync(entity.page, entity.limit);
        }

        /// <summary>
        /// 根据医疗室ID获取设备列表
        /// </summary>
        /// <param name="room_id">医疗室ID</param>
        /// <returns></returns>
        public async Task<dynamic> GetListAsync(int room_id)
        {
            return await Db.Queryable<p_equipment_detials, p_equipment>((ed, e) => new object[] { JoinType.Left, ed.id == e.id })
                .Where((ed, e) => ed.room_id == room_id && ed.state == 1)
                .Select((ed, e) => new { ed.id, ed.no, e.name, e.model })
                .WithCache()
                .ToListAsync();
        }

        /// <summary>
        /// 根据项目规格获取设备列表
        /// </summary>
        /// <param name="item_id">项目ID</param>
        /// <param name="spec_id">规格ID</param>
        /// <param name="dateTime">日期</param>
        /// <returns></returns>
        public async Task<dynamic> GetListByItemSpecAsync(int item_id, int spec_id, DateTime? dateTime)
        {
            if (dateTime == null)
            {
                dateTime = DateTime.Now.Date;
            }
            else
            {
                dateTime = dateTime.Value.Date;
            }

            var result = await Db.Queryable<p_equipment_itemspec, p_equipment_detials, p_equipment, p_room>((ei, ed, e, r) => new object[] { JoinType.Left, ei.equipment_id == ed.id, JoinType.Left, ei.equipment_id == e.id, JoinType.Left, ed.room_id == r.id })
                .Where((ei, ed, e, r) => ei.itemid == item_id && ei.specid == spec_id && ed.state == 1)
                .Select((ei, ed, e, r) => new { ed.id, room_id = r.id, e.name, e.model, ed.no, room_name = r.name, address = r.position, ei.work_times, num = SqlFunc.Subqueryable<his_equipment_scheduling>().Where(w => w.equipment_id == ed.id && w.no == ed.no && w.work_date == dateTime && w.stateid == 16).Count(), total_times = SqlFunc.Subqueryable<his_equipment_scheduling>().Where(w => w.equipment_id == ed.id && w.no == ed.no && w.work_date == dateTime && w.stateid == 16).Select(s => SqlFunc.IsNull(SqlFunc.AggregateSum(s.work_times.Value + s.wait_times.Value), 0)).ToString() })
                .WithCache()
                .ToListAsync();

            if (result.Count > 0)
            {
                return result;
            }
            List<object> nothing = new List<object>();
            return nothing;
        }
    }
}
