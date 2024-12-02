using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Forms.Views.AdministrativeStaff;
using Microsoft.VisualBasic.ApplicationServices;
using Repositories.Repositories.Entities;
using Services.DTOs.AccountDTO;
using Services.DTOs.LoginDTO;
using Services.DTOs.MaintenanceDTO;
using Services.Interfaces.AdministrativeStaffServices;
using System.Collections.ObjectModel;

namespace Forms.ViewModels.AdministativeStaff
{
    public partial class MaintainanceViewModel : ObservableObject
    {
        private readonly IMaintananceService _maintananceService;

        [ObservableProperty]
        private ObservableCollection<ResponseMaintenance> maintenances = [];

        [ObservableProperty]
        private ObservableCollection<ResponseMaintenance> filteredMaintenances = [];

        [ObservableProperty]
        private string searchText = "";

        public LoginResponseDTO? User { get; set; }

        public MaintainanceViewModel(IMaintananceService maintananceService)
        {
            _maintananceService = maintananceService;
            _ = LoadMaintenancesAsync();
        }

        private async Task LoadMaintenancesAsync()
        {
            var maintenances = await _maintananceService.GetAll();
            FilteredMaintenances = Maintenances = new ObservableCollection<ResponseMaintenance>(maintenances);
        }

        [RelayCommand]
        public void Refresh()
        {
            // Clear search content
            SearchText = string.Empty;
            // Return the initial list of maintenances
            FilteredMaintenances = new ObservableCollection<ResponseMaintenance>(Maintenances);
        }

        [RelayCommand]
        public async Task Search()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredMaintenances = new ObservableCollection<ResponseMaintenance>(Maintenances);
            }
            else
            {
                var result = await _maintananceService.Search(SearchText);
                FilteredMaintenances = new ObservableCollection<ResponseMaintenance>(result);
            }
        }

        [RelayCommand]
        public async Task CompleteMaintance(int id)
        {
            await _maintananceService.CompleteMaintenance(id);
            await LoadMaintenancesAsync();
        }

        [RelayCommand]
        public void AddMaintenance()
        {
            AddMaintenanceView f = new(_maintananceService);
            f.User = User;
            f.ShowDialog();
        }
    }
}
