using SqlSugar;

namespace Models.DB
{
    /// <summary>
    /// 诊室所属科室
    /// </summary>
    public class p_cons_room_dept
    {
        /// <summary>
        /// 诊室所属科室
        /// </summary>
        public p_cons_room_dept()
        {
        }

        private System.Int32 _cons_room_id;
        /// <summary>
        /// 诊室ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 cons_room_id { get { return this._cons_room_id; } set { this._cons_room_id = value; } }

        private System.Int32 _dept_id;
        /// <summary>
        /// 科室ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public System.Int32 dept_id { get { return this._dept_id; } set { this._dept_id = value; } }
    }
}