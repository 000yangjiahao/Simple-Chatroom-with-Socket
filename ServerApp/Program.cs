using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SocketDemo;

namespace ServerApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string command = null;
            SocketServer server = new SocketServer(8888);
            server.StartServer();


            do
            {
                Socket client = server.GetClient();
                string clientName = server.NameAccept(client);
                Task.Run(() => server.ReceiveMessage(client, clientName));
                Task.Run(() =>
                {
                    while (true)
                    {
                        command = server.SendMessage(client);
                        if (command.Equals("kick all client"))
                        {
                            server.KickAllClients();
                            break;
                        }
                    }
                });
            } while (true);
        }
    }
}
