﻿using Forms.ViewModels.ServiceSupervisor;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Forms.Views.ServiceSupervisor
{
    public partial class ParkingView : UserControl
    {
        public ParkingView()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider?.GetService<ParkingViewModel>();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RenderHeader.Children.Clear();
            RenderHeader.Children.Add(new Header());
        }
    }
}