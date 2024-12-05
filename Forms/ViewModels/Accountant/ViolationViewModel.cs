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

        [ObservableProperty]
        private string _selectedPenaltyLevel = string.Empty;

        [ObservableProperty]
        private decimal _fine;

        [ObservableProperty]
        private string _penaltyMethod = string.Empty;

        [ObservableProperty]
        private string _selectedProcessingStatus = "Chờ xử lý";

        [ObservableProperty]
        private ObservableCollection<ViolationPenaltyDTO> _penaltyHistory = new();

        [ObservableProperty]
        private ObservableCollection<string> _penaltyLevels = new(new[] { "Nhẹ", "Trung bình", "Nặng" });

        [ObservableProperty]
        private ObservableCollection<string> _processingStatuses = new(new[] { "Chờ xử lý", "Đang xử lý", "Đã xử lý" });

        [ObservableProperty]
        private string _filterStatus = string.Empty;

        [ObservableProperty]
        private ViolationPenaltyDTO? _selectedPenaltyHistory;

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
                
                if (!string.IsNullOrEmpty(FilterStatus))
                {
                    results = results.Where(v => v.LatestProcessingStatus == FilterStatus);
                }
                
                Violations = new ObservableCollection<ViolationResponseDTO>(results);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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

        [RelayCommand]
        private async Task SavePenaltyAsync()
        {
            try
            {
                if (SelectedViolation == null)
                    throw new BusinessException("Vui lòng chọn vi phạm cần xử lý");

                if (string.IsNullOrEmpty(SelectedPenaltyLevel))
                    throw new BusinessException("Vui lòng chọn mức độ xử phạt");

                if (Fine <= 0)
                    throw new BusinessException("Số tiền phạt phải lớn hơn 0");

                if (string.IsNullOrEmpty(PenaltyMethod))
                    throw new BusinessException("Vui lòng nhập phương án xử lý");

                IsLoading = true;

                var penaltyDto = new ViolationPenaltyDTO
                {
                    PenaltyId = SelectedPenaltyHistory?.PenaltyId ?? 0,
                    ViolationId = SelectedViolation.ViolationId,
                    PenaltyLevel = SelectedPenaltyLevel,
                    Fine = Fine,
                    PenaltyMethod = PenaltyMethod,
                    ProcessingStatus = SelectedProcessingStatus,
                    ProcessedDate = DateTime.Now
                };

                if (SelectedPenaltyHistory != null)
                    await _violationService.UpdatePenaltyAsync(penaltyDto);
                else
                    await _violationService.SavePenaltyAsync(penaltyDto);

                await LoadPenaltyHistoryAsync(SelectedViolation.ViolationId);
                ResetPenaltyForm();
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

        private void ResetPenaltyForm()
        {
            SelectedPenaltyLevel = string.Empty;
            Fine = 0;
            PenaltyMethod = string.Empty;
            SelectedProcessingStatus = "Chờ xử lý";
        }

        private async Task LoadPenaltyHistoryAsync(int violationId)
        {
            try
            {
                var history = await _violationService.GetPenaltyHistoryAsync(violationId);
                PenaltyHistory = new ObservableCollection<ViolationPenaltyDTO>(history);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải lịch sử xử lý: {ex.Message}");
            }
        }

        private async Task LoadPenaltyForEdit(ViolationPenaltyDTO penalty)
        {
            SelectedPenaltyLevel = penalty.PenaltyLevel;
            Fine = penalty.Fine;
            PenaltyMethod = penalty.PenaltyMethod;
            SelectedProcessingStatus = penalty.ProcessingStatus;
        }

        [RelayCommand]
        private async Task EditPenaltyAsync()
        {
            if (SelectedPenaltyHistory == null) return;
            await LoadPenaltyForEdit(SelectedPenaltyHistory);
        }

        partial void OnSelectedViolationChanged(ViolationResponseDTO? value)
        {
            if (value != null)
            {
                SelectedApartment = Apartments.FirstOrDefault(a => a.ApartmentCode == value.ApartmentCode);
                SelectedRegulation = Regulations.FirstOrDefault(r => r.Title == value.RegulationTitle);
                ViolationDetail = value.Detail ?? string.Empty;
                ViolationDate = value.CreatedDate;
                _ = LoadPenaltyHistoryAsync(value.ViolationId);
            }
            else
            {
                ResetPenaltyForm();
                PenaltyHistory.Clear();
            }
        }

        partial void OnFilterStatusChanged(string value)
        {
            if (string.IsNullOrEmpty(value)) return;
            FilterViolationsAsync().ConfigureAwait(false);
        }

        private async Task FilterViolationsAsync()
        {
            try 
            {
                IsLoading = true;
                var allViolations = await _violationService.GetAllAsync();
                var filtered = allViolations.AsEnumerable();
                
                if (!string.IsNullOrEmpty(SearchText))
                {
                    filtered = filtered.Where(v => 
                        v.ApartmentCode.Contains(SearchText, StringComparison.OrdinalIgnoreCase) || 
                        v.RegulationTitle.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
                }
                
                if (!string.IsNullOrEmpty(FilterStatus))
                {
                    // Lọc theo trạng thái xử lý mới nhất
                    filtered = filtered.Where(v => 
                        v.LatestProcessingStatus == FilterStatus);
                }
                
                Violations = new ObservableCollection<ViolationResponseDTO>(filtered);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
