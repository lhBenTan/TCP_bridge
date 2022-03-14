using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Utils;
using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace 通讯桥
{
    public class CommunicationViewModel:ViewModelBase
    {
        private string FilePath;

        public CommunicationViewModel()
        {
            //读取配置

            ServerIP = "192.168.8.186";
            ServerPort = "80";

            ClientIP = "192.168.8.186";
            ClientPort = "81";
        }

        Queue<string> output = new Queue<string>();

        /// <summary>
        /// 界面显示
        /// </summary>
        public string Console
        {
            get => GetProperty(() => Console);
            set => SetProperty(() => Console, value);
        }

        /// <summary>
        /// 服务端IP
        /// </summary>
        public string ServerIP
        {
            get => GetProperty(() => ServerIP);
            set => SetProperty(() => ServerIP, value);
        }

        /// <summary>
        /// 服务端端口
        /// </summary>
        public string ServerPort
        {
            get => GetProperty(() => ServerPort);
            set => SetProperty(() => ServerPort, value);
        }

        /// <summary>
        /// 客户端IP
        /// </summary>
        public string ClientIP
        {
            get => GetProperty(() => ClientIP);
            set => SetProperty(() => ClientIP, value);
        }

        /// <summary>
        /// 客户端端口
        /// </summary>
        public string ClientPort
        {
            get => GetProperty(() => ClientPort);
            set => SetProperty(() => ClientPort, value);
        }

        public SocketServer server;
        public SocketClient client;
        
        public void Connect()
        {
            client = new SocketClient(ClientIP, ClientPort, ServerIP);
            //server = new SocketServer(ServerIP, ServerPort);

            client.MV_onMess += MV_onClient;
            //server.MV_onMess += MV_onServer;

            client.StartClient();
            //server.StartListen();
        }
        
        public void DisConnect()
        {
            if (client != null) client.StopClient();
            if (server != null) server.StopListen();

            Growl.Clear();
        }
        
        /// <summary>
        /// 客户端收到一条信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MV_onClient(object sender, string e)
        {
            //将信息打印至前台
            string str = "[";
            str += DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString();
            str += "]\n";
            str += ">>>客户端接收：" + e;
            str += "\n";
            WriteToChatBox(str);

            //将信息转发至服务端
            server.SocketSend(Encoding.Default.GetBytes(e));
        }

        /// <summary>
        /// 服务端收到一条信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MV_onServer(object sender, string e)
        {
            //将信息打印至前台
            string str = "[";
            str += DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString();
            str += "]\n";
            str += "<<<服务端接收：" + e;
            str += "\n";
            WriteToChatBox(str);

            //将信息转发至客户端
            client.SocketSend(Encoding.Default.GetBytes(e));
        }

        void WriteToChatBox(string str)
        {
            output.Enqueue(str);
            if (output.Count >= 20) output.Dequeue();

            Console = "";
            foreach (var item in output)
            {
                Console += item;
            }
        }

        //public void WriteToChatBox(string str, HandyControl.Data.ChatRoleType chatRoleType)
        //{
        //    ChatBubble chatBubble = new ChatBubble() { Role = chatRoleType, Content = str };
        //}
    }
}
