using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.OA
{
    /// <summary>
    /// 修改文件或文件夹名
    /// </summary>
    public class UpdateFilesOrFolderName : oa_files
    {
        /// <summary>
        /// 旧名称
        /// </summary>
        public string old_name { get; set; }
    }
}
