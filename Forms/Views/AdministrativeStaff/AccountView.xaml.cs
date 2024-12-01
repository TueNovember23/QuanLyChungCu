﻿using Microsoft.Extensions.DependencyInjection;
using Services.Services.AdministrativeStaffServices;
using System.Windows.Controls;

namespace Forms.Views.AdministrativeStaff
{
    /// <summary>
    /// Interaction logic for AccountView.xaml
    /// </summary>
    public partial class AccountView : UserControl
    {
        public AccountView()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider?.GetService<AccountService>()!;
        }
    }
}
