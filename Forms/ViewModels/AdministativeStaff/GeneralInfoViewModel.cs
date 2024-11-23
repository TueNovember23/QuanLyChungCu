using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core;
using Forms.ViewModels.AdministativeStaff.GeneralInfo;
using Forms.Views.AdministrativeStaff.Dialogs;
using MaterialDesignThemes.Wpf;
using Services.DTOs.GeneralInfo.AreaDTO;
using Services.Interfaces.AdministrativeStaffServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forms.ViewModels.AdministativeStaff
{
   public partial class GeneralInfoViewModel : ObservableObject
    {
        private readonly IGeneralInfoService _generalInfoService;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string searchAreaText;

        [ObservableProperty]
        private ObservableCollection<AreaResponseDTO> areas;

        [ObservableProperty]
        private AreaResponseDTO selectedArea;

        public GeneralInfoViewModel(IGeneralInfoService generalInfoService)
        {
            _generalInfoService = generalInfoService;
            Areas = new ObservableCollection<AreaResponseDTO>();
        }

        [RelayCommand]
        private async Task LoadAreas()
        {
            try
            {
                IsLoading = true;
                var areas = await _generalInfoService.GetAllAreasAsync();
                Areas = new ObservableCollection<AreaResponseDTO>(areas);
            }
            catch (BusinessException ex)
            {
                // Will be handled by global exception handler
                throw;
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task RefreshData()
        {
            await LoadAreas();
        }

        [RelayCommand]
        private async Task SearchArea()
        {
            try
            {
                IsLoading = true;
                var results = await _generalInfoService.SearchAreasAsync(SearchAreaText);
                Areas = new ObservableCollection<AreaResponseDTO>(results);
            }
            catch (BusinessException ex)
            {
                throw;
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task DeleteArea(AreaResponseDTO area)
        {
            if (area == null) return;

            try
            {
                IsLoading = true;
                await _generalInfoService.DeleteAreaAsync(area.AreaId);
                Areas.Remove(area);
            }
            catch (BusinessException ex)
            {
                throw;
            }
            finally
            {
                IsLoading = false;
            }
        }

        partial void OnSearchAreaTextChanged(string value)
        {
            SearchAreaCommand.Execute(null);
        }

        [RelayCommand]
        private async Task AddArea()
        {
            var dialog = new AddEditAreaDialog
            {
                DataContext = new AddEditAreaViewModel(_generalInfoService)
            };

            var result = await DialogHost.Show(dialog, "RootDialog");
            if (result != null)
            {
                await LoadAreas();
            }
        }

        [RelayCommand]
        private async Task EditArea(AreaResponseDTO area)
        {
            if (area == null) return;

            var dialog = new AddEditAreaDialog
            {
                DataContext = new AddEditAreaViewModel(_generalInfoService, area)
            };

            var result = await DialogHost.Show(dialog, "RootDialog");
            if (result != null)
            {
                await LoadAreas();
            }
        }
    }
}
