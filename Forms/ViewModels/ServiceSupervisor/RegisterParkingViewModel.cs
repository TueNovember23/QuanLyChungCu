using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core;
using Services.DTOs.ApartmentDTO;
using Services.DTOs.VehicleDTO;
using Services.Interfaces.ServiceSupervisorServices;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Forms.ViewModels.ServiceSupervisor
{
    public partial class RegisterParkingViewModel : ObservableObject
    {
        private readonly IRegisterVehicleService _registerVehicleService;

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
        private const string ERROR_MAX_VEHICLES = "Căn hộ đã đăng ký tối đa 10 xe!";
        private const string ERROR_PROCESSING = "Đang xử lý, vui lòng đợi!";

        public RegisterParkingViewModel(IRegisterVehicleService registerVehicleService)
        {
            _registerVehicleService = registerVehicleService;
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

            if (string.IsNullOrWhiteSpace(VehicleNumber))
            {
                MessageBox.Show(ERROR_LICENSE_PLATE_REQUIRED);
                return false;
            }

            string normalizedNumber = VehicleNumber.Trim().Replace(" ", "").Replace("-", "").ToUpper();

            if (normalizedNumber.Length > 12)
            {
                MessageBox.Show(ERROR_LICENSE_PLATE_TOO_LONG);
                return false;
            }

            string pattern = @"^\d{2}[A-Z]{1,2}\d{4,6}$";
            if (!Regex.IsMatch(normalizedNumber, pattern))
            {
                MessageBox.Show(ERROR_LICENSE_PLATE_INVALID);
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

                var normalizedNumber = !IsLicensePlateRequired ? string.Empty :
                                    VehicleNumber.Trim().Replace(" ", "").Replace("-", "").ToUpper();

                var existingVehicle = await _registerVehicleService.GetVehicleByNumberAsync(normalizedNumber);
                if (existingVehicle != null)
                {
                    MessageBox.Show(ERROR_LICENSE_PLATE_EXISTS);
                    return;
                }

                var vehicle = new VehicleDTO
                {
                    VehicleNumber = normalizedNumber,
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
                    ClearForm();
                }
                else
                {
                    MessageBox.Show(result.Message);
                }
            }
            catch (BusinessException bex)
            {
                MessageBox.Show($"Lỗi nghiệp vụ: {bex.Message}");
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
            SelectedVehicleType = string.Empty;
            VehicleOwner = string.Empty;
            SelectedApartment = null;
            SelectedApartmentCode = string.Empty;
            IsLicensePlateRequired = true;
        }
    }
}