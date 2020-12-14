using Models.DB;
using Models.View.His;
using Service.Extensions;
using Service.Repository.Interfaces.His;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace Service.Repository.Implements.His
{
    /// <summary>
    /// 就诊评价
    /// </summary>
    public class EvaluateService : DbContext, IEvaluateService
    {
        /// <summary>
        /// 评价（客户端）
        /// </summary>
        /// <returns></returns>
        public async Task<int> EvaluateAsync(r_evaluate entity)
        {
            entity.evaluate_time = DateTime.Now;
            entity.id = DateTime.Now.ToString("yyyyMMddHHmmssffffff");
            redisCache.RemoveAll<r_evaluate>();
            return await Db.Insertable(entity).ExecuteCommandAsync();
        }

        /// <summary>
        /// 获取评价信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<r_evaluate> GetByIdAsync(string id)
        {
            return await Db.Queryable<r_evaluate>().Where(w => w.id == id).WithCache().FirstAsync();
        }

        /// <summary>
        /// 获得列表（客户端）
        /// </summary>
        /// <param name="state">状态（-1=所有，1=已评价，0=待评价）</param>
        /// <returns></returns>
        public async Task<dynamic> GetListAsync(short state)
        {
            //获客户信息
            var archivesInfo = new Tools.IdentityModels.GetArchives().archives;
            return await Db.Queryable<his_clinicrecord, r_evaluate>((hc, re) => new object[] { JoinType.Left, hc.clinicid == re.clinic_id })
                .Where((hc, re) => hc.centerid == archivesInfo.id && hc.stateid == 15)
                .WhereIF(state == 1, (hc, re) => re.id != null)
                .WhereIF(state == 0, (hc, re) => re.id == null)
                .Select((hc, re) => new { hc.regdate, hc.deptname, state = SqlFunc.HasValue(re.id), re.id, clinic_id = hc.clinicid })
                .WithCache()
                .ToListAsync();
        }

        /// <summary>
        /// 评价分页
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Page<EvaluatePageModel>> GetPageAsync(EvaluateSearch entity )
        {
            //获客户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }

            return await Db.Queryable<r_evaluate, his_clinicrecord,his_register,p_store,c_archives>((e, c,r,s,a) => new object[] { JoinType.Left, e.clinic_id == c.clinicid,JoinType.Left,r.clinicid==c.clinicid,JoinType.Left,r.store_id==s.id,JoinType.Left,a.id==r.centerid })
                 .Where((e, c,r,s,a) => c.orgid == userInfo.org_id)
                 .WhereIF(entity.storeId > 0, (e, c,r,s,a) =>r.store_id==entity.storeId)
                 .WhereIF(entity.clinic_name!=null, (e, c, r, s,a) => a.name.Contains(entity.clinic_name))
                 .WhereIF(entity.startTime != null, (e, c, r,s, a) => e.evaluate_time >= entity.startTime)
                 .WhereIF(entity.endTime != null, (e, c, r,s, a) => e.evaluate_time <= entity.endTime)
                 .Select((e,c,r,s, a) =>new EvaluatePageModel { clinic_id=e.clinic_id, doctors_level=e.doctors_level, doctor_context=e.doctor_context, evaluate_time=e.evaluate_time, id=e.id, manager_context=e.manager_context, manager_level=e.manager_level, other_context=e.other_context, other_level=e.other_level, storeName=s.name, clinic_name=a.name, phone=a.phone })
                 .OrderBy(entity.order + orderTypeStr)
                 .WithCache()
                 .ToPageAsync(entity.page, entity.limit);

        }
    }
}
