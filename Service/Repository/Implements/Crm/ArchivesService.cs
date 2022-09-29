using Microsoft.AspNetCore.Http;
using Models.DB;
using Models.View.Crm;
using Models.View.Mobile;
using Models.View.Public;
using Service.Extensions;
using Service.Repository.Interfaces.Crm;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Tools;
using Tools.Filter;

namespace Service.Repository.Implements.Crm
{
    /// <summary>
    /// 档案信息
    /// </summary>
    public class ArchivesService : DbContext, IArchivesService
    {
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <returns></returns>
        public async Task<bool> AddAsync(Archives archives)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var c_archives = archives.archives;
            var archivesSupplement = archives.archivesSupplement;
            //var c_archives_extend = archives.archivesExtend;
            //var tags = archives.archivesTags;
            //var preference = archives.preference;

            var exist = await Db.Queryable<c_archives>().Where(w => w.phone == c_archives.phone && w.org_id == userInfo.org_id).WithCache().AnyAsync();
            if (exist)
            {
                throw new MessageException("手机号已存在，请确认信息是否正确");
            }
            if (!string.IsNullOrEmpty(c_archives.id_card))
            {
                exist = await Db.Queryable<c_archives>().Where(w => w.id_card == c_archives.id_card && w.org_id == userInfo.org_id).WithCache().AnyAsync();
                if (exist)
                {
                    throw new MessageException("身份证号已存在，请确认信息是否正确");
                }
            }

            var result = Db.Ado.UseTran(() =>
            {
                #region 档案信息
                //var info = Utils.GetInfoFromIDCard(c_archives.id_card);
                //if (!info.IsSuccess)
                //{
                //    throw new MessageException(info.Message);
                //}
                c_archives.no = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                c_archives.virtual_no = "vn" + DateTime.Now.ToString("yyyyMMddHHmmssffff");
                c_archives.org_id = userInfo.org_id;
                c_archives.spell = ToSpell.GetFirstPinyin(c_archives.name);
                //c_archives.age = info.age;
                //c_archives.month = info.month;
                //c_archives.day = info.day;
                //c_archives.birthday = info.birthday;
                //c_archives.sex = info.sex;
                c_archives.state = 1;
                //c_archives.constellation = info.constellation;
                //c_archives.zodiac = info.zodiac;
                c_archives.password = MetarnetRegex.Encrypt(c_archives.phone.Substring(c_archives.phone.Length - 6));
                c_archives.grade_id = 0;
                c_archives.creater_id = userInfo.id;
                c_archives.creater = userInfo.name;
                c_archives.create_date = DateTime.Now;

                //添加档案信息返回Id
                var archives_id = Db.Insertable(c_archives).ExecuteReturnIdentity();
                redisCache.RemoveAll<c_archives>();

                #endregion

                ////添加档案详情信息
                //c_archives_extend.id = archives_id;
                //c_archives_extend.to_director = userInfo.name;
                //c_archives_extend.to_director_id = userInfo.id;
                //Db.Insertable(c_archives_extend).RemoveDataCache().ExecuteCommand();

                #region 档案转移
                if (c_archives.to_director_id > 0)
                {
                    var to_store_name = Db.Queryable<p_store>().Where(w => w.id == c_archives.store_id).WithCache().First().name;
                    var c_maintain = new c_maintain
                    {
                        archives = c_archives.name,
                        id_card = c_archives.id_card,
                        archives_id = archives_id,
                        archives_phone = c_archives.phone,
                        cause = "创建负责人",
                        creater = userInfo.name,
                        creater_id = userInfo.id,
                        create_date = DateTime.Now,
                        from_director = "",
                        from_director_id = 0,
                        org_id = userInfo.org_id,
                        from_store_id = 0,
                        from_store_name = "",
                        to_store_id = c_archives.store_id,
                        to_store_name = to_store_name,
                        to_director = c_archives.to_director,
                        to_director_id = c_archives.to_director_id
                    };
                    Db.Insertable(c_maintain).RemoveDataCache().ExecuteCommand();
                }
                #endregion

                //if (tags.Count > 0)
                //{
                //    //添加档案标签
                //    var list = tags.Select(s => new c_archives_tag { archives_id = archives_id, tag = s.tag, tag_id = s.tag_id }).ToList();
                //    Db.Insertable(list).ExecuteCommand();
                //    redisCache.RemoveAll<c_archives_tag>();
                //}
                //if (preference.Count > 0)
                //{
                //    //添加偏好
                //    var list = preference.Select(s => new c_archives_preference { archives_id = archives_id, text = s }).ToList();
                //    Db.Insertable(list).ExecuteCommand();
                //    redisCache.RemoveAll<c_archives_preference>();
                //}

                #region 档案账户
                var pwd = MetarnetRegex.Encrypt(c_archives.phone.Substring(c_archives.phone.Length - 6));
                var rate = 0;
                var code = MetarnetRegex.Encrypt(MetarnetRegex.SHA512Encrypt((archives_id + rate).ToString()));

                //查询券码是否存在
                var isExist = Db.Queryable<c_new_coupon_detials, c_activity>((n, a) => new object[] { JoinType.Left, n.activity_id == a.id })
                      .Where((n, a) => n.org_id == userInfo.org_id && n.use_state == 3 && a.start_date <= DateTime.Now.Date && a.end_date >= DateTime.Now.Date && n.no == archives.archives.coupon_no.Trim() && a.state == 1)
                      .Select((n, a) => new { n.money, n.no, a.name, a.id, coupon_id = n.id, a.start_date, a.end_date })
                      .WithCache()
                      .First();

                //档案账户
                var account = new c_account { amount = 0, archives_id = archives_id, balance = 0, consume = 0, coupon = isExist == null ? 0 : isExist.money.Value, integral = 0, noneamount = 0, password = pwd, rate = rate, recharge = 0, salseamount = 0, settleamount = 0, code = code, total_coupon = isExist == null ? 0 : isExist.money.Value, total_integral = 0 };
                Db.Insertable(account).RemoveDataCache().ExecuteCommand();
                #endregion

                #region 档案附页
                if (archivesSupplement != null)
                {
                    //编辑档案附页
                    archivesSupplement.archives_id = archives_id;
                    Db.Insertable(archivesSupplement).RemoveDataCache().ExecuteCommand();
                }
                #endregion

                ////会员卡信息
                //var card = new c_archives_card
                //{
                //    archives = c_archives.name,
                //    archives_id = archives_id,
                //    archives_phone = c_archives.phone,
                //    balance = 0,
                //    card_no = DateTime.Now.ToString("yyyyMMddHHmmssffff"),
                //    creater = userInfo.name,
                //    creater_id = userInfo.id,
                //    create_date = DateTime.Now,
                //    give_balance = 0,
                //    give_total_money = 0,
                //    integral = 0,
                //    level = "无",
                //    level_id = 0,
                //    org_id = userInfo.org_id,
                //    password = pwd,
                //    store_id = c_archives.store_id,
                //    total_integral = 0,
                //    total_money = 0
                //};
                //Db.Insertable(card).RemoveDataCache().ExecuteCommand();

                #region 立即充值
                if (archives.is_recharge == 2)
                {

                    if (isExist == null)
                    {
                        throw new MessageException("优惠券信息有误！");
                    }

                    if (isExist.money <= 0)
                    {
                        throw new MessageException("未获取到赠送金额！");
                    }
                    //查询会员卡等级信息
                    var levelDetail = Db.Queryable<c_archives_level>().Where(a => a.org_id == userInfo.org_id && a.min_money == 0&&a.special!=2).WithCache().First();

                    var recharge = new r_recharge();
                    recharge.archives_id = archives_id;
                    recharge.card_no = c_archives.card_no;
                    recharge.archives = c_archives.name;
                    recharge.archives_phone = c_archives.phone;
                    recharge.recharge_date = DateTime.Now;
                    recharge.recharge_money = 0;
                    recharge.recharge_integral = 0;
                    recharge.money_integral = 0;
                    recharge.total_money = 0;
                    recharge.total_integral = 0;
                    recharge.way_code = "";
                    recharge.way = "";
                    recharge.bill_no = archives_id + DateTime.Now.ToString("yyyyMMddhhmmssffffff");
                    recharge.occurrence_date = DateTime.Now;
                    recharge.level = levelDetail == null ? "" : levelDetail.name;
                    recharge.org_id = userInfo.org_id;
                    recharge.store_id = c_archives.store_id;
                    recharge.balance = 0;
                    recharge.integral = 0;
                    recharge.give_total_money = isExist.money;
                    recharge.give_balance = isExist.money;
                    recharge.give_money = isExist.money;
                    recharge.creater = userInfo.name;
                    recharge.creater_id = userInfo.id;
                    recharge.consultation_id = 0;
                    recharge.categroy_id = 1;
                    recharge.no = archives_id + DateTime.Now.ToString("yyMMddhhmmssffff");
                    recharge.state_id = 6;
                    recharge.state = "已充值";
                    recharge.to_director_id = c_archives.to_director_id;
                    recharge.to_director = c_archives.to_director;
                    recharge.coupon_no = isExist.no;

                    Db.Insertable(recharge).ExecuteCommand();
                    redisCache.RemoveAll<r_recharge>();

                    //修改优惠券的状态
                    if (!string.IsNullOrEmpty(archives.archives.coupon_no))
                    {
                        var isTrue = Db.Updateable<c_new_coupon_detials>()
                          .SetColumns(w => w.use_state == 2)
                          .Where(w => w.org_id == userInfo.org_id && w.no == isExist.no)
                          .RemoveDataCache()
                          .EnableDiffLogEvent()
                          .ExecuteCommand();
                        if (isTrue <= 0)
                        {
                            throw new MessageException("优惠券未使用成功！");
                        }

                        //添加到会员领取表
                        var archives_coupon = new c_archives_activity_coupon();
                        archives_coupon.archives_id = archives_id;
                        archives_coupon.activity_id = isExist.id;
                        archives_coupon.coupon_id = isExist.coupon_id;
                        archives_coupon.state = 2;
                        archives_coupon.org_id = userInfo.org_id;
                        archives_coupon.store_id = archives.store_id;
                        archives_coupon.use_date = DateTime.Now;
                        archives_coupon.coupon_no = isExist.no;

                        Db.Insertable(archives_coupon).ExecuteCommand();
                        redisCache.RemoveAll<c_archives_activity_coupon>();
                    }
                }
                #endregion
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 添加一条数据（移动端）
        /// </summary>
        /// <returns></returns>
        public async Task<bool> MobileAddAsync(MobileArchives archives)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            archives.to_director_id = userInfo.id;
            archives.to_director = userInfo.name;
            return await AddArc(archives, userInfo);
        }

        /// <summary>
        /// 添加一条数据（分销人员端）
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DistributorAddAsync(MobileArchives archives)
        {
            //获分销人员信息
            var distributorInfo = new Tools.IdentityModels.GetDistributor().Distributor;
            var userInfo = new Tools.IdentityModels.GetUser.UserInfo { id = 0, is_admin = false, name = distributorInfo.name, org_id = distributorInfo.org_id, phone_no = distributorInfo.phone_no };
            archives.to_director_id = distributorInfo.director_id;
            archives.to_director = distributorInfo.director;
            archives.distributor_id = distributorInfo.id;
            archives.store_id = distributorInfo.store_id;
            return await AddArc(archives, userInfo);
        }

        /// <summary>
        /// 添加一条数据（客户端注册）
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ArcAddAsync(RegisterArchives archives)
        {
            var userInfo = new Tools.IdentityModels.GetUser.UserInfo { id = 0, is_admin = false, name = archives.name, org_id = archives.org_id, phone_no = archives.phone };
            var mobileArchives = new MobileArchives { drink = "", foods = "", fruits = "", habit = "", hobby = "", id = 0, id_card = archives.id_card, name = archives.name, phone = archives.phone, smoke = "", store_id = archives.store_id, to_director_id = archives.to_director_id, to_director = "" };
            if (archives.to_director_id > 0)
            {
                var employee = await Db.Queryable<p_employee>().Where(w => w.id == archives.to_director_id).WithCache().FirstAsync();
                mobileArchives.to_director = employee.name;
            }
            return await AddArc(mobileArchives, userInfo);
        }

        private async Task<bool> AddArc(MobileArchives archives, Tools.IdentityModels.GetUser.UserInfo userInfo)
        {
            var c_archives = new c_archives { id_card = archives.id_card, name = archives.name, phone = archives.phone, flagid = "1", flag = "身份证", store_id = archives.store_id, to_director = archives.to_director, to_director_id = archives.to_director_id, org_id = userInfo.org_id, card_no = "", carno = "", address = "", emergencyno = "", type_code = "1", type = "潜在客户" };
            var archivesSupplement = new c_archives_supplement { drink = archives.drink, foods = archives.foods, fruits = archives.fruits, habit = archives.habit, hobby = archives.hobby, smoke = archives.smoke };

            var exist = await Db.Queryable<c_archives>().Where(w => w.phone == c_archives.phone && w.org_id == userInfo.org_id).WithCache().AnyAsync();
            if (exist)
            {
                throw new MessageException("手机号已存在，请确认信息是否正确");
            }
            if (!string.IsNullOrEmpty(c_archives.id_card))
            {
                exist = await Db.Queryable<c_archives>().Where(w => w.id_card == c_archives.id_card && w.org_id == userInfo.org_id).WithCache().AnyAsync();
                if (exist)
                {
                    throw new MessageException("身份证号已存在，请确认信息是否正确");
                }
            }

            var result = Db.Ado.UseTran(() =>
            {
                #region 档案信息
                var info = Utils.GetInfoFromIDCard(c_archives.id_card);
                if (!info.IsSuccess)
                {
                    throw new MessageException(info.Message);
                }
                c_archives.no = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                c_archives.virtual_no = "vn" + DateTime.Now.ToString("yyyyMMddHHmmssffff");
                c_archives.spell = ToSpell.GetFirstPinyin(c_archives.name);
                c_archives.age = info.age;
                c_archives.month = info.month;
                c_archives.day = info.day;
                c_archives.birthday = info.birthday;
                c_archives.sex = info.sex;
                c_archives.sex_code = GetSex(info.sex);
                c_archives.state = 1;
                c_archives.password = MetarnetRegex.Encrypt(c_archives.phone.Substring(c_archives.phone.Length - 6));
                c_archives.grade_id = 0;
                c_archives.creater_id = userInfo.id;
                c_archives.creater = userInfo.name;
                c_archives.create_date = DateTime.Now;

                //添加档案信息返回Id
                var archives_id = Db.Insertable(c_archives).ExecuteReturnIdentity();
                redisCache.RemoveAll<c_archives>();

                #endregion

                #region 档案转移
                if (c_archives.to_director_id > 0)
                {
                    var to_store_name = Db.Queryable<p_store>().Where(w => w.id == c_archives.store_id).WithCache().First().name;
                    var c_maintain = new c_maintain
                    {
                        archives = c_archives.name,
                        id_card = c_archives.id_card,
                        archives_id = archives_id,
                        archives_phone = c_archives.phone,
                        cause = "创建负责人",
                        creater = userInfo.name,
                        creater_id = userInfo.id,
                        create_date = DateTime.Now,
                        from_director = "",
                        from_director_id = 0,
                        org_id = userInfo.org_id,
                        from_store_id = 0,
                        from_store_name = "",
                        to_store_id = c_archives.store_id,
                        to_store_name = to_store_name,
                        to_director = c_archives.to_director,
                        to_director_id = c_archives.to_director_id
                    };
                    Db.Insertable(c_maintain).RemoveDataCache().ExecuteCommand();
                }
                #endregion

                #region 分销人员

                if (archives.distributor_id > 0)
                {
                    Db.Insertable(new c_distributor { archives_id = archives_id, distributor_id = archives.distributor_id }).RemoveDataCache().ExecuteCommand();
                }

                #endregion

                #region 档案账户
                var pwd = MetarnetRegex.Encrypt(c_archives.phone.Substring(c_archives.phone.Length - 6));
                var rate = 0;
                var code = MetarnetRegex.Encrypt(MetarnetRegex.SHA512Encrypt((archives_id + rate).ToString()));

                //档案账户
                var account = new c_account { amount = 0, archives_id = archives_id, balance = 0, consume = 0, coupon = 0, integral = 0, noneamount = 0, password = pwd, rate = rate, recharge = 0, salseamount = 0, settleamount = 0, code = code, total_coupon = 0, total_integral = 0 };
                Db.Insertable(account).RemoveDataCache().ExecuteCommand();
                #endregion

                #region 档案附页
                if (archivesSupplement != null)
                {
                    //编辑档案附页
                    archivesSupplement.archives_id = archives_id;
                    Db.Insertable(archivesSupplement).RemoveDataCache().ExecuteCommand();
                }
                #endregion
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        private static string GetSex(string sex)
        {
            if (sex == "男")
            {
                return "1";
            }
            else if (sex == "女")
            {
                return "2";
            }
            return "9";
        }

        /// <summary>
        /// 根据档案ID获取信息
        /// </summary>
        /// <param name="archives_id"></param>
        /// <returns></returns>
        public async Task<ArchivesExtend> GetArchivesAsync(int archives_id)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            return await Db.Queryable<c_archives, p_store, c_archives_supplement>((a, s, ars) => new object[] { JoinType.Left, a.store_id == s.id, JoinType.Left, a.id == ars.archives_id })
                .Where((a, s, ars) => a.org_id == userInfo.org_id && a.id == archives_id).Select((a, s, ars) => new ArchivesExtend { image_url = a.image_url, address = a.address, age = a.age, birthday = a.birthday, aboid = a.aboid, abo = a.abo, rhid = a.rhid, rh = a.rh, card_no = a.card_no, city = a.city, city_code = a.city_code, constellation = a.constellation, constellation_code = a.constellation_code, contacts = a.contacts, contacts_phone = a.contacts_phone, county = a.county, county_code = a.county_code, creater = a.creater, day = a.day, email = a.email, id_card = a.id_card, id = a.id, income = a.income, marital_status = a.marital_status, marital_status_code = a.marital_status_code, month = a.month, name = a.name, nation = a.nation, nation_code = a.nation_code, occupation = a.occupation, occupation_code = a.occupation_code, org_id = a.org_id, phone = a.phone, province = a.province, province_code = a.province_code, qq = a.qq, remarks = a.remarks, sex = a.sex, sex_code = a.sex_code, spell = a.spell, state = a.state, street = a.street, store_id = a.store_id, street_code = a.street_code, to_director = a.to_director, type = a.type, type_code = a.type_code, to_director_id = a.to_director_id, wechat = a.wechat, zodiac = a.zodiac, zodiac_code = a.zodiac_code, create_date = a.create_date, store_name = s.name, drink = ars.drink, foods = ars.foods, fruits = ars.fruits, habit = ars.habit, hobby = ars.hobby, smoke = ars.smoke, emergencyno = a.emergencyno, carno = a.carno, education = a.education, educationid = a.educationid, flag = a.flag, flagid = a.flagid, grade_id = a.grade_id, no = a.no, relation = a.relation, relationid = a.relationid, virtual_no = a.virtual_no, archives_class = a.archives_class, class_code = a.class_code, source = a.source, coupon_no = a.coupon_no, source_code = a.source_code }).WithCache().FirstAsync();
        }

        /// <summary>
        /// 获取客户量
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<dynamic> GetArchivesCountAsync(short type)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            //获取负责人所有充值
            var archives = await Db.Queryable<c_archives>().Where(w => w.to_director_id == userInfo.id).ToListAsync();
            //总绩效
            var count = archives.Count;
            var type_count = 0;
            if (type == 1)//年
            {
                type_count = archives.Where(w => w.create_date.Value.Year == DateTime.Now.Year).Count();
            }
            else if (type == 2)//月
            {
                type_count = archives.Where(w => w.create_date.Value.Year == DateTime.Now.Year && w.create_date.Value.Month == DateTime.Now.Month).Count();
            }
            else//日
            {
                type_count = archives.Where(w => w.create_date.Value.Year == DateTime.Now.Year && w.create_date.Value.Month == DateTime.Now.Month && w.create_date.Value.Day == DateTime.Now.Day).Count();
            }
            return new { count, type_count };
        }

        /// <summary>
        /// 获取档案列表
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <param name="is_all">是否所有</param>
        /// <param name="name">姓名/手机号</param>
        /// <returns></returns>
        public async Task<dynamic> GetListAsync(int store_id, bool is_all, string name)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            return await Db.Queryable<c_archives, c_archives_supplement>((a, ars) => new object[] { JoinType.Left, a.id == ars.archives_id })
                //.Where((a,ars)=> a.state==1)
                .WhereIF(store_id != -1, (a, ars) => a.store_id == store_id)
                .WhereIF(!is_all, (a, ars) => a.to_director_id == userInfo.id)
                .WhereIF(!string.IsNullOrEmpty(name), (a, ars) => a.name.Contains(name) || a.phone.Contains(name))
                .Select((a, ars) => a)
                .WithCache()
                .ToListAsync();
        }

        /// <summary>
        /// 获取档案分页列表（分销人员端）
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> GetPagesByDistributorAsync(Search entity)
        {
            //获分销人员信息
            var distributorInfo = new Tools.IdentityModels.GetDistributor().Distributor;
            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            var list = await Db.Queryable<c_distributor, c_archives, c_archives_supplement>((cd, a, ars) => new object[] { JoinType.Left, cd.archives_id == a.id, JoinType.Left, a.id == ars.archives_id })
                .Where((cd, a, ars) => a.store_id == distributorInfo.store_id)
                .Select((cd, a, ars) => new { a.name, a.phone, a.id_card, a.image_url, a.create_date, ars.drink, ars.foods, ars.fruits, ars.habit, ars.hobby, ars.smoke, a.sex })
                .OrderBy(entity.order + orderTypeStr)
                .WithCache()
                .ToPageAsync(entity.page, entity.limit);
            list.Items = list.Items.Select(s => new { s.name, s.phone, s.id_card, image_url = Utils.GetImage_url(s.image_url, s.sex), s.create_date, s.drink, s.foods, s.fruits, s.habit, s.hobby, s.smoke, s.sex }).ToList();
            return list;
        }

        ///// <summary>
        ///// 获得列表
        ///// </summary>
        ///// <param name="entity">查询对象</param>
        ///// <returns></returns>
        //public async Task<Page<ArchivesExtend>> GetPagesAsync(ArchivesSearch entity)
        //{
        //    //获取用户信息
        //    var userInfo = new Tools.IdentityModels.GetUser().userInfo;

        //    var orderTypeStr = " Asc";
        //    if (entity.orderType == 1)
        //    {
        //        orderTypeStr = " Desc";
        //    }
        //    var tags = new List<int>();
        //    if (!string.IsNullOrEmpty(entity.tags))
        //    {
        //        tags = entity.tags.Split(',').Select(s => int.Parse(s)).ToList();
        //    }
        //    var ageStart = 0;
        //    int.TryParse(entity.ageStart, out ageStart);
        //    var ageEnd = 99999;
        //    int.TryParse(entity.ageEnd, out ageEnd);
        //    //获取档案信息、档案扩展信息、会员卡信息
        //    var list = await Db.Queryable<c_archives, c_archives_tag, c_archives_extend, c_archives_card, p_store, c_archives_preference>((a, at, ae, ac, s, ap) => new object[] { JoinType.Left, at.archives_id == a.id, JoinType.Left, a.id == ae.id, JoinType.Left, a.id == ac.archives_id, JoinType.Left, a.store_id == s.id, JoinType.Left, a.id == ap.archives_id })
        //        .Where((a, at, ae, ac, s, ap) => a.org_id == userInfo.org_id)
        //        .WhereIF(entity.state != -1, (a, at, ae, ac, s, ap) => a.state == entity.state)
        //        .WhereIF(entity.storeId != -1, (a, at, ae, ac, s, ap) => a.store_id == entity.storeId)
        //        .WhereIF(entity.isMe, (a, at, ae, ac, s, ap) => ae.to_director_id == userInfo.id)
        //        .WhereIF(entity.to_director_id > 0, (a, at, ae, ac, s, ap) => ae.to_director_id == entity.to_director_id)
        //        .WhereIF(!string.IsNullOrEmpty(entity.name), (a, at, ae, ac, s, ap) => a.name.Contains(entity.name) || a.spell.Contains(entity.name))
        //        .WhereIF(!string.IsNullOrEmpty(entity.sexCode), (a, at, ae, ac, s, ap) => a.sex_code == entity.sexCode)
        //        .WhereIF(!string.IsNullOrEmpty(entity.phone), (a, at, ae, ac, s, ap) => a.phone.Contains(entity.phone))
        //        .WhereIF(!string.IsNullOrEmpty(entity.idCard), (a, at, ae, ac, s, ap) => a.id_card.Contains(entity.idCard))
        //        .WhereIF(!string.IsNullOrEmpty(entity.maritalStatusCode), (a, at, ae, ac, s, ap) => ae.marital_status_code == entity.maritalStatusCode)
        //        .WhereIF(!string.IsNullOrEmpty(entity.zodiacCode), (a, at, ae, ac, s, ap) => a.zodiac_code == entity.zodiacCode)
        //        .WhereIF(!string.IsNullOrEmpty(entity.constellationCode), (a, at, ae, ac, s, ap) => a.constellation_code == entity.constellationCode)
        //        .WhereIF(!string.IsNullOrEmpty(entity.favourWayCode), (a, at, ae, ac, s, ap) => ae.favour_way_code == entity.favourWayCode)
        //        .WhereIF(!string.IsNullOrEmpty(entity.consumeHabitCode), (a, at, ae, ac, s, ap) => ae.consume_habit_code == entity.consumeHabitCode)
        //        .WhereIF(!string.IsNullOrEmpty(entity.ageStart), (a, at, ae, ac, s, ap) => a.age >= ageStart)
        //        .WhereIF(!string.IsNullOrEmpty(entity.ageEnd), (a, at, ae, ac, s, ap) => a.age <= ageEnd)
        //        .WhereIF(tags.Count > 0, (a, at, ae, ac, s, ap) => tags.Contains(at.tag_id))
        //        .WhereIF(!string.IsNullOrEmpty(entity.preference), (a, at, ae, ac, s, ap) => ap.text.Contains(entity.preference))
        //        .GroupBy((a, at, ae, ac, s, ap) => new { ae.address, a.age, a.birthday, ae.blood_type, ae.blood_type_code, ac.card_no, ae.city, ae.city_code, a.constellation, a.constellation_code, ae.consume_habit, ae.consume_habit_code, ae.contacts, ae.contacts_phone, ae.county, ae.county_code, a.creater, a.day, ae.email, ae.favour_way, ae.favour_way_code, ae.hobby, a.id_card, a.id, ae.income, ae.marital_status, ae.marital_status_code, a.month, a.name, ae.nation, ae.nation_code, ae.occupation, ae.occupation_code, a.org_id, a.phone, ae.province, ae.province_code, ae.qq, ae.remarks, a.sex, a.sex_code, ae.source, ae.source_code, a.spell, a.state, ae.street, a.store_id, ae.street_code, ae.to_director, a.type, a.type_code, ae.to_director_id, ae.wechat, a.zodiac, a.zodiac_code, a.create_date, store_name = s.name })
        //        .Select((a, at, ae, ac, s, ap) => new ArchivesExtend
        //        {
        //            address = ae.address,
        //            age = a.age,
        //            birthday = a.birthday,
        //            blood_type = ae.blood_type,
        //            blood_type_code = ae.blood_type_code,
        //            card_no = ac.card_no,
        //            city = ae.city,
        //            city_code = ae.city_code,
        //            constellation = a.constellation,
        //            constellation_code = a.constellation_code,
        //            consume_habit = ae.consume_habit,
        //            consume_habit_code = ae.consume_habit_code,
        //            contacts = ae.contacts,
        //            contacts_phone = ae.contacts_phone,
        //            county = ae.county,
        //            county_code = ae.county_code,
        //            creater = a.creater,
        //            day = a.day,
        //            email = ae.email,
        //            favour_way = ae.favour_way,
        //            favour_way_code = ae.favour_way_code,
        //            hobby = ae.hobby,
        //            id_card = a.id_card,
        //            id = a.id,
        //            income = ae.income,
        //            marital_status = ae.marital_status,
        //            marital_status_code = ae.marital_status_code,
        //            month = a.month,
        //            name = a.name,
        //            nation = ae.nation,
        //            nation_code = ae.nation_code,
        //            occupation = ae.occupation,
        //            occupation_code = ae.occupation_code,
        //            org_id = a.org_id,
        //            phone = a.phone,
        //            province = ae.province,
        //            province_code = ae.province_code,
        //            qq = ae.qq,
        //            remarks = ae.remarks,
        //            sex = a.sex,
        //            sex_code = a.sex_code,
        //            source = ae.source,
        //            source_code = ae.source_code,
        //            spell = a.spell,
        //            state = a.state,
        //            street = ae.street,
        //            store_id = a.store_id,
        //            street_code = ae.street_code,
        //            to_director = ae.to_director,
        //            type = a.type,
        //            type_code = a.type_code,
        //            to_director_id = ae.to_director_id,
        //            wechat = ae.wechat,
        //            zodiac = a.zodiac,
        //            zodiac_code = a.zodiac_code,
        //            create_date = a.create_date,
        //            store_name = s.name
        //        })
        //        .OrderBy(entity.order + orderTypeStr)
        //        .WithCache()
        //        .ToPageAsync(entity.page, entity.limit);
        //    //档案标签
        //    var archivesTags = await Db.Queryable<c_archives_tag>().WithCache().ToListAsync();
        //    //档案偏好
        //    var archivesPreference = await Db.Queryable<c_archives_preference>().WithCache().ToListAsync();
        //    //绑定档案标签、偏好
        //    list.Items = list.Items.Select(s => new ArchivesExtend
        //    {
        //        address = s.address,
        //        age = s.age,
        //        birthday = s.birthday,
        //        blood_type = s.blood_type,
        //        blood_type_code = s.blood_type_code,
        //        card_no = s.card_no,
        //        city = s.city,
        //        city_code = s.city_code,
        //        constellation = s.constellation,
        //        constellation_code = s.constellation_code,
        //        consume_habit = s.consume_habit,
        //        consume_habit_code = s.consume_habit_code,
        //        contacts = s.contacts,
        //        contacts_phone = s.contacts_phone,
        //        county = s.county,
        //        county_code = s.county_code,
        //        creater = s.creater,
        //        day = s.day,
        //        email = s.email,
        //        favour_way = s.favour_way,
        //        favour_way_code = s.favour_way_code,
        //        hobby = s.hobby,
        //        id_card = s.id_card,
        //        id = s.id,
        //        income = s.income,
        //        marital_status = s.marital_status,
        //        marital_status_code = s.marital_status_code,
        //        month = s.month,
        //        name = s.name,
        //        nation = s.nation,
        //        nation_code = s.nation_code,
        //        occupation = s.occupation,
        //        occupation_code = s.occupation_code,
        //        org_id = s.org_id,
        //        phone = s.phone,
        //        province = s.province,
        //        province_code = s.province_code,
        //        qq = s.qq,
        //        remarks = s.remarks,
        //        sex = s.sex,
        //        sex_code = s.sex_code,
        //        source = s.source,
        //        source_code = s.source_code,
        //        spell = s.spell,
        //        state = s.state,
        //        street = s.street,
        //        store_id = s.store_id,
        //        street_code = s.street_code,
        //        to_director = s.to_director,
        //        type = s.type,
        //        type_code = s.type_code,
        //        to_director_id = s.to_director_id,
        //        wechat = s.wechat,
        //        zodiac = s.zodiac,
        //        zodiac_code = s.zodiac_code,
        //        create_date = s.create_date,
        //        store_name = s.store_name,
        //        archivesTags = archivesTags.Where(w => w.archives_id == s.id).Select(ss => new archives_tag { tag = ss.tag, tag_id = ss.tag_id }).ToList(),
        //        archivesPreference = archivesPreference.Where(w => w.archives_id == s.id).Select(ss => ss.text).ToList()
        //    }).ToList();
        //    return list;
        //}

        /// <summary>
        /// 获得列表
        /// </summary>
        /// <param name="entity">查询对象</param>
        /// <returns></returns>
        public async Task<Page<ArchivesExtend>> GetPagesAsync(ArchivesSearch entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            //var tags = new List<int>();
            //if (!string.IsNullOrEmpty(entity.tags))
            //{
            //    tags = entity.tags.Split(',').Select(s => int.Parse(s)).ToList();
            //}
            var ageStart = 0;
            int.TryParse(entity.ageStart, out ageStart);
            var ageEnd = 99999;
            int.TryParse(entity.ageEnd, out ageEnd);
            //获取档案信息、档案扩展信息、会员卡信息
            var list = await Db.Queryable<c_archives, p_store, c_archives_supplement>((a, s, ars) => new object[] { JoinType.Left, a.store_id == s.id, JoinType.Left, a.id == ars.archives_id })
                .Where((a, s, ars) => a.org_id == userInfo.org_id)
                .WhereIF(entity.state != -1, (a, s, ars) => a.state == entity.state)
                .WhereIF(entity.storeId != -1, (a, s, ars) => a.store_id == entity.storeId)
                .WhereIF(entity.isMe, (a, s, ars) => a.to_director_id == userInfo.id)
                .WhereIF(entity.to_director_id > 0, (a, s, ars) => a.to_director_id == entity.to_director_id)
                .WhereIF(!string.IsNullOrEmpty(entity.name), (a, s, ars) => a.name.Contains(entity.name) || a.spell.Contains(entity.name))
                .WhereIF(!string.IsNullOrEmpty(entity.sexCode), (a, s, ars) => a.sex_code == entity.sexCode)
                .WhereIF(!string.IsNullOrEmpty(entity.phone), (a, s, ars) => a.phone.Contains(entity.phone))
                .WhereIF(!string.IsNullOrEmpty(entity.idCard), (a, s, ars) => a.id_card.Contains(entity.idCard))
                .WhereIF(!string.IsNullOrEmpty(entity.maritalStatusCode), (a, s, ars) => a.marital_status_code == entity.maritalStatusCode)
                .WhereIF(!string.IsNullOrEmpty(entity.zodiacCode), (a, s, ars) => a.zodiac_code == entity.zodiacCode)
                .WhereIF(!string.IsNullOrEmpty(entity.constellationCode), (a, s, ars) => a.constellation_code == entity.constellationCode)
                .WhereIF(!string.IsNullOrEmpty(entity.drink), (a, s, ars) => ars.drink.Contains(entity.drink))
                .WhereIF(!string.IsNullOrEmpty(entity.foods), (a, s, ars) => ars.foods.Contains(entity.foods))
                .WhereIF(!string.IsNullOrEmpty(entity.fruits), (a, s, ars) => ars.fruits.Contains(entity.fruits))
                .WhereIF(!string.IsNullOrEmpty(entity.habit), (a, s, ars) => ars.habit.Contains(entity.habit))
                .WhereIF(!string.IsNullOrEmpty(entity.hobby), (a, s, ars) => ars.hobby.Contains(entity.hobby))
                .WhereIF(!string.IsNullOrEmpty(entity.smoke), (a, s, ars) => ars.smoke.Contains(entity.smoke))
                .WhereIF(!string.IsNullOrEmpty(entity.ageStart), (a, s, ars) => a.age >= ageStart)
                .WhereIF(!string.IsNullOrEmpty(entity.ageEnd), (a, s, ars) => a.age <= ageEnd)
                //.WhereIF(tags.Count > 0, (a, at, s, ars) => tags.Contains(at.tag_id))
                //.WhereIF(!string.IsNullOrEmpty(entity.preference), (a, at, s, ars) => ap.text.Contains(entity.preference))
                //.GroupBy((a, s, ars) => new { a.address, a.age, a.birthday, a.aboid, a.abo, a.rhid, a.rh, a.card_no, a.city, a.city_code, a.constellation, a.constellation_code, a.contacts, a.contacts_phone, a.county, a.county_code, a.creater, a.day, a.email, a.id_card, a.id, a.income, a.marital_status, a.marital_status_code, a.month, a.name, a.nation, a.nation_code, a.occupation, a.occupation_code, a.org_id, a.phone, a.province, a.province_code, a.qq, a.remarks, a.sex, a.sex_code, a.spell, a.state, a.street, a.store_id, a.street_code, a.to_director, a.type, a.type_code, a.to_director_id, a.wechat, a.zodiac, a.zodiac_code, a.create_date, store_name = s.name, ars.drink, ars.fruits, ars.habit, ars.hobby, ars.smoke, ars.foods })
                .Select((a, s, ars) => new ArchivesExtend { image_url = a.image_url, address = a.address, age = a.age, birthday = a.birthday, aboid = a.aboid, abo = a.abo, rhid = a.rhid, rh = a.rh, card_no = a.card_no, city = a.city, city_code = a.city_code, constellation = a.constellation, constellation_code = a.constellation_code, contacts = a.contacts, contacts_phone = a.contacts_phone, county = a.county, county_code = a.county_code, creater = a.creater, day = a.day, email = a.email, id_card = a.id_card, id = a.id, income = a.income, marital_status = a.marital_status, marital_status_code = a.marital_status_code, month = a.month, name = a.name, nation = a.nation, nation_code = a.nation_code, occupation = a.occupation, occupation_code = a.occupation_code, org_id = a.org_id, phone = a.phone, province = a.province, province_code = a.province_code, qq = a.qq, remarks = a.remarks, sex = a.sex, sex_code = a.sex_code, spell = a.spell, state = a.state, street = a.street, store_id = a.store_id, street_code = a.street_code, to_director = a.to_director, type = a.type, type_code = a.type_code, to_director_id = a.to_director_id, wechat = a.wechat, zodiac = a.zodiac, zodiac_code = a.zodiac_code, create_date = a.create_date, store_name = s.name, drink = ars.drink, foods = ars.foods, fruits = ars.fruits, habit = ars.habit, hobby = ars.hobby, smoke = ars.smoke, emergencyno = a.emergencyno, carno = a.carno, education = a.education, educationid = a.educationid, flag = a.flag, flagid = a.flagid, grade_id = a.grade_id, no = a.no, relation = a.relation, relationid = a.relationid, virtual_no = a.virtual_no, archives_class = a.archives_class, class_code = a.class_code, source = a.source, coupon_no = a.coupon_no, source_code = a.source_code })
                .OrderBy(entity.order + orderTypeStr)
                .WithCache()
                .ToPageAsync(entity.page, entity.limit);
            ////档案标签
            //var archivesTags = await Db.Queryable<c_archives_tag>().WithCache().ToListAsync();
            ////档案偏好
            //var archivesPreference = await Db.Queryable<c_archives_preference>().WithCache().ToListAsync();
            ////绑定档案标签、偏好
            //list.Items = list.Items.Select(s => new ArchivesExtend
            //{
            //    address = s.address,
            //    age = s.age,
            //    birthday = s.birthday,
            //    blood_type = s.blood_type,
            //    blood_type_code = s.blood_type_code,
            //    card_no = s.card_no,
            //    city = s.city,
            //    city_code = s.city_code,
            //    constellation = s.constellation,
            //    constellation_code = s.constellation_code,
            //    consume_habit = s.consume_habit,
            //    consume_habit_code = s.consume_habit_code,
            //    contacts = s.contacts,
            //    contacts_phone = s.contacts_phone,
            //    county = s.county,
            //    county_code = s.county_code,
            //    creater = s.creater,
            //    day = s.day,
            //    email = s.email,
            //    favour_way = s.favour_way,
            //    favour_way_code = s.favour_way_code,
            //    hobby = s.hobby,
            //    id_card = s.id_card,
            //    id = s.id,
            //    income = s.income,
            //    marital_status = s.marital_status,
            //    marital_status_code = s.marital_status_code,
            //    month = s.month,
            //    name = s.name,
            //    nation = s.nation,
            //    nation_code = s.nation_code,
            //    occupation = s.occupation,
            //    occupation_code = s.occupation_code,
            //    org_id = s.org_id,
            //    phone = s.phone,
            //    province = s.province,
            //    province_code = s.province_code,
            //    qq = s.qq,
            //    remarks = s.remarks,
            //    sex = s.sex,
            //    sex_code = s.sex_code,
            //    source = s.source,
            //    source_code = s.source_code,
            //    spell = s.spell,
            //    state = s.state,
            //    street = s.street,
            //    store_id = s.store_id,
            //    street_code = s.street_code,
            //    to_director = s.to_director,
            //    type = s.type,
            //    type_code = s.type_code,
            //    to_director_id = s.to_director_id,
            //    wechat = s.wechat,
            //    zodiac = s.zodiac,
            //    zodiac_code = s.zodiac_code,
            //    create_date = s.create_date,
            //    store_name = s.store_name,
            //    archivesTags = archivesTags.Where(w => w.archives_id == s.id).Select(ss => new archives_tag { tag = ss.tag, tag_id = ss.tag_id }).ToList(),
            //    archivesPreference = archivesPreference.Where(w => w.archives_id == s.id).Select(ss => ss.text).ToList()
            //}).ToList();
            return list;
        }

        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ModifyAsync(Archives archives)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var c_archives = archives.archives;
            var archivesSupplement = archives.archivesSupplement;
            //var c_archives_extend = archives.archivesExtend;
            //var tags = archives.archivesTags;
            //var preference = archives.preference;

            var exist = await Db.Queryable<c_archives>().Where(w => w.id_card == c_archives.id_card && w.org_id == userInfo.org_id && w.id != c_archives.id).WithCache().AnyAsync();
            if (exist)
            {
                throw new MessageException("身份证号已存在，请确认信息是否正确");
            }

            var result = Db.Ado.UseTran(() =>
            {
                #region 档案信息
                //var info = Utils.GetInfoFromIDCard(c_archives.id_card);
                //if (!info.IsSuccess)
                //{
                //    throw new MessageException(info.Message);
                //}
                //c_archives.org_id = userInfo.org_id;
                c_archives.spell = ToSpell.GetFirstPinyin(c_archives.name);
                //c_archives.age = info.age;
                //c_archives.month = info.month;
                //c_archives.day = info.day;
                //c_archives.birthday = info.birthday;
                //c_archives.sex = info.sex;
                //c_archives.creater_id = userInfo.id;
                //c_archives.creater = userInfo.name;
                //c_archives.create_date = DateTime.Now;
                //c_archives.constellation = info.constellation;
                //c_archives.zodiac = info.zodiac;

                //编辑档案信息
                if (archives.is_change_password)
                {
                    c_archives.password = MetarnetRegex.Encrypt(c_archives.phone.Substring(c_archives.phone.Length - 6));
                    Db.Updateable(c_archives)
                    .IgnoreColumns(it => new { it.org_id, it.no, it.store_id, it.state, it.grade_id, it.virtual_no, it.creater_id, it.creater, it.create_date })
                    .Where(a => a.id == c_archives.id)
                    .RemoveDataCache()
                    .EnableDiffLogEvent()
                    .ExecuteCommand();
                }
                else
                {
                    Db.Updateable(c_archives)
                    .IgnoreColumns(it => new { it.password, it.org_id, it.no, it.store_id, it.state, it.grade_id, it.virtual_no, it.creater_id, it.creater, it.create_date })
                    .Where(a => a.id == c_archives.id)
                    .RemoveDataCache()
                    .EnableDiffLogEvent()
                    .ExecuteCommand();
                }

                if (archivesSupplement != null)
                {
                    //编辑档案附页
                    archivesSupplement.archives_id = c_archives.id;
                    Db.Updateable(archivesSupplement).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                }

                //if (c_archives_extend != null)
                //{
                //    //编辑档案详情信息
                //    c_archives_extend.id = c_archives.id;
                //    Db.Updateable(c_archives_extend).IgnoreColumns(it => new { it.to_director, it.to_director_id }).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                //}
                ////删除档案标签
                //Db.Deleteable<c_archives_tag>().Where(a => a.archives_id == c_archives.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                //if (tags.Count > 0)
                //{
                //    //添加档案标签
                //    var list = tags.Select(s => new c_archives_tag { archives_id = c_archives.id, tag = s.tag, tag_id = s.tag_id }).ToList();
                //    Db.Insertable(list).ExecuteCommand();
                //    redisCache.RemoveAll<c_archives_tag>();
                //}
                ////删除档案偏好
                //Db.Deleteable<c_archives_preference>().Where(a => a.archives_id == c_archives.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                //if (preference.Count > 0)
                //{
                //    //添加档案偏好
                //    var list = preference.Select(s => new c_archives_preference { archives_id = c_archives.id, text = s }).ToList();
                //    Db.Insertable(list).ExecuteCommand();
                //    redisCache.RemoveAll<c_archives_preference>();
                //}

                #endregion
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <returns></returns>
        public async Task<int> ModifyDisabledAsync(c_archives archives)
        {
            return await Db.Updateable<c_archives>()
                .SetColumns(s => new c_archives { state = archives.state })
                .Where(w => w.id == archives.id)
                .RemoveDataCache()
                .EnableDiffLogEvent()
                .ExecuteCommandAsync();
        }

        /// <summary>
        /// 修改一条数据（移动端）
        /// </summary>
        /// <returns></returns>
        public async Task<bool> MobileModifyAsync(MobileArchives archives)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var c_archives = new c_archives { id = archives.id, id_card = archives.id_card, name = archives.name, phone = archives.phone, flagid = "1", flag = "身份证" };
            var archivesSupplement = new c_archives_supplement { drink = archives.drink, foods = archives.foods, fruits = archives.fruits, habit = archives.habit, hobby = archives.hobby, smoke = archives.smoke };

            var exist = await Db.Queryable<c_archives>().Where(w => w.id_card == c_archives.id_card && w.org_id == userInfo.org_id && w.id != c_archives.id).WithCache().AnyAsync();
            if (exist)
            {
                throw new MessageException("身份证号已存在，请确认信息是否正确");
            }

            var result = Db.Ado.UseTran(() =>
            {
                #region 档案信息
                var info = Utils.GetInfoFromIDCard(c_archives.id_card);
                if (!info.IsSuccess)
                {
                    throw new MessageException(info.Message);
                }
                c_archives.spell = ToSpell.GetFirstPinyin(c_archives.name);
                c_archives.age = info.age;
                c_archives.month = info.month;
                c_archives.day = info.day;
                c_archives.birthday = info.birthday;
                c_archives.sex = info.sex;
                c_archives.sex_code = GetSex(info.sex);

                Db.Updateable(c_archives)
                .SetColumns(it => new c_archives { name = c_archives.name, spell = c_archives.spell, age = c_archives.age, month = c_archives.month, day = c_archives.day, birthday = c_archives.birthday, sex = c_archives.sex, sex_code = c_archives.sex_code, id_card = c_archives.id_card })
                .Where(a => a.id == c_archives.id)
                .RemoveDataCache()
                .EnableDiffLogEvent()
                .ExecuteCommand();

                if (archivesSupplement != null)
                {
                    //编辑档案附页
                    archivesSupplement.archives_id = c_archives.id;
                    Db.Updateable(archivesSupplement).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                }

                #endregion
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 修改一条数据（客户端）
        /// </summary>
        /// <returns></returns>
        public async Task<int> ArcModifyAsync(ArcArchives archives)
        {
            //获取客户信息
            var archivesInfo = new Tools.IdentityModels.GetArchives().archives;

            var exist = await Db.Queryable<c_archives>().Where(w => w.id_card == archives.id_card && w.org_id == archivesInfo.org_id && w.id != archivesInfo.id).WithCache().AnyAsync();
            if (exist)
            {
                throw new MessageException("身份证号已存在，请确认信息是否正确");
            }
            var info = Utils.GetInfoFromIDCard(archives.id_card);
            if (!info.IsSuccess)
            {
                throw new MessageException(info.Message);
            }
            var spell = ToSpell.GetFirstPinyin(archives.name);
            var sex_code = GetSex(info.sex);
            return await Db.Updateable<c_archives>()
                .SetColumns(it => new c_archives { name = archives.name, spell = spell, age = info.age, month = info.month, day = info.day, birthday = info.birthday, sex = info.sex, sex_code = sex_code, id_card = archives.id_card, address = archives.address })
                .Where(a => a.id == archivesInfo.id)
                .RemoveDataCache()
                .EnableDiffLogEvent()
                .ExecuteCommandAsync();
        }

        /// <summary>
        /// 根据Id获取客户头像（客户端）
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetPhotoAsync()
        {
            var archivesInfo = new Tools.IdentityModels.GetArchives().archives;
            var archives = await Db.Queryable<c_archives>().Where(w => w.id == archivesInfo.id).Select(w => new { w.image_url, w.sex }).FirstAsync();
            var url = archives.image_url;
            return Utils.GetImage_url(url, archives.sex);
        }

        /// <summary>
        /// 根据Id修改客户头像（客户端）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> ModifyPhotoAsync(ArcPhoto entity)
        {
            var archivesInfo = new Tools.IdentityModels.GetArchives().archives;
            return await Db.Updateable<c_archives>().SetColumns(s => new c_archives { image_url = entity.url }).Where(w => w.id == archivesInfo.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="org_id">集团ID</param>
        /// <param name="code">短信验证码</param>
        /// <returns></returns>
        public async Task<dynamic> Login(string userName, string password, int org_id, string code)
        {
            p_sms_send_record getCode = new p_sms_send_record();
            //验证码登录
            if (!String.IsNullOrEmpty(code))
            {
                //查询短信验证码
                getCode = await Db.Queryable<p_sms_send_record>().Where(a => a.phone_no == userName && a.scene == "登录确认验证码" && a.expire_time > DateTime.Now).OrderBy(a => a.expire_time, SqlSugar.OrderByType.Desc).FirstAsync();

                if (getCode == null)
                {
                    throw new MessageException("请重新获取验证码!");
                }

                //手机验证码
                if (code != getCode.code)
                {
                    throw new MessageException("验证码错误!");
                }
            }

            //获取用户登录信息
            var user = await Db.Queryable<c_archives>().Where(a => a.phone == userName && a.org_id == org_id).FirstAsync();
            if (user == null)
            {
                throw new MessageException("用户不存在!");
            }

            if (string.IsNullOrEmpty(code) && password != user.password)
            {
                throw new MessageException("密码错误!");
            }

            if (!string.IsNullOrEmpty(code))
            {
                //修改验证码时间
                getCode.expire_time = DateTime.Now;
                await Db.Updateable(getCode).RemoveDataCache().EnableDiffLogEvent(true).ExecuteCommandAsync();
            }
            var log = new r_logs { code = DateTime.Now.ToString("yyyyMMddHHmmssffff"), content = "", date = DateTime.Now, ip = Utils.GetIp(), parameter = "", state = 1, url = Utils.GetUrl(), creater = user.name, creater_id = user.id, after_data = "", before_data = "", type = "客户登录" };

            await Db.Insertable(log).ExecuteCommandAsync();
            redisCache.RemoveAll<r_logs>();

            var token = MetarnetRegex.SHA512Encrypt("archives-" + user.id + "-" + org_id + "-" + DateTime.Now);

            redisCache.Add(token, user, 3600);

            return new { token, user.id, user.name, user.phone };
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> UpdatePassword(ArcLoginPassword entity)
        {
            //获客户信息
            var archivesInfo = new Tools.IdentityModels.GetArchives().archives;

            if (archivesInfo == null)
            {
                throw new MessageException("客户信息无效!");
            }

            if (entity == null)
            {
                throw new MessageException("传值错误!");
            }
            //验证原密码是否正确
            var isPwd = await Db.Queryable<c_archives>().Where(a => a.id == archivesInfo.id && a.org_id == entity.org_id).Select(a => a.password).WithCache().FirstAsync();
            if (entity.pwd != isPwd)
            {
                throw new MessageException("原密码错误!");
            }

            //修改密码
            //获取修改返回条数
            var result = await Db.Updateable<c_archives>().SetColumns(s => new c_archives { password = entity.passwd }).Where(it => it.id == archivesInfo.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
            if (result <= 0)
            {
                throw new MessageException("密码修改失败!");
            }
            return result;
        }

        /// <summary>
        /// 忘记密码
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> ModifyPassword(ArcLoginPassword entity)
        {
            //查询短信验证码
            var getCode = await Db.Queryable<p_sms_send_record>().Where(a => a.phone_no == entity.phone_no && a.scene == "修改密码验证码" && a.expire_time > DateTime.Now).OrderBy(a => a.expire_time, SqlSugar.OrderByType.Desc).FirstAsync();

            if (getCode == null)
            {
                throw new MessageException("请重新获取验证码!");
            }

            //手机验证码
            if (entity.code != getCode.code && !string.IsNullOrEmpty(entity.code))
            {
                throw new MessageException("验证码错误!");
            }
            if (entity.passwd != entity.passwd2)
            {
                throw new MessageException("两次输入密码不一致!");
            }
            //验证用户
            await GetArc(entity.phone_no);

            //修改密码
            //获取修改返回条数
            var result = await Db.Updateable<c_archives>().SetColumns(s => new c_archives { password = entity.passwd }).Where(it => it.phone == entity.phone_no).RemoveDataCache().EnableDiffLogEvent(true).ExecuteCommandAsync();
            if (result <= 0)
            {
                throw new MessageException("密码修改失败!");
            }
            //修改验证码时间
            await Db.Updateable<p_sms_send_record>().SetColumns(s => new p_sms_send_record { expire_time = DateTime.Now }).Where(w => w.code == entity.code && w.phone_no == entity.phone_no && w.send_time == getCode.send_time).RemoveDataCache().EnableDiffLogEvent(true).ExecuteCommandAsync();

            return result;
        }

        /// <summary>
        /// 获取客户，是否存在
        /// </summary>
        /// <param name="phone">电话号码</param>
        /// <returns></returns>
        public async Task<bool> GetArc(string phone)
        {
            if (String.IsNullOrEmpty(phone))
            {
                throw new MessageException("手机号不能为空!");
            }
            var isExit = await Db.Queryable<c_archives>().WithCache().AnyAsync(a => a.phone == phone);
            if (!isExit)
            {
                throw new MessageException("客户不存在!");
            }
            return isExit;
        }

        /// <summary>
        /// 验证券码是否存在
        /// </summary>
        /// <param name="coupon_no"></param>
        /// <returns></returns>
        public async Task<object> GetCoupon(string coupon_no)
        {
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var isExist = await Db.Queryable<c_new_coupon_detials, c_activity>((n, a) => new object[] { JoinType.Left, n.activity_id == a.id })
                           .Where((n, a) => n.org_id == userInfo.org_id && n.use_state == 3 && a.start_date <= DateTime.Now.Date && a.end_date >= DateTime.Now.Date && n.no == coupon_no.Trim() && a.state == 1)
                           .Select((n, a) => new { n.money, n.no, a.name })
                           .WithCache()
                           .FirstAsync();
            if (isExist == null)
            {
                return new { Exist = false };
            }

            return new { Exist = true, coupon_money = isExist.money, coupon_ = isExist.no, activity_name = isExist.name };
        }

        /// <summary>
        /// 档案导入
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> ArchivesImportAsync(ArchivesImport entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var num = 1;
            var result = Db.Ado.UseTran(() =>
            {
                //获取最低等级
                var level = Db.Queryable<c_archives_level>().Where(w => w.org_id == userInfo.org_id && w.special == 3).OrderBy(o => o.min_money).WithCache().First();
                if (level == null)
                {
                    throw new MessageException("请先设置会员等级");
                }

                entity.archives.ForEach(item =>
                {
                    //手机号是否存在，存在则不导入
                    var exist = Db.Queryable<c_archives>().Where(w => w.phone == item.phone && w.org_id == userInfo.org_id).WithCache().Any();
                    if (!exist)
                    {
                        //身份证是否存在，存在则不导入
                        if (!string.IsNullOrEmpty(item.id_card))
                        {
                            exist = Db.Queryable<c_archives>().Where(w => w.id_card == item.id_card && w.org_id == userInfo.org_id).WithCache().Any();
                        }
                        if (!exist)
                        {
                            var to_director = Db.Queryable<p_employee>().Where(w => w.org_id == userInfo.org_id && w.phone_no == item.to_director_phone).WithCache().First();

                            var this_no = DateTime.Now.ToString("yyyyMMddHHmmssf") + num.ToString().PadLeft(3, '0');
                            var c_archives = new c_archives { id_card = item.id_card, name = item.name, phone = item.phone, flagid = "1", flag = "身份证", store_id = entity.store_id, org_id = userInfo.org_id, card_no = "", carno = "", address = "", emergencyno = "", type_code = "1", type = "潜在客户", no = this_no, virtual_no = "vn" + this_no, spell = ToSpell.GetFirstPinyin(item.name), password = MetarnetRegex.Encrypt(item.phone.Substring(item.phone.Length - 6)), age = item.age, birthday = item.birthday, constellation = item.constellation, constellation_code = item.constellation_code, creater = userInfo.name, creater_id = userInfo.id, create_date = DateTime.Now, month = item.month, sex = item.sex, sex_code = item.sex_code, state = 1, grade_id = level.id, zodiac = item.zodiac, zodiac_code = item.zodiac_code };
                            if (to_director != null)
                            {
                                c_archives.to_director = to_director.name;
                                c_archives.to_director_id = to_director.id;
                            }
                            //添加档案信息返回Id
                            var archives_id = Db.Insertable(c_archives).ExecuteReturnIdentity();
                            redisCache.RemoveAll<c_archives>();

                            #region 档案转移
                            if (to_director != null)
                            {
                                var to_store_name = Db.Queryable<p_store>().Where(w => w.id == c_archives.store_id).WithCache().First().name;
                                var c_maintain = new c_maintain
                                {
                                    archives = c_archives.name,
                                    id_card = c_archives.id_card,
                                    archives_id = archives_id,
                                    archives_phone = c_archives.phone,
                                    cause = "创建负责人",
                                    creater = userInfo.name,
                                    creater_id = userInfo.id,
                                    create_date = DateTime.Now,
                                    from_director = "",
                                    from_director_id = 0,
                                    org_id = userInfo.org_id,
                                    from_store_id = 0,
                                    from_store_name = "",
                                    to_store_id = c_archives.store_id,
                                    to_store_name = to_store_name,
                                    to_director = c_archives.to_director,
                                    to_director_id = c_archives.to_director_id
                                };
                                Db.Insertable(c_maintain).RemoveDataCache().ExecuteCommand();
                            }
                            #endregion

                            #region 档案账户
                            var pwd = MetarnetRegex.Encrypt(c_archives.phone.Substring(c_archives.phone.Length - 6));
                            var rate = 0;
                            var code = MetarnetRegex.Encrypt(MetarnetRegex.SHA512Encrypt((archives_id + rate + item.balance).ToString()));

                            //档案账户
                            var account = new c_account { amount = 0, archives_id = archives_id, balance = item.balance, consume = 0, coupon = 0, integral = 0, noneamount = 0, password = pwd, rate = rate, recharge = 0, salseamount = 0, settleamount = 0, code = code, total_coupon = 0, total_integral = 0 };
                            Db.Insertable(account).RemoveDataCache().ExecuteCommand();
                            #endregion
                        }
                    }
                });
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 设置会员等级
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> SetLevelAsync(c_archives entity)
        {
            return await Db.Updateable<c_archives>().SetColumns(s => new c_archives { grade_id = entity.grade_id }).Where(w => w.id == entity.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
        }
    }
}
