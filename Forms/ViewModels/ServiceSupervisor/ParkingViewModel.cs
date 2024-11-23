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
        private bool _isLoading;

        [ObservableProperty]
        private string _searchText = string.Empty;

        public ParkingViewModel(IParkingService parkingService)
        {
            _parkingService = parkingService;
            _ = LoadVehiclesAsync();
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
            if (IsLoading) return;

            try
            {
                IsLoading = true;
                var vehicles = await _parkingService.SearchVehiclesAsync(SearchText);
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

        private async Task UpdateUI(List<VehicleResponseDTO> vehicles, VehicleStatisticsDTO statistics)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                Vehicles = new ObservableCollection<VehicleResponseDTO>(vehicles);
                BicycleCount = statistics.BicycleCount;
                MotorcycleCount = statistics.MotorcycleCount;
                CarCount = statistics.CarCount;
            });
        }
    }
}
