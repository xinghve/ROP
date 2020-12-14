using Models.DB;
using Models.View.Business;
using Models.View.His;
using Service.Extensions;
using Service.Repository.Interfaces.Business;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace Service.Repository.Implements.Business
{
    /// <summary>
    /// 通知接口
    /// </summary>
    public class NoticeService:DbContext,INoticeService
    {
        /// <summary>
        /// 添加通知
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool AddNotice(List<AddNoticeModel> entity)
        {
            //没有负责人跳过
            if (entity == null)
            {
                return true;
            }
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            //新增通知
            entity.ForEach(c => {
                var newEntity = new oa_notice();
                newEntity.creater = userInfo.name;
                newEntity.creater_id = userInfo.id;
                newEntity.create_date = DateTime.Now;
                newEntity.org_id = userInfo.org_id;
                newEntity.notice_content = c.notice_content;
                newEntity.notice_property = c.notice_property;
                newEntity.notice_property_id = c.notice_property_id;
                newEntity.notice_type_code = c.notice_type_code;
                newEntity.notice_type_id = c.notice_type_id;
                newEntity.notice_type_name = c.notice_type_name;
                newEntity.store_id = c.store_id;
                newEntity.relation_no = c.relation_no;

                var id = Db.Insertable(newEntity).ExecuteReturnIdentity(); ;
                redisCache.RemoveAll<oa_notice>();

                var newDetail = c.employeeMes.Select(w => new oa_notice_employee {employee_id = w.employee_id, employee = w.employee, id = id, img_url = c?.img_url, read_state = 3, service_object = c?.service_object, service_object_id = c?.service_object_id, service_object_phone = c?.service_object_phone }).ToList();

                Db.Insertable(newDetail).ExecuteCommand();
                redisCache.RemoveAll<oa_notice_employee>();
            });

            return true;
        }

        /// <summary>
        /// 添加异步
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> AddNoticeAsync(List<AddNoticeModel> entity)
        {
            //没有负责人跳过
            if (entity == null)
            {
                return true;
            }
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var result = await Db.Ado.UseTranAsync(() => {

                //新增通知
                entity.ForEach(c => {
                    var newEntity = new oa_notice();
                    newEntity.creater = userInfo.name;
                    newEntity.creater_id = userInfo.id;
                    newEntity.create_date = DateTime.Now;
                    newEntity.org_id = userInfo.org_id;
                    newEntity.notice_content = c.notice_content;
                    newEntity.name = c.name;
                    newEntity.notice_property = c.notice_property;
                    newEntity.notice_property_id = c.notice_property_id;
                    newEntity.notice_type_code = c.notice_type_code;
                    newEntity.notice_type_id = c.notice_type_id;
                    newEntity.notice_type_name = c.notice_type_name;
                    newEntity.store_id = c.store_id;
                    newEntity.relation_no = c.relation_no;

                    var id = Db.Insertable(newEntity).ExecuteReturnIdentity(); ;
                    redisCache.RemoveAll<oa_notice>();

                    var newDetail = c.employeeMes.Select(w => new oa_notice_employee { employee_id = w.employee_id, employee = w.employee, id = id, img_url = c?.img_url, read_state = 3, service_object = c?.service_object, service_object_id = c?.service_object_id, service_object_phone = c?.service_object_phone }).ToList();

                    Db.Insertable(newDetail).ExecuteCommand();
                    redisCache.RemoveAll<oa_notice_employee>();
                });
            });

            return result.IsSuccess;
        }

        /// <summary>
        /// 获取通知详情
        /// </summary>
        /// <param name="value_id"></param>
        /// <param name="property_id"></param>
        /// <returns></returns>
        public b_basecode GetBaseDetail(short value_id,short property_id)
        {
            var basecode=  Db.Queryable<b_basecode>()
                           .Where(s => s.baseid == 85 && s.stateid == 1 && s.valueid == value_id && s.propertyid == property_id)
                           .WithCache()
                           .First();
            return basecode;
        }

        /// <summary>
        /// 获取通知类型
        /// </summary>
        /// <returns></returns>
        public List<b_basecode> GetBaseType()
        {
            var basecode = Db.Queryable<b_basecode>()
                         .Where(s => s.baseid == 85 && s.stateid == 1)
                         .Select(s=>new b_basecode{ stateid=s.stateid, valueid=s.valueid, value=s.value, baseid=s.baseid })
                         .GroupBy(s=>new { s.valueid,s.baseid,s.stateid,s.value })
                         .WithCache()
                         .ToList();
            return basecode;
        }


        /// <summary>
        /// 设置通知
        /// </summary>
        /// <param name="relation_no"></param>
        /// <param name="archives"></param>
        /// <param name="store_id"></param>
        /// <param name="notice"></param>
        /// <param name="listModel"></param>
        /// <param name="property_id"></param>
        /// <param name="value_id"></param>
        /// <param name="content"></param>
        /// <param name="name"></param>
        /// <param name="employeeMes"></param>
        public List<AddNoticeModel> NewMethod(string relation_no, c_archives archives, int store_id, Business.NoticeService notice, List<AddNoticeModel> listModel, short value_id, short property_id, string content, string name, List<employeeMes> employeeMes)
        {
            var noticeEntity = new AddNoticeModel();
            noticeEntity.notice_content = content;
            noticeEntity.name = name;
            noticeEntity.store_id = store_id;
            noticeEntity.relation_no = relation_no;
            var nn = new NoticeService();
            var noticeBase = nn.GetBaseDetail(value_id, property_id);
            noticeEntity.notice_type_id = value_id;
            noticeEntity.notice_type_code = noticeBase.valuecode;
            noticeEntity.notice_type_name = noticeBase.value;
            noticeEntity.notice_property_id = property_id;
            noticeEntity.notice_property = noticeBase.property;
            noticeEntity.img_url = archives.image_url;
            noticeEntity.service_object_id = archives.id;
            noticeEntity.service_object = archives.name;
            noticeEntity.service_object_phone = archives.phone;
            noticeEntity.employeeMes = employeeMes;

            listModel.Add(noticeEntity);
            return listModel;
        }

        /// <summary>
        /// 获取消息通知分页数据
        /// </summary>
        /// <returns></returns>
        public async Task<object> GetNoticePage(NoticeSearchModel entity)
        {
            //获取用户信息
            var userinfo = new Tools.IdentityModels.GetUser().userInfo;
            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }

            var notice_count = await Db.Queryable<oa_notice_employee, oa_notice>((a, b) => new object[] { JoinType.Left, a.id == b.id })
                                   .Where((a, b) => b.org_id == userinfo.org_id && a.employee_id == userinfo.id && a.read_state == 3)
                                   .WithCache()
                                   .CountAsync();


            //查询通知表
            var noticeList= await Db.Queryable<oa_notice_employee, oa_notice>((a, b) => new object[] { JoinType.Left, a.id == b.id })
                                   .Where((a, b) => b.org_id == userinfo.org_id && a.employee_id == userinfo.id )
                                   .WhereIF(entity.readstate>0,(a,b)=>a.read_state==entity.readstate)
                                   .WhereIF(entity.valie_id>0,(a,b)=>b.notice_type_id==entity.valie_id)
                                   .Select((a,b)=>new NoticePageModel { id=a.id, notice_content=b.notice_content, read_state=a.read_state.Value, notice_property=b.notice_property,  propertyid=b.notice_property_id, valueid=b.notice_type_id, notice_type_name=b.notice_type_name, relation_no=b.relation_no, service_object=a.service_object, service_object_id=a.service_object_id.Value, notice_time=b.create_date })
                                   .WithCache()
                                   .OrderBy(entity.order + orderTypeStr)
                                   .ToPageAsync(entity.page, entity.limit);

            return new { notice_count, noticeList };

        }

        /// <summary>
        /// 标为已读
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> SetRead(NoticeRead entity)
        {
            //获取用户信息
            var userinfo = new Tools.IdentityModels.GetUser().userInfo;

            if (entity.id>0&&entity.all_id<=0)
            {
                return await Db.Updateable<oa_notice_employee>()
                          .SetColumns(s => s.read_state == 2)
                          .Where(s => s.id == entity.id && s.employee_id == userinfo.id)
                          .EnableDiffLogEvent()
                          .RemoveDataCache()
                          .ExecuteCommandAsync();

            }
            else if (entity.all_id>0&& entity.id < 0)
            {

                return await Db.Updateable<oa_notice_employee>()
                          .SetColumns(s => s.read_state == 2)
                          .Where(s => s.employee_id == userinfo.id)
                          .EnableDiffLogEvent()
                          .RemoveDataCache()
                          .ExecuteCommandAsync();

            }
            else
            {
                return -1;
            }
        }
    }
}
