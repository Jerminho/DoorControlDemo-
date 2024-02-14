using DoorControlDemo.Data;
using System;
using System.Windows;

namespace DoorControlDemo.Models
{
    public class LoginService : ILoginService
    {
        // Private field to store the login state
        private bool _isLoggedIn = false;

        // Implementation of the IsLoggedIn property from the interface
        public bool IsLoggedIn
        {
            get { return _isLoggedIn; }
            set { _isLoggedIn = value; }
        }

        // Additional methods or properties related to login management could go here
    }
}
