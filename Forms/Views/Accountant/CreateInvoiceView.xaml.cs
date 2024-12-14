using Core;
using Repositories.Repositories.Entities;
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

        private void Button_Click(object sender, RoutedEventArgs e)
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

            if (!int.TryParse(NumberOfPeopleInput.Text.Replace("người", "").Trim(), out int numberOfPeople) || numberOfPeople <= 0)
            {
                throw new BusinessException("Số lượng cư dân không hợp lệ.");
            }

            double averageUsage = (endIndex - startIndex) / (double)numberOfPeople;
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

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // lưu hóa đơn
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
