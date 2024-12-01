using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Forms.Views.AdministrativeStaff;
using Services.DTOs.AccountDTO;
using Services.Interfaces.AdministrativeStaffServices;
using System.Collections.ObjectModel;
using System.Windows;

namespace Forms.ViewModels.AdministativeStaff
{
    public partial class AccountViewModel : ObservableObject
    {
        private readonly IAccountService _service;

        [ObservableProperty]
        private ObservableCollection<ResponseAccountDTO> accounts = [];

        [ObservableProperty]
        private ObservableCollection<ResponseAccountDTO> filteredAccounts = [];

        [ObservableProperty]
        private string searchText = "";

        public AccountViewModel(IAccountService service)
        {
            _service = service;
            _ = LoadAccountsAsync();
        }

        private async Task LoadAccountsAsync()
        {
            var accounts = await _service.GetAll();
            FilteredAccounts = Accounts = new ObservableCollection<ResponseAccountDTO>(accounts);
        }

        [RelayCommand]
        public void Refresh()
        {
            // Clear search content
            SearchText = string.Empty;
            // Return the initial list of accounts
            FilteredAccounts = new ObservableCollection<ResponseAccountDTO>(Accounts);
        }

        [RelayCommand]
        public async Task Search()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredAccounts = new ObservableCollection<ResponseAccountDTO>(Accounts);
            }
            else
            {
                var result = await _service.Search(SearchText);
                FilteredAccounts = new ObservableCollection<ResponseAccountDTO>(result);
            }
        }

        [RelayCommand]
        public async Task AddAccount()
        {
            RegisterAccountView f = new RegisterAccountView(_service);
            f.ShowDialog();
            await LoadAccountsAsync();
        }

        [RelayCommand]
        public async Task DeleteAccount(string username)
        {
            var result = MessageBox.Show($"Bạn có chắc muốn xóa tài khoản {username}?", "Xóa tài khoản", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                await _service.Delete(username);
                await LoadAccountsAsync();
            }
        }

        [RelayCommand]
        public async Task EditAccount(string username)
        {
            UpdateAccountView f = new UpdateAccountView(_service, username);
            f.ShowDialog();
            await LoadAccountsAsync();
        }
    }
}
