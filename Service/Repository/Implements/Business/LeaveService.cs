using Models.DB;
using Models.View.Business;
using Service.Extensions;
using Service.Repository.Interfaces.Business;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;
using Tools.Filter;
using Tools.WebSocket;

namespace Service.Repository.Implements.Business
{
    /// <summary>
    /// 请假业务流程
    /// </summary>
    public class LeaveService : DbContext, ILeaveService
    {
        /// <summary>
        /// 获取请假流程
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<List<VerifyProcess>> GetVerifyProcess(VerifyProcessSearch entity)
        {
            if (entity.leave_type_id <= 0 || entity.duration <= 0)
            {
                throw new MessageException("未获取到请假信息!");
            }
            //获取当前用户信息
            var userinfo = new Tools.IdentityModels.GetUser().userInfo;
            if (entity.whichuser!=2&&entity.whichuser!=1)
            {
                throw new MessageException("未获取到用户!");
            }

            //获取用户所属部门
            var deptList = await Db.Queryable<p_employee_role, p_dept, p_role>((er, d, r) => new object[] { JoinType.Left, er.dept_id == d.id, JoinType.Left, er.role_id == r.id }).Where((er, d, r) => er.org_id == userinfo.org_id)
                .WhereIF(entity.whichuser==2, (er, d, r) => er.employee_id ==entity.user_id)
                .WhereIF(entity.whichuser==1, (er, d, r) => er.employee_id == userinfo.id)
                .Select((er, d, r) => new {d.name, er.store_id, er.is_admin, er.dept_id,d.link_id }).WithCache().ToListAsync();
            //机构数组
            List<int> deptOrgArry = new List<int>();
            //门店数组
            List<int> deptStoreArry = new List<int>();

            deptList.ForEach(c =>
            {
                //机构
                if (c.store_id == 0)
                {
                    deptOrgArry.Add(c.dept_id);
                }
                else
                {
                    if (c.link_id!=null&&c.link_id>0)
                    {
                        deptStoreArry.Add(c.link_id.Value);
                    }
                }

            });
            if (deptOrgArry.Count <= 0 && deptStoreArry.Count <= 0)
            {
                throw new MessageException("未查询到所属部门！");
            }

            //根据部门，时长，类型获取请假流程
            var processList = await Db.Queryable<p_process>()
                         .Where(p => p.org_id == userinfo.org_id && p.type_id == 4 && p.state == 1 && p.leave_type_id == entity.leave_type_id &&(deptOrgArry.Contains(p.dept_id) || deptStoreArry.Contains(p.dept_id)))
                         .WhereIF(entity.store_id>0,p=>p.store_id>0)
                         .WhereIF(entity.store_id==0,p=>p.store_id==0)
                         .OrderBy(p => new { p.dept_id, p.duration })
                         .WithCache()
                         .ToListAsync();

            //获取请假流程ids
            var processIds = processList.Select(ids => ids.id).ToList();

            //查询请假详情
            var processDetail = await Db.Queryable<p_process_detials>()
                                        .Where(pdd => processIds.Contains(pdd.id))
                                        .WithCache()
                                        .ToListAsync();



            if (processList.Count<=0)
            {
                throw new MessageException("请设置审核流程！");
            }

            var newProcessList = new List<p_process>();
            //查询对应时长的数据
            deptList.ForEach(i =>
            {
                if (i.dept_id>0)
                {
                    var dept_id = 0;
                    if (i.store_id>0)
                    {
                        if (i.link_id != null && i.link_id > 0)
                        {
                            dept_id = i.link_id.Value;
                        }
                    }
                    else
                    {
                        dept_id = i.dept_id;
                      
                    }
                   
                    var process = processList
                    .Where(w => w.dept_id == dept_id && w.duration <= entity.duration)
                    .OrderByDescending(o => o.duration).FirstOrDefault();
                    if (process == null)
                    {
                        process = processList.Where(w => w.dept_id == dept_id && w.duration > entity.duration).OrderBy(o => o.duration).FirstOrDefault();
                    }
                    if (process==null)
                    {
                        throw new MessageException($"请{i.name}设置请假流程信息！");
                    }

                    newProcessList.Add(process);
                }
                
            });

            return newProcessList.Select(s => new VerifyProcess { is_org = s.store_id == 0 ? true : false, dept_name = s.dept_name, process_id = s.id, VerifyProcessDetail = processDetail.Where(w => w.id == s.id).Select(w => new VerifyProcessDetail { no=w.id, level = w.level, name = w.employee, role_name=w.role_name, is_org=w.is_org.Value  }).ToList() }).ToList();

        }

        /// <summary>
        /// 申请请假
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<bool> AddLeavel(AddLeavelModel entity,dynamic type)
        {
            if (entity.leave_type_id <= 0 || entity.duration < 0||entity.store_id<0||entity.apply_start_time==null||entity.apply_end_time==null)
            {
                throw new MessageException("请假信息填写错误！");
            }
            //通知
            var notice = new Business.NoticeService();
            var noticeList = new List<AddNoticeModel>();
            var notice_content = "";


            //获取当前用户信息
            var userinfo = new Tools.IdentityModels.GetUser().userInfo;

            if (entity.leave_process_list.Count>0)
            {
                var result = await Db.Ado.UseTranAsync(() =>
                {
                    //查询所有流程
                    var processList = Db.Queryable<p_process>()
                                        .Where(p => entity.leave_process_list.Contains(p.id))
                                        .WithCache()
                                        .ToList();


                var roleList = new List<int>();
                //查询当前登录门店所有角色
                roleList = GetRole(entity, userinfo, roleList);

                //根据流程id查询流程明细
                var processDetails = Db.Queryable<p_process_detials>()
                                           .Where(pd => entity.leave_process_list.Contains(pd.id))
                                           .WithCache()
                                           .ToList();


                //应该查询的等级
                var levelProcess = 1;

                
                    var newEntityList = new List<oa_leave>();
                    //循环
                    var xh = 0;
                    processList.ForEach(c =>
                    {
                        var thisEntity = new oa_leave();
                        thisEntity.org_id = userinfo.org_id;
                        thisEntity.store_id = entity.store_id;
                        thisEntity.store = entity.store_name;
                        thisEntity.leave_cause = entity.leave_cause;
                        thisEntity.applicant_id = userinfo.id;
                        thisEntity.applicant = userinfo.name;
                        thisEntity.apply_time = DateTime.Now;
                        thisEntity.leave_type_id = entity.leave_type_id;
                        thisEntity.leave_type = entity.leave_type;
                        thisEntity.duration = entity.duration;
                        thisEntity.apply_start_time = entity.apply_start_time;
                        thisEntity.apply_end_time = entity.apply_end_time;
                        thisEntity.state = 26;
                        thisEntity.leave_no = "QJSQ" + c.id + userinfo.id + DateTime.Now.ToString("yyyyMMddHHmmss") + xh;

                        var prd = new p_process_detials();
                        //获取审核流程
                        var  newprocess_detail = GetProcess(entity, c, roleList, processDetails, levelProcess, ref prd,thisEntity.leave_no, userinfo);
                        prd = newprocess_detail.p_Process_Detials;
                        if (prd == null)
                        {
                            throw new MessageException("没有查询到待审核人!");
                        }


                        
                        thisEntity.org_process_id = c.id;
                        thisEntity.total_level += c.total_level;
                        thisEntity.level = Convert.ToInt16(newprocess_detail.processleave);
                        thisEntity.await_verifier_id = prd.employee_id == 0 ? 0 : prd.employee_id;
                        thisEntity.await_verifier = prd.employee == null ? " " : prd.employee;
                        thisEntity.role_id = prd.employee_id == 0 ? prd.role_id : 0;
                        thisEntity.role_name = prd.employee_id == 0 ? prd.role_name : " ";
                        thisEntity.is_org = prd.is_org;

                        if (prd != null)
                        {
                            //设置请假通知
                            SetLeaveNotice(entity, notice, noticeList, notice_content, userinfo, thisEntity);
                        }

                        if (entity.leaveImg.Count > 0)
                        {
                            //设置图片
                            SetAsync(thisEntity.leave_no, entity.leaveImg, type);
                        }
                        newEntityList.Add(thisEntity);
                        xh++;

                    });
                    Db.Insertable(newEntityList).ExecuteCommand();
                    redisCache.RemoveAll<oa_leave>();
                });
                if (!result.IsSuccess)
                {
                    throw new MessageException(result.ErrorMessage);
                }
                return result.IsSuccess;

            }
            //没有审批流程直接通过
            else
            {
                //新增请假记录
                var newEntity = new oa_leave();
                newEntity.org_id = userinfo.org_id;
                newEntity.store_id = entity.store_id;
                newEntity.store = entity.store_name;
                newEntity.leave_cause = entity.leave_cause;
                newEntity.applicant_id = userinfo.id;
                newEntity.applicant = userinfo.name;
                newEntity.apply_time = DateTime.Now;
                newEntity.leave_type_id = entity.leave_type_id;
                newEntity.leave_type = entity.leave_type;
                newEntity.duration = entity.duration;
                newEntity.apply_start_time = entity.apply_start_time;
                newEntity.apply_end_time = entity.apply_end_time;
                newEntity.leave_no = "QJSQ" + userinfo.id + DateTime.Now.ToString("yyyyMMddHHmmss");
                newEntity.state = 34;

                var result = await Db.Ado.UseTranAsync(() => {
                    //设置图片
                      if (entity.leaveImg.Count>0)
                        {
                            //设置图片
                            SetAsync(newEntity.leave_no, entity.leaveImg, type);
                        }
                       

                    Db.Insertable(newEntity).ExecuteCommand();
                    redisCache.RemoveAll<oa_leave>();

                });
                if (!result.IsSuccess)
                {
                    throw new MessageException(result.ErrorMessage);
                }
                return result.IsSuccess;
            }
            

        }

        /// <summary>
        /// 查询当前登录门店所有角色
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userinfo"></param>
        /// <param name="roleList"></param>
        /// <returns></returns>
        private List<int> GetRole(AddLeavelModel entity, Tools.IdentityModels.GetUser.UserInfo userinfo, List<int> roleList)
        {
            if (entity.store_id > 0)
            {
                roleList = Db.Queryable<p_role>()
                                 .Where(s => s.org_id == userinfo.org_id && s.store_id == entity.store_id && s.disabled_code == 1)
                                 .Select(s => s.link_id.Value)
                                 .WithCache()
                                 .ToList();
            }
            else
            {
                roleList = Db.Queryable<p_role>()
                                .Where(s => s.org_id == userinfo.org_id && s.store_id == 0 && s.disabled_code == 1)
                                .Select(s => s.id)
                                .WithCache()
                                .ToList();
            }

            return roleList;
        }

        /// <summary>
        /// 获取请假待审核人
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="c"></param>
        /// <param name="roleList"></param>
        /// <param name="processDetails"></param>
        /// <param name="levelProcess"></param>
        /// <param name="prd"></param>
        /// <returns></returns>
        private NewProcessModel GetProcess(AddLeavelModel entity, p_process c, List<int> roleList, List<p_process_detials> processDetails,  int levelProcess, ref p_process_detials prd,string leave_no, Tools.IdentityModels.GetUser.UserInfo userInfo)
        {
            //如果走门店流程，可能跳过没有的部门
            if (entity.store_id > 0)
            {
                var newprdList = processDetails.Where(s => s.id == c.id).ToList();
                r_verify newVerify = new r_verify();
                r_verify_detials newVerifyDetail = new r_verify_detials();
                foreach (var item in newprdList)
                {
                    if (item.is_org == false)
                    {
                        prd = processDetails.Where(s => s.id == item.id && s.level == item.level && roleList.Contains(s.role_id)).FirstOrDefault();
                        if (prd == null)
                        {
                            //审核记录
                            CommonModel model = new CommonModel();
                            model.total_level = c.total_level;
                            model.level = item.level;
                            model.store_id = entity.store_id;
                            model.store = entity.store_name;
                            model.applicant_id = userInfo.id;
                            model.applicant = userInfo.name;
                            model.process_type_id = 4;
                            model.process_type = "请假";

                            ApprovalLeaveModel aentity = new ApprovalLeaveModel();
                            aentity.apply_no = leave_no;
                            aentity.approval_state = 34;
                            aentity.verify_remark = " ";
                            var ver = new ApprovalService();

                            var process_currentMes = Db.Queryable<p_process_detials, p_process>((p, pr) => new object[] { JoinType.Left, p.id == pr.id })
                                        .Where((p, pr) => p.id == c.id && p.level == item.level)
                                        .Select((p, pr) => new process_currentMesModel { dept_id = p.dept_id, role_id = p.role_id, employee_id = p.employee_id.Value, dept_name = p.dept_name, role_name = p.role_name, id = p.id, name = pr.name, leave_type_id = pr.leave_type_id, leave_type = pr.leave_type })
                                        .WithCache()
                                        .First();
                            if (process_currentMes == null)
                            {
                                throw new MessageException("未获取到当前审核流程信息");
                            }

                            //添加审核记录                               
                            ver.SetVerify(aentity, process_currentMes, userInfo, item.level, model,2,prd);


                            levelProcess += 1;
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        prd = processDetails.Where(s => s.id == item.id && s.level == levelProcess).FirstOrDefault();
                    }

                }

            }
            //机构存在所有的部门
            else
            {
                prd = processDetails.Where(s => s.id == c.id && s.level == levelProcess).FirstOrDefault();
            }

            return new NewProcessModel{ p_Process_Detials=prd, processleave= Convert.ToInt16(levelProcess) };
        }

        /// <summary>
        /// 请假通知
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="notice"></param>
        /// <param name="noticeList"></param>
        /// <param name="notice_content"></param>
        /// <param name="userinfo"></param>
        /// <param name="thisEntity"></param>
        private void SetLeaveNotice(AddLeavelModel entity, NoticeService notice, List<AddNoticeModel> noticeList, string notice_content, Tools.IdentityModels.GetUser.UserInfo userinfo, oa_leave thisEntity)
        {
            //查询此角色下面的人员    
            var rolenotice = Db.Queryable<p_employee_role, p_role, p_employee>((a, b, e) => new object[] { JoinType.Left, a.role_id == b.id, JoinType.Left, e.id == a.employee_id })
                                   .Where((a, b, e) => b.disabled_code == 1)
                                   .WhereIF(thisEntity.is_org == false, (a, b, e) => b.link_id == thisEntity.role_id && a.store_id > 0)
                                   .WhereIF(thisEntity.is_org == true, (a, b, e) => b.id == thisEntity.role_id && a.store_id == 0)
                                   .Select((a, b, e) => new { e.id, e.name })
                                   .WithCache()
                                   .ToList();

            if (rolenotice.Count > 0)
            {               
                //请假通知
                notice_content = $"{{\"name\":\"{userinfo.name}\",\"leave_type\":\"{thisEntity.leave_type}\",\"date\":\"{thisEntity.apply_start_time.ToString("yyyy-MM-dd")}至{thisEntity.apply_end_time.ToString("yyyy-MM-dd")}\",\"duration\":\"{thisEntity.duration}\",\"leave_cause\":\"{thisEntity.leave_cause}\",\"no\":\"{thisEntity.leave_no}\",\"msg\":\" 请假申请已提交\"}}";
                var archives = new c_archives();
                archives.id = userinfo.id;
                archives.name = userinfo.name;
                archives.phone = userinfo.phone_no;
                var employeenotice = new List<employeeMes>();
                var employeeSocket = new List<WebSocketModel>();
                rolenotice.ForEach(r =>
                {
                    var con = $"{{\"name\":\"{userinfo.name}\",\"msg\":\"提交了请假申请，请处理！\"}}";
                    employeeSocket.Add(new WebSocketModel { userId = r.id, content = con });

                    employeenotice.Add(new employeeMes { employee_id = r.id, employee = r.name });
                 
                });
                notice.NewMethod(thisEntity.leave_no, archives, entity.store_id, notice, noticeList, 3, 1, notice_content, userinfo.name, employeenotice);

                //新增
                notice.AddNotice(noticeList);
                //消息提醒
                ChatWebSocketMiddleware.SendListAsync(employeeSocket);


            }
        }

        /// <summary>
        /// 设置图片
        /// </summary>
        /// <param name="image"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public bool SetAsync(string leave_no, List<string> image, dynamic type)
        {
           
            string currentDirectory = Path.GetDirectoryName(type.Assembly.Location);

            var newVs = image;//传过来的图片
            var list = Db.Queryable<oa_leave_image>().Where(w => w.leave_no == leave_no && !newVs.Contains(w.img_url)).WithCache().ToList();//需要删除的
            foreach (var item in list)
            {
                var url = currentDirectory + "/wwwroot/" + item.img_url.Trim();
                if (System.IO.File.Exists(url))
                {
                    System.IO.File.Delete(url);//删除无用文件
                }
            }
            var newImage = new List<oa_leave_image>();
            for (int i = 0; i < newVs.Count; i++)
            {
                //新增
                newImage.Add(new oa_leave_image { leave_no = leave_no, img_url = newVs[i] });
            }
            var ret = -1;
            ret =  Db.Deleteable<oa_leave_image>().Where(w => w.leave_no == leave_no).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
            if (ret != -1)
            {
                 Db.Insertable(newImage).ExecuteCommand();
                redisCache.RemoveAll<oa_leave_image>();
            }
            return true;
        }

        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="leave_no"></param>
        /// <returns></returns>
        public async Task<List<oa_leave_image>> GetAsync(string leave_no)
        {
            return await Db.Queryable<oa_leave_image>().Where(w => w.leave_no == leave_no).WithCache().ToListAsync();
        }

        /// <summary>
        /// 获取请假记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Page<LeaveRecord>> GetLeaveRecord(LeaveRecordSearch entity)
        {
            //获取当前用户信息
            var userinfo = new Tools.IdentityModels.GetUser().userInfo;

            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }

            var start = !string.IsNullOrEmpty(entity.apply_start_time) ? DateTime.Parse(entity.apply_start_time) : DateTime.Parse("1949-01-01");
            var end = !string.IsNullOrEmpty(entity.apply_end_time) ? DateTime.Parse(entity.apply_end_time) : DateTime.Parse("3000-01-01");
           

            //查询请假数据
            var leaveList = await Db.Queryable<oa_leave,p_employee>((o,p)=>new object[] {JoinType.Left,o.applicant_id==p.id})
                                  .Where((o, p) => o.org_id == userinfo.org_id&& SqlFunc.Between(o.apply_time, start, end))
                                  .WhereIF(entity.store_id > 0, (o, p) => o.store_id == entity.store_id)
                                  .WhereIF(entity.select_store_id > 0, (o, p) => o.store_id == entity.select_store_id)
                                  .WhereIF(!string.IsNullOrEmpty(entity.name), (o, p) => o.applicant.Contains(entity.name) || o.leave_no.Contains(entity.name))
                                  .WhereIF(entity.leave_type_id > 0, (o, p) => o.leave_type_id == entity.leave_type_id)
                                  .WhereIF(entity.state > 0, (o, p) => o.state == entity.state)
                                  .Select((o, p) => new LeaveRecord { phone=p.phone_no, apply_time=o.apply_time, apply_start_time=o.apply_start_time, applicant=o.applicant, applicant_id=o.applicant_id, apply_end_time=o.apply_end_time, await_verifier=o.await_verifier, await_verifier_id=o.await_verifier_id, duration=o.duration, leave_cause=o.leave_cause, leave_no=o.leave_no, leave_type=o.leave_type, leave_type_id=o.leave_type_id, level=o.level, org_id=o.org_id,  org_process_id=o.org_process_id,  state=o.state, store=o.store, store_id=o.store_id,total_level=o.total_level, verifier=o.verifier, verifier_id=o.verifier_id, verify_remark=o.verify_remark, verify_time=o.verify_time })
                                  .OrderBy(entity.order + orderTypeStr)
                                  .WithCache()
                                  .ToPageAsync(entity.page, entity.limit);

            var ImgList = leaveList.Items.Select(ss => ss.leave_no).ToList();

            //查询请假图片
            var newItem =await Db.Queryable<oa_leave_image>()
                                 .Where(s => ImgList.Contains(s.leave_no))
                                 .WithCache()
                                 .ToListAsync();

            leaveList.Items = leaveList.Items.Select(sss => new LeaveRecord { phone=sss.phone, apply_time = sss.apply_time, apply_start_time = sss.apply_start_time, applicant = sss.applicant, applicant_id = sss.applicant_id, apply_end_time = sss.apply_end_time, await_verifier = sss.await_verifier, await_verifier_id = sss.await_verifier_id, duration = sss.duration, leave_cause = sss.leave_cause, leave_no = sss.leave_no, leave_type = sss.leave_type, leave_type_id = sss.leave_type_id, level = sss.level, org_id = sss.org_id,  org_process_id = sss.org_process_id, state = sss.state, store = sss.store, store_id = sss.store_id, total_level = sss.total_level, verifier = sss.verifier, verifier_id = sss.verifier_id, verify_remark = sss.verify_remark, verify_time = sss.verify_time, leaveImg = newItem.Where(w => w.leave_no == sss.leave_no).ToList() }).ToList();
            return leaveList;
        }

        /// <summary>
        /// 获取请假流程
        /// </summary>
        /// <param name="leaveDetail"></param>
        /// <returns></returns>
        public async Task<List<VerifyProcess>> GetVerifyProcessDetail(oa_leave leaveDetail)
        {
            if (leaveDetail==null)
            {
                throw new MessageException("未获取到请假信息!");
            }
            //获取当前用户信息
            var userinfo = new Tools.IdentityModels.GetUser().userInfo;
          

            var newProcessList = new List<p_process>();
            //查询对应时长的数据
            newProcessList = await Db.Queryable<p_process>()
                                     .Where(p => p.org_id == userinfo.org_id)
                                     .WhereIF(leaveDetail.org_process_id > 0, p => p.id == leaveDetail.org_process_id)
                                     .WithCache()
                                     .ToListAsync();


            var ids = newProcessList.Select(s => s.id).ToList();

               // 查询流程详情
            var processDetail = await Db.Queryable<p_process_detials>()
                                   .Where(pdd => ids.Contains(pdd.id))
                                   .WithCache()
                                   .ToListAsync();



            return newProcessList.Select(s => new VerifyProcess { is_org = s.store_id == 0 ? true : false, dept_name = s.dept_name, process_id = s.id, VerifyProcessDetail = processDetail.Where(w => w.id == s.id).Select(w => new VerifyProcessDetail { no = w.id, level = w.level, name = w.employee, role_name = w.role_name, is_org=w.is_org.Value }).ToList() }).ToList();

        }
    }
}
