using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;



namespace DoorControlDemo.ViewModels
{
    //Notes for myself

    // Inherit from INotifyPropertyChanged

    // When ViewModelBase is inherited, INotifyPropertyChanged will also be inherited
    // Only inherit when you need the ViewModelBase

    // The viewModelBase can consist of methods that would essentially be re-used
    // Adding them in the ViewModelBase prevents the repition of code
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) //Make sure to include Library: using System.Runtime.CompilerServices;
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void SetValue<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null) //T here stands for type
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
                return;

            backingField = value;

            OnPropertyChanged(propertyName);
        }

        // Method to navigate back to the Main window
        public void NavigateToWindow(Window window)
        {
            window.Show();
            Application.Current.Windows[0]?.Close();
        }

        // Navigate back to the AssignBadge Window
        public void NavigateToAssignBadgeWindow(Window window)
        {
            window.Show();
            /*Application.Current.Windows[0]?.Close();*/
        }

        public static int m_UserID = 1;
        public void loginDevice()
        {
            bool isLoggedIn = false;

            // Prepare the login info
            CHCNetSDK.NET_DVR_USER_LOGIN_INFO loginInfo = new CHCNetSDK.NET_DVR_USER_LOGIN_INFO();
            CHCNetSDK.NET_DVR_DEVICEINFO_V40 deviceInfo = new CHCNetSDK.NET_DVR_DEVICEINFO_V40();
            deviceInfo.struDeviceV30.sSerialNumber = new byte[CHCNetSDK.SERIALNO_LEN];
            loginInfo.sDeviceAddress = "192.0.0.64";
            loginInfo.sUserName = "admin";
            loginInfo.sPassword = "Vika12345";

            ushort.TryParse("8000", out loginInfo.wPort);

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
                isLoggedIn = true;
            }
        }
    }
}
