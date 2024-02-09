using DoorControlDemo.Data;
using DoorControlDemo.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DoorControlDemo.Views
{
    /// <summary>
    /// Interaction logic for CreateDeviceView.xaml
    /// </summary>
    public partial class CreateDeviceView : Window
    {
        public CreateDeviceView()
        {
            InitializeComponent();
            InitializeComponent();
            DataContext = new CreateDeviceViewModel(((App)Application.Current)._serviceProvider.GetRequiredService<DoorControlDbContext>());

        }
    }
}
