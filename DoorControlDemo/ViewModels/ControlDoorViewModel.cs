/*using DoorControlDemo.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Windows;
using System.Windows.Input;

namespace DoorControlDemo.ViewModels
{
    public class ControlDoorViewModel : ViewModelBase
    {
        public ILoginService _loginService;

        public ControlDoorViewModel(ILoginService loginService)
        {
            _loginService = loginService ?? throw new ArgumentNullException(nameof(loginService));
            // Other initialization code
            // Initialize commands
            OpenDoorCommand = new RelayCommand(OpenDoor, CanOperateDoor);
            CloseDoorCommand = new RelayCommand(CloseDoor, CanOperateDoor);
            StayOpenCommand = new RelayCommand(StayOpen, CanOperateDoor);
            StayCloseCommand = new RelayCommand(StayClose, CanOperateDoor);
        }

        // Commands
        public ICommand OpenDoorCommand { get; private set; }
        public ICommand CloseDoorCommand { get; private set; }
        public ICommand StayOpenCommand { get; private set; }
        public ICommand StayCloseCommand { get; private set; }
        private bool CanOperateDoor()
        {
            // Check if the user is logged in
            return _loginService.IsLoggedIn;
        }

        private void OpenDoor()
        {
            // Check if the user is logged in before opening the door
            if (_loginService.IsLoggedIn)
            {
                if (CHCNetSDK.NET_DVR_ControlGateway(LoginDeviceViewModel.m_UserID, 1, 1))
                {
                    MessageBox.Show("NET_DVR_ControlGateway: open door succeed");
                }
                else
                {
                    MessageBox.Show("NET_DVR_ControlGateway: open door failed error:" + CHCNetSDK.NET_DVR_GetLastError());
                }
            }
            else
            {
                MessageBox.Show("You must log in first.");
            }
        }

        private void CloseDoor()
        {
            // Check if the user is logged in before closing the door
            if (_loginService.IsLoggedIn)
            {
                if (CHCNetSDK.NET_DVR_ControlGateway(LoginDeviceViewModel.m_UserID, 1, 0))
                {
                    MessageBox.Show("NET_DVR_ControlGateway: close door succeed");
                }
                else
                {
                    MessageBox.Show("NET_DVR_ControlGateway: close door failed error:" + CHCNetSDK.NET_DVR_GetLastError());
                }
            }
            else
            {
                MessageBox.Show("You must log in first.");
            }
        }

        private void StayOpen()
        {
            // Check if the user is logged in before setting the door to stay open
            if (_loginService.IsLoggedIn)
            {
                if (CHCNetSDK.NET_DVR_ControlGateway(LoginDeviceViewModel.m_UserID, 1, 2))
                {
                    MessageBox.Show("NET_DVR_ControlGateway: stay open door succeed");
                }
                else
                {
                    MessageBox.Show("NET_DVR_ControlGateway: stay open door failed error:" + CHCNetSDK.NET_DVR_GetLastError());
                }
            }
            else
            {
                MessageBox.Show("You must log in first.");
            }
        }

        private void StayClose()
        {
            // Check if the user is logged in before setting the door to stay closed
            if (_loginService.IsLoggedIn)
            {
                if (CHCNetSDK.NET_DVR_ControlGateway(LoginDeviceViewModel.m_UserID, 1, 3))
                {
                    MessageBox.Show("NET_DVR_ControlGateway: stay close door succeed");
                }
                else
                {
                    MessageBox.Show("NET_DVR_ControlGateway: stay close door failed error:" + CHCNetSDK.NET_DVR_GetLastError());
                }
            }
            else
            {
                MessageBox.Show("You must log in first.");
            }
        }

        // Add other methods as needed
    }
}
*/