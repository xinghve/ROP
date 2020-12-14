using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 活动图片
    /// </summary>
    public class r_activity_img
    {
        /// <summary>
        /// 活动图片
        /// </summary>
        public r_activity_img()
        {
        }

        private System.Int32 _activity_id;
        /// <summary>
        /// 活动ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 activity_id { get { return this._activity_id; } set { this._activity_id = value; } }

        private System.String _img_url;
        /// <summary>
        /// 图片路径
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.String img_url { get { return this._img_url; } set { this._img_url = value?.Trim(); } }
    }
}