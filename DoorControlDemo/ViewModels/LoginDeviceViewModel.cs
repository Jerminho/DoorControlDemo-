using DoorControlDemo.Data;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DoorControlDemo.ViewModels
{
    public class LoginDeviceViewModel: ViewModelBase
    {
        // Declare the database
        public readonly DoorControlDbContext dbContext;

        //Declare a MessageBoxDisplay
        private MessageBoxDisplay _messageBoxDisplay = new();

        // Set the constructor
        public LoginDeviceViewModel(DoorControlDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            LoginCommand = new RelayCommand(LoginDeviceButton);
            CreateMainCommand = new RelayCommand(() => NavigateToWindow(new MainWindow()));
        }


        // Declare the Login Command
        public ICommand LoginCommand { get; set; }

        // Declare the Create Main Command to redirect home
        public ICommand CreateMainCommand { get; set; }



        // Declare a private field for the new value
        string _ip;
        // Set its new value
        public string Ip
        {
            get => _ip;
            set
            {
                _ip = value;
                OnPropertyChanged(nameof(Ip));
            }
        }

        private string _portNumber;
        public string PortNumber
        {
            get => _portNumber;
            set
            {
                _portNumber = value;
                OnPropertyChanged(nameof(PortNumber));
            }
        }


        public void LoginDeviceButton()
        {
            // Check if the required information is provided
            if (string.IsNullOrEmpty(_ip) || string.IsNullOrEmpty(_portNumber))
            {
                _messageBoxDisplay.DisplayMessage("IP Address and Port are required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Check if a device with the provided IP address and port number exists in the database
            var deviceToLogIn = dbContext.Devices.FirstOrDefault(d => d.Ip == _ip && d.PortNumber == _portNumber);



            if (deviceToLogIn != null)
            {
                // If the device exists, check if it is already logged in.
                if (deviceToLogIn.IsLoggedIn != true)
                {
                    // Set the IsLoggedIn property to true
                    deviceToLogIn.IsLoggedIn = true;

                    // Save changes to the database
                    dbContext.SaveChanges();

                    // Additional logic if needed

                    // Display a success message
                    _messageBoxDisplay.DisplayMessage($"Device {_ip}:{_portNumber} logged in successfully!", "Login Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Display a success message
                    _messageBoxDisplay.DisplayMessage($"Device {_ip} : {_portNumber} Already Logged in !", "Login Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }


            }
            else
            {
                // Display an error message if the device is not found
                _messageBoxDisplay.DisplayMessage($"Device {_ip}:{_portNumber} not found. Check credentials or add a new device.", "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }


            dbContext.SaveChanges();
        }
        
       
    }
}
