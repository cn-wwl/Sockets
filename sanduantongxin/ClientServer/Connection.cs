using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientServer
{
    /// <summary>
    /// 客户端连接器
    /// </summary>
    public class Connection
    {
        /// <summary>
        /// 联通的Socket
        /// </summary>
        public Socket Socket;

        /// <summary>
        /// 连接器对应的缓冲
        /// </summary>
        public byte[] Buffer;


        /// <summary>
        /// 消息集合
        /// </summary>
        public ConcurrentDictionary<string, ClickToCopyModel> msgdic { get; set; }

        /// <summary>
        /// 连接器
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="bufferSize"></param>
        public Connection(Socket socket, int bufferSize)
        {
            Socket = socket;
            Buffer = new byte[bufferSize];
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Release()
        {
            Buffer = null;
            Socket.Close();
        }
    }
}
