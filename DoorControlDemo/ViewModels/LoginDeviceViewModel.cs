using DoorControlDemo.Data;
using DoorControlDemo.Models;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using static DoorControlDemo.CHCNetSDK;

namespace DoorControlDemo.ViewModels
{
    public class LoginDeviceViewModel : ViewModelBase
    {
        public static int m_UserID = 1;
        private RemoteConfigCallback g_fGetGatewayCardCallback = null;

        public NET_DVR_SINGLE_PLAN_SEGMENT[][] struPlanCfg;

        public Int32 m_lGetCardCfgHandle = -1;
        public Int32 m_lSetCardCfgHandle = -1;
        public Int32 m_lDelCardCfgHandle = -1;
        public IntPtr hwnd;



        public ILoginService _loginService;
        private readonly MessageBoxDisplay _messageBoxDisplay = new MessageBoxDisplay();
        private readonly DoorControlDbContext _dbContext;

        public LoginDeviceViewModel(DoorControlDbContext dbContext, ILoginService loginService)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(DbContext));
            LoginCommand = new RelayCommand(LoginDeviceButtonClick);
            SetUserCommand = new RelayCommand(SetUserClick);
            DeleteUserCommand = new RelayCommand(DeleteUserClick);
            _loginService = loginService ?? throw new ArgumentNullException(nameof(loginService));
            CreateMainCommand = new RelayCommand(() => NavigateToWindow(new MainWindow()));

            // Initialize door control commands
            OpenDoorCommand = new RelayCommand(OpenDoor);
            CloseDoorCommand = new RelayCommand(CloseDoor);
            StayOpenCommand = new RelayCommand(StayOpen);
            StayCloseCommand = new RelayCommand(StayClose);
        }

        public ICommand LoginCommand { get; set; }

        public ICommand SetUserCommand { get; set; }
        public ICommand DeleteUserCommand { get; set; }
        public ICommand CreateMainCommand { get; set; }

        // Door control commands
        public ICommand OpenDoorCommand { get; private set; }
        public ICommand CloseDoorCommand { get; private set; }
        public ICommand StayOpenCommand { get; private set; }
        public ICommand StayCloseCommand { get; private set; }

        //LOGIN

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

        // Set User
        private string _setUserName;
        public string SetUserName
        {
            get => _setUserName;
            set
            {
                _setUserName = value;
                OnPropertyChanged(nameof(SetUserName));
            }
        }

        private string _userId;
        public string UserId
        {
            get => _userId;
            set
            {
                _userId = value;
                OnPropertyChanged(nameof(UserId));
            }
        }

        
        private string _cardNumber;
        public string CardNumber
        {
            get => _cardNumber;
            set
            {
                _cardNumber = value;
                OnPropertyChanged(nameof(CardNumber));
            }
        }


        private string _CardDeletionNo;
        public string CardDeletionNo
        {
            get => _CardDeletionNo;
            set
            {
                _CardDeletionNo = value;
                OnPropertyChanged(nameof(CardDeletionNo));
            }
        }


        private void LoginDeviceButtonClick()
        {
            if (string.IsNullOrEmpty(DeviceAddress) || string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
            {
                _messageBoxDisplay.DisplayMessage("Device address, username, and password are required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                loginDevice();
                MessageBox.Show("Login Successful");

            }

        }

        private void SetUserClick()
        {
            if (string.IsNullOrEmpty(DeviceAddress) || string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
            {
                _messageBoxDisplay.DisplayMessage("Device address, username, and password are required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                loginDevice();
                uploadCardNo(_userId, _setUserName, _cardNumber);

            }
        }

        private void DeleteUserClick()
        {
            try
            {
                getUserByCardNo(CardDeletionNo);
            }
            catch (Exception)
            {

                throw;
            }
            deleteCardNo(_CardDeletionNo);
        }


        // works, but redundant. unused in program
        public void getUserByCardNo(string cardNo)
        {
            string doorCardNo;
            string doorEmployeeNo;
            string doorUserName;

            if (m_lGetCardCfgHandle != -1)
            {
                if (CHCNetSDK.NET_DVR_StopRemoteConfig(m_lGetCardCfgHandle))
                {
                    m_lGetCardCfgHandle = -1;
                }
            }
            CHCNetSDK.NET_DVR_CARD_COND struCond = new CHCNetSDK.NET_DVR_CARD_COND();
            struCond.Init();
            struCond.dwSize = (uint)Marshal.SizeOf(struCond);
            struCond.dwCardNum = 1;
            IntPtr ptrStruCond = Marshal.AllocHGlobal((int)struCond.dwSize);
            Marshal.StructureToPtr(struCond, ptrStruCond, false);

            CHCNetSDK.NET_DVR_CARD_RECORD struData = new CHCNetSDK.NET_DVR_CARD_RECORD();
            struData.Init();
            struData.dwSize = (uint)Marshal.SizeOf(struData);
            byte[] byTempCardNo = new byte[CHCNetSDK.ACS_CARD_NO_LEN];
            byTempCardNo = System.Text.Encoding.UTF8.GetBytes(cardNo);
            for (int i = 0; i < byTempCardNo.Length; i++)
            {
                struData.byCardNo[i] = byTempCardNo[i];
            }
            IntPtr ptrStruData = Marshal.AllocHGlobal((int)struData.dwSize);
            Marshal.StructureToPtr(struData, ptrStruData, false);

            CHCNetSDK.NET_DVR_CARD_SEND_DATA struSendData = new CHCNetSDK.NET_DVR_CARD_SEND_DATA();
            struSendData.Init();
            struSendData.dwSize = (uint)Marshal.SizeOf(struSendData);
            for (int i = 0; i < byTempCardNo.Length; i++)
            {
                struSendData.byCardNo[i] = byTempCardNo[i];
            }
            IntPtr ptrStruSendData = Marshal.AllocHGlobal((int)struSendData.dwSize);
            Marshal.StructureToPtr(struSendData, ptrStruSendData, false);

            m_lGetCardCfgHandle = CHCNetSDK.NET_DVR_StartRemoteConfig(m_UserID, CHCNetSDK.NET_DVR_GET_CARD, ptrStruCond, (int)struCond.dwSize, null, hwnd);
            if (m_lGetCardCfgHandle < 0)
            {
                MessageBox.Show("NET_DVR_GET_CARD error: " + CHCNetSDK.NET_DVR_GetLastError());
                Marshal.FreeHGlobal(ptrStruCond);
                return;
            }
            else
            {
                int dwState = (int)CHCNetSDK.NET_SDK_SENDWITHRECV_STATUS.NET_SDK_CONFIG_STATUS_SUCCESS;
                uint dwReturned = 0;
                while (true)
                {
                    dwState = CHCNetSDK.NET_DVR_SendWithRecvRemoteConfig(m_lGetCardCfgHandle, ptrStruSendData, struSendData.dwSize, ptrStruData, struData.dwSize, ref dwReturned);
                    struData = (CHCNetSDK.NET_DVR_CARD_RECORD)Marshal.PtrToStructure(ptrStruData, typeof(CHCNetSDK.NET_DVR_CARD_RECORD));
                    if (dwState == (int)CHCNetSDK.NET_SDK_SENDWITHRECV_STATUS.NET_SDK_CONFIG_STATUS_NEEDWAIT)
                    {
                        Thread.Sleep(10);
                        continue;
                    }
                    else if (dwState == (int)CHCNetSDK.NET_SDK_SENDWITHRECV_STATUS.NET_SDK_CONFIG_STATUS_FAILED)
                    {
                        MessageBox.Show("NET_DVR_GET_CARD fail error: " + CHCNetSDK.NET_DVR_GetLastError());
                    }
                    else if (dwState == (int)CHCNetSDK.NET_SDK_SENDWITHRECV_STATUS.NET_SDK_CONFIG_STATUS_SUCCESS)
                    {
                        doorCardNo = Encoding.UTF8.GetString(struData.byCardNo).TrimEnd('\0');
                        //textBoxCardRightPlan.Text = struData.wCardRightPlan[0].ToString();
                        doorEmployeeNo = struData.byName.ToString();
                        doorUserName = System.Text.Encoding.Default.GetString(struData.byName);
                        MessageBox.Show(doorEmployeeNo);
                        MessageBox.Show("NET_DVR_GET_CARD success: " + cardNo.ToString());
                    }
                    else if (dwState == (int)CHCNetSDK.NET_SDK_SENDWITHRECV_STATUS.NET_SDK_CONFIG_STATUS_FINISH)
                    {
                        MessageBox.Show("NET_DVR_GET_CARD finish");
                        break;
                    }
                    else if (dwState == (int)CHCNetSDK.NET_SDK_SENDWITHRECV_STATUS.NET_SDK_CONFIG_STATUS_EXCEPTION)
                    {
                        MessageBox.Show("NET_DVR_GET_CARD exception error: " + CHCNetSDK.NET_DVR_GetLastError());
                        break;
                    }
                    else
                    {
                        MessageBox.Show("unknown status error: " + CHCNetSDK.NET_DVR_GetLastError());
                        break;
                    }
                }
            }
            CHCNetSDK.NET_DVR_StopRemoteConfig(m_lGetCardCfgHandle);
            m_lGetCardCfgHandle = -1;
            Marshal.FreeHGlobal(ptrStruSendData);
            Marshal.FreeHGlobal(ptrStruData);



        }
        
        public void deleteCardNo(string cardNo)
        {
            if (m_lDelCardCfgHandle != -1)
            {
                if (CHCNetSDK.NET_DVR_StopRemoteConfig(m_lDelCardCfgHandle))
                {
                    m_lDelCardCfgHandle = -1;
                }
            }
            CHCNetSDK.NET_DVR_CARD_COND struCond = new CHCNetSDK.NET_DVR_CARD_COND();
            struCond.Init();
            struCond.dwSize = (uint)Marshal.SizeOf(struCond);
            struCond.dwCardNum = 1;
            IntPtr ptrStruCond = Marshal.AllocHGlobal((int)struCond.dwSize);
            Marshal.StructureToPtr(struCond, ptrStruCond, false);

            CHCNetSDK.NET_DVR_CARD_SEND_DATA struSendData = new CHCNetSDK.NET_DVR_CARD_SEND_DATA();
            struSendData.Init();
            struSendData.dwSize = (uint)Marshal.SizeOf(struSendData);
            byte[] byTempCardNo = new byte[CHCNetSDK.ACS_CARD_NO_LEN];
            byTempCardNo = System.Text.Encoding.UTF8.GetBytes(cardNo);
            for (int i = 0; i < byTempCardNo.Length; i++)
            {
                struSendData.byCardNo[i] = byTempCardNo[i];
            }
            IntPtr ptrStruSendData = Marshal.AllocHGlobal((int)struSendData.dwSize);
            Marshal.StructureToPtr(struSendData, ptrStruSendData, false);

            CHCNetSDK.NET_DVR_CARD_STATUS struStatus = new CHCNetSDK.NET_DVR_CARD_STATUS();
            struStatus.Init();
            struStatus.dwSize = (uint)Marshal.SizeOf(struStatus);
            IntPtr ptrdwState = Marshal.AllocHGlobal((int)struStatus.dwSize);
            Marshal.StructureToPtr(struStatus, ptrdwState, false);

            m_lGetCardCfgHandle = CHCNetSDK.NET_DVR_StartRemoteConfig(m_UserID, CHCNetSDK.NET_DVR_DEL_CARD, ptrStruCond, (int)struCond.dwSize, null, hwnd);
            if (m_lGetCardCfgHandle < 0)
            {
                MessageBox.Show("NET_DVR_DEL_CARD error:" + CHCNetSDK.NET_DVR_GetLastError());
                Marshal.FreeHGlobal(ptrStruCond);
                return;
            }
            else
            {
                int dwState = (int)CHCNetSDK.NET_SDK_SENDWITHRECV_STATUS.NET_SDK_CONFIG_STATUS_SUCCESS;
                uint dwReturned = 0;
                while (true)
                {
                    dwState = CHCNetSDK.NET_DVR_SendWithRecvRemoteConfig(m_lGetCardCfgHandle, ptrStruSendData, struSendData.dwSize, ptrdwState, struStatus.dwSize, ref dwReturned);
                    if (dwState == (int)CHCNetSDK.NET_SDK_SENDWITHRECV_STATUS.NET_SDK_CONFIG_STATUS_NEEDWAIT)
                    {
                        Thread.Sleep(10);
                        continue;
                    }
                    else if (dwState == (int)CHCNetSDK.NET_SDK_SENDWITHRECV_STATUS.NET_SDK_CONFIG_STATUS_FAILED)
                    {
                        MessageBox.Show("NET_DVR_DEL_CARD fail error: " + CHCNetSDK.NET_DVR_GetLastError());
                    }
                    else if (dwState == (int)CHCNetSDK.NET_SDK_SENDWITHRECV_STATUS.NET_SDK_CONFIG_STATUS_SUCCESS)
                    {
                        MessageBox.Show("NET_DVR_DEL_CARD success");
                    }
                    else if (dwState == (int)CHCNetSDK.NET_SDK_SENDWITHRECV_STATUS.NET_SDK_CONFIG_STATUS_FINISH)
                    {
                        MessageBox.Show("NET_DVR_DEL_CARD finish");
                        break;
                    }
                    else if (dwState == (int)CHCNetSDK.NET_SDK_SENDWITHRECV_STATUS.NET_SDK_CONFIG_STATUS_EXCEPTION)
                    {
                        MessageBox.Show("NET_DVR_DEL_CARD exception error: " + CHCNetSDK.NET_DVR_GetLastError());
                        break;
                    }
                    else
                    {
                        MessageBox.Show("unknown status error: " + CHCNetSDK.NET_DVR_GetLastError());
                        break;
                    }
                }
            }
            CHCNetSDK.NET_DVR_StopRemoteConfig(m_lDelCardCfgHandle);
            m_lDelCardCfgHandle = -1;
            Marshal.FreeHGlobal(ptrStruSendData);
            Marshal.FreeHGlobal(ptrdwState);
        }

        // Create and send user to device
        // RUNS, after creation, IVMS must import data sent from the device.
        public void uploadCardNo(string employeeNo, string userName, string cardNo)
        {
            if (m_lSetCardCfgHandle != -1)
            {
                if (CHCNetSDK.NET_DVR_StopRemoteConfig(m_lSetCardCfgHandle))
                {
                    m_lSetCardCfgHandle = -1;
                }
            }

            CHCNetSDK.NET_DVR_CARD_COND struCond = new CHCNetSDK.NET_DVR_CARD_COND();
            struCond.Init();
            struCond.dwSize = (uint)Marshal.SizeOf(struCond);
            struCond.dwCardNum = 1;
            IntPtr ptrStruCond = Marshal.AllocHGlobal((int)struCond.dwSize);
            Marshal.StructureToPtr(struCond, ptrStruCond, false);

            m_lSetCardCfgHandle = CHCNetSDK.NET_DVR_StartRemoteConfig(m_UserID, CHCNetSDK.NET_DVR_SET_CARD, ptrStruCond, (int)struCond.dwSize, null, IntPtr.Zero);
            if (m_lSetCardCfgHandle < 0)
            {
                MessageBox.Show("NET_DVR_SET_CARD error:" + CHCNetSDK.NET_DVR_GetLastError());
                Marshal.FreeHGlobal(ptrStruCond);
                return;
            }
            else
            {
                SendCardData(employeeNo, userName, cardNo);
                Marshal.FreeHGlobal(ptrStruCond);
            }
        }

        private void SendCardData(string employeeNo, string userName, string cardNo)
        {

            CHCNetSDK.NET_DVR_CARD_RECORD struData = new CHCNetSDK.NET_DVR_CARD_RECORD();
            struData.Init();
            struData.dwSize = (uint)Marshal.SizeOf(struData);
            struData.byCardType = 1;
            byte[] byTempCardNo = new byte[CHCNetSDK.ACS_CARD_NO_LEN];
            byTempCardNo = System.Text.Encoding.UTF8.GetBytes(cardNo);
            for (int i = 0; i < byTempCardNo.Length; i++)
            {
                struData.byCardNo[i] = byTempCardNo[i];
            }
            ushort.TryParse("1", out struData.wCardRightPlan[0]);
            uint.TryParse(employeeNo, out struData.dwEmployeeNo);
            byte[] byTempName = new byte[CHCNetSDK.NAME_LEN];
            byTempName = System.Text.Encoding.Default.GetBytes(userName);
            for (int i = 0; i < byTempName.Length; i++)
            {
                struData.byName[i] = byTempName[i];
            }
            struData.struValid.byEnable = 1;
            struData.struValid.struBeginTime.wYear = 2000;
            struData.struValid.struBeginTime.byMonth = 1;
            struData.struValid.struBeginTime.byDay = 1;
            struData.struValid.struBeginTime.byHour = 11;
            struData.struValid.struBeginTime.byMinute = 11;
            struData.struValid.struBeginTime.bySecond = 11;
            struData.struValid.struEndTime.wYear = 2030;
            struData.struValid.struEndTime.byMonth = 1;
            struData.struValid.struEndTime.byDay = 1;
            struData.struValid.struEndTime.byHour = 11;
            struData.struValid.struEndTime.byMinute = 11;
            struData.struValid.struEndTime.bySecond = 11;
            struData.byDoorRight[0] = 1;
            struData.wCardRightPlan[0] = 1;
            IntPtr ptrStruData = Marshal.AllocHGlobal((int)struData.dwSize);
            Marshal.StructureToPtr(struData, ptrStruData, false);

            CHCNetSDK.NET_DVR_CARD_STATUS struStatus = new CHCNetSDK.NET_DVR_CARD_STATUS();
            struStatus.Init();
            struStatus.dwSize = (uint)Marshal.SizeOf(struStatus);
            IntPtr ptrdwState = Marshal.AllocHGlobal((int)struStatus.dwSize);
            Marshal.StructureToPtr(struStatus, ptrdwState, false);

            int dwState = (int)CHCNetSDK.NET_SDK_SENDWITHRECV_STATUS.NET_SDK_CONFIG_STATUS_SUCCESS;
            uint dwReturned = 0;
            while (true)
            {
                dwState = CHCNetSDK.NET_DVR_SendWithRecvRemoteConfig(m_lSetCardCfgHandle, ptrStruData, struData.dwSize, ptrdwState, struStatus.dwSize, ref dwReturned);
                struStatus = (CHCNetSDK.NET_DVR_CARD_STATUS)Marshal.PtrToStructure(ptrdwState, typeof(CHCNetSDK.NET_DVR_CARD_STATUS));
                if (dwState == (int)CHCNetSDK.NET_SDK_SENDWITHRECV_STATUS.NET_SDK_CONFIG_STATUS_NEEDWAIT)
                {
                    Thread.Sleep(10);
                    continue;
                }
                else if (dwState == (int)CHCNetSDK.NET_SDK_SENDWITHRECV_STATUS.NET_SDK_CONFIG_STATUS_FAILED)
                {
                    MessageBox.Show("NET_DVR_SET_CARD fail error: " + CHCNetSDK.NET_DVR_GetLastError());
                }
                else if (dwState == (int)CHCNetSDK.NET_SDK_SENDWITHRECV_STATUS.NET_SDK_CONFIG_STATUS_SUCCESS)
                {
                    if (struStatus.dwErrorCode != 0)
                    {
                        MessageBox.Show("NET_DVR_SET_CARD success but errorCode:" + struStatus.dwErrorCode);
                    }
                    else
                    {
                        MessageBox.Show("NET_DVR_SET_CARD success");
                    }
                }
                else if (dwState == (int)CHCNetSDK.NET_SDK_SENDWITHRECV_STATUS.NET_SDK_CONFIG_STATUS_FINISH)
                {
                    MessageBox.Show("NET_DVR_SET_CARD finish");
                    break;
                }
                else if (dwState == (int)CHCNetSDK.NET_SDK_SENDWITHRECV_STATUS.NET_SDK_CONFIG_STATUS_EXCEPTION)
                {
                    MessageBox.Show("NET_DVR_SET_CARD exception error: " + CHCNetSDK.NET_DVR_GetLastError());
                    break;
                }
                else
                {
                    MessageBox.Show("unknown status error: " + CHCNetSDK.NET_DVR_GetLastError());
                    break;
                }
            }
            CHCNetSDK.NET_DVR_StopRemoteConfig(m_lSetCardCfgHandle);
            m_lSetCardCfgHandle = -1;
            Marshal.FreeHGlobal(ptrStruData);
            Marshal.FreeHGlobal(ptrdwState);
            return;
        }

        public void loginDevice()
        {
            bool isLoggedIn = false;

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
                
                _loginService.IsLoggedIn = true;
                isLoggedIn = true;

                // Call the method to set reader plan teamplate
                // get the readerplan with the template
                // and then set the reader plan to the cuser

                // User should not be able to authenticate with card outside the specified timeframe in the tempalte
                // However, even though code runs correctly, authentication remains.

                //Calling the method here for testing purposes, no UI made yet

                /*SetVerifyWeekPlanAndCardReaderPlan(m_UserID);*/
            }
        }

        NET_DVR_WEEK_PLAN_CFG weekPlanCfg = new NET_DVR_WEEK_PLAN_CFG();

        long m_ISetCardCFfgHandle;
        long m_IGetCardCFfgHandle;
        string m_csCardPassword;
        string m_csCardNo;
        bool bGetCardCfgFinish = false;
        bool bSetCardCfgFinish = false;


        // Structure for holiday plan configuration
        public struct NET_DVR_HOLIDAY_PLAN_CFG
        {
            public uint dwSize;
            public byte byEnable; // Whether to enable: 1-no, 0-yes
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] byRes1; // Reserved, set to 0
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public NET_DVR_SINGLE_PLAN_SEGMENT[] struPlanCfg; // Holiday plan parameters
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] byRes2; // Reserved, set to 0

            // Initialize the structure
            public void Init()
            {
                dwSize = (uint)Marshal.SizeOf(typeof(NET_DVR_HOLIDAY_PLAN_CFG));
                byRes1 = new byte[3];
                byRes2 = new byte[16];
                struPlanCfg = new NET_DVR_SINGLE_PLAN_SEGMENT[32];
                foreach (var segment in struPlanCfg)
                {
                    segment.Init();
                }
            }
        }


        CHCNetSDK.NET_DVR_SINGLE_PLAN_SEGMENT[] plan;
        const int NET_DVR_GET_WEEK_PLAN_CFG = 2124; //get door status week plan config 
       
        const int MAX_DAYS = 7; // Assuming there are 7 days in a week

        //Code below runs succesfully but device does not react to authentication
        public void SetVerifyWeekPlanAndCardReaderPlan(int lUserID)
        {
            //  Week plan configuration
            CHCNetSDK.NET_DVR_WEEK_PLAN_CFG weekPlan = new CHCNetSDK.NET_DVR_WEEK_PLAN_CFG();
            weekPlan.dwSize = (uint)Marshal.SizeOf(weekPlan);
            weekPlan.byEnable = 1; // Enable week schedule

            // Single plan segment for each day
            CHCNetSDK.NET_DVR_SINGLE_PLAN_SEGMENT singlePlanSegment = new CHCNetSDK.NET_DVR_SINGLE_PLAN_SEGMENT();
            singlePlanSegment.byEnable = 1;
            singlePlanSegment.byVerifyMode = 4; // Authentication mode: card or password
            singlePlanSegment.struTimeSegment.struBeginTime.byHour = 7; // Start time
            singlePlanSegment.struTimeSegment.struBeginTime.byMinute = 0;
            singlePlanSegment.struTimeSegment.struBeginTime.bySecond = 0;
            singlePlanSegment.struTimeSegment.struEndTime.byHour = 10; // End time
            singlePlanSegment.struTimeSegment.struEndTime.byMinute = 59;
            singlePlanSegment.struTimeSegment.struEndTime.bySecond = 59;

            // Initialize the array for plan configurations
            weekPlan.struPlanCfg = new CHCNetSDK.NET_DVR_SINGLE_PLAN_SEGMENT[CHCNetSDK.MAX_DAYS * CHCNetSDK.MAX_TIMESEGMENT_V30];

            // Copy the single plan segment to each day
            for (int i = 0; i < CHCNetSDK.MAX_DAYS; i++)
            {
                weekPlan.struPlanCfg[i].Init();
                Array.Copy(new CHCNetSDK.NET_DVR_SINGLE_PLAN_SEGMENT[] { singlePlanSegment }, 0, weekPlan.struPlanCfg, i * CHCNetSDK.MAX_TIMESEGMENT_V30, 1);
            }

            IntPtr ptrWeekPlanConfig = Marshal.AllocHGlobal(Marshal.SizeOf(weekPlan));
            Marshal.StructureToPtr(weekPlan, ptrWeekPlanConfig, false);

            // Set week plan configuration
            bool setWeekPlanSuccess = CHCNetSDK.NET_DVR_SetDVRConfig(lUserID, CHCNetSDK.NET_DVR_SET_VERIFY_WEEK_PLAN, 1,
                ptrWeekPlanConfig, (uint)Marshal.SizeOf(weekPlan));

            Marshal.FreeHGlobal(ptrWeekPlanConfig);

            if (!setWeekPlanSuccess)
            {
                MessageBox.Show($"Setting week schedule for card reader authentication mode failed, error: {CHCNetSDK.NET_DVR_GetLastError()}");
                CHCNetSDK.NET_DVR_Logout_V30(lUserID);
                CHCNetSDK.NET_DVR_Cleanup();
                return;
            }

            // Card reader plan configuration
            CHCNetSDK.NET_DVR_CARD_READER_PLAN cardReaderPlan = new CHCNetSDK.NET_DVR_CARD_READER_PLAN();
            cardReaderPlan.dwSize = (uint)Marshal.SizeOf(cardReaderPlan);
            cardReaderPlan.dwTemplateNo = 1;

            
            IntPtr ptrCardReaderPlanConfig = Marshal.AllocHGlobal(Marshal.SizeOf(cardReaderPlan));
            Marshal.StructureToPtr(cardReaderPlan, ptrCardReaderPlanConfig, false);

            // Set card reader plan configuration
            bool setCardReaderPlanSuccess = CHCNetSDK.NET_DVR_SetDVRConfig(lUserID, CHCNetSDK.NET_DVR_SET_CARD_READER_PLAN, 1,
                ptrCardReaderPlanConfig, (uint)Marshal.SizeOf(cardReaderPlan));

            Marshal.FreeHGlobal(ptrCardReaderPlanConfig);

            if (!setCardReaderPlanSuccess)
            {
                MessageBox.Show($"Setting card reader plan configuration failed, error: {CHCNetSDK.NET_DVR_GetLastError()}");
                CHCNetSDK.NET_DVR_Logout_V30(lUserID);
                CHCNetSDK.NET_DVR_Cleanup();
                return;
            }

            MessageBox.Show("Week schedule and card reader plan configuration successfully set");
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
