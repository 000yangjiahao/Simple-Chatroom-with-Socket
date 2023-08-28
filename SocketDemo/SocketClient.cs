using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketDemo
{
    public class SocketClient
    {
        private string _ip = string.Empty;
        private int _port = 0;
        private Socket _socket = null;
        private bool isConnect;
        private byte[] buffer = new byte[1024 * 1024 * 2];

        public SocketClient(string ip, int port)
        {
            this._ip = ip;
            this._port = port;
        }
        public SocketClient(int port)
        {
            this._ip = "127.0.0.1";
            this._port = port;
        }

        public Socket StartClient()
        {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress address = IPAddress.Parse(_ip);
                IPEndPoint endPoint = new IPEndPoint(address, _port);
                _socket.Connect(endPoint);
                Console.WriteLine("连接服务器成功");
                isConnect = true;
                return _socket;
        }
        public void SendMessage(Socket socket)
        {
            try
            {
                string sendMessage = Console.ReadLine();
                if (sendMessage.Equals("exit"))
                {
                    socket.Send(Encoding.UTF8.GetBytes(sendMessage));
                    Console.WriteLine("您已经退出.");
                    isConnect = false;
                    socket.Close();
                }
                else
                {
                    socket.Send(Encoding.UTF8.GetBytes(sendMessage));
                    Console.WriteLine("发送给服务器: {0}", sendMessage);
                }
            }
            catch
            {
                socket.Close();
            }
        }
        public void ReceiveMessage(Socket socket)
        {
            try
            {
                int receiveMessage = socket.Receive(buffer);
                if(Encoding.UTF8.GetString(buffer, 0, receiveMessage).Equals("kick all client"))
                {
                    Console.WriteLine("您已经被服务器踢出");
                    isConnect = false;
                    socket.Close();
                }
                else
                {
                    Console.WriteLine("服务端 {0} 发来消息 {1}", socket.RemoteEndPoint.ToString(), Encoding.UTF8.GetString(buffer, 0, receiveMessage));
                }
            }
            catch
            {
                socket.Close();
            }

        }

        public bool IsConnected
        {
            get { return isConnect; }
        }
    }
}
