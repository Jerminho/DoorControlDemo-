using DoorControlDemo.Data;
using DoorControlDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using GalaSoft.MvvmLight.Command;

namespace DoorControlDemo.ViewModels
{
    public class AssignBadgeViewModel : ViewModelBase
    {
        // Declare the database
        public readonly DoorControlDbContext _dbContext;

        //Declare a MessageBoxDisplay
        private MessageBoxDisplay _messageBoxDisplay = new();

        public AssignBadgeViewModel(DoorControlDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            AssignBadgeCommand = new RelayCommand(AssignBadgeButton);

        }

        public ICommand AssignBadgeCommand { get; set; }


        // Exposed properties for data binding
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


        private string _badgeId;
        public string BadgeId
        {
            get => _badgeId;
            set
            {
                _badgeId = value;
                OnPropertyChanged(nameof(BadgeId));
            }
        }


        public void AssignBadgeButton()
        {

            try
            {
                Convert.ToInt32(_badgeId);
                Convert.ToInt32(_userId);

            }
            catch (Exception)
            {
                MessageBox.Show("Please provide correct values");
                throw;
            }


            //If the badge and user exist in the database
            if (_dbContext.Users.Any(user => (user.UserId).ToString() == _userId) && _dbContext.Badges.Any
                (badge => badge.BadgeId == _badgeId))
            {
                // 
                User user = _dbContext.Users.First(u => (u.UserId.ToString() == _userId));
                // If the user has not yet been assigned this badge, assign.
                if (!user.AssignedBadges.Any(b => b.BadgeId == _badgeId))
                {
                    Badge badge = _dbContext.Badges.First(b => b.BadgeId == _badgeId);
                    //Assign the badge
                    user.AssignedBadges.Add(badge);
                    _dbContext.SaveChanges();

                    MessageBox.Show($"Badge has been succesfully assigned to {user.Name} : UserId: {UserId} ");
                }
                // If the user already has this badge assigned, throw message
                else
                {
                    MessageBox.Show("Badge has already been assigned to the user.");
                }
            }
            else
            {
                MessageBox.Show("User does not exist or badge is invalid.");
            }

        }
    }
}
