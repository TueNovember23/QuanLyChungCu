using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Repositories.Repositories.Entities;
using Services.Interfaces.AccountantServices;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace Forms.ViewModels.Accountant
{
    public partial class FeeManagementViewModel : ObservableObject
    {
        private readonly IFeeService _feeService;

        [ObservableProperty]
        private ObservableCollection<WaterFee> waterFees = new();

        [ObservableProperty]
        private ObservableCollection<ManagementFee> managementFees = new();

        [ObservableProperty]
        private ObservableCollection<VehicleCategory> vehicleCategories = new();

        [ObservableProperty]
        private VehicleCategory selectedVehicleCategory;

        [ObservableProperty]
        private bool isLoading;

        // Các thuộc tính cho form thêm mới
        [ObservableProperty]
        private int level1;
        [ObservableProperty]
        private double price1;
        [ObservableProperty]
        private int level2;
        [ObservableProperty]
        private double price2;
        [ObservableProperty]
        private double price3;
        [ObservableProperty]
        private double managementPrice;
        [ObservableProperty]
        private string categoryName = string.Empty;
        [ObservableProperty]
        private double monthlyFee;

        public FeeManagementViewModel(IFeeService feeService)
        {
            _feeService = feeService;
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            try
            {
                await LoadAllData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadAllData()
        {
            IsLoading = true;
            try
            {
                // Load tuần tự để tránh xung đột DbContext
                await LoadWaterFees();
                await LoadManagementFees();
                await LoadVehicleCategories();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadWaterFees()
        {
            try
            {
                var fees = await _feeService.GetWaterFees();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    WaterFees.Clear();
                    foreach (var fee in fees)
                    {
                        WaterFees.Add(fee);
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải giá nước: {ex.Message}");
            }
        }

        private async Task LoadManagementFees()
        {
            try
            {
                var fees = await _feeService.GetManagementFees();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ManagementFees.Clear();
                    foreach (var fee in fees)
                    {
                        ManagementFees.Add(fee);
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải phí quản lý: {ex.Message}");
            }
        }

        private async Task LoadVehicleCategories()
        {
            try
            {
                var categories = await _feeService.GetVehicleCategories();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    VehicleCategories.Clear();
                    foreach (var category in categories)
                    {
                        VehicleCategories.Add(category);
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải loại xe: {ex.Message}");
            }
        }

        partial void OnSelectedVehicleCategoryChanged(VehicleCategory value)
        {
            if (value != null)
            {
                CategoryName = value.CategoryName;
                MonthlyFee = value.MonthlyFee ?? 0;
            }
        }

        [RelayCommand]
        private async Task AddWaterFee()
        {
            if (Level1 <= 0 || Price1 <= 0 || Level2 <= 0 || Price2 <= 0 || Price3 <= 0)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin giá nước!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var newFee = new WaterFee
                {
                    CreatedDate = DateOnly.FromDateTime(DateTime.Now),
                    Level1 = Level1,
                    Price1 = Price1,
                    Level2 = Level2,
                    Price2 = Price2,
                    Price3 = Price3
                };

                await _feeService.AddWaterFee(newFee);
                await LoadWaterFees();
                MessageBox.Show("Thêm giá nước mới thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                // Reset form
                Level1 = 0;
                Price1 = 0;
                Level2 = 0;
                Price2 = 0;
                Price3 = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm giá nước: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task AddManagementFee()
        {
            if (ManagementPrice <= 0)
            {
                MessageBox.Show("Vui lòng nhập giá phí quản lý!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var newFee = new ManagementFee
                {
                    CreatedDate = DateOnly.FromDateTime(DateTime.Now),
                    Price = ManagementPrice
                };

                await _feeService.AddManagementFee(newFee);
                await LoadManagementFees();
                MessageBox.Show("Thêm phí quản lý mới thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                // Reset form
                ManagementPrice = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm phí quản lý: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task AddVehicleCategory()
        {
            if (string.IsNullOrWhiteSpace(CategoryName) || MonthlyFee <= 0)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin loại xe!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var newCategory = new VehicleCategory
                {
                    CategoryName = CategoryName.Trim(),
                    MonthlyFee = MonthlyFee
                };

                await _feeService.AddVehicleCategory(newCategory);
                await LoadVehicleCategories();
                MessageBox.Show("Cập nhật phí gửi xe thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                // Reset form
                CategoryName = string.Empty;
                MonthlyFee = 0;
                SelectedVehicleCategory = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật phí gửi xe: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task Refresh()
        {
            try
            {
                await LoadAllData();
                MessageBox.Show("Làm mới dữ liệu thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi làm mới dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}