using Repositories.Repositories.Entities;
using Services.Interfaces.AdministrativeStaffServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Forms.Views.AdministrativeStaff
{
    public partial class ResidentsOfApartmentView : Window
    {
        private readonly IApartmentService _apartmentService;
        private Apartment? _apartment;

        public ResidentsOfApartmentView(IApartmentService service, string apartmentCode)
        {
            InitializeComponent();
            _apartmentService = service;

            // Khởi tạo dữ liệu cho cửa sổ
            _ = InitializeAsync(apartmentCode);
        }

        public async Task InitializeAsync(string apartmentCode)
        {
            try
            {
                _apartment = await _apartmentService.GetApartmentByCode(apartmentCode)
                             ?? throw new Exception($"Căn hộ {apartmentCode} không tồn tại");

                ResidentsDataGrid.ItemsSource = _apartment.Residents
                    .Select(r => new
                    {
                        ResidentId = r.ResidentId,
                        FullName = r.FullName,
                        Gender = r.Gender,
                        DateOfBirth = r.DateOfBirth?.ToString("dd/MM/yyyy"),
                        RelationShipWithOwner = r.RelationShipWithOwner,
                        MoveInDate = r.MoveInDate?.ToString("dd/MM/yyyy")
                    }).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
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
    }
}
