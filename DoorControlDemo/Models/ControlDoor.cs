using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace DoorControlDemo.Models
{
    internal class ControlDoor
    {
        // Declare the properties
        public int Id { get; set; }

        // User instance to get the role
        private User _user;

        // Constructor to initialize the ControlDoor with a user instance
        public ControlDoor(User user)
        {
            _user = user;
        }

        // Implementation of the SDK Door commands
        public void OpenDoor()
        {
            int userId = _user.Role; // Get the role from the user

            if (CHCNetSDK.NET_DVR_ControlGateway(userId, 1, 1))
            {
                MessageBox.Show("NET_DVR_ControlGateway: open door succeed");
            }
            else
            {
                MessageBox.Show("NET_DVR_ControlGateway: open door failed error:" + CHCNetSDK.NET_DVR_GetLastError());
            }

            MessageBox.Show("Test");
        }

        public void CloseDoor()
        {
            int userId = _user.Role; // Get the role from the user

            if (CHCNetSDK.NET_DVR_ControlGateway(userId, 1, 0))
            {
                MessageBox.Show("NET_DVR_ControlGateway: close door succeed");
            }
            else
            {
                MessageBox.Show("NET_DVR_ControlGateway: close door failed error:" + CHCNetSDK.NET_DVR_GetLastError());
            }
        }

        public void StayOpen()
        {
            int userId = _user.Role; // Get the role from the user

            if (CHCNetSDK.NET_DVR_ControlGateway(userId, 1, 3))
            {
                MessageBox.Show("NET_DVR_ControlGateway: stay close door succeed");
            }
            else
            {
                MessageBox.Show("NET_DVR_ControlGateway:  stay close door failed error:" + CHCNetSDK.NET_DVR_GetLastError());
            }
        }

        public void StayClose()
        {
            int userId = _user.Role; // Get the role from the user

            if (CHCNetSDK.NET_DVR_ControlGateway(userId, 1, 2))
            {
                MessageBox.Show("NET_DVR_ControlGateway: stay open door succeed");
            }
            else
            {
                MessageBox.Show("NET_DVR_ControlGateway:  stay open door failed error:" + CHCNetSDK.NET_DVR_GetLastError());
            }
        }
    }
}
