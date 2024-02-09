using DoorControlDemo.Data;
using DoorControlDemo.Models;
using DoorControlDemo.Views;
using GalaSoft.MvvmLight.Command;
using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

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
            get => _userName;
            set
            {
                _userName = value;
                OnPropertyChanged(nameof(UserName));
            }
        }

        private string _userMail;
        public string UserMail
        {
            get => _userMail;
            set
            {
                _userMail = value;
                OnPropertyChanged(nameof(UserMail));
            }
        }

        private string _userPhoneNumber;
        public string UserPhoneNumber
        {
            get => _userPhoneNumber;
            set
            {
                _userPhoneNumber = value;
                OnPropertyChanged(nameof(UserPhoneNumber));
            }
        }

        // Create the method to be used as command
        // Use the data context to add the new user to the database
        public void CreateUserButton()
        {
            //Create an instance of a user
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
        }

    }
}
