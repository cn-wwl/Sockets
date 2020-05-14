using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClientServer
{
    class Program
    {
        static void Main(string[] args)
        { 
            ServerAsynSocket server = new ServerAsynSocket();
            server.Init();

            Task task = Task.Run(() =>
            {
                while (true)
                {
                    server.SendData();
                }
            });
            Console.ReadKey(); 
        }
    }
}
