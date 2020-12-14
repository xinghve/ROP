using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 
    /// </summary>
    public class oa_leave_image
    {
        /// <summary>
        /// 
        /// </summary>
        public oa_leave_image()
        {
        }

        private System.String _leave_no;
        /// <summary>
        /// 请假单号
        /// </summary>
        public System.String leave_no { get { return this._leave_no; } set { this._leave_no = value?.Trim(); } }

        private System.String _img_url;
        /// <summary>
        /// 图片路径
        /// </summary>
        public System.String img_url { get { return this._img_url; } set { this._img_url = value?.Trim(); } }
    }
}