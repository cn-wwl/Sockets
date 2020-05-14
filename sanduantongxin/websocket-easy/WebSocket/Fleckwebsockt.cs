using Fleck;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using websocket_easy.ClientSocket;
using websocket_easy.Models;
using websocket_easy.WebSocket;

namespace websocket_easy
{
    public class Fleckwebsockt
    {
        /// <summary>
        /// 消息服务器
        /// </summary>
        public static WebSocketServer ChartSocket { get; set; }

        /// <summary>
        /// websocket连接管理器
        /// </summary>
        public static ConcurrentDictionary<Guid, websocketModel> Sockets { get; set; }

        public string msg = string.Empty;
        public static void ServerStart()
        {

            var _port = System.Configuration.ConfigurationManager.AppSettings["WebsocketProt"].ToString();
            if (ChartSocket != null)
            {
                ChartSocket.Dispose();
            }

            ChartSocket = new WebSocketServer("ws://0.0.0.0:" + _port);

            ChartSocket.Start(socket =>
            {

                socket.OnOpen = () =>
                {
                    socket.Send("连接成功");
                    if (Sockets == null)
                    {
                        Sockets = new ConcurrentDictionary<Guid, websocketModel>();
                    }
                    Sockets.TryAdd(socket.ConnectionInfo.Id, new websocketModel(socket));
                };
                socket.OnMessage = (msg) =>
                {
                    //{"ModuleName":"ClickToCopy","Content":{"ConcentratorNo":"101","WaterMtrAddress":"101001"}} 
                    if (!msg.Contains("ConcentratorNo"))
                    {
                        socket.Send($"请发送点抄指令数据");
                        return;
                    }
                    var message = JsonConvert.DeserializeObject<MessageModel>(msg);
                       
                    var msglist = Sockets[socket.ConnectionInfo.Id].msgdic;
                    if (ClientAsynSocket.ClientSocket == null || !ClientAsynSocket.ClientSocket.Connected )
                    {
                        ClientAsynSocket.Init();
                        System.Threading.Thread.Sleep(2000);
                        if (ClientAsynSocket.ClientSocket == null || !ClientAsynSocket.ClientSocket.Connected)
                        {
                            socket.Send($"抄表应用未启动");
                            return;
                        }
                    }
                    if (msglist.Any(s => s.ConcentratorNo.Equals(message.Content.ConcentratorNo) && s.WaterMtrAddress.Equals(message.Content.WaterMtrAddress)))
                    {
                        socket.Send($"不要重复发送，{ msg }");
                        return;
                    }
                    Sockets[socket.ConnectionInfo.Id].msgdic.Add(message.Content); 
                };
            });

            Task sendclientServer = Task.Run(() =>
            {
                while (true)
                {
                    SendMessage();
                }
            });
            Task ReplyWebClient = Task.Run(() =>
            {
                while (true)
                {
                    Reply();
                }
            });
        }


        /// <summary>
        /// 发送给clientserver监听程序
        /// </summary>
        public static void SendMessage()
        {
            if (Sockets!=null && Sockets.Count>0)
            {
                foreach (var item in Sockets.Values)
                { 
                    foreach (var command in item.msgdic)
                    {
                        if (!command.IsDownCommand)
                        {
                            var msg = JsonConvert.SerializeObject(command);
                            ClientAsynSocket.Send(msg);
                            command.IsDownCommand = true;
                        }

                    }
                }
            }
        }


        /// <summary>
        /// 回复webClient
        /// </summary>
        public static void Reply()
        {
            if (Sockets != null && Sockets.Count > 0 && ClientAsynSocket.msglis != null && ClientAsynSocket.msglis.Count > 0)
            {
                ClickToCopyModel msg = null;
                ClientAsynSocket.msglis.TryDequeue(out msg);


                foreach (var item in Sockets.Values)
                {
                    if (item.msgdic.Any(s => s.WaterMtrAddress.Equals(msg.WaterMtrAddress)))
                    {
                        ClickToCopyModel msgr = null;
                        item.msgdic.TryTake(out msgr);

                        string sendmsg = JsonConvert.SerializeObject(msg);
                        item.Socket.Send(sendmsg);
                    }
                }
            }
        } 
    } 
}