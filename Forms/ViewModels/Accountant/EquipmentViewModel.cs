using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Repositories.Repositories.Entities;
using Services.DTOs.EquipmentDTO;
using Services.Interfaces.AccountantServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public EquipmentViewModel(IEquipmentService equipmentService)
        {
            _equipmentService = equipmentService;
            _ = LoadEquipmentsAsync();
        }

        private async Task LoadEquipmentsAsync()
        {
            var equipmentList = await _equipmentService.GetAll();
            FilteredEquipments = Equipments = new ObservableCollection<ResponseEquipmentDTO>(equipmentList);

            // Load unique areas for the filter
            Areas = new ObservableCollection<string>(
                equipmentList.Select(e => e.AreaName).Distinct()
            );
        }

        [RelayCommand]
        private void Refresh()
        {
            SearchText = string.Empty;
            SelectedArea = null;
            FilteredEquipments = new ObservableCollection<ResponseEquipmentDTO>(Equipments);
        }

        [RelayCommand]
        private async Task Search()
        {
            if (string.IsNullOrWhiteSpace(SearchText) && string.IsNullOrWhiteSpace(SelectedArea))
            {
                FilteredEquipments = new ObservableCollection<ResponseEquipmentDTO>(Equipments);
            }
            else
            {
                var searchResults = await _equipmentService.Search(SearchText);

                // Apply area filter if selected
                if (!string.IsNullOrWhiteSpace(SelectedArea))
                {
                    searchResults = searchResults.Where(e => e.AreaName == SelectedArea).ToList();
                }

                FilteredEquipments = new ObservableCollection<ResponseEquipmentDTO>(searchResults);
            }
        }

        partial void OnSelectedAreaChanged(string? value)
        {
            _ = Search();
        }

        [RelayCommand]
        private void AddNewEquipment()
        {
            // Implement new equipment addition logic
            System.Diagnostics.Debug.WriteLine("AddNewEquipment called");
        }

        [RelayCommand]
        private void ScheduleMaintenance()
        {
            if (SelectedEquipment == null) return;
            System.Diagnostics.Debug.WriteLine($"ScheduleMaintenance called for equipment: {SelectedEquipment.EquipmentId}");
        }

        [RelayCommand]
        private void SaveChanges()
        {
            if (SelectedEquipment == null) return;
            System.Diagnostics.Debug.WriteLine($"SaveChanges called for equipment: {SelectedEquipment.EquipmentId}");
        }

        [RelayCommand]
        private void CancelChanges()
        {
            // Reset selected equipment to original state
            if (SelectedEquipment != null)
            {
                var original = Equipments.FirstOrDefault(e => e.EquipmentId == SelectedEquipment.EquipmentId);
                if (original != null)
                {
                    SelectedEquipment = original;
                }
            }
        }

        [RelayCommand]
        private void ReportIssue()
        {
            if (SelectedEquipment == null) return;
            System.Diagnostics.Debug.WriteLine($"ReportIssue called for equipment: {SelectedEquipment.EquipmentId}");
        }
    }
}
