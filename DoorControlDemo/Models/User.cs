using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows;

namespace DoorControlDemo.Models
{
    public class User
    {
        // Set the primary key
        [Key]

        // Notes for myself
        //the database should automatically generate a unique value
        //for that property when a new record is inserted
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        // Role, with default value 1 for admin
        public int Role { get; set; } = 1;

        public string Name { get; set; }

        // Collection of badges for the user
         public List<Badge> AssignedBadges { get; set; } = new List<Badge>();

        public string? Mail { get; set; }

        //A phone number can hold max 15 numbers
        [StringLength(15)]
        public string? PhoneNumber { get; set; }

        // Add a message in case one must be given
        private string? _message;
        public string? Message { get => _message; set => _message = value; }

        [ForeignKey("Device")]
        public int DeviceId { get; set; }

        // Navigation Property
        public Device? Device { get; set; }


        // Methods

        // Add badge to user
        public void AssignBadge(Badge badge)
        {
            if (!AssignedBadges.Contains(badge)){
                AssignedBadges.Add(badge);
            }
            else
            {
                throw new InvalidOperationException("Badge has already been assigned.");

            }
        }


        // Remove badge from user
        public void RemoveAssignedBadge(Badge badge)
        {
            if (AssignedBadges.Contains(badge))
            {
                AssignedBadges.Remove(badge);
            }
            else
            {
                MessageBox.Show("Badge cannot be removed. It was never assigned.");

            }
        }

        public User CreateUser(string name, string? email, string? phonenumber)
        {

            // Check if required fields are empty
            if (string.IsNullOrWhiteSpace(Name))
            {
                _message = "Please fill in a name.";
            }


            // Code below is not fully operational, must be reviewed and optimized--!

            /*if (!Regex.IsMatch(Name, "^[a-zA-Z]+$"))
            {
                MessageBox.Show("Invalid name. The name should contain only letters.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                
            }*/


            // Create an instance of the Class, set properties and return
            User user = new User
            {
                Name = name,
                Mail = email,
                PhoneNumber = phonenumber
            };
            return user;

        }
        

    }
}
