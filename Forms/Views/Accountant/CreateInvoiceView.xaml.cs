using Core;
using Repositories.Repositories.Entities;
using Services.DTOs.LoginDTO;
using Services.Interfaces.AccountantServices;
using Services.Services.AccountantServices;
using System.Windows;

namespace Forms.Views.Accountant
{
    /// <summary>
    /// Interaction logic for CreateInvoiceView.xaml
    /// </summary>
    public partial class CreateInvoiceView : Window
    {
        private readonly IInvoiceService _service;

        public LoginResponseDTO? User { get; set; }

        public CreateInvoiceView(IInvoiceService service)
        {
            InitializeComponent();
            _service = service;
            InitializeData();
        }

        private async Task InitializeData()
        {
            List<int> months = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12];
            List<int> years = [];
            for (int i = 2010; i <= 2050; i++)
            {
                years.Add(i);
            }
            MonthCombobox.ItemsSource = months;
            YearCombobox.ItemsSource = years;
            MonthCombobox.SelectedItem = DateTime.Now.Month;
            YearCombobox.SelectedItem = DateTime.Now.Year;
            ApartmentComboBox.ItemsSource = (await _service.GetApartmentsAsync()).Select(_ => _.ApartmentCode);
        }

        private async void LoadApartmentInvoiceInformation(string apartmentCode)
        {
            Apartment apartment = await _service.GetApartmentByCode(apartmentCode);
            await LoadWaterInvoiceInformation(apartment);
            await LoadManageFeeInvoiceInformation(apartment);
            await LoadVehicleInvoiceInformation(apartment);
        }

        private async Task LoadWaterInvoiceInformation(Apartment apartment)
        {
            StartIndexInput.Text = (await _service.GetLastWaterInvoiceStartIndex(apartment.ApartmentCode!)).ToString();
            NumberOfPeopleInput.Text = apartment.NumberOfPeople.ToString() + "người";
        }

        private async Task LoadManageFeeInvoiceInformation(Apartment apartment)
        {
            AreaInput.Text = apartment.Area.ToString();
            ManageFeeInput.Text = (await _service.GetCurrentManagementFee()).Price.ToString();
        }

        private async Task LoadVehicleInvoiceInformation(Apartment apartment)
        {
            List<ResponseVehicle> vehicles = await _service.GetVehiclesByApartmentCode(apartment.ApartmentCode!);
            NumberOfVehicleInput.Text = vehicles.Count.ToString();
            VehicleDataGrid.ItemsSource = vehicles;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void EndIndexInput_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ApartmentComboBox.SelectedItem == null)
            {
                throw new BusinessException("Vui lòng chọn mã căn hộ.");
            }

            if (MonthCombobox.SelectedItem == null)
            {
                throw new BusinessException("Vui lòng chọn tháng.");
            }

            if (YearCombobox.SelectedItem == null)
            {
                throw new BusinessException("Vui lòng chọn năm.");
            }

            if (!int.TryParse(StartIndexInput.Text, out int startIndex))
            {
                throw new BusinessException("Chỉ số đầu kỳ không hợp lệ.");
            }

            if (!int.TryParse(EndIndexInput.Text, out int endIndex))
            {
                throw new BusinessException("Vui lòng nhập chỉ số cuối kỳ.");
            }

            if (endIndex < startIndex)
            {
                throw new BusinessException("Chỉ số cuối kỳ phải lớn hơn chỉ số đầu kỳ.");
            }
            int peoples = 0;
            if (!int.TryParse(NumberOfPeopleInput.Text.Replace("người", "").Trim(), out int numberOfPeople) || numberOfPeople <= 0)
            {
                throw new BusinessException("Số lượng cư dân không hợp lệ.");
            }
            else
            {
                peoples = numberOfPeople;
            }
            double currenUsage = (endIndex - startIndex);
            double averageUsage = currenUsage / (double)numberOfPeople;
            if (averageUsage >= 10)
            {
                MessageBoxResult result = MessageBox.Show(
                    "Lượng nước sử dụng trung bình trên mỗi người lớn hơn hoặc bằng 10. Hãy chắn chắn đã nhập đúng chỉ số cuối kỳ. Bạn có muốn tiếp tục không?",
                    "Cảnh báo",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (result == MessageBoxResult.No)
                {
                    return;
                }
            }

            // Tính tiền nước
            WaterFee waterFee = await _service.GetCurrentWaterFee();
            double waterPrice = 0;
            int level1 = (waterFee.Level1 ?? 0) * peoples;
            int level2 = (waterFee.Level2 ?? 0) * peoples;
            if (currenUsage <= level1)
            {
                waterPrice = currenUsage * (waterFee.Price1 ?? 0);
            }
            else if (currenUsage <= level2)
            {
                waterPrice = level1 * (waterFee.Price1 ?? 0) + (currenUsage - level1) * (waterFee.Price2 ?? 0);
            }
            else
            {
                waterPrice = level1 * (waterFee.Price1 ?? 0) + (level2 - level1) * (waterFee.Price2 ?? 0) + (currenUsage - level2) * (waterFee.Price3 ?? 0);
            }
            WaterInvoiceAmountInput.Text = waterPrice.ToString() + "vnđ";

            // Tính tiền phí quản lý
            double manageFee = (await _service.GetCurrentManagementFee()).Price ?? 0;
            double apartmentArea = double.Parse(AreaInput.Text);
            double manageFeePrice = manageFee * apartmentArea;
            ManageFeeInvoiceAmoutInput.Text = manageFeePrice.ToString() + "vnđ";

            // Tính tiền phí xe
            double vehiclePrice = 0;
            List<ResponseVehicle> vehicles = (VehicleDataGrid.ItemsSource as List<ResponseVehicle>)!;
            foreach (var vehicle in vehicles)
            {
                vehiclePrice += vehicle.Fee ?? 0;
            }
            VehicleInvoiceAmount.Text = vehiclePrice.ToString() + "vnđ";

            // Cập nhật thành tiền
            TotalAmountInput.Text = (waterPrice + manageFeePrice + vehiclePrice).ToString() + "vnđ";
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string apartmentCode = ApartmentComboBox.SelectedItem as string;
            Account account = await _service.GetAccountByUsername(User!.Username);
            Apartment apartment = await _service.GetApartmentByCode(apartmentCode!);
            WaterFee waterFee = await _service.GetCurrentWaterFee();
            ManagementFee managementFee = await _service.GetCurrentManagementFee();
            Invoice invoice = new()
            {
                Month = (int)MonthCombobox.SelectedItem,
                Year = (int)YearCombobox.SelectedItem,
                TotalAmount = double.Parse(TotalAmountInput.Text.Replace("vnđ", "")),
                CreatedBy = account.AccountId
            };

            WaterInvoice waterInvoice = new()
            {
                StartIndex = int.Parse(StartIndexInput.Text),
                EndIndex = int.Parse(EndIndexInput.Text),
                NumberOfPeople = int.Parse(NumberOfPeopleInput.Text.Replace("người", "").Trim()),
                TotalAmount = double.Parse(WaterInvoiceAmountInput.Text.Replace("vnđ", "")),
                ApartmentId = apartment.ApartmentId,
                WaterFeeId = waterFee.WaterFeeId,
            };

            ManagementFeeInvoice managementInvoce = new()
            {
                Price = 0,
                TotalAmount = double.Parse(ManageFeeInvoiceAmoutInput.Text.Replace("vnđ", "")),
                ApartmentId = apartment.ApartmentId,
                ManagementFeeHistoryId = managementFee.ManagementFeeId,
            };

            List<ResponseVehicle> vehicles = (VehicleDataGrid.ItemsSource as List<ResponseVehicle>)!;
            VechicleInvoice vechicleInvoice = new()
            {
                TotalAmount = double.Parse(VehicleInvoiceAmount.Text.Replace("vnđ", "")),
                ApartmentId = apartment.ApartmentId
            };
            List<VechicleInvoiceDetail> vechicleInvoiceDetails = new();
            foreach (var v in vehicles)
            {
                vechicleInvoiceDetails.Add(new()
                {
                    VehicleId = v.Id!,
                    Price = v.Fee ?? 0
                });
            }
            await _service.CreateInvoice(invoice, waterInvoice, managementInvoce, vechicleInvoice, vechicleInvoiceDetails);
        }


        private void ApartmentComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            string? selectedApartmentCode = ApartmentComboBox.SelectedItem as string;

            if (!string.IsNullOrEmpty(selectedApartmentCode))
            {
                // Load lại dữ liệu theo mã căn hộ
                LoadApartmentInvoiceInformation(selectedApartmentCode);
            }
        }
    }
}
