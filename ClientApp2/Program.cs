using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using SocketDemo;

namespace ClientApp2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SocketClient client = new SocketClient(8888);
            Socket clientSocket = client.StartClient();
            bool exit = false;
            do
            {
                Task.Run(() =>
                {
                    client.SendMessage(clientSocket);
                });
                Task.Run(() =>
                {
                    client.ReceiveMessage(clientSocket);
                    if (!client.IsConnected)
                        exit = true;
                    return;
                }); 

            } while (client.IsConnected && !exit);

            Console.WriteLine("程序已经退出，按任意键关闭……");
            Console.ReadKey();
        }
    }
}
