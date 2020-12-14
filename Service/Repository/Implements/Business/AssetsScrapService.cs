using Models.DB;
using Models.View.Business;
using OfficeOpenXml;
using OfficeOpenXml.Style;
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
    /// 固定资产报废
    /// </summary>
    public class AssetsScrapService : DbContext, IAssetsScrapService
    {
        /// <summary>
        /// 新增资产报废
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> AddAsync(r_assets_scrap entity)
        {
            var result = await Db.Ado.UseTranAsync(() =>
            {
                //获取用户信息
                var userInfo = new Tools.IdentityModels.GetUser().userInfo;

                var assets = Db.Queryable<bus_assets>().Where(w => w.id == entity.assets_id).WithCache().First();

                if (assets.state != 30)
                {
                    throw new MessageException("固定资产不在闲置状态，不能进行报废");
                }

                //修改固定资产状态
                Db.Updateable<bus_assets>().SetColumns(s => new bus_assets { state = 44 }).Where(w => w.id == entity.assets_id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                entity.applicant = userInfo.name;
                entity.applicant_id = userInfo.id;
                entity.apply_time = DateTime.Now;
                entity.assets_no = assets.no;
                entity.org_id = userInfo.org_id;
                entity.spec = assets.spec;
                entity.state = 26;
                entity.std_item_id = assets.std_item_id;
                entity.type = assets.type;
                entity.type_id = assets.type_id;
                entity.manufactor = assets.manufactor;
                entity.manufactor_id = assets.manufactor_id;
                entity.buy_date = assets.buy_date;
                entity.name = assets.name;
                entity.no = "BF" + DateTime.Now.ToString("yyMMdd");
                //查询最大单号
                var max_no = Db.Queryable<r_assets_scrap>().Where(w => w.no.StartsWith(entity.no)).OrderBy(o => o.no, OrderByType.Desc).Select(s => s.no).WithCache().First();
                if (max_no == null)
                {
                    entity.no += "0001";
                }
                else
                {
                    max_no = max_no.Substring(entity.no.Length);
                    entity.no += (int.Parse(max_no) + 1).ToString().PadLeft(4, '0');
                }
                entity.process_id = 0;
                SetProcess(entity);
                Db.Insertable(entity).ExecuteCommand();
                redisCache.RemoveAll<r_assets_scrap>();
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        //设置审核信息
        private void SetProcess(r_assets_scrap entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            //查询调拨单需要进行的审核流程
            var store = 0;
            if (entity.store_id != 0)
            {
                store = 99;
            }
            var process = Db.Queryable<p_process>().Where(w => w.type_id == 5 && w.org_id == userInfo.org_id && w.dept_id == 0 && w.state == 1 && w.store_id == store).OrderBy(o => o.use_money, OrderByType.Desc).WithCache().First();
            if (process == null)
            {
                throw new MessageException("请先设置固定资产报废审核流程");
            }
            entity.total_level = process.total_level;
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
                    model.store = entity.store;
                    model.applicant_id = userInfo.id;
                    model.applicant = userInfo.name;
                    model.process_type_id = 5;
                    model.process_type = "报废";

                    ApprovalLeaveModel entityA = new ApprovalLeaveModel();
                    entityA.apply_no = entity.no;
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
                //报废通知
                notice_content = $"{{\"name\":\"{entity.applicant}\",\"no\":\"{entity.no}\",\"date\":\"{entity.apply_time.ToString()}\",\"remark\":\"{entity.remark}\",\"msg\":\" 报废待审批\"}}";
                var archives = new c_archives();
                archives.id = userInfo.id;
                archives.name = userInfo.name;
                archives.phone = userInfo.phone_no;
                var employeenotice = new List<employeeMes>();


                rolenotice.ForEach(r =>
                {
                    var con = $"{{\"name\":\"{entity.applicant}\",\"msg\":\"提交了报废申请，请处理！\"}}";
                    employeeSocket.Add(new WebSocketModel { userId = r.id, content = con });

                    employeenotice.Add(new employeeMes { employee_id = r.id, employee = r.name });

                });
                notice.NewMethod(entity.no, archives, entity.store_id, notice, noticeList, 3, 9, notice_content, entity.applicant, employeenotice);

                //新增
                notice.AddNotice(noticeList);
                ChatWebSocketMiddleware.SendListAsync(employeeSocket);
            }
        }

        /// <summary>
        /// 取消资产报废
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> CancelAsync(r_assets_scrap entity)
        {
            var assets_scrap = await Db.Queryable<r_assets_scrap>().Where(w => w.no == entity.no).WithCache().FirstAsync();

            //修改固定资产状态
            Db.Updateable<bus_assets>().SetColumns(s => new bus_assets { state = 30 }).Where(w => w.id == assets_scrap.assets_id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

            if (assets_scrap.state == 32)
            {
                //添加库存数量
                var storage = await Db.Queryable<bus_storage>().Where(w => w.org_id == assets_scrap.org_id && w.store_id == assets_scrap.store_id && w.std_item_id == assets_scrap.std_item_id).WithCache().FirstAsync();
                await Db.Updateable<bus_storage>().SetColumns(s => new bus_storage { num = s.num + 1, use_num = s.use_num + 1 }).Where(w => w.id == storage.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
                var storage_detials = await Db.Queryable<bus_storage_detials>().Where(w => w.id == storage.id && w.spec == assets_scrap.spec && w.manufactor_id == assets_scrap.manufactor_id && w.buy_date == assets_scrap.buy_date).WithCache().FirstAsync();
                await Db.Updateable<bus_storage_detials>().SetColumns(s => new bus_storage_detials { num = s.num + 1, use_num = s.use_num + 1 }).Where(w => w.id == storage.id && w.no == storage_detials.no).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
            }

            return await Db.Updateable<r_assets_scrap>().SetColumns(s => new r_assets_scrap { state = 35 }).Where(w => w.no == entity.no).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
        }

        /// <summary>
        /// 资产报废单分页
        /// </summary>
        /// <returns></returns>
        public async Task<Page<AssetsScrap>> GetPageAsync(AssetsScrapPageSearch entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }

            var pages = await Db.Queryable<r_assets_scrap>()
                            .Where(w => w.org_id == userInfo.org_id)
                            .WhereIF(!string.IsNullOrEmpty(entity.no), w => w.no.Contains(entity.no))
                            .WhereIF(!string.IsNullOrEmpty(entity.str), w => w.name.Contains(entity.str) || w.spec.Contains(entity.str) || w.assets_no.Contains(entity.str))
                            .WhereIF(entity.state != -1, w => w.state == entity.state)
                            .WhereIF(entity.store_id != -1, w => w.store_id == entity.store_id)
                            .Select(s => new AssetsScrap { applicant = s.applicant, no = s.no, applicant_id = s.applicant_id, apply_time = s.apply_time, assets_id = s.assets_id, assets_no = s.assets_no, await_verifier = s.await_verifier, await_verifier_id = s.await_verifier_id, buy_date = s.buy_date, id = s.id, is_org = s.is_org, level = s.level, manufactor = s.manufactor, manufactor_id = s.manufactor_id, name = s.name, org_id = s.org_id, process_id = s.process_id, remark = s.remark, spec = s.spec, state = s.state, store = s.store, store_id = s.store_id, total_level = s.total_level, type = s.type, type_id = s.type_id, verifier = s.verifier, verifier_id = s.verifier_id, verify_remark = s.verify_remark, verify_time = s.verify_time, std_item_id = s.std_item_id })
                            .OrderBy(entity.order + orderTypeStr)
                            .WithCache()
                            .ToPageAsync(entity.page, entity.limit);

            //列表中的报废单号
            var nos = pages.Items.Select(s => s.no).ToList();

            //列表中的审核流程ID
            var process_ids = pages.Items.Select(s => s.process_id).ToList();

            //审核流程
            var process_list = await Db.Queryable<p_process>().Where(w => process_ids.Contains(w.id)).WithCache().ToListAsync();

            //审核流程明细
            var process_detials_list = await Db.Queryable<p_process_detials>().Where(w => process_ids.Contains(w.id)).WithCache().ToListAsync();

            //审核记录
            var verify_detials_list = await Db.Queryable<r_verify_detials>().Where(w => nos.Contains(w.realted_no)).WithCache().ToListAsync();

            //赋值明细
            pages.Items = pages.Items.Select(s => new AssetsScrap { applicant = s.applicant, no = s.no, applicant_id = s.applicant_id, apply_time = s.apply_time, assets_id = s.assets_id, assets_no = s.assets_no, await_verifier = s.await_verifier, await_verifier_id = s.await_verifier_id, buy_date = s.buy_date, id = s.id, is_org = s.is_org, level = s.level, manufactor = s.manufactor, manufactor_id = s.manufactor_id, name = s.name, org_id = s.org_id, process_id = s.process_id, remark = s.remark, spec = s.spec, state = s.state, store = s.store, store_id = s.store_id, total_level = s.total_level, type = s.type, type_id = s.type_id, verifier = s.verifier, verifier_id = s.verifier_id, verify_remark = s.verify_remark, verify_time = s.verify_time, std_item_id = s.std_item_id, verifyInfos = verify(s.process_id.Value, s.no, process_list, process_detials_list, verify_detials_list) }).ToList();

            return pages;
        }

        private List<verifyInfo> verify(int process_id, string no, List<p_process> process_list, List<p_process_detials> process_detials_list, List<r_verify_detials> verify_detials_list)
        {
            var list = new List<verifyInfo>();
            if (process_id > 0)
            {
                //审核流程
                var process = process_list.Where(w => w.id == process_id).FirstOrDefault();
                //审核流程明细
                var p_process_detials = process_detials_list.Where(w => w.id == process.id).OrderBy(o => o.level).ToList();
                //审核记录
                var Verify_Detials = verify_detials_list.Where(w => w.realted_no == no).ToList();
                foreach (var item in p_process_detials)
                {
                    var vd = Verify_Detials.Where(w => w.process_level == item.level).FirstOrDefault();
                    if (vd == null)
                    {
                        list.Add(new verifyInfo { is_org = item.is_org.Value, state = 26, verifier = "", verify_remark = "", verify_time = "", dept = item.dept_name, role = item.role_name });
                    }
                    else
                    {
                        list.Add(new verifyInfo { is_org = item.is_org.Value, state = vd.state, verifier = vd.verifier, verify_remark = vd.verify_remark, verify_time = vd.verify_time.ToString("yyyy-MM-dd HH:mm:ss"), dept = item.dept_name, role = item.role_name });
                        if (vd.state == 28)
                        {
                            break;
                        }
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 获取报废单信息
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        public async Task<AssetsScrap> GetAssetsScrapAsync(string no)
        {
            var assets_scrap = await Db.Queryable<r_assets_scrap>().Where(w => w.no == no).WithCache().FirstAsync();

            //审核流程
            var process_list = await Db.Queryable<p_process>().Where(w => w.id == assets_scrap.process_id).WithCache().ToListAsync();

            //审核流程明细
            var process_detials_list = await Db.Queryable<p_process_detials>().Where(w => w.id == assets_scrap.process_id).WithCache().ToListAsync();

            //审核记录
            var verify_detials_list = await Db.Queryable<r_verify_detials>().Where(w => w.realted_no == no).WithCache().ToListAsync();

            var verifyInfos = verify(assets_scrap.process_id.Value, no, process_list, process_detials_list, verify_detials_list);

            return new AssetsScrap { applicant = assets_scrap.applicant, no = assets_scrap.no, process_id = assets_scrap.process_id, applicant_id = assets_scrap.applicant_id, apply_time = assets_scrap.apply_time, assets_id = assets_scrap.assets_id, assets_no = assets_scrap.assets_no, await_verifier = assets_scrap.await_verifier, await_verifier_id = assets_scrap.await_verifier_id, buy_date = assets_scrap.buy_date, id = assets_scrap.id, is_org = assets_scrap.is_org, level = assets_scrap.level, manufactor = assets_scrap.manufactor, manufactor_id = assets_scrap.manufactor_id, name = assets_scrap.name, org_id = assets_scrap.org_id, remark = assets_scrap.remark, spec = assets_scrap.spec, state = assets_scrap.state, std_item_id = assets_scrap.std_item_id, store = assets_scrap.store, store_id = assets_scrap.store_id, total_level = assets_scrap.total_level, type = assets_scrap.type, type_id = assets_scrap.type_id, verifier = assets_scrap.verifier, verifier_id = assets_scrap.verifier_id, verify_remark = assets_scrap.verify_remark, verify_time = assets_scrap.verify_time, verifyInfos = verifyInfos };
        }

        /// <summary>
        /// 导出报废单
        /// </summary>
        /// <param name="list_no"></param>
        /// <returns></returns>
        public async Task<string> ExportAsync(List<string> list_no)
        {
            if (list_no.Count == 0)
            {
                throw new MessageException("请选择报废单进行导出");
            }


            var applys = await Db.Queryable<r_assets_scrap>()
                            .Where(w => list_no.Contains(w.no))
                            .WithCache().ToListAsync();

         
            string sWebRootFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot\\tempExcel");//如果用浏览器url下载的方式  存放excel的文件夹一定要建在网站首页的同级目录下！！！
            if (!Directory.Exists(sWebRootFolder))
            {
                Directory.CreateDirectory(sWebRootFolder);
            }

            //查询用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            string sFileName = $@"scrap_{userInfo.id}.xlsx";
            var path = Path.Combine(sWebRootFolder, sFileName);
            FileInfo file = new FileInfo(path);
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(path);
            }
            using (ExcelPackage package = new ExcelPackage(file))
            {
                applys.ForEach(item =>
                {
                    //创建sheet
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add($"{item.no}");
                    worksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[1, 1, 2, 9].Merge = true;
                    worksheet.Cells[1, 1].Value = "报废单";
                    worksheet.Cells[1, 1].Style.Font.Size = 20;
                    worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[3, 1, 3, 3].Merge = true;
                    worksheet.Cells[3, 1].Value = $"单号：{item.no}";
                    worksheet.Cells[3, 4, 3, 6].Merge = true;
                    worksheet.Cells[3, 4].Value = $"报废物品：{item.name}";
                    worksheet.Cells[3, 7, 3, 9].Merge = true;
                    worksheet.Cells[3, 7].Value = $"物品编号：{item.assets_no}";
                    worksheet.Cells[4, 1, 4, 3].Merge = true;
                    worksheet.Cells[4, 1].Value = $"规格：{item.spec}";
                    worksheet.Cells[4, 4, 4, 6].Merge = true;
                    worksheet.Cells[4, 6].Value = $"厂家：{item.manufactor}";
                    worksheet.Cells[4, 7, 4, 9].Merge = true;
                    worksheet.Cells[4, 7].Value = $"厂家：{item.manufactor}";
                    worksheet.Cells[5, 1, 5, 3].Merge = true;
                    worksheet.Cells[5, 1].Value = $"操作人：{item.applicant}";
                    worksheet.Cells[5, 4, 5, 6].Merge = true;
                    worksheet.Cells[5, 6].Value = $"操作时间：{item.apply_time.ToString("yyyy-MM-dd hh:mm:ss")}";
                    worksheet.Cells[6, 1, 6, 9].Merge = true;
                    worksheet.Cells[6, 1].Value = $"报废原由：{item.remark}";
                });

                package.Save();
            }
            return path;//这是返回文件的方式
            //return "tempExcel/" + sFileName;    //如果用浏览器url下载的方式  这里直接返回生成的文件名就可以了
        }

    }
}
