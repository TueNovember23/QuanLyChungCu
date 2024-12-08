using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Services.DTOs.VehicleDTO;
using Services.Interfaces.ServiceSupervisorServices;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Threading;
using System.Text.RegularExpressions;

namespace Forms.ViewModels.ServiceSupervisor
{
    public partial class ParkingViewModel : ObservableObject
    {
        private readonly IParkingService _parkingService;

        [ObservableProperty]
        private ObservableCollection<VehicleResponseDTO> _vehicles = new();

        [ObservableProperty]
        private int _bicycleCount;
        [ObservableProperty]
        private int _motorcycleCount;
        [ObservableProperty]
        private int _carCount;

        [ObservableProperty]
        private VehicleResponseDTO? _selectedVehicle;

        [ObservableProperty]
        private string _selectedApartmentCode = string.Empty;

        [ObservableProperty]
        private string _selectedVehicleType = string.Empty;

        [ObservableProperty]
        private string _selectedVehicleNumber = string.Empty;

        [ObservableProperty]
        private string _selectedVehicleOwner = string.Empty;

        [ObservableProperty]
        private string _selectedVehicleStatus = string.Empty;

        [ObservableProperty]
        private ObservableCollection<string> _vehicleStatuses = new() { "Đang gửi", "Huỷ gửi" };

        [ObservableProperty]
        private ObservableCollection<string> _vehicleTypes = new()
        { "Xe đạp", "Xe máy", "Xe máy điện", "Ô tô", "Ô tô điện" };

        [ObservableProperty]
        private int _electricMotorcycleCount;

        [ObservableProperty]
        private int _electricCarCount;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _searchText = string.Empty;

        public ParkingViewModel(IParkingService parkingService)
        {
            _parkingService = parkingService;
            _ = LoadVehiclesAsync();
        }

        partial void OnSelectedVehicleChanged(VehicleResponseDTO? value)
        {
            if (value != null)
            {
                SelectedApartmentCode = value.ApartmentCode;
                SelectedVehicleType = value.VehicleType;
                SelectedVehicleNumber = value.VehicleNumber?.Trim();
                SelectedVehicleOwner = value.VehicleOwner;
                SelectedVehicleStatus = value.Status;
            }
        }

        [RelayCommand]
        private async Task LoadVehiclesAsync()
        {
            try
            {
                IsLoading = true;
                var vehicles = await _parkingService.GetAllVehiclesAsync();
                var statistics = await _parkingService.GetVehicleStatisticsAsync();
                await UpdateUI(vehicles, statistics);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tải danh sách xe!");
                Debug.WriteLine($"Load error: {ex}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task SearchVehiclesAsync()
        {
            try
            {
                IsLoading = true;
                var vehicles = await _parkingService.GetAllVehiclesAsync();

                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    vehicles = vehicles.Where(v =>
                        (!string.IsNullOrEmpty(v.VehicleNumber) &&
                         v.VehicleNumber.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(v.VehicleOwner) &&
                         v.VehicleOwner.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                    ).ToList();
                }

                var statistics = await _parkingService.GetVehicleStatisticsAsync();
                await UpdateUI(vehicles, statistics);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tìm kiếm xe!");
                Debug.WriteLine($"Search error: {ex}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task EditVehicleAsync()
        {
            if (SelectedVehicle == null)
            {
                MessageBox.Show("Vui lòng chọn xe cần sửa!");
                return;
            }

            if (string.IsNullOrWhiteSpace(SelectedVehicleOwner) ||
                string.IsNullOrWhiteSpace(SelectedVehicleType) ||
                string.IsNullOrWhiteSpace(SelectedVehicleStatus) ||
                (SelectedVehicleType != "Xe đạp" && string.IsNullOrWhiteSpace(SelectedVehicleNumber)))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return;
            }

            if (!ValidateLicensePlate(SelectedVehicleNumber, SelectedVehicleType))
                return;

            if (!ValidateOwnerName(SelectedVehicleOwner))
                return;

            try
            {
                IsLoading = true;

                var updatedVehicle = new VehicleDTO
                {
                    VehicleNumber = SelectedVehicle.VehicleNumber,
                    VehicleType = SelectedVehicleType,
                    VehicleOwner = SelectedVehicleOwner,
                    ApartmentId = SelectedVehicle.ApartmentId,
                    Status = SelectedVehicleStatus
                };

                var success = await _parkingService.UpdateVehicleAsync(updatedVehicle);

                if (success)
                {
                    MessageBox.Show("Cập nhật thông tin xe thành công!");
                    await LoadVehiclesAsync();
                    ClearSelection();
                }
                else
                {
                    MessageBox.Show("Không thể cập nhật thông tin xe!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra khi cập nhật thông tin xe!");
                Debug.WriteLine($"Edit error: {ex}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ClearSelection()
        {
            SelectedVehicle = null;
            SelectedVehicleNumber = string.Empty;
            SelectedVehicleOwner = string.Empty;
            SelectedVehicleType = string.Empty;
            SelectedApartmentCode = string.Empty;
            SelectedVehicleStatus = string.Empty;
        }

        private async Task UpdateUI(List<VehicleResponseDTO> vehicles, VehicleStatisticsDTO statistics)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                Vehicles.Clear();
                foreach (var vehicle in vehicles)
                {
                    Vehicles.Add(vehicle);
                }

                BicycleCount = statistics.BicycleCount;
                MotorcycleCount = statistics.MotorcycleCount;
                CarCount = statistics.CarCount;
                ElectricMotorcycleCount = statistics.ElectricMotorcycleCount;
                ElectricCarCount = statistics.ElectricCarCount;
            });
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

            string normalizedNumber = licensePlate.Trim().Replace(" ", "").Replace("-", "").ToUpper();

            if (normalizedNumber.Length > 12)
            {
                MessageBox.Show("Biển số xe không được vượt quá 12 ký tự!");
                return false;
            }

            string pattern = @"^\d{2}[A-Z]{1,2}\d{4,6}$";
            if (!Regex.IsMatch(normalizedNumber, pattern))
            {
                MessageBox.Show("Biển số xe không hợp lệ!");
                return false; 
            }

            return true;
        }

        private bool ValidateOwnerName(string? ownerName)
        {
            if (string.IsNullOrWhiteSpace(ownerName))
            {
                MessageBox.Show("Vui lòng nhập tên chủ xe!");
                return false;
            }

            var normalizedName = ownerName.Trim();
            
            if (normalizedName.Length > 50)
            {
                MessageBox.Show("Tên chủ xe không được vượt quá 50 ký tự!");
                return false;
            }

            string pattern = @"^[a-zA-ZÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơưăạảấầẩẫậắằẳẵặẹẻẽềềểễệỉịọỏốồổỗộớờởỡợụủứừửữựỳỵỷỹ\s]+$";
            if (!Regex.IsMatch(normalizedName, pattern))
            {
                MessageBox.Show("Tên chủ xe không hợp lệ! Chỉ được phép nhập chữ cái và khoảng trắng.");
                return false;
            }

            return true;
        }

        [RelayCommand]
        private async Task DeleteVehicleAsync()
        {
            if (SelectedVehicle == null)
            {
                MessageBox.Show("Vui lòng chọn xe cần xoá!");
                return;
            }

            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xoá xe {SelectedVehicle.VehicleNumber}?",
                "Xác nhận xoá",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                IsLoading = true;

                var success = await _parkingService.DeleteVehicleAsync(SelectedVehicle.VehicleNumber);

                if (success)
                {
                    MessageBox.Show("Xoá xe thành công!");
                    await LoadVehiclesAsync();
                    ClearSelection();
                }
                else
                {
                    MessageBox.Show("Không thể xoá xe!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể xoá xe đang gửi!");
                Debug.WriteLine($"Delete error: {ex}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
