using Models.DB;
using Models.View.Business;
using Service.Extensions;
using Service.Repository.Interfaces.Business;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;
using Tools.Filter;
using Tools.WebSocket;

namespace Service.Repository.Implements.Business
{
    /// <summary>
    /// 审批业务（个人中心）
    /// </summary>
    public class ApprovalService : DbContext, IApprovalService
    {
        #region 分页查询数据
        /// <summary>
        /// 获取登录人审核分页数据（待审核与我发起的审核）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Page<ApprovalModel>> GetApprovalPage(ApprovalSearchModel entity)
        {
            //获取当前用户信息
            var userinfo = new Tools.IdentityModels.GetUser().userInfo;

            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            var start = !string.IsNullOrEmpty(entity.apply_start_time) ? DateTime.Parse(entity.apply_start_time + " 00:00:00") : DateTime.Parse("1949-01-01");
            var end = !string.IsNullOrEmpty(entity.apply_end_time) ? DateTime.Parse(entity.apply_end_time + " 23:59:59") : DateTime.Parse("3000-01-01");

            //查询当前登录人所属部门角色
            var roleList = new List<int>();
            if (entity.store_id > 0)
            {
                roleList = await Db.Queryable<p_employee_role, p_dept, p_role>((er, d, r) => new object[] { JoinType.Left, er.dept_id == d.id, JoinType.Left, er.role_id == r.id }).Where((er, d, r) => er.org_id == userinfo.org_id && er.employee_id == userinfo.id && r.link_id > 0).Select((er, d, r) => r.link_id.Value).WithCache().ToListAsync();

            }
            else
            {
                roleList = await Db.Queryable<p_employee_role, p_dept, p_role>((er, d, r) => new object[] { JoinType.Left, er.dept_id == d.id, JoinType.Left, er.role_id == r.id }).Where((er, d, r) => er.org_id == userinfo.org_id && er.employee_id == userinfo.id && r.id != 0).Select((er, d, r) => r.id).WithCache().ToListAsync();

            }
            //通过包含
            var passList = new List<short?> { 34, 39, 33, 25, 15 };
            //撤销包含
            var cancelList = new List<short?> { 35, 32, 7 };
            Page<ApprovalModel> leaveList = new Page<ApprovalModel>();
            //26为待审核
            if (entity.state == 26)
            {
                leaveList = await GetOwnVerify(entity, userinfo, orderTypeStr, start, end, roleList, passList, cancelList);
            }
            else
            {
                leaveList = await GetOwnSet(entity, userinfo, orderTypeStr, start, end, roleList, passList, cancelList);
            }


            return leaveList;

        }

        /// <summary>
        /// 26为待审核
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userinfo"></param>
        /// <param name="orderTypeStr"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="roleList"></param>
        /// <param name="passList"></param>
        /// <returns></returns>
        private async Task<Page<ApprovalModel>> GetOwnVerify(ApprovalSearchModel entity, Tools.IdentityModels.GetUser.UserInfo userinfo, string orderTypeStr, DateTime start, DateTime end, List<int> roleList, List<short?> passList, List<short?> cancelList)
        {

            //查询信息
            return await Db.UnionAll(
                                  Db.Queryable<oa_leave, p_employee, p_process>((o, p, pro) => new object[] { JoinType.Left, o.applicant_id == p.id, JoinType.Left, o.org_process_id == pro.id })
                                  .Where((o, p, pro) => o.org_id == userinfo.org_id && (o.await_verifier_id == userinfo.id || roleList.Contains(o.role_id)) && (o.state == 36 || o.state == 26))
                                  .WhereIF(entity.store_id > 0, (o, p, pro) => o.is_org == false && o.store_id > 0)
                                  .WhereIF(entity.store_id == 0, (o, p, pro) => o.is_org == true)
                                  .Select((o, p, pro) => new ApprovalModel { apply_id = o.applicant_id, role_id = o.role_id, await_verifier_id = o.await_verifier_id.Value, apply_name = o.applicant, apply_no = o.leave_no, apply_time = o.apply_time, state = o.state.Value, type = "请假", type_id = 4, phone = p.phone_no, leave_type_id = o.leave_type_id, duration = o.duration, process_name = pro.name, store = " " })
                                  .WithCache()
                                  ,
                                  Db.Queryable<bus_buy_apply, p_employee, p_process>((o, p, pro) => new object[] { JoinType.Left, o.applicant_id == p.id, JoinType.Left, o.org_process_id == pro.id })
                                  .Where((o, p, pro) => o.org_id == userinfo.org_id && (o.await_verifier_id == userinfo.id || roleList.Contains(o.await_verifier_id.Value)) && (o.state == 36 || o.state == 26) && o.delete_no == null)
                                  .WhereIF(entity.store_id > 0, (o, p, pro) => o.is_org == false && o.store_id > 0)
                                  .WhereIF(entity.store_id == 0, (o, p, pro) => o.is_org == true)
                                  .Select((o, p, pro) => new ApprovalModel { apply_id = o.applicant_id.Value, role_id = o.await_verifier_id.Value, await_verifier_id = o.await_verifier_id.Value, apply_name = o.applicant, apply_no = o.apply_no, apply_time = o.apply_time.Value, state = o.state.Value, type = "采购申请", type_id = 1, phone = p.phone_no, leave_type_id = 0, duration = o.total_price.Value, process_name = pro.name, store = o.store })
                                  .WithCache()
                                  ,
                                   Db.Queryable<bus_transfer_bill, p_employee, p_process>((o, p, pro) => new object[] { JoinType.Left, o.creater_id == p.id, JoinType.Left, o.process_id == pro.id })
                                  .Where((o, p, pro) => o.org_id == userinfo.org_id && (o.await_verifier_id == userinfo.id || roleList.Contains(o.await_verifier_id.Value)) && (o.state == 36 || o.state == 26) && o.delete_no == null)
                                  .WhereIF(entity.store_id > 0, (o, p, pro) => o.is_org == false && o.out_store_id > 0)
                                  .WhereIF(entity.store_id == 0, (o, p, pro) => o.is_org == true)
                                  .Select((o, p, pro) => new ApprovalModel { apply_id = o.creater_id, role_id = o.await_verifier_id.Value, await_verifier_id = o.await_verifier_id.Value, apply_name = o.creater, apply_no = o.bill_no, apply_time = o.create_time, state = o.state, type = "调拨申请", type_id = 2, phone = p.phone_no, leave_type_id = 0, duration = 0, process_name = pro.name, store = o.in_store_name })
                                  .WithCache()
                                  ,
                                    Db.Queryable<bus_requisitions_bill, p_employee, p_process>((o, p, pro) => new object[] { JoinType.Left, o.creater_id == p.id, JoinType.Left, o.process_id == pro.id })
                                  .Where((o, p, pro) => o.org_id == userinfo.org_id && (o.await_verifier_id == userinfo.id || roleList.Contains(o.await_verifier_id.Value)) && (o.state == 36 || o.state == 26))
                                  .WhereIF(entity.store_id > 0, (o, p, pro) => o.is_org == false && o.store_id > 0)
                                  .WhereIF(entity.store_id == 0, (o, p, pro) => o.is_org == true)
                                  .Select((o, p, pro) => new ApprovalModel { apply_id = o.creater_id, role_id = o.await_verifier_id.Value, await_verifier_id = o.await_verifier_id.Value, apply_name = o.creater, apply_no = o.bill_no, apply_time = o.create_time, state = o.state, type = "领用申请", type_id = 3, phone = p.phone_no, leave_type_id = o.type_id, duration = 0, process_name = pro.name, store = o.store_name })
                                  .WithCache()
                                   ,
                                    Db.Queryable<r_assets_scrap, p_employee, p_process>((o, p, pro) => new object[] { JoinType.Left, o.applicant_id == p.id, JoinType.Left, o.process_id == pro.id })
                                  .Where((o, p, pro) => o.org_id == userinfo.org_id && (o.await_verifier_id == userinfo.id || roleList.Contains(o.await_verifier_id.Value)) && (o.state == 36 || o.state == 26))
                                  .WhereIF(entity.store_id > 0, (o, p, pro) => o.is_org == false && o.store_id > 0)
                                  .WhereIF(entity.store_id == 0, (o, p, pro) => o.is_org == true)
                                  .Select((o, p, pro) => new ApprovalModel { apply_id = o.applicant_id, role_id = o.await_verifier_id.Value, await_verifier_id = o.await_verifier_id.Value, apply_name = o.applicant, apply_no = o.no, apply_time = o.apply_time, state = o.state.Value, type = "报废申请", type_id = 5, phone = p.phone_no, leave_type_id = o.type_id, duration = 0, process_name = pro.name, store = o.store })
                                  .WithCache()
                                    ,
                                    Db.Queryable<bus_loss_overflow, p_employee, p_process>((o, p, pro) => new object[] { JoinType.Left, o.creater_id == p.id, JoinType.Left, o.process_id == pro.id })
                                  .Where((o, p, pro) => o.org_id == userinfo.org_id && (o.await_verifier_id == userinfo.id || roleList.Contains(o.await_verifier_id.Value)) && (o.state == 36 || o.state == 26))
                                  .WhereIF(entity.store_id > 0, (o, p, pro) => o.is_org == false && o.store_id > 0)
                                  .WhereIF(entity.store_id == 0, (o, p, pro) => o.is_org == true)
                                  .Select((o, p, pro) => new ApprovalModel { apply_id = o.creater_id.Value, role_id = o.await_verifier_id.Value, await_verifier_id = o.await_verifier_id.Value, apply_name = o.creater, apply_no = o.no, apply_time = o.create_time.Value, state = o.state.Value, type = "报损报溢申请", type_id = 6, phone = p.phone_no, leave_type_id = 0, duration = 0, process_name = pro.name, store = o.store_name })
                                  .WithCache()

                                  )
                                 .Where(s => SqlFunc.Between(s.apply_time, start, end) && (s.await_verifier_id == userinfo.id || roleList.Contains(s.role_id)))
                                 .WhereIF(entity.type_id > 0, s => s.type_id == entity.type_id)
                                 .WhereIF(!string.IsNullOrEmpty(entity.name), s => s.apply_name.Contains(entity.name) || s.apply_no.Contains(entity.name))
                                 .OrderBy(entity.order + orderTypeStr)
                                 .WithCache()
                                 .ToPageAsync(entity.page, entity.limit);
        }

        /// <summary>
        /// 自己发起的
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userinfo"></param>
        /// <param name="orderTypeStr"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="roleList"></param>
        /// <param name="passList"></param>
        /// <returns></returns>
        private async Task<Page<ApprovalModel>> GetOwnSet(ApprovalSearchModel entity, Tools.IdentityModels.GetUser.UserInfo userinfo, string orderTypeStr, DateTime start, DateTime end, List<int> roleList, List<short?> passList, List<short?> cancelList)
        {

            //查询信息
            return await Db.UnionAll(
                                  Db.Queryable<oa_leave, p_employee, p_process>((o, p, pro) => new object[] { JoinType.Left, o.applicant_id == p.id, JoinType.Left, o.org_process_id == pro.id })
                                  .Where((o, p, pro) => o.applicant_id == userinfo.id && o.org_id == userinfo.org_id)
                                  .WhereIF(entity.own_state > 0, (o, p, pro) => o.state == entity.own_state)
                                  .Select((o, p, pro) => new ApprovalModel { apply_id = o.applicant_id, role_id = o.role_id, await_verifier_id = o.await_verifier_id.Value, apply_name = o.applicant, apply_no = o.leave_no, apply_time = o.apply_time, state = o.state.Value, type = "请假", type_id = 4, phone = p.phone_no, leave_type_id = o.leave_type_id, duration = o.duration, process_name = pro.name, store = " " })
                                  .WithCache()
                                  ,
                                  Db.Queryable<bus_buy_apply, p_employee, p_process>((o, p, pro) => new object[] { JoinType.Left, o.applicant_id == p.id, JoinType.Left, o.org_process_id == pro.id })
                                  .Where((o, p, pro) => o.applicant_id == userinfo.id && o.org_id == userinfo.org_id && o.delete_no == null)
                                  .WhereIF(entity.own_state > 0 && entity.own_state != 34 && entity.own_state != 35, (o, p, pro) => o.state == entity.own_state)
                                  .WhereIF(entity.own_state > 0 && entity.own_state == 34, (o, p, pro) => passList.Contains(o.state))
                                  .WhereIF(entity.own_state > 0 && entity.own_state == 35, (o, p, pro) => cancelList.Contains(o.state))
                                  .Select((o, p, pro) => new ApprovalModel { apply_id = o.applicant_id.Value, role_id = o.await_verifier_id.Value, await_verifier_id = o.await_verifier_id.Value, apply_name = o.applicant, apply_no = o.apply_no, apply_time = o.apply_time.Value, state = o.state.Value, type = "采购申请", type_id = 1, phone = p.phone_no, leave_type_id = 0, duration = o.total_price.Value, process_name = pro.name, store = o.store })
                                  .WithCache()
                                  ,
                                   Db.Queryable<bus_transfer_bill, p_employee, p_process>((o, p, pro) => new object[] { JoinType.Left, o.creater_id == p.id, JoinType.Left, o.process_id == pro.id })
                                  .Where((o, p, pro) => o.creater_id == userinfo.id && o.state != 30 && o.org_id == userinfo.org_id && o.delete_no == null)
                                  .WhereIF(entity.own_state > 0 && entity.own_state != 34 && entity.own_state != 35, (o, p, pro) => o.state == entity.own_state)
                                  .WhereIF(entity.own_state > 0 && entity.own_state == 34, (o, p, pro) => passList.Contains(o.state))
                                   .WhereIF(entity.own_state > 0 && entity.own_state == 35, (o, p, pro) => cancelList.Contains(o.state))
                                  .Select((o, p, pro) => new ApprovalModel { apply_id = o.creater_id, role_id = o.await_verifier_id.Value, await_verifier_id = o.await_verifier_id.Value, apply_name = o.creater, apply_no = o.bill_no, apply_time = o.create_time, state = o.state, type = "调拨申请", type_id = 2, phone = p.phone_no, leave_type_id = 0, duration = 0, process_name = pro.name, store = o.in_store_name })
                                  .WithCache()
                                  ,
                                  Db.Queryable<bus_requisitions_bill, p_employee, p_process>((o, p, pro) => new object[] { JoinType.Left, o.creater_id == p.id, JoinType.Left, o.process_id == pro.id })
                                  .Where((o, p, pro) => o.creater_id == userinfo.id && o.org_id == userinfo.org_id)
                                  .WhereIF(entity.own_state > 0 && entity.own_state != 34 && entity.own_state != 35, (o, p, pro) => o.state == entity.own_state)
                                  .WhereIF(entity.own_state > 0 && entity.own_state == 34, (o, p, pro) => passList.Contains(o.state))
                                   .WhereIF(entity.own_state > 0 && entity.own_state == 35, (o, p, pro) => cancelList.Contains(o.state))
                                  .Select((o, p, pro) => new ApprovalModel { apply_id = o.creater_id, role_id = o.await_verifier_id.Value, await_verifier_id = o.await_verifier_id.Value, apply_name = o.creater, apply_no = o.bill_no, apply_time = o.create_time, state = o.state, type = "领用申请", type_id = 3, phone = p.phone_no, leave_type_id = o.type_id, duration = 0, process_name = pro.name, store = o.store_name })
                                  .WithCache()
                                   ,
                                  Db.Queryable<r_assets_scrap, p_employee, p_process>((o, p, pro) => new object[] { JoinType.Left, o.applicant_id == p.id, JoinType.Left, o.process_id == pro.id })
                                  .Where((o, p, pro) => o.applicant_id == userinfo.id && o.org_id == userinfo.org_id)
                                  .WhereIF(entity.own_state > 0 && entity.own_state != 34 && entity.own_state != 35, (o, p, pro) => o.state == entity.own_state)
                                  .WhereIF(entity.own_state > 0 && entity.own_state == 34, (o, p, pro) => passList.Contains(o.state))
                                   .WhereIF(entity.own_state > 0 && entity.own_state == 35, (o, p, pro) => cancelList.Contains(o.state))
                                  .Select((o, p, pro) => new ApprovalModel { apply_id = o.applicant_id, role_id = o.await_verifier_id.Value, await_verifier_id = o.await_verifier_id.Value, apply_name = o.applicant, apply_no = o.no, apply_time = o.apply_time, state = o.state.Value, type = "报废申请", type_id = 5, phone = p.phone_no, leave_type_id = o.type_id, duration = 0, process_name = pro.name, store = o.store })
                                  .WithCache()
                                    ,
                                  Db.Queryable<bus_loss_overflow, p_employee, p_process>((o, p, pro) => new object[] { JoinType.Left, o.creater_id == p.id, JoinType.Left, o.process_id == pro.id })
                                  .Where((o, p, pro) => o.creater_id == userinfo.id && o.org_id == userinfo.org_id)
                                  .WhereIF(entity.own_state > 0 && entity.own_state != 34 && entity.own_state != 35, (o, p, pro) => o.state == entity.own_state)
                                  .WhereIF(entity.own_state > 0 && entity.own_state == 34, (o, p, pro) => passList.Contains(o.state))
                                   .WhereIF(entity.own_state > 0 && entity.own_state == 35, (o, p, pro) => cancelList.Contains(o.state))
                                  .Select((o, p, pro) => new ApprovalModel { apply_id = o.creater_id.Value, role_id = o.await_verifier_id.Value, await_verifier_id = o.await_verifier_id.Value, apply_name = o.creater, apply_no = o.no, apply_time = o.create_time.Value, state = o.state.Value, type = "报损报溢申请", type_id = 6, phone = p.phone_no, leave_type_id = o.type_id.Value, duration = 0, process_name = pro.name, store = o.store_name })
                                  .WithCache()

                                  )
                                 .Where(s => SqlFunc.Between(s.apply_time, start, end) && s.apply_id == userinfo.id)
                                 .WhereIF(entity.type_id > 0, s => s.type_id == entity.type_id)
                                 .WhereIF(!string.IsNullOrEmpty(entity.name), s => s.apply_name.Contains(entity.name) || s.apply_no.Contains(entity.name))
                                 .OrderBy(entity.order + orderTypeStr)
                                 .WithCache()
                                 .ToPageAsync(entity.page, entity.limit);
        }


        /// <summary>
        /// 已审核数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Page<ApprovalModel>> GetAudited(ApprovalSearchModel entity)
        {
            //获取当前用户信息
            var userinfo = new Tools.IdentityModels.GetUser().userInfo;

            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            var start = !string.IsNullOrEmpty(entity.apply_start_time) ? DateTime.Parse(entity.apply_start_time + " 00:00:00") : DateTime.Parse("1949-01-01");
            var end = !string.IsNullOrEmpty(entity.apply_end_time) ? DateTime.Parse(entity.apply_end_time + " 23:59:59") : DateTime.Parse("3000-01-01");

            //查询当前登录人所属部门角色
            var roleList = new List<int>();

            roleList = await Db.Queryable<p_employee_role, p_dept, p_role>((er, d, r) => new object[] { JoinType.Left, er.dept_id == d.id, JoinType.Left, er.role_id == r.id }).Where((er, d, r) => er.org_id == userinfo.org_id && er.employee_id == userinfo.id && r.id != 0).Select((er, d, r) => r.id).WithCache().ToListAsync();

            var passList = new List<short?> { 34, 39, 33, 25, 15 };

            var cancelList = new List<short?> { 35, 32, 7 };
            //查询信息
            var leaveList = await Db.UnionAll(
                                  Db.Queryable<r_verify_detials, oa_leave, p_employee>((v, o, p) => new object[] { JoinType.Left, v.realted_no == o.leave_no, JoinType.Left, o.applicant_id == p.id })
                                  .Where((v, o, p) => v.verifier_id == userinfo.id && o.org_id == userinfo.org_id)
                                  .WhereIF(entity.state > 0, (v, o, p) => o.state == entity.state)
                                  .Select((v, o, p) => new ApprovalModel { phone = p.phone_no, apply_id = o.applicant_id, role_id = o.role_id, await_verifier_id = o.await_verifier_id.Value, apply_name = o.applicant, apply_no = o.leave_no, apply_time = o.apply_time, state = o.state.Value, type = "请假", type_id = 4 })
                                  .WithCache()
                                  ,
                                  Db.Queryable<r_verify_detials, bus_buy_apply, p_employee>((v, o, p) => new object[] { JoinType.Left, v.realted_no == o.apply_no, JoinType.Left, o.applicant_id == p.id })
                                  .Where((v, o, p) => v.verifier_id == userinfo.id && o.org_id == userinfo.org_id)
                                  .WhereIF(entity.state > 0 && entity.state != 34 && entity.state != 35, (v, o, p) => o.state == entity.state)
                                   .WhereIF(entity.state == 34, (v, o, p) => passList.Contains(o.state))
                                   .WhereIF(entity.state == 35, (v, o, p) => cancelList.Contains(o.state))
                                  .Select((v, o, p) => new ApprovalModel { duration = o.total_price.Value, store = o.store, phone = p.phone_no, apply_id = o.applicant_id.Value, role_id = o.await_verifier_id.Value, await_verifier_id = o.await_verifier_id.Value, apply_name = o.applicant, apply_no = o.apply_no, apply_time = o.apply_time.Value, state = o.state.Value, type = "采购申请", type_id = 1 })
                                  .WithCache()
                                  ,
                                  Db.Queryable<r_verify_detials, bus_transfer_bill, p_employee>((v, o, p) => new object[] { JoinType.Left, v.realted_no == o.bill_no, JoinType.Left, o.creater_id == p.id })
                                  .Where((v, o, p) => v.verifier_id == userinfo.id && o.org_id == userinfo.org_id)
                                  .WhereIF(entity.state > 0 && entity.state != 34 && entity.state != 35, (v, o, p) => o.state == entity.state)
                                   .WhereIF(entity.state == 34, (v, o, p) => passList.Contains(o.state))
                                   .WhereIF(entity.state == 34, (v, o, p) => cancelList.Contains(o.state))
                                  .Select((v, o, p) => new ApprovalModel { store = o.in_store_name, phone = p.phone_no, apply_id = o.creater_id, role_id = o.await_verifier_id.Value, await_verifier_id = o.await_verifier_id.Value, apply_name = o.creater, apply_no = o.bill_no, apply_time = o.create_time, state = o.state, type = "调拨申请", type_id = 2 })
                                  .WithCache()
                                  ,
                                  Db.Queryable<r_verify_detials, bus_requisitions_bill, p_employee>((v, o, p) => new object[] { JoinType.Left, v.realted_no == o.bill_no, JoinType.Left, o.creater_id == p.id })
                                  .Where((v, o, p) => v.verifier_id == userinfo.id && o.org_id == userinfo.org_id)
                                  .WhereIF(entity.state > 0 && entity.state != 34 && entity.state != 35, (v, o, p) => o.state == entity.state)
                                   .WhereIF(entity.state == 34, (v, o, p) => passList.Contains(o.state))
                                   .WhereIF(entity.state == 34, (v, o, p) => cancelList.Contains(o.state))
                                  .Select((v, o, p) => new ApprovalModel { store = o.store_name, phone = p.phone_no, apply_id = o.creater_id, role_id = o.await_verifier_id.Value, await_verifier_id = o.await_verifier_id.Value, apply_name = o.creater, apply_no = o.bill_no, apply_time = o.create_time, state = o.state, type = "领用申请", type_id = 3 })
                                  .WithCache()
                                   ,
                                  Db.Queryable<r_verify_detials, r_assets_scrap, p_employee>((v, o, p) => new object[] { JoinType.Left, v.realted_no == o.no, JoinType.Left, o.applicant_id == p.id })
                                  .Where((v, o, p) => v.verifier_id == userinfo.id && o.org_id == userinfo.org_id)
                                  .WhereIF(entity.state > 0 && entity.state != 34 && entity.state != 35, (v, o, p) => o.state == entity.state)
                                   .WhereIF(entity.state == 34, (v, o, p) => passList.Contains(o.state))
                                   .WhereIF(entity.state == 34, (v, o, p) => cancelList.Contains(o.state))
                                  .Select((v, o, p) => new ApprovalModel { store = o.store, phone = p.phone_no, apply_id = o.applicant_id, role_id = o.await_verifier_id.Value, await_verifier_id = o.await_verifier_id.Value, apply_name = o.applicant, apply_no = o.no, apply_time = o.apply_time, state = o.state.Value, type = "报废申请", type_id = 5 })
                                  .WithCache()
                                    ,
                                  Db.Queryable<r_verify_detials, bus_loss_overflow, p_employee>((v, o, p) => new object[] { JoinType.Left, v.realted_no == o.no, JoinType.Left, o.creater_id == p.id })
                                  .Where((v, o, p) => v.verifier_id == userinfo.id && o.org_id == userinfo.org_id)
                                  .WhereIF(entity.state > 0 && entity.state != 34 && entity.state != 35, (v, o, p) => o.state == entity.state)
                                   .WhereIF(entity.state == 34, (v, o, p) => passList.Contains(o.state))
                                   .WhereIF(entity.state == 34, (v, o, p) => cancelList.Contains(o.state))
                                  .Select((v, o, p) => new ApprovalModel { store = o.store_name, phone = p.phone_no, apply_id = o.creater_id.Value, role_id = o.await_verifier_id.Value, await_verifier_id = o.await_verifier_id.Value, apply_name = o.creater, apply_no = o.no, apply_time = o.create_time.Value, state = o.state.Value, type = "报损报溢申请", type_id = 6 })
                                  .WithCache()

                                  )
                                 .Where(s => SqlFunc.Between(s.apply_time, start, end))
                                 .WhereIF(entity.type_id > 0, s => s.type_id == entity.type_id)
                                 .WhereIF(!string.IsNullOrEmpty(entity.name), s => s.apply_name.Contains(entity.name) || s.apply_no.Contains(entity.name))
                                 .OrderBy(entity.order + orderTypeStr)
                                 .ToPageAsync(entity.page, entity.limit);


            return leaveList;
        }


        /// <summary>
        /// 获取请假详情
        /// </summary>
        /// <param name="apply_no"></param>
        /// <returns></returns>
        public async Task<LeaveRecord> GetLeaveDetail(string apply_no)
        {
            if (string.IsNullOrEmpty(apply_no))
            {
                throw new MessageException("请选择请假单!");
            }
            var userinfo = new Tools.IdentityModels.GetUser().userInfo;

            var leaveDetail = await Db.Queryable<oa_leave>()
                                    .Where(o => o.org_id == userinfo.org_id && o.leave_no == apply_no)
                                    .WithCache()
                                    .FirstAsync();
            if (leaveDetail == null)
            {
                throw new MessageException("未获取到请假信息!");
            }

            //请假图片
            var leaveImg = await Db.Queryable<oa_leave_image>()
                                 .Where(o => o.leave_no == apply_no)
                                 .WithCache()
                                 .ToListAsync();


            //获取审核流程
            var verifyProcess = await Db.Queryable<r_verify_detials>()
                                      .Where(s => s.realted_no == apply_no)
                                      .WithCache()
                                      .ToListAsync();

            var newentity = new VerifyProcessSearch();
            newentity.duration = leaveDetail.duration;
            newentity.leave_type_id = leaveDetail.leave_type_id;
            newentity.store_id = leaveDetail.store_id;
            newentity.user_id = leaveDetail.applicant_id;
            newentity.whichuser = 2;
            var leaveService = new LeaveService();
            var verifyProcessList = await leaveService.GetVerifyProcessDetail(leaveDetail);
            var newProcess = new List<object>();

            verifyProcessList.ForEach(c =>
            {
                c.VerifyProcessDetail = c.VerifyProcessDetail.Select(w => new VerifyProcessDetail { is_org = w.is_org, level = w.level, name = w.name, reason = verifyProcess.Where(v => v.process_level == w.level && v.process_id == w.no).Select(v => v.verify_remark).FirstOrDefault(), role_name = w.role_name, state = verifyProcess.Where(v => v.process_level == w.level && v.process_id == w.no).Select(v => v.state).FirstOrDefault() }).ToList();

            });

            return new LeaveRecord { apply_time = leaveDetail.apply_time, apply_start_time = leaveDetail.apply_start_time, applicant = leaveDetail.applicant, applicant_id = leaveDetail.applicant_id, apply_end_time = leaveDetail.apply_end_time, await_verifier = leaveDetail.await_verifier, await_verifier_id = leaveDetail.await_verifier_id, duration = leaveDetail.duration, leave_cause = leaveDetail.leave_cause, leave_no = leaveDetail.leave_no, leave_type = leaveDetail.leave_type, leave_type_id = leaveDetail.leave_type_id, level = leaveDetail.level, org_id = leaveDetail.org_id, org_process_id = leaveDetail.org_process_id, state = leaveDetail.state, store = leaveDetail.store, store_id = leaveDetail.store_id, total_level = leaveDetail.total_level, verifier = leaveDetail.verifier, verifier_id = leaveDetail.verifier_id, verify_remark = leaveDetail.verify_remark, verify_time = leaveDetail.verify_time, leaveImg = leaveImg, verifyProcess = verifyProcessList };
        }

        #endregion

        #region Common
        /// <summary>
        /// 撤销申请
        /// </summary>
        /// <param name="apply_no"></param>
        /// <returns></returns>
        public async Task<int> RevokeLeave(ApprovalLeaveModel apply_no)
        {
            if (string.IsNullOrEmpty(apply_no.apply_no))
            {
                throw new MessageException("请选择请假单!");
            }

            var userinfo = new Tools.IdentityModels.GetUser().userInfo;

            //查询次单是否可撤销
            var leavemes = await Db.Queryable<oa_leave>()
                                 .Where(o => o.leave_no == apply_no.apply_no)
                                 .WithCache()
                                 .FirstAsync();

            if (leavemes != null && leavemes.state != 26)
            {
                throw new MessageException("非待审核状态的单子无法撤销!");
            }

            redisCache.RemoveAll<ApprovalModel>();

            return await Db.Updateable<oa_leave>()
                           .SetColumns(s => s.state == 35)
                           .Where(s => s.leave_no == apply_no.apply_no && s.org_id == userinfo.org_id)
                           .RemoveDataCache()
                           .EnableDiffLogEvent()
                           .ExecuteCommandAsync();

        }

        /// <summary>
        /// 标为已读
        /// </summary>
        /// <param name="userinfo"></param>
        /// <param name="no"></param>
        /// <param name="type_id"></param>
        /// <param name="property_id"></param>
        private void UpdateMethod(Tools.IdentityModels.GetUser.UserInfo userinfo, string no, int type_id, int property_id)
        {
            var noticeIs = Db.Queryable<oa_notice, oa_notice_employee>((oaa, obb) => new object[] { JoinType.Left, oaa.id == obb.id })
              .Where((oaa, obb) => oaa.org_id == userinfo.org_id && oaa.notice_type_id == type_id && oaa.notice_property_id == property_id && oaa.relation_no == no && obb.employee_id == userinfo.id)
              .WithCache()
              .First();
            if (noticeIs != null && noticeIs?.id > 0)
            {
                Db.Updateable<oa_notice_employee>()
                .SetColumns(s => s.read_state == 2)
                .Where(s => s.id == noticeIs.id && s.employee_id == userinfo.id)
                .EnableDiffLogEvent()
                .RemoveDataCache()
                .ExecuteCommand();

            }
        }

        /// <summary>
        /// 待审核信息查询
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userinfo"></param>
        /// <param name="processmodel"></param>
        /// <param name="currentlevel"></param>
        /// <param name="process_currentMes"></param>
        /// <param name="newroleList"></param>
        /// <param name="newVerify"></param>
        /// <param name="newVerifyDetail"></param>
        /// <param name="processleave"></param>
        /// <param name="processMes"></param>
        /// <returns></returns>
        private NewProcessModel NewProcess(ApprovalLeaveModel entity, Tools.IdentityModels.GetUser.UserInfo userinfo, CommonModel processmodel, short currentlevel, process_currentMesModel process_currentMes, List<int> newroleList, r_verify newVerify, r_verify_detials newVerifyDetail, ref p_process_detials processMes)
        {
            var level = currentlevel;
            //查询下一审核者
            if (processmodel.level < processmodel.total_level)
            {
                //下一等级
                level = Convert.ToInt16(currentlevel + 1);
                //当前下一审核者角色
                var newprocessMes = Db.Queryable<p_process_detials>()
                                           .Where(p => p.id == processmodel.org_process_id && p.level > currentlevel)
                                           .WithCache()
                                           .ToList();

                foreach (var item in newprocessMes)
                {
                    if (item.is_org == false)
                    {
                        processMes = newprocessMes.Where(s => s.id == item.id && s.level == level && newroleList.Contains(s.role_id)).FirstOrDefault();
                        if (processMes != null)
                        {
                            break;
                        }
                        else
                        {
                            //添加审核记录                               
                            SetVerify(entity, process_currentMes, userinfo, item.level, processmodel, 2, processMes);

                            if (level >= processmodel.total_level)
                            {
                                entity.approval_state = 34;
                            }
                            else
                            {
                                level += 1;
                                currentlevel += 1;
                                continue;

                            }

                        }

                    }
                    else if (item.is_org == true)
                    {
                        processMes = newprocessMes.Where(s => s.id == item.id && s.level == level).FirstOrDefault();
                    }

                }
            }
            else if (processmodel.level == processmodel.total_level || (processMes == null && level == processmodel.total_level))
            {
                entity.approval_state = 34;
            }


            return new NewProcessModel { p_Process_Detials = processMes, processleave = Convert.ToInt16(level) };
        }

        /// <summary>
        /// 审核记录
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="process_currentMes"></param>
        /// <param name="userinfo"></param>
        /// <param name="currentlevel"></param>
        /// <param name="model"></param>
        /// <param name="isJump"></param>
        /// <param name="processDetail"></param>
        public void SetVerify(ApprovalLeaveModel entity, process_currentMesModel process_currentMes, Tools.IdentityModels.GetUser.UserInfo userinfo, int currentlevel, CommonModel model, int isJump, p_process_detials processDetail)
        {
            r_verify newVerify = new r_verify();
            r_verify_detials newVerifyDetail = new r_verify_detials();

            //查询审核表有没有此审核单
            var verifyMes = Db.Queryable<r_verify>()
                            .Where(rv => rv.realted_no == entity.apply_no)
                            .WithCache()
                            .First();
            var plevel = isJump == 2 ? model.level : currentlevel;
            var state = Convert.ToInt16(entity.approval_state == 28 ? 28 : (entity.approval_state != 28 && processDetail?.id <= 0 && plevel >= model.total_level) ? 34 : 33);
            if (verifyMes == null)
            {
                //添加审核表
                newVerify.realted_no = entity.apply_no;
                newVerify.total_level = model.total_level;
                newVerify.level = Convert.ToInt16(model.level);
                newVerify.state = state;
                newVerify.process_type_id = model.process_type_id;
                newVerify.process_type = model.process_type;
                newVerify.org_id = userinfo.org_id;
                newVerify.store_id = model.store_id;
                newVerify.store_name = model.store;
                newVerify.dept_id = process_currentMes.dept_id;
                newVerify.dept_name = process_currentMes.dept_name;
                newVerify.process_id = process_currentMes.id;

                Db.Insertable<r_verify>(newVerify).ExecuteCommand();
                redisCache.RemoveAll<r_verify>();
            }
            else
            {
                var r_level = Convert.ToInt16(entity.approval_state == 28 ? currentlevel : model.level);

                Db.Updateable<r_verify>()
                  .SetColumns(rr => new r_verify { level = r_level, state = state })
                  .Where(rr => rr.realted_no == entity.apply_no)
                  .RemoveDataCache()
                  .EnableDiffLogEvent()
                  .ExecuteCommand();
            }



            //添加审核表明细
            newVerifyDetail.realted_no = entity.apply_no;
            newVerifyDetail.process_id = process_currentMes.id;
            newVerifyDetail.process_name = process_currentMes.name;
            newVerifyDetail.level_type_id = Convert.ToInt16(process_currentMes.leave_type_id);
            newVerifyDetail.level_type = process_currentMes.leave_type;
            newVerifyDetail.process_total_level = model.total_level;
            newVerifyDetail.process_level = Convert.ToInt16(currentlevel);
            newVerifyDetail.state = Convert.ToInt16(entity.approval_state == 28 ? 28 : 29);
            newVerifyDetail.verifier_id = isJump == 2 ? 0 : userinfo.id;
            newVerifyDetail.verifier = isJump == 2 ? " " : userinfo.name;
            newVerifyDetail.verify_time = DateTime.Now;
            newVerifyDetail.verify_remark = isJump == 2 ? " 跳过审核" : entity.verify_remark;
            newVerifyDetail.role_id = isJump == 2 ? 0 : process_currentMes.role_id;
            newVerifyDetail.role_name = isJump == 2 ? " " : process_currentMes.role_name;
            newVerifyDetail.dept_id = process_currentMes.dept_id;
            newVerifyDetail.dept_name = process_currentMes.dept_name;
            newVerifyDetail.employee_id = model.applicant_id;
            newVerifyDetail.employee = model.applicant;

            Db.Insertable<r_verify_detials>(newVerifyDetail).ExecuteCommand();
            redisCache.RemoveAll<r_verify_detials>();

        }

        /// <summary>
        /// 查询门店下拥有的角色
        /// </summary>
        /// <param name="userinfo"></param>
        /// <param name="store_id"></param>
        /// <param name="newroleList"></param>
        /// <returns></returns>
        private List<int> GetNewRole(Tools.IdentityModels.GetUser.UserInfo userinfo, int store_id, List<int> newroleList)
        {
            if (store_id > 0)
            {
                newroleList = Db.Queryable<p_role>()
                                 .Where(s => s.org_id == userinfo.org_id && s.store_id == store_id && s.disabled_code == 1)
                                 .Select(s => s.link_id.Value)
                                 .WithCache()
                                 .ToList();
            }
            else
            {
                newroleList = Db.Queryable<p_role>()
                                .Where(s => s.org_id == userinfo.org_id && s.store_id == store_id && s.disabled_code == 1)
                                .Select(s => s.id)
                                .WithCache()
                                .ToList();
            }

            return newroleList;
        }

        /// <summary>
        ///  查询当前登录人不是机构人员 则所属部门关联角色
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userinfo"></param>
        /// <param name="role_id"></param>
        /// <param name="await_verifier_id"></param>
        /// <param name="is_org"></param>
        /// <param name="roleList"></param>
        /// <returns></returns>
        private List<int> GetRole(ApprovalLeaveModel entity, Tools.IdentityModels.GetUser.UserInfo userinfo, int role_id, int await_verifier_id, bool is_org, List<int> roleList)
        {
            if (entity.store_id > 0 || is_org == false)
            {
                roleList = Db.Queryable<p_employee_role, p_dept, p_role>((er, d, r) => new object[] { JoinType.Left, er.dept_id == d.id, JoinType.Left, er.role_id == r.id }).Where((er, d, r) => er.org_id == userinfo.org_id && er.employee_id == userinfo.id && r.link_id > 0).Select((er, d, r) => r.link_id.Value).WithCache().ToList();

                if (await_verifier_id != userinfo.id && !roleList.Contains(role_id) || entity.store_id <= 0)
                {
                    throw new MessageException("没有审核条件!");
                }
            }
            else
            {
                roleList = Db.Queryable<p_employee_role, p_dept, p_role>((er, d, r) => new object[] { JoinType.Left, er.dept_id == d.id, JoinType.Left, er.role_id == r.id }).Where((er, d, r) => er.org_id == userinfo.org_id && er.employee_id == userinfo.id && r.id != 0).Select((er, d, r) => r.id).WithCache().ToList();

                if (await_verifier_id != userinfo.id && !roleList.Contains(role_id))
                {
                    throw new MessageException("没有审核条件!");
                }
            }

            return roleList;
        }

        #endregion

        #region 各种审核
        /// <summary>
        /// 请假审核
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> ApprovaLeave(ApprovalLeaveModel entity)
        {
            if (string.IsNullOrEmpty(entity.apply_no))
            {
                throw new MessageException("请选择请假单!");
            }

            //通知
            var notice = new Business.NoticeService();
            var noticeList = new List<AddNoticeModel>();
            var notice_content = "";
            var employeeSocket = new List<WebSocketModel>();

            var userinfo = new Tools.IdentityModels.GetUser().userInfo;
            var result = Db.Ado.UseTran(() =>
            {
                //查询单子可不可以审核，有没有权限
                var oaList = Db.Queryable<oa_leave>()
                                  .Where(o => o.leave_no == entity.apply_no && o.org_id == userinfo.org_id && (o.state == 26 || o.state == 36))
                                  .WithCache()
                                  .First();


                if (oaList == null)
                {
                    throw new MessageException("未查询到符合条件的请假单!");
                }

                //查询申请人电话
                var apply_phone = Db.Queryable<p_employee>()
                                    .Where(ss => ss.id == oaList.applicant_id)
                                    .Select(ss => ss.phone_no)
                                    .WithCache()
                                    .First();

                //查询当前登录人是否机构人员 则所属部门关联角色
                var roleList = new List<int>();
                int role_id = oaList.role_id;
                int await_verifier_ids = oaList.await_verifier_id.Value;
                bool is_org = oaList.is_org.Value;
                roleList = GetRole(entity, userinfo, role_id, await_verifier_ids, is_org, roleList);

                //查询当前审核人信息
                var currentlevel = oaList.level;
                var process_currentMes = Db.Queryable<p_process_detials, p_process>((p, pr) => new object[] { JoinType.Left, p.id == pr.id })
                                      .Where((p, pr) => p.id == oaList.org_process_id && p.level == currentlevel)
                                      .Select((p, pr) => new process_currentMesModel { dept_id = p.dept_id, role_id = p.role_id, employee_id = p.employee_id.Value, dept_name = p.dept_name, role_name = p.role_name, id = p.id, name = pr.name, leave_type_id = pr.leave_type_id, leave_type = pr.leave_type })
                                      .WithCache()
                                      .First();

                if (process_currentMes == null)
                {
                    throw new MessageException("未获取到当前审核流程信息");
                }

                var storeroleList = new List<int>();
                //查询当前门店所有角色
                int store_id = entity.store_id;
                storeroleList = GetNewRole(userinfo, store_id, storeroleList);


                var newVerify = new r_verify();//审核表
                var newVerifyDetail = new r_verify_detials();//审核详情表
                var new_leave = new oa_leave();//请假表
                var await_verifier_id = 0;//待审核人id
                var await_role_id = 0;//待审核角色id
                var await_verifier = " ";//待审核人
                var await_role = " ";//待审核角色

                //下一流程详细
                var processMes = new p_process_detials();
                //下一流程名所需
                var process_need = new object();

                //查询待审核数据所需字段
                CommonModel processmodel = new CommonModel();
                processmodel.total_level = oaList.total_level;
                processmodel.level = oaList.level;
                processmodel.store_id = oaList.store_id;
                processmodel.store = oaList.store;
                processmodel.applicant = oaList.applicant;
                processmodel.applicant_id = oaList.applicant_id;
                processmodel.org_process_id = oaList.org_process_id.Value;
                processmodel.process_type_id = 4;
                processmodel.process_type = "请假";

                //审核流程
                if (processmodel.org_process_id <= 0 || processmodel.level > processmodel.total_level)
                {
                    throw new MessageException("此审核流程数据不正确!");
                }

                //通过,查询待审核人信息
                var newprocess_detail = new NewProcessModel();
                if (entity.approval_state == 34)
                {
                    newprocess_detail = NewProcess(entity, userinfo, processmodel, currentlevel, process_currentMes, storeroleList, newVerify, newVerifyDetail, ref processMes);

                    processMes = newprocess_detail?.p_Process_Detials;
                    new_leave.level = newprocess_detail.processleave;
                }
                //部分审核完成
                if (processMes?.id > 0)
                {
                    //获取流程名
                    process_need = Db.Queryable<p_process>()
                                              .Where(pr => pr.id == processMes.id)
                                              .Select(pr => new { pr.name, pr.leave_type_id, pr.leave_type })
                                              .WithCache()
                                              .First();


                    new_leave.await_verifier_id = processMes.employee_id > 0 ? processMes.employee_id : 0;
                    new_leave.await_verifier = processMes.employee_id > 0 ? processMes.employee : " ";
                    new_leave.role_id = processMes.employee_id == 0 ? processMes.role_id : 0;
                    new_leave.role_name = processMes.employee_id == 0 ? processMes.role_name : " ";
                    entity.approval_state = 36;
                    new_leave.is_org = processMes.is_org;

                    //查询此角色下面的人员     
                    var rolenotice = Db.Queryable<p_employee_role, p_role, p_employee>((a, b, e) => new object[] { JoinType.Left, a.role_id == b.id, JoinType.Left, e.id == a.employee_id })
                                           .Where((a, b, e) => b.disabled_code == 1)
                                           .WhereIF(new_leave.is_org == false, (a, b, e) => b.link_id == new_leave.role_id && a.store_id > 0)
                                           .WhereIF(new_leave.is_org == true, (a, b, e) => b.id == new_leave.role_id && a.store_id == 0)
                                           .Select((a, b, e) => new { e.id, e.name })
                                           .WithCache()
                                           .ToList();

                    if (rolenotice.Count > 0)
                    {
                        //请假通知
                        notice_content = $"{{\"name\":\"{userinfo.name}\",\"leave_type\":\"{oaList.leave_type}\",\"date\":\"{oaList.apply_start_time.ToString("yyyy-MM-dd")}至{oaList.apply_end_time.ToString("yyyy-MM-dd")}\",\"duration\":\"{oaList.duration}\",\"leave_cause\":\"{oaList.leave_cause}\",\"no\":\"{oaList.leave_no}\",\"msg\":\" 请假申请已提交\"}}";
                        var archives = new c_archives();
                        archives.id = oaList.applicant_id;
                        archives.name = oaList.applicant;
                        archives.phone = apply_phone;
                        var employeenotice = new List<employeeMes>();

                        rolenotice.ForEach(r =>
                        {
                            var con = $"{{\"name\":\"{userinfo.name}\",\"msg\":\"提交了请假申请，请处理！\"}}";
                            employeeSocket.Add(new WebSocketModel { userId = r.id, content = con });

                            employeenotice.Add(new employeeMes { employee_id = r.id, employee = r.name });

                        });
                        notice.NewMethod(oaList.leave_no, archives, entity.store_id, notice, noticeList, 3, 1, notice_content, userinfo.name, employeenotice);


                    }

                }
                else
                {
                    if (entity.approval_state == 34)
                    {
                        var con = $"{{\"name\":\"{oaList.applicant}\",\"msg\":\"提交了请假申请已通过，请查看！\"}}";
                        employeeSocket.Add(new WebSocketModel { userId = oaList.applicant_id, content = con });
                    }
                    else
                    {
                        var con = $"{{\"name\":\"{oaList.applicant}\",\"msg\":\"提交了请假申请被拒绝，请查看！\"}}";
                        employeeSocket.Add(new WebSocketModel { userId = oaList.applicant_id, content = con });


                    }



                    //  new_leave.level = Convert.ToInt16(currentlevel);
                    new_leave.is_org = false;

                    //请假审批完成通知
                    notice_content = $"{{\"content\":\"{entity.verify_remark}\",\"name\":\"{oaList.applicant}\",\"msg\":\" 请假审批结果，请查收！\",\"state\":\"{entity.approval_state}\",\"no\":\"{oaList.leave_no}\",\"duration\":\"{oaList.duration}\",\"leave_type\":\"{oaList.leave_type}\",\"date\":\"{oaList.apply_start_time.ToString("yyyy-MM-dd")}至{oaList.apply_end_time.ToString("yyyy-MM-dd")}\",\"leave_cause\":\"{oaList.leave_cause}\"}}";
                    var archives = new c_archives();
                    archives.id = oaList.applicant_id;
                    archives.name = oaList.applicant;
                    archives.phone = apply_phone;
                    var employeeNotice = new List<employeeMes>();
                    employeeNotice.Add(new employeeMes { employee = oaList.applicant, employee_id = oaList.applicant_id });

                    notice.NewMethod(entity.apply_no, archives, entity.store_id, notice, noticeList, 3, 2, notice_content, oaList.applicant, employeeNotice);
                }

                new_leave.verifier_id = userinfo.id;
                new_leave.verifier = userinfo.name;
                new_leave.verify_time = DateTime.Now;
                await_verifier_id = entity.approval_state == 36 ? new_leave.await_verifier_id.Value : 0;
                await_verifier = entity.approval_state == 36 ? new_leave.await_verifier : " ";
                await_role_id = entity.approval_state == 36 ? new_leave.role_id : 0;
                await_role = entity.approval_state == 36 ? new_leave.role_name : " ";

                //将此单标为已读
                UpdateMethod(userinfo, oaList.leave_no, 3, 1);

                //修改请假表
                Db.Updateable(new_leave)
                  .SetColumns(s => new oa_leave { role_id = await_role_id, role_name = await_role, await_verifier_id = await_verifier_id, await_verifier = await_verifier, level = new_leave.level, verifier = new_leave.verifier, verifier_id = new_leave.verifier_id, verify_time = new_leave.verify_time, state = entity.approval_state, is_org = new_leave.is_org })
                  .Where(s => s.leave_no == entity.apply_no)
                  .RemoveDataCache()
                  .EnableDiffLogEvent()
                  .ExecuteCommand();

                //审核记录
                CommonModel model = new CommonModel();
                model.total_level = oaList.total_level;
                model.level = newprocess_detail.processleave;
                model.store_id = oaList.store_id;
                model.store = oaList.store;
                model.applicant_id = oaList.applicant_id;
                model.applicant = oaList.applicant;
                model.process_type_id = 4;
                model.process_type = "请假";


                SetVerify(entity, process_currentMes, userinfo, currentlevel, model, 3, processMes);
                if (noticeList.Count > 0)
                {
                    //新增
                    notice.AddNotice(noticeList);
                    ChatWebSocketMiddleware.SendListAsync(employeeSocket);
                }

            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;



        }

        /// <summary>
        /// 采购审核
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> ApprovaBuy(ApprovalLeaveModel entity)
        {
            if (string.IsNullOrEmpty(entity.apply_no))
            {
                throw new MessageException("请选择采购单!");
            }

            //通知
            var notice = new Business.NoticeService();
            var noticeList = new List<AddNoticeModel>();
            var notice_content = "";
            var employeeSocket = new List<WebSocketModel>();

            var userinfo = new Tools.IdentityModels.GetUser().userInfo;
            var result = Db.Ado.UseTran(() =>
            {
                //查询单子可不可以审核，有没有权限
                var oaList = Db.Queryable<bus_buy_apply>()
                               .Where(o => o.apply_no == entity.apply_no && o.org_id == userinfo.org_id && (o.state == 26 || o.state == 36))
                               .WithCache()
                               .First();
                if (oaList == null)
                {
                    throw new MessageException("未查询到符合条件的采购单!");
                }
                //查询申请人电话
                var apply_phone = Db.Queryable<p_employee>()
                                    .Where(ss => ss.id == oaList.applicant_id)
                                    .Select(ss => ss.phone_no)
                                    .WithCache()
                                    .First();

                //查询当前登录人不是机构人员 则所属部门关联角色
                var roleList = new List<int>();
                int role_id = oaList.await_verifier_id.Value;
                int await_verifier_ids = oaList.await_verifier_id.Value;
                bool is_org = oaList.is_org.Value;
                roleList = GetRole(entity, userinfo, role_id, await_verifier_ids, is_org, roleList);


                //查询当前审核人信息
                var currentlevel = oaList.level;
                var process_currentMes = Db.Queryable<p_process_detials, p_process>((p, pr) => new object[] { JoinType.Left, p.id == pr.id })
                                      .Where((p, pr) => p.id == oaList.org_process_id && p.level == currentlevel)
                                      .Select((p, pr) => new process_currentMesModel { dept_id = p.dept_id, role_id = p.role_id, employee_id = p.employee_id.Value, dept_name = p.dept_name, role_name = p.role_name, id = p.id, name = pr.name, leave_type_id = pr.leave_type_id, leave_type = pr.leave_type })
                                      .WithCache()
                                      .First();

                if (process_currentMes == null)
                {
                    throw new MessageException("未获取到当前审核流程信息");
                }

                var storeroleList = new List<int>();
                //查询当前门店所有角色
                int store_id = entity.store_id;
                storeroleList = GetNewRole(userinfo, store_id, storeroleList);



                var newVerify = new r_verify();//审核表
                var newVerifyDetail = new r_verify_detials();//审核详情表
                var new_leave = new bus_buy_apply();//采购表
                var await_verifier_id = 0;//待审核人id
                var await_verifier = " ";//待审核人

                //下一流程
                var processMes = new p_process_detials();
                //下一流程详情
                var process_need = new object();

                //查询待审核数据所需字段
                CommonModel processmodel = new CommonModel();
                processmodel.total_level = oaList.total_level;
                processmodel.level = oaList.level;
                processmodel.store_id = oaList.store_id.Value;
                processmodel.store = oaList.store;
                processmodel.applicant = oaList.applicant;
                processmodel.applicant_id = oaList.applicant_id.Value;
                processmodel.org_process_id = oaList.org_process_id.Value;
                processmodel.process_type_id = 1;
                processmodel.process_type = "采购";

                //审核流程
                if (processmodel.org_process_id <= 0 || processmodel.level > processmodel.total_level)
                {
                    throw new MessageException("此审核流程数据不正确!");
                }

                //通过,查询待审核人信息
                var newprocess_detail = new NewProcessModel();
                if (entity.approval_state == 34)
                {

                    newprocess_detail = NewProcess(entity, userinfo, processmodel, currentlevel, process_currentMes, storeroleList, newVerify, newVerifyDetail, ref processMes);

                    processMes = newprocess_detail?.p_Process_Detials;
                    new_leave.level = newprocess_detail.processleave;
                }
                //部分审核完成
                if (processMes?.id > 0)
                {
                    //获取流程名
                    process_need = Db.Queryable<p_process>()
                                              .Where(pr => pr.id == processMes.id)
                                              .Select(pr => new { pr.name, pr.leave_type_id, pr.leave_type })
                                              .WithCache()
                                              .First();

                    new_leave.await_verifier_id = processMes.role_id > 0 ? processMes.role_id : 0;
                    new_leave.await_verifier = processMes.role_id > 0 ? processMes.role_name : " ";
                    entity.approval_state = 36;
                    new_leave.is_org = processMes.is_org;

                    //查询此角色下面的人员     

                    var rolenotice = Db.Queryable<p_employee_role, p_role, p_employee>((a, b, e) => new object[] { JoinType.Left, a.role_id == b.id, JoinType.Left, e.id == a.employee_id })
                                           .Where((a, b, e) => b.disabled_code == 1)
                                           .WhereIF(new_leave.is_org == false, (a, b, e) => b.link_id == new_leave.await_verifier_id && a.store_id > 0)
                                           .WhereIF(new_leave.is_org == true, (a, b, e) => b.id == new_leave.await_verifier_id && a.store_id == 0)
                                           .Select((a, b, e) => new { e.id, e.name })
                                           .WithCache()
                                           .ToList();

                    if (rolenotice.Count > 0)
                    {
                        //采购通知
                        notice_content = $"{{\"name\":\"{oaList.applicant}\",\"no\":\"{oaList.apply_no}\",\"date\":\"{oaList.apply_time.ToString()}\",\"total_price\":\"{oaList.total_price}\",\"remark\":\"{oaList.remark}\",\"msg\":\" 采购待审批\"}}";
                        var archives = new c_archives();
                        archives.id = oaList.applicant_id.Value;
                        archives.name = oaList.applicant;
                        archives.phone = apply_phone;
                        var employeenotice = new List<employeeMes>();


                        rolenotice.ForEach(r =>
                        {
                            var con = $"{{\"name\":\"{oaList.applicant}\",\"msg\":\"提交了采购申请，请处理！\"}}";
                            employeeSocket.Add(new WebSocketModel { userId = r.id, content = con });

                            employeenotice.Add(new employeeMes { employee_id = r.id, employee = r.name });
                        });
                        notice.NewMethod(oaList.apply_no, archives, oaList.store_id.Value, notice, noticeList, 3, 3, notice_content, oaList.applicant, employeenotice);



                    }



                }
                else
                {
                    if (entity.approval_state == 34)
                    {
                        var con = $"{{\"name\":\"{oaList.applicant}\",\"msg\":\"提交了采购申请已通过，请查看！\"}}";
                        employeeSocket.Add(new WebSocketModel { userId = oaList.applicant_id.Value, content = con });
                    }
                    else
                    {
                        var con = $"{{\"name\":\"{oaList.applicant}\",\"msg\":\"提交了采购申请被拒绝，请查看！\"}}";
                        employeeSocket.Add(new WebSocketModel { userId = oaList.applicant_id.Value, content = con });

                    }

                    new_leave.is_org = false;

                    //采购审批完成通知
                    notice_content = $"{{\"content\":\"{entity.verify_remark}\",\"name\":\"{oaList.applicant}\",\"msg\":\" 采购审批结果，请查收！\",\"state\":\"{entity.approval_state}\",\"no\":\"{oaList.apply_no}\",\"total_price\":\"{oaList.total_price}\",\"date\":\"{oaList.apply_time.ToString()}\",\"remark\":\"{oaList.remark}\"}}";
                    var archives = new c_archives();
                    archives.id = oaList.applicant_id.Value;
                    archives.name = oaList.applicant;
                    archives.phone = apply_phone;
                    var employeenotice = new List<employeeMes>();
                    employeenotice.Add(new employeeMes { employee_id = oaList.applicant_id.Value, employee = oaList.applicant });

                    notice.NewMethod(entity.apply_no, archives, entity.store_id, notice, noticeList, 3, 4, notice_content, oaList.applicant, employeenotice);
                }

                new_leave.verifier_id = userinfo.id;
                new_leave.verifier = userinfo.name;
                new_leave.verify_time = DateTime.Now;
                await_verifier_id = entity.approval_state == 36 ? new_leave.await_verifier_id.Value : 0;
                await_verifier = entity.approval_state == 36 ? new_leave.await_verifier : " ";

                //将此单标为已读
                UpdateMethod(userinfo, oaList.apply_no, 3, 3);


                //修改采购表
                Db.Updateable(new_leave)
                  .SetColumns(s => new bus_buy_apply { await_verifier_id = await_verifier_id, await_verifier = await_verifier, level = new_leave.level, verifier = new_leave.verifier, verifier_id = new_leave.verifier_id, verify_time = new_leave.verify_time, state = entity.approval_state, is_org = new_leave.is_org, verify_remark = entity.verify_remark })
                  .Where(s => s.apply_no == entity.apply_no)
                  .RemoveDataCache()
                  .EnableDiffLogEvent()
                  .ExecuteCommand();

                //审核记录
                CommonModel model = new CommonModel();
                model.total_level = oaList.total_level;
                model.level = newprocess_detail.processleave;
                model.store_id = oaList.store_id.Value;
                model.store = oaList.store;
                model.applicant_id = oaList.applicant_id.Value;
                model.applicant = oaList.applicant;
                model.process_type_id = 1;
                model.process_type = "采购";


                SetVerify(entity, process_currentMes, userinfo, currentlevel, model, 3, processMes);
                if (noticeList.Count > 0)
                {
                    //新增
                    notice.AddNotice(noticeList);
                    ChatWebSocketMiddleware.SendListAsync(employeeSocket);
                }

            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;



        }

        /// <summary>
        /// 调拨审核
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> ApprovaTransfer(ApprovalLeaveModel entity)
        {
            if (string.IsNullOrEmpty(entity.apply_no))
            {
                throw new MessageException("请选择调拨单!");
            }

            //通知
            var notice = new Business.NoticeService();
            var noticeList = new List<AddNoticeModel>();
            var notice_content = "";
            var employeeSocket = new List<WebSocketModel>();

            var userinfo = new Tools.IdentityModels.GetUser().userInfo;
            var result = Db.Ado.UseTran(() =>
            {
                //查询单子可不可以审核，有没有权限
                var oaList = Db.Queryable<bus_transfer_bill>()
                               .Where(o => o.bill_no == entity.apply_no && o.org_id == userinfo.org_id && (o.state == 26 || o.state == 36))
                               .WithCache()
                               .First();
                if (oaList == null)
                {
                    throw new MessageException("未查询到符合条件的调拨单!");
                }
                //查询申请人电话
                var apply_phone = Db.Queryable<p_employee>()
                                    .Where(ss => ss.id == oaList.creater_id)
                                    .Select(ss => ss.phone_no)
                                    .WithCache()
                                    .First();

                //查询当前登录人不是机构人员 则所属部门关联角色
                var roleList = new List<int>();
                int role_id = oaList.await_verifier_id.Value;
                int await_verifier_ids = oaList.await_verifier_id.Value;
                bool is_org = oaList.is_org.Value;
                roleList = GetRole(entity, userinfo, role_id, await_verifier_ids, is_org, roleList);


                //查询当前审核人信息
                var currentlevel = oaList.level;
                var process_currentMes = Db.Queryable<p_process_detials, p_process>((p, pr) => new object[] { JoinType.Left, p.id == pr.id })
                                      .Where((p, pr) => p.id == oaList.process_id && p.level == currentlevel)
                                      .Select((p, pr) => new process_currentMesModel { dept_id = p.dept_id, role_id = p.role_id, employee_id = p.employee_id.Value, dept_name = p.dept_name, role_name = p.role_name, id = p.id, name = pr.name, leave_type_id = pr.leave_type_id, leave_type = pr.leave_type })
                                      .WithCache()
                                      .First();

                if (process_currentMes == null)
                {
                    throw new MessageException("未获取到当前审核流程信息");
                }

                var storeroleList = new List<int>();
                //查询当前门店所有角色
                int store_id = entity.store_id;
                storeroleList = GetNewRole(userinfo, store_id, storeroleList);



                var newVerify = new r_verify();//审核表
                var newVerifyDetail = new r_verify_detials();//审核详情表
                var new_leave = new bus_transfer_bill();//调拨表
                var await_verifier_id = 0;//待审核人id
                var await_verifier = " ";//待审核人

                //下一流程
                var processMes = new p_process_detials();
                //下一流程详情
                var process_need = new object();

                //查询待审核数据所需字段
                CommonModel processmodel = new CommonModel();
                processmodel.total_level = oaList.total_level;
                processmodel.level = oaList.level;
                processmodel.store_id = oaList.in_store_id;
                processmodel.store = oaList.in_store_name;
                processmodel.applicant = oaList.creater;
                processmodel.applicant_id = oaList.creater_id;
                processmodel.org_process_id = oaList.process_id.Value;
                processmodel.process_type_id = 2;
                processmodel.process_type = "调拨";

                //审核流程
                if (processmodel.org_process_id <= 0 || processmodel.level > processmodel.total_level)
                {
                    throw new MessageException("此审核流程数据不正确!");
                }

                //通过,查询待审核人信息
                var newprocess_detail = new NewProcessModel();
                if (entity.approval_state == 34)
                {

                    newprocess_detail = NewProcess(entity, userinfo, processmodel, currentlevel, process_currentMes, storeroleList, newVerify, newVerifyDetail, ref processMes);

                    processMes = newprocess_detail?.p_Process_Detials;
                    new_leave.level = newprocess_detail.processleave;
                }
                //部分审核完成
                if (processMes?.id > 0)
                {
                    //获取流程名
                    process_need = Db.Queryable<p_process>()
                                              .Where(pr => pr.id == processMes.id)
                                              .Select(pr => new { pr.name, pr.leave_type_id, pr.leave_type })
                                              .WithCache()
                                              .First();

                    new_leave.await_verifier_id = processMes.role_id > 0 ? processMes.role_id : 0;
                    new_leave.await_verifier = processMes.role_id > 0 ? processMes.role_name : " ";
                    entity.approval_state = 36;
                    new_leave.is_org = processMes.is_org;

                    //查询此角色下面的人员     

                    var rolenotice = Db.Queryable<p_employee_role, p_role, p_employee>((a, b, e) => new object[] { JoinType.Left, a.role_id == b.id, JoinType.Left, e.id == a.employee_id })
                                           .Where((a, b, e) => b.disabled_code == 1)
                                           .WhereIF(new_leave.is_org == false, (a, b, e) => b.link_id == new_leave.await_verifier_id && a.store_id > 0)
                                           .WhereIF(new_leave.is_org == true, (a, b, e) => b.id == new_leave.await_verifier_id && a.store_id == 0)
                                           .Select((a, b, e) => new { e.id, e.name })
                                           .WithCache()
                                           .ToList();

                    if (rolenotice.Count > 0)
                    {
                        //调拨通知
                        notice_content = $"{{\"name\":\"{oaList.creater}\",\"no\":\"{oaList.bill_no}\",\"date\":\"{oaList.create_time.ToString()}\",\"remark\":\"{oaList.remark}\",\"msg\":\" 调拨待审批\"}}";
                        var archives = new c_archives();
                        archives.id = oaList.creater_id;
                        archives.name = oaList.creater;
                        archives.phone = apply_phone;
                        var employeenotice = new List<employeeMes>();


                        rolenotice.ForEach(r =>
                        {
                            var con = $"{{\"name\":\"{oaList.creater}\",\"msg\":\"提交了调拨申请，请处理！\"}}";
                            employeeSocket.Add(new WebSocketModel { userId = r.id, content = con });

                            employeenotice.Add(new employeeMes { employee_id = r.id, employee = r.name });
                        });
                        notice.NewMethod(oaList.apply_no, archives, oaList.in_store_id, notice, noticeList, 3, 5, notice_content, oaList.creater, employeenotice);

                    }

                }
                else
                {
                    if (entity.approval_state == 34)
                    {
                        var con = $"{{\"name\":\"{oaList.creater}\",\"msg\":\"提交了调拨申请已通过，请查看！\"}}";
                        employeeSocket.Add(new WebSocketModel { userId = oaList.creater_id, content = con });
                    }
                    else
                    {
                        //拒绝的话归还可用数量,生成负单
                        BackTransfer(userinfo, oaList);

                        var con = $"{{\"name\":\"{oaList.creater}\",\"msg\":\"提交了调拨申请被拒绝，请查看！\"}}";
                        employeeSocket.Add(new WebSocketModel { userId = oaList.creater_id, content = con });

                    }

                    new_leave.is_org = false;

                    //调拨审批完成通知
                    notice_content = $"{{\"content\":\"{entity.verify_remark}\",\"name\":\"{oaList.creater}\",\"msg\":\" 调拨审批结果，请查收！\",\"state\":\"{entity.approval_state}\",\"no\":\"{oaList.apply_no}\",\"date\":\"{oaList.create_time.ToString()}\",\"remark\":\"{oaList.remark}\"}}";
                    var archives = new c_archives();
                    archives.id = oaList.creater_id;
                    archives.name = oaList.creater;
                    archives.phone = apply_phone;
                    var employeenotice = new List<employeeMes>();
                    employeenotice.Add(new employeeMes { employee_id = oaList.creater_id, employee = oaList.creater });

                    notice.NewMethod(entity.apply_no, archives, entity.store_id, notice, noticeList, 3, 6, notice_content, oaList.creater, employeenotice);
                }

                new_leave.verifier_id = userinfo.id;
                new_leave.verifier = userinfo.name;
                new_leave.verify_time = DateTime.Now;
                await_verifier_id = entity.approval_state == 36 ? new_leave.await_verifier_id.Value : 0;
                await_verifier = entity.approval_state == 36 ? new_leave.await_verifier : " ";

                //将此单标为已读
                UpdateMethod(userinfo, oaList.apply_no, 3, 5);


                //修改调拨表
                Db.Updateable(new_leave)
                  .SetColumns(s => new bus_transfer_bill { await_verifier_id = await_verifier_id, await_verifier = await_verifier, level = new_leave.level, verifier = new_leave.verifier, verifier_id = new_leave.verifier_id, verify_time = new_leave.verify_time, state = entity.approval_state, is_org = new_leave.is_org, verify_remark = entity.verify_remark })
                  .Where(s => s.bill_no == entity.apply_no)
                  .RemoveDataCache()
                  .EnableDiffLogEvent()
                  .ExecuteCommand();

                //审核记录
                CommonModel model = new CommonModel();
                model.total_level = oaList.total_level;
                model.level = newprocess_detail.processleave;
                model.store_id = oaList.out_store_id;
                model.store = oaList.out_store_name;
                model.applicant_id = oaList.creater_id;
                model.applicant = oaList.creater;
                model.process_type_id = 2;
                model.process_type = "调拨";


                SetVerify(entity, process_currentMes, userinfo, currentlevel, model, 3, processMes);
                if (noticeList.Count > 0)
                {
                    //新增
                    notice.AddNotice(noticeList);
                    ChatWebSocketMiddleware.SendListAsync(employeeSocket);
                }

            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;



        }


        private void BackTransfer(Tools.IdentityModels.GetUser.UserInfo userinfo, bus_transfer_bill oaList)
        {
            var bill = Db.Queryable<bus_transfer_bill>().Where(w => w.bill_no == oaList.bill_no).WithCache().First();
            Db.Updateable<bus_transfer_bill>().SetColumns(s => new bus_transfer_bill { state = 7 }).Where(w => w.bill_no == oaList.bill_no).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

            //生成调拨单负单
            bill.state = 7;
            bill.delete_no = bill.bill_no;
            bill.bill_no = "-" + bill.bill_no;
            bill.creater = userinfo.name;
            bill.creater_id = userinfo.id;
            bill.create_time = DateTime.Now;
            Db.Insertable(bill).ExecuteCommand();
            redisCache.RemoveAll<bus_transfer_bill>();

            //查询调拨单明细
            var transfer_Bill_Detials = Db.Queryable<bus_transfer_bill_detials>().Where(w => w.bill_no == oaList.bill_no).WithCache().ToList();

            //生成调拨单明细负单
            var del_transfer_Bill_Detials = transfer_Bill_Detials.Select(s => new bus_transfer_bill_detials { aog_num = Convert.ToInt16(0 - s.aog_num), approval_no = s.approval_no, bill_no = bill.bill_no, buy_multiple = s.buy_multiple, buy_num = 0 - s.buy_num, buy_price = s.buy_price, buy_unit = s.buy_unit, manufactor = s.manufactor, manufactor_id = s.manufactor_id, name = s.name, num = Convert.ToInt16(0 - s.num), price = s.price, remark = s.remark, spec = s.spec, std_item_id = s.std_item_id, type = s.type, type_id = s.type_id, unit = s.unit }).ToList();
            Db.Insertable(del_transfer_Bill_Detials).ExecuteCommand();
            redisCache.RemoveAll<bus_transfer_bill_detials>();

            //生成调拨单明细对应库存明细负单
            var transfer_detials_storage = Db.Queryable<bus_transfer_detials_storage>().Where(w => w.bill_no == oaList.bill_no).WithCache().ToList();

            var del_transfer_detials_storage = transfer_detials_storage.Select(s => new bus_transfer_detials_storage { bill_no = bill.bill_no, expire_date = s.expire_date, id = s.id, manufactor_id = s.manufactor_id, no = s.no, num = Convert.ToInt16(0 - s.num), spec = s.spec, std_item_id = s.std_item_id }).ToList();
            Db.Insertable(del_transfer_detials_storage).ExecuteCommand();
            redisCache.RemoveAll<bus_transfer_detials_storage>();
            var ts = new TransferService();
            ts.CancelDetials(bill, transfer_Bill_Detials, transfer_detials_storage);

            //获取固定资产ID
            var assets = Db.Queryable<bus_transfer_assets>().Where(w => w.bill_no == oaList.bill_no).ToList();
            if (assets != null)
            {
                var assets_ids = assets.Select(s => s.assets_id).ToList();
                //修改固定资产状态
                Db.Updateable<bus_assets>().SetColumns(s => new bus_assets { state = 30 }).Where(w => assets_ids.Contains(w.id)).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                //生成调拨单明细对应固定资产负单
                var del_assets = assets.Select(s => new bus_transfer_assets { assets_id = s.assets_id, bill_no = bill.bill_no, manufactor_id = s.manufactor_id, spec = s.spec, std_item_id = s.std_item_id }).ToList();
                Db.Insertable(del_assets).ExecuteCommand();
                redisCache.RemoveAll<bus_transfer_assets>();
            }
        }

        /// <summary>
        /// 领用审批
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> ApprovaRequisition(ApprovalLeaveModel entity)
        {
            if (string.IsNullOrEmpty(entity.apply_no))
            {
                throw new MessageException("请选择领用单!");
            }

            //通知
            var notice = new Business.NoticeService();
            var noticeList = new List<AddNoticeModel>();
            var notice_content = "";
            var employeeSocket = new List<WebSocketModel>();

            var userinfo = new Tools.IdentityModels.GetUser().userInfo;
            var result = Db.Ado.UseTran(() =>
            {
                //查询单子可不可以审核，有没有权限
                var oaList = Db.Queryable<bus_requisitions_bill>()
                                  .Where(o => o.bill_no == entity.apply_no && o.org_id == userinfo.org_id && (o.state == 26 || o.state == 36))
                                  .WithCache()
                                  .First();
                if (oaList == null)
                {
                    throw new MessageException("未查询到符合条件的调拨单!");
                }
                //查询申请人电话
                var apply_phone = Db.Queryable<p_employee>()
                                    .Where(ss => ss.id == oaList.creater_id)
                                    .Select(ss => ss.phone_no)
                                    .WithCache()
                                    .First();

                //查询当前登录人不是机构人员 则所属部门关联角色
                var roleList = new List<int>();
                int role_id = oaList.await_verifier_id.Value;
                int await_verifier_ids = oaList.await_verifier_id.Value;
                bool is_org = oaList.is_org.Value;
                roleList = GetRole(entity, userinfo, role_id, await_verifier_ids, is_org, roleList);


                //查询当前审核人信息
                var currentlevel = oaList.level;
                var process_currentMes = Db.Queryable<p_process_detials, p_process>((p, pr) => new object[] { JoinType.Left, p.id == pr.id })
                                      .Where((p, pr) => p.id == oaList.process_id && p.level == currentlevel)
                                      .Select((p, pr) => new process_currentMesModel { dept_id = p.dept_id, role_id = p.role_id, employee_id = p.employee_id.Value, dept_name = p.dept_name, role_name = p.role_name, id = p.id, name = pr.name, leave_type_id = pr.leave_type_id, leave_type = pr.leave_type })
                                      .WithCache()
                                      .First();

                if (process_currentMes == null)
                {
                    throw new MessageException("未获取到当前审核流程信息");
                }

                var storeroleList = new List<int>();
                //查询当前门店所有角色
                int store_id = entity.store_id;
                storeroleList = GetNewRole(userinfo, store_id, storeroleList);



                var newVerify = new r_verify();//审核表
                var newVerifyDetail = new r_verify_detials();//审核详情表
                var new_leave = new bus_requisitions_bill();//调拨表
                var await_verifier_id = 0;//待审核人id
                var await_verifier = " ";//待审核人

                //下一流程
                var processMes = new p_process_detials();
                //下一流程详情
                var process_need = new object();

                //查询待审核数据所需字段
                CommonModel processmodel = new CommonModel();
                processmodel.total_level = oaList.total_level;
                processmodel.level = oaList.level;
                processmodel.store_id = oaList.store_id;
                processmodel.store = oaList.store_name;
                processmodel.applicant = oaList.creater;
                processmodel.applicant_id = oaList.creater_id;
                processmodel.org_process_id = oaList.process_id.Value;
                processmodel.process_type_id = 3;
                processmodel.process_type = "领用";

                //审核流程
                if (processmodel.org_process_id <= 0 || processmodel.level > processmodel.total_level)
                {
                    throw new MessageException("此审核流程数据不正确!");
                }

                //通过,查询待审核人信息
                var newprocess_detail = new NewProcessModel();
                if (entity.approval_state == 34)
                {

                    newprocess_detail = NewProcess(entity, userinfo, processmodel, currentlevel, process_currentMes, storeroleList, newVerify, newVerifyDetail, ref processMes);

                    processMes = newprocess_detail?.p_Process_Detials;
                    new_leave.level = newprocess_detail.processleave;
                }
                //部分审核完成
                if (processMes?.id > 0)
                {
                    //获取流程名
                    process_need = Db.Queryable<p_process>()
                                              .Where(pr => pr.id == processMes.id)
                                              .Select(pr => new { pr.name, pr.leave_type_id, pr.leave_type })
                                              .WithCache()
                                              .First();

                    new_leave.await_verifier_id = processMes.role_id > 0 ? processMes.role_id : 0;
                    new_leave.await_verifier = processMes.role_id > 0 ? processMes.role_name : " ";
                    entity.approval_state = 36;
                    new_leave.is_org = processMes.is_org;

                    //查询此角色下面的人员     

                    var rolenotice = Db.Queryable<p_employee_role, p_role, p_employee>((a, b, e) => new object[] { JoinType.Left, a.role_id == b.id, JoinType.Left, e.id == a.employee_id })
                                           .Where((a, b, e) => b.disabled_code == 1)
                                           .WhereIF(new_leave.is_org == false, (a, b, e) => b.link_id == new_leave.await_verifier_id && a.store_id > 0)
                                           .WhereIF(new_leave.is_org == true, (a, b, e) => b.id == new_leave.await_verifier_id && a.store_id == 0)
                                           .Select((a, b, e) => new { e.id, e.name })
                                           .WithCache()
                                           .ToList();

                    if (rolenotice.Count > 0)
                    {
                        //领用通知
                        notice_content = $"{{\"name\":\"{oaList.creater}\",\"no\":\"{oaList.bill_no}\",\"date\":\"{oaList.create_time.ToString()}\",\"remark\":\"{oaList.remark}\",\"msg\":\" 领用待审批\"}}";
                        var archives = new c_archives();
                        archives.id = oaList.creater_id;
                        archives.name = oaList.creater;
                        archives.phone = apply_phone;
                        var employeenotice = new List<employeeMes>();


                        rolenotice.ForEach(r =>
                        {
                            var con = $"{{\"name\":\"{oaList.creater}\",\"msg\":\"提交了领用申请，请处理！\"}}";
                            employeeSocket.Add(new WebSocketModel { userId = r.id, content = con });

                            employeenotice.Add(new employeeMes { employee_id = r.id, employee = r.name });
                        });
                        notice.NewMethod(oaList.bill_no, archives, oaList.store_id, notice, noticeList, 3, 7, notice_content, oaList.creater, employeenotice);

                    }

                }
                else
                {
                    if (entity.approval_state == 34)
                    {
                        var con = $"{{\"name\":\"{oaList.creater}\",\"msg\":\"提交了领用申请已通过，请查看！\"}}";
                        employeeSocket.Add(new WebSocketModel { userId = oaList.creater_id, content = con });
                    }
                    else
                    {
                        var con = $"{{\"name\":\"{oaList.creater}\",\"msg\":\"提交了领用申请被拒绝，请查看！\"}}";
                        employeeSocket.Add(new WebSocketModel { userId = oaList.creater_id, content = con });

                    }

                    new_leave.is_org = false;

                    //调拨审批完成通知
                    notice_content = $"{{\"content\":\"{entity.verify_remark}\",\"name\":\"{oaList.creater}\",\"msg\":\" 领用审批结果，请查收！\",\"state\":\"{entity.approval_state}\",\"no\":\"{oaList.bill_no}\",\"date\":\"{oaList.create_time.ToString()}\",\"remark\":\"{oaList.remark}\"}}";
                    var archives = new c_archives();
                    archives.id = oaList.creater_id;
                    archives.name = oaList.creater;
                    archives.phone = apply_phone;
                    var employeenotice = new List<employeeMes>();
                    employeenotice.Add(new employeeMes { employee_id = oaList.creater_id, employee = oaList.creater });

                    notice.NewMethod(entity.apply_no, archives, entity.store_id, notice, noticeList, 3, 8, notice_content, oaList.creater, employeenotice);
                }

                new_leave.verifier_id = userinfo.id;
                new_leave.verifier = userinfo.name;
                new_leave.verify_time = DateTime.Now;
                await_verifier_id = entity.approval_state == 36 ? new_leave.await_verifier_id.Value : 0;
                await_verifier = entity.approval_state == 36 ? new_leave.await_verifier : " ";

                //将此单标为已读
                UpdateMethod(userinfo, oaList.bill_no, 3, 5);


                //修改领用表
                Db.Updateable(new_leave)
                  .SetColumns(s => new bus_requisitions_bill { await_verifier_id = await_verifier_id, await_verifier = await_verifier, level = new_leave.level, verifier = new_leave.verifier, verifier_id = new_leave.verifier_id, verify_time = new_leave.verify_time, state = entity.approval_state, is_org = new_leave.is_org, verify_remark = entity.verify_remark })
                  .Where(s => s.bill_no == entity.apply_no)
                  .RemoveDataCache()
                  .EnableDiffLogEvent()
                  .ExecuteCommand();

                //审核记录
                CommonModel model = new CommonModel();
                model.total_level = oaList.total_level;
                model.level = newprocess_detail.processleave;
                model.store_id = oaList.store_id;
                model.store = oaList.store_name;
                model.applicant_id = oaList.creater_id;
                model.applicant = oaList.creater;
                model.process_type_id = 3;
                model.process_type = "领用";


                SetVerify(entity, process_currentMes, userinfo, currentlevel, model, 3, processMes);
                if (noticeList.Count > 0)
                {
                    //新增
                    notice.AddNotice(noticeList);
                    ChatWebSocketMiddleware.SendListAsync(employeeSocket);
                }

            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;



        }

        /// <summary>
        /// 报废审核
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> ApprovaScrap(ApprovalLeaveModel entity)
        {
            if (string.IsNullOrEmpty(entity.apply_no))
            {
                throw new MessageException("请选择报废单!");
            }

            //通知
            var notice = new Business.NoticeService();
            var noticeList = new List<AddNoticeModel>();
            var notice_content = "";
            var employeeSocket = new List<WebSocketModel>();

            var userinfo = new Tools.IdentityModels.GetUser().userInfo;
            var result = Db.Ado.UseTran(() =>
            {
                //查询单子可不可以审核，有没有权限
                var oaList = Db.Queryable<r_assets_scrap>()
                                   .Where(o => o.no == entity.apply_no && o.org_id == userinfo.org_id && (o.state == 26 || o.state == 36))
                                   .WithCache()
                                   .First();
                if (oaList == null)
                {
                    throw new MessageException("未查询到符合条件的报废单!");
                }
                //查询申请人电话
                var apply_phone = Db.Queryable<p_employee>()
                                    .Where(ss => ss.id == oaList.applicant_id)
                                    .Select(ss => ss.phone_no)
                                    .WithCache()
                                    .First();

                //查询当前登录人不是机构人员 则所属部门关联角色
                var roleList = new List<int>();
                int role_id = oaList.await_verifier_id.Value;
                int await_verifier_ids = oaList.await_verifier_id.Value;
                bool is_org = oaList.is_org.Value;
                roleList = GetRole(entity, userinfo, role_id, await_verifier_ids, is_org, roleList);


                //查询当前审核人信息
                var currentlevel = oaList.level;
                var process_currentMes = Db.Queryable<p_process_detials, p_process>((p, pr) => new object[] { JoinType.Left, p.id == pr.id })
                                      .Where((p, pr) => p.id == oaList.process_id && p.level == currentlevel)
                                      .Select((p, pr) => new process_currentMesModel { dept_id = p.dept_id, role_id = p.role_id, employee_id = p.employee_id.Value, dept_name = p.dept_name, role_name = p.role_name, id = p.id, name = pr.name, leave_type_id = pr.leave_type_id, leave_type = pr.leave_type })
                                      .WithCache()
                                      .First();

                if (process_currentMes == null)
                {
                    throw new MessageException("未获取到当前审核流程信息");
                }

                var storeroleList = new List<int>();
                //查询当前门店所有角色
                int store_id = entity.store_id;
                storeroleList = GetNewRole(userinfo, store_id, storeroleList);



                var newVerify = new r_verify();//审核表
                var newVerifyDetail = new r_verify_detials();//审核详情表
                var new_leave = new r_assets_scrap();//报废表
                var await_verifier_id = 0;//待审核人id
                var await_verifier = " ";//待审核人

                //下一流程
                var processMes = new p_process_detials();
                //下一流程详情
                var process_need = new object();

                //查询待审核数据所需字段
                CommonModel processmodel = new CommonModel();
                processmodel.total_level = oaList.total_level;
                processmodel.level = oaList.level;
                processmodel.store_id = oaList.store_id;
                processmodel.store = oaList.store;
                processmodel.applicant = oaList.applicant;
                processmodel.applicant_id = oaList.applicant_id;
                processmodel.org_process_id = oaList.process_id.Value;
                processmodel.process_type_id = 5;
                processmodel.process_type = "报废";

                //审核流程
                if (processmodel.org_process_id <= 0 || processmodel.level > processmodel.total_level)
                {
                    throw new MessageException("此审核流程数据不正确!");
                }

                //通过,查询待审核人信息
                var newprocess_detail = new NewProcessModel();
                if (entity.approval_state == 34)
                {

                    newprocess_detail = NewProcess(entity, userinfo, processmodel, currentlevel, process_currentMes, storeroleList, newVerify, newVerifyDetail, ref processMes);

                    processMes = newprocess_detail?.p_Process_Detials;
                    new_leave.level = newprocess_detail.processleave;
                }
                //部分审核完成
                if (processMes?.id > 0)
                {
                    //获取流程名
                    process_need = Db.Queryable<p_process>()
                                              .Where(pr => pr.id == processMes.id)
                                              .Select(pr => new { pr.name, pr.leave_type_id, pr.leave_type })
                                              .WithCache()
                                              .First();

                    new_leave.await_verifier_id = processMes.role_id > 0 ? processMes.role_id : 0;
                    new_leave.await_verifier = processMes.role_id > 0 ? processMes.role_name : " ";
                    entity.approval_state = 36;
                    new_leave.is_org = processMes.is_org;

                    //查询此角色下面的人员     

                    var rolenotice = Db.Queryable<p_employee_role, p_role, p_employee>((a, b, e) => new object[] { JoinType.Left, a.role_id == b.id, JoinType.Left, e.id == a.employee_id })
                                           .Where((a, b, e) => b.disabled_code == 1)
                                           .WhereIF(new_leave.is_org == false, (a, b, e) => b.link_id == new_leave.await_verifier_id && a.store_id > 0)
                                           .WhereIF(new_leave.is_org == true, (a, b, e) => b.id == new_leave.await_verifier_id && a.store_id == 0)
                                           .Select((a, b, e) => new { e.id, e.name })
                                           .WithCache()
                                           .ToList();

                    if (rolenotice.Count > 0)
                    {
                        //报废通知
                        notice_content = $"{{\"name\":\"{oaList.applicant}\",\"no\":\"{oaList.no}\",\"date\":\"{oaList.apply_time.ToString()}\",\"remark\":\"{oaList.remark}\",\"msg\":\" 报废待审批\"}}";
                        var archives = new c_archives();
                        archives.id = oaList.applicant_id;
                        archives.name = oaList.applicant;
                        archives.phone = apply_phone;
                        var employeenotice = new List<employeeMes>();


                        rolenotice.ForEach(r =>
                        {
                            var con = $"{{\"name\":\"{oaList.applicant}\",\"msg\":\"提交了报废申请，请处理！\"}}";
                            employeeSocket.Add(new WebSocketModel { userId = r.id, content = con });

                            employeenotice.Add(new employeeMes { employee_id = r.id, employee = r.name });
                        });
                        notice.NewMethod(oaList.no, archives, oaList.store_id, notice, noticeList, 3, 5, notice_content, oaList.applicant, employeenotice);

                    }

                }
                else
                {

                    if (entity.approval_state == 34)
                    {
                        //新增报废信息
                        AddScarp(entity, userinfo);

                        var con = $"{{\"name\":\"{oaList.applicant}\",\"msg\":\"提交了报废申请已通过，请查看！\"}}";
                        employeeSocket.Add(new WebSocketModel { userId = oaList.applicant_id, content = con });
                    }
                    else
                    {
                        //修改固定资产状态
                        Db.Updateable<bus_assets>().SetColumns(s => new bus_assets { state = 30 }).Where(w => w.id == oaList.assets_id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();


                        var con = $"{{\"name\":\"{oaList.applicant}\",\"msg\":\"提交了报废申请被拒绝，请查看！\"}}";
                        employeeSocket.Add(new WebSocketModel { userId = oaList.applicant_id, content = con });

                    }

                    new_leave.is_org = false;

                    //报废审批完成通知
                    notice_content = $"{{\"content\":\"{entity.verify_remark}\",\"name\":\"{oaList.applicant}\",\"msg\":\" 报废审批结果，请查收！\",\"state\":\"{entity.approval_state}\",\"no\":\"{oaList.no}\",\"date\":\"{oaList.apply_time.ToString()}\",\"remark\":\"{oaList.remark}\"}}";
                    var archives = new c_archives();
                    archives.id = oaList.applicant_id;
                    archives.name = oaList.applicant;
                    archives.phone = apply_phone;
                    var employeenotice = new List<employeeMes>();
                    employeenotice.Add(new employeeMes { employee_id = oaList.applicant_id, employee = oaList.applicant });

                    notice.NewMethod(entity.apply_no, archives, entity.store_id, notice, noticeList, 3, 10, notice_content, oaList.applicant, employeenotice);
                }

                new_leave.verifier_id = userinfo.id;
                new_leave.verifier = userinfo.name;
                new_leave.verify_time = DateTime.Now;
                await_verifier_id = entity.approval_state == 36 ? new_leave.await_verifier_id.Value : 0;
                await_verifier = entity.approval_state == 36 ? new_leave.await_verifier : " ";

                //将此单标为已读
                UpdateMethod(userinfo, oaList.no, 3, 9);


                //修改报废表
                Db.Updateable(new_leave)
                  .SetColumns(s => new r_assets_scrap { await_verifier_id = await_verifier_id, await_verifier = await_verifier, level = new_leave.level, verifier = new_leave.verifier, verifier_id = new_leave.verifier_id, verify_time = new_leave.verify_time, state = entity.approval_state, is_org = new_leave.is_org, verify_remark = entity.verify_remark })
                  .Where(s => s.no == entity.apply_no)
                  .RemoveDataCache()
                  .EnableDiffLogEvent()
                  .ExecuteCommand();

                //审核记录
                CommonModel model = new CommonModel();
                model.total_level = oaList.total_level;
                model.level = newprocess_detail.processleave;
                model.store_id = oaList.store_id;
                model.store = oaList.store;
                model.applicant_id = oaList.applicant_id;
                model.applicant = oaList.applicant;
                model.process_type_id = 5;
                model.process_type = "报废";


                SetVerify(entity, process_currentMes, userinfo, currentlevel, model, 3, processMes);
                if (noticeList.Count > 0)
                {
                    //新增
                    notice.AddNotice(noticeList);
                    ChatWebSocketMiddleware.SendListAsync(employeeSocket);
                }

            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;



        }


        /// <summary>
        /// 报损报溢审核
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> ApprovaLossOver(ApprovalLeaveModel entity)
        {
            if (string.IsNullOrEmpty(entity.apply_no))
            {
                throw new MessageException("请选择单据!");
            }

            //通知
            var notice = new Business.NoticeService();
            var noticeList = new List<AddNoticeModel>();
            var notice_content = "";
            var employeeSocket = new List<WebSocketModel>();

            var userinfo = new Tools.IdentityModels.GetUser().userInfo;
            var result = Db.Ado.UseTran(() =>
            {
                //查询单子可不可以审核，有没有权限
                var oaList = Db.Queryable<bus_loss_overflow>()
                                   .Where(o => o.no == entity.apply_no && o.org_id == userinfo.org_id && (o.state == 26 || o.state == 36))
                                   .WithCache()
                                   .First();
                if (oaList == null)
                {
                    throw new MessageException("未查询到符合条件的单据!");
                }
                //查询申请人电话
                var apply_phone = Db.Queryable<p_employee>()
                                    .Where(ss => ss.id == oaList.creater_id)
                                    .Select(ss => ss.phone_no)
                                    .WithCache()
                                    .First();

                //查询当前登录人不是机构人员 则所属部门关联角色
                var roleList = new List<int>();
                int role_id = oaList.await_verifier_id.Value;
                int await_verifier_ids = oaList.await_verifier_id.Value;
                bool is_org = oaList.is_org.Value;
                roleList = GetRole(entity, userinfo, role_id, await_verifier_ids, is_org, roleList);


                //查询当前审核人信息
                var currentlevel = oaList.level;
                var process_currentMes = Db.Queryable<p_process_detials, p_process>((p, pr) => new object[] { JoinType.Left, p.id == pr.id })
                                      .Where((p, pr) => p.id == oaList.process_id && p.level == currentlevel)
                                      .Select((p, pr) => new process_currentMesModel { dept_id = p.dept_id, role_id = p.role_id, employee_id = p.employee_id.Value, dept_name = p.dept_name, role_name = p.role_name, id = p.id, name = pr.name, leave_type_id = pr.leave_type_id, leave_type = pr.leave_type })
                                      .WithCache()
                                      .First();

                if (process_currentMes == null)
                {
                    throw new MessageException("未获取到当前审核流程信息");
                }

                var storeroleList = new List<int>();
                //查询当前门店所有角色
                int store_id = entity.store_id;
                storeroleList = GetNewRole(userinfo, store_id, storeroleList);



                var newVerify = new r_verify();//审核表
                var newVerifyDetail = new r_verify_detials();//审核详情表
                var new_leave = new bus_loss_overflow();//报损报溢表
                var await_verifier_id = 0;//待审核人id
                var await_verifier = " ";//待审核人

                //下一流程
                var processMes = new p_process_detials();
                //下一流程详情
                var process_need = new object();

                //查询待审核数据所需字段
                CommonModel processmodel = new CommonModel();
                processmodel.total_level = oaList.total_level;
                processmodel.level = oaList.level;
                processmodel.store_id = oaList.store_id;
                processmodel.store = oaList.store_name;
                processmodel.applicant = oaList.creater;
                processmodel.applicant_id = oaList.creater_id.Value;
                processmodel.org_process_id = oaList.process_id.Value;
                processmodel.process_type_id = 6;
                processmodel.process_type = "报损报溢";

                //审核流程
                if (processmodel.org_process_id <= 0 || processmodel.level > processmodel.total_level)
                {
                    throw new MessageException("此审核流程数据不正确!");
                }

                //通过,查询待审核人信息
                var newprocess_detail = new NewProcessModel();
                if (entity.approval_state == 34)
                {

                    newprocess_detail = NewProcess(entity, userinfo, processmodel, currentlevel, process_currentMes, storeroleList, newVerify, newVerifyDetail, ref processMes);

                    processMes = newprocess_detail?.p_Process_Detials;
                    new_leave.level = newprocess_detail.processleave;
                }
                //部分审核完成
                if (processMes?.id > 0)
                {
                    //获取流程名
                    process_need = Db.Queryable<p_process>()
                                              .Where(pr => pr.id == processMes.id)
                                              .Select(pr => new { pr.name, pr.leave_type_id, pr.leave_type })
                                              .WithCache()
                                              .First();

                    new_leave.await_verifier_id = processMes.role_id > 0 ? processMes.role_id : 0;
                    new_leave.await_verifier = processMes.role_id > 0 ? processMes.role_name : " ";
                    entity.approval_state = 36;
                    new_leave.is_org = processMes.is_org;

                    //查询此角色下面的人员     

                    var rolenotice = Db.Queryable<p_employee_role, p_role, p_employee>((a, b, e) => new object[] { JoinType.Left, a.role_id == b.id, JoinType.Left, e.id == a.employee_id })
                                           .Where((a, b, e) => b.disabled_code == 1)
                                           .WhereIF(new_leave.is_org == false, (a, b, e) => b.link_id == new_leave.await_verifier_id && a.store_id > 0)
                                           .WhereIF(new_leave.is_org == true, (a, b, e) => b.id == new_leave.await_verifier_id && a.store_id == 0)
                                           .Select((a, b, e) => new { e.id, e.name })
                                           .WithCache()
                                           .ToList();

                    if (rolenotice.Count > 0)
                    {
                        //报损报溢通知
                        notice_content = $"{{\"name\":\"{oaList.creater}\",\"no\":\"{oaList.no}\",\"date\":\"{oaList.create_time.ToString()}\",\"remark\":\"{oaList.remark}\",\"msg\":\" 报损报溢待审批\"}}";
                        var archives = new c_archives();
                        archives.id = oaList.creater_id.Value;
                        archives.name = oaList.creater;
                        archives.phone = apply_phone;
                        var employeenotice = new List<employeeMes>();


                        rolenotice.ForEach(r =>
                        {
                            var con = $"{{\"name\":\"{oaList.creater}\",\"msg\":\"提交了报损报溢申请，请处理！\"}}";
                            employeeSocket.Add(new WebSocketModel { userId = r.id, content = con });

                            employeenotice.Add(new employeeMes { employee_id = r.id, employee = r.name });
                        });
                        notice.NewMethod(oaList.no, archives, oaList.store_id, notice, noticeList, 3, 11, notice_content, oaList.creater, employeenotice);

                    }

                }
                else
                {

                    if (entity.approval_state == 34)
                    {
                        //通过审核，操作数据
                        LossPass(entity, userinfo);

                        var con = $"{{\"name\":\"{oaList.creater}\",\"msg\":\"提交了报损报溢申请已通过，请查看！\"}}";
                        employeeSocket.Add(new WebSocketModel { userId = oaList.creater_id.Value, content = con });
                    }
                    else
                    {
                        //拒绝
                        UnPass(entity, userinfo);

                        var con = $"{{\"name\":\"{oaList.creater}\",\"msg\":\"提交了报损报溢申请被拒绝，请查看！\"}}";
                        employeeSocket.Add(new WebSocketModel { userId = oaList.creater_id.Value, content = con });

                    }

                    new_leave.is_org = false;

                    //报损报溢审批完成通知
                    notice_content = $"{{\"content\":\"{entity.verify_remark}\",\"name\":\"{oaList.creater}\",\"msg\":\" 报损报溢审批结果，请查收！\",\"state\":\"{entity.approval_state}\",\"no\":\"{oaList.no}\",\"date\":\"{oaList.create_time.ToString()}\",\"remark\":\"{oaList.remark}\"}}";
                    var archives = new c_archives();
                    archives.id = oaList.creater_id.Value;
                    archives.name = oaList.creater;
                    archives.phone = apply_phone;
                    var employeenotice = new List<employeeMes>();
                    employeenotice.Add(new employeeMes { employee_id = oaList.creater_id.Value, employee = oaList.creater });

                    notice.NewMethod(entity.apply_no, archives, entity.store_id, notice, noticeList, 3, 12, notice_content, oaList.creater, employeenotice);
                }

                new_leave.verifier_id = userinfo.id;
                new_leave.verifier = userinfo.name;
                new_leave.verify_time = DateTime.Now;
                await_verifier_id = entity.approval_state == 36 ? new_leave.await_verifier_id.Value : 0;
                await_verifier = entity.approval_state == 36 ? new_leave.await_verifier : " ";

                //将此单标为已读
                UpdateMethod(userinfo, oaList.no, 3, 9);


                //修改报损报溢表
                Db.Updateable(new_leave)
                  .SetColumns(s => new bus_loss_overflow { await_verifier_id = await_verifier_id, await_verifier = await_verifier, level = new_leave.level, verifier = new_leave.verifier, verifier_id = new_leave.verifier_id, verify_time = new_leave.verify_time, state = entity.approval_state, is_org = new_leave.is_org, verify_remark = entity.verify_remark })
                  .Where(s => s.no == entity.apply_no)
                  .RemoveDataCache()
                  .EnableDiffLogEvent()
                  .ExecuteCommand();

                //审核记录
                CommonModel model = new CommonModel();
                model.total_level = oaList.total_level;
                model.level = newprocess_detail.processleave;
                model.store_id = oaList.store_id;
                model.store = oaList.store_name;
                model.applicant_id = oaList.creater_id.Value;
                model.applicant = oaList.creater;
                model.process_type_id = 6;
                model.process_type = "报损报溢";


                SetVerify(entity, process_currentMes, userinfo, currentlevel, model, 3, processMes);
                if (noticeList.Count > 0)
                {
                    //新增
                    notice.AddNotice(noticeList);
                    ChatWebSocketMiddleware.SendListAsync(employeeSocket);
                }

            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;



        }

        #endregion

        /// <summary>
        /// 操作库存，出库
        /// </summary>
        /// <param name="userinfo"></param>
        /// <param name="list"></param>
        public void SetSum(Tools.IdentityModels.GetUser.UserInfo userinfo, bus_loss_overflow list, int asset_id)
        {
            //查询库存
            var storage = Db.Queryable<bus_storage>()
                          .Where(s => s.std_item_id == list.std_item_id && s.org_id == userinfo.org_id && s.store_id == list.store_id && s.type_id == list.item_type_id)
                          .WithCache()
                          .First();

            if (storage == null)
            {
                throw new MessageException("未找到对应库存！");
            }

            var num_d = list.process_id == 1 ? storage.num - 1 : storage.num - list.num;
            var use_num_d = list.process_id == 1 ? storage.use_num - 1 : storage.use_num - list.num;
            if (num_d < 0 || use_num_d < 0)
            {
                throw new MessageException("库存数量不足");
            }

            //修改库存数量
            Db.Updateable<bus_storage>()
              .SetColumns(s => new bus_storage { num = num_d, use_num = use_num_d })
              .Where(s => s.id == storage.id)
              .RemoveDataCache()
              .EnableDiffLogEvent()
              .ExecuteCommand();

            //查询库存明细
            var storage_detail = Db.Queryable<bus_storage_detials>()
                                 .Where(s => s.id == storage.id && s.spec == list.spec && s.manufactor_id == list.manufactor_id)
                                 .OrderBy(o => o.expire_date)
                                 .OrderBy(o => o.no)
                                 .WithCache()
                                 .ToList();

            var d_num = list.process_id == 1 ? 1 : list.num;
            //修改库存明细数量
            foreach (var storage_detials in storage_detail)
            {
                if (storage_detials.use_num > 0)
                {
                    short this_num = 0;
                    //修改库存明细可用数量
                    if (storage_detials.use_num >= d_num)
                    {
                        storage_detials.use_num -= d_num;
                        storage_detials.num -= d_num;
                        this_num = Convert.ToInt16(d_num);
                        d_num = 0;
                    }
                    else
                    {
                        this_num = Convert.ToInt16(storage_detials.use_num);
                        d_num -= storage_detials.use_num;
                        storage_detials.use_num = 0;
                        storage_detials.num = 0;
                    }

                    //修改库存明细
                    Db.Updateable<bus_storage_detials>().SetColumns(s => new bus_storage_detials { use_num = storage_detials.use_num, num = storage_detials.num }).Where(w => w.id == storage_detials.id && w.no == storage_detials.no && w.manufactor_id == storage_detials.manufactor_id)
                    .RemoveDataCache()
                    .EnableDiffLogEvent()
                    .ExecuteCommand();

                    var is_asset = list.property_id == 1 ? 2 : 3;
                    //出库
                    OutStorage(userinfo, 6, "报损", list.store_id, list.no, list.store_name, "报损出库", storage_detials, this_num, is_asset, asset_id);

                    if (d_num == 0)
                    {
                        break;
                    }
                }
            }

        }


        /// <summary>
        /// 操作库存（入库）
        /// </summary>
        /// <param name="userinfo"></param>
        /// <param name="list"></param>
        public void SetAddSum(Tools.IdentityModels.GetUser.UserInfo userinfo, bus_loss_overflow list)
        {
            //查询库存
            var storage = Db.Queryable<bus_storage>()
                          .Where(s => s.std_item_id == list.std_item_id && s.org_id == userinfo.org_id && s.store_id == list.store_id && s.type_id == list.item_type_id)
                          .WithCache()
                          .First();

            if (storage == null)
            {
                throw new MessageException("未找到对应库存！");
            }

            var num_d = storage.num + list.num;
            var use_num_d = storage.use_num + list.num;
            if (num_d != use_num_d)
            {
                throw new MessageException("实际可用库存数量不相等");
            }

            //修改库存数量
            Db.Updateable<bus_storage>()
              .SetColumns(s => new bus_storage { num = num_d, use_num = use_num_d })
              .Where(s => s.id == storage.id)
              .RemoveDataCache()
              .EnableDiffLogEvent()
              .ExecuteCommand();



            var bill = new bus_put_in_storage();
            bill.bill_no = "RK" + DateTime.Now.ToString("yyMMdd");
            //查询最大单号
            var max_no = Db.Queryable<bus_put_in_storage>().Where(w => w.bill_no.StartsWith(bill.bill_no)).OrderBy(o => o.bill_no, OrderByType.Desc).Select(s => s.bill_no).WithCache().First();
            if (max_no == null)
            {
                bill.bill_no += "0001";
            }
            else
            {
                max_no = max_no.Substring(bill.bill_no.Length);
                bill.bill_no += (int.Parse(max_no) + 1).ToString().PadLeft(4, '0');
            }

            bill.realted_no = list.no;
            bill.org_id = userinfo.org_id;
            bill.store_id = list.store_id;
            bill.store_name = list.store_name;
            bill.type_id = 5;
            bill.type = "报溢";
            bill.state = 40;
            bill.remark = "报溢入库";
            bill.creater = userinfo.name;
            bill.creater_id = userinfo.id;
            bill.put_in_time = DateTime.Now;

            //添加入库单返回ID
            var command_num = Db.Insertable(bill).ExecuteCommand();
            redisCache.RemoveAll<bus_put_in_storage>();
            if (command_num <= 0)
            {
                throw new MessageException("入库失败！");
            }

            //查询基础数据
            var base_detail = Db.Queryable<p_std_item, p_std_item_detials>((s, sd) => new object[] { JoinType.Left, s.id == sd.id })
                              .Where((s, sd) => sd.id == list.std_item_id && sd.manufactor_id == list.manufactor_id && sd.spec == list.spec)
                              .Select((s, sd) => new { sd.id, sd.manufactor_id, s.name, s.type, s.type_id, sd.buy_price, sd.buy_unit, sd.manufactor, sd.price, sd.spec, s.unit, sd.approval_no, sd.buy_multiple })
                              .WithCache()
                              .First();

            if (base_detail == null)
            {
                throw new MessageException("没获取到基础数据！");
            }

            var no = DateTime.Now.ToString("yyMMddfff");
            var storage_detials = new bus_storage_detials { approval_no = base_detail.approval_no, bill_no = bill.bill_no, bill_num = list.num, buy_multiple = base_detail.buy_multiple, buy_price = base_detail.buy_price, buy_unit = base_detail.buy_unit, id = storage.id, manufactor = base_detail.manufactor, manufactor_id = base_detail.manufactor_id, name = list.name, no = no, num = list.num, price = base_detail.price, spec = list.spec, std_item_id = list.std_item_id, type = list.item_type, type_id = list.item_type_id, unit = base_detail.unit, use_num = list.num, put_in_type_id = 5, put_in_type = "报溢", put_in_num = list.num, buy_date = DateTime.Now };

            //添加库存明细
            Db.Insertable(storage_detials).ExecuteCommand();
            redisCache.RemoveAll<bus_storage_detials>();

            //添加入库单明细
            var put_in_List = new bus_put_in_storage_detials { bill_no = bill.bill_no, name = list.name, spec = list.spec, std_item_id = list.std_item_id, unit = list.unit, approval_no = base_detail.approval_no, buy_multiple = base_detail.buy_multiple, buy_price = base_detail.buy_price, buy_unit = base_detail.buy_unit, manufactor = base_detail.manufactor, manufactor_id = base_detail.manufactor_id, price = base_detail.price, type = list.item_type, type_id = list.item_type_id, buy_date = DateTime.Now.Date, num = Convert.ToInt16(list.num), no = 0 };
            Db.Insertable(put_in_List).ExecuteCommand();
            redisCache.RemoveAll<bus_put_in_storage_detials>();

            //获取固定资产基础项目ID
            var assets_std_list = Db.Queryable<p_std_item, b_codebase>((si, cb) => new object[] { JoinType.Left, si.type_id == cb.id }).Where((si, cb) => cb.property_id == 1).WithCache().Select((si, cb) => new { si.id, cb.year }).ToList();


            //固资入库
            if (list.property_id == 1)
            {

                //固定资产列表
                var assetsList = new List<bus_assets>();
                List<bus_put_in_assets> put_in_assets = new List<bus_put_in_assets>();
                for (int i = 1; i <= list.num; i++)
                {
                    assetsList.Add(new bus_assets { bill_no = bill.bill_no, buy_date = DateTime.Now.Date, buy_price = base_detail.buy_price, manufactor = base_detail.manufactor, manufactor_id = base_detail.manufactor_id, name = base_detail.name, no = $"{base_detail.type_id}{bill.bill_no.Replace("RK", "")}{i}", org_id = userinfo.org_id, price = base_detail.price, spec = base_detail.spec, state = 30, std_item_id = list.std_item_id, store_id = list.store_id, type = base_detail.type, type_id = base_detail.type_id.Value, unit = base_detail.unit, net_salvage_rate = 5, net_salvage = base_detail.buy_price * 5 / 100, total_depreciation = 0, depreciation = base_detail.buy_price * 95 / 100, remaining_depreciation = base_detail.buy_price * 95 / 100, net_residual = base_detail.buy_price, month_depreciation = 95 / assets_std_list.Where(w => w.id == base_detail.id).FirstOrDefault().year / 100 / 12 * base_detail.buy_price });

                }

                //添加固定资产
                Db.Insertable(assetsList).ExecuteCommand();
                redisCache.RemoveAll<bus_assets>();

                var assets = Db.Queryable<bus_assets>().Where(w => w.bill_no == bill.bill_no).OrderBy(o => o.no).WithCache().ToList();
                assets.ForEach(item =>
                {
                    put_in_assets.Add(new bus_put_in_assets { assets_id = item.id, bill_no = item.bill_no, manufactor_id = item.manufactor_id, spec = item.spec, std_item_id = item.std_item_id });
                });

                //添加固定资产入库
                Db.Insertable(put_in_assets).ExecuteCommand();
                redisCache.RemoveAll<bus_put_in_assets>();


            }

        }

        /// <summary>
        /// 报损报溢拒绝
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userinfo"></param>
        private void UnPass(ApprovalLeaveModel entity, Tools.IdentityModels.GetUser.UserInfo userinfo)
        {
            var loss_overflow = Db.Queryable<bus_loss_overflow>().Where(w => w.no == entity.apply_no).WithCache().First();

            //生成报损报溢单负单
            loss_overflow.state = 28;
            loss_overflow.delete_no = loss_overflow.no;
            loss_overflow.no = "-" + loss_overflow.no;
            loss_overflow.creater = userinfo.name;
            loss_overflow.creater_id = userinfo.id;
            loss_overflow.create_time = DateTime.Now;
            loss_overflow.num = -loss_overflow.num;
            Db.Insertable(loss_overflow).ExecuteCommand();
            redisCache.RemoveAll<bus_loss_overflow>();

            //查询盘点明细
            var stocktaking_detials = Db.Queryable<bus_stocktaking_detials>().Where(w => w.no == loss_overflow.realted_no && w.std_item_id == loss_overflow.std_item_id && w.spec == loss_overflow.spec && w.manufactor_id == loss_overflow.manufactor_id).WithCache().First();
            if (loss_overflow.type_id == 1)//还原报溢
            {
                stocktaking_detials.not_report_num -= loss_overflow.num;
                stocktaking_detials.report_num += loss_overflow.num;
            }
            else//还原报损
            {
                stocktaking_detials.not_report_num += loss_overflow.num;
                stocktaking_detials.report_num -= loss_overflow.num;
            }
            //修改盘点待报已报数量
            Db.Updateable<bus_stocktaking_detials>().SetColumns(s => new bus_stocktaking_detials { not_report_num = stocktaking_detials.not_report_num, report_num = stocktaking_detials.report_num }).Where(w => w.no == loss_overflow.realted_no && w.std_item_id == loss_overflow.std_item_id && w.spec == loss_overflow.spec && w.manufactor_id == loss_overflow.manufactor_id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

            //修改盘点单状态
            Db.Updateable<bus_stocktaking>().SetColumns(s => new bus_stocktaking { state = 33 }).Where(w => w.no == loss_overflow.realted_no).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

            //查询报损报溢单明细
            var loss_overflow_detials = Db.Queryable<bus_loss_overflow_detials>().Where(w => w.no == entity.apply_no).WithCache().ToList();

            //修改固定资产状态为闲置中
            var nos = loss_overflow_detials.Select(s => s.assets_no).ToList();
            Db.Updateable<bus_assets>().SetColumns(s => new bus_assets { state = 30 }).Where(w => nos.Contains(w.no)).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

            //生成报损报溢单明细负单
            var del_loss_overflow_detials = loss_overflow_detials.Select(s => new bus_loss_overflow_detials { remark = s.remark, no = loss_overflow.no, assets_no = s.assets_no }).ToList();
            Db.Insertable(del_loss_overflow_detials).ExecuteCommand();
            redisCache.RemoveAll<bus_loss_overflow_detials>();
        }

        /// <summary>
        /// 报损报溢通过
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userinfo"></param>
        private void LossPass(ApprovalLeaveModel entity, Tools.IdentityModels.GetUser.UserInfo userinfo)
        {
            //查询单据
            var list = Db.Queryable<bus_loss_overflow>()
                       .Where(b => b.no == entity.apply_no && b.org_id == userinfo.org_id && b.store_id == entity.store_id && b.delete_no == null)
                       .WithCache()
                       .First();

            if (list == null)
            {
                throw new MessageException("未查询到此单据详情!");
            }

            //报溢
            if (list?.type_id == 1)
            {
                //操作库存(入库)
                SetAddSum(userinfo, list);
            }
            //报损
            else if (list?.type_id == 2)
            {
                //固资
                if (list?.property_id == 1)
                {
                    //查询关联固资详情
                    var list_detail = Db.Queryable<bus_loss_overflow_detials>()
                                      .Where(bl => bl.no == list.no)
                                      .WithCache()
                                      .ToList();
                    if (list_detail.Count == 0)
                    {
                        throw new MessageException("没有数据可处理!");
                    }

                    list_detail.ForEach(f =>
                    {
                        //处理此数据报损
                        //查询固资id
                        var asset_id = Db.Queryable<bus_assets>()
                                       .Where(b => b.no == f.assets_no)
                                       .Select(b => b.id)
                                       .WithCache()
                                       .First();
                        if (asset_id == 0)
                        {
                            throw new MessageException("没有查询到固资id!");
                        }

                        Db.Updateable<bus_assets>()
                          .SetColumns(b => b.state == 47)
                          .Where(b => b.no == f.assets_no)
                          .RemoveDataCache()
                          .EnableDiffLogEvent()
                          .ExecuteCommand();

                        //操作库存
                        SetSum(userinfo, list, asset_id);
                    });

                }
                else
                {
                    //操作库存(出库)
                    SetSum(userinfo, list, 0);
                }
            }
            else
            {
                throw new MessageException("非报损报溢单据!");
            }
        }

        /// <summary>
        /// 报废操作
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userinfo"></param>
        private void AddScarp(ApprovalLeaveModel entity, Tools.IdentityModels.GetUser.UserInfo userinfo)
        {
            var scarp_det = Db.Queryable<r_assets_scrap>()
                            .Where(ras => ras.no == entity.apply_no)
                            .WithCache()
                            .First();

            if (scarp_det == null)
            {
                throw new MessageException("未找到报废单明细！");
            }

            //查询固资id
            var asset_id = Db.Queryable<bus_assets>()
                         .Where(a => a.no == scarp_det.assets_no && a.org_id == userinfo.org_id && a.store_id == scarp_det.store_id)
                         .WithCache()
                         .Select(a => a.id)
                         .First();

            if (asset_id == 0)
            {
                throw new MessageException("未找到固资id！");
            }

            //修改固资状态
            Db.Updateable<bus_assets>()
              .SetColumns(a => a.state == 32)
              .Where(a => a.id == asset_id)
              .RemoveDataCache()
              .EnableDiffLogEvent()
              .ExecuteCommand();

            //查询库存
            var storage = Db.Queryable<bus_storage>()
                          .Where(s => s.std_item_id == scarp_det.std_item_id && s.org_id == userinfo.org_id && s.store_id == scarp_det.store_id && s.type_id == scarp_det.type_id)
                          .WithCache()
                          .First();

            if (storage == null)
            {
                throw new MessageException("未找到对应库存！");
            }

            var num_d = storage.num - 1;
            var use_num_d = storage.use_num - 1;
            if (num_d < 0 || use_num_d < 0)
            {
                throw new MessageException("库存数量不足");
            }

            //修改库存数量
            Db.Updateable<bus_storage>()
              .SetColumns(s => new bus_storage { num = num_d, use_num = use_num_d })
              .Where(s => s.id == storage.id)
              .RemoveDataCache()
              .EnableDiffLogEvent()
              .ExecuteCommand();

            //查询库存明细
            var storage_detail = Db.Queryable<bus_storage_detials>()
                                 .Where(s => s.id == storage.id && s.spec == scarp_det.spec && s.manufactor_id == scarp_det.manufactor_id)
                                 .OrderBy(o => o.expire_date)
                                 .OrderBy(o => o.no)
                                 .WithCache()
                                 .ToList();

            var d_num = 1;
            //修改库存明细数量
            foreach (var storage_detials in storage_detail)
            {
                if (storage_detials.use_num > 0)
                {
                    short this_num = 0;
                    //修改库存明细可用数量
                    if (storage_detials.use_num >= d_num)
                    {
                        storage_detials.use_num -= d_num;
                        storage_detials.num -= d_num;
                        this_num = Convert.ToInt16(d_num);
                        d_num = 0;
                    }
                    else
                    {
                        this_num = Convert.ToInt16(storage_detials.use_num);
                        d_num -= storage_detials.use_num;
                        storage_detials.use_num = 0;
                        storage_detials.num = 0;
                    }

                    //修改库存明细
                    Db.Updateable<bus_storage_detials>().SetColumns(s => new bus_storage_detials { use_num = storage_detials.use_num, num = storage_detials.num }).Where(w => w.id == storage_detials.id && w.no == storage_detials.no && w.manufactor_id == storage_detials.manufactor_id)
                    .RemoveDataCache()
                    .EnableDiffLogEvent()
                    .ExecuteCommand();
                    //出库
                    OutStorage(userinfo, 5, "报废", scarp_det.store_id, scarp_det.no, scarp_det.store, "报废出库", storage_detials, this_num, 2, asset_id);

                    if (d_num == 0)
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 出库
        /// </summary>
        /// <param name="userinfo"></param>
        /// <param name="type_id"></param>
        /// <param name="type">类型</param>
        /// <param name="store_id">门店id</param>
        /// <param name="no">关联单号</param>
        /// <param name="store">门店</param>
        /// <param name="remark">备注</param>
        /// <param name="storage_detials">库存明细</param>
        /// <param name="this_num">出库数量</param>
        /// <param name="is_asset">是否固资</param>
        /// <param name="asset_id">固资id</param>
        private void OutStorage(Tools.IdentityModels.GetUser.UserInfo userinfo, short type_id, string type, int store_id, string no, string store, string remark, bus_storage_detials storage_detials, short this_num, int is_asset, int asset_id)
        {
            //查询是否存在出库单
            var have_grant = Db.Queryable<bus_out_storage>()
                             .Where(g => g.org_id == userinfo.org_id && g.store_id == store_id && g.realted_no == no)
                             .WithCache()
                             .First();

            //新增出库表
            var out_bill = new bus_out_storage();
            //出库单号
            var bill_no = "CK" + DateTime.Now.ToString("yyMMdd");
            //查询最大单号
            var max_no = Db.Queryable<bus_out_storage>().Where(w => w.bill_no.StartsWith(bill_no)).OrderBy(o => o.bill_no, OrderByType.Desc).Select(s => s.bill_no).WithCache().First();
            if (max_no == null)
            {
                bill_no += "0001";
            }
            else
            {
                max_no = max_no.Substring(bill_no.Length);
                bill_no += (int.Parse(max_no) + 1).ToString().PadLeft(4, '0');
            }

            if (have_grant == null)
            {
                out_bill.bill_no = bill_no;
                out_bill.creater = userinfo.name;
                out_bill.creater_id = userinfo.id;
                out_bill.create_time = DateTime.Now;
                out_bill.org_id = userinfo.org_id;
                out_bill.out_time = DateTime.Now;
                out_bill.realted_no = no;
                out_bill.remark = remark;
                out_bill.state = 42;
                out_bill.store_id = store_id;
                out_bill.store_name = store;
                out_bill.type_id = type_id;
                out_bill.type = type;

                Db.Insertable<bus_out_storage>(out_bill).ExecuteCommand();
                redisCache.RemoveAll<bus_out_storage>();
            }

            //生成出库单明细
            var out_storage_detials = new bus_out_storage_detials();

            out_storage_detials.approval_no = storage_detials.approval_no;
            out_storage_detials.bill_no = have_grant == null ? bill_no : have_grant?.bill_no;
            out_storage_detials.bill_num = Convert.ToInt32(this_num);
            out_storage_detials.manufactor = storage_detials.manufactor;
            out_storage_detials.manufactor_id = storage_detials.manufactor_id;
            out_storage_detials.name = storage_detials.name;
            out_storage_detials.price = 0;
            out_storage_detials.spec = storage_detials.spec;
            out_storage_detials.std_item_id = storage_detials.std_item_id;
            out_storage_detials.storage_id = storage_detials.id;
            out_storage_detials.storage_no = storage_detials.no;
            out_storage_detials.type = storage_detials.type;
            out_storage_detials.type_id = storage_detials.type_id;
            out_storage_detials.unit = storage_detials.unit;

            //新增出库明细

            Db.Insertable(out_storage_detials).ExecuteCommand();
            redisCache.RemoveAll<bus_out_storage_detials>();

            if (is_asset == 2)
            {
                var asset = new bus_out_assets { assets_id = asset_id, bill_no = out_storage_detials.bill_no, manufactor_id = storage_detials.manufactor_id, spec = storage_detials.spec, std_item_id = storage_detials.std_item_id };

                //新增固资出库
                Db.Insertable(asset).ExecuteCommand();
                redisCache.RemoveAll<bus_out_assets>();

            }
        }

    }
}
