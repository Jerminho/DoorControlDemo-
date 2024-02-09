using System.Windows;
using DoorControlDemo.Data;
using DoorControlDemo.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DoorControlDemo.Views
{
    /// <summary>
    /// Interaction logic for CreateUserView.xaml
    /// </summary>
    public partial class CreateUserView : Window
    {
        // Create an instance of your view model and set it as the data context
        public CreateUserView()
        {
            InitializeComponent();
            DataContext = new CreateUserViewModel(((App)Application.Current)._serviceProvider.GetRequiredService<DoorControlDbContext>());

        }
    }
}
