using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace 通讯桥
{
    /// <summary>
    /// CommunicationCtrl.xaml 的交互逻辑
    /// </summary>
    public partial class CommunicationCtrl : UserControl
    {
        public CommunicationCtrl()
        {
            InitializeComponent();
        }

        //public void WriteToCB(ChatBubble msg)
        //{
        //    ChatBox.Children.Add(msg);
        //    if (ChatBox.Children.Count >= 20)
        //    {
        //        ChatBox.Children.RemoveAt(0);
        //    }
        //}
    }
}
