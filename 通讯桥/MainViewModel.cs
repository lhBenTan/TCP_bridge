using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace 通讯桥
{
    public class MainViewModel:ViewModelBase
    {
        public ObservableCollection<CommunicationViewModel> communications
        {
            get => GetProperty(() => communications);
            set => SetProperty(() => communications, value);
        }

        public int SelectIndex
        {
            get => GetProperty(() => SelectIndex);
            set => SetProperty(() => SelectIndex, value);
        }

        public WindowState windowState
        {
            get => GetProperty(() => windowState);
            set => SetProperty(() => windowState, value);
        }
        
        public MainViewModel()
        {
            SelectIndex = 0;
            windowState = WindowState.Normal;
            communications = new ObservableCollection<CommunicationViewModel>();

            for (int i = 0; i < 4; i++)
            {
                communications.Add(new CommunicationViewModel());
            }
        }

        [AsyncCommand]
        public void ConnectCommand(object obj)
        {
            for (int i = 0; i < SelectIndex + 1; i++)
            {
                communications[i].Connect();
            }
        }

        [AsyncCommand]
        public void DisconnectCommand(object obj)
        {
            for (int i = 0; i < SelectIndex + 1; i++)
            {
                communications[i].DisConnect();
            }
        }

        [AsyncCommand]
        public void MinimizedCommand(object obj)
        {
            windowState = WindowState.Minimized;
        }

    }
}
