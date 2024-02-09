using DoorControlDemo.ViewModels;
using System.Windows;

namespace DoorControlDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel MainViewModel { get; set; } = new(); 
        public MainWindow()
        {
            // Set the datacontext
            DataContext = MainViewModel;
            InitializeComponent();
            if (CHCNetSDK.NET_DVR_Init() == false)
            {
                MessageBox.Show("NET_DVR_Init Error!");
                return;
            }
        }
    }
}
