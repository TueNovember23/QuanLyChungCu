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
using System.Threading;

namespace Forms.ViewModels.Accountant
{
    public partial class ViolationViewModel : ObservableObject
    {
        private readonly IViolationService _violationService;
        private readonly IApartmentService _apartmentService;
        private readonly IRegulationService _regulationService;

        private CancellationTokenSource? _searchCancellationToken;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _searchText = string.Empty;

        partial void OnSearchTextChanged(string value)
        {
            _searchCancellationToken?.Cancel();
            _searchCancellationToken = new CancellationTokenSource();

            _ = SearchWithDebounceAsync(_searchCancellationToken.Token);
        }

        private async Task SearchWithDebounceAsync(CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay(300, cancellationToken);
                await Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    await FilterViolationsAsync();
                });
            }
            catch (OperationCanceledException)
            {
                // Search was cancelled, ignore
            }
        }

        private async Task FilterViolationsAsync()
        {
            if (IsLoading) return;

            try 
            {
                IsLoading = true;
                
                IEnumerable<ViolationResponseDTO> searchResults;
                
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    searchResults = await _violationService.SearchAsync(SearchText);
                }
                else 
                {
                    searchResults = await _violationService.GetAllAsync();
                }
                
                if (!string.IsNullOrEmpty(FilterStatus))
                {
                    searchResults = searchResults.Where(v => 
                        v.LatestProcessingStatus == FilterStatus).ToList();
                }
                
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Violations = new ObservableCollection<ViolationResponseDTO>(searchResults);
                });
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}");
                });
            }
            finally
            {
                IsLoading = false;
            }
        }

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

                Violations = new ObservableCollection<ViolationResponseDTO>(
                    await _violationService.GetAllAsync());

                Apartments = new ObservableCollection<ResponseApartmentDTO>(
                    await _apartmentService.GetAllApartmentForViolation());

                // Chỉ lấy regulations đang active
                var activeRegulations = await _regulationService.GetAllAsync();
                Regulations = new ObservableCollection<RegulationResponseDTO>(
                    activeRegulations.Where(r => r.IsActive)
                );
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
                    var result = await _violationService.CreateAsync(dto);
                    Violations.Add(result);
                }
                else
                {
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
                if (Fine > 100000000) 
                    throw new BusinessException("Số tiền phạt không được vượt quá 100 triệu");

                if (string.IsNullOrEmpty(PenaltyMethod))
                    throw new BusinessException("Vui lòng nhập phương án xử lý");

                if (string.IsNullOrEmpty(SelectedProcessingStatus))
                    throw new BusinessException("Vui lòng chọn trạng thái xử lý");

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
                await LoadDataAsync();
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
            
            var result = MessageBox.Show(
                "Bạn có chắc muốn sửa thông tin xử lý này?",
                "Xác nhận", 
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                await LoadPenaltyForEdit(SelectedPenaltyHistory);
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
    }
}
