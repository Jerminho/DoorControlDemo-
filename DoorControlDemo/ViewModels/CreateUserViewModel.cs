using DoorControlDemo.Data;
using DoorControlDemo.Models;
using DoorControlDemo.Views;
using GalaSoft.MvvmLight.Command;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using static DoorControlDemo.CHCNetSDK;

namespace DoorControlDemo.ViewModels
{   
    public class CreateUserViewModel : ViewModelBase
    {
        // Declare the database
        public readonly DoorControlDbContext _dbContext;

        //Declare a MessageBoxDisplay
        private MessageBoxDisplay _messageBoxDisplay = new();

        // Set the constructor
        public CreateUserViewModel(DoorControlDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            CreateUserCommand = new RelayCommand(CreateUserButton);

            // Lambda expression
            // Navigate to MainWindow
            CreateMainCommand = new RelayCommand(()=>NavigateToWindow(new MainWindow()));
            //Navigate to AssignBadgeWindow
            NavigateToAssignBadgeCommand = new RelayCommand(() => NavigateToAssignBadgeWindow(new AssignBadgeView()));
        }

        // Declare the Create User Command
        public ICommand CreateUserCommand { get; }

        // Declare the Create Main Command to redirect home
        public ICommand CreateMainCommand { get; set; }

        public ICommand NavigateToAssignBadgeCommand { get; set; }

        // Declare a private field for the new value
        string _userName;
        // Set its new value
        public string UserName
        {
            get =>      _userName;
            set
            {
                _userName = value;
                OnPropertyChanged(nameof(UserName));
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

        private string _userCardNo;
        public string UserCardNo
        {
            get => _userCardNo;
            set
            {
                _userCardNo = value;
                OnPropertyChanged(nameof(UserCardNo));
            }
        }

        // Create the method to be used as command
        // Use the data context to add the new user to the database
        public void CreateUserButton()
        {
            /*//Create an instance of a user
            User user = new();

            // Check if a device with the same properties already exists in the database and return
            if (_dbContext.Users.Any(u => u.Name == _userName && u.Mail == _userMail && u.PhoneNumber == _userPhoneNumber))
            {
                MessageBox.Show($"User with the same properties already exists.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // create a new badge using the instance
            var createdUser = user.CreateUser(_userName,_userMail,_userPhoneNumber);

            // If the createdbadge is null, return an error
            if (createdUser == null)
            {
                _messageBoxDisplay.DisplayMessage(user.Message);
                return;
            }

            // If the Name field is null or empty, return an error
            if (string.IsNullOrEmpty(createdUser.Name)) 
            {
                _messageBoxDisplay.DisplayMessage(user.Message);
                return;
            }

            // Add the user to the context
            _dbContext.Users.Add(createdUser);

            // Save changes to the database
            _dbContext.SaveChanges();


            // Add additional logic as needed, e.g., validation, interaction with your data context
            // Construct a message string with information about all Users
            StringBuilder usersInfo = new StringBuilder("Users in the database:\n");

            foreach (var u in _dbContext.Users)
            {
                usersInfo.AppendLine($" User: {u.Name}");
            }

            // Display the message with badge information
            MessageBox.Show($"User {createdUser.Name} created successfully!\n\n{usersInfo.ToString()}");
            */
            loginDevice();
            uploadCardNo(_userId.ToString(),_userName.ToString(), _userCardNo.ToString());
        }

        public static int m_UserID = 1;
        private RemoteConfigCallback g_fGetGatewayCardCallback = null;

        string m_csCardNo;

        public IntPtr hwnd;
        public Int32 m_lGetCardCfgHandle = -1;
        public Int32 m_lSetCardCfgHandle = -1;
        public Int32 m_lDelCardCfgHandle = -1;

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


    }
}
