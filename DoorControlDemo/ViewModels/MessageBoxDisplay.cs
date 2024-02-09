using System.Windows;

// Class to display error messages
namespace DoorControlDemo.ViewModels
{
    public class MessageBoxDisplay
    {
        // Set the Methods
        public void DisplayMessage(string message) 
        { 
            MessageBox.Show(message);
        }


        public void DisplayMessage(string message, string errorType, MessageBoxButton messageBoxButton, MessageBoxImage messageBoxImage)
        {
            MessageBox.Show(message, errorType, messageBoxButton, messageBoxImage);
        }
    }
}
