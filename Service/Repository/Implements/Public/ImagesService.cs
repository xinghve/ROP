using Models.DB;
using Models.View.Public;
using Service.Repository.Interfaces.Public;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace Service.Repository.Implements.Public
{
    /// <summary>
    /// 图片管理
    /// </summary>
    public class ImagesService : DbContext, IImagesService
    {
        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="relation_code">关联编码</param>
        /// <returns></returns>
        public async Task<List<p_image>> GetAsync(string relation_code)
        {
            return await Db.Queryable<p_image>().Where(w => w.relation_code == relation_code).WithCache().ToListAsync();
        }

        /// <summary>
        /// 设置图片
        /// </summary>
        /// <param name="image"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<bool> SetAsync(Image image, dynamic type)
        {
            string currentDirectory = Path.GetDirectoryName(type.Assembly.Location);
            var newVs = image.vs;
            //需要删除的图片
            var list = await Db.Queryable<p_image>().Where(w => w.relation_code == image.relation_code && !newVs.Contains(w.url)).WithCache().ToListAsync();
            foreach (var item in list)
            {
                var url = currentDirectory + "/wwwroot/" + item.url.Trim();
                if (System.IO.File.Exists(url))
                {
                    System.IO.File.Delete(url);//删除无用文件
                }
            }
            var newImage = new List<p_image>();
            for (int i = 0; i < newVs.Count; i++)
            {
                newImage.Add(new p_image { relation_code = image.relation_code, url = newVs[i] });
            }
            var ret = -1;
            ret = await Db.Deleteable<p_image>().Where(w => w.relation_code == image.relation_code).RemoveDataCache().EnableDiffLogEvent().ExecuteCommandAsync();
            if (ret != -1)
            {
                await Db.Insertable(newImage).ExecuteCommandAsync();
                redisCache.RemoveAll<p_image>();
            }
            return true;
        }
    }
}
