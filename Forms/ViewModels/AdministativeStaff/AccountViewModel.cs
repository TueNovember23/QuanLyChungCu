using CommunityToolkit.Mvvm.ComponentModel;
using Services.DTOs.AccountDTO;
using Services.Interfaces.AdministrativeStaffServices;
using System.Collections.ObjectModel;

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
            // _ = LoadAccountsAsync();
        }
    }
}
