using DoorControlDemo.Data;
using DoorControlDemo.Models;
using GalaSoft.MvvmLight.Command;
using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace DoorControlDemo.ViewModels
{
    public class CreateDeviceViewModel : ViewModelBase
    {
        // Declare the database
        public readonly DoorControlDbContext _dbContext;

        //Declare a MessageBoxDisplay
        private MessageBoxDisplay _messageBoxDisplay = new();

        // Set the constructor
        public CreateDeviceViewModel(DoorControlDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            CreateDeviceCommand = new RelayCommand(CreateDeviceButton);

            // Lambda expression
            // Navigate to MainWindow
            CreateMainCommand = new RelayCommand(()=>NavigateToWindow(new MainWindow()));
        }

        // Declare the Create User Command
        public ICommand CreateDeviceCommand { get; }
        // Declare the Create Main Command to redirect home
        public ICommand CreateMainCommand { get; set; }

        // Declare a private field for the new value
        string _deviceName;

        // Set its new value
        public string DeviceName
        {
            get => _deviceName;
            set
            {
                _deviceName = value;
                OnPropertyChanged(nameof(DeviceName));
            }
        }

        string _ipAddress;
        public string IpAddress
        {
            get => _ipAddress;
            set
            {
                _ipAddress = value;
                OnPropertyChanged(nameof(IpAddress));
            }
        }

        string _port;
        public string Port
        {
            get => _port;
            set
            {
                _port = value;
                OnPropertyChanged(nameof(Port));
            }
        }

        //Set a method to create a device
        public void CreateDeviceButton()
        {
            //Create an instance of a device
            Device device = new();

            // Check if a device with the same properties already exists in the database
            if ((string.IsNullOrEmpty(_deviceName)  || string.IsNullOrEmpty(_ipAddress) || string.IsNullOrEmpty(_port)))
            {
                if(_dbContext.Devices.Any(d => d.Name == "Default Access Device" && d.Ip == "192.168.1.1" && d.PortNumber == "8008"))
                {
                    _messageBoxDisplay.DisplayMessage("Device with the same properties already exists.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            if (_dbContext.Devices.Any(d => d.Name == _deviceName /*&& d.Ip == _ipAddress && d.PortNumber == _port*/))
            {
                _messageBoxDisplay.DisplayMessage("Device with the same Name already exists.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var createdDevice = device.CreateDevice(_port, _ipAddress, _deviceName);

            
            if (createdDevice is null)
            {
                _messageBoxDisplay.DisplayMessage(device.Message);
                return;
            }

            // Add the device to the context
            _dbContext.Devices.Add(createdDevice);

            // Save changes to the database
            _dbContext.SaveChanges();

            // Add additional logic as needed, e.g., validation, interaction with your data context
            // Construct a message string with information about all devices
            StringBuilder devicesInfo = new StringBuilder("Devices in the database:\n");

            foreach (var dev in _dbContext.Devices)
            {
                devicesInfo.AppendLine($"Name: {dev.Name}, IP: {dev.Ip}, Port: {dev.PortNumber}");
            }

            // Display the message with device information
            _messageBoxDisplay.DisplayMessage($"Device {createdDevice.Name} created successfully!\n\n{devicesInfo.ToString()}");
        }
    }
}
