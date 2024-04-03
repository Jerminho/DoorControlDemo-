﻿using DoorControlDemo.Data;
using DoorControlDemo.Models;
using GalaSoft.MvvmLight.Command;
using System;
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
                /*getUserByCardNo("0010517883");*/
                /* uploadCardNo("6","oran","0010517866");
                 uploadCardNo(_userId, _setUserName, _cardNumber);*/

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

        // Method to check if door operations can be performed
        private bool CanOperateDoor()
        {
            // Check if the user is logged in
            return _loginService.IsLoggedIn;
        }

        //
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
                        doorCardNo = System.Text.Encoding.Default.GetString(struData.byCardNo);
                        //textBoxCardRightPlan.Text = struData.wCardRightPlan[0].ToString();
                        doorEmployeeNo = struData.dwEmployeeNo.ToString();
                        doorUserName = System.Text.Encoding.Default.GetString(struData.byName);
                        MessageBox.Show("NET_DVR_GET_CARD success");
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

        // To be used
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
            }
        }



        long m_ISetCardCFfgHandle;
        long m_IGetCardCFfgHandle;
        string m_csCardPassword;
        string m_csCardNo;
        bool bGetCardCfgFinish = false;
        bool bSetCardCfgFinish = false;

        public void g_fGetGatewayCardCallBack(uint dwType, IntPtr lpBuffer, uint dwBufLen, IntPtr pUserData)
        {
            if (dwType == (uint)NET_SDK_CALLBACK_TYPE.NET_SDK_CALLBACK_TYPE_DATA)
            {
                /*string ok;*/

                NET_DVR_CARD_CFG_V50 lpCardCfg = new();
                CHCNetSDK.NET_DVR_ACS_EVENT_CFG lpAcsEventCfg = new CHCNetSDK.NET_DVR_ACS_EVENT_CFG();
                lpAcsEventCfg = (CHCNetSDK.NET_DVR_ACS_EVENT_CFG)Marshal.PtrToStructure(lpBuffer, typeof(CHCNetSDK.NET_DVR_ACS_EVENT_CFG));
                IntPtr ptrAcsEventCfg = Marshal.AllocHGlobal(Marshal.SizeOf(lpAcsEventCfg));
                Marshal.StructureToPtr(lpAcsEventCfg, ptrAcsEventCfg, true);

                CHCNetSDK.NET_DVR_ACS_EVENT_CFG struEventCfg = new CHCNetSDK.NET_DVR_ACS_EVENT_CFG();
                struEventCfg = (CHCNetSDK.NET_DVR_ACS_EVENT_CFG)Marshal.PtrToStructure(ptrAcsEventCfg, typeof(CHCNetSDK.NET_DVR_ACS_EVENT_CFG));
                Marshal.FreeHGlobal(ptrAcsEventCfg);

                MessageBox.Show($"Numero empleado: {Encoding.UTF8.GetString(struEventCfg.struAcsEventInfo.byEmployeeNo)}");

                /*Marshal.Copy(lpBuffer, lpCardCfg, 0, sizeof(lpCardCfg));*/
            }
            else if (dwType == (uint)CHCNetSDK.NET_SDK_CALLBACK_TYPE.NET_SDK_CALLBACK_TYPE_STATUS)
            {
                int dwStatus = Marshal.ReadInt32(lpBuffer);
                if (dwStatus == (uint)CHCNetSDK.NET_SDK_CALLBACK_STATUS_NORMAL.NET_SDK_CALLBACK_STATUS_SUCCESS)
                {
                    MessageBox.Show($"Finalizando la lectura");
                }
                else if (dwStatus == (uint)CHCNetSDK.NET_SDK_CALLBACK_STATUS_NORMAL.NET_SDK_CALLBACK_STATUS_FAILED)
                {
                    MessageBox.Show("Existio un error en la lectura de accesos... " + NET_DVR_GetLastError());
                    //g_formList.AddLog(m_iDeviceIndex, AcsDemoPublic.OPERATION_FAIL_T, "NET_DVR_GET_ACS_EVENT failed");
                }
            }
        }

        public void getCardDetails()
        {
           string errorMessage = NET_DVR_GetLastError().ToString();

            NET_DVR_Init();

            //Login
            int lUserID;
            CHCNetSDK.NET_DVR_USER_LOGIN_INFO loginInfo = new CHCNetSDK.NET_DVR_USER_LOGIN_INFO();
            CHCNetSDK.NET_DVR_DEVICEINFO_V40 deviceInfo = new CHCNetSDK.NET_DVR_DEVICEINFO_V40();
            deviceInfo.struDeviceV30.sSerialNumber = new byte[CHCNetSDK.SERIALNO_LEN];
            loginInfo.sDeviceAddress = "192.0.0.64";
            loginInfo.sUserName = "admin";
            loginInfo.sPassword = "Vika12345";

            ushort.TryParse("8000", out loginInfo.wPort);
            lUserID = CHCNetSDK.NET_DVR_Login_V40(ref loginInfo, ref deviceInfo);
            if (lUserID < 0 )
            {
                
                MessageBox.Show(errorMessage);
                NET_DVR_Cleanup();
            }
            else
            {
                MessageBox.Show("Login Succes for Card Retrieval");
            }


            // Get card information
            NET_DVR_CARD_CFG_COND struCond = new();
            struCond.dwSize = (uint)Marshal.SizeOf(struCond);
            struCond.dwCardNum = 1; //Number of cards
            struCond.byCheckCardNo = 1;

            //Tapped
            Int32 dwSize = Marshal.SizeOf(struCond);
            IntPtr ptrIpParaCfgV40 = Marshal.AllocHGlobal(dwSize);
            Marshal.StructureToPtr(struCond, ptrIpParaCfgV40, false);
            //Tapped

            NET_DVR_CARD_CFG_SEND_DATA struSendData = new();
            struSendData.dwSize = (uint)Marshal.SizeOf(struSendData);
            m_csCardNo = "0010592488";

            // Assuming struSendData.byCardNo is a byte array to store the card number
            byte[] byCardNo = new byte[ACS_CARD_NO_LEN];

            // Convert the string to bytes using ASCII encoding
            byte[] cardNoBytes = Encoding.ASCII.GetBytes(m_csCardNo);

            // Copy the bytes to the byCardNo array
            Array.Copy(cardNoBytes, 0, byCardNo, 0, Math.Min(cardNoBytes.Length, ACS_CARD_NO_LEN));

            // Pad with zeroes if necessary to fill ACS_CARD_NO_LEN bytes
            if (cardNoBytes.Length < ACS_CARD_NO_LEN)
            {
                Array.Clear(byCardNo, cardNoBytes.Length, ACS_CARD_NO_LEN - cardNoBytes.Length);
            }



            //Start RemoteConfig

            m_IGetCardCFfgHandle = NET_DVR_StartRemoteConfig(lUserID, NET_DVR_GET_CARD_CFG_V50, ptrIpParaCfgV40, 
                dwSize, g_fGetGatewayCardCallBack, new IntPtr(0));

            if (m_IGetCardCFfgHandle == -1)
            {
                MessageBox.Show("NET_DVR_StartRemoteConfig fail, Error ", errorMessage);
                NET_DVR_Logout_V30(lUserID);
                NET_DVR_Cleanup();
                return;
            }
            else
            {
                MessageBox.Show("NET_DVR_StartRemoteConfig Succes");
            }

            if (!NET_DVR_SendRemoteConfig((int)m_IGetCardCFfgHandle, (uint)LONG_CFG_SEND_DATA_TYPE_ENUM.ENUM_ACS_SEND_DATA, ptrIpParaCfgV40, (uint)dwSize))
            {
                MessageBox.Show("Search Remotely Failed: ", errorMessage);
                NET_DVR_StopRemoteConfig((int)m_IGetCardCFfgHandle);
                NET_DVR_Logout_V30(lUserID);
                NET_DVR_Cleanup();

            }
            else
            {
                MessageBox.Show("Remote Search SUCCES");
            }


            Thread.Sleep(10000);
            if (bGetCardCfgFinish)
            {
                NET_DVR_StopRemoteConfig((int)m_IGetCardCFfgHandle);
            }
        }

        

        public void g_fSetGatewayCardCallback(uint dwType, IntPtr lpBuffer, uint dwBufLen, IntPtr pUserData)
        {
            if (dwType != (uint)NET_SDK_CALLBACK_TYPE.NET_SDK_CALLBACK_TYPE_STATUS)
            {
                return; //Returning the status
            }

            uint dwStatus = (uint)lpBuffer;

            if (dwStatus == (uint)NET_SDK_CALLBACK_STATUS_NORMAL.NET_SDK_CALLBACK_STATUS_PROCESSING)
            {
                // Assuming lpBuffer is a single IntPtr representing the buffer
                IntPtr lpBufferPtr = lpBuffer;

                // Read the string from lpBuffer starting at an offset of 4 bytes
                string szCardNumber = Marshal.PtrToStringAnsi(lpBufferPtr + 4, ACS_CARD_NO_LEN);

                MessageBox.Show("SetCard PROCESSING, CardNo: " + szCardNumber);
            }
            else if (dwStatus == (uint)NET_SDK_CALLBACK_STATUS_NORMAL.NET_SDK_CALLBACK_STATUS_FAILED)
            {
                byte[] szCardNumber = new byte[ACS_CARD_NO_LEN + 1]; // Create a byte array to store the card number
                IntPtr lpbufferPtr = lpBuffer; // Assuming lpbuffer is a pointer to unmanaged memory

                // Error code
                int dwErrCode = Marshal.ReadInt32(lpbufferPtr + 1);

                // Copy card number bytes from lpbuffer to szCardNumber
                Marshal.Copy(lpbufferPtr + 8, szCardNumber, 0, ACS_CARD_NO_LEN);

                // Convert byte array to string
                string cardNumber = System.Text.Encoding.ASCII.GetString(szCardNumber).TrimEnd('\0');

                MessageBox.Show($"SetCard Error code {dwErrCode}, Card Number {cardNumber}");
            }
            else if (dwStatus == (uint)NET_SDK_CALLBACK_STATUS_NORMAL.NET_SDK_CALLBACK_STATUS_SUCCESS)
            {
                MessageBox.Show("SetCard SUCCESS!");
                bSetCardCfgFinish = true;
            }
            else if (dwStatus == (uint)NET_SDK_CALLBACK_STATUS_NORMAL.NET_SDK_CALLBACK_STATUS_EXCEPTION)
            {
                MessageBox.Show("Exception");
                bSetCardCfgFinish = true;
            }
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
