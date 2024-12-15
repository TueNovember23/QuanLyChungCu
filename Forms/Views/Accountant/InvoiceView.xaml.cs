﻿using Forms.ViewModels.Accountant;
using Forms.ViewModels.AdministativeStaff;
using Microsoft.Extensions.DependencyInjection;
using Services.DTOs.LoginDTO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Forms.Views.Accountant
{
    /// <summary>
    /// Interaction logic for InvoiceView.xaml
    /// </summary>
    public partial class InvoiceView : UserControl
    {
        private LoginResponseDTO? _user;
        public LoginResponseDTO? User
        {
            get { return _user; }
            set
            {
                _user = value;
                (DataContext as InvoiceViewModel)!.User = User;
            }
        }

        public InvoiceView()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider?.GetService<InvoiceViewModel>()!;
        }
    }
}
