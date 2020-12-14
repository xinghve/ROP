using Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.His
{
    /// <summary>
    /// 挂号类别
    /// </summary>
    public class RegisterType : his_registertype
    {

        /// <summary>
        /// 类别费用
        /// </summary>
        public List<his_registertypefee> registertypefees { get; set; }
    }
}
