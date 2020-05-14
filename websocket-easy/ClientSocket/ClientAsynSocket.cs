using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web;
using websocket_easy.Models;

namespace websocket_easy.ClientSocket
{
    public class ClientAsynSocket
    {
        public static Socket ClientSocket;

        public static ConcurrentQueue<ClickToCopyModel> msglis = new ConcurrentQueue<ClickToCopyModel>();



        /// <summary>
        /// 接收的单个报文的最大长度 
        /// </summary> 
        public const int ReceiveBufferSize = 1166;


        public static void Init()
        {
            String IP = "127.0.0.1"; 
            var port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ReadingMtrPort"]);

            IPAddress ip = IPAddress.Parse(IP);  //将IP地址字符串转换成IPAddress实例
            ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//使用指定的地址簇协议、套接字类型和通信协议
            IPEndPoint endPoint = new IPEndPoint(ip, port); // 用指定的ip和端口号初始化IPEndPoint实例

            ClientSocket.BeginConnect(endPoint, new AsyncCallback(ConnectCallBack), ClientSocket);
        }

        private static void ConnectCallBack(IAsyncResult iar)
        {
            Socket client = (Socket)iar.AsyncState;
            try
            {
                client.EndConnect(iar);
                Recive();
            }
            catch (SocketException e)
            {
                Console.WriteLine("服务器程序未运行或服务器端口未开放");

            }
        }

        public static void Recive()
        {
            byte[] data = new byte[1024];
            try
            {
                ClientSocket.BeginReceive(data, 0, data.Length, SocketFlags.None,
                asyncResult =>
                {
                    try
                    {
                        int length = ClientSocket.EndReceive(asyncResult);
                        var msg=JsonConvert.DeserializeObject<ClickToCopyModel>(Encoding.GetEncoding("GB2312").GetString(data));
                        Console.WriteLine("接收到消息：" + Encoding.GetEncoding("GB2312").GetString(data));
                        msglis.Enqueue(msg);
                        Recive();
                    }
                    catch (SocketException e)
                    {
                        if (e.ErrorCode == 10054)
                        {
                            Console.WriteLine("服务器已断线");
                        }
                        else
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                }, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void Send(string messagestr)
        {
            byte[] message = Encoding.Default.GetBytes(messagestr);  //通信时实际发送的是字节数组，所以要将发送消息转换字节
             ClientSocket.Send(message);
            Console.WriteLine("发送消息为:" + messagestr);
        } 
    }
}