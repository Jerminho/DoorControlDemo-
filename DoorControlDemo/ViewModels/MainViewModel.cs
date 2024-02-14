using DoorControlDemo.Models;
using DoorControlDemo.Views;
using GalaSoft.MvvmLight.Command;
using System.Windows;

namespace DoorControlDemo.ViewModels
{
    public class MainViewModel
    {
        //Set the Command Propeties
        public RelayCommand CreateBadgeCommand { get; private set; }
        public RelayCommand CreateDeviceCommand { get; private set; }
        public RelayCommand CreateUserCommand { get; private set; }
        public RelayCommand LogInDeviceCommand { get; private set; }
        public RelayCommand NavigateToControlDoorCommand { get; private set; }

        private ILoginService loginService;

        // Property to set loginService
        public ILoginService LoginService
        {
            get { return loginService; }
            set { loginService = value; }
        }

        // Link the commands to their respective actions and call them
        public MainViewModel() 
        {
            // Instantiate the commands and pass a method
            CreateBadgeCommand = new RelayCommand(CreateBadgeButtonClick);
            CreateDeviceCommand = new RelayCommand(CreateDeviceButtonClick);
            CreateUserCommand = new RelayCommand(CreateUserButtonClick);
            LogInDeviceCommand = new RelayCommand(LogInDeviceButtonClick);
            NavigateToControlDoorCommand = new RelayCommand(NavigateToControlDoorButtonClick);

        }

        // Set the methods
        // Create the views and close the existing view upon opening the new View
        public void CreateBadgeButtonClick()
        {
            // Set an instance of the view
            CreateBadgeView createBadgeView = new();
            // Show the view
            createBadgeView.Show();
            // Close the current window
            Application.Current.Windows[0]?.Close();
        }

        public void CreateDeviceButtonClick()
        {
            CreateDeviceView createDeviceView = new();
            createDeviceView.Show();
            Application.Current.Windows[0]?.Close();
        }

        public void CreateUserButtonClick()
        {
            CreateUserView createUserView = new();
            createUserView.Show();
            Application.Current.Windows[0]?.Close();
        }

        public void LogInDeviceButtonClick()
        {
            LoginDeviceView loginDeviceView = new();
            loginDeviceView.Show();
        }

        public void NavigateToControlDoorButtonClick()
        {
            // Set an instance of the ControlDoorView
            ControlDoorView controlDoorView = new ControlDoorView(LoginService);
            // Show the ControlDoorView
            controlDoorView.Show();
            // Close the current window
            Application.Current.Windows[0]?.Close();
        }
    }
}
