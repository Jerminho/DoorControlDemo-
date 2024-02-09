using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;



namespace DoorControlDemo.ViewModels
{
    //Notes for myself

    // Inherit from INotifyPropertyChanged

    // When ViewModelBase is inherited, INotifyPropertyChanged will also be inherited
    // Only inherit when you need the ViewModelBase

    // The viewModelBase can consist of methods that would essentially be re-used
    // Adding them in the ViewModelBase prevents the repition of code
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) //Make sure to include Library: using System.Runtime.CompilerServices;
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void SetValue<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null) //T here stands for type
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
                return;

            backingField = value;

            OnPropertyChanged(propertyName);
        }

        // Method to navigate back to the Main window
        public void NavigateToWindow(Window window)
        {
            window.Show();
            Application.Current.Windows[0]?.Close();
        }

        // Navigate back to the AssignBadge Window
        public void NavigateToAssignBadgeWindow(Window window)
        {
            window.Show();
            /*Application.Current.Windows[0]?.Close();*/
        }
    }
}
