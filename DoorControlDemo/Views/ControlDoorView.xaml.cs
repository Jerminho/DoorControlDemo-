﻿using DoorControlDemo.Data;
using DoorControlDemo.ViewModels;
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
    /// Interaction logic for ControlDoorView.xaml
    /// </summary>
    public partial class ControlDoorView : Window
    {
        public ControlDoorView()
        {
            InitializeComponent();
            //DataContext = new ControlDoorOperationsViewModel(((App)Application.Current)._serviceProvider.GetRequiredService<DoorControlDbContext>());
        }
    }
}
