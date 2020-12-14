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
using static Tools.IdentityModels.GetUser;

namespace Service.Repository.Implements.Public
{
    /// <summary>
    /// 诊室
    /// </summary>
    public class ConsRoomService : DbContext, IConsRoomService
    {
        //获取用户信息
        UserInfo userInfo = new Tools.IdentityModels.GetUser().userInfo;

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> AddAsync(ConsRoomDept entity)
        {
            var consRoom = new p_cons_room { org_id = userInfo.org_id, store_id = entity.store_id, name = entity.name, position = entity.position, remarks = entity.remarks };
            var depts = entity.depts;
            //判断是否存在
            var isExisteName = await Db.Queryable<p_cons_room>().WithCache().AnyAsync(a => a.org_id == userInfo.org_id && a.store_id == consRoom.store_id && a.name == consRoom.name);
            if (isExisteName)
            {
                throw new MessageException("当前门店已存在此诊室");
            }
            var result = await Db.Ado.UseTranAsync(() =>
            {
                var id = Db.Insertable(consRoom).ExecuteReturnIdentity();
                redisCache.RemoveAll<p_cons_room>();

                //添加诊室所属科室
                if (entity.depts.Count > 0)
                {
                    var list = entity.depts.Select(s => new p_cons_room_dept { cons_room_id = id, dept_id = s.id }).ToList();
                    Db.Insertable(list).ExecuteCommand();
                    redisCache.RemoveAll<p_cons_room_dept>();
                }
            });
            return result.IsSuccess;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteAsync(int id)
        {
            var isExiste = await Db.Queryable<his_empschedule>().WithCache().AnyAsync(a => a.roomid == id && a.orderend >= DateTime.Now);
            if (isExiste)
            {
                throw new MessageException("诊室已排班，若需删除，请先修改排班");
            }
            //删除诊室所属科室
            await Db.Deleteable<p_cons_room_dept>().Where(w => w.cons_room_id == id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
            //删除诊室
            return await Db.Deleteable<p_cons_room>().Where(w => w.id == id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="store_id">门店ID</param>
        /// <param name="dept_id">部门ID</param>
        /// <returns></returns>
        public async Task<List<ConsRoomDept>> GetListAsync(string name, int store_id, int dept_id)
        {
            var roomDept = await Db.Queryable<p_cons_room_dept, p_dept>((crd, d) => new object[] { JoinType.Left, crd.dept_id == d.id }).Select((crd, d) => new { crd.cons_room_id, crd.dept_id, d.name }).ToListAsync();
            var list = await Db.Queryable<p_cons_room, p_cons_room_dept>((cr, crd) => new object[] { JoinType.Left, cr.id == crd.cons_room_id })
                .Where((cr, crd) => cr.org_id == userInfo.org_id && cr.store_id == store_id)
                .WhereIF(!string.IsNullOrEmpty(name), (cr, crd) => cr.name.Contains(name))
                .WhereIF(dept_id > 0, (cr, crd) => crd.dept_id == dept_id)
                .GroupBy((cr, crd) => new { cr.id, cr.org_id, cr.store_id, cr.name, cr.position, cr.remarks })
                .Select((cr, crd) => new ConsRoomDept { id = cr.id, org_id = cr.org_id, store_id = cr.store_id, name = cr.name, position = cr.position, remarks = cr.remarks })
                .WithCache()
                .ToListAsync();
            list = list.Select(s => new ConsRoomDept { id = s.id, org_id = s.org_id, store_id = s.store_id, name = s.name, position = s.position, remarks = s.remarks, depts = roomDept.Where(w => w.cons_room_id == s.id).Select(ss => new dept { id = ss.dept_id, name = ss.name }).ToList() }).ToList();
            return list;
        }

        /// <summary>
        /// 获得分页列表
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="name">名称</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        public async Task<Page<ConsRoomDept>> GetPagesAsync(int store_id, string name, string order, int orderType, int limit, int page)
        {
            var orderTypeStr = " Asc";
            if (orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            var roomDept = await Db.Queryable<p_cons_room_dept, p_dept>((crd, d) => new object[] { JoinType.Left, crd.dept_id == d.id }).Select((crd, d) => new { crd.cons_room_id, crd.dept_id, d.name }).ToListAsync();
            var list = await Db.Queryable<p_cons_room>()
                .Where(w => w.org_id == userInfo.org_id && w.store_id == store_id)
                .WhereIF(!string.IsNullOrEmpty(name), w => w.name.Contains(name))
                .Select(s => new ConsRoomDept { id = s.id, org_id = s.org_id, store_id = s.store_id, name = s.name, position = s.position, remarks = s.remarks })
                .OrderBy(order + orderTypeStr)
                .WithCache()
                .ToPageAsync(page, limit);
            list.Items = list.Items.Select(s => new ConsRoomDept { id = s.id, org_id = s.org_id, store_id = s.store_id, name = s.name, position = s.position, remarks = s.remarks, depts = roomDept.Where(w => w.cons_room_id == s.id).Select(ss => new dept { id = ss.dept_id, name = ss.name }).ToList() }).ToList();
            return list;
            //list.Items = list.Items.Select(s => new ConsRoomDept { id = s.id, org_id = s.org_id, store_id = s.store_id, name = s.name, position = s.position, remarks = s.remarks, depts = roomDept.Where(w => w.cons_room_id == s.id).Select(ss => new dept { id = ss.dept_id, name = ss.name }).ToList() }).ToList();
            //return list;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> ModifyAsync(ConsRoomDept entity)
        {
            var consRoom = new p_cons_room { id = entity.id, org_id = userInfo.org_id, store_id = entity.store_id, name = entity.name, position = entity.position, remarks = entity.remarks };
            var depts = entity.depts;
            //判断是否存在
            var isExisteName = await Db.Queryable<p_cons_room>().WithCache().AnyAsync(a => a.org_id == userInfo.org_id && a.store_id == consRoom.store_id && a.name == consRoom.name && a.id != consRoom.id);
            if (isExisteName)
            {
                throw new MessageException("当前门店已存在此诊室");
            }
            var result = await Db.Ado.UseTranAsync(() =>
            {
                //修改诊室信息
                Db.Updateable(consRoom).IgnoreColumns(it => new { it.org_id, it.store_id }).Where(w => w.id == consRoom.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                //删除诊室所属科室
                Db.Deleteable<p_cons_room_dept>().Where(w => w.cons_room_id == consRoom.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                //添加诊室所属科室
                if (entity.depts.Count > 0)
                {
                    var list = entity.depts.Select(s => new p_cons_room_dept { cons_room_id = consRoom.id, dept_id = s.id }).ToList();
                    Db.Insertable(list).ExecuteCommand();
                    redisCache.RemoveAll<p_cons_room_dept>();
                }
            });
            return result.IsSuccess;
        }
    }
}
