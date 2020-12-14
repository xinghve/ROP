using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.OA
{
    /// <summary>
    /// 文件分页实体
    /// </summary>
    public class FilesPageModel : oa_files
    {
        /// <summary>
        /// 文件夹数量
        /// </summary>
        public int folder_num { get; set; }

        /// <summary>
        /// 文件数量
        /// </summary>
        public int files_num { get; set; }
    }
}
