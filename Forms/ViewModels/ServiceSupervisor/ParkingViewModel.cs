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
    public partial class ParkingViewModel : ObservableObject, IDisposable
    {
        private readonly IVehicleService _vehicleService;
        private CancellationTokenSource? _cts;

        [ObservableProperty]
        private ObservableCollection<VehicleResponseDTO> _vehicles = new();

        [ObservableProperty]
        private VehicleResponseDTO? _selectedVehicle;

        [ObservableProperty]
        private ObservableCollection<string> _vehicleTypes = new() { "Xe đạp", "Xe máy", "Ô tô", "Xe máy điện", "Ô tô điện" };

        [ObservableProperty]
        private string _selectedVehicleType = string.Empty;

        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private int _bicycleCount;
        [ObservableProperty]
        private int _motorcycleCount;
        [ObservableProperty]
        private int _carCount;

        [ObservableProperty]
        private bool _isLicensePlateRequired = true;

        [ObservableProperty]
        private bool _isLoading = false;

        public ICommand SearchCommand { get; }
        public ICommand DeleteCommand { get; }

        public ParkingViewModel(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService ?? throw new ArgumentNullException(nameof(vehicleService));
            
            _vehicles = new ObservableCollection<VehicleResponseDTO>();
            _vehicleTypes = new ObservableCollection<string> { "Xe đạp", "Xe máy", "Ô tô", "Xe máy điện", "Ô tô điện" };
            _selectedVehicleType = string.Empty;
            _searchText = string.Empty;

            SearchCommand = new RelayCommand(async () => await SearchVehiclesAsync());
            DeleteCommand = new RelayCommand<VehicleResponseDTO?>(async vehicle => await DeleteVehicleAsync(vehicle));

            _ = LoadVehiclesAsync();
        }

        [RelayCommand]
        private async Task LoadVehiclesAsync()
        {
            try
            {
                IsLoading = true;
                _cts?.Cancel();
                _cts = new CancellationTokenSource();

                var vehicles = await _vehicleService.GetAllVehiclesAsync();
                
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Vehicles.Clear();
                    foreach (var vehicle in vehicles)
                    {
                        Vehicles.Add(vehicle);
                    }
                    UpdateVehicleCounts();
                });
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

                var vehicles = string.IsNullOrWhiteSpace(SearchText) 
                    ? await _vehicleService.GetAllVehiclesAsync()
                    : await _vehicleService.SearchVehiclesAsync(SearchText);

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Vehicles.Clear();
                    foreach (var vehicle in vehicles)
                    {
                        Vehicles.Add(vehicle);
                    }
                    UpdateVehicleCounts();
                });
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
        private async Task DeleteVehicleAsync(VehicleResponseDTO? vehicle)
        {
            if (vehicle == null || IsLoading) return;

            var result = MessageBox.Show(
                $"Bạn có chắc muốn xóa xe {vehicle.VehicleNumber}?",
                "Xác nhận",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                IsLoading = true;
                await _vehicleService.DeleteVehicleAsync(vehicle.VehicleNumber);
                
                Vehicles.Remove(vehicle);
                UpdateVehicleCounts();
                
                MessageBox.Show("Xóa xe thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể xóa xe!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                Debug.WriteLine($"Delete error: {ex}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void UpdateVehicleCounts()
        {
            BicycleCount = Vehicles.Count(v => v.VehicleType == "Xe đạp");
            MotorcycleCount = Vehicles.Count(v => 
                v.VehicleType == "Xe máy" || v.VehicleType == "Xe máy điện");
            CarCount = Vehicles.Count(v => 
                v.VehicleType == "Ô tô" || v.VehicleType == "Ô tô điện");
        }

        partial void OnSelectedVehicleTypeChanged(string value)
        {
            IsLicensePlateRequired = value != "Xe đạp";
            if (!IsLicensePlateRequired && SelectedVehicle != null)
            {
                SelectedVehicle.VehicleNumber = string.Empty;
            }
        }

        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }
}
