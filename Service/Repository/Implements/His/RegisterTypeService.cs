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
    /// 挂号类别
    /// </summary>
    public class RegisterTypeService : DbContext, IRegisterTypeService
    {
        //获取用户信息
        UserInfo userInfo = new Tools.IdentityModels.GetUser().userInfo;

        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <returns></returns>
        public async Task<bool> AddAsync(RegisterType entity)
        {
            //判断是否存在
            var isExisteName = await Db.Queryable<his_registertype>().WithCache().AnyAsync(a => a.orgid == userInfo.org_id && a.typename == entity.typename);
            if (isExisteName)
            {
                throw new MessageException("已存在此挂号类别");
            }
            //判断总金额是否正确
            //if (entity.registertypefees.Sum(s=>s.))
            //{

            //}
            var result = Db.Ado.UseTran(() =>
            {
                var registertype = new his_registertype { amount = entity.amount, orderflag = entity.orderflag, typename = entity.typename, typeid = entity.typeid, orgid = userInfo.org_id, stateid = entity.stateid };
                //添加类别
                var id = Db.Insertable(registertype).ExecuteReturnIdentity();
                redisCache.RemoveAll<his_registertype>();
                //添加类别费用
                //var list = entity.registertypefees.Select(s => new his_registertypefee { orderflag = s.orderflag, quantiry = s.quantiry, specid = s.specid, typeid = id }).ToList();
                var list = entity.registertypefees.Select(s => new his_registertypefee { orderflag = 3, quantiry = 1, specid = s.specid, typeid = id }).ToList();
                Db.Insertable(list).ExecuteCommand();
                redisCache.RemoveAll<his_registertypefee>();
            });
            return result.IsSuccess;
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <returns></returns>
        public async Task<int> DeleteAsync(int id)
        {
            //使用中不能删除
            var now = DateTime.Now.ToString("yyyy-MM-dd");
            //使用中不能删除
            var reg = await Db.Queryable<his_empschedule>()
                            .Where(h => h.typeid == id)
                            .Where(" (@now between to_char(orderbegin,'yyyy-MM-dd') and to_char(orderend,'yyyy-MM-dd') or  to_char(orderbegin,'yyyy-MM-dd')>=@now ) ", new { now })
                            .WithCache()
                            .AnyAsync();
           
            if (reg)
            {
                throw new MessageException("使用中的号别不能删除！");
            }
            return await Db.Deleteable<his_registertype>().Where(w => w.typeid == id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
        }

        /// <summary>
        /// 数据列表
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="orderflag">预约标志</param>
        /// <param name="stateid">状态ID</param>
        /// <returns></returns>
        public async Task<List<RegisterType>> GetListAsync(string name, int orderflag, int stateid)
        {
            var registertypefeeList = await Db.Queryable<his_registertypefee>().ToListAsync();
            var list = await Db.Queryable<his_registertype>()
                .Where(w => w.orgid == userInfo.org_id)
                .WhereIF(!string.IsNullOrEmpty(name), w => w.typename.Contains(name))
                .WhereIF(orderflag > 0, w => w.orderflag == orderflag)
                .WhereIF(stateid > 0, w => w.stateid == stateid)
                .Select(s => new RegisterType { amount = s.amount, orderflag = s.orderflag, orgid = s.orgid, stateid = s.stateid, typeid = s.typeid, typename = s.typename })
                .WithCache()
                .ToListAsync();
            list = list.Select(s => new RegisterType { amount = s.amount, orderflag = s.orderflag, orgid = s.orgid, stateid = s.stateid, typeid = s.typeid, typename = s.typename, registertypefees = registertypefeeList.Where(w => w.typeid == s.typeid).ToList() }).ToList();
            return list;
        }

        /// <summary>
        /// 获得分页列表
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="orderflag">预约标志</param>
        /// <param name="stateid">状态ID</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        public async Task<Page<RegisterType>> GetPagesAsync(string name, int orderflag, int stateid, string order, int orderType, int limit, int page)
        {
            var orderTypeStr = " Asc";
            if (orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            var registertypefeeList = await Db.Queryable<his_registertypefee>().ToListAsync();
            var list = await Db.Queryable<his_registertype>()
                .Where(w => w.orgid == userInfo.org_id)
                .WhereIF(!string.IsNullOrEmpty(name), w => w.typename.Contains(name))
                .WhereIF(orderflag > 0, w => w.orderflag == orderflag)
                .WhereIF(stateid > 0, w => w.stateid == stateid)
                .Select(s => new RegisterType { amount = s.amount, orderflag = s.orderflag, orgid = s.orgid, stateid = s.stateid, typeid = s.typeid, typename = s.typename })
                .OrderBy(order + orderTypeStr)
                .WithCache()
                .ToPageAsync(page, limit);
            list.Items = list.Items.Select(s => new RegisterType { amount = s.amount, orderflag = s.orderflag, orgid = s.orgid, stateid = s.stateid, typeid = s.typeid, typename = s.typename, registertypefees = registertypefeeList.Where(w => w.typeid == s.typeid).ToList() }).ToList();
            return list;
        }

        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ModifyAsync(RegisterType entity)
        {
            //判断是否存在
            var isExisteName = await Db.Queryable<his_registertype>().WithCache().AnyAsync(a => a.orgid == userInfo.org_id && a.typename == entity.typename && a.typeid != entity.typeid);
            if (isExisteName)
            {
                throw new MessageException("已存在此挂号类别");
            }
            //判断总金额是否正确
            //if (entity.registertypefees.Sum(s=>s.))
            //{

            //}
            var now = DateTime.Now.ToString("yyyy-MM-dd");
            //使用中不能禁用
            var reg = await Db.Queryable<his_empschedule>()
                            .Where(h => h.typeid == entity.typeid)
                            .Where(" (@now between to_char(orderbegin,'yyyy-MM-dd') and to_char(orderend,'yyyy-MM-dd') or  to_char(orderbegin,'yyyy-MM-dd')>=@now)", new { now })
                            .WithCache()
                            .AnyAsync();
            if (reg&&entity.stateid==0)
            {
                throw new MessageException("使用中的号别不能禁用！");
            }

            var result = Db.Ado.UseTran(() =>
            {
                var registertype = new his_registertype { amount = entity.amount, orderflag = entity.orderflag, typename = entity.typename, typeid = entity.typeid, stateid = entity.stateid };
                //修改类别
                Db.Updateable(registertype).IgnoreColumns(it => new { it.orgid }).Where(w => w.typeid == entity.typeid).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                //删除类别费用
                Db.Deleteable<his_registertypefee>().Where(w => w.typeid == entity.typeid).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                //添加类别费用
                //var list = entity.registertypefees.Select(s => new his_registertypefee { orderflag = s.orderflag, quantiry = s.quantiry, specid = s.specid, typeid = entity.registertype.typeid }).ToList();
                var list = entity.registertypefees.Select(s => new his_registertypefee { orderflag = 3, quantiry = 1, specid = s.specid, typeid = entity.typeid }).ToList();
                Db.Insertable(list).ExecuteCommand();
                redisCache.RemoveAll<his_registertypefee>();
            });
            return result.IsSuccess;
        }
    }
}
