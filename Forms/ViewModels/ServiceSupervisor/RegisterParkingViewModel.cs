using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core;
using Services.DTOs.ApartmentDTO;
using Services.DTOs.VehicleDTO;
using Services.Interfaces.ServiceSupervisorServices;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Forms.ViewModels.ServiceSupervisor
{
    public partial class RegisterParkingViewModel : ObservableObject
    {
        private readonly IRegisterVehicleService _registerVehicleService;
        private readonly IParkingService _parkingService;
            private CancellationTokenSource? _loadDataCancellationToken;

        [ObservableProperty]
        private ObservableCollection<ApartmentDTO> _apartments = new();

        [ObservableProperty]
        private ApartmentDTO? _selectedApartment;

        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private ObservableCollection<string> _vehicleTypes = new() { "Xe đạp", "Xe máy", "Ô tô", "Xe máy điện", "Ô tô điện" };

        [ObservableProperty]
        private string _selectedVehicleType = string.Empty;

        [ObservableProperty]
        private string _vehicleNumber = string.Empty;

        [ObservableProperty]
        private bool _isLicensePlateRequired = true;

        [ObservableProperty]
        private bool _isProcessing = false;

        [ObservableProperty]
        private string _vehicleOwner = string.Empty;

        [ObservableProperty]
        private string _selectedApartmentCode = string.Empty;

        [ObservableProperty]
        private ObservableCollection<ParkingSpaceDTO> _parkingSpaces = new();

        [ObservableProperty]
        private VehicleLimitDTO? _apartmentLimits;

        public ICommand SearchCommand { get; }
        public ICommand RegisterCommand { get; }

        private const string ERROR_VEHICLE_TYPE_REQUIRED = "Vui lòng chọn loại xe!";
        private const string ERROR_LICENSE_PLATE_REQUIRED = "Vui lòng nhập biển số xe!";
        private const string ERROR_LICENSE_PLATE_INVALID = "Biển số xe không hợp lệ!";
        private const string ERROR_LICENSE_PLATE_TOO_LONG = "Biển số xe không được vượt quá 12 ký tự!";
        private const string ERROR_LICENSE_PLATE_EXISTS = "Biển số xe đã được đăng ký!";
        private const string ERROR_APARTMENT_REQUIRED = "Vui lòng chọn căn hộ!";
        private const string ERROR_APARTMENT_ID_INVALID = "Mã căn hộ không hợp lệ!";
        private const string ERROR_OWNER_NAME_REQUIRED = "Vui lòng nhập tên chủ xe!";
        private const string ERROR_OWNER_NAME_TOO_LONG = "Tên chủ xe không được vượt quá 50 ký tự!";
        private const string ERROR_OWNER_NAME_INVALID = "Tên chủ xe không hợp lệ! Chỉ được phép nhập chữ cái và khoảng trắng.";
        private const string ERROR_MAX_VEHICLES = "Căn hộ đã đăng ký tối đa 5 xe!";
        private const string ERROR_PROCESSING = "Đang xử lý, vui lòng đợi!";

        public RegisterParkingViewModel(IRegisterVehicleService registerVehicleService, IParkingService parkingService)
        {
            _registerVehicleService = registerVehicleService;
            _parkingService = parkingService;
            SearchCommand = new RelayCommand(SearchApartments);
            RegisterCommand = new RelayCommand(async () => await RegisterVehicleAsync());
            LoadApartmentsAsync();
        }

        private async void SearchApartments()
        {
            try
            {
                IsProcessing = true;
                var apartmentList = await _registerVehicleService.SearchApartmentsAsync(SearchText);
                Apartments = new ObservableCollection<ApartmentDTO>(apartmentList);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm căn hộ: {ex.Message}");
            }
            finally
            {
                IsProcessing = false;
            }
        }

        private async void LoadApartmentsAsync()
        {
            try
            {
                IsProcessing = true;
                var apartmentList = await _registerVehicleService.GetAllApartmentsAsync();
                Apartments = new ObservableCollection<ApartmentDTO>(apartmentList);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách căn hộ: {ex.Message}");
            }
            finally
            {
                IsProcessing = false;
            }
        }

        private async Task LoadParkingSpacesAsync()
        {
            try
            {
                var spaces = await _parkingService.GetParkingSpacesAsync();
                ParkingSpaces = new ObservableCollection<ParkingSpaceDTO>(spaces);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading parking spaces: {ex.Message}");
            }
        }

        private async Task LoadApartmentLimitsAsync(int apartmentId)
        {
            try
            {
                ApartmentLimits = await _parkingService.GetVehicleLimitsByApartmentAsync(apartmentId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading apartment limits: {ex.Message}");
            }
        }

        private async Task ValidateVehicleRegistrationAsync()
        {
            if (SelectedApartment == null || string.IsNullOrEmpty(SelectedVehicleType))
                return;

            var isValid = await _parkingService.ValidateVehicleRegistrationAsync(
                SelectedVehicleType, 
                SelectedApartment.ApartmentId);

            if (!isValid)
            {
                throw new BusinessException("Exceeded vehicle limits for this apartment or no available parking spaces");
            }
        }

        partial void OnSelectedVehicleTypeChanged(string value)
        {
            IsLicensePlateRequired = value != "Xe đạp";
            if (!IsLicensePlateRequired)
            {
                VehicleNumber = string.Empty;
            }
        }

        partial void OnSelectedApartmentChanged(ApartmentDTO? value)
        {
            if (value != null)
            {
                SelectedApartmentCode = value.ApartmentCode;

                _loadDataCancellationToken?.Cancel();
                _loadDataCancellationToken = new CancellationTokenSource();
                _ = LoadParkingDataAsync(value.ApartmentId, _loadDataCancellationToken.Token);
            }
        }

        private async Task LoadParkingDataAsync(int apartmentId, CancellationToken cancellationToken)
        {
            try
            {
                IsProcessing = true;

                var parkingData = await _parkingService.GetParkingDataAsync(
                    apartmentId,
                    cancellationToken
                );

                if (!cancellationToken.IsCancellationRequested)
                {
                    ParkingSpaces = new ObservableCollection<ParkingSpaceDTO>(parkingData.spaces);
                    ApartmentLimits = parkingData.limits;
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải thông tin: {ex.Message}");
            }
            finally
            {
                IsProcessing = false;
            }
        }

        private bool ValidateForm()
        {
            if (IsProcessing)
            {
                MessageBox.Show(ERROR_PROCESSING);
                return false;
            }

            if (string.IsNullOrWhiteSpace(SelectedVehicleType))
            {
                MessageBox.Show(ERROR_VEHICLE_TYPE_REQUIRED);
                return false;
            }

            if (!ValidateApartment())
                return false;

            if (!ValidateLicensePlate())
                return false;

            if (!ValidateOwnerInformation())
                return false;

            return true;
        }

        private bool ValidateApartment()
        {
            if (SelectedApartment == null)
            {
                MessageBox.Show(ERROR_APARTMENT_REQUIRED);
                return false;
            }

            if (SelectedApartment.ApartmentId <= 0)
            {
                MessageBox.Show(ERROR_APARTMENT_ID_INVALID);
                return false;
            }

            return true;
        }

        private bool ValidateLicensePlate()
        {
            if (!IsLicensePlateRequired)
                return true;
                
            return ValidateLicensePlate(VehicleNumber, SelectedVehicleType);
        }

        private bool ValidateLicensePlate(string? licensePlate, string vehicleType) 
        {
            if (vehicleType == "Xe đạp")
                return true;

            if (string.IsNullOrWhiteSpace(licensePlate))
            {
                MessageBox.Show("Vui lòng nhập biển số xe!");
                return false;  
            }

            var normalized = licensePlate.Trim()
                                    .Replace(" ", "")
                                    .Replace("-", "")
                                    .ToUpper();

            if (normalized.Length > 12)
            {
                MessageBox.Show("Biển số xe không được vượt quá 12 ký tự!");
                return false;
            }

            // Regex kiểm tra format: 2 số - 1-2 chữ - 4-6 số
            string pattern = @"^\d{2}[A-Z]{1,2}\d{4,6}$";
            if (!Regex.IsMatch(normalized, pattern))
            {
                MessageBox.Show("Biển số xe không hợp lệ!");
                return false;
            }

            return true;
        }

        private bool ValidateOwnerInformation()
        {
            if (string.IsNullOrWhiteSpace(VehicleOwner))
            {
                MessageBox.Show(ERROR_OWNER_NAME_REQUIRED);
                return false;
            }

            var normalizedName = VehicleOwner.Trim();
            
            if (normalizedName.Length > 50)
            {
                MessageBox.Show(ERROR_OWNER_NAME_TOO_LONG);
                return false;
            }

            string pattern = @"^[a-zA-ZÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơưăạảấầẩẫậắằẳẵặẹẻẽềềểễệỉịọỏốồổỗộớờởỡợụủứừửữựỳỵỷỹ\s]+$";
            if (!Regex.IsMatch(normalizedName, pattern))
            {
                MessageBox.Show(ERROR_OWNER_NAME_INVALID);
                return false;
            }

            return true;
        }

        private async Task RegisterVehicleAsync()
        {
            if (!ValidateForm()) return;

            try
            {
                IsProcessing = true;

                await ValidateVehicleLimitsAsync();

                if (IsLicensePlateRequired)  
                {
                    var normalizedNumber = VehicleNumber.Trim().Replace(" ", "").Replace("-", "").ToUpper();
                    var existingVehicle = await _registerVehicleService.GetVehicleByNumberAsync(normalizedNumber);
                    if (existingVehicle != null)
                    {
                        MessageBox.Show(ERROR_LICENSE_PLATE_EXISTS);
                        return;
                    }
                }

                var vehicle = new VehicleDTO
                {
                    VehicleNumber = IsLicensePlateRequired ? 
                        FormatLicensePlate(VehicleNumber) :  
                        $"{SelectedApartment.ApartmentId}_Xedap",
                    VehicleType = SelectedVehicleType,
                    VehicleOwner = VehicleOwner.Trim(),
                    ApartmentId = SelectedApartment?.ApartmentId ?? 0,
                    RegisterDate = DateTime.Now,
                    Status = "Đang gửi"
                };

                var result = await _registerVehicleService.RegisterVehicleAsync(vehicle);
                
                if (result.Success)
                {
                    MessageBox.Show("Đăng ký xe thành công!");
                    if (SelectedApartment != null)
                    {
                        await LoadParkingDataAsync(SelectedApartment.ApartmentId, CancellationToken.None);
                    }
                    ClearForm();
                }
                else
                {
                    MessageBox.Show(result.Message);
                }
            }
            catch (BusinessException bex)
            {
                MessageBox.Show($"{bex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi đăng ký xe!");
                Debug.WriteLine($"Lỗi khi đăng ký: {ex}");
            }
            finally
            {
                IsProcessing = false;
            }
        }

        private void ClearForm()
        {
            SearchText = string.Empty;
            VehicleNumber = string.Empty;
            VehicleOwner = string.Empty;
            IsLicensePlateRequired = true;
        }

        private string FormatLicensePlate(string licensePlate)
        {
            if (string.IsNullOrWhiteSpace(licensePlate))
                return string.Empty;
                
            var normalized = licensePlate.Trim()
                                    .Replace(" ", "")
                                    .Replace("-", "")
                                    .ToUpper();
                                    
            // Validate format trước khi format
            if (!Regex.IsMatch(normalized, @"^\d{2}[A-Z]{1,2}\d{4,6}$"))
                return normalized;

            try {
                var numbers = normalized.Substring(0, 2);
                var letters = normalized.Substring(2, normalized.Length - 6);
                var lastNumbers = normalized.Substring(normalized.Length - 4);
                return $"{numbers}-{letters}-{lastNumbers}";
            }
            catch {
                return normalized; 
            }
        }

        private async Task ValidateVehicleLimitsAsync()
        {
            if (SelectedApartment == null || string.IsNullOrEmpty(SelectedVehicleType))
                return;

            var limits = await _parkingService.GetVehicleLimitsByApartmentAsync(SelectedApartment.ApartmentId);

            switch (SelectedVehicleType)
            {
                case "Xe đạp" when limits.CurrentBicycles >= limits.MaxBicycles:
                    throw new BusinessException("Căn hộ đã đạt giới hạn xe đạp");

                case "Xe máy" or "Xe máy điện" when limits.CurrentMotorcycles >= limits.MaxMotorcycles:
                    throw new BusinessException("Căn hộ đã đạt giới hạn xe máy/xe máy điện");

                case "Ô tô" or "Ô tô điện" when limits.CurrentCars >= limits.MaxCars:
                    throw new BusinessException("Căn hộ đã đạt giới hạn ô tô/ô tô điện");
            }
        }

        [RelayCommand]
        private void FormatLicensePlate()
        {
            VehicleNumber = FormatLicensePlateText(VehicleNumber);
        }

        partial void OnVehicleNumberChanged(string value)
        {
            if (!IsLicensePlateRequired) return;
    
            if (string.IsNullOrWhiteSpace(value)) return;

            var formatted = FormatLicensePlateText(value);
            if (formatted != value)
            {
                VehicleNumber = formatted;
            }
        }

        private string FormatLicensePlateText(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            var normalized = input.Replace(" ", "").Replace("-", "").ToUpper();

            if (normalized.Length < 4)
                return normalized;

            try
            {
                var result = new StringBuilder();
        
                if (normalized.Length >= 2)
                {
                    result.Append(normalized.Substring(0, 2));
                }

                if (normalized.Length > 2)
                {
                    result.Append('-');
                }

                var letterStart = 2;
                var letterCount = 0;
                while (letterStart + letterCount < normalized.Length && 
                       char.IsLetter(normalized[letterStart + letterCount]) && 
                       letterCount < 2)
                {
                    letterCount++;
                }

                if (letterCount > 0)
                {
                    result.Append(normalized.Substring(letterStart, letterCount));
                }

                if (letterStart + letterCount < normalized.Length)
                {
                    result.Append('-');
                }

                if (letterStart + letterCount < normalized.Length)
                {
                    result.Append(normalized.Substring(letterStart + letterCount));
                }

                return result.ToString();
            }
            catch
            {
                return normalized;
            }
        }
    }
}