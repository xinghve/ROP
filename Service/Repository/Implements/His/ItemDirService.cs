using Models.DB;
using Models.View.His;
using Newtonsoft.Json;
using Service.Repository.Interfaces.His;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;
using Tools.Filter;
using static Tools.IdentityModels.GetUser;

namespace Service.Repository.Implements.His
{
    /// <summary>
    /// 项目目录业务
    /// </summary>
    public class ItemDirService : DbContext, IItemDirService
    {
        //获取用户
        private UserInfo userInfo = new Tools.IdentityModels.GetUser().userInfo;

        /// <summary>
        /// 添加项目目录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> AddAsync(h_itemdir entity)
        {
            entity.orgid = userInfo.org_id;
            var code = "";
            //判断是否存在
            var isExisteName = await Db.Queryable<h_itemdir>().WithCache().AnyAsync(a => a.parentid == entity.parentid && a.name == entity.name&&a.typeid==entity.typeid);
            if (isExisteName)
            {
                throw new MessageException("当前层级已存在此类型目录！");
            }
            //生成编码,父级id不为0的情况
            if (entity.parentid > 0)
            {
                var itemList = await Db.Queryable<h_itemdir>().WithCache().ToListAsync();
                //查询相同父级编码的最大编码
                var maxCode = itemList.Where(w => w.orgid == userInfo.org_id && w.parentid == entity.parentid).Max(m => m.code);
                //查询父级最大编码
                var p_code = itemList.Where(w => w.orgid == userInfo.org_id && w.dirid == entity.parentid).Select(m => m.code).First();
                if (maxCode != null)
                {
                    code = (Int64.Parse(maxCode) + 1).ToString().PadLeft(p_code.Length + 2, '0');
                }
                else
                {
                    code = p_code + "01";
                    //查询父级类别id
                    var typeId =  itemList.Where(w => w.orgid == userInfo.org_id && w.dirid == entity.parentid).Select(s=>s.typeid).First();
                    entity.typeid = typeId;
                }
            }
            else
            {
                var maxCode =await Db.Queryable<h_itemdir>().Where(w => w.orgid == userInfo.org_id && w.parentid == 0).WithCache().MaxAsync(m => m.code);
                if (maxCode == null)
                {
                    code = "01";
                }
                else
                {
                    code = (Int64.Parse(maxCode) + 1).ToString().PadLeft(2, '0');
                }
            }
            entity.code = code;
            var isSueccess = await Db.Insertable<h_itemdir>(entity).ExecuteCommandAsync();
            redisCache.RemoveAll<h_itemdir>();

            return isSueccess;

        }

        /// <summary>
        /// 编辑项目目录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> ModifyAsync(h_itemdir entity)
        {
            //判断是否存在
            var isExisteName = await Db.Queryable<h_itemdir>().WithCache().AnyAsync(a => a.parentid == entity.parentid && a.name == entity.name&&a.dirid!=entity.dirid && a.typeid == entity.typeid);
            if (isExisteName)
            {
                throw new MessageException("当前层级已存在此类型目录！");
            }
            return await Db.Updateable(entity)
                .SetColumns(it => new h_itemdir { name = entity.name})
                .Where(w => w.dirid == entity.dirid&&w.orgid==userInfo.org_id)
                .EnableDiffLogEvent()
                .RemoveDataCache()
                .ExecuteCommandAsync();
        }

        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
       public async Task<bool> DeleteAsync(int id)
       {
            if (id<=0)
            {
                throw new MessageException("请选择目录！");
            }
            var result = await Db.Ado.UseTranAsync(() =>
            {
                //查询该目录下的项目规格
                var specIdList = Db.Queryable<h_itemdir, h_item,h_itemspec>((dir, item,spec) => new object[] {JoinType.Left, item.dir_id == dir.dirid,JoinType.Left,spec.itemid==item.item_id })
                                   .Where((dir, item, spec) => dir.dirid == id && dir.orgid==userInfo.org_id)
                                   .Select((dir, item, spec) =>spec.specid)
                                   .WithCache()
                                   .ToList();
            if (specIdList.Count>0)
            {
                //查询结算中是否有此目录下的规格
                var IsSpec = Db.Queryable<f_balancedetail>()
                                 .WithCache()
                                 .Any(s => specIdList.Contains(s.specid));

                if (IsSpec)
                {
                    throw new MessageException("此项目使用中，不能删除！");
                }
            }         

           
                if (specIdList.Count > 0)
                {
                    //删除规格
                    Db.Deleteable<h_itemspec>().Where(w => specIdList.Contains(w.specid)).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                }
                //删除项目
                Db.Deleteable<h_item>().Where(w => w.dir_id == id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                //删除目录
                Db.Deleteable<h_itemdir>().Where(w => w.dirid == id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();

            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
            
        }

        /// <summary>
        /// 获取项目目录树
        /// </summary>
        /// <returns></returns>
        public async Task<object> GetItemTree()
        {
            //获取项目List
            var list =await Db.Queryable<h_itemdir>().Where(s => s.orgid == userInfo.org_id).WithCache().OrderBy(s=>s.dirid).ToListAsync();
            var jsonStr = "[";
            jsonStr += GetJson(list,0);
            jsonStr = jsonStr.TrimEnd(',');
            jsonStr += "]";
            return JsonConvert.DeserializeObject(jsonStr);
        }

        private string GetJson(List<h_itemdir> lst,int parentid)
        {
            var str = "";
            var newLst = lst.Where(s =>  s.parentid == parentid).ToList();
            if (newLst.Count > 0)
            {
                foreach (var item in newLst)
                {
                    str += "{";

                    str += "label: '" + item.name + "',";
                    str += "typeId: '" + item.typeid + "',";
                    str += "parentid: '" + item.parentid + "',";
                    str += "name: '" + item.name + "',";
                    str += "code: '" + item.code + "',";
                    str += "dirid: '" + item.dirid + "',";
                    str += "type_name: '" + item.type_name + "',";
                    str += "type_parent_id: '" + item.type_parent_id + "',";
                    str += "children: [";
                    str += GetJson(lst, item.dirid);
                    str += "]";

                    str += "},";
                }
                str = str.TrimEnd(',');
            }
            return str;
        }

        /// <summary>
        ///  获取项目类别
        /// </summary>
        /// <param name="typeId">没有传-1</param>
        /// <param name="parentId">目录ID</param>
        /// <returns></returns>
        public async Task<List<b_basecode>> GetItemType(int typeId,int parentId)
        {
            //int[] itemArray =new int[] { 37, 50, 105, 106 };
            int[] itemArray =new int[] {  50, 105,111,112,113 };
            //如果类型id不存在
            if (typeId==-1)
            {
                return await Db.Queryable<b_basecode>().Where(w => itemArray.Contains(w.baseid) && w.stateid == 1).GroupBy(w => new { w.baseid, w.name }).Select(s => new b_basecode { baseid = s.baseid, name = SqlFunc.Substring(s.name,5, 5) }).OrderBy(s => s.baseid,OrderByType.Desc).WithCache().ToListAsync();
            }
            else
            {
                if (typeId==50||typeId==105|| typeId ==111|| typeId ==112||typeId==113)
                {
                    return await Db.Queryable<b_basecode>().Where(w => w.baseid == typeId && w.stateid == 1).GroupBy(w => new { w.valueid, w.value }).Select(s => new b_basecode { baseid = s.valueid, name = SqlFunc.Substring(s.value, 5, 5) }).OrderBy(s => s.valueid).WithCache().ToListAsync();
                }
                else
                {
                    //查询父级id的类型id跟名字
                    return await Db.Queryable<h_itemdir>().Where(w =>w.dirid==parentId && w.orgid == userInfo.org_id).Select(s => new  b_basecode{ baseid = s.typeid, name = s.type_name }).WithCache().ToListAsync();

                }
            }
          

        }

        /// <summary>
        /// 导入项目目录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> ImportDir(List<h_itemdir> entity)
        {
            if (entity.Count==0)
            {
                throw new MessageException("未传输数据！");
            }

            var code = "";
            entity.ForEach(c => {
                if (string.IsNullOrEmpty(c.type_name))
                {
                    throw new MessageException("目录类型未填写！");
                }

                int[] itemArray = new int[] { 50, 105, 111, 112, 113 };
                //根据类型名查询类型id
                var typeId = Db.Queryable<b_basecode>().Where(w => itemArray.Contains(w.baseid) && w.stateid == 1 && w.name.Contains(c.type_name)).Select(s => new b_basecode { baseid = s.baseid }).WithCache().First();

                if (typeId==null||typeId?.baseid==0)
                {
                    throw new MessageException("未获取到目录类型id！");
                }

                var isExisteName = Db.Queryable<h_itemdir>().WithCache().Any(a => a.parentid == 0 && a.name == c.name && a.typeid == c.typeid);
                if (isExisteName)
                {
                    throw new MessageException($"当前层级已存在此{c.name}类型目录！");
                }

                var maxCode =  Db.Queryable<h_itemdir>().Where(w => w.orgid == userInfo.org_id && w.parentid == 0).WithCache().Max(m => m.code);
                if (maxCode == null)
                {
                    code = "01";
                }
                else
                {
                    code = (Int64.Parse(maxCode) + 1).ToString().PadLeft(2, '0');
                }
                c.code = code;
                c.orgid = userInfo.org_id;
            });
            var isSueccess = await Db.Insertable<h_itemdir>(entity).ExecuteCommandAsync();
            redisCache.RemoveAll<h_itemdir>();

            return isSueccess;

        }

        /// <summary>
        /// 导入所有项目
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> ImportAllItem(List<ItemAllModel> entity)
        {
            if (entity.Count == 0)
            {
                throw new MessageException("未传输数据！");
            }


            var result = await Db.Ado.UseTranAsync(() =>
            {
                //目录
                var groupby_dir = entity.GroupBy(d => new { d.dir_name, d.dir_type_name }).Select(d => new h_itemdir { name = d.Key.dir_name, type_name = d.Key.dir_type_name }).ToList();

                if (groupby_dir.Count==0)
                {
                    throw new MessageException("没有目录！");
                }
                var code = "";
                groupby_dir.ForEach(d => {
                    if (string.IsNullOrEmpty(d.type_name))
                    {
                        throw new MessageException("有目录类型未填写！");
                    }

                    var dir = new h_itemdir();

                    int[] itemArray = new int[] { 50, 105, 111, 112, 113 };
                    //根据类型名查询类型id
                    var typeId = Db.Queryable<b_basecode>().Where(w => itemArray.Contains(w.baseid) && w.stateid == 1 && w.name.Contains(d.type_name)).Select(s => new b_basecode { baseid = s.baseid }).WithCache().First();

                    if (typeId == null || typeId?.baseid == 0)
                    {
                        throw new MessageException("未获取到目录类型id！");
                    }

                    var maxCode = Db.Queryable<h_itemdir>().Where(w => w.orgid == userInfo.org_id && w.parentid == 0).WithCache().Max(m => m.code);
                    if (maxCode == null)
                    {
                        code = "01";
                    }
                    else
                    {
                        code = (Int64.Parse(maxCode) + 1).ToString().PadLeft(2, '0');
                    }

                    d.code = code;
                    d.orgid = userInfo.org_id;
                    d.typeid = Convert.ToInt16(typeId.baseid);


                    //判断是否存在此目录名
                    var isExisteName = Db.Queryable<h_itemdir>().Where(a => a.parentid == 0 && a.name == d.name && a.typeid == d.typeid).WithCache().First();

                    var isDir = -1;
                    if (isExisteName!=null)
                    {
                        isDir = isExisteName.dirid;
                    }
                    else
                    {
                        isDir = Db.Insertable<h_itemdir>(d).ExecuteReturnIdentity();
                        redisCache.RemoveAll<h_itemdir>();

                    }

                    if (isDir<=0)
                    {
                        throw new MessageException($"{d.name} 目录未导入成功！");
                    }

                    var is2 = Convert.ToInt16(2);
                    var is3 = Convert.ToInt16(3);
                    //项目                   

                    var groupby_item= entity.Where(i => i.dir_name == d.name).Select(i => new h_item { short_code = ToSpell.GetFirstPinyin(i.trade_name), code =d.code,creater=userInfo.name, creater_id=userInfo.id, create_date=DateTime.Now, dir_id=isDir, org_id=userInfo.org_id, type_id=d.typeid, trade_name=i.trade_name,  common_name=i.common_name, fee_name=i.fee_name,price_code= i.price_code, publisher= i.publisher=="是"?is2:is3, external= i.external == "是" ? is2 : is3, discount= i.discount == "是" ? is2 : is3,equipment=i.equipment == "是" ? is2 : is3, sexid=GetSex(i.sex), serviceid=GetService(i.service_name), fee_id =GetFeed(i.fee_name)}).ToList();


                    if (groupby_item.Count>0)
                    {

                        groupby_item.ForEach(item => {

                            //查询是否存在相同项目
                            var name =  Db.Queryable<h_item>().Where(w => w.org_id == userInfo.org_id && w.dir_id == isDir && w.trade_name == item.trade_name && w.type_id == d.typeid).WithCache().First();

                            var isDi = -1;
                            if (name!=null)
                            {
                                isDi = name.item_id;
                                Db.Updateable(item)
                                .IgnoreColumns(it=>new {it.creater,it.creater_id,it.create_date,it.state_id,it.dir_id,it.item_id,it.org_id,it.type_id,it.trade_name })
                                .Where(it=>it.item_id==isDi)
                                .RemoveDataCache()
                                .EnableDiffLogEvent()
                                .ExecuteCommand();

                            }
                            else
                            {
                                isDi=Db.Insertable<h_item>(item).ExecuteReturnIdentity();
                                redisCache.RemoveAll<h_item>();

                            }
                            if (isDi<=0)
                            {
                                throw new MessageException($"{item.trade_name} 项目未导入成功！");
                            }


                            //项目规格

                            var groupby_item_spec = entity.Where(i=>i.trade_name==item.trade_name).Select(i => new h_itemspec { packmodulus=1, packunit="次", itemid = isDi, specname = i.specname, feegrade = i.feegrade, sale_price = i.sale_price, buy_price = i.buy_price, salseunit = i.salseunit, mindosage = i.mindosage, dosageunit = i.dosageunit, minunit = i.minunit, hint = i.hint, standardcode = i.standardcode, incomeid = GetFeed(i.income), billid = GetBill(i.bill) }).ToList();


                            if (groupby_item_spec.Count>0)
                            {
                                groupby_item_spec.ForEach(itsp => {

                                    //查询是否存在相同规格
                                    var spec = Db.Queryable<h_itemspec>().Where(w => w.itemid == isDi && w.specname == itsp.specname).WithCache().First();
                                    if (spec==null)
                                    {
                                        Db.Insertable<h_itemspec>(itsp).ExecuteCommand();
                                        redisCache.RemoveAll<h_itemspec>();

                                    }
                                    else
                                    {
                                        Db.Updateable(itsp)
                                          .IgnoreColumns(it => new { it.specid,it.itemid,it.specname })
                                          .Where(it => it.specid == spec.specid)
                                          .RemoveDataCache()
                                          .EnableDiffLogEvent()
                                          .ExecuteCommand();

                                    }

                                });
                                
                            }

                        });
                       
                        

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
        /// 获取性别id
        /// </summary>
        /// <returns></returns>
        public short GetSex(string sex)
        {
            var sexid = Db.Queryable<b_basecode>()
                        .Where(b => b.baseid == 73 && b.stateid == 1 && b.value.Contains(sex))
                        .WithCache()
                        .First();

            if (sexid==null)
            {
                return -1;
            }
            return sexid.valueid;
        }

        /// <summary>
        /// 获取服务对象id
        /// </summary>
        /// <param name="sex"></param>
        /// <returns></returns>
        public short GetService(string service)
        {
            var serviceid = Db.Queryable<b_basecode>()
                        .Where(b => b.baseid == 67 && b.stateid == 1 && b.value.Contains(service))
                        .WithCache()
                        .First();

            if (serviceid == null)
            {
                return -1;
            }
            return serviceid.valueid;
        }

        /// <summary>
        /// 获取费别id
        /// </summary>
        /// <param name="sex"></param>
        /// <returns></returns>
        public short GetFeed(string fee)
        {
            var feeid = Db.Queryable<b_basecode>()
                        .Where(b => b.baseid == 66 && b.stateid == 1 && b.value.Contains(fee))
                        .WithCache()
                        .First();

            if (feeid == null)
            {
                return -1;
            }
            return feeid.valueid;
        }

        /// <summary>
        /// 获取单据
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetBill(string name)
        {
            var list = Db.Queryable<h_bill>()
            .Where(w => w.org_id == userInfo.org_id && w.bill_name.Contains(name))
            .WithCache()
            .First();

            if (list == null)
            {
                return 0;
            }
            return list.bill_id;

        }
    }
  
}
