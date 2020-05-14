using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using websocket_easy.Models;

namespace websocket_easy.WebSocket
{
    public class MessageModel
    {
        /// <summary>
        /// 模块名称
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public ClickToCopyModel Content { get; set; }
    }
}