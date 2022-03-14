using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace 通讯桥
{
    public class SocketClient: DispatcherObject
    {
        private string _ip = string.Empty;
        private string _port = string.Empty;
        private string _serIP = string.Empty;
        private Socket Client = null;
        private byte[] buffer = new byte[1024 * 1024 * 2];

        private BackgroundWorker m_ReceServiceBuf_BackgroundWorker = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ip">连接服务器的IP</param>
        /// <param name="port">连接服务器的端口</param>
        public SocketClient(string ip, string port,string SerIP)
        {
            this._ip = ip;
            this._port = port;
            this._serIP = SerIP;

            Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        /// <summary>
        /// 开启服务,连接服务端
        /// </summary>
        public bool StartClient()
        {
            try
            {
                Client.Bind(new IPEndPoint(IPAddress.Parse(_serIP), int.Parse(_port)));
                Client.Connect(new IPEndPoint(IPAddress.Parse(_ip), int.Parse("8081")));

                m_ReceServiceBuf_BackgroundWorker = new BackgroundWorker();
                m_ReceServiceBuf_BackgroundWorker.DoWork += ReceServiceDataClick;
                m_ReceServiceBuf_BackgroundWorker.WorkerSupportsCancellation = true; //允许取消
                m_ReceServiceBuf_BackgroundWorker.RunWorkerAsync();//开始执行DoWork
                Growl.Success("客户端开启");
                return true;

                #region 弃用
                ////1.0 实例化套接字(IP4寻址地址,流式传输,TCP协议)
                //Cilent = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                ////2.0 创建IP对象
                //IPAddress address = IPAddress.Parse(_ip);
                ////3.0 创建网络端口包括ip和端口
                //IPEndPoint endPoint = new IPEndPoint(address, int.Parse(_port));
                ////4.0 建立连接
                //Cilent.Connect(endPoint);
                //Console.WriteLine("连接服务器成功");
                ////5.0 接收数据
                //int length = Cilent.Receive(buffer);
                //Console.WriteLine("接收服务器{0},消息:{1}", Cilent.RemoteEndPoint.ToString(), Encoding.UTF8.GetString(buffer, 0, length));
                ////6.0 像服务器发送消息
                //for (int i = 0; i < 10; i++)
                //{
                //    Thread.Sleep(2000);
                //    string sendMessage = string.Format("客户端发送的消息,当前时间{0}", DateTime.Now.ToString());
                //    Cilent.Send(Encoding.UTF8.GetBytes(sendMessage));
                //    Console.WriteLine("像服务发送的消息:{0}", sendMessage);
                //}
                #endregion
            }
            catch
            {
                Growl.Error("客户端开启失败");

                if (Client.Connected == true)
                {
                    Client.Shutdown(SocketShutdown.Both);
                    Client.Close();
                }
                return false;
            }
        }

        /// <summary>
        /// 发送一条消息
        /// </summary>
        /// <param name="buf">信息文本</param>
        public void SocketSend(byte[] buf)
        {
            try
            {
                if (Client != null && Client.Connected)
                {
                    Client.Send(buf);
                }
            }
            catch
            {
                
            }
        }

        /// <summary>
        /// 监听
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReceServiceDataClick(object sender, DoWorkEventArgs e)
        {
            try
            {
                while (!m_ReceServiceBuf_BackgroundWorker.CancellationPending)
                {
                    if (Client.Poll(10, SelectMode.SelectRead))
                    {
                        m_ReceServiceBuf_BackgroundWorker.CancelAsync();//停止执行DoWork
                        break;
                    }
                    byte[] buffer = new byte[4096];
                    int nReceBufCnt = Client.Receive(buffer);

                    Dispatcher.Invoke(() =>
                    {
                        MV_GetMess(Encoding.UTF8.GetString(buffer));
                    });

                }
                Client.Shutdown(SocketShutdown.Both);
                Client.Close();
            }
            catch (Exception ex)
            {
                Growl.Error("信息监听失败");
                //System.Diagnostics.Debug.WriteLine(ex.Message);
                if (Client.Connected == true)
                {
                    m_ReceServiceBuf_BackgroundWorker.CancelAsync();
                    Client.Shutdown(SocketShutdown.Both);
                    Client.Close();
                }

            }
        }

        private void MV_GetMess(string s)
        {
            MV_onMess?.Invoke(this, s);
        }

        /// <summary>
        /// 事件
        /// </summary>
        public event EventHandler<string> MV_onMess;

        public void StopClient()
        {
            try
            {
                if (Client.Connected == true)
                {

                    m_ReceServiceBuf_BackgroundWorker.CancelAsync();
                    Client.Shutdown(SocketShutdown.Both);
                    Client.Close();
                    Client.Dispose();
                }
            }
            catch (Exception)
            {
                Growl.Info("本地客户端关闭失败");
                return;
            }
            

            Growl.Info("本地客户端关闭成功");
        }
    }
}
