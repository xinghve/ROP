using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.His
{
    /// <summary>
    /// 预约挂号
    /// </summary>
    public class HisRegister
    {
        private System.Int32 _store_id;
        /// <summary>
        /// 门店ID
        /// </summary>
        public System.Int32 store_id { get { return this._store_id; } set { this._store_id = value; } }

        private System.Int32 _deptid;
        /// <summary>
        /// 部门ID
        /// </summary>
        public System.Int32 deptid { get { return this._deptid; } set { this._deptid = value; } }

        private System.String _deptname;
        /// <summary>
        /// 部门
        /// </summary>
        public System.String deptname { get { return this._deptname; } set { this._deptname = value?.Trim(); } }

        private System.Int32 _doctorid;
        /// <summary>
        /// 医生ID
        /// </summary>
        public System.Int32 doctorid { get { return this._doctorid; } set { this._doctorid = value; } }

        private System.String _doctorname;
        /// <summary>
        /// 医生
        /// </summary>
        public System.String doctorname { get { return this._doctorname; } set { this._doctorname = value?.Trim(); } }

        private System.Int32 _typeid;
        /// <summary>
        /// 类别ID
        /// </summary>
        public System.Int32 typeid { get { return this._typeid; } set { this._typeid = value; } }

        private System.Int32 _centerid;
        /// <summary>
        /// 档案ID
        /// </summary>
        public System.Int32 centerid { get { return this._centerid; } set { this._centerid = value; } }

        private System.DateTime _regdate;
        /// <summary>
        /// 挂号时间
        /// </summary>
        public System.DateTime regdate { get { return this._regdate; } set { this._regdate = value; } }

        private System.String _cardno;
        /// <summary>
        /// 就诊卡
        /// </summary>
        public System.String cardno { get { return this._cardno; } set { this._cardno = value?.Trim(); } }

        private System.Decimal _shouldamount;
        /// <summary>
        /// 应收金额
        /// </summary>
        public System.Decimal shouldamount { get { return this._shouldamount; } set { this._shouldamount = value; } }

        private System.Int16 _orderflag;
        /// <summary>
        /// 预约标志
        /// </summary>
        public System.Int16 orderflag { get { return this._orderflag; } set { this._orderflag = value; } }

        private System.Int32? _roomid;
        /// <summary>
        /// 诊室ID
        /// </summary>
        public System.Int32? roomid { get { return this._roomid; } set { this._roomid = value ?? default(System.Int32); } }

        private System.String _room;
        /// <summary>
        /// 诊室
        /// </summary>
        public System.String room { get { return this._room; } set { this._room = value?.Trim(); } }

        private System.String _summary;
        /// <summary>
        /// 摘要
        /// </summary>
        public System.String summary { get { return this._summary; } set { this._summary = value?.Trim(); } }

        private System.Int32 _scheduleid;
        /// <summary>
        /// 排班ID
        /// </summary>
        public System.Int32 scheduleid { get { return this._scheduleid; } set { this._scheduleid = value; } }

        private System.Int16 _days;
        /// <summary>
        /// 星期数
        /// </summary>
        public System.Int16 days { get { return this._days; } set { this._days = value; } }

        private System.Int16 _no;
        /// <summary>
        /// 排班时间段序号
        /// </summary>
        public System.Int16 no { get { return this._no; } set { this._no = value; } }

        private System.Int16 _sourceid;
        /// <summary>
        /// 来源ID
        /// </summary>
        public System.Int16 sourceid { get { return this._sourceid; } set { this._sourceid = value; } }

        private System.String _source;
        /// <summary>
        /// 来源
        /// </summary>
        public System.String source { get { return this._source; } set { this._source = value?.Trim(); } }
    }

    /// <summary>
    /// 预约改期
    /// </summary>
    public class ModifyDateModel: HisRegister
    {
        /// <summary>
        /// 当前挂号单id
        /// </summary>
        public int regid { get; set; }
    }
}
