using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 投诉图片
    /// </summary>
    public class r_complaint_img
    {
        /// <summary>
        /// 投诉图片
        /// </summary>
        public r_complaint_img()
        {
        }

        private System.Int32 _feedback_id;
        /// <summary>
        /// 反馈ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 feedback_id { get { return this._feedback_id; } set { this._feedback_id = value; } }

        private System.String _img_url;
        /// <summary>
        /// 图片路径
        /// </summary>
        public System.String img_url { get { return this._img_url; } set { this._img_url = value?.Trim(); } }
    }
}