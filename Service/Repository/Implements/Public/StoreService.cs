using Models.DB;
using Models.View.Public;
using Service.Extensions;
using Service.Repository.Interfaces.Public;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tools;
using Tools.Filter;

namespace Service.Repository.Implements.Public
{
    /// <summary>
    /// 门店
    /// </summary>
    public class StoreService : DbContext, IStoreService
    {
        /// <summary>
        /// 获得列表
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="order">排序字段</param>
        /// <param name="orderType">排序方式(0：升序 1：降序)</param>
        /// <param name="limit">单页数据条数</param>
        /// <param name="page">查询第几页（第一页为1）</param>
        /// <returns></returns>
        public async Task<Page<p_store>> GetPageAsync(string name, string order, int orderType, int limit, int page)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var orderTypeStr = " Asc";
            if (orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            //查询门店
            return await Db.Queryable<p_store>()
                .Where(w => w.use_status != -1 && w.org_id == userInfo.org_id)
                .WhereIF(!string.IsNullOrEmpty(name), w => w.name.Contains(name))
                .OrderBy(order + orderTypeStr)
                .WithCache()
                .ToPageAsync(page, limit);
        }

        /// <summary>
        /// 添加门店信息
        /// </summary>
        /// <param name="entity">门店实体</param>
        /// <returns></returns>
        public async Task<bool> AddAsync(p_store entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            //判断是否存在门店
            var isExistName = await Db.Queryable<p_store>().WithCache().AnyAsync(a => a.name == entity.name && a.org_id == userInfo.org_id && a.use_status != -1);
            if (isExistName)
            {
                throw new MessageException("此门店名已存在!");
            }
            var result = Db.Ado.UseTran(() =>
            {
                entity.cert_status = 0;
                entity.cert_no = "";
                entity.cert_path = "";
                entity.org_id = userInfo.org_id;
                var code = "";
                var maxCode = Db.Queryable<p_store>().Where(w => w.org_id == userInfo.org_id).WithCache().Max(m => m.code);
                var org = Db.Queryable<p_org>().Where(w => w.id == userInfo.org_id).WithCache().First();
                if (maxCode == null)
                {
                    code = org.code + "00001";
                }
                else
                {
                    code = (int.Parse(maxCode) + 1).ToString().PadLeft(10, '0');
                }
                entity.code = code;
                entity.status = 2;
                entity.use_status = 1;
                entity.create_time = DateTime.Now;
                entity.expire_time = DateTime.Now.AddDays(int.Parse(ConfigExtensions.Configuration["BaseConfig:ExpireDay"]));
                var store_id = Db.Insertable(entity).ExecuteReturnIdentity(); //添加门店信息
                redisCache.RemoveAll<p_store>();

                var employee_id = 0;

                var employee = Db.Queryable<p_employee>().Where(w => w.phone_no == entity.phone_no && w.org_id == userInfo.org_id).WithCache().First();//根据手机号查询当前集团下的人员信息

                if (employee == null)
                {
                    employee = new p_employee()
                    {
                        create_time = DateTime.Now,
                        expire_time = org.expire_time,
                        id_no = "",
                        image_url = "",
                        is_admin = false,
                        name = entity.manager,
                        org_id = userInfo.org_id,
                        password = MetarnetRegex.Encrypt(entity.phone_no.Substring(entity.phone_no.Length - 6)),
                        phone_no = entity.phone_no,
                        pinyin = ToSpell.GetFirstPinyin(entity.manager)
                    };
                    employee_id = Db.Insertable(employee).ExecuteReturnIdentity();//当前集团不存在手机号则添加
                    redisCache.RemoveAll<p_employee>();
                }
                else
                {
                    employee_id = employee.id;
                }

                var employee_role = new p_employee_role()
                {
                    dept_id = 0,
                    employee_id = employee_id,
                    org_id = userInfo.org_id,
                    role_id = 0,
                    store_id = store_id,
                    is_admin = true
                };
                Db.Insertable(employee_role).RemoveDataCache().ExecuteCommand();//添加门店超级管理员
            });

            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 编辑门店信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> ModifyAsync(p_store entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            //判断是否存在门店
            var isExistName = await Db.Queryable<p_store>().WithCache().AnyAsync(a => a.name == entity.name && a.org_id == userInfo.org_id && a.id != entity.id && a.use_status != -1);
            if (isExistName)
            {
                throw new MessageException("此门店名已存在!");
            }
            var result = Db.Ado.UseTran(() =>
            {
                var org = Db.Queryable<p_org>().Where(w => w.id == userInfo.org_id).WithCache().First();
                Db.Updateable<p_store>()
                .SetColumns(s => new p_store
                {
                    name = entity.name,
                    no = entity.no,
                    manager = entity.manager,
                    phone_no = entity.phone_no,
                    province_code = entity.province_code,
                    province_name = entity.province_name,
                    city_code = entity.city_code,
                    city_name = entity.city_name,
                    county_code = entity.county_code,
                    county_name = entity.county_name,
                    street_code = entity.street_code,
                    street_name = entity.street_name,
                    address = entity.address,
                    license_no = entity.license_no,
                    license_path = entity.license_path,
                    introduce = entity.introduce,
                    tel=entity.tel
                })
                .Where(w => w.id == entity.id)
                .RemoveDataCache()
                .EnableDiffLogEvent()
                .ExecuteCommand();//修改门店信息

                var employee_id = 0;

                var employee = Db.Queryable<p_employee>().Where(w => w.phone_no == entity.phone_no && w.org_id == userInfo.org_id).WithCache().First();//根据手机号查询当前集团下的人员信息

                if (employee == null)
                {
                    employee = new p_employee()
                    {
                        create_time = DateTime.Now,
                        expire_time = org.expire_time,
                        id_no = "",
                        image_url = "",
                        is_admin = false,
                        name = entity.manager,
                        org_id = userInfo.org_id,
                        password = MetarnetRegex.Encrypt(entity.phone_no.Substring(entity.phone_no.Length - 6)),
                        phone_no = entity.phone_no,
                        pinyin = ToSpell.GetFirstPinyin(entity.manager)
                    };
                    employee_id = Db.Insertable(employee).ExecuteReturnIdentity();//当前集团不存在手机号则添加
                    redisCache.RemoveAll<p_employee>();
                }
                else
                {
                    employee_id = employee.id;
                }

                //删除集团门店超级管理员
                Db.Deleteable<p_employee_role>().Where(w => w.org_id == userInfo.org_id && w.store_id == entity.id && w.is_admin == true).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

                var employee_role = new p_employee_role()
                {
                    dept_id = 0,
                    employee_id = employee_id,
                    org_id = userInfo.org_id,
                    role_id = 0,
                    store_id = entity.id,
                    is_admin = true
                };
                Db.Insertable(employee_role).RemoveDataCache().ExecuteCommand();//添加集团门店管理员
            });

            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 根据门店ID获取门店信息
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <returns></returns>
        public async Task<p_store> GetAsync(int store_id)
        {
            return await Db.Queryable<p_store>().Where(w => w.id == store_id).WithCache().FirstAsync();
        }

        /// <summary>
        /// 修改状态（状态（0=停用，1=正常））
        /// </summary>
        /// <returns></returns>
        public async Task<int> ModifyUseStatusdAsync(p_store entity)
        {
            var status = -2;
            if (entity.use_status == 0)
            {
                status= await Db.Updateable<p_store>()
                .SetColumns(s => new p_store { use_status = entity.use_status, expire_time=DateTime.Now })
                .Where(w => w.id == entity.id)
                .RemoveDataCache()
                .EnableDiffLogEvent()
                .ExecuteCommandAsync();
            }
            else if(entity.use_status==1)
            {
                var end = DateTime.Parse("3000-12-31 23:59:59");
                status = await Db.Updateable<p_store>()
                .SetColumns(s => new p_store { use_status = entity.use_status,expire_time=end })
                .Where(w => w.id == entity.id)
                .RemoveDataCache()
                .EnableDiffLogEvent()
                .ExecuteCommandAsync();
            }
            return status;
           
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <returns></returns>
        public async Task<int> DeleteAsync(List<int> vs)
        {
            return await Db.Updateable<p_store>()
                .SetColumns(s => new p_store { use_status = -1 })
                .Where(w => vs.Contains(w.id))
                .RemoveDataCache()
                .EnableDiffLogEvent()
                .ExecuteCommandAsync();
        }

        /// <summary>
        /// 获取门店列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<Store>> GetListAsync(bool contain_org)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var list = new List<Store>();
            list = await Db.Queryable<p_store>()
                .Where(w => w.org_id == userInfo.org_id && w.use_status != -1)
                .Select(s => new Store { id = s.id, name = s.name })
                .WithCache()
                .ToListAsync();
            if (contain_org)
            {
                var orgName = await Db.Queryable<p_org>().Where(a => a.id == userInfo.org_id).Select(a => new { a.name }).WithCache().FirstAsync();
                if (orgName == null)
                {
                    orgName = new { name = "管理平台" };
                }
                list.Add(new Store { id = 0, name = orgName.name });
            }
            return list;
        }

        /// <summary>
        /// 获取客户端门店列表
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public async Task<List<Store>> GetStoreList(int orgId)
        {
            if (orgId <= 0)
            {
                throw new MessageException("未获取到机构！");
            }

            return await Db.Queryable<p_store>()
                           .Where(w => w.org_id == orgId && w.use_status == 1)
                           .Select(w => new Store { id = w.id, name = w.name })
                           .WithCache()
                           .ToListAsync();
        }

        /// <summary>
        /// 获取门店简介
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public async Task<StoreIntroduceModel> GetStoreIntroduce(int storeId)
        {
            if (storeId<=0)
            {
                throw new MessageException("未获取到门店!");
            }
            //查询门店信息
            var storeDetail = await Db.Queryable<p_store>()
                                    .Where(w => w.id == storeId && w.use_status == 1)
                                    .Select(w => new p_store { code = w.code, tel = w.tel, address = w.address, introduce = w.introduce, org_id=w.org_id })
                                    .WithCache()
                                    .FirstAsync();
            //获取图片List
            var imgList = await Db.Queryable<p_image>()
                                .Where(w => w.relation_code == storeDetail.code)
                                .Select(w => w.url)
                                .WithCache()
                                .ToListAsync();

            //查询上班时间
            var workTime = await Db.Queryable<p_scheduling_time>()
                                 .Where(w => w.org_id == storeDetail.org_id && w.store_id == storeId)
                                 .Select(w=>new p_scheduling_time { start=SqlFunc.AggregateMin( w.start), end= SqlFunc.AggregateMax(w.end) })
                                 .WithCache()
                                 .FirstAsync();

            var time = workTime.start + "-" + workTime.end;
            return new StoreIntroduceModel
            {
                address = storeDetail.address,
                imgUrlList = imgList,
                introduce = storeDetail.introduce,
                tel = storeDetail.tel,
                worktime = time
            };


        }



    }
}
