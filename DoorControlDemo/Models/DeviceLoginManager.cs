using System.Runtime.InteropServices;

namespace DoorControlDemo.Models
{
    public class DeviceLoginManager
    {
        //property for login status
        public bool IsLoggedIn { get; private set; }

        // login parameters structure
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_USER_LOGIN_INFO
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string sDeviceAddress; // Device address

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string sUserName; // User name

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string sPassword; // Password

        }

        // device information structure
        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_DEVICEINFO_V40
        {
            //
        }


        // Import function from the SDK
        [DllImport(@"SdkLib\lib\HCNetSDK.dll")]
        public static extern int NET_DVR_Login_V40(ref NET_DVR_USER_LOGIN_INFO pLoginInfo, out NET_DVR_DEVICEINFO_V40 lpDeviceInfo);

        // Define a method to perform device login
        public int LoginToDevice(string deviceAddress, string userName, string password)
        {
            // Define login parameters
            NET_DVR_USER_LOGIN_INFO loginInfo = new NET_DVR_USER_LOGIN_INFO();
            loginInfo.sDeviceAddress = deviceAddress;
            loginInfo.sUserName = userName;
            loginInfo.sPassword = password;

            // Call the login function
            NET_DVR_DEVICEINFO_V40 deviceInfo;
            int userID = NET_DVR_Login_V40(ref loginInfo, out deviceInfo);

            // Update login status
            IsLoggedIn = userID != -1;

            // Return  user ID
            return userID;
        }
    }
}
