using Microsoft.AspNetCore.Http;
using Models.DB;
using Models.View.OA;
using Newtonsoft.Json;
using Service.Extensions;
using Service.Repository.Interfaces.OA;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;
using Tools.Filter;

namespace Service.Repository.Implements.OA
{
    /// <summary>
    /// 文件管理
    /// </summary>
    public class FilesService : DbContext, IFilesService
    {
        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<bool> CreateFolder(Folder entity, dynamic type)
        {
            //查询用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            string currentDirectory = Path.GetDirectoryName(type.Assembly.Location);

            string fileurl = Path.Combine(currentDirectory + @"\wwwroot");
            if (!string.IsNullOrEmpty(entity.url))
            {
                fileurl += "/" + entity.url + "/";
            }
            else
            {
                fileurl += "/FilesManage/" + userInfo.org_id + "/" + entity.store_id + "/";
                entity.url = "FilesManage/" + userInfo.org_id + "/" + entity.store_id;
            }
            fileurl += entity.name + "/";
            if (!Directory.Exists(fileurl))
            {
                Directory.CreateDirectory(fileurl);
                redisCache.RemoveAll<oa_files>();
                //添加oa_files
                return await Db.Insertable(new oa_files { creater = userInfo.name, creater_id = userInfo.id, create_time = DateTime.Now, name = entity.name, org_id = userInfo.org_id, size = 0, store_id = entity.store_id, store_name = entity.store_name, type = "folder", url = entity.url }).ExecuteCommandAsync() > 0;
            }
            else
            {
                throw new MessageException($"文件夹（{entity.name}）已存在当前路径下");
            }
        }

        /// <summary>
        /// 删除选中
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<int> DeleteAsync(oa_files entity, dynamic type)
        {
            //查询用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            entity.org_id = userInfo.org_id;
            string currentDirectory = Path.GetDirectoryName(type.Assembly.Location);
            if (entity.type != "folder")
            {
                var path = Path.Combine(currentDirectory, "wwwroot", entity.url, entity.name + entity.type);
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                return await Db.Deleteable<oa_files>().Where(w => w.name == entity.name && w.org_id == entity.org_id && w.store_id == entity.store_id && w.type == entity.type && w.url == entity.url).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
            }
            else
            {
                var path = Path.Combine(currentDirectory, "wwwroot", entity.url, entity.name);
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
                var url = entity.url + "/" + entity.name;
                return await Db.Deleteable<oa_files>().Where(w => w.org_id == entity.org_id && w.store_id == entity.store_id && (w.url.StartsWith(url) || (w.name == entity.name && w.url == entity.url && entity.type == "folder"))).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
            }
        }

        /// <summary>
        /// 文件分页
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Page<FilesPageModel>> GetPageAsync(FilesPageSearch entity)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            var orderTypeStr = " Asc";
            if (entity.orderType == 1)
            {
                orderTypeStr = " Desc";
            }
            if (string.IsNullOrEmpty(entity.url))
            {
                entity.url = "FilesManage/" + userInfo.org_id + "/" + entity.store_id;
            }

            //定义有权限的文件夹及文件集合
            var fileUrls = new List<string>();
            if (entity.is_me)
            {
                //获取文件集合
                var this_fileUrls = await Db.Queryable<oa_files_authority, oa_files>((fa, f) => new object[] { JoinType.Left, fa.name == f.name && fa.type == f.type && fa.url == f.url && fa.org_id == f.org_id && fa.store_id == f.store_id }).Where((fa, f) => f.org_id == userInfo.org_id && f.store_id == entity.store_id && fa.link_id_type == 2 && fa.link_id == userInfo.id && f.url.StartsWith(entity.url)).Select((fa, f) => SqlFunc.MergeString(f.url, "/", f.name, f.type)).ToListAsync();
                var num = entity.url.Split("/").Length;
                //添加文件及文件夹到集合
                this_fileUrls.ForEach(item =>
                {
                    fileUrls.Add(item);
                    var strs = item.Split("/");
                    if (strs.Length > num)
                    {
                        fileUrls.Add(entity.url + "/" + strs[num]);
                    }
                });
            }
            return await Db.Queryable<oa_files>()
                            .Where(w => w.org_id == userInfo.org_id && w.store_id == entity.store_id)
                            .WhereIF(!string.IsNullOrEmpty(entity.name), w => w.name.Contains(entity.name))
                            .WhereIF(string.IsNullOrEmpty(entity.name), w => w.url == entity.url)
                            .WhereIF(entity.is_me, w => fileUrls.Contains(SqlFunc.MergeString(w.url, "/", w.name)) || fileUrls.Contains(SqlFunc.MergeString(w.url, "/", w.name, w.type)))
                            .Select(s => new FilesPageModel { creater = s.creater, url = s.url, creater_id = s.creater_id, create_time = s.create_time, org_id = s.org_id, name = s.name, size = s.size, store_id = s.store_id, store_name = s.store_name, type = s.type, files_num = SqlFunc.Subqueryable<oa_files>().Where(w => w.url == SqlFunc.MergeString(s.url, "/", s.name) && w.type != "folder").Count(), folder_num = SqlFunc.Subqueryable<oa_files>().Where(w => w.url == SqlFunc.MergeString(s.url, "/", s.name) && w.type == "folder").Count() })
                            .OrderBy(entity.order + orderTypeStr)
                            .WithCache()
                            .ToPageAsync(entity.page, entity.limit);
        }

        #region 获取树结构（部门、职位、人员）及选中项 文件夹权限设置获取
        /// <summary>
        /// 获取树结构（部门、职位、人员）
        /// </summary>
        /// <param name="store_id">门店ID</param>
        /// <returns></returns>
        public async Task<object> GetTreeAsync(int store_id)
        {
            //获取用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;

            var type = 0;
            var dept_list = await Db.Queryable<p_dept>().Where(w => w.org_id == userInfo.org_id && w.store_id == store_id && w.state == 1).WithCache().ToListAsync();
            var role_list = await Db.Queryable<p_role>().Where(w => w.org_id == userInfo.org_id && w.store_id == store_id && w.disabled_code == 1).WithCache().ToListAsync();
            var employee_list = await Db.Queryable<p_employee_role, p_employee>((er, e) => new object[] { JoinType.Left, er.employee_id == e.id }).Where((er, e) => er.org_id == userInfo.org_id && er.store_id == store_id && e.expire_time > DateTime.Now).GroupBy((er, e) => new { e.id, e.name, er.role_id }).Select((er, e) => new employee { id = e.id, name = e.name, role_id = er.role_id }).WithCache().ToListAsync();
            var jsonStr = "[";
            jsonStr += GetJson(0, type, dept_list, role_list, employee_list);
            jsonStr += "]";
            return JsonConvert.DeserializeObject(jsonStr);
        }

        private class employee
        {
            public int id { get; set; }

            public int role_id { get; set; }

            public string name { get; set; }
        }

        private string GetJson(int id, int type, List<p_dept> depts, List<p_role> roles, List<employee> employees)
        {
            var str = "";
            List<tree> newLst = new List<tree>();
            if (type == 0)
            {
                newLst = depts.Select(s => new tree { id = "0_" + s.id, label = s.name, link_id = s.id, type = 1 }).ToList();
            }
            else if (type == 1)
            {
                newLst = roles.Where(w => w.dept_id == id).Select(s => new tree { id = "1_" + s.id, label = s.name, link_id = s.id, type = 2 }).ToList();
            }
            else
            {
                newLst = employees.Where(w => w.role_id == id).Select(s => new tree { id = "2_" + s.id, label = s.name, link_id = s.id, type = 3 }).ToList();
            }
            if (newLst.Count > 0)
            {
                foreach (var item in newLst)
                {
                    str += "{";

                    str += "label: '" + item.label + "',";
                    str += "id: '" + item.id + "',";
                    str += "link_id: " + item.link_id + ",";
                    str += "type: " + (item.type - 1);
                    if (item.type != 3)
                    {
                        str += ",children: [";
                        str += GetJson(item.link_id, item.type, depts, roles, employees);
                        str += "]";
                    }

                    str += "},";
                }
                str = str.TrimEnd(',');
            }
            return str;
        }

        /// <summary>
        /// 设置文件权限
        /// </summary>
        /// <param name="filesAuthority"></param>
        /// <returns></returns>
        public async Task<bool> SetAuthorityAsync(FilesAuthority filesAuthority)
        {
            //查询用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            //事务创建
            var result = Db.Ado.UseTran(() =>
            {
                var ret = -1;
                //删除角色已有权限
                ret = Db.Deleteable<oa_files_authority>().Where(w => w.name == filesAuthority.name && w.url == filesAuthority.url && w.type == filesAuthority.type && w.org_id == userInfo.org_id && w.store_id == filesAuthority.store_id).RemoveDataCache().EnableDiffLogEvent().ExecuteCommand();
                if (ret != -1)
                {
                    if (filesAuthority.authorities.Count > 0)
                    {
                        //获取需要添加的权限
                        var addlst = filesAuthority.authorities.Select(s => new oa_files_authority { link_id = s.link_id, link_id_type = s.link_type, name = filesAuthority.name, org_id = userInfo.org_id, store_id = filesAuthority.store_id, type = filesAuthority.type, url = filesAuthority.url, link_name = s.link_name }).ToList();
                        //添加角色权限
                        Db.Insertable(addlst).ExecuteCommand();
                        redisCache.RemoveAll<oa_files_authority>();
                    }
                }
            });
            return result.IsSuccess;
        }

        /// <summary>
        /// 获取选中文件权限
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<object> GetAuthorityAsync(oa_files entity)
        {
            //查询用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            //查询角色已有权限
            var lst = await Db.Queryable<oa_files_authority>().Where(w => w.name == entity.name && w.url == entity.url && w.type == entity.type && w.org_id == userInfo.org_id && w.store_id == entity.store_id && w.link_id_type == 2).WithCache().ToListAsync();
            var jsonStr = "[";
            if (lst.Count > 0)
            {
                foreach (var item in lst)
                {
                    jsonStr += "{";
                    jsonStr += "id: '" + item.link_id_type + "_" + item.link_id + "',";
                    jsonStr += "label: '" + item.link_name + "'";
                    jsonStr += "},";
                }
                jsonStr = jsonStr.TrimEnd(',');
            }
            jsonStr = jsonStr.TrimEnd(',');
            jsonStr += "]";
            return JsonConvert.DeserializeObject(jsonStr);
        }

        private class tree
        {
            public string label { get; set; }

            public string id { get; set; }

            public int link_id { get; set; }

            public int type { get; set; }
        }
        #endregion

        /// <summary>
        /// 修改文件名称/文件夹名称
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ModifyAsync(UpdateFilesOrFolderName entity, dynamic type)
        {
            //查询用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            entity.org_id = userInfo.org_id;
            string currentDirectory = Path.GetDirectoryName(type.Assembly.Location);
            var result = Db.Ado.UseTran(() =>
            {
                if (entity.type != "folder")
                {
                    var path = Path.Combine(currentDirectory, "wwwroot", entity.url, entity.old_name + entity.type);
                    if (File.Exists(path))
                    {
                        var new_path = Path.Combine(currentDirectory, "wwwroot", entity.url, entity.name + entity.type);
                        File.Move(path, new_path);
                    }
                    //获取文件
                    var file = Db.Queryable<oa_files>().Where(w => w.name == entity.old_name && w.org_id == entity.org_id && w.store_id == entity.store_id && w.type == entity.type && w.url == entity.url).WithCache().First();
                    //删除文件
                    Db.Deleteable<oa_files>().Where(w => w.name == entity.old_name && w.org_id == entity.org_id && w.store_id == entity.store_id && w.type == entity.type && w.url == entity.url).ExecuteCommand();
                    file.name = entity.name;
                    redisCache.RemoveAll<oa_files>();
                    //写入数据库
                    Db.Insertable(file).ExecuteCommand();
                }
                else
                {
                    var path = Path.Combine(currentDirectory, "wwwroot", entity.url, entity.old_name);
                    if (Directory.Exists(path))
                    {
                        var new_path = Path.Combine(currentDirectory, "wwwroot", entity.url, entity.name);
                        Directory.Move(path, new_path);
                    }
                    var url = entity.url + "/" + entity.old_name;
                    //获取修改文件夹下的所有文件及文件夹信息
                    var list = Db.Queryable<oa_files>().Where(w => w.url.StartsWith(url)).WithCache().ToList();
                    //获取文件夹
                    var folder = Db.Queryable<oa_files>().Where(w => w.name == entity.old_name && w.type == "folder" && w.org_id == entity.org_id && w.store_id == entity.store_id && w.url == entity.url).WithCache().First();
                    //删除文件夹
                    Db.Deleteable<oa_files>().Where(w => w.name == entity.old_name && w.type == "folder" && w.org_id == entity.org_id && w.store_id == entity.store_id && w.url == entity.url).ExecuteCommand();
                    folder.name = entity.name;
                    //删除修改文件夹下的所有文件及文件夹信息
                    Db.Deleteable<oa_files>().Where(w => w.url.StartsWith(url)).ExecuteCommand();
                    //修改文件夹下的所有文件及文件夹Url
                    list = list.Select(s => new oa_files { creater = s.creater, creater_id = s.creater_id, create_time = s.create_time, name = s.name, org_id = s.org_id, size = s.size, store_id = s.store_id, store_name = s.store_name, type = s.type, url = s.url.Replace(url, entity.url + "/" + entity.name) }).ToList();
                    list.Add(folder);
                    redisCache.RemoveAll<oa_files>();
                    //写入数据库
                    Db.Insertable(list).ExecuteCommand();
                }
            });
            if (!result.IsSuccess)
            {
                throw new MessageException(result.ErrorMessage);
            }
            return result.IsSuccess;
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="entity">文件夹路径</param>
        /// <param name="files"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<List<oa_files>> UploadFilesAsync(Files entity, IEnumerable<IFormFile> files, dynamic type)
        {
            //查询用户信息
            var userInfo = new Tools.IdentityModels.GetUser().userInfo;
            //获取上传文件信息
            List<FilesInfo> result = await UpLoadFiles.FilesSave(entity.url, userInfo.org_id, entity.store_id, files, type);
            //oa_files列表
            var oa_files = result.Select(s => new oa_files { url = s.url, type = s.type, creater = userInfo.name, creater_id = userInfo.id, create_time = s.create_time, name = s.name, org_id = userInfo.org_id, size = s.size, store_id = entity.store_id, store_name = entity.store_name }).ToList();
            //添加oa_files
            Db.Insertable(oa_files).ExecuteCommand();
            redisCache.RemoveAll<oa_files>();
            return oa_files;
        }
    }
}
