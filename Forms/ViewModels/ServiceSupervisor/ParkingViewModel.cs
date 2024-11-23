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
        private bool _isLicensePlateEnabled = true;

        [ObservableProperty]
        private ObservableCollection<string> _vehicleTypes = new() 
        { "Xe đạp", "Xe máy", "Xe máy điện", "Ô tô", "Ô tô điện" };

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
                SelectedVehicleType = value.VehicleType; // ComboBox chỉ hiển thị, không binding 2 chiều
                SelectedVehicleNumber = value.VehicleNumber;
                SelectedVehicleOwner = value.VehicleOwner;
                IsLicensePlateEnabled = value.VehicleType != "Xe đạp";
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
        private async Task FilterByVehicleTypeAsync()
        {
            if (string.IsNullOrEmpty(SelectedVehicleType))
            {
                await LoadVehiclesAsync();
                return;
            }

            try
            {
                IsLoading = true;
                var vehicles = (await _parkingService.GetAllVehiclesAsync())
                    .Where(v => v.VehicleType == SelectedVehicleType)
                    .ToList();
                var statistics = await _parkingService.GetVehicleStatisticsAsync();

                await UpdateUI(vehicles, statistics);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể lọc danh sách xe!");
                Debug.WriteLine($"Filter error: {ex}");
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

                List<VehicleResponseDTO> vehicles;
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    // Nếu ô tìm kiếm trống thì load lại tất cả
                    vehicles = await _parkingService.GetAllVehiclesAsync();
                }
                else
                {
                    // Tìm kiếm theo biển số xe chứa SearchText
                    vehicles = await _parkingService.GetAllVehiclesAsync();
                    vehicles = vehicles.Where(v => 
                        !string.IsNullOrEmpty(v.VehicleNumber) &&
                        v.VehicleNumber.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0)
                        .ToList();
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
        private async Task DeleteVehicleAsync(VehicleResponseDTO vehicle)
        {
            if (vehicle == null || IsLoading) return;

            var result = MessageBox.Show(
                $"Bạn có chắc muốn xóa xe {vehicle.VehicleNumber}?",
                "Xác nhận",
                MessageBoxButton.YesNo);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                IsLoading = true;
                var success = await _parkingService.DeleteVehicleAsync(vehicle.VehicleNumber);

                if (success)
                {
                    await LoadVehiclesAsync();
                    MessageBox.Show("Xóa xe thành công!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể xóa xe!");
                Debug.WriteLine($"Delete error: {ex}");
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
                (SelectedVehicleType != "Xe đạp" && string.IsNullOrWhiteSpace(SelectedVehicleNumber)))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return;
            }

            try
            {
                IsLoading = true;

                var updatedVehicle = new VehicleDTO
                {
                    VehicleNumber = SelectedVehicleType == "Xe đạp" ? "" : SelectedVehicleNumber,
                    VehicleType = SelectedVehicleType,
                    VehicleOwner = SelectedVehicleOwner,
                    ApartmentId = SelectedVehicle.ApartmentId
                };

                var success = await _parkingService.UpdateVehicleAsync(updatedVehicle);

                if (success)
                {
                    MessageBox.Show("Cập nhật thông tin xe thành công!");
                    
                    await LoadVehiclesAsync();
                    
                    SelectedVehicle = null;
                    SelectedVehicleNumber = string.Empty;
                    SelectedVehicleOwner = string.Empty;
                    SelectedVehicleType = string.Empty;
                    SelectedApartmentCode = string.Empty;
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

        partial void OnSelectedVehicleTypeChanged(string value)
        {
            if (value == "Xe đạp")
            {
                SelectedVehicleNumber = string.Empty;
                IsLicensePlateEnabled = false;
            }
            else
            {
                IsLicensePlateEnabled = true;
            }

            _ = FilterByVehicleTypeAsync();
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
            });
        }
    }
}
