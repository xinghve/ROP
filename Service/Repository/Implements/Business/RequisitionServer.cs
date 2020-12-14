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
    /// 物资领用
    /// </summary>
    public class RequisitionServer:DbContext,IRequisitionService
    {
        /// <summary>
        /// 添加领用
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> AddRequisition(RequisitionAddModel entity)
        {
            if (entity.bill_details.Count==0)
            {
                throw new MessageException("请填写物资详情！");
            }
            //获取当前用户信息
            var userinfo = new Tools.IdentityModels.GetUser().userInfo;

            entity.bill.creater = userinfo.name;
            entity.bill.creater_id = userinfo.id;
            entity.bill.create_time = DateTime.Now;
            entity.bill.org_id = userinfo.org_id;
            entity.bill.state = 26;
            entity.bill.bill_no=entity.bill.is_dept==2? "LYSQ"+entity.bill.dept_id + userinfo.id + DateTime.Now.ToString("yyyyMMddHHmmssff") : "LYSQ" + userinfo.id + DateTime.Now.ToString("yyyyMMddHHmmssff");

            var result = await Db.Ado.UseTranAsync(() => {
                //根据分类属性获取审核流程(1.固资2.低耗)
                var process =Db.Queryable<p_process>()
                                .Where(p => p.state == 1 && p.type_id == 3 && p.org_id == userinfo.org_id && p.leave_type_id == entity.bill.type_id)
                                .WhereIF(entity.bill.store_id > 0, p => p.store_id > 0)
                                .WhereIF(entity.bill.store_id == 0, p => p.store_id == 0)
                                .WithCache()
                                .First();

            if (process==null||process?.id<=0)
            {
                throw new MessageException("领用审核流程未获取到！");
            }

            entity.bill.total_level = process.total_level;            

                //待审核人
                SetProcess(entity.bill, process);

                //新增数据库
                Db.Insertable<bus_requisitions_bill>(entity.bill).ExecuteCommand();
                redisCache.RemoveAll<bus_requisitions_bill>();


                var new_bill_detail = entity.bill_details.Select(b => new bus_requisitions_detail { bill_no = entity.bill.bill_no, name = b.name, num = b.num, remark = b.remark, spec = b.spec, std_item_id = b.std_item_id, type = b.type, type_id = b.type_id, estimate_time=b.estimate_time, unit=b.unit,state=30 }).ToList();

                Db.Insertable<bus_requisitions_detail>(new_bill_detail).ExecuteCommand();
                redisCache.RemoveAll<bus_requisitions_detail>();
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }

            return result.IsSuccess;

        }

        //设置审核信息
        private void SetProcess(bus_requisitions_bill entity,p_process process)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            //查询调拨单需要进行的审核流程
            var store = 0;
            if (entity.store_id != 0)
            {
                store = 99;
            }                        
            entity.level = 0;
            entity.process_id = process.id;

            entity.await_verifier_id = 0;
            entity.is_org = false;

            //审核流程明细
            var process_detials = Db.Queryable<p_process_detials>().Where(w => w.id == process.id).OrderBy(o => o.level).WithCache().ToList();
            //循环流程明细
            foreach (var item in process_detials)
            {
                var isExiste = false;
                if (!item.is_org.Value)
                {
                    isExiste = Db.Queryable<p_role>().Any(w => w.link_id == item.role_id && w.store_id == entity.store_id);
                }
                else
                {
                    isExiste = Db.Queryable<p_role>().Any(w => w.id == item.role_id && w.store_id == 0);
                }
                if (isExiste)
                {
                    entity.level = item.level;
                    entity.await_verifier_id = item.role_id;
                    entity.await_verifier = item.role_name;
                    entity.is_org = item.is_org;
                    break;
                }
                else
                {
                    //审核记录
                    CommonModel model = new CommonModel();
                    model.total_level = process.total_level;
                    model.level = item.level;
                    model.store_id = entity.store_id;
                    model.store = entity.store_name;
                    model.applicant_id = userInfo.id;
                    model.applicant = userInfo.name;
                    model.process_type_id = 3;
                    model.process_type = "领用";

                    ApprovalLeaveModel entityA = new ApprovalLeaveModel();
                    entityA.apply_no = entity.bill_no;
                    entityA.approval_state = 34;
                    entityA.verify_remark = " ";
                    var ver = new ApprovalService();

                    var process_currentMes = Db.Queryable<p_process_detials, p_process>((p, pr) => new object[] { JoinType.Left, p.id == pr.id })
                                .Where((p, pr) => p.id == process.id && p.level == item.level)
                                .Select((p, pr) => new process_currentMesModel { dept_id = p.dept_id, role_id = p.role_id, employee_id = p.employee_id.Value, dept_name = p.dept_name, role_name = p.role_name, id = p.id, name = pr.name, leave_type_id = pr.leave_type_id, leave_type = pr.leave_type })
                                .WithCache()
                                .First();
                    if (process_currentMes == null)
                    {
                        throw new MessageException("未获取到当前审核流程信息");
                    }
                    var nextp = process_detials.Where(dd => dd.level == item.level + 1).FirstOrDefault();
                    entity.state = 36;
                    if (nextp == null)
                    {
                        entity.state = 34;
                    }
                    //添加审核记录                               
                    ver.SetVerify(entityA, process_currentMes, userInfo, item.level, model, 2, nextp);

                }
            }

            //通知
            var notice = new Business.NoticeService();
            var noticeList = new List<AddNoticeModel>();
            var notice_content = "";
            var employeeSocket = new List<WebSocketModel>();

            //查询此角色下面的人员    
            var rolenotice = Db.Queryable<p_employee_role, p_role, p_employee>((a, b, e) => new object[] { JoinType.Left, a.role_id == b.id, JoinType.Left, e.id == a.employee_id })
                                   .Where((a, b, e) => b.disabled_code == 1)
                                   .WhereIF(entity.is_org == false, (a, b, e) => b.link_id == entity.await_verifier_id && a.store_id > 0)
                                   .WhereIF(entity.is_org == true, (a, b, e) => b.id == entity.await_verifier_id && a.store_id == 0)
                                   .Select((a, b, e) => new { e.id, e.name })
                                   .WithCache()
                                   .ToList();

            if (rolenotice.Count > 0)
            {
                //通知
                notice_content = $"{{\"name\":\"{entity.creater}\",\"no\":\"{entity.bill_no}\",\"date\":\"{entity.create_time.ToString()}\",\"remark\":\"{entity.remark}\",\"msg\":\" 领用待审批\"}}";
                var archives = new c_archives();
                archives.id = userInfo.id;
                archives.name = userInfo.name;
                archives.phone = userInfo.phone_no;
                var employeenotice = new List<employeeMes>();


                rolenotice.ForEach(r =>
                {
                    var con = $"{{\"name\":\"{entity.creater}\",\"msg\":\"提交了领用申请，请处理！\"}}";
                    employeeSocket.Add(new WebSocketModel { userId = r.id, content = con });

                    employeenotice.Add(new employeeMes { employee_id = r.id, employee = r.name });

                });
                notice.NewMethod(entity.bill_no, archives, entity.store_id, notice, noticeList, 3, 7, notice_content, entity.creater, employeenotice);

                //新增
                notice.AddNotice(noticeList);
                ChatWebSocketMiddleware.SendListAsync(employeeSocket);
            }
        }

        /// <summary>
        /// 取消申领
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> CancelRequisition(CancelRequisitionModel entity)
        {
            if (string.IsNullOrEmpty(entity.apply_no))
            {
                throw new MessageException("请选择数据！");
            }
            //获取当前用户信息
            var userinfo = new Tools.IdentityModels.GetUser().userInfo;

            //查询领用是否发放
            var isPut = await Db.Queryable<bus_requisitions_bill>()
                              .Where(b => b.bill_no == entity.apply_no && b.store_id == entity.store_id && b.org_id == userinfo.org_id)
                              .WithCache()
                              .FirstAsync();
           
            if (isPut==null||string.IsNullOrEmpty(isPut?.bill_no))
            {
                throw new MessageException("数据为空！");
            }

            if (isPut.creater_id != userinfo.id)
            {
                throw new MessageException("非本人创建领用单，不能撤销");
            }

            if (isPut.state==15)
            {
                throw new MessageException("物品已发放不能撤销！");
            }
            if (isPut.state==35)
            {
                throw new MessageException("已撤销无需再撤销！");
            }
            redisCache.RemoveAll<ApprovalModel>();

          return await Db.Updateable<bus_requisitions_bill>().SetColumns(w => w.state == 35)
              .Where(w => w.bill_no == entity.apply_no && w.org_id == userinfo.org_id && w.store_id == entity.store_id)
              .RemoveDataCache()
              .EnableDiffLogEvent()
              .ExecuteCommandAsync();
         }

        /// <summary>
        /// 个人固资领用记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Page<bus_grant_detail>> GetOwnRequisition(GrantSearch entity)
        {
            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }

            var userinfo = new Tools.IdentityModels.GetUser().userInfo;

            return await Db.Queryable<bus_grant_detail,bus_grant_bill,bus_requisitions_bill>((a,b,c)=>new object[] { JoinType.Left,a.bill_no==b.bill_no,JoinType.Left,b.realted_no==c.bill_no})
                           .Where((a,b,c)=>b.org_id==userinfo.org_id&&b.type_id==1&&c.creater_id==userinfo.id)
                           .WhereIF(entity.state>0,(a,b,c)=>a.state==entity.state)
                           .WhereIF(entity.start_time != null, (a, b, c) => a.create_time >= entity.start_time)
                           .WhereIF(entity.end_time != null, (a, b, c) => a.create_time <= entity.end_time)

                           .OrderBy(entity.order + orderTypeStr)
                           .WithCache()
                           .ToPageAsync(entity.page, entity.limit);


        }

        /// <summary>
        /// 个人领用记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Page<RequisitionPageModel>> GetOwnRequisitionApply(RequisitionRecordSearch entity)
        {
            entity.own = 2;
            return await GetRequisitionRecord(entity);
        }

        /// <summary>
        /// 获取领用详情
        /// </summary>
        /// <param name="apply_no"></param>
        /// <returns></returns>
        public async Task<RequisitionDetail> GetRequisitionDetail(string apply_no)
        {
            if (string.IsNullOrEmpty(apply_no))
            {
                throw new MessageException("请选择领用单!");
            }
            var userinfo = new Tools.IdentityModels.GetUser().userInfo;

            var detail= await Db.Queryable<bus_requisitions_bill>()
                                    .Where(o => o.bill_no == apply_no&&o.org_id==userinfo.org_id)
                                    .WithCache()
                                    .FirstAsync();

            if (detail==null)
            {
                throw new MessageException("领用单不存在!");
            }

            var reqdetail = await Db.Queryable<bus_requisitions_detail>()
                                    .Where(o =>  o.bill_no == apply_no)
                                    .WithCache()
                                    .ToListAsync();
                   

            //获取审核流程
            var verifyProcess = await Db.Queryable<r_verify_detials>()
                                      .Where(s => s.realted_no == apply_no)
                                      .WithCache()
                                      .ToListAsync();

            var newProcessList = new List<p_process>();
           

            newProcessList = await Db.Queryable<p_process>()
                                     .Where(p => p.org_id == userinfo.org_id)
                                     .WhereIF(detail.org_id > 0, p => p.id == detail.process_id)
                                     .WithCache()
                                     .ToListAsync();


            var ids = newProcessList.Select(s => s.id).ToList();

            // 查询流程详情
            var processDetail = await Db.Queryable<p_process_detials>()
                                   .Where(pdd => ids.Contains(pdd.id))
                                   .WithCache()
                                   .ToListAsync();



            var vp= newProcessList.Select(s => new VerifyProcess { is_org = s.store_id == 0 ? true : false, dept_name = s.dept_name, process_id = s.id, VerifyProcessDetail = processDetail.Where(w => w.id == s.id).Select(w => new VerifyProcessDetail { no = w.id, level = w.level, name = w.employee, role_name = w.role_name, is_org = w.is_org.Value }).ToList() }).ToList();



            vp.ForEach(c =>
            {
                c.VerifyProcessDetail = c.VerifyProcessDetail.Select(w => new VerifyProcessDetail { is_org = w.is_org, level = w.level, name = w.name, reason = verifyProcess.Where(v => v.process_level == w.level && v.process_id == w.no).Select(v => v.verify_remark).FirstOrDefault(), role_name = w.role_name, state = verifyProcess.Where(v => v.process_level == w.level && v.process_id == w.no).Select(v => v.state).FirstOrDefault() }).ToList();

            });

            return new RequisitionDetail { dept_name=detail.dept_name, is_dept=detail.is_dept, type=detail.type, re_list = reqdetail,  verifyProcess=vp };

        }

        /// <summary>
        /// 领用记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Page<RequisitionPageModel>> GetRequisitionRecord(RequisitionRecordSearch entity)
        {
            var userinfo = new Tools.IdentityModels.GetUser().userInfo;
            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }

            //查询所有
            var reList = await Db.Queryable<bus_requisitions_bill>()
                               .Where(b => b.org_id == userinfo.org_id && b.store_id == entity.store_id)
                               .WhereIF(entity.own==2,b=>b.creater_id==userinfo.id)
                               .WhereIF(entity.type_id > 0, b => b.type_id == entity.type_id)
                               .WhereIF(!string.IsNullOrEmpty(entity.name), b => b.creater.Contains(entity.name))
                               .WhereIF(entity.state == 15, b => b.state == 15)
                               .WhereIF(entity.state ==33, b => b.state==33)
                               .WhereIF(entity.state ==0, b => b.state == 34)
                               .Select(b => new RequisitionPageModel { type_id = b.type_id, bill_no = b.bill_no, await_verifier = b.await_verifier, await_verifier_id = b.await_verifier_id, creater = b.creater, creater_id = b.creater_id, create_time = b.create_time, dept_id = b.dept_id, dept_name = b.dept_name, is_dept = b.is_dept, is_org = b.is_org, remark = b.remark, state = b.state, store_name = b.store_name, type = b.type, verifier = b.verifier, verify_remark = b.verify_remark, verify_time = b.verify_time })
                               .OrderBy(entity.order + orderTypeStr)
                               .WithCache()
                               .ToPageAsync(entity.page, entity.limit);

            var detileIds = reList.Items.Select(r => r.bill_no).ToList();

            //查询领用详情
            var detList = await Db.Queryable<bus_requisitions_detail>()
                                .Where(rd => detileIds.Contains(rd.bill_no))
                                .WhereIF(entity.state==0,rd=>rd.state!=15)
                                .WhereIF(entity.state==15,rd=>(rd.state==15||rd.state==22))
                                .WithCache()
                                .ToListAsync();

            //获取固定资产基础项目ID
            var assets_std_list = await Db.Queryable<p_std_item, b_codebase>((si, cb) => new object[] { JoinType.Left, si.type_id == cb.id }).Where((si, cb) => cb.property_id == 1).WithCache().Select((si, cb) => si.id).ToListAsync();

            var newdetList = detList.Select(s => new NewDetail { approval_no=s.approval_no, estimate_time=s.estimate_time, state=s.state, std_item_id=s.std_item_id, unit=s.unit, num = s.num, spec = s.spec, name = s.name, type = s.type, bill_no = s.bill_no, type_id = s.type_id, new_num = assets_std_list.Contains(s.std_item_id) ? GetNum(1,s.std_item_id, entity.store_id,s.spec) : GetNum(2, s.std_item_id, entity.store_id,s.spec) }).ToList();

            var allList = reList.Items.Select(s => new RequisitionPageModel {  bill_no = s.bill_no, await_verifier = s.await_verifier, await_verifier_id = s.await_verifier_id, creater = s.creater, creater_id = s.creater_id, create_time = s.create_time, dept_id = s.dept_id, dept_name = s.dept_name, is_dept = s.is_dept, is_org = s.is_org, remark = s.remark, state = s.state, store_name = s.store_name, type_id=s.type_id, type = s.type, verifier = s.verifier, verify_remark = s.verify_remark, verify_time = s.verify_time, bill_details = newdetList.Where(d => d.bill_no == s.bill_no).ToList() }).ToList();

            reList.Items = allList;
            return reList;
        }

        private  int  GetNum(int isgz,int de,int store_id,string spec)
        {
            if (isgz==1)
            {
               
             var assets_list =  Db.Queryable<bus_assets,p_std_item_detials>((a,st) => new object[] { JoinType.Left, (st.id == a.std_item_id && st.spec == a.spec && st.manufactor_id == a.manufactor_id) })
             .Where((a,st) => a.std_item_id == de && a.store_id == store_id && a.state == 30 && st.spec == spec)
             .GroupBy((a,st) => new { st.spec, a.std_item_id})
             .Select((a,st) => new { num = SqlFunc.AggregateCount(a.id), st.spec })
             .WithCache()
             .ToList();

             return assets_list.Select(w=>w.num).Sum();
            }
            else
            {
              
                var list =  Db.Queryable<p_std_item_detials, bus_storage, bus_storage_detials>((d, si, a) => new object[] { JoinType.Left, si.std_item_id == d.id, JoinType.Left, (d.id == a.std_item_id && d.spec==a.spec ) })
               .Where((d, si, a) => d.id == de && si.store_id == store_id&&d.spec==spec&&si.id==a.id)
               .GroupBy((d, si, a) => new { d.spec, d.id })
               .Select((d, si, a) => new { num = SqlFunc.AggregateSum(a.use_num), d.spec, d.id })
               .WithCache()
               .ToList();

                return list.Select(w => w.num).Sum();

            }


        }
        /// <summary>
        /// 发放物资
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> GrantRequisition(RequisitionAddModel entity)
        {
            //查询物资够不够，扣库存数量，增加出库记录
            if (string.IsNullOrEmpty(entity.bill.bill_no)||entity.bill_details.Count==0)
            {
                throw new MessageException("请选择领用申请单！");
            }
            var userinfo = new Tools.IdentityModels.GetUser().userInfo;

            var result = await Db.Ado.UseTranAsync(() =>
            {

                //查询所属分类
                var which_class = Db.Queryable<bus_requisitions_bill>()
                                        .Where(b => b.bill_no == entity.bill.bill_no && b.org_id == userinfo.org_id && b.store_id == entity.bill.store_id)
                                        .WithCache()
                                        .First();

                if (which_class == null || which_class?.type_id != which_class.type_id)
                {
                    throw new MessageException("所属分类不匹配！");
                }

                //发放明细
                var grant_detial_list = new List<bus_grant_detail>();
                //出库明细
                var bus_out_storage_detials_List = new List<bus_out_storage_detials>();
                //非固资的
                if (which_class.type_id != 1)
                {
                    entity.bill_details.ForEach(c =>
                    {
                        //查询发放明细
                        var grant_det = Db.Queryable<bus_requisitions_detail>()
                                        .Where(brd => brd.bill_no == c.bill_no && brd.std_item_id == c.std_item_id && brd.spec == c.spec&&brd.state!=15)
                                        .WithCache()
                                        .First();
                        if (grant_det==null)
                        {
                            throw new MessageException("发放数据不匹配！");
                        }
                        if (grant_det.num<c.num)
                        {
                            throw new MessageException("发放数量大于申领数量！");
                        }
                        c.unit = grant_det.unit;
                        c.type_id = grant_det.type_id;
                        c.type = grant_det.type;

                        var storage = Db.Queryable<bus_storage,bus_storage_detials>((s,sd)=>new object[] {JoinType.Left,sd.std_item_id==s.std_item_id })
                        .Where((s,sd) =>s.id==sd.id&&sd.spec==c.spec && s.org_id == userinfo.org_id && s.store_id == which_class.store_id)
                        .GroupBy((s,sd)=> new {s.id, s.std_item_id,sd.spec })
                         .Select((s, sd) => new StorageRequisitionModel { num_det = SqlFunc.AggregateSum(sd.num), id = s.id, use_num_det = SqlFunc.AggregateSum(sd.use_num),num= s.num, use_num= s.use_num })
                       
                        .WithCache()
                        .First();

                        if (storage==null)
                        {
                            throw new MessageException($"{c.spec}没有库存");
                        }

                        storage.use_num_det -= Convert.ToInt32(c.num);
                        storage.use_num -= Convert.ToInt32(c.num);
                        storage.num_det -= Convert.ToInt32(c.num);
                        storage.num -= Convert.ToInt32(c.num);
                        if (storage.use_num_det < 0 || storage.num_det < 0)
                        {
                            throw new MessageException($"{c.spec}库存数量不足");
                        }

                        Db.Updateable<bus_storage>().SetColumns(s => new bus_storage { use_num = storage.use_num, num = storage.num }).Where(w => w.std_item_id == c.std_item_id && w.org_id == userinfo.org_id && w.store_id == which_class.store_id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                        //查询库存明细
                        var storage_detials_list = Db.Queryable<bus_storage_detials>().Where(w => w.id == storage.id && w.spec == c.spec).OrderBy(o => o.expire_date).OrderBy(o => o.no).WithCache().ToList();

                        var d_num = Convert.ToInt32(c.num);
                        foreach (var storage_detials in storage_detials_list)
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

                                //新增发放表
                                AddGrant(entity, c, userinfo, grant_detial_list, storage_detials, this_num);
                                //新增出库表
                                AddOut(entity, c, userinfo, bus_out_storage_detials_List, storage_detials, this_num);

                                if (d_num == 0)
                                {
                                    break;
                                }
                            }
                        }

                        //修改领用状态
                        Db.Updateable<bus_requisitions_detail>()
                          .SetColumns(ur => ur.state == 15)
                          .Where(ur => ur.bill_no == entity.bill.bill_no && ur.spec == c.spec && ur.std_item_id == c.std_item_id)
                          .RemoveDataCache()
                          .EnableDiffLogEvent()
                          .ExecuteCommand();


                        //新增发放明细
                        Db.Insertable<bus_grant_detail>(grant_detial_list).ExecuteCommand();
                        redisCache.RemoveAll<bus_grant_detail>();

                        //新增出库明细

                        Db.Insertable(bus_out_storage_detials_List).ExecuteCommand();
                        redisCache.RemoveAll<bus_out_storage_detials>();
                    });

                }
                //固资
                if (which_class.type_id==1)
                {
                    var groupby = entity.bill_details.GroupBy(g=>new  { g.bill_no, g.std_item_id, g.spec }).Select(s=>new bus_requisitions_detail { bill_no= s.Key.bill_no, std_item_id= s.Key.std_item_id, spec=s.Key.spec }).ToList();

                    groupby.ForEach(g => {
                        //查询发放明细
                        var grant_det = Db.Queryable<bus_requisitions_detail>()
                                        .Where(brd => brd.bill_no == g.bill_no && brd.std_item_id == g.std_item_id && brd.spec == g.spec && brd.state != 15)
                                        .WithCache()
                                        .First();

                        var sl = entity.bill_details.Where(bd => bd.bill_no == g.bill_no && bd.std_item_id == g.std_item_id && bd.spec == g.spec).Count();

                        if (grant_det == null)
                        {
                            throw new MessageException("发放数据不匹配！");
                        }
                        if (grant_det.num != sl)
                        {
                            throw new MessageException("发放数量不匹配！");
                        }

                    });                   

                    entity.bill_details.ForEach(c =>
                    {
                        if (string.IsNullOrEmpty(c.no))
                        {
                            throw new MessageException("请选择设备编号！");
                        }

                        //查询固资状态
                        var asset_state = Db.Queryable<bus_assets>()
                                          .Where(a => a.org_id == userinfo.org_id && a.store_id == entity.bill.store_id&& a.no == c.no && a.std_item_id == c.std_item_id && a.spec == c.spec && a.manufactor_id == c.manufactor_id)
                                          .WithCache()
                                          .First();

                        if (asset_state == null || asset_state?.state!=30)
                        {
                            throw new MessageException($"{c.manufactor}没有可用物资");
                        }


                        //修固资明细
                        Db.Updateable<bus_assets>().SetColumns(s => s.state==31)
                        .Where(s=> s.id == asset_state.id)
                        .RemoveDataCache()
                        .EnableDiffLogEvent()
                        .ExecuteCommand();

                        //查询是否存在领用单
                        var have_grant = Db.Queryable<bus_grant_bill>()
                                         .Where(g => g.org_id == userinfo.org_id && g.store_id == entity.bill.store_id && g.realted_no == entity.bill.bill_no)
                                         .WithCache()
                                         .First();

                        //新增领用单
                        var grant_bill = new bus_grant_bill();
                        if (have_grant == null)
                        {
                            grant_bill.bill_no = "FFWZ" + userinfo.id + DateTime.Now.ToString("yyyyMMddHHmmssff");
                            grant_bill.creater = userinfo.name;
                            grant_bill.creater_id = userinfo.id;
                            grant_bill.create_time = DateTime.Now;
                            grant_bill.org_id = userinfo.org_id;
                            grant_bill.realted_no = entity.bill.bill_no;
                            grant_bill.store_id = entity.bill.store_id;
                            grant_bill.store_name = entity.bill.store_name;
                            grant_bill.type_id = entity.bill.type_id;
                            grant_bill.type = entity.bill.type;
                            grant_bill.state = 33;

                            Db.Insertable<bus_grant_bill>(grant_bill).ExecuteCommand();
                            redisCache.RemoveAll<bus_grant_bill>();

                        }

                        //新增发放明细
                        var gr_de = new bus_grant_detail();
                        gr_de.bill_no = have_grant == null ? grant_bill.bill_no : have_grant?.bill_no;
                        gr_de.storage_id = asset_state.id;
                        gr_de.storage_no = asset_state.no;
                        gr_de.std_item_id = asset_state.std_item_id;
                        gr_de.name = c.name;
                        gr_de.type_id = c.type_id;
                        gr_de.type = c.type;
                        gr_de.spec = c.spec;
                        gr_de.bill_num = 1;
                        gr_de.unit = c.unit;
                        gr_de.manufactor_id = c.manufactor_id;
                        gr_de.manufactor = c.manufactor;
                        gr_de.create_time = DateTime.Now;
                        gr_de.creater = userinfo.name;
                        gr_de.creater_id = userinfo.id;
                        gr_de.no = c.no;
                        gr_de.remark = c.remark;
                        gr_de.state = 15;

                        //新增发放明细
                        Db.Insertable<bus_grant_detail>(gr_de).ExecuteCommand();
                        redisCache.RemoveAll<bus_grant_detail>();


                        //修改领用状态
                        Db.Updateable<bus_requisitions_detail>()
                          .SetColumns(ur => ur.state == 15)
                          .Where(ur => ur.bill_no == entity.bill.bill_no && ur.spec == c.spec && ur.std_item_id == c.std_item_id)
                          .RemoveDataCache()
                          .EnableDiffLogEvent()
                          .ExecuteCommand();


                    });
                }

                //查询是否所有已完成
                var is_all = Db.Queryable<bus_requisitions_detail>()
                             .Where(rd => rd.bill_no == entity.bill.bill_no && rd.state == 30)
                             .WithCache()
                             .Any();

                if (is_all)
                {
                    Db.Updateable<bus_requisitions_bill>()
                      .SetColumns(brb => brb.state == 33)
                      .Where(brb => brb.bill_no == entity.bill.bill_no)
                      .RemoveDataCache()
                      .EnableDiffLogEvent()
                      .ExecuteCommand();
                }
                else
                {
                    Db.Updateable<bus_requisitions_bill>()
                      .SetColumns(brb => brb.state == 15)
                      .Where(brb => brb.bill_no == entity.bill.bill_no)
                      .RemoveDataCache()
                      .EnableDiffLogEvent()
                      .ExecuteCommand();

                    Db.Updateable<bus_grant_bill>()
                      .SetColumns(bgb => bgb.state == 15)
                      .Where(bgb => bgb.realted_no == entity.bill.bill_no)
                      .RemoveDataCache()
                      .EnableDiffLogEvent()
                      .ExecuteCommand();


                }

            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;

        }

        private void AddOut(RequisitionAddModel entity, NewDetail c, Tools.IdentityModels.GetUser.UserInfo userinfo, List<bus_out_storage_detials> bus_out_storage_detials_List, bus_storage_detials storage_detials, short this_num)
        {
            //查询是否存在出库单
            var have_grant = Db.Queryable<bus_out_storage>()
                             .Where(g => g.org_id == userinfo.org_id && g.store_id == entity.bill.store_id && g.realted_no == entity.bill.bill_no)
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
                out_bill.realted_no = entity.bill.bill_no;
                out_bill.remark = "领用出库";
                out_bill.state = 42;
                out_bill.store_id = entity.bill.store_id;
                out_bill.store_name = entity.bill.store_name;
                out_bill.type_id = 2;
                out_bill.type = "领用";

                Db.Insertable<bus_out_storage>(out_bill).ExecuteCommand();
                redisCache.RemoveAll<bus_out_storage>();
            }

            //生成出库单明细
            var out_storage_detials = new bus_out_storage_detials();

            out_storage_detials.approval_no = c.approval_no;
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

            bus_out_storage_detials_List.Add(out_storage_detials);
        }

        /// <summary>
        /// 新增发放数据
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="c"></param>
        /// <param name="userinfo"></param>
        /// <param name="grant_detial_list"></param>
        /// <param name="storage_detials"></param>
        /// <param name="this_num"></param>
        private void AddGrant(RequisitionAddModel entity, bus_requisitions_detail c, Tools.IdentityModels.GetUser.UserInfo userinfo, List<bus_grant_detail> grant_detial_list, bus_storage_detials storage_detials, short this_num)
        {
            //查询是否存在领用单
            var have_grant = Db.Queryable<bus_grant_bill>()
                             .Where(g => g.org_id == userinfo.org_id && g.store_id == entity.bill.store_id && g.realted_no == entity.bill.bill_no)
                             .WithCache()
                             .First();

            //新增领用单
            var grant_bill = new bus_grant_bill();
            if (have_grant == null)
            {
                grant_bill.bill_no = "FFWZ" + userinfo.id + DateTime.Now.ToString("yyyyMMddHHmmssff");
                grant_bill.creater = userinfo.name;
                grant_bill.creater_id = userinfo.id;
                grant_bill.create_time = DateTime.Now;
                grant_bill.org_id = userinfo.org_id;
                grant_bill.realted_no = entity.bill.bill_no;
                grant_bill.store_id = entity.bill.store_id;
                grant_bill.store_name = entity.bill.store_name;
                grant_bill.type_id = entity.bill.type_id;
                grant_bill.type = entity.bill.type;
                grant_bill.state = 33;

                Db.Insertable<bus_grant_bill>(grant_bill).ExecuteCommand();
                redisCache.RemoveAll<bus_grant_bill>();

            }

            //新增发放明细
            var gr_de = new bus_grant_detail();
            gr_de.bill_no = have_grant == null ? grant_bill.bill_no : have_grant?.bill_no;
            gr_de.storage_id = storage_detials.id;
            gr_de.storage_no = storage_detials.no;
            gr_de.std_item_id = storage_detials.std_item_id;
            gr_de.name = c.name;
            gr_de.type_id = c.type_id;
            gr_de.type = c.type;
            gr_de.spec = c.spec;
            gr_de.bill_num = this_num;
            gr_de.unit = c.unit;
            gr_de.manufactor_id = storage_detials.manufactor_id;
            gr_de.manufactor = storage_detials.manufactor;
            gr_de.create_time = DateTime.Now;
            gr_de.creater = userinfo.name;
            gr_de.creater_id = userinfo.id;
            gr_de.state = 15;

            grant_detial_list.Add(gr_de);
        }

        /// <summary>
        /// 所有发放记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Page<AllGrantModel>> AllGrantRecord(GrantSearch entity)
        {
            var userinfo = new Tools.IdentityModels.GetUser().userInfo;
            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }
 
            return await Db.Queryable<bus_grant_detail,bus_grant_bill,bus_requisitions_bill,bus_requisitions_detail>((a,b,c,d)=>new object[] {JoinType.Left,a.bill_no==b.bill_no,JoinType.Left,b.realted_no==c.bill_no,JoinType.Left,(c.bill_no==d.bill_no&&d.spec==a.spec) })
                           .Where((a,b,c,d)=>b.org_id==userinfo.org_id)
                           .WhereIF(entity.asset==2,(a,b,c,d)=>b.type_id==1)
                           .WhereIF(entity.asset != 2&&entity.store_select_id>0,(a,b,c,d)=>b.store_id==entity.store_select_id)
                           .WhereIF(entity.store_id>0,(a,b,c,d)=>b.store_id==entity.store_id)
                           .WhereIF(entity.asset!=2&& entity.type_id>0,(a,b,c,d)=>b.type_id==entity.type_id)
                           .WhereIF(entity.start_time != null, (a,b,c,d) => a.create_time >= entity.start_time)
                           .WhereIF(entity.end_time != null, (a,b,c,d) => a.create_time <= entity.end_time)
                           .WhereIF(!string.IsNullOrEmpty(entity.name),(a,b,c,d)=>a.creater.Contains(entity.name)||c.creater.Contains(entity.name))
                           .WhereIF(entity.state>0,(a,b,c,d)=>a.state==entity.state)
                           .Select((a,b,c,d)=>new AllGrantModel { return_time=a.return_time,returner=a.returner, store_name=b.store_name, type_id_det=a.type_id, type_det=a.type, no=a.no, state=a.state, bill_no=a.bill_no, bill_num=a.bill_num, creater=a.creater, creater_id=a.creater_id, create_time=a.create_time, manufactor=a.manufactor, manufactor_id=a.manufactor_id, name=a.name, re_name=c.creater, re_time=c.create_time, spec=a.spec, std_item_id=a.std_item_id, type=b.type, type_id=b.type_id, unit=a.unit, except_time=d.estimate_time })
                           .OrderBy(entity.order + orderTypeStr)
                           .WithCache()
                           .ToPageAsync(entity.page, entity.limit);




        }

        /// <summary>
        /// 获取固定资产记录（归还）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Page<AllGrantModel>> AllAssentGrantRecord(GrantSearch entity)
        {
            entity.asset = 2;
            return await AllGrantRecord(entity);
        }


        /// <summary>
        /// 获取供应商
        /// </summary>
        /// <param name="std_item_id"></param>
        /// <param name="spec"></param>
        /// <param name="store_id"></param>
        /// <returns></returns>
        public async Task<List<GetManufactor>> GetManufactors(string name,int std_item_id, string spec, int store_id)
        {
            var userinfo = new Tools.IdentityModels.GetUser().userInfo;

            //获取供应商
            var manList = await Db.Queryable<bus_assets,h_manufactor>((b,a)=>new object[] {JoinType.Left,b.manufactor_id==a.id })
                                .Where((b,a) => b.org_id == userinfo.org_id && b.store_id == store_id && b.state == 30 && b.std_item_id == std_item_id && b.spec == spec)
                                .WhereIF(!string.IsNullOrEmpty(name), (b, a) => b.manufactor.Contains(name)||a.spell.Contains(name.ToUpper()))
                                .GroupBy((b, a) => new { b.manufactor_id, b.manufactor })
                                .Select((b, a) => new GetManufactor { manufactor_id = b.manufactor_id, manufactor = b.manufactor })
                                .WithCache()
                                .ToListAsync();

            //获取编号
            var noList= await Db.Queryable<bus_assets>()
                                .Where(b => b.org_id == userinfo.org_id && b.store_id == store_id && b.state == 30 && b.std_item_id == std_item_id && b.spec == spec)
                                .Select(b =>new  { b.manufactor_id, b.manufactor,b.no })
                                .WithCache()
                                .ToListAsync();

            return manList.Select(m => new GetManufactor { manufactor = m.manufactor, manufactor_id = m.manufactor_id, no = noList.Where(n => n.manufactor_id == m.manufactor_id).Select(n => n.no).ToList() }).ToList();


        }

        /// <summary>
        /// 归还物资
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> ReturnRequisition(ReturnModel entity)
        {
            if (string.IsNullOrEmpty(entity.bill_no))
            {
                throw new MessageException("请选择单据！");
            }
            var userinfo = new Tools.IdentityModels.GetUser().userInfo;

            var result = await Db.Ado.UseTranAsync(() => {
                //查询单据
                var detail = Db.Queryable<bus_grant_detail>()
                               .Where(b => b.bill_no == entity.bill_no && b.std_item_id == entity.std_item_id && b.spec == entity.spec && b.manufactor_id == entity.manufactor_id && b.no == entity.no)
                               .WithCache()
                               .First();

                if (detail==null)
                {
                    throw new MessageException("归还数据不匹配！");
                }

                //修改固定资产状态
                Db.Updateable<bus_assets>()
                  .SetColumns(a => a.state == 30)
                  .Where(a => a.org_id == userinfo.org_id && a.no == detail.no && a.std_item_id == detail.std_item_id)
                  .RemoveDataCache()
                  .EnableDiffLogEvent()
                  .ExecuteCommand();

                //修改发放明细状态
                Db.Updateable<bus_grant_detail>()
                .SetColumns(g =>new bus_grant_detail {  state = 22, return_id=userinfo.id, return_time=DateTime.Now, returner=userinfo.name, re_remark=entity.remark  })
                .Where(g =>g.bill_no==entity.bill_no&& g.std_item_id == detail.std_item_id&&g.spec==detail.spec&&g.manufactor_id==detail.manufactor_id&&g.no==detail.no)
                .RemoveDataCache()
                .EnableDiffLogEvent()
                .ExecuteCommand();

                //查询关联申请单
               var relation_no= Db.Queryable<bus_grant_bill>()
                  .Where(gd => gd.bill_no == entity.bill_no)
                  .Select(gd=>gd.realted_no)
                  .WithCache()
                  .First();

                if (relation_no==null)
                {
                    throw new MessageException("未找到申请单据信息！");
                }
                //修改申请状态
                Db.Updateable<bus_requisitions_detail>()
                  .SetColumns(r => r.state == 22)
                  .Where(r => r.bill_no == relation_no && r.std_item_id == detail.std_item_id && r.spec == detail.spec)
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
    }
}
