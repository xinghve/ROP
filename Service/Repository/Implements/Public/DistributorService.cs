using Models.DB;
using Models.View.Mobile;
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
    /// 分销人员
    /// </summary>
    public class DistributorService : DbContext, IDistributorService
    {
        /// <summary>
        /// 根据分销人员ID获取信息
        /// </summary>
        /// <param name="distributor_id">分销人员ID</param>
        /// <returns></returns>
        public async Task<p_distributor> GetDistributorAsync(int distributor_id)
        {
            return await Db.Queryable<p_distributor>().Where(w => w.id == distributor_id).WithCache().FirstAsync();
        }

        /// <summary>
        /// 获得分页列表
        /// </summary>
        /// <param name="entity">查询对象</param>
        /// <returns></returns>
        public async Task<Page<DistributorPageModel>> GetPagesAsync(DistributorSearch entity)
        {
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            return await Db.Queryable<p_distributor,p_store,c_archives>((w,s,a)=>new object[] {JoinType.Left,w.store_id==s.id,JoinType.Left,w.director_id==a.id })
                .Where((w, s, a) => w.org_id== userInfo.org_id)
                .WhereIF(entity.store_id>0, (w, s, a) => w.store_id==entity.store_id)
                .WhereIF(!string.IsNullOrEmpty(entity.sex_name), (w, s, a) => w.sex_name.Contains(entity.sex_name))
                .WhereIF(!string.IsNullOrEmpty(entity.director), (w, s, a) => w.director.Contains(entity.director))
                .WhereIF(entity.startTime!=null,(w,s,a)=>w.create_time>=entity.startTime)
                .WhereIF(entity.endTime!=null,(w,s,a)=>w.create_time<=entity.endTime)
                .WhereIF(!string.IsNullOrEmpty(entity.search_condition), (w, s, a) => (w.name.Contains(entity.search_condition) ||w.id_no.Contains(entity.search_condition) ||w.pinyin.Contains(entity.search_condition) ||w.phone_no.Contains(entity.search_condition)||a.phone.Contains(entity.search_condition)))
                .Select((w,s,a)=>new DistributorPageModel { address=w.address, director=w.director, amount=w.amount, create_time=w.create_time, director_id=w.director_id, expire_time=w.expire_time, id=w.id, id_no=w.id_no, image_url=w.image_url, name=w.name, noneamount=w.noneamount, open_id=w.open_id, org_id=w.org_id, phone_no=w.phone_no, pinyin=w.pinyin, settleamount=w.settleamount, sex_name=w.sex_name, store_id=w.store_id, store_name=s.name, dir_phone=a.phone })
                .OrderBy(entity.order + orderTypeStr)
                .WithCache()
                .ToPageAsync(entity.page, entity.limit);
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="org_id"></param>
        /// <param name="code"></param>
        /// <param name="open_id">微信open_id</param>
        /// <returns></returns>
        public async Task<dynamic> Login(string userName, string password, int org_id, string code, string open_id)
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
            var user = await Db.Queryable<p_distributor>().Where(a => a.phone_no == userName && a.org_id == org_id).FirstAsync();
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
            var log = new r_logs { code = DateTime.Now.ToString("yyyyMMddHHmmssffff"), content = "", date = DateTime.Now, ip = Utils.GetIp(), parameter = "", state = 1, url = Utils.GetUrl(), creater = user.name, creater_id = user.id, after_data = "", before_data = "", type = "分销人员登录" };

            await Db.Insertable(log).ExecuteCommandAsync();
            redisCache.RemoveAll<r_logs>();

            var token = MetarnetRegex.SHA512Encrypt("distributor-" + user.id + "-" + org_id + "-" + DateTime.Now);

            redisCache.Add(token, user, 3600);

            //修改分销人员当前对应微信open_id
            await Db.Updateable<p_distributor>().SetColumns(s => new p_distributor { open_id = open_id }).Where(a => a.phone_no == userName && a.org_id == org_id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();

            return new { token, user.id, user.name, user.phone_no };
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <returns></returns>
        public async Task<int> RegisterAsync(DistributorRegister entity)
        {
            //查询短信验证码
            var getCode = await Db.Queryable<p_sms_send_record>().Where(a => a.phone_no == entity.phone_no && a.scene == "用户注册验证码" && a.expire_time > DateTime.Now).OrderBy(a => a.expire_time, SqlSugar.OrderByType.Desc).FirstAsync();

            if (getCode == null)
            {
                throw new MessageException("请重新获取验证码!");
            }

            //手机验证码
            if (entity.code != getCode.code && !string.IsNullOrEmpty(entity.code))
            {
                throw new MessageException("验证码错误!");
            }

            var exist = await Db.Queryable<p_distributor>().Where(w => (w.id_no == entity.id_no || w.phone_no == entity.phone_no) && w.org_id == entity.org_id).WithCache().AnyAsync();
            if (exist)
            {
                throw new MessageException("身份证号或手机号已存在，请确认信息是否正确");
            }

            //查询负责人姓名
            var director = await Db.Queryable<p_employee>().Where(w => w.id == entity.director_id).Select(s => s.name).WithCache().FirstAsync();

            var distributor = new p_distributor { id_no = entity.id_no, name = entity.name, phone_no = entity.phone_no, store_id = entity.store_id, org_id = entity.org_id, address = "", amount = 0, create_time = DateTime.Now, director = director, director_id = entity.director_id, expire_time = DateTime.Parse("3000-12-31 23:59:59"), image_url = "", noneamount = 0, open_id = "", password = MetarnetRegex.Encrypt(entity.phone_no.Substring(entity.phone_no.Length - 6)), pinyin = ToSpell.GetFirstPinyin(entity.name), settleamount = 0 };

            var info = Utils.GetInfoFromIDCard(entity.id_no);
            if (!info.IsSuccess)
            {
                throw new MessageException(info.Message);
            }
            distributor.sex_name = info.sex;
            distributor.sex_code = GetSex(info.sex);
            redisCache.RemoveAll<p_distributor>();
            return await Db.Insertable(distributor).ExecuteReturnIdentityAsync();
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
        /// 修改密码
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> UpdatePassword(ArcLoginPassword entity)
        {
            //获分销人员信息
            var distributorInfo = new Tools.IdentityModels.GetDistributor().Distributor;

            if (distributorInfo == null)
            {
                throw new MessageException("分销人员信息无效!");
            }

            if (entity == null)
            {
                throw new MessageException("传值错误!");
            }
            //验证原密码是否正确
            var isPwd = await Db.Queryable<p_distributor>().Where(a => a.id == distributorInfo.id && a.org_id == entity.org_id).Select(a => a.password).WithCache().FirstAsync();
            if (entity.pwd != isPwd)
            {
                throw new MessageException("原密码错误!");
            }

            //修改密码
            //获取修改返回条数
            var result = await Db.Updateable<p_distributor>().SetColumns(s => new p_distributor { password = entity.passwd }).Where(it => it.id == distributorInfo.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
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
            await GetDistributor(entity.phone_no);

            //修改密码
            //获取修改返回条数
            var result = await Db.Updateable<p_distributor>().SetColumns(s => new p_distributor { password = entity.passwd }).Where(it => it.phone_no == entity.phone_no).RemoveDataCache().EnableDiffLogEvent(true).ExecuteCommandAsync();
            if (result <= 0)
            {
                throw new MessageException("密码修改失败!");
            }
            //修改验证码时间
            await Db.Updateable<p_sms_send_record>().SetColumns(s => new p_sms_send_record { expire_time = DateTime.Now }).Where(w => w.code == entity.code && w.phone_no == entity.phone_no && w.send_time == getCode.send_time).RemoveDataCache().EnableDiffLogEvent(true).ExecuteCommandAsync();

            return result;
        }

        /// <summary>
        /// 获取分销人员，是否存在
        /// </summary>
        /// <param name="phone">电话号码</param>
        /// <returns></returns>
        public async Task<bool> GetDistributor(string phone)
        {
            if (String.IsNullOrEmpty(phone))
            {
                throw new MessageException("手机号不能为空!");
            }
            var isExit = await Db.Queryable<p_distributor>().WithCache().AnyAsync(a => a.phone_no == phone);
            if (!isExit)
            {
                throw new MessageException("分销人员不存在!");
            }
            return isExit;
        }

        /// <summary>
        /// 根据Id获取客户头像（分销人员端）
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetPhotoAsync()
        {
            var distributorInfo = new Tools.IdentityModels.GetDistributor().Distributor;
            var distributor = await Db.Queryable<p_distributor>().Where(w => w.id == distributorInfo.id).Select(w => new { w.image_url, w.sex_name }).FirstAsync();
            var url = distributor.image_url;
            return Utils.GetImage_url(url, distributor.sex_name);
        }

        /// <summary>
        /// 根据Id修改客户头像（分销人员端）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> ModifyPhotoAsync(ArcPhoto entity)
        {
            var distributorInfo = new Tools.IdentityModels.GetDistributor().Distributor;
            return await Db.Updateable<p_distributor>().SetColumns(s => new p_distributor { image_url = entity.url }).Where(w => w.id == distributorInfo.id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
        }

        /// <summary>
        /// 修改一条数据（客户端）
        /// </summary>
        /// <returns></returns>
        public async Task<int> ModifyAsync(p_distributor distributor)
        {
            //获取客户信息
            var distributorInfo = new Tools.IdentityModels.GetDistributor().Distributor;

            var exist = await Db.Queryable<p_distributor>().Where(w => w.id_no == distributor.id_no && w.org_id == distributorInfo.org_id && w.id != distributorInfo.id).WithCache().AnyAsync();
            if (exist)
            {
                throw new MessageException("身份证号已存在，请确认信息是否正确");
            }
            var info = Utils.GetInfoFromIDCard(distributor.id_no);
            if (!info.IsSuccess)
            {
                throw new MessageException(info.Message);
            }
            var spell = ToSpell.GetFirstPinyin(distributor.name);
            var sex_code = GetSex(info.sex);
            return await Db.Updateable<p_distributor>()
                .SetColumns(it => new p_distributor { name = distributor.name, pinyin = spell, sex_name = info.sex, sex_code = sex_code, id_no = distributor.id_no, address = distributor.address })
                .Where(a => a.id == distributorInfo.id)
                .RemoveDataCache()
                .EnableDiffLogEvent()
                .ExecuteCommandAsync();
        }
        /// <summary>
        /// 分销人员修改状态
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> ModifyStateAsync(DistributorModify entity)
        {
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            if (entity.distributor_state==0)
            {
                return await Db.Updateable<p_distributor>()
                               .SetColumns(w => w.expire_time == DateTime.Now)
                               .Where(w => w.store_id == entity.store_id && w.org_id == userInfo.org_id)
                               .RemoveDataCache()
                               .EnableDiffLogEvent()
                               .ExecuteCommandAsync();
            }
            else if (entity.distributor_state==1)
            {
                var exTime = DateTime.Parse("3000-12-31 23:59:59");
                return await Db.Updateable<p_distributor>()
                              .SetColumns(w => w.expire_time == exTime)
                              .Where(w => w.store_id == entity.store_id && w.org_id == userInfo.org_id)
                              .RemoveDataCache()
                              .EnableDiffLogEvent()
                              .ExecuteCommandAsync();
            }
            else
            {
                return -1;
            }
        }
    }
}
