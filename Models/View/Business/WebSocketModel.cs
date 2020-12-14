using System;
using System.Collections.Generic;
using System.Text;

namespace Models.View.Business
{
    /// <summary>
    /// 消息提醒model
    /// </summary>
    public class WebSocketModel
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public int userId { get; set; }

        /// <summary>
        /// 提醒内容
        /// </summary>
        public string content { get; set; }
    }

   
}
