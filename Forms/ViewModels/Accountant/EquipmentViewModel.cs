using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Forms.Views.Accountant;
using Repositories.Repositories.Entities;
using Services.DTOs.EquipmentDTO;
using Services.Interfaces.AccountantServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forms.ViewModels.Accountant
{
    public partial class EquipmentViewModel : ObservableObject
    {
        private readonly IEquipmentService _equipmentService;

        [ObservableProperty]
        private ObservableCollection<ResponseEquipmentDTO> equipments = [];

        [ObservableProperty]
        private ObservableCollection<ResponseEquipmentDTO> filteredEquipments = [];

        [ObservableProperty]
        private ResponseEquipmentDTO? selectedEquipment;

        [ObservableProperty]
        private string searchText = "";

        [ObservableProperty]
        private ObservableCollection<string> areas = [];

       [ObservableProperty]
        private string? selectedArea;

        [ObservableProperty]
        private ObservableCollection<string> statusList = new() { "Hoạt động", "Hỏng", "Bảo trì" };

        [ObservableProperty]
        private string? selectedStatus;

        [ObservableProperty]
        private bool isAddingNewEquipment = false;

        [ObservableProperty]
        private ResponseEquipmentDTO newEquipment;

        [ObservableProperty]
        private bool isDetailsVisible = false;

        public EquipmentViewModel(IEquipmentService equipmentService)
        {
            _equipmentService = equipmentService;
            newEquipment = CreateNewEquipmentDTO();
            _ = LoadEquipmentsAsync();
        }

        private ResponseEquipmentDTO CreateNewEquipmentDTO()
        {
            return new ResponseEquipmentDTO
            {
                EquipmentName = "",
                Status = "Hoạt động",
                Discription = "",
                AreaName = Areas.FirstOrDefault() ?? "",
                LastMaintenanceDate = null,
                Notes = "",
                LastCheckDate = null
            };
        }

        private async Task LoadEquipmentsAsync()
        {
            var equipmentList = await _equipmentService.GetAll();
            FilteredEquipments = Equipments = new ObservableCollection<ResponseEquipmentDTO>(equipmentList);

            Areas = new ObservableCollection<string>(
                equipmentList.Select(e => e.AreaName).Distinct()
            );

        }

        [RelayCommand]
        private async Task Refresh()
        {
            await LoadEquipmentsAsync(); 

            SearchText = string.Empty;
            SelectedArea = null;
            SelectedEquipment = null;
            IsDetailsVisible = false;

            FilteredEquipments = new ObservableCollection<ResponseEquipmentDTO>(Equipments);
        }

        [RelayCommand]
        private async Task Search()
        {
            var searchResults = Equipments;

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                searchResults = new ObservableCollection<ResponseEquipmentDTO>(
                    searchResults.Where(e =>
                        e.EquipmentName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                        (e.Discription != null && e.Discription.Contains(SearchText, StringComparison.OrdinalIgnoreCase))));
            }

            if (!string.IsNullOrWhiteSpace(SelectedArea))
            {
                searchResults = new ObservableCollection<ResponseEquipmentDTO>(
                    searchResults.Where(e => e.AreaName == SelectedArea));
            }

            if (!string.IsNullOrWhiteSpace(SelectedStatus))
            {
                searchResults = new ObservableCollection<ResponseEquipmentDTO>(
                    searchResults.Where(e => e.Status == SelectedStatus));
            }

            FilteredEquipments = searchResults;
        }


        partial void OnSelectedAreaChanged(string? value)
        {
            _ = Search();
        }

    


        [RelayCommand]
        private void ShowAddNewEquipmentForm()
        {
            IsAddingNewEquipment = true;
            NewEquipment = CreateNewEquipmentDTO();
            SelectedEquipment = null;
            IsDetailsVisible = false;
        }

        [RelayCommand]
        private async Task SaveNewEquipment()
        {
            if (string.IsNullOrWhiteSpace(NewEquipment.EquipmentName) ||
                string.IsNullOrWhiteSpace(NewEquipment.AreaName) ||
                string.IsNullOrWhiteSpace(NewEquipment.Status))
            {
                return;
            }

            try
            {
                await _equipmentService.Add(NewEquipment);
                await LoadEquipmentsAsync();
                IsAddingNewEquipment = false;
                NewEquipment = CreateNewEquipmentDTO();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving new equipment: {ex.Message}");
            }
        }

        [RelayCommand]
        private void CancelAddNewEquipment()
        {
            IsAddingNewEquipment = false;
            NewEquipment = CreateNewEquipmentDTO();
        }

        [RelayCommand]
        private async Task SaveChanges()
        {
            if (SelectedEquipment == null) return;

            await _equipmentService.Update(SelectedEquipment);
            await LoadEquipmentsAsync();
            IsDetailsVisible = false;
        }

        [RelayCommand]
        private async Task DeleteEquipment()
        {
            if (SelectedEquipment == null) return;

            await _equipmentService.SoftDelete(SelectedEquipment.EquipmentId);
            Equipments.Remove(SelectedEquipment);
            FilteredEquipments.Remove(SelectedEquipment);
            SelectedEquipment = null;
            IsDetailsVisible = false;
        }

        [RelayCommand]
        private async Task ReportIssue()
        {
            if (SelectedEquipment == null) return;

            SelectedEquipment.Status = "Hỏng";
            await SaveChanges();
        }

        partial void OnSelectedEquipmentChanged(ResponseEquipmentDTO? value)
            {
            if (value == null)
            {
                IsDetailsVisible = false;
                return;
            }
            SelectedStatus = value.Status;
            SelectedEquipment.EquipmentName = value.EquipmentName;
            SelectedEquipment.AreaName = value.AreaName;
            SelectedEquipment.LastMaintenanceDate = value.LastMaintenanceDate;
            SelectedEquipment.Notes = value.Notes;  
            SelectedEquipment.LastCheckDate = value.LastCheckDate;  
            IsDetailsVisible = true;
        }

        [RelayCommand]
        private void ViewAddEquipment(string equipmentCode)
        {
            var addEquipmentViewModel = new AddEquipmentViewModel(_equipmentService, equipmentCode);

            addEquipmentViewModel.EquipmentAdded += async () =>
            {
                await LoadEquipmentsAsync(); 
            };

            var addEquipmentView = new AddEquipmentView(_equipmentService, equipmentCode)
            {
                DataContext = addEquipmentViewModel
            };
            addEquipmentView.ShowDialog();
        }


    }
}
