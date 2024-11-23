using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core;
using QRCoder;
using Services.DTOs.ApartmentDTO;
using Services.DTOs.VehicleDTO;
using Services.Interfaces.ServiceSupervisorServices;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

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

        [ObservableProperty]
        private bool _canProcessPayment = false;

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
                    VehicleOwner = VehicleOwner,
                    ApartmentId = SelectedApartment?.ApartmentId ?? 0,
                    RegisterDate = DateTime.Now,
                    Status = "Active"
                };

                var result = await _registerVehicleService.RegisterVehicleAsync(vehicle);
                
                if (result.Success)
                {
                    PaymentAmount = result.PaymentAmount;
                    CanProcessPayment = true;
                    // KHÔNG xóa form ở đây
                    // KHÔNG hiển thị message box thành công ở đây
                    MessageBox.Show("Đã tạo thông tin xe. Vui lòng thanh toán để hoàn tất đăng ký!");
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

        [RelayCommand]
        private async Task ProcessPaymentAsync()
        {
            if (!CanProcessPayment || string.IsNullOrEmpty(SelectedPaymentMethod) || string.IsNullOrEmpty(SelectedPaymentStatus)) 
            {
                MessageBox.Show("Vui lòng chọn phương thức và trạng thái thanh toán!");
                return;
            }

            if (SelectedPaymentStatus != "Đã thanh toán")
            {
                MessageBox.Show("Vui lòng xác nhận trạng thái đã thanh toán!");
                return;
            }

            try
            {
                IsProcessing = true;
                
                // Giả lập xử lý thanh toán
                await Task.Delay(1500); 

                // Kiểm tra điều kiện trước khi thực hiện animation và set trạng thái
                if (!IsProcessing) return;
                
                // Thực hiện animation
                var storyboard = Application.Current.MainWindow.FindResource("PaymentSuccessAnimation") as Storyboard;
                storyboard?.Begin();
                
                IsPaymentCompleted = true;
                CanPrintCard = true;
                
                MessageBox.Show("Thanh toán thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thanh toán: {ex.Message}");
                Debug.WriteLine($"Lỗi thanh toán: {ex}");
                IsPaymentCompleted = false;
                CanPrintCard = false;
            }
            finally
            {
                IsProcessing = false;
            }
        }

        // ...

        [RelayCommand]
        private async Task PrintCardAsync()
        {
            if (!CanPrintCard) return;

            try
            {
                PrintDialog dialog = new();
                if (dialog.ShowDialog() == true)
                {
                    var cardVisual = new DrawingVisual();
                    using (var context = cardVisual.RenderOpen())
                    {
                        // Card background
                        context.DrawRectangle(
                            new LinearGradientBrush(
                                Colors.White, 
                                Color.FromRgb(240, 240, 255), 
                                new Point(0, 0), 
                                new Point(1, 1)),
                            new Pen(Brushes.Gray, 2),
                            new Rect(0, 0, 400, 250));

                        // Header
                        var headerText = new FormattedText(
                            "THẺ XE - CHUNG CƯ ABC",
                            CultureInfo.CurrentCulture,
                            FlowDirection.LeftToRight,
                            new Typeface("Segoe UI Semibold"),
                            20,
                            Brushes.DarkBlue,
                            VisualTreeHelper.GetDpi(cardVisual).PixelsPerDip);
                        context.DrawText(headerText, new Point(20, 20));

                        // QR Code (using QRCoder library)
                        var qrGenerator = new QRCodeGenerator();
                        var qrData = $"VehicleID:{VehicleNumber}-{DateTime.Now:yyyyMMdd}";
                        var qrCode = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q);
                        var qrBmp = new BitmapImage();
                        // Convert QR to image and draw at position
                        context.DrawImage(qrBmp, new Rect(300, 20, 80, 80));

                        // Vehicle Info
                        var infoStyle = new Typeface("Segoe UI");
                        var infoBrush = Brushes.Black;
                        double y = 70;

                        DrawInfoLine(context, "Biển số xe:", VehicleNumber, y);
                        DrawInfoLine(context, "Loại xe:", SelectedVehicleType, y += 25);
                        DrawInfoLine(context, "Chủ xe:", VehicleOwner, y += 25);
                        DrawInfoLine(context, "Căn hộ:", SelectedApartmentCode, y += 25);
                        DrawInfoLine(context, "Ngày cấp:", DateTime.Now.ToString("dd/MM/yyyy"), y += 25);

                        // Card Number
                        var cardNumber = $"Số thẻ: {DateTime.Now:yyyyMMdd}{new Random().Next(1000, 9999)}";
                        DrawInfoLine(context, "", cardNumber, y += 40);

                        // Watermark
                        var watermark = new FormattedText(
                            "CHUNG CƯ ABC",
                            CultureInfo.CurrentCulture,
                            FlowDirection.LeftToRight,
                            new Typeface("Arial"),
                            60,
                            new SolidColorBrush(Color.FromArgb(30, 0, 0, 0)),
                            VisualTreeHelper.GetDpi(cardVisual).PixelsPerDip);
                        context.PushTransform(new RotateTransform(45, 200, 125));
                        context.DrawText(watermark, new Point(50, 80));
                        context.Pop();
                    }

                    dialog.PrintVisual(cardVisual, "Vehicle Card");
                    MessageBox.Show("In thẻ xe thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi in thẻ: {ex.Message}");
            }
        }

        private void DrawInfoLine(DrawingContext context, string label, string value, double y)
        {
            var labelText = new FormattedText(
                label,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Segoe UI"),
                12,
                Brushes.Gray,
                VisualTreeHelper.GetDpi(new DrawingVisual()).PixelsPerDip);

            var valueText = new FormattedText(
                value,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Segoe UI Semibold"),
                12,
                Brushes.Black,
                VisualTreeHelper.GetDpi(new DrawingVisual()).PixelsPerDip);

            context.DrawText(labelText, new Point(20, y));
            context.DrawText(valueText, new Point(100, y));
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