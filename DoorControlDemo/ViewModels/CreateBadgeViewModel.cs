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
    public class CreateBadgeViewModel : ViewModelBase
    {
        // Declare the database
        public readonly DoorControlDbContext _dbContext;

        //Declare a MessageBoxDisplay
        private MessageBoxDisplay _messageBoxDisplay = new();

        // Set the constructor
        public CreateBadgeViewModel(DoorControlDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            CreateBadgeCommand = new RelayCommand(CreateBadgeButton);

            // Lambda expression
            // Navigate to MainWindow
            CreateMainCommand = new RelayCommand(()=>NavigateToWindow(new MainWindow()));
            //Navigate to AssignBadgeWindow
            NavigateToAssignBadgeCommand = new RelayCommand(() => NavigateToAssignBadgeWindow(new AssignBadgeView()));
        }

        // Declare the Create Badge Command
        public ICommand CreateBadgeCommand { get; set; }
        // Declare the Create Main Command to redirect home
        public ICommand CreateMainCommand { get; set; }
        public ICommand NavigateToAssignBadgeCommand { get; set; }


        // Declare a private field for the new value
        string _badgeId;
        // Set its new value
        public string BadgeId
        {
            get => _badgeId;
            set
            {
                _badgeId = value;
                OnPropertyChanged(nameof(BadgeId));
            }
        }
/*
        // Navigate back to the Main window
        public void CreateMainButtonClick()
        {
            MainWindow mainindow = new MainWindow();
            mainindow.Show();
            Application.Current.Windows[0]?.Close();
        }*/


        // Create a method for a badge creation
        public void CreateBadgeButton()
        {
            //Create an instance of a badge
            Badge badge = new();

            // Check if a badge with the same BadgeId already exists in the database
            if (_dbContext.Badges.Any(b => b.BadgeId == _badgeId))
            {
                _messageBoxDisplay.DisplayMessage("This badge already exists.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Implement logic to create a Badge by calling a method from your model
          
            // create a new badge using the instance
            var createdBadge = badge.createBadge(_badgeId);

            // If the createdbadge is null, return an error
            if (createdBadge is null)
            {
                _messageBoxDisplay.DisplayMessage(badge.Message);
                return;
            }

            if (string.IsNullOrEmpty(createdBadge.BadgeId))
            {
                _messageBoxDisplay.DisplayMessage(badge.Message);
                return;
            }

            // Add the badge to the context
            _dbContext.Badges.Add(createdBadge);

            // Save changes to the database
            _dbContext.SaveChanges();

            // Add additional logic as needed, e.g., validation, interaction with your data context
            // Construct a message string with information about all Badges
            StringBuilder badgesInfo = new StringBuilder("Badges in the database:\n");

            foreach (var b in _dbContext.Badges)
            {
                badgesInfo.AppendLine($" BadgeID: {b.BadgeId}");
            }

            // Display the message with badge information
            MessageBox.Show($"Badge {createdBadge.BadgeId} created successfully!\n\n{badgesInfo.ToString()}");
        }

    }
}
