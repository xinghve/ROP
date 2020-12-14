﻿using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.His
{
    /// <summary>
    /// 排班时间
    /// </summary>
    public class EmpscheduleScheduletimes: his_empschedule
    {
        private System.Int16 _days;
        /// <summary>
        /// 星期数
        /// </summary>
        public System.Int16 days { get { return this._days; } set { this._days = value; } }

        private System.Int16 _no;
        /// <summary>
        /// 序号
        /// </summary>
        public System.Int16 no { get { return this._no; } set { this._no = value; } }

        private System.TimeSpan _begintime;
        /// <summary>
        /// 开始时间
        /// </summary>
        public System.TimeSpan begintime { get { return this._begintime; } set { this._begintime = value; } }

        private System.TimeSpan _endtime;
        /// <summary>
        /// 结束时间
        /// </summary>
        public System.TimeSpan endtime { get { return this._endtime; } set { this._endtime = value; } }

        private System.Int16 _stateid;
        /// <summary>
        /// 状态ID
        /// </summary>
        public System.Int16 stateid { get { return this._stateid; } set { this._stateid = value; } }

        private System.String _statename;
        /// <summary>
        /// 状态
        /// </summary>
        public System.String statename { get { return this._statename; } set { this._statename = value?.Trim(); } }

        private System.Int32? _replaceid;
        /// <summary>
        /// 替诊ID
        /// </summary>
        public System.Int32? replaceid { get { return this._replaceid; } set { this._replaceid = value ?? default(System.Int32); } }

        private System.Int32? _replacedocid;
        /// <summary>
        /// 替诊医生ID
        /// </summary>
        public System.Int32? replacedocid { get { return this._replacedocid; } set { this._replacedocid = value ?? default(System.Int32); } }

        private System.String _replacedocname;
        /// <summary>
        /// 替诊医生
        /// </summary>
        public System.String replacedocname { get { return this._replacedocname; } set { this._replacedocname = value?.Trim(); } }

        private System.String _typename;
        /// <summary>
        /// 类别名称
        /// </summary>
        public System.String typename { get { return this._typename; } set { this._typename = value?.Trim(); } }

        /// <summary>
        /// 挂号费
        /// </summary>
        public decimal price { get; set; }

        /// <summary>
        /// 诊室位置
        /// </summary>
        public string position { get; set; }
    }
}
