using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace 通讯桥
{
    public class SocketServer
    {
        string _ip = string.Empty;
        string _port = string.Empty;
        private Socket Server = null;
        private byte[] buffer = new byte[1024 * 1024 * 2];
        Socket clientSocket = null;
        
        public SocketServer(string IP,string Port)
        {
            this._ip = IP;
            this._port = Port;


            //1.0 实例化套接字(IP4寻找协议,流式协议,TCP协议)
            Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        ~SocketServer()
        {
            StopListen();
        }

        public void StartListen()
        {
            try
            {
                //2.0 创建IP对象
                IPAddress address = IPAddress.Parse(_ip);
                //3.0 创建网络端口,包括ip和端口
                IPEndPoint endPoint = new IPEndPoint(address, int.Parse(_port));
                //4.0 绑定套接字
                Server.Bind(endPoint);
                //5.0 设置最大连接数
                Server.Listen(int.MaxValue);
                //Console.WriteLine("监听{0}消息成功", Server.LocalEndPoint.ToString());
                //6.0 开始监听
                Thread thread = new Thread(ListenClientConnect);
                thread.Start();
                Growl.Success("服务器开启");
            }
            catch
            {
                Growl.Error("服务器创建失败");
                if (Server.Connected == true)
                {
                    Server.Shutdown(SocketShutdown.Both);
                    Server.Close();
                }
            }
        }

        /// <summary>
        /// 监听客户端连接
        /// </summary>
        private void ListenClientConnect()
        {
            try
            {
                while (true)
                {
                    //Socket创建的新连接
                    clientSocket = Server.Accept();
                    Thread thread = new Thread(ReceiveMessage);
                    thread.Start(clientSocket);
                    Growl.Success("服务器接受到连接请求");
                }
            }
            catch
            {
                Growl.Error("服务器开启失败");
            }
        }
        
        /// <summary>
        /// 接收客户端消息
        /// </summary>
        /// <param name="socket">来自客户端的socket</param>
        private void ReceiveMessage(object socket)
        {
            Socket clientSocket = (Socket)socket;
            while (true)
            {
                try
                {
                    //获取从客户端发来的数据
                    int length = clientSocket.Receive(buffer);
                    if (length == 0)
                    {
                        Growl.Error("客户端" + clientSocket.RemoteEndPoint.ToString() + "已断开连接");
                        clientSocket.Shutdown(SocketShutdown.Both);
                        clientSocket.Close();
                        break;
                    }
                    MV_GetMess(Encoding.UTF8.GetString(buffer, 0, length));
                }
                catch
                {
                    //Console.WriteLine(ex.Message);
                    Growl.Error("服务器接受失败");

                    if (clientSocket!=null && clientSocket.Connected == true)
                    {
                        clientSocket.Shutdown(SocketShutdown.Both);
                        clientSocket.Close();
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// 事件
        /// </summary>
        public event EventHandler<string> MV_onMess;

        private void MV_GetMess(string s)
        {
            MV_onMess?.Invoke(this, s);
        }

        /// <summary>
        /// 发送一条消息
        /// </summary>
        /// <param name="buf">信息文本</param>
        public void SocketSend(byte[] buf)
        {
            try
            {
                if (clientSocket != null && clientSocket.Connected)
                {
                    clientSocket.Send(buf);
                }
            }
            catch
            {
                Growl.Error("服务器信息发送失败");
            }
        }
        
        public void StopListen()
        {
            try
            {
                if (clientSocket.Connected == true)
                {

                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                    clientSocket.Dispose();
                }
            }
            catch 
            {
                Growl.Error("远程客户端连接无法关闭");
                return;
            }

            try
            {
                if (Server.Connected == true)
                {
                    Server.Shutdown(SocketShutdown.Both);
                    Server.Close();
                    Server.Dispose();
                }
            }
            catch
            {
                Growl.Error("本地服务端无法关闭");
                return;
            }

            Growl.Info("服务器关闭成功");
            
        }
    }
}
