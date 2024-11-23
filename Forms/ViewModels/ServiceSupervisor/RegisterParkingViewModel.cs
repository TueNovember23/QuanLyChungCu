using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core;
using Services.DTOs.ApartmentDTO;
using Services.DTOs.VehicleDTO;
using Services.Interfaces.ServiceSupervisorServices;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;
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
        private bool _isPaymentCompleted = false;

        [ObservableProperty]
        private bool _canPrintCard = false;

        [ObservableProperty]
        private float _paymentAmount;

        [ObservableProperty]
        private ObservableCollection<string> _paymentMethods = new() { "Tiền mặt", "Ví điện tử" };

        [ObservableProperty]
        private string _selectedPaymentMethod = string.Empty;

        [ObservableProperty]
        private ObservableCollection<string> _paymentStatuses = new() { "Đã thanh toán", "Chưa thanh toán" };

        [ObservableProperty]
        private string _selectedPaymentStatus = string.Empty;

        public ICommand SearchCommand { get; }
        public ICommand RegisterCommand { get; }

        private bool ValidateForm()
        {
            if (string.IsNullOrEmpty(SelectedVehicleType))
            {
                MessageBox.Show("Vui lòng chọn loại xe!");
                return false;
            }

            if (IsLicensePlateRequired && !ValidateVehicleNumber(VehicleNumber))
            {
                MessageBox.Show("Biển số xe không hợp lệ!");
                return false;
            }

            if (SelectedApartment == null)
            {
                MessageBox.Show("Vui lòng chọn căn hộ!");
                return false;
            }

            if (string.IsNullOrEmpty(VehicleOwner))
            {
                MessageBox.Show("Vui lòng nhập tên chủ xe!");
                return false;
            }

            return true;
        }

        partial void OnSelectedVehicleTypeChanged(string value)
        {
            IsLicensePlateRequired = value != "Xe đạp";

            if (!IsLicensePlateRequired)
            {
                VehicleNumber = string.Empty;
            }
        }

        private bool ValidateVehicle()
        {
            if (!ValidateVehicleNumber(VehicleNumber))
            {
                MessageBox.Show("Biển số xe không hợp lệ!");
                return false;
            }

            if (SelectedApartment == null)
            {
                MessageBox.Show("Vui lòng chọn căn hộ!");
                return false;
            }

            return true;
        }

        private bool ValidateVehicleNumber(string number)
        {
            if (!IsLicensePlateRequired)
                return true;

            if (string.IsNullOrWhiteSpace(number))
                return false;

            number = number.Replace(" ", "").Replace("-", "").ToUpper();

            string pattern = @"^\d{2}[A-Z]{1,2}\d{4,5}$";

            var isValid = Regex.IsMatch(number, pattern);
            Debug.WriteLine($"Xác nhận biển số xe: {number} -> {isValid}");
            return isValid;
        }

        [ObservableProperty]
        private string _selectedApartmentCode = string.Empty;


        partial void OnSelectedApartmentChanged(ApartmentDTO? value)
        {
            if (value != null)
            {
                SelectedApartmentCode = value.ApartmentCode;

            }
        }


        public RegisterParkingViewModel(IRegisterVehicleService registerVehicleService)
        {
            _registerVehicleService = registerVehicleService;
            SearchCommand = new RelayCommand(SearchApartments);
            RegisterCommand = new RelayCommand(async () => await RegisterVehicleAsync()); // Sửa ở đây

            LoadApartmentsAsync();
        }

        private async void SearchApartments()
        {
            var apartmentList = await _registerVehicleService.SearchApartmentsAsync(SearchText);
            Apartments = new ObservableCollection<ApartmentDTO>(apartmentList);
        }

        private async void LoadApartmentsAsync()
        {
            var apartmentList = await _registerVehicleService.GetAllApartmentsAsync();
            Apartments = new ObservableCollection<ApartmentDTO>(apartmentList);
        }


        private async Task RegisterVehicleAsync()
        {
            if (IsProcessing || !ValidateForm()) return;

            try
            {
                IsProcessing = true;

                var normalizedNumber = !IsLicensePlateRequired ? string.Empty :
                                        VehicleNumber.Trim().Replace(" ", "").Replace("-", "").ToUpper();

                var existingVehicle = await _registerVehicleService.GetVehicleByNumberAsync(normalizedNumber);
                if (existingVehicle != null)
                {
                    MessageBox.Show($"Biển số xe {VehicleNumber} đã được đăng ký!");
                    return;
                }

                var vehicle = new VehicleDTO
                {
                    VehicleNumber = normalizedNumber,
                    VehicleType = SelectedVehicleType,
                    VehicleOwner = VehicleOwner, // Sử dụng input VehicleOwner
                    ApartmentId = SelectedApartment?.ApartmentId ?? 0,
                    RegisterDate = DateTime.Now,
                    Status = "Active"
                };

                await _registerVehicleService.RegisterVehicleAsync(vehicle);

                PaymentAmount = _registerVehicleService.CalculatePaymentAmount(SelectedVehicleType);
                IsPaymentCompleted = false;
                CanPrintCard = false;

                MessageBox.Show("Đăng ký xe thành công!");
                ClearForm();
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

        [RelayCommand]
        private async Task ProcessPaymentAsync()
        {
            if (IsProcessing || string.IsNullOrEmpty(SelectedPaymentMethod))
            {
                MessageBox.Show("Vui lòng chọn phương thức thanh toán!");
                return;
            }

            if (string.IsNullOrEmpty(SelectedPaymentStatus))
            {
                MessageBox.Show("Vui lòng chọn trạng thái thanh toán!");
                return;
            }

            try
            {
                IsProcessing = true;
                // Thêm xử lý thanh toán thực tế
                IsPaymentCompleted = true;
                CanPrintCard = true;
                MessageBox.Show("Thanh toán thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thanh toán!");
                Debug.WriteLine($"Lỗi thanh toán: {ex}");
                IsPaymentCompleted = false;
                CanPrintCard = false;
            }
            finally
            {
                IsProcessing = false;
            }
        }


        [RelayCommand]
        private void PrintCard()
        {
            if (!IsPaymentCompleted)
            {
                MessageBox.Show("Vui lòng thanh toán trước khi in thẻ xe!");
                return;
            }

            try
            {
                MessageBox.Show("In thẻ xe thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi in thẻ: {ex.Message}");
            }
        }

        private void ClearForm()
        {
            VehicleNumber = string.Empty;
            SelectedVehicleType = string.Empty;
            VehicleOwner = string.Empty; // Uncomment this
            SelectedApartment = null; // Add this
            SelectedApartmentCode = string.Empty;
            PaymentAmount = 0;
            IsPaymentCompleted = false;
            CanPrintCard = false;
            SelectedPaymentMethod = string.Empty; // Add this
            SelectedPaymentStatus = string.Empty; // Add this
        }

    }
}