using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SocketDemo
{
    public class SocketServer
    {
        private string _ip = string.Empty;
        private int _port = 0;
        private Socket _socket = null;
        private byte[] buffer = new byte[1024 * 1024 * 2];
        Dictionary<string, Socket> _sockets = new Dictionary<string, Socket>();

        public SocketServer(string ip, int port)
        {
            this._ip = ip;
            this._port = port;
        }
        public SocketServer(int port)
        {
            this._ip = "0.0.0.0";
            this._port = port;
        }

        public void StartServer()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress address = IPAddress.Parse(_ip);
            IPEndPoint endPoint = new IPEndPoint(address, _port);
            _socket.Bind(endPoint);
            _socket.Listen(int.MaxValue);
        }
        public Socket GetClient()
        {
            Socket clientSocket = _socket.Accept();
            return clientSocket;
        }

        public void ReceiveMessage(object socket, string name)
        {
            Socket clientSocket = (Socket)socket;
            while (true)
            {
                if (clientSocket.Connected)
                {
                    try
                    {
                        int length = clientSocket.Receive(buffer);
                        string message = Encoding.UTF8.GetString(buffer, 0, length);

                        if (message.Equals("exit"))
                        {
                            clientSocket.Close();
                            _sockets.Remove(name);
                            Console.WriteLine($"用户名为 {name} 的客户端已经断开连接.");
                            break; 
                        }
                        else
                        {
                            string formattedMessage = $"[{name}] {message}";
                            Console.WriteLine("收到来自 {0}, 的消息: {1}", name, formattedMessage);
                            foreach (var socketTemp in _sockets)
                            {
                                if (clientSocket != socketTemp.Value)
                                {
                                    byte[] messageBytes = Encoding.UTF8.GetBytes(formattedMessage);
                                    socketTemp.Value.Send(messageBytes);
                                    Console.WriteLine("转发来自{0}, 的消息: {1}", name, formattedMessage);
                                }
                            }
                        }
                    }
                    catch
                    {
                        clientSocket.Close();
                        _sockets.Remove(name);
                        break;
                    }
                }
                else
                {
                    _sockets.Remove(name);
                    break;
                }
            }
        }
        public string SendMessage(object socket)
        {
            Socket clientSocket = (Socket)socket;
            try
            {
                if (_sockets.Count != 0)
                {
                    string sendMessage = Console.ReadLine();
                    foreach (var socketTemp in _sockets)
                    {
                        socketTemp.Value.Send(Encoding.UTF8.GetBytes(sendMessage));
                        Console.WriteLine($"像客户端发送 {socketTemp.Key}的消息 : {sendMessage}");
                    }
                    return sendMessage;
                }
                else
                    return null;

            }
            catch
            {
                clientSocket.Close();
                return null;
            }
        }
        public string NameAccept(object socket)
        {
            Socket clientSocket = (Socket)socket;
            Console.WriteLine("监听{0}消息成功", clientSocket.RemoteEndPoint.ToString());
            clientSocket.Send(Encoding.UTF8.GetBytes("请输入你的用户名"));
            int length = clientSocket.Receive(buffer);
            string name = Encoding.UTF8.GetString(buffer, 0, length) + "#" + (clientSocket.RemoteEndPoint as IPEndPoint).Port;
            Console.WriteLine("成功接收用户名" + name);
            _sockets.Add(name, clientSocket);
            return name;
        }
        public void KickAllClients()
        {
            lock (_sockets)
            {
                try
                {
                    foreach (var clientSocket in _sockets.Values)
                    {
                        try
                        {
                            clientSocket.Send(Encoding.UTF8.GetBytes("您已经被服务器关闭"));
                            clientSocket.Close();
                        }
                        catch
                        {
                            clientSocket.Close();
                        }
                    }
                }
                catch
                {

                }

                _sockets.Clear(); 
            }
        }
    }
}
