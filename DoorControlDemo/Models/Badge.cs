using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoorControlDemo.Models
{
    public class Badge
    {
        
        [Key]
        public string BadgeId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        // Add a message in case one must be given
        private string? _message;
        public string? Message { get => _message; set => _message = value; }


        //Navigation Property.
        public User? User { get; set; }



        // Set the Necessary Methods.
        public Badge createBadge(string Id)
        {
            
            // Consider the Validations

            // If the user has not entered any value, return theh error message.
            if (string.IsNullOrEmpty(BadgeId))
            {
                _message = "Please enter a correct ID number!";
            }

            // Check whether the user has entered a valid Id (Which sould be an int).
            try
            {
                // If the string can succesfully converted, the user has entered a valid value.
                Convert.ToInt32(Id);
            }
            catch
            {
                // An error message gets returned when the entered values are invalid.
                _message = "Please enter a correct ID number!";
                return null;
            }

            // Create the Badge.
            Badge badge = new()
            {
                BadgeId = Id,
            };

            // Return the badge.
            return badge;

        }
    }

}

// BadgeId can just be  Id
// [ForeignKey("User")] is not required after setting up the navigation property, as the best way to set up relationships is in the DbContext 
