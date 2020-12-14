using Models.DB;
using Models.View.Crm;
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
    /// 会员等级业务
    /// </summary>
    public class ALevelService : DbContext, IALevelService
    {
        /// <summary>
        /// 获取会员等级分页数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Page<c_archives_level>> GetPageAsync(SearchMl entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var levelList = Db.Queryable<c_archives_level>()
                .Where(w => w.org_id == userInfo.org_id)
                .WhereIF(!string.IsNullOrEmpty(entity.name), w => w.name.Contains(entity.name))
                .WhereIF(entity.startTime != null, w => w.create_date >= entity.startTime)
                .WhereIF(entity.endTime != null, w => w.create_date <= entity.endTime);

            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }

            levelList = levelList.OrderBy(entity.order + orderTypeStr);
            return await levelList.WithCache().ToPageAsync(entity.page, entity.limit);
        }

        /// <summary>
        /// 获取等级列表
        /// </summary>
        /// <param name="store_id"></param>
        /// <returns></returns>
        public async Task<List<c_archives_level>> GetList(int store_id)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            return await Db.Queryable<c_archives_level>()
                .Where(w => w.org_id == userInfo.org_id).WithCache()
                .ToListAsync();
        }

        /// <summary>
        /// 添加会员等级
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> Add(c_archives_level entity)
        {
            //获取登录人信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            if (entity.special == 3)
            {
                //判断最小金额是否大于最大金额
                if (entity.min_money >= entity.max_money || entity.min_money < 0 || entity.max_money <= 0)
                {
                    throw new MessageException("金额范围填写不正确");
                }
            }
            if (entity.money_integral <= 0)
            {
                throw new MessageException("现金积分不能<0!");
            }
            if (entity.integral_money <= 0)
            {
                throw new MessageException("积分兑换不能<0!");
            }
            if (entity.discount_rate < 0 || entity.discount_rate > 100)
            {
                throw new MessageException("折扣率应在0-100之间!");
            }
            if (entity.royalty_rate < 0 || entity.royalty_rate > 100)
            {
                throw new MessageException("提成比例应在0-100之间!");
            }
            if (entity.balance_limit_lower <= 0)
            {
                throw new MessageException("余额下限不能<0!");
            }

            //判断等级名是否存在
            var levelList = Db.Queryable<c_archives_level>();

            var isExisteName = await levelList.Clone().WithCache().AnyAsync(a => a.name == entity.name && a.org_id == userInfo.org_id);
            if (isExisteName)
            {
                throw new MessageException("此等级名称已存在");
            }

            if (entity.special == 3)
            {
                var isZero = await levelList.Clone().WithCache().AnyAsync(a => a.min_money == 0 && a.org_id == userInfo.org_id);
                if (isZero == false && entity.min_money != 0)
                {
                    throw new MessageException("请先添加最小金额为0的等级!");
                }
                if (isZero && entity.min_money == 0)
                {
                    throw new MessageException("已存在最小金额为0的等级!");
                }
                //判断金额区间段，是否重复
                var currentRanges = await levelList.Clone().Where(a => a.org_id == userInfo.org_id).Select(s => new { s.min_money, s.max_money }).WithCache().ToListAsync();
                var isTrue = currentRanges.Any(z => (z.min_money <= entity.min_money && entity.min_money < z.max_money) || (z.min_money < entity.max_money && entity.max_money <= z.max_money) || (entity.min_money < z.min_money && entity.max_money >= z.max_money) && z.min_money != entity.max_money);
                if (isTrue)
                {
                    throw new MessageException("此区间已存在!");
                }
            }

            var levelEntity = new c_archives_level();
            levelEntity = entity;
            levelEntity.org_id = userInfo.org_id;
            levelEntity.creater_id = userInfo.id;
            levelEntity.creater = userInfo.name;
            levelEntity.create_date = DateTime.Now;

            var isSuccess = await Db.Insertable(levelEntity).RemoveDataCache().ExecuteCommandAsync();
            redisCache.RemoveAll<c_archives_level>();
            if (isSuccess <= 0)
            {
                throw new MessageException("未添加成功!");
            }
            return isSuccess;
        }


        /// <summary>
        /// 编辑会员等级
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> Modify(c_archives_level entity)
        {
            //获取登录人信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            if (entity.special == 3)
            {
                //判断最小金额是否大于最大金额
                if (entity.min_money >= entity.max_money || entity.min_money < 0 || entity.max_money <= 0)
                {
                    throw new MessageException("金额范围填写不正确!");
                }
            }
            if (entity.money_integral <= 0)
            {
                throw new MessageException("现金积分不能<0!");
            }
            if (entity.integral_money <= 0)
            {
                throw new MessageException("积分兑换不能<0!");
            }
            if (entity.discount_rate < 0 || entity.discount_rate > 100)
            {
                throw new MessageException("折扣率应在0-100之间!");
            }
            if (entity.royalty_rate < 0 || entity.royalty_rate > 100)
            {
                throw new MessageException("提成比例应在0-100之间!");
            }
            if (entity.balance_limit_lower <= 0)
            {
                throw new MessageException("余额下限不能<0!");
            }
            //查询等级数据
            var levelList = Db.Queryable<c_archives_level>();
            //判断等级名是否存在
            var isExisteName = await levelList.Clone().WithCache().AnyAsync(a => a.name == entity.name && a.org_id == userInfo.org_id && a.id != entity.id);
            if (isExisteName)
            {
                throw new MessageException("此等级名称已存在");
            }
            if (entity.special == 3)
            {
                //最小金额为0的不能修改
                var Zero = await levelList.Clone().Where(a => a.org_id == userInfo.org_id && a.id == entity.id).WithCache().FirstAsync();
                if (Zero.min_money == 0)
                {
                    if (entity.min_money != 0)
                    {
                        throw new MessageException("此最小金额不能修改!");
                    }
                }
                //查询最小金额的id
                var mixId = await levelList.Clone().WithCache().Where(a => a.min_money == 0 && a.org_id == userInfo.org_id).Select(w => w.id).FirstAsync();
                //其他最小金额不能修改为0
                if (mixId != entity.id && entity.min_money == 0)
                {
                    throw new MessageException("已存在积分为0的等级!");
                }
                //判断金额区间段，是否重复
                var currentRanges = await levelList.Clone().Where(a => a.org_id == userInfo.org_id && a.id != entity.id).Select(s => new { s.min_money, s.max_money }).WithCache().ToListAsync();
                var isTrue = currentRanges.Any(z => (z.min_money <= entity.min_money && entity.min_money < z.max_money) || (z.min_money < entity.max_money && entity.max_money <= z.max_money) || (entity.min_money < z.min_money && entity.max_money >= z.max_money) && z.min_money != entity.max_money);
                if (isTrue)
                {
                    throw new MessageException("此区间已存在!");
                }
            }

            var isSuccess = await Db.Updateable(entity).IgnoreColumns(it => new { it.creater, it.creater_id, it.create_date, it.org_id }).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
            if (isSuccess <= 0)
            {
                throw new MessageException("未编辑成功!");
            }
            return isSuccess;
        }

        /// <summary>
        /// 删除会员等级
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> Delete(int id)
        {
            var is_use = await Db.Queryable<c_archives>().WithCache().AnyAsync(w => w.grade_id == id);
            if (is_use)
            {
                throw new MessageException("当前等级被使用中，不能删除");
            }
            return await Db.Deleteable<c_archives_level>().Where(w => w.id == id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
        }
    }
}
