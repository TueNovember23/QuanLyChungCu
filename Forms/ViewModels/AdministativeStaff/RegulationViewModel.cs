using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core;
using Repositories.Repositories.Entities;
using Services.DTOs.RegulationDTO;
using Services.Interfaces.AdministrativeStaffServices;
using Services.Services.SharedServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Forms.ViewModels.AdministativeStaff
{
    public partial class RegulationViewModel : ObservableObject
    {
        private readonly IRegulationService _regulationService;

        [ObservableProperty]
        private ObservableCollection<RegulationDTO> _regulations;

        [ObservableProperty]
        private RegulationDTO? _selectedRegulation;

        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private string _selectedCategory = string.Empty;

        [ObservableProperty]
        private bool _isLoading;

        public ObservableCollection<string> Categories { get; } =
            new(RegulationConstantsService.Categories);

        public ObservableCollection<string> PriorityLevels { get; } =
            new(RegulationConstantsService.PriorityLevels);

        public RegulationViewModel(IRegulationService regulationService)
        {
            _regulationService = regulationService;
            _regulations = new ObservableCollection<RegulationDTO>();
            LoadRegulationsAsync().ConfigureAwait(false);
        }

        [RelayCommand]
        private async Task LoadRegulationsAsync()
        {
            try
            {
                IsLoading = true;
                var regulations = await _regulationService.GetAllRegulationsAsync();
                Regulations = new ObservableCollection<RegulationDTO>(regulations);
                SearchText = string.Empty;
                SelectedCategory = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}");
            }
            finally 
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            if (SelectedRegulation == null)
                return;

            if (string.IsNullOrWhiteSpace(SelectedRegulation.Title))
                throw new BusinessException("Tiêu đề không được để trống");

            if (SelectedRegulation.RegulationId == 0)
                await _regulationService.CreateRegulationAsync(SelectedRegulation);
            else
                await _regulationService.UpdateRegulationAsync(SelectedRegulation);

            await LoadRegulationsAsync();
        }

        [RelayCommand]
        private async Task SearchAsync()
        {
            var results = await _regulationService.SearchRegulationsAsync(SearchText, SelectedCategory);
            Regulations = new ObservableCollection<RegulationDTO>(results);
        }

        [RelayCommand]
        private async Task ExportToPdfAsync()
        {
            if (SelectedRegulation == null)
                return;

            try
            {
                var pdfBytes = await _regulationService.ExportToPdfAsync(SelectedRegulation);
                // Implement save file dialog and saving logic
            }
            catch (NotImplementedException)
            {
                throw new BusinessException("Tính năng đang được phát triển");
            }
        }

        [RelayCommand]
        private void CreateNew()
        {
            SelectedRegulation = new RegulationDTO
            {
                CreatedDate = DateTime.Now,
                IsActive = true
            };
        }

        partial void OnSearchTextChanged(string value)
        {
            SearchAsync().ConfigureAwait(false);
        }

        partial void OnSelectedCategoryChanged(string value)
        {
            SearchAsync().ConfigureAwait(false);
        }
    }
}
