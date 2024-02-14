/*using DoorControlDemo.Data;
using DoorControlDemo.Models;
using GalaSoft.MvvmLight.Command;
using System;
using System.Data.Entity;
using System.Windows;
using System.Windows.Input;

namespace DoorControlDemo.ViewModels
{
    public class LoginDeviceViewModel : ViewModelBase
    {


        public static int m_UserID = 1;

        private readonly ILoginService _loginService;
        private readonly MessageBoxDisplay _messageBoxDisplay = new MessageBoxDisplay();
        private readonly DoorControlDbContext _dbContext;

        public LoginDeviceViewModel(DoorControlDbContext dbContext, ILoginService loginService)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(DbContext));
            _loginService = loginService ?? throw new ArgumentNullException(nameof(loginService));
            LoginCommand = new RelayCommand(LoginDeviceButtonClick);
            CreateMainCommand = new RelayCommand(() => NavigateToWindow(new MainWindow()));
        }

        public ICommand LoginCommand { get; set; }
        public ICommand CreateMainCommand { get; set; }

        private string _deviceAddress;
        public string DeviceAddress
        {
            get => _deviceAddress;
            set
            {
                _deviceAddress = value;
                OnPropertyChanged(nameof(DeviceAddress));
            }
        }

        private string _userName;
        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                OnPropertyChanged(nameof(UserName));
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }


        private string _port;
        public string Port
        {
            get => _port;
            set
            {
                _port = value;
                OnPropertyChanged(nameof(Port));
            }
        }

        private void LoginDeviceButtonClick()
        {
            if (string.IsNullOrEmpty(DeviceAddress) || string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
            {
                _messageBoxDisplay.DisplayMessage("Device address, username, and password are required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Prepare the login info
            CHCNetSDK.NET_DVR_USER_LOGIN_INFO loginInfo = new CHCNetSDK.NET_DVR_USER_LOGIN_INFO();
            CHCNetSDK.NET_DVR_DEVICEINFO_V40 deviceInfo = new CHCNetSDK.NET_DVR_DEVICEINFO_V40();
            deviceInfo.struDeviceV30.sSerialNumber = new byte[CHCNetSDK.SERIALNO_LEN];
            loginInfo.sDeviceAddress = DeviceAddress;
            loginInfo.sUserName = UserName;
            loginInfo.sPassword = Password;

            ushort.TryParse(Port, out loginInfo.wPort);

            // Perform login
            int userID = CHCNetSDK.NET_DVR_Login_V40(ref loginInfo, ref deviceInfo);
            if (userID < 0)
            {
                uint errorCode = CHCNetSDK.NET_DVR_GetLastError();

                if (errorCode == CHCNetSDK.NET_DVR_PASSWORD_ERROR)
                {
                    MessageBox.Show("user name or password error!");
                    if (1 == deviceInfo.bySupportLock)
                    {
                        string strTemp1 = string.Format("Left {0} try opportunity", deviceInfo.byRetryLoginTime);
                        MessageBox.Show(strTemp1);
                    }
                }

                else if (errorCode == CHCNetSDK.NET_DVR_USER_LOCKED)
                {
                    if (1 == deviceInfo.bySupportLock)
                    {
                        string strTemp1 = string.Format("user is locked, the remaining lock time is {0}", deviceInfo.dwSurplusLockTime);
                        MessageBox.Show(strTemp1);
                    }
                }
                else
                {
                    MessageBox.Show("net error or dvr is busy!");
                }
            }
            else
            {
                // Login successful
                MessageBox.Show("Login Successful");
                _loginService.IsLoggedIn = true;

            }
        }
    }
}
*/

using DoorControlDemo.Data;
using DoorControlDemo.Models;
using GalaSoft.MvvmLight.Command;
using System;
using System.Data.Entity;
using System.Windows;
using System.Windows.Input;

namespace DoorControlDemo.ViewModels
{
    public class LoginDeviceViewModel : ViewModelBase
    {
        public static int m_UserID = 1;

        public  ILoginService _loginService;
        private readonly MessageBoxDisplay _messageBoxDisplay = new MessageBoxDisplay();
        private readonly DoorControlDbContext _dbContext;

        public LoginDeviceViewModel(DoorControlDbContext dbContext, ILoginService loginService)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(DbContext));
            LoginCommand = new RelayCommand(LoginDeviceButtonClick);
            _loginService = loginService ?? throw new ArgumentNullException(nameof(loginService));
            CreateMainCommand = new RelayCommand(() => NavigateToWindow(new MainWindow()));

            // Initialize door control commands
            OpenDoorCommand = new RelayCommand(OpenDoor);
            CloseDoorCommand = new RelayCommand(CloseDoor);
            StayOpenCommand = new RelayCommand(StayOpen);
            StayCloseCommand = new RelayCommand(StayClose);
        }

        public ICommand LoginCommand { get; set; }
        public ICommand CreateMainCommand { get; set; }

        // Door control commands
        public ICommand OpenDoorCommand { get; private set; }
        public ICommand CloseDoorCommand { get; private set; }
        public ICommand StayOpenCommand { get; private set; }
        public ICommand StayCloseCommand { get; private set; }

        private string _deviceAddress;
        public string DeviceAddress
        {
            get => _deviceAddress;
            set
            {
                _deviceAddress = value;
                OnPropertyChanged(nameof(DeviceAddress));
            }
        }

        private string _userName;
        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                OnPropertyChanged(nameof(UserName));
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        private string _port;
        public string Port
        {
            get => _port;
            set
            {
                _port = value;
                OnPropertyChanged(nameof(Port));
            }
        }

        private void LoginDeviceButtonClick()
        {
            if (string.IsNullOrEmpty(DeviceAddress) || string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
            {
                _messageBoxDisplay.DisplayMessage("Device address, username, and password are required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Prepare the login info
            CHCNetSDK.NET_DVR_USER_LOGIN_INFO loginInfo = new CHCNetSDK.NET_DVR_USER_LOGIN_INFO();
            CHCNetSDK.NET_DVR_DEVICEINFO_V40 deviceInfo = new CHCNetSDK.NET_DVR_DEVICEINFO_V40();
            deviceInfo.struDeviceV30.sSerialNumber = new byte[CHCNetSDK.SERIALNO_LEN];
            loginInfo.sDeviceAddress = DeviceAddress;
            loginInfo.sUserName = UserName;
            loginInfo.sPassword = Password;

            ushort.TryParse(Port, out loginInfo.wPort);

            int lUserID = 1;
            lUserID = CHCNetSDK.NET_DVR_Login_V40(ref loginInfo, ref deviceInfo);
            if (lUserID < 0)
            {
                uint errorCode = CHCNetSDK.NET_DVR_GetLastError();

                if (errorCode == CHCNetSDK.NET_DVR_PASSWORD_ERROR)
                {
                    MessageBox.Show("user name or password error!");
                    if (1 == deviceInfo.bySupportLock)
                    {
                        string strTemp1 = string.Format("Left {0} try opportunity", deviceInfo.byRetryLoginTime);
                        MessageBox.Show(strTemp1);
                    }
                }
                else if (errorCode == CHCNetSDK.NET_DVR_USER_LOCKED)
                {
                    if (1 == deviceInfo.bySupportLock)
                    {
                        string strTemp1 = string.Format("user is locked, the remaining lock time is {0}", deviceInfo.dwSurplusLockTime);
                        MessageBox.Show(strTemp1);
                    }
                }
                else
                {
                    MessageBox.Show("net error or dvr is busy!");
                }
            }
            else
            {
                // Login successful
                m_UserID = lUserID;
                MessageBox.Show("Login Successful");
                _loginService.IsLoggedIn = true;
            }
        }

        // Method to check if door operations can be performed
        private bool CanOperateDoor()
        {
            // Check if the user is logged in
            return _loginService.IsLoggedIn;
        }

        // Methods for door control
        private void OpenDoor()
        {
            // Check if the user is logged in before opening the door
            if (_loginService.IsLoggedIn)
            {
                if (CHCNetSDK.NET_DVR_ControlGateway(m_UserID, 1, 1))
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
                if (CHCNetSDK.NET_DVR_ControlGateway(m_UserID, 1, 0))
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
                if (CHCNetSDK.NET_DVR_ControlGateway(m_UserID, 1, 2))
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
    }
}
