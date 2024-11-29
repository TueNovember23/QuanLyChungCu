using Core;
using Repositories.Repositories.Entities;
using Services.DTOs.ApartmentDTO;
using Services.DTOs.ResidentDTO;
using Services.Interfaces.AdministrativeStaffServices;
using System.Windows;

namespace Forms.Views.AdministrativeStaff
{
    public partial class ResidentsOfApartmentView : Window
    {
        private readonly IApartmentService _apartmentService;
        private Apartment? _apartment;
        private List<ResponseResidentDTO> _residents;

        public ResidentsOfApartmentView(IApartmentService service, string apartmentCode)
        {
            InitializeComponent();
            _apartmentService = service;

            // Khởi tạo dữ liệu cho cửa sổ
            _ = InitializeAsync(apartmentCode);
        }

        public async Task InitializeAsync(string apartmentCode)
        {
            _apartment = await _apartmentService.GetApartmentByCode(apartmentCode)
                         ?? throw new BusinessException($"Căn hộ {apartmentCode} không tồn tại");

            ApartmentCodeTxt.Text = $"Căn hộ {apartmentCode}";

            _residents = await _apartmentService.GetResidentsOfApartment(apartmentCode);
            ResidentsDataGrid.ItemsSource = _residents;
        }

        private void EditResident_Click(object sender, RoutedEventArgs e)
        {
            // Lấy ResidentId từ CommandParameter
            var residentId = ((FrameworkElement)sender).Tag as string;
            if (!string.IsNullOrEmpty(residentId))
            {
                // Thực hiện chỉnh sửa cư dân
                MessageBox.Show($"Chỉnh sửa cư dân có ID: {residentId}", "Thông báo", MessageBoxButton.OK);
                // TODO: Mở form chỉnh sửa cư dân hoặc thêm logic chỉnh sửa tại đây
            }
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void RegisterResident_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
