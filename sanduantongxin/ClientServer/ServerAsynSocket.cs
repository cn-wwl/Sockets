using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientServer
{
    public class ServerAsynSocket
    {
        static Socket ServerSocket;

        /// <summary>
        /// 连接管理器
        /// </summary>
        public static ConcurrentDictionary<string, Connection> dic_conn = new ConcurrentDictionary<string, Connection>();


        /// <summary>
        /// 消息集合
        /// </summary>
        public ConcurrentQueue<ClickToCopyModel> msgdic = new ConcurrentQueue<ClickToCopyModel>();


        /// <summary>
        /// 监听队列长度
        /// </summary>
        public int ListenCount = 1000;


        /// <summary>
        /// 接收的单个报文的最大长度 
        /// </summary> 
        public const int ReceiveBufferSize = 1166;



        public void Init()
        {
            String IP = "127.0.0.1";
            var port = Convert.ToInt32(ConfigurationManager.AppSettings["ReadingMtrPort"]);

            IPAddress ip = IPAddress.Parse(IP);  //将IP地址字符串转换成IPAddress实例
            IPEndPoint endPoint = new IPEndPoint(ip, port);
            ServerSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            ServerSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
            ServerSocket.Bind(endPoint);
            ServerSocket.Listen(ListenCount);
            Console.WriteLine($"{endPoint.Address}:{endPoint.Port}已开启监听");
            Accept();
        }

        /// <summary>
        /// 接入请求
        /// </summary>
        void Accept()
        {
            //开启异步监听
            ServerSocket.BeginAccept(AcceptDone, null);
        }

        /// <summary>
        /// 完成接入请求事件
        /// </summary>
        /// <param name="result"></param>
        void AcceptDone(IAsyncResult result)
        {
            try
            {
                var clientSocket = ServerSocket.EndAccept(result);
                var conn = new Connection(clientSocket, ReceiveBufferSize);
                Receive(conn);
                Accept();//接着进行接收
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "AcceptDone出错");
            }
        }

        /// <summary>
        /// 开始接收
        /// </summary>
        /// <param name="socket"></param>
        public void Receive(Connection connection)
        {
            try
            {
                connection.Socket.BeginReceive(
                       connection.Buffer,
                       0,
                       ReceiveBufferSize,
                       SocketFlags.None,
                       ReceiveDone,
                       connection
                   );
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "Receive出错");
            }
        }

        /// <summary>
        /// 完成接收
        /// </summary>
        /// <param name="result"></param>
        void ReceiveDone(IAsyncResult result)
        {
            try
            {
                var connection = (Connection)result.AsyncState;

                EndPoint iPEnd = connection.Socket.RemoteEndPoint;


                //通过获取本次回传的数据长度来截取数据，故可以重复利用该缓冲区而不需要清空
                var bytesTransferred = 0;
                try
                {
                    bytesTransferred = connection.Socket.EndReceive(result);
                }
                catch (Exception ex)
                {
                    if (dic_conn.Values.Any(s => s.Socket.Equals(connection.Socket)))
                    {
                        Connection par = null;
                        dic_conn.TryRemove(iPEnd.ToString(), out par);
                    }
                    Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "远程客户端主动关闭");
                    connection.Release();
                    return;
                }

                if (bytesTransferred == 0)
                {
                    //TCP返回0至少可以肯定对方关闭了写方向的socket；所以这里直接关闭socket,进行资源释放
                    connection.Release();
                    return;
                }

                //将接受到的报文拷贝出来
                byte[] info = new byte[bytesTransferred];
                Buffer.BlockCopy(
                                        connection.Buffer,
                                        0,
                                        info,
                                        0,
                                        bytesTransferred
                                    );

                if (!dic_conn.Values.Any(s => s.Socket.Equals(connection.Socket)))
                {
                    dic_conn.TryAdd(iPEnd.ToString(), connection);
                }
                var commandmsg = Encoding.GetEncoding("GB2312").GetString(info);

                Console.WriteLine($"【webserver发送消息】：" + commandmsg);

                //简单判断了一下是不是web网站服务端的连接发过来的数据
                if (commandmsg.Contains("ConcentratorNo"))
                {
                    var model = JsonConvert.DeserializeObject<ClickToCopyModel>(commandmsg);

                    Random random = new Random();
                    var acc = Convert.ToDouble(random.Next() * 10);

                    model.AccumVal = acc;
                    msgdic.Enqueue(model);
                }
               


                //继续接收
                Receive(connection);

            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "ReceiveDone出错");
            }
        } 

        /// <summary>
        /// 回复网站服务端
        /// </summary>
        public void SendData()
        {
            while (true)
            {
                if (msgdic.IsEmpty)
                {
                    return;
                }
                ClickToCopyModel data = new ClickToCopyModel();
                msgdic.TryDequeue(out data);

                var msg = JsonConvert.SerializeObject(data);
                byte[] send = Encoding.Default.GetBytes(msg);
                if (!dic_conn.IsEmpty)
                {
                    dic_conn.First().Value.Socket.Send(send);
                    Console.WriteLine($"【clinetserver回复】：{ msg }");
                }
            }
        }
    }
}
