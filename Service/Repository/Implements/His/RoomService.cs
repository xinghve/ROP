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

namespace Service.Repository.Implements.His
{
    /// <summary>
    /// 医疗室
    /// </summary>
    public class RoomService : DbContext, IRoomService
    {
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <returns></returns>
        public async Task<bool> AddAsync(Room entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            //判断是否存在
            var isExisteName = await Db.Queryable<p_room>().WithCache().AnyAsync(a => a.org_id == userInfo.org_id && a.store_id == entity.store_id && a.name == entity.name);
            if (isExisteName)
            {
                throw new MessageException("当前门店已存在此医疗室");
            }

            //查询门店名称
            var store_name = await Db.Queryable<p_store>().Where(w => w.id == entity.store_id).Select(s => s.name).WithCache().FirstAsync();

            //新增
            var result = await Db.Ado.UseTranAsync(() =>
            {
                var room = new p_room { org_id = userInfo.org_id, store_name = store_name, creater = userInfo.name, creater_id = userInfo.id, create_date = DateTime.Now, equipment = entity.equipment, name = entity.name, position = entity.position, state = 30, store_id = entity.store_id, wait_times = entity.wait_times, work_times = entity.work_times };
                //添加医疗室返回医疗室id
                var roomId = Db.Insertable(room).ExecuteReturnIdentity();
                redisCache.RemoveAll<p_room>();

                //添加医疗室对应项目规格
                if (entity.room_itemspecs_list.Count > 0)
                {
                    var equipment_Itemspecs = entity.room_itemspecs_list.Select(s => new p_room_itemspec { room_id = roomId, itemid = s.itemid, specid = s.specid, specname = s.specname, tradename = s.tradename, work_times = s.work_times }).ToList();
                    Db.Insertable(equipment_Itemspecs).ExecuteCommand();
                    redisCache.RemoveAll<p_room_itemspec>();
                }
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 获取医疗室列表
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="state">状态</param>
        /// <param name="equipment">是否存放设备</param>
        /// <returns></returns>
        public async Task<List<p_room>> GetListAsync(int store_id, short state, short equipment)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            return await Db.Queryable<p_room>()
                .Where(w => w.org_id == userInfo.org_id)
                .WhereIF(store_id > 0, w => w.store_id == store_id)
                .WhereIF(state == 0, w => w.state == 0)
                .WhereIF(state == 1, w => w.state != 0)
                .WhereIF(state > 1, w => w.state == state)
                .WhereIF(equipment > 0, w => w.equipment == equipment)
                .WithCache()
                .ToListAsync();
        }

        /// <summary>
        /// 获得分页列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Page<Room>> GetPagesAsync(RoomPagesSearch entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            var list = await Db.Queryable<p_room>()
            .Where(w => w.org_id == userInfo.org_id)
            .WhereIF(entity.store_id > 0, w => w.store_id == entity.store_id)
            .WhereIF(!string.IsNullOrEmpty(entity.name), w => w.name.Contains(entity.name))
            .Select(s => new Room { id = s.id, name = s.name, org_id = s.org_id, store_id = s.store_id, store_name = s.store_name, creater = s.creater, creater_id = s.creater_id, create_date = s.create_date, equipment = s.equipment, position = s.position, state = s.state, wait_times = s.wait_times, work_times = s.work_times })
            .OrderBy(entity.order + orderTypeStr)
            .WithCache()
            .ToPageAsync(entity.page, entity.limit);
            var room_ids = list.Items.Select(s => s.id).ToList();
            var room_itemspec_list = await Db.Queryable<p_room_itemspec, h_itemspec>((ri, i) => new object[] { JoinType.Left, ri.itemid == i.itemid && ri.specid == i.specid }).Where((ri, i) => room_ids.Contains(ri.room_id)).Select((ri, i) => new room_itemspec { itemid = ri.itemid, sale_price = i.sale_price, room_id = ri.room_id, specid = ri.specid, specname = ri.specname, tradename = ri.tradename, salseunit = i.salseunit, work_times = ri.work_times }).ToListAsync();
            list.Items = list.Items.Select(s => new Room { id = s.id, name = s.name, org_id = s.org_id, store_id = s.store_id, store_name = s.store_name, creater = s.creater, creater_id = s.creater_id, create_date = s.create_date, equipment = s.equipment, position = s.position, state = s.state, work_times = s.work_times, wait_times = s.wait_times, room_itemspecs_list = room_itemspec_list.Where(w => w.room_id == s.id).ToList() }).ToList();
            return list;
        }

        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ModifyAsync(Room entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            //判断是否存在
            var isExisteName = await Db.Queryable<p_room>().WithCache().AnyAsync(a => a.org_id == userInfo.org_id && a.store_id == entity.store_id && a.name == entity.name && a.id != entity.id);
            if (isExisteName)
            {
                throw new MessageException("当前门店已存在此医疗室");
            }

            //判断是否存在使用中的设备
            var isExiste = await Db.Queryable<p_equipment_detials>()
                .WithCache()
                .AnyAsync(ed => ed.room_id == entity.id&&ed.state!=0);
            if (isExiste&&entity.state==0)
            {
                throw new MessageException("有使用中的设备存在此治疗室，不能禁用!");
            }


            //新增
            var result = await Db.Ado.UseTranAsync(() =>
            {
                //修改医疗室
                Db.Updateable<p_room>().SetColumns(s => new p_room { name = entity.name, equipment = entity.equipment, position = entity.position, wait_times = entity.wait_times, work_times = entity.work_times }).Where(w => w.id == entity.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                //删除医疗室对应项目规格
                Db.Deleteable<p_room_itemspec>().Where(w => w.room_id == entity.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                //添加医疗室对应项目规格
                if (entity.room_itemspecs_list.Count > 0)
                {
                    var equipment_Itemspecs = entity.room_itemspecs_list.Select(s => new p_room_itemspec { room_id = entity.id, itemid = s.itemid, specid = s.specid, specname = s.specname, tradename = s.tradename, work_times = s.work_times }).ToList();
                    Db.Insertable(equipment_Itemspecs).ExecuteCommand();
                    redisCache.RemoveAll<p_room_itemspec>();
                }
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <returns></returns>
        public async Task<int> ModifyStateAsync(p_room entity)
        {
            //判断是否存在
            var isExiste = await Db.Queryable<p_room>()
                .Where(a => a.id == entity.id)
                .WithCache()
                .FirstAsync();
            if (isExiste==null)
            {
                throw new MessageException("当前门店无此医疗室");
            }
            if (isExiste?.state==31)
            {
                throw new MessageException("使用中无法修改");
            }
            //判断是否存在使用中的设备
            var isExisteEqu = await Db.Queryable<p_equipment_detials>()
                .WithCache()
                .AnyAsync(ed => ed.room_id == entity.id && ed.state != 0);
            if (isExisteEqu && entity.state == 0)
            {
                throw new MessageException("有使用中的设备存在此治疗室，不能禁用!");
            }



            return await Db.Updateable<p_room>().SetColumns(s => new p_room { state = entity.state }).Where(w => w.id == entity.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
        }

        /// <summary>
        /// 根据项目规格获取医疗室列表
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

            var result =await Db.Queryable<p_room_itemspec, p_room>((ri, r) => new object[] { JoinType.Left, ri.room_id == r.id })
                .Where((ri, r) => ri.itemid == item_id && ri.specid == spec_id && r.state != 0)
                .Select((ri, r) => new
                {
                    r.id,
                    room_name = r.name,
                    address = r.position,
                    ri.work_times,
                    num = SqlFunc.Subqueryable<his_room_scheduling>()
                .Where(w => w.room_id == r.id && w.work_date == dateTime && w.stateid == 16).Count(),
                    total_times = SqlFunc.Subqueryable<his_room_scheduling>().Where(w => w.room_id == r.id && w.work_date == dateTime && w.stateid == 16)
                .Select(s => SqlFunc.IsNull(SqlFunc.AggregateSum(s.work_times.Value + s.wait_times.Value), 0)).ToString()
                })
                .WithCache()
                .ToListAsync();
            if (result.Count>0)
            {
                return result;
            }

            List<object> nothing = new List<object>();
            return nothing;
        }
    }
}
