using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core;
using Services.DTOs.ApartmentDTO;
using Services.DTOs.VehicleDTO;
using Services.Interfaces.ServiceSupervisorServices;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Forms.ViewModels.ServiceSupervisor
{
    public partial class RegisterParkingViewModel : ObservableObject
    {
        private readonly IVehicleService _vehicleService;


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

        partial void OnSelectedVehicleTypeChanged(string value)
        {
            // Disable license plate input for bicycles
            IsLicensePlateRequired = value != "Xe đạp";
            
            // Clear license plate if bicycle is selected
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
            // Skip validation for bicycles
            if (!IsLicensePlateRequired)
                return true;

            // Normalize input
            number = number.Replace(" ", "").ToUpper();
            string pattern = @"^\d{2}[-]?[A-Z]{1,2}[-]?\d{4,5}$";
            return Regex.IsMatch(number, pattern);
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

        [ObservableProperty]
        private string _ownerName = string.Empty;

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

        public RegisterParkingViewModel(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
            SearchCommand = new RelayCommand(SearchApartments);
            RegisterCommand = new RelayCommand(async () => await RegisterVehicle());

            LoadApartmentsAsync();
        }

        private async void SearchApartments()
        {
            var apartmentList = await _vehicleService.SearchApartmentsAsync(SearchText);
            Apartments = new ObservableCollection<ApartmentDTO>(apartmentList);
        }

        private async void LoadApartmentsAsync()
        {
            var apartmentList = await _vehicleService.GetAllApartmentsAsync();
            Apartments = new ObservableCollection<ApartmentDTO>(apartmentList);
        }


        [RelayCommand]
        private async Task RegisterVehicle()
        {
            try
            {
                if (!ValidateVehicle())
                    return;

                // Check for existing vehicle first
                var existingVehicle = await _vehicleService.GetVehicleByNumberAsync(VehicleNumber);
                if (existingVehicle != null)
                {
                    MessageBox.Show("Biển số xe này đã được đăng ký trong hệ thống!");
                    return;
                }

                var vehicle = new VehicleDTO
                {
                    VehicleNumber = VehicleNumber,
                    VehicleType = SelectedVehicleType,
                    VehicleOwner = SelectedApartment?.OwnerName ?? string.Empty,
                    ApartmentId = SelectedApartment?.ApartmentId ?? 0
                };

                await _vehicleService.RegisterVehicleAsync(vehicle);
                PaymentAmount = _vehicleService.CalculatePaymentAmount(SelectedVehicleType);
                IsPaymentCompleted = false;
                MessageBox.Show("Đăng ký xe thành công!");

                // Clear form after successful registration
                ClearForm();
            }
            catch (BusinessException bex)
            {
                MessageBox.Show(bex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đăng ký xe: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task ProcessPayment()
        {
            if (string.IsNullOrEmpty(SelectedPaymentMethod))
            {
                MessageBox.Show("Vui lòng chọn phương thức thanh toán!");
                return;
            }

            try
            {
                await Task.Delay(1000);

                IsPaymentCompleted = true;
                CanPrintCard = true;
                MessageBox.Show($"Thanh toán thành công!\nSố tiền: {PaymentAmount:N0} VNĐ\nPhương thức: {SelectedPaymentMethod}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thanh toán: {ex.Message}");
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
            //VehicleOwner = string.Empty;
            SelectedApartmentCode = string.Empty;
            PaymentAmount = 0;
            IsPaymentCompleted = false;
            CanPrintCard = false;
        }

    }
}