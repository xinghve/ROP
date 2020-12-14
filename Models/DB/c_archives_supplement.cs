using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 档案附页
    /// </summary>
    public class c_archives_supplement
    {
        /// <summary>
        /// 档案附页
        /// </summary>
        public c_archives_supplement()
        {
        }

        private System.Int32 _archives_id;
        /// <summary>
        /// 档案ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 archives_id { get { return this._archives_id; } set { this._archives_id = value; } }

        private System.String _drink;
        /// <summary>
        /// 饮料
        /// </summary>
        public System.String drink { get { return this._drink; } set { this._drink = value?.Trim(); } }

        private System.String _fruits;
        /// <summary>
        /// 水果
        /// </summary>
        public System.String fruits { get { return this._fruits; } set { this._fruits = value?.Trim(); } }

        private System.String _foods;
        /// <summary>
        /// 饮食
        /// </summary>
        public System.String foods { get { return this._foods; } set { this._foods = value?.Trim(); } }

        private System.String _smoke;
        /// <summary>
        /// 吸烟
        /// </summary>
        public System.String smoke { get { return this._smoke; } set { this._smoke = value?.Trim(); } }

        private System.String _habit;
        /// <summary>
        /// 习惯
        /// </summary>
        public System.String habit { get { return this._habit; } set { this._habit = value?.Trim(); } }

        private System.String _hobby;
        /// <summary>
        /// 爱好
        /// </summary>
        public System.String hobby { get { return this._hobby; } set { this._hobby = value?.Trim(); } }
    }
}