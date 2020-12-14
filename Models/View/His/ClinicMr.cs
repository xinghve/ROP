using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.His
{
    /// <summary>
    /// 病历
    /// </summary>
    public class ClinicMr
    {

        private System.Int32 _clinicid;
        /// <summary>
        /// 就诊ID
        /// </summary>
        public System.Int32 clinicid { get { return this._clinicid; } set { this._clinicid = value; } }

        private System.DateTime _clinic_time;
        /// <summary>
        /// 就诊时间
        /// </summary>
        public System.DateTime clinic_time { get { return this._clinic_time; } set { this._clinic_time = value; } }

        private System.String _diagnosis;
        /// <summary>
        /// 诊断
        /// </summary>
        public System.String diagnosis { get { return this._diagnosis; } set { this._diagnosis = value?.Trim(); } }
    }
}
