using Models.DB;
using Models.View.Business;
using Service.Repository.Interfaces.Business;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;
using Tools.Filter;
using static Tools.IdentityModels.GetUser;

namespace Service.Repository.Implements.Business
{
    /// <summary>
    /// 流程管理业务
    /// </summary>
    public class ProcessService : DbContext, IProcessService
    {
        private UserInfo userInfo = new Tools.IdentityModels.GetUser().userInfo;

        #region Common
        /// <summary>
        /// 各种流程新增,门店管理员标志
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> AddProcess(AddProcessModel entity)
        {
            if (entity.type_id <= 0)
            {
                throw new MessageException("请选择流程类型!");
            }

            switch (entity.type_id)
            {
                //采购
                case 1: return await AddBuyProcess(entity);
                //调拨
                case 2: return await AddTransferProcess(entity);
                //领用
                case 3: return await SetConsumer(entity);
                //请假
                case 4: return await SetLeavel(entity);
                //报废
                case 5: return await SetScrap(entity);
                //报损
                case 6:return await SetFaulty(entity);

                default:
                    return false;
                    
            }
           
        }


        /// <summary>
        /// 删除流程
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> DeleteProcess(DeleteProcessModel entity)
        {
            if (entity.id <= 0)
            {
                throw new MessageException("请选择流程!");
            }

            //查询流程
            var process_message = await Db.Queryable<p_process>()
                                      .Where(ps => ps.id == entity.id && ps.org_id == userInfo.org_id)
                                      .Select(ps => new { ps.state, ps.type_id, ps.leave_type_id, ps.duration })
                                      .WithCache()
                                      .FirstAsync();
            if (process_message.state == 1)
            {
                throw new MessageException("启用中的流程无法删除!");
            }
            var process = false;
            //查询流程是否使用中
            process = await Db.Queryable<r_verify>()
                                      .Where(mp => mp.org_id == userInfo.org_id && mp.state == 33 && mp.process_id == entity.id)
                                      .WithCache()
                                      .AnyAsync();
            if (process)
            {
                throw new MessageException("使用中的流程无法删除!");
            }
            //作废流程单
            return await Db.Updateable<p_process>()
              .SetColumns(s => s.state == 32)
              .Where(s => s.id == entity.id && s.org_id == userInfo.org_id)
              .RemoveDataCache()
              .EnableDiffLogEvent()
              .ExecuteCommandAsync();

        }
        /// <summary>
        /// 启用禁用流程
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> ProcessEnable(EnableProcessModel entity)
        {
            if (entity.id <= 0)
            {
                throw new MessageException("请选择流程!");
            }

            var result = await Db.Ado.UseTranAsync(() =>
            {
                //查询流程信息
                var process_message = Db.Queryable<p_process>()
                                     .Where(ps => ps.id == entity.id && ps.org_id == userInfo.org_id)
                                     .WithCache()
                                     .First();
            if (process_message == null)
            {
                throw new MessageException("未查询到流程信息!");
            }
            var process = false;
            //查询流程是否使用中
            process = Db.Queryable<r_verify>()
                                      .Where(mp => mp.org_id == userInfo.org_id && mp.state == 33 && mp.process_id == entity.id)
                                      .WithCache()
                                      .Any();
            if (process)
            {
                throw new MessageException("使用中的流程无法禁用!");
            }


           
                if (entity.state==1)
                {
                    //启用一个禁用其他
                    NewMethod(entity.state, entity.is_org, process_message);
                }               

                //编辑流程
                var is_success = Db.Updateable<p_process>().SetColumns(p => new p_process { state = entity.state })
                                              .Where(p => p.id == entity.id && p.org_id == userInfo.org_id)
                                              .RemoveDataCache()
                                              .EnableDiffLogEvent()
                                              .ExecuteCommand();


            });
             
           
            return result.IsSuccess;
        }

        private void NewMethod(short state,bool is_org, p_process process_message)
        {
            //调拨
            if (process_message.type_id == 2)
            {
                
                    if (is_org == false)
                    {
                        Db.Updateable<p_process>().SetColumns(p => p.state == 0)
                                                                   .Where(p => p.org_id == userInfo.org_id && p.store_id > 0 && p.type_id == process_message.type_id&&p.state!=32)
                                                                   .RemoveDataCache()
                                                                   .EnableDiffLogEvent()
                                                                   .ExecuteCommand();

                    }
                    else
                    {
                        Db.Updateable<p_process>().SetColumns(p => p.state == 0)
                                                                   .Where(p => p.org_id == userInfo.org_id && p.store_id == 0 && p.type_id == process_message.type_id && p.state != 32)
                                                                   .RemoveDataCache()
                                                                   .EnableDiffLogEvent()
                                                                   .ExecuteCommand();

                    }
                
            }
            //领用
            if (process_message.type_id == 3)
            {               
                    if (is_org == false)
                    {
                        Db.Updateable<p_process>().SetColumns(p => p.state == 0)
                                                                   .Where(p => p.org_id == userInfo.org_id && p.store_id > 0 && p.type_id == process_message.type_id && p.state != 32&&p.leave_type_id==process_message.leave_type_id)
                                                                   .RemoveDataCache()
                                                                   .EnableDiffLogEvent()
                                                                   .ExecuteCommand();

                    }
                    else
                    {
                        Db.Updateable<p_process>().SetColumns(p => p.state == 0)
                                                                   .Where(p => p.org_id == userInfo.org_id && p.store_id == 0 && p.type_id == process_message.type_id && p.state != 32 && p.leave_type_id == process_message.leave_type_id)
                                                                   .RemoveDataCache()
                                                                   .EnableDiffLogEvent()
                                                                   .ExecuteCommand();

                    }
            }
            //报废
            if (process_message.type_id==5||process_message.type_id==6)
            {
                if (is_org == false)
                {
                    Db.Updateable<p_process>().SetColumns(p => p.state == 0)
                                                               .Where(p => p.org_id == userInfo.org_id && p.store_id > 0 && p.type_id == process_message.type_id && p.state != 32 )
                                                               .RemoveDataCache()
                                                               .EnableDiffLogEvent()
                                                               .ExecuteCommand();

                }
                else
                {
                    Db.Updateable<p_process>().SetColumns(p => p.state == 0)
                                                               .Where(p => p.org_id == userInfo.org_id && p.store_id == 0 && p.type_id == process_message.type_id && p.state != 32 )
                                                               .RemoveDataCache()
                                                               .EnableDiffLogEvent()
                                                               .ExecuteCommand();

                }
            }
        }

        /// <summary>
        /// 新增流程到数据库
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="processEntity"></param>
        private void NewProcessAdd(AddProcessModel entity, p_process processEntity)
        {
            var processId = Db.Insertable(processEntity).ExecuteReturnIdentity();
            redisCache.RemoveAll<p_process>();

            //新增流程详情
            if (entity.detailsList.Count > 0)
            {
                var newDetails = entity.detailsList.Select(d => new p_process_detials { id = processId, dept_id = d.dept_id, dept_name = d.dept_name, employee = d.employee, employee_id = d.employee_id, level = d.level, role_id = d.role_id, role_name = d.role_name, is_org = d.is_org }).ToList();

                Db.Insertable(newDetails).ExecuteCommand();
                redisCache.RemoveAll<p_process_detials>();
            }
        }

        /// <summary>
        /// 编辑共用
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="process_message"></param>
        /// <returns></returns>
        private async Task<bool> ModifyCommon(ModifyProcessModel entity, p_process process_message)
        {
            var result = await Db.Ado.UseTranAsync(() =>
            {
                //使用中的流程无法编辑,查询审核记录
                var modifyProcess = Db.Queryable<r_verify>()
                                       .Where(mp => mp.org_id == userInfo.org_id && mp.process_type_id == process_message.type_id && mp.state == 33 && mp.process_id == entity.id)
                                       .WhereIF(entity.dept_id > 0, mp => mp.dept_id == entity.dept_id)
                                       .WhereIF(entity.is_org == false, mp => mp.store_id > 0)
                                       .WhereIF(entity.is_org == true, mp => mp.store_id == 0)
                                       .WithCache()
                                       .Any();
            if (modifyProcess)
            {
                throw new MessageException("仍有审核进行中，无法编辑！");
            }


            var processEntity = new p_process();
            processEntity.spell = ToSpell.GetFirstPinyin(entity.name);
            processEntity.dept_id = process_message.type_id == 4 ? entity.dept_id : 0;
            processEntity.dept_name = process_message.type_id == 4 ? entity.dept_name : " ";
            processEntity.enabler = entity.state == 1 ? userInfo.name : " ";
            processEntity.enabler_id = entity.state == 1 ? userInfo.id : 0;
            processEntity.enable_time = entity.state == 1 ? DateTime.Now : Convert.ToDateTime(" 3000-12-31 23:59:59");
            processEntity.total_level = Convert.ToInt16(entity.detailsList.Count > 0 ? entity.detailsList.Count : 0);

            //查询是否有相同流程
            var sameProcess = Db.Queryable<p_process>()
                                     .Where(sp => sp.org_id == userInfo.org_id && sp.type_id == process_message.type_id && sp.id != entity.id && sp.name == entity.name.Trim())
                                     .WhereIF(process_message.type_id == 1, sp => sp.use_money == entity.use_money)
                                     .WhereIF(process_message.type_id == 2, sp => sp.duration == entity.duration)
                                     .WhereIF(entity.dept_id > 0, sp => sp.dept_id == entity.dept_id)
                                     .WhereIF(entity.is_org == false, mp => mp.store_id > 0)
                                     .WhereIF(entity.is_org == true, mp => mp.store_id == 0)
                                     .WithCache()
                                     .Any();
            if (sameProcess)
            {
                throw new MessageException("已存在此流程!");
            }

           
                if (entity.state == 1&&process_message.type_id!=1)
                {
                    //(调拨)启用一个禁用其他
                    NewMethod(entity.state, entity.is_org, process_message);
                }

                //编辑流程
                Db.Updateable(processEntity).SetColumns(p => new p_process { use_money = entity.use_money, name = entity.name, spell = processEntity.spell, remark = entity.remark, dept_id = processEntity.dept_id, dept_name = processEntity.dept_name, state = entity.state, enabler = processEntity.enabler, enabler_id = processEntity.enabler_id, enable_time = processEntity.enable_time, total_level = processEntity.total_level })
                                                  .Where(p => p.id == entity.id && p.org_id == userInfo.org_id)
                                                  .RemoveDataCache()
                                                  .EnableDiffLogEvent()
                                                  .ExecuteCommand();


                //新增流程详情
                if (entity.detailsList.Count > 0)
                {
                    //删除之前流程
                    Db.Deleteable<p_process_detials>()
                              .Where(w => w.id == entity.id)
                              .RemoveDataCache()
                              .EnableDiffLogEvent()
                              .ExecuteCommand();

                    //新增详情
                    var newDetails = entity.detailsList.Select(d => new p_process_detials { id = entity.id, dept_id = d.dept_id, dept_name = d.dept_name, employee = d.employee, employee_id = d.employee_id, level = d.level, role_id = d.role_id, role_name = d.role_name, is_org = d.is_org }).ToList();

                    Db.Insertable(newDetails).ExecuteCommand();
                    redisCache.RemoveAll<p_process_detials>();
                }

            });

            return result.IsSuccess;
        }

        /// <summary>
        /// 流程分页数据，除开请假
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<List<ProcessModel>> GetProcessAsync(ProcessSearchModel entity)
        {
            if (entity.type_id <= 0)
            {
                throw new MessageException("未获取类型!");
            }
            //查询流程
            var processList = await Db.Queryable<p_process>()
                                    .Where(p => p.org_id == userInfo.org_id && p.type_id == entity.type_id && p.state != 32)
                                    .WhereIF(entity.store_id > 0, p => p.store_id == entity.store_id)
                                    .WhereIF(entity.is_org == false, p => p.store_id > 0)
                                    .WhereIF(entity.is_org == true, p => p.store_id == 0)
                                    .WhereIF(!String.IsNullOrEmpty(entity.name), p => p.name.Contains(entity.name.Trim()))
                                    .OrderBy(p=>p.create_time,OrderByType.Desc)
                                    .OrderBy(p=>p.state,OrderByType.Desc)
                                    .WithCache()
                                    .ToListAsync();

            var processIds = processList.Select(s => s.id).ToList();

            //查询流程详情
            var detailsList = await Db.Queryable<p_process_detials>()
                                   .Where(pdd => processIds.Contains(pdd.id))
                                   .WithCache()
                                   .ToListAsync();

            return processList.Select(w => new ProcessModel { process = w, Details = detailsList.Where(pd => pd.id == w.id).ToList() }).ToList();



        }

        #endregion

        #region 采购流程

        /// <summary>
        /// 添加采购流程
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> AddBuyProcess(AddProcessModel entity)
        {
            if (entity.use_money < 0)
            {
                throw new MessageException("请输入正确金额!");
            }
            var result = await Db.Ado.UseTranAsync(() =>
            {
                //查询是否有相同流程
                var sameProcess = Db.Queryable<p_process>()
                                         .Where(sp => sp.org_id == userInfo.org_id && sp.type_id == entity.type_id && sp.name == entity.name.Trim() && sp.use_money == entity.use_money)
                                         .WhereIF(entity.dept_id > 0, sp => sp.dept_id == entity.dept_id)
                                         .WhereIF(entity.is_org == false, sp => sp.store_id > 0)
                                         .WhereIF(entity.is_org == true, sp => sp.store_id == 0)
                                         .WithCache()
                                         .Any();
            if (sameProcess)
            {
                throw new MessageException("已存在此流程!");
            }

            var processEntity = new p_process();
            processEntity.use_money = entity.use_money;
            processEntity.name = entity.name;
            processEntity.spell = ToSpell.GetFirstPinyin(entity.name);
            processEntity.org_id = userInfo.org_id;
            processEntity.remark = entity.remark;
            processEntity.creater = userInfo.name;
            processEntity.creater_id = userInfo.id;
            processEntity.create_time = DateTime.Now;
            processEntity.dept_id = entity.type_id == 4 ? entity.dept_id : 0;
            processEntity.dept_name = entity.type_id == 4 ? entity.dept_name : " ";
            if (entity.state == 1)
            {
                processEntity.enabler = userInfo.name;
                processEntity.enabler_id = userInfo.id;
                processEntity.enable_time = DateTime.Now;
            }
            processEntity.state = entity.state;
            processEntity.type = entity.type_name;
            processEntity.type_id = entity.type_id;
            processEntity.total_level = Convert.ToInt16(entity.detailsList.Count > 0 ? entity.detailsList.Count : 0);


           

                processEntity.store_id = entity.is_org == true ? 0 : 99;
                processEntity.store_name = "无";
                //新增流程
                NewProcessAdd(entity, processEntity);

            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;

        }

    
        /// <summary>
        /// 编辑流程
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> ModifyProcess(ModifyProcessModel entity)
        {
            if (entity.id <= 0)
            {
                throw new MessageException("未选择流程！");
            }

            //查询当前流程类型
            var process_message = await Db.Queryable<p_process>()
                                .Where(s => s.org_id == userInfo.org_id && s.id == entity.id)
                                .Select(s => new p_process { type_id = s.type_id, store_id = s.store_id, leave_type_id = s.leave_type_id, duration = s.duration, dept_id = s.dept_id })
                                .WithCache()
                                .FirstAsync();
            if (process_message==null)
            {
                throw new MessageException("未获取到流程！");
            }

            switch (process_message.type_id)
            {
                //采购
                case 1: return await ModifyCommon(entity, process_message);
                //调拨
                case 2: return await ModifyCommon(entity, process_message);
                //领用
                case 3: return await ModifyConsumer(entity, process_message);
                //请假
                case 4:
                    if (process_message.leave_type_id <= 0 || process_message.duration <= 0)
                    {
                        throw new MessageException("未获取到请假流程！");
                    }
                    return await ModifyLeavel(entity, process_message);

                //报废
                case 5: return await ModifyCommon(entity,process_message);
                //报损报溢
                case 6:return await ModifyCommon(entity,process_message);

                default:
                    return false;

            }          
        }      
        #endregion

        #region 请假流程
        /// <summary>
        /// 获取请假部门信息
        /// </summary>
        /// <param name="store_id"></param>
        /// <param name="is_org"></param>
        /// <returns></returns>
        public async Task<List<LeavelProcessModel>> GetLeavelProcessAsync(int store_id, bool is_org)
        {
            //查询请假类型
            var leave_typeList = await Db.Queryable<b_basecode>()
                                       .Where(b => b.baseid == 83 && b.stateid == 1)
                                       .WithCache()
                                       .ToListAsync();

            //获取部门信息
            var deptList = await Db.Queryable<p_process>()
                                  .Where(p => p.org_id == userInfo.org_id && p.type_id == 4 && p.leave_type_id > 0 && p.state != 32)
                                  .WhereIF(store_id > 0, p => p.store_id == store_id)
                                  .WhereIF(is_org == false, p => p.store_id > 0)
                                  .WhereIF(is_org == true, p => p.store_id == 0)
                                  .GroupBy(p => new { p.dept_id, p.dept_name, p.leave_type_id })
                                  .Select(p => new DeptProcessModel { dept_id = p.dept_id, dept_name = p.dept_name, leave_type_id = p.leave_type_id })
                                  .WithCache()
                                  .ToListAsync();

            return leave_typeList.Select(w => new LeavelProcessModel { leave_type = w.value, leave_type_id = w.valueid, deptModel = deptList.Where(pd => pd.leave_type_id == w.valueid).ToList() }).ToList();

        }

        /// <summary>
        /// 根据部门获取请假流程
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<List<ProcessModel>> GetLeavelAsync(ProcessLeaveSearchModel entity)
        {
            if ((entity.dept_id <= 0 && entity.dept_id != -99) || entity.leave_type_id <= 0)
            {
                throw new MessageException("未获取到请假类型/部门!");
            }
            //查询流程
            var processList = await Db.Queryable<p_process>()
                                    .Where(p => p.org_id == userInfo.org_id && p.type_id == 4 && p.state != 32 && p.dept_id == entity.dept_id && p.leave_type_id == entity.leave_type_id)
                                    .WhereIF(entity.store_id > 0, p => p.store_id == entity.store_id)
                                    .WhereIF(entity.is_org == false, p => p.store_id > 0)
                                    .WhereIF(entity.is_org == true, p => p.store_id == 0)
                                    .WhereIF(!String.IsNullOrEmpty(entity.name), p => p.name.Contains(entity.name.Trim()))
                                    .OrderBy(p=>p.create_time,OrderByType.Desc)
                                    .WithCache()
                                    .ToListAsync();

            var processIds = processList.Select(s => s.id).ToList();

            //查询流程详情
            var detailsList = await Db.Queryable<p_process_detials>()
                                   .Where(pdd => processIds.Contains(pdd.id))
                                   .WithCache()
                                   .ToListAsync();

            return processList.Select(w => new ProcessModel { process = w, Details = detailsList.Where(pd => pd.id == w.id).ToList() }).ToList();

        }

        /// <summary>
        /// 添加请假流程
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> SetLeavel(AddProcessModel entity)
        {
            if (entity.duration <= 0)
            {
                throw new MessageException("请输入正确时长!");
            }
            if (entity.leavel_type_List.Count <= 0)
            {
                throw new MessageException("请选择请假类型!");
            }

            var processEntity = new p_process();

            processEntity.spell = ToSpell.GetFirstPinyin(entity.name);
            processEntity.enabler = entity.state == 1 ? userInfo.name : " ";
            processEntity.enabler_id = entity.state == 1 ? userInfo.id : 0;
            processEntity.enable_time = entity.state == 1 ? DateTime.Now : Convert.ToDateTime(" 3000-12-31 23:59:59");
            processEntity.total_level = Convert.ToInt16(entity.detailsList.Count > 0 ? entity.detailsList.Count : 0);

            var result = await Db.Ado.UseTranAsync(() =>
            {
                entity.leavel_type_List.ForEach(c =>
                {
                    //查询是否有相同流程
                    var sameProcess = Db.Queryable<p_process>()
                                             .Where(sp => sp.org_id == userInfo.org_id && sp.type_id == entity.type_id && sp.duration == entity.duration && sp.leave_type_id == c.leave_type_id)
                                             .WhereIF(entity.dept_id > 0, sp => sp.dept_id == entity.dept_id)
                                             .WhereIF(entity.is_org == false, sp => sp.store_id > 0)
                                             .WhereIF(entity.is_org == true, sp => sp.store_id == 0)
                                             .WithCache()
                                             .Any();

                    if (sameProcess)
                    {
                        //查询请假类型
                        var typename = Db.Queryable<b_basecode>()
                                       .Where(b => b.baseid == 83 && b.valueid == c.leave_type_id)
                                       .Select(b => b.value)
                                       .WithCache()
                                       .First();

                        throw new MessageException($"已存在此时长的:{typename}流程!");
                    }

                    var store_id = entity.is_org == true ? 0 : 99;

                    var newStoreProcess = new p_process { name = entity.name, leave_type_id = c.leave_type_id, creater = userInfo.name, creater_id = userInfo.id, create_time = DateTime.Now, dept_id = entity.dept_id, dept_name = entity.dept_name, duration = entity.duration, enabler = processEntity.enabler, enabler_id = processEntity.enabler_id, enable_time = processEntity.enable_time, leave_type = c.leave_type, org_id = userInfo.org_id, remark = entity.remark, spell = processEntity.spell, state = entity.state, store_id = store_id, store_name = "无", total_level = processEntity.total_level, type = entity.type_name, type_id = entity.type_id };

                    //新增流程
                    NewProcessAdd(entity, newStoreProcess);
                });

            });

            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }

            return result.IsSuccess;
        }
        /// <summary>
        /// 编辑请假流程
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ModifyLeavel(ModifyProcessModel entity, p_process process_message)
        {
            var result = await Db.Ado.UseTranAsync(() => {
                //使用中的流程无法编辑,查询审核记录
                var modifyProcess = Db.Queryable<r_verify, p_process>((mp, p) => new object[] { JoinType.Left, mp.process_id == p.id })
                                       .Where((mp, p) => mp.org_id == userInfo.org_id && mp.process_type_id == process_message.type_id && mp.state == 33 && mp.process_id == entity.id && p.leave_type_id == process_message.leave_type_id && p.duration == process_message.duration)
                                       .WhereIF(entity.dept_id > 0, (mp, p) => p.dept_id == entity.dept_id)
                                       .WhereIF(entity.is_org == false, (mp, p) => p.store_id > 0)
                                       .WhereIF(entity.is_org == true, (mp, p) => p.store_id == 0)
                                       .WithCache()
                                       .Any();
            if (modifyProcess)
            {
                throw new MessageException("仍有审核进行中，无法编辑！");
            }

            var processEntity = new p_process();
            processEntity.spell = ToSpell.GetFirstPinyin(entity.name);
            processEntity.enabler = entity.state == 1 ? userInfo.name : " ";
            processEntity.enabler_id = entity.state == 1 ? userInfo.id : 0;
            processEntity.enable_time = entity.state == 1 ? DateTime.Now : Convert.ToDateTime(" 3000-12-31 23:59:59");
            processEntity.total_level = Convert.ToInt16(entity.detailsList.Count > 0 ? entity.detailsList.Count : 0);

            //查询是否有相同流程
            var sameProcess = Db.Queryable<p_process>()
                                     .Where(sp => sp.org_id == userInfo.org_id && sp.type_id == process_message.type_id && sp.id != entity.id && sp.name == entity.name.Trim() && sp.leave_type_id == process_message.leave_type_id && sp.duration == entity.duration && sp.dept_id == process_message.dept_id)
                                      .WhereIF(entity.is_org == false, sp => sp.store_id > 0)
                                       .WhereIF(entity.is_org == true, sp => sp.store_id == 0)
                                     .WithCache()
                                     .Any();
            if (sameProcess)
            {
                throw new MessageException("已存在此请假流程!");
            }

           
                //编辑流程
                Db.Updateable(processEntity).SetColumns(p => new p_process { name = entity.name, spell = processEntity.spell, remark = entity.remark, state = entity.state, enabler = processEntity.enabler, enabler_id = processEntity.enabler_id, enable_time = processEntity.enable_time, total_level = processEntity.total_level, duration = entity.duration })
                                                  .Where(p => p.id == entity.id && p.org_id == userInfo.org_id)
                                                  .RemoveDataCache()
                                                  .EnableDiffLogEvent()
                                                  .ExecuteCommand();


                //新增流程详情
                if (entity.detailsList.Count > 0)
                {
                    //删除之前流程
                    Db.Deleteable<p_process_detials>()
                              .Where(w => w.id == entity.id)
                              .RemoveDataCache()
                              .EnableDiffLogEvent()
                              .ExecuteCommand();

                    //新增详情
                    var newDetails = entity.detailsList.Select(d => new p_process_detials { id = entity.id, dept_id = d.dept_id, dept_name = d.dept_name, employee = d.employee, employee_id = d.employee_id, level = d.level, role_id = d.role_id, role_name = d.role_name, is_org = d.is_org }).ToList();

                    Db.Insertable(newDetails).ExecuteCommand();
                    redisCache.RemoveAll<p_process_detials>();
                }

            });

            return result.IsSuccess;
        }

        #endregion

        #region 调拨流程
        /// <summary>
        /// 新增调拨流程
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> AddTransferProcess(AddProcessModel entity)
        {
            var result = await Db.Ado.UseTranAsync(() =>
            {
                //查询是否有相同流程
                var sameProcess = Db.Queryable<p_process>()
                                         .Where(sp => sp.org_id == userInfo.org_id && sp.type_id == entity.type_id && sp.name == entity.name.Trim())
                                         .WhereIF(entity.is_org == false, sp => sp.store_id > 0)
                                         .WhereIF(entity.is_org == true, sp => sp.store_id == 0)
                                         .WithCache()
                                         .Any();
            if (sameProcess)
            {
                throw new MessageException("已存在此流程!");
            }

            var processEntity = new p_process();
            processEntity.name = entity.name;
            processEntity.spell = ToSpell.GetFirstPinyin(entity.name);
            processEntity.org_id = userInfo.org_id;
            processEntity.remark = entity.remark;
            processEntity.creater = userInfo.name;
            processEntity.creater_id = userInfo.id;
            processEntity.create_time = DateTime.Now;
            processEntity.dept_id =  0;
            processEntity.dept_name = " ";
            processEntity.state = entity.state;
            processEntity.type = entity.type_name;
            processEntity.type_id = 2;
            processEntity.total_level = Convert.ToInt16(entity.detailsList.Count > 0 ? entity.detailsList.Count : 0);


           
                //查询启用中的流程
                var useProcess = Db.Queryable<p_process>()
                                  .Where(us => us.state == 1 && us.type_id == processEntity.type_id)
                                  .WhereIF(entity.is_org==true,us=>us.store_id==0)
                                  .WhereIF(entity.is_org==false,us=>us.store_id>0)
                                  .WithCache()
                                  .First();
                var process = false;

                if (useProcess!=null||useProcess?.id>0)
                {
                    //查询流程是否使用中
                    process = Db.Queryable<r_verify>()
                                              .Where(mp => mp.org_id == userInfo.org_id && mp.state == 33 && mp.process_id ==useProcess.id )
                                              .WithCache()
                                              .Any();

                }
                if (process)
                {
                    processEntity.state = 0;
                }
                else
                {
                    processEntity.enabler = userInfo.name;
                    processEntity.enabler_id = userInfo.id;
                    processEntity.enable_time = DateTime.Now;

                    if (entity.state==1)
                    {
                        //如果启用关闭其他启用的流程   
                        NewMethod(entity.state, entity.is_org, processEntity);
                    }
                   

                }

                processEntity.store_id = entity.is_org == true ? 0 : 99;
                processEntity.store_name = "无";
                //新增流程
                NewProcessAdd(entity, processEntity);

            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;

        }
        #endregion


        #region 领用流程
        /// <summary>
        /// 添加领用流程
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> SetConsumer(AddProcessModel entity)
        {            
            if (entity.leavel_type_List.Count <= 0)
            {
                throw new MessageException("请选择物资类型!");
            }

            var processEntity = new p_process();

            processEntity.spell = ToSpell.GetFirstPinyin(entity.name);
            processEntity.total_level = Convert.ToInt16(entity.detailsList.Count > 0 ? entity.detailsList.Count : 0);
            processEntity.state = entity.state;
            entity.type_id = 3;
            processEntity.type_id = entity.type_id;

            var result = await Db.Ado.UseTranAsync(() =>
            {
                entity.leavel_type_List.ForEach(c =>
                {
                    processEntity.leave_type_id = c.leave_type_id;

                    //查询是否有相同流程
                    var sameProcess = Db.Queryable<p_process>()
                                             .Where(sp => sp.org_id == userInfo.org_id && sp.type_id == entity.type_id  && sp.leave_type_id == c.leave_type_id&&sp.name==entity.name)
                                             .WhereIF(entity.is_org == false, sp => sp.store_id > 0)
                                             .WhereIF(entity.is_org == true, sp => sp.store_id == 0)
                                             .WithCache()
                                             .Any();

                    if (sameProcess)
                    {
                        //查询领用类型
                        var typename = Db.Queryable<b_basecode>()
                                       .Where(b => b.baseid == 80 && b.valueid == c.leave_type_id)
                                       .Select(b => b.value)
                                       .WithCache()
                                       .First();

                        throw new MessageException($"已存在此:{typename}的流程!");
                    }


                    //查询启用中的流程
                    var useProcess = Db.Queryable<p_process>()
                                      .Where(us => us.state == 1 && us.type_id == entity.type_id&&us.leave_type_id==c.leave_type_id)
                                      .WhereIF(entity.is_org == true, us => us.store_id == 0)
                                      .WhereIF(entity.is_org == false, us => us.store_id > 0)
                                      .WithCache()
                                      .First();
                    var process = false;

                    if (useProcess != null || useProcess?.id > 0)
                    {
                        //查询流程是否使用中
                        process = Db.Queryable<r_verify>()
                                                  .Where(mp => mp.org_id == userInfo.org_id && mp.state == 33 && mp.process_id == useProcess.id)
                                                  .WithCache()
                                                  .Any();

                    }
                    if (process)
                    {
                        processEntity.state = 0;
                    }
                    else
                    {                      
                        processEntity.enabler = entity.state == 1 ? userInfo.name : " ";
                        processEntity.enabler_id = entity.state == 1 ? userInfo.id : 0;
                        processEntity.enable_time = entity.state == 1 ? DateTime.Now : Convert.ToDateTime(" 3000-12-31 23:59:59");
                       

                        if (entity.state==1)
                        {
                            //如果启用关闭其他启用的流程   
                            NewMethod(entity.state, entity.is_org, processEntity);
                        }
                        

                    }


                    var store_id = entity.is_org == true ? 0 : 99;

                    var newStoreProcess = new p_process { name = entity.name, leave_type_id = c.leave_type_id, creater = userInfo.name, creater_id = userInfo.id, create_time = DateTime.Now, dept_id = 0, dept_name =" ", duration = 0, enabler = processEntity.enabler, enabler_id = processEntity.enabler_id, enable_time = processEntity.enable_time, leave_type = c.leave_type, org_id = userInfo.org_id, remark = entity.remark, spell = processEntity.spell, state = processEntity.state, store_id = store_id, store_name = "无", total_level = processEntity.total_level, type = entity.type_name, type_id = entity.type_id };

                    //新增流程
                    NewProcessAdd(entity, newStoreProcess);
                });

            });

            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }

            return result.IsSuccess;
        }
        /// <summary>
        /// 编辑领用流程
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ModifyConsumer(ModifyProcessModel entity, p_process process_message)
        {
            var result = await Db.Ado.UseTranAsync(() => {
                //使用中的流程无法编辑,查询审核记录
                var modifyProcess = Db.Queryable<r_verify, p_process>((mp, p) => new object[] { JoinType.Left, mp.process_id == p.id })
                                       .Where((mp, p) => mp.org_id == userInfo.org_id && mp.process_type_id == process_message.type_id && mp.state == 33 && mp.process_id == entity.id && p.leave_type_id == process_message.leave_type_id )
                                       .WhereIF(entity.is_org == false, (mp, p) => p.store_id > 0)
                                       .WhereIF(entity.is_org == true, (mp, p) => p.store_id == 0)
                                       .WithCache()
                                       .Any();
            if (modifyProcess)
            {
                throw new MessageException("仍有审核进行中，无法编辑！");
            }

            var processEntity = new p_process();
            processEntity.spell = ToSpell.GetFirstPinyin(entity.name);
            processEntity.enabler = entity.state == 1 ? userInfo.name : " ";
            processEntity.enabler_id = entity.state == 1 ? userInfo.id : 0;
            processEntity.enable_time = entity.state == 1 ? DateTime.Now : Convert.ToDateTime(" 3000-12-31 23:59:59");
            processEntity.total_level = Convert.ToInt16(entity.detailsList.Count > 0 ? entity.detailsList.Count : 0);
            processEntity.type_id = process_message.type_id;
            processEntity.leave_type_id = process_message.leave_type_id;

            //查询是否有相同流程
            var sameProcess = Db.Queryable<p_process>()
                                     .Where(sp => sp.org_id == userInfo.org_id && sp.type_id == process_message.type_id && sp.id != entity.id && sp.name == entity.name.Trim() && sp.leave_type_id == process_message.leave_type_id )
                                      .WhereIF(entity.is_org == false, sp => sp.store_id > 0)
                                       .WhereIF(entity.is_org == true, sp => sp.store_id == 0)
                                     .WithCache()
                                     .Any();
            if (sameProcess)
            {
                throw new MessageException("已存在此领用流程!");
            }

           
                if (entity.state==1)
                {
                    //如果启用关闭其他启用的流程   
                    NewMethod(entity.state, entity.is_org, process_message);
                }
              

                //编辑流程
                Db.Updateable(processEntity).SetColumns(p => new p_process { name = entity.name, spell = processEntity.spell, remark = entity.remark, state = entity.state, enabler = processEntity.enabler, enabler_id = processEntity.enabler_id, enable_time = processEntity.enable_time, total_level = processEntity.total_level, duration = entity.duration })
                                                  .Where(p => p.id == entity.id && p.org_id == userInfo.org_id)
                                                  .RemoveDataCache()
                                                  .EnableDiffLogEvent()
                                                  .ExecuteCommand();


                //新增流程详情
                if (entity.detailsList.Count > 0)
                {
                    //删除之前流程
                    Db.Deleteable<p_process_detials>()
                              .Where(w => w.id == entity.id)
                              .RemoveDataCache()
                              .EnableDiffLogEvent()
                              .ExecuteCommand();

                    //新增详情
                    var newDetails = entity.detailsList.Select(d => new p_process_detials { id = entity.id, dept_id = d.dept_id, dept_name = d.dept_name, employee = d.employee, employee_id = d.employee_id, level = d.level, role_id = d.role_id, role_name = d.role_name, is_org = d.is_org }).ToList();

                    Db.Insertable(newDetails).ExecuteCommand();
                    redisCache.RemoveAll<p_process_detials>();
                }

            });

            return result.IsSuccess;
        }

        #endregion

        #region 报废流程
        /// <summary>
        /// 添加报废流程
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> SetScrap(AddProcessModel entity)
        {           
            var processEntity = new p_process();

            processEntity.spell = ToSpell.GetFirstPinyin(entity.name);
            processEntity.total_level = Convert.ToInt16(entity.detailsList.Count > 0 ? entity.detailsList.Count : 0);
            processEntity.state = entity.state;
            entity.type_id = 5;
            processEntity.type_id = entity.type_id;

            var result = await Db.Ado.UseTranAsync(() =>
            {
                processEntity.leave_type_id = 0;

                //查询是否有相同流程
                var sameProcess = Db.Queryable<p_process>()
                                         .Where(sp => sp.org_id == userInfo.org_id && sp.type_id == entity.type_id  && sp.name == entity.name)
                                         .WhereIF(entity.is_org == false, sp => sp.store_id > 0)
                                         .WhereIF(entity.is_org == true, sp => sp.store_id == 0)
                                         .WithCache()
                                         .Any();

                if (sameProcess)
                {                   
                    throw new MessageException($"已存在此流程!");
                }
                
                //查询启用中的流程
                var useProcess = Db.Queryable<p_process>()
                                  .Where(us => us.state == 1 && us.type_id == entity.type_id )
                                  .WhereIF(entity.is_org == true, us => us.store_id == 0)
                                  .WhereIF(entity.is_org == false, us => us.store_id > 0)
                                  .WithCache()
                                  .First();
                var process = false;

                if (useProcess != null || useProcess?.id > 0)
                {
                    //查询流程是否使用中
                    process = Db.Queryable<r_verify>()
                                              .Where(mp => mp.org_id == userInfo.org_id && mp.state == 33 && mp.process_id == useProcess.id)
                                              .WithCache()
                                              .Any();

                }
                if (process)
                {
                    processEntity.state = 0;
                }
                else
                {
                    processEntity.enabler = entity.state == 1 ? userInfo.name : " ";
                    processEntity.enabler_id = entity.state == 1 ? userInfo.id : 0;
                    processEntity.enable_time = entity.state == 1 ? DateTime.Now : Convert.ToDateTime(" 3000-12-31 23:59:59");


                    if (entity.state == 1)
                    {
                        //如果启用关闭其他启用的流程   
                        NewMethod(entity.state, entity.is_org, processEntity);
                    }


                }


                var store_id = entity.is_org == true ? 0 : 99;

                var newStoreProcess = new p_process { name = entity.name, leave_type_id = 0, creater = userInfo.name, creater_id = userInfo.id, create_time = DateTime.Now, dept_id = 0, dept_name = " ", duration = 0, enabler = processEntity.enabler, enabler_id = processEntity.enabler_id, enable_time = processEntity.enable_time, leave_type =" ", org_id = userInfo.org_id, remark = entity.remark, spell = processEntity.spell, state = processEntity.state, store_id = store_id, store_name = "无", total_level = processEntity.total_level, type = entity.type_name, type_id = entity.type_id };

                //新增流程
                NewProcessAdd(entity, newStoreProcess);

            });

            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }

            return result.IsSuccess;
        }

        #endregion

        #region 报损报溢流程
        /// <summary>
        /// 报损报溢
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> SetFaulty(AddProcessModel entity)
        {
            var processEntity = new p_process();

            processEntity.spell = ToSpell.GetFirstPinyin(entity.name);
            processEntity.total_level = Convert.ToInt16(entity.detailsList.Count > 0 ? entity.detailsList.Count : 0);
            processEntity.state = entity.state;
            entity.type_id = 6;
            processEntity.type_id = entity.type_id;

            var result = await Db.Ado.UseTranAsync(() =>
            {
                processEntity.leave_type_id = 0;

                //查询是否有相同流程
                var sameProcess = Db.Queryable<p_process>()
                                         .Where(sp => sp.org_id == userInfo.org_id && sp.type_id == entity.type_id && sp.name == entity.name)
                                         .WhereIF(entity.is_org == false, sp => sp.store_id > 0)
                                         .WhereIF(entity.is_org == true, sp => sp.store_id == 0)
                                         .WithCache()
                                         .Any();

                if (sameProcess)
                {
                    throw new MessageException($"已存在此流程!");
                }

                //查询启用中的流程
                var useProcess = Db.Queryable<p_process>()
                                  .Where(us => us.state == 1 && us.type_id == entity.type_id)
                                  .WhereIF(entity.is_org == true, us => us.store_id == 0)
                                  .WhereIF(entity.is_org == false, us => us.store_id > 0)
                                  .WithCache()
                                  .First();
                var process = false;

                if (useProcess != null || useProcess?.id > 0)
                {
                    //查询流程是否使用中
                    process = Db.Queryable<r_verify>()
                                              .Where(mp => mp.org_id == userInfo.org_id && mp.state == 33 && mp.process_id == useProcess.id)
                                              .WithCache()
                                              .Any();

                }
                if (process)
                {
                    processEntity.state = 0;
                }
                else
                {
                    processEntity.enabler = entity.state == 1 ? userInfo.name : " ";
                    processEntity.enabler_id = entity.state == 1 ? userInfo.id : 0;
                    processEntity.enable_time = entity.state == 1 ? DateTime.Now : Convert.ToDateTime(" 3000-12-31 23:59:59");


                    if (entity.state == 1)
                    {
                        //如果启用关闭其他启用的流程   
                        NewMethod(entity.state, entity.is_org, processEntity);
                    }


                }


                var store_id = entity.is_org == true ? 0 : 99;

                var newStoreProcess = new p_process { name = entity.name, leave_type_id = 0, creater = userInfo.name, creater_id = userInfo.id, create_time = DateTime.Now, dept_id = 0, dept_name = " ", duration = 0, enabler = processEntity.enabler, enabler_id = processEntity.enabler_id, enable_time = processEntity.enable_time, leave_type = " ", org_id = userInfo.org_id, remark = entity.remark, spell = processEntity.spell, state = processEntity.state, store_id = store_id, store_name = "无", total_level = processEntity.total_level, type = entity.type_name, type_id = entity.type_id };

                //新增流程
                NewProcessAdd(entity, newStoreProcess);

            });

            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }

            return result.IsSuccess;
        }
        #endregion

    }
}
