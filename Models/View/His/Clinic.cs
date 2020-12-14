using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.His
{
    /// <summary>
    /// 接诊
    /// </summary>
    public class Clinic
    {
        private System.Int32 _regid;
        /// <summary>
        /// 挂号ID
        /// </summary>
        public System.Int32 regid { get { return this._regid; } set { this._regid = value; } }
    }
}
