using Fleck;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using websocket_easy.Models;

namespace websocket_easy
{
    public class websocketModel
    {
        public websocketModel(IWebSocketConnection socket)
        {
            Socket = socket;
            SocketID = socket.ConnectionInfo.Id;
            msgdic = new ConcurrentBag<ClickToCopyModel>();
        }

        public Guid SocketID { get; set; }

        public IWebSocketConnection Socket { get; set; }


        /// <summary>
        /// 消息集合
        /// </summary>
        public ConcurrentBag<ClickToCopyModel> msgdic { get; set; }
    }
}