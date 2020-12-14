using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DB;
using Models.View.OA;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Filter;

namespace Tools
{
    /// <summary>
    /// 上传
    /// </summary>
    public class UpLoadFiles
    {
        /// <summary>
        /// 多图
        /// </summary>
        /// <param name="context"></param>
        /// <param name="names"></param>
        /// <param name="type"></param>
        /// <param name="FilesName"></param>
        /// <returns></returns>
        public uploadResult UploadCreat(HttpContext context, List<string> names, dynamic type, string FilesName = "Image")
        {
            uploadResult result = new uploadResult();
            try
            {
                var oFile = context.Request.Form.Files;

                //dynamic type = (new Program()).GetType();
                string currentDirectory = Path.GetDirectoryName(type.Assembly.Location);
                string file = Path.Combine(currentDirectory + @"\wwwroot\Files");//获取绝对路径

                result.fileNames = new List<string>();
                //循环文件
                foreach (var item in oFile)
                {
                    //判断文件类型为图片
                    if (item.ContentType == "image/jpeg" || item.ContentType == "image/png")
                    {
                        Stream sm = item.OpenReadStream();
                        //判断文件夹是否存在，不存在则新建文件夹
                        if (!Directory.Exists(file + "/" + FilesName + "/"))
                        {
                            Directory.CreateDirectory(file + "/" + FilesName + "/");
                        }
                        var repostFile = "";
                        if (names.Count > 0)//判断是否存在传入文件名
                        {
                            repostFile = names.Last().Trim().Split("/").Last();//获取第一个文件名并赋值
                            names.Remove(names.Last());//移除第一个文件名
                        }
                        else
                        {
                            repostFile = DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".png";//创建文件名
                        }
                        string filename = file + "/" + FilesName + "/" + repostFile;//文件路径(绝对路径)
                        FileStream fs = new FileStream(filename, FileMode.Create);
                        byte[] buffer = new byte[sm.Length];
                        sm.Read(buffer, 0, buffer.Length);
                        fs.Write(buffer, 0, buffer.Length);
                        fs.Dispose();
                        result.fileNames.Add("Files/" + FilesName + "/" + repostFile);//添加相对路径到结果中的文件名集合
                    }
                }
                if (names.Count > 0)//判断是否还存在传入文件名
                {
                    foreach (var item in names)
                    {
                        var url = file + "/" + FilesName + "/" + item.Trim().Split("/").Last();
                        if (File.Exists(url))
                        {
                            File.Delete(url);//删除无用文件
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                result.error = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 单图 若repostFile（图片名）存在便替换
        /// </summary>
        /// <param name="context"></param>
        /// <param name="type"></param>
        /// <param name="FilesName"></param>
        /// <param name="repostFile"></param>
        /// <returns></returns>
        public uploadResult Upload(HttpContext context, dynamic type, string FilesName = "Image", string repostFile = "")
        {
            uploadResult result = new uploadResult();
            try
            {
                var oFile = context.Request.Form.Files["txt_file"];
                Stream sm = oFile.OpenReadStream();

                //string str = Directory.GetCurrentDirectory();

                //dynamic type = (new Program()).GetType();
                string currentDirectory = Path.GetDirectoryName(type.Assembly.Location);

                string file = Path.Combine(currentDirectory + @"\wwwroot\Files");

                if (!Directory.Exists(file + "/" + FilesName + "/"))
                {
                    Directory.CreateDirectory(file + "/" + FilesName + "/");
                }
                if (string.IsNullOrEmpty(repostFile))
                {
                    repostFile = DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".png";
                }
                string filename = file + "/" + FilesName + "/" + repostFile;
                FileStream fs = new FileStream(filename, FileMode.Create);
                byte[] buffer = new byte[sm.Length];
                sm.Read(buffer, 0, buffer.Length);
                fs.Write(buffer, 0, buffer.Length);
                fs.Dispose();
                result.fileName = "Files/" + FilesName + "/" + repostFile;
            }
            catch (Exception ex)
            {
                result.error = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 上传结果
        /// </summary>
        public class uploadResult
        {
            /// <summary>
            /// 单图
            /// </summary>
            public string fileName { get; set; }
            /// <summary>
            /// 错误信息
            /// </summary>
            public string error { get; set; }
            /// <summary>
            /// 多图
            /// </summary>
            public List<string> fileNames { get; set; }
        }

        /// <summary>
        /// 图片上传
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="Name"></param>
        /// <param name="inputStr"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static async Task<string> FileSave(string Url, string Name, string inputStr, dynamic type)
        {
            MemoryStream stream = null;
            string fileExt = ".jpg";
            if (!string.IsNullOrEmpty(inputStr) && inputStr.Contains("base64"))
            {
                fileExt = "." + inputStr.Split(new char[] { ';' })[0].Replace("data:image/", "");
                inputStr = inputStr.Substring(inputStr.IndexOf("base64,") + 7);
                byte[] arr = Convert.FromBase64String(inputStr);
                stream = new MemoryStream(arr);

            }

            string currentDirectory = Path.GetDirectoryName(type.Assembly.Location);

            string file = Path.Combine(currentDirectory + @"\wwwroot");//获取绝对路径


            if (!Directory.Exists(file + "/Files/" + Url + "/"))
            {
                Directory.CreateDirectory(file + "/Files/" + Url + "/");
            }

            var newFilePath = Name;
            var filePath = Path.Combine(file, newFilePath);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);//删除无用文件
            }

            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssffff") + fileExt; //生成新的文件名

            newFilePath = Path.Combine("Files", Url, newFileName);

            filePath = Path.Combine(file, newFilePath);


            int count = 0;
            const int bytesLength = 4096;
            byte[] bytes = new byte[bytesLength];

            FileStream fs = null;
            try
            {
                //创建文件
                fs = new FileStream(filePath, FileMode.Create);

                if (!fs.CanRead)
                {
                    throw new Exception("数据流不可读！");
                }


                //把 Stream 转换成 byte[] ,写入文件
                while ((count = stream.Read(bytes, 0, bytesLength)) > 0)
                {
                    await fs.WriteAsync(bytes, 0, count);
                }

            }
            catch (Exception ex)
            {
                return "";
            }
            finally
            {
                //关闭流与文件
                stream.Close();
                fs.Close();
            }

            return newFilePath.Replace("\\", "/");
        }

        /// <summary>
        /// 图片上传
        /// </summary>
        /// <param name="Url">文件夹路径</param>
        /// <param name="files"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static async Task<List<string>> FileSave(string Url, IEnumerable<IFormFile> files, dynamic type)
        {
            var list = new List<string>();
            string strpath = string.Empty;
            string path = string.Empty;
            if (files == null || files.Count() <= 0)
            {
                throw new MessageException("请上传图片");
            }
            string currentDirectory = Path.GetDirectoryName(type.Assembly.Location);

            string fileurl = Path.Combine(currentDirectory + @"\wwwroot\Files");

            if (!Directory.Exists(fileurl + "/" + Url + "/"))
            {
                Directory.CreateDirectory(fileurl + "/" + Url + "/");
            }
            //格式限制
            var allowType = new string[] { "image/jpg", "image/png", "image/jpeg" };
            if (files.Any(c => allowType.Contains(c.ContentType)))
            {
                if (files.Sum(c => c.Length) <= 1024 * 1024 * 10)
                {
                    foreach (var file in files)
                    {
                        //文件后缀
                        var fileExtension = Path.GetExtension(file.FileName);
                        strpath = Path.Combine("Files", Url, DateTime.Now.ToString("yyyyMMddHHmmssffff") + fileExtension);// + file.FileName
                        path = Path.Combine(currentDirectory, "wwwroot", strpath);

                        using (var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                        {
                            await file.CopyToAsync(stream);
                        }
                        list.Add(strpath.Replace("\\", "/"));
                    }
                    return list;
                }
                else
                {
                    throw new MessageException("图片大小大于10M，不能上传");
                }
            }
            else
            {
                throw new MessageException("请上传图片文件（格式为jpg，jpeg，png）");
            }
        }

        /// <summary>
        /// 图片上传
        /// </summary>
        /// <param name="context"></param>
        /// <param name="InputID"></param>
        /// <param name="FilesName"></param>
        /// <param name="repostFile"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string FileSave(HttpContext context, string InputID, string FilesName, string repostFile, dynamic type)
        {
            var oFile = context.Request.Form.Files[InputID];
            Stream sm = oFile.OpenReadStream();

            string currentDirectory = Path.GetDirectoryName(type.Assembly.Location);

            string file = Path.Combine(currentDirectory + @"\wwwroot\Files");

            if (!Directory.Exists(file + "/" + FilesName + "/"))
            {
                Directory.CreateDirectory(file + "/" + FilesName + "/");
            }
            if (string.IsNullOrEmpty(repostFile))
            {
                repostFile = DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".png";
            }
            string filename = file + "/" + FilesName + "/" + repostFile;
            FileStream fs = new FileStream(filename, FileMode.Create);
            byte[] buffer = new byte[sm.Length];
            sm.Read(buffer, 0, buffer.Length);
            fs.Write(buffer, 0, buffer.Length);
            fs.Dispose();
            return Path.Combine("Files", FilesName, repostFile).Replace("\\", "/");
        }


        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="Url">文件夹路径</param>
        /// <param name="files"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static async Task<List<FilesInfo>> FilesSave(string Url, IEnumerable<IFormFile> files, dynamic type)
        {
            var list = new List<FilesInfo>();
            string strpath = string.Empty;
            string path = string.Empty;
            string fileName = string.Empty;
            if (files == null || files.Count() <= 0)
            {
                throw new MessageException("请上传文件（格式为7z,rar,zip,doc,docx,xls,xlsx,jpg,jpeg,png,txt,pdf）");
            }
            string currentDirectory = Path.GetDirectoryName(type.Assembly.Location);

            string fileurl = Path.Combine(currentDirectory + @"\wwwroot\Files");

            if (!Directory.Exists(fileurl + "/" + Url + "/"))
            {
                Directory.CreateDirectory(fileurl + "/" + Url + "/");
            }
            //格式限制
            var allowType = new string[] { "application/x-7z-compressed", "application/octet-stream", "application/x-zip-compressed", "application/zip", "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "application/vnd.ms-excel", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "image/jpg", "image/png", "image/jpeg", "text/plain", "application/pdf" };
            if (files.Any(c => allowType.Contains(c.ContentType)))
            {
                if (files.Sum(c => c.Length) <= 1024 * 1024 * 100)
                {
                    foreach (var file in files)
                    {
                        //文件后缀
                        var fileExtension = Path.GetExtension(file.FileName);
                        //文件名
                        fileName = Path.GetFileNameWithoutExtension(file.FileName);
                        strpath = Path.Combine("Files", Url, fileName + fileExtension);
                        path = Path.Combine(currentDirectory, "wwwroot", strpath);
                        short num = 1;
                        while (true)
                        {
                            if (!File.Exists(path))
                            {
                                break;
                            }
                            else
                            {
                                //文件名
                                fileName = Path.GetFileNameWithoutExtension(file.FileName) + "（" + num + "）";
                                strpath = Path.Combine("Files", Url, fileName + fileExtension);
                                path = Path.Combine(currentDirectory, "wwwroot", strpath);
                                num += 1;
                            }
                        }
                        using (var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                        {
                            await file.CopyToAsync(stream);
                        }
                        list.Add(new FilesInfo { url = strpath.Replace("\\", "/"), name = fileName, size = int.Parse(Math.Ceiling(file.Length / 1024.0).ToString()), type = fileExtension, create_time = DateTime.Now });
                    }
                    return list;
                }
                else
                {
                    throw new MessageException("文件大小大于100M，不能上传");
                }
            }
            else
            {
                throw new MessageException("请上传文件（格式为7z,rar,zip,doc,docx,xls,xlsx,jpg,jpeg,png,txt,pdf）");
            }
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="org_id"></param>
        /// <param name="store_id"></param>
        /// <param name="files"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static async Task<List<FilesInfo>> FilesSave(string Url, int org_id, int store_id, IEnumerable<IFormFile> files, dynamic type)
        {
            var list = new List<FilesInfo>();
            string strpath = string.Empty;
            string path = string.Empty;
            string fileName = string.Empty;
            if (files == null || files.Count() <= 0)
            {
                throw new MessageException("请上传文件（格式为7z,rar,zip,doc,docx,xls,xlsx,jpg,jpeg,png,txt,pdf）");
            }
            string currentDirectory = Path.GetDirectoryName(type.Assembly.Location);

            string fileurl = Path.Combine(currentDirectory + @"\wwwroot");

            string dirUrl = "";
            if (string.IsNullOrEmpty(Url))
            {
                dirUrl = fileurl + "/FilesManage/" + org_id + "/" + store_id + "/";
            }
            else
            {
                dirUrl = fileurl + "/" + Url;
            }

            if (!Directory.Exists(dirUrl))
            {
                Directory.CreateDirectory(dirUrl);
            }
            //格式限制
            var allowType = new string[] { "application/x-7z-compressed", "application/octet-stream", "application/x-zip-compressed", "application/zip", "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "application/vnd.ms-excel", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "image/jpg", "image/png", "image/jpeg", "text/plain", "application/pdf" };
            if (files.Any(c => allowType.Contains(c.ContentType)))
            {
                if (files.Sum(c => c.Length) <= 1024 * 1024 * 100)
                {
                    strpath = Path.Combine("FilesManage", org_id.ToString(), store_id.ToString());// + file.FileName
                    if (!string.IsNullOrEmpty(Url))
                    {
                        strpath = Url;
                    }
                    foreach (var file in files)
                    {
                        //文件后缀
                        var fileExtension = Path.GetExtension(file.FileName);
                        //文件名
                        fileName = Path.GetFileNameWithoutExtension(file.FileName);
                        path = Path.Combine(currentDirectory, "wwwroot", strpath, file.FileName);
                        short num = 1;
                        while (true)
                        {
                            if (!File.Exists(path))
                            {
                                break;
                            }
                            else
                            {
                                //文件名
                                fileName = Path.GetFileNameWithoutExtension(file.FileName) + "（" + num + "）";
                                path = Path.Combine(currentDirectory, "wwwroot", strpath, fileName + fileExtension);
                                num += 1;
                            }
                        }
                        using (var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                        {
                            await file.CopyToAsync(stream);
                        }
                        list.Add(new FilesInfo { url = strpath.Replace("\\", "/"), name = fileName, size = int.Parse(Math.Ceiling(file.Length / 1024.0).ToString()), type = fileExtension, create_time = DateTime.Now });
                    }
                    return list;
                }
                else
                {
                    throw new MessageException("文件大于100M，不能上传");
                }
            }
            else
            {
                throw new MessageException("请上传文件（格式为7z,rar,zip,doc,docx,xls,xlsx,jpg,jpeg,png,txt,pdf）");
            }
        }
    }
}
