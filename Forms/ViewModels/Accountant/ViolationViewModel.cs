using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core;
using Repositories.Repositories.Entities;
using Services.DTOs.ApartmentDTO;
using Services.DTOs.RegulationDTO;
using Services.DTOs.ViolationDTO;
using Services.Interfaces.AccountantServices;
using Services.Interfaces.AdministrativeStaffServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Forms.ViewModels.Accountant
{
    public partial class ViolationViewModel : ObservableObject
    {
        private readonly IViolationService _violationService;
        private readonly IApartmentService _apartmentService;
        private readonly IRegulationService _regulationService;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private ObservableCollection<ViolationResponseDTO> _violations = new();

        [ObservableProperty]
        private ViolationResponseDTO? _selectedViolation;

        [ObservableProperty]
        private ObservableCollection<ResponseApartmentDTO> _apartments = new();

        [ObservableProperty]
        private ObservableCollection<RegulationResponseDTO> _regulations = new();

        [ObservableProperty]
        private ResponseApartmentDTO? _selectedApartment;

        [ObservableProperty]
        private RegulationResponseDTO? _selectedRegulation;

        [ObservableProperty]
        private string _violationDetail = string.Empty;

        [ObservableProperty]
        private DateTime _violationDate = DateTime.Today;

        public ViolationViewModel(
            IViolationService violationService,
            IApartmentService apartmentService,
            IRegulationService regulationService)
        {
            _violationService = violationService;
            _apartmentService = apartmentService;
            _regulationService = regulationService;

            LoadDataAsync().ConfigureAwait(false);
        }

        private async Task LoadDataAsync()
        {
            try
            {
                IsLoading = true;

                // Load data sequentially instead of using Task.WhenAll
                Violations = new ObservableCollection<ViolationResponseDTO>(
                    await _violationService.GetAllAsync());

                Apartments = new ObservableCollection<ResponseApartmentDTO>(
                    await _apartmentService.GetAllApartmentForViolation());

                Regulations = new ObservableCollection<RegulationResponseDTO>(
                    await _regulationService.GetAllAsync());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task SearchAsync()
        {
            try
            {
                IsLoading = true;
                var results = await _violationService.SearchAsync(SearchText);
                Violations = new ObservableCollection<ViolationResponseDTO>(results);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task LoadRegulationsAsync()
        {
            await LoadDataAsync();
        }

        [RelayCommand]
        private void CreateNew()
        {
            SelectedViolation = null;
            SelectedApartment = null;
            SelectedRegulation = null;
            ViolationDetail = string.Empty;
            ViolationDate = DateTime.Today;
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            try
            {
                if (SelectedApartment == null || SelectedRegulation == null)
                {
                    throw new BusinessException("Vui lòng chọn đầy đủ thông tin");
                }

                IsLoading = true;

                var dto = new CreateViolationDTO
                {
                    ApartmentId = SelectedApartment.ApartmentId,
                    RegulationId = SelectedRegulation.RegulationId,
                    CreatedDate = ViolationDate,
                    Detail = ViolationDetail
                };

                if (SelectedViolation == null)
                {
                    // Create new
                    var result = await _violationService.CreateAsync(dto);
                    Violations.Add(result);
                }
                else
                {
                    // Update existing
                    var result = await _violationService.UpdateAsync(dto, SelectedViolation.ViolationId);
                    var index = Violations.IndexOf(SelectedViolation);
                    if (index != -1)
                    {
                        Violations[index] = result;
                    }
                }

                CreateNew();
            }
            catch (BusinessException bex)
            {
                MessageBox.Show(bex.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        partial void OnSelectedViolationChanged(ViolationResponseDTO? value)
        {
            if (value != null)
            {
                SelectedApartment = Apartments.FirstOrDefault(a => a.ApartmentCode == value.ApartmentCode);
                SelectedRegulation = Regulations.FirstOrDefault(r => r.Title == value.RegulationTitle);
                ViolationDetail = value.Detail ?? string.Empty;
                ViolationDate = value.CreatedDate;
            }
        }
    }
}
