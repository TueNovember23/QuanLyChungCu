using Core;
using Repositories.Repositories.Entities;
using Services.DTOs.ApartmentDTO;
using Services.DTOs.ResidentDTO;
using Services.Interfaces.AdministrativeStaffServices;
using System.Windows;
using System.Windows.Controls;

namespace Forms.Views.AdministrativeStaff
{
    public partial class ResidentsOfApartmentView : Window
    {
        private readonly IApartmentService _apartmentService;
        private Apartment? _apartment;
        private List<ResponseResidentDTO> _residents = [];

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
                             ?? throw new BusinessException($"Căn hộ {apartmentCode} không tồn tại");

                ApartmentCodeTxt.Text = $"Căn hộ {apartmentCode}";
                ApartmentCodeInput.Text = apartmentCode;
                _residents = await _apartmentService.GetResidentsOfApartment(apartmentCode);
                ResidentsDataGrid.ItemsSource = _residents;
            } catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void EditResident_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var residentId = ((Button)sender).CommandParameter as string;

                if (string.IsNullOrEmpty(residentId))
                {
                    throw new BusinessException("Không thể lấy thông tin cư dân. Vui lòng thử lại.");
                }

                var updateResidentWindow = new UpdateResidentView(_apartmentService, residentId)
                {
                    Owner = this // Đặt cửa sổ cha
                };
                updateResidentWindow.ShowDialog();
                RefreshResidents();
            }
            catch
            {
                throw;
            }
        }

        private async void RefreshResidents()
        {
            _residents = await _apartmentService.GetResidentsOfApartment(ApartmentCodeInput.Text.Trim());
            ResidentsDataGrid.ItemsSource = _residents;
        }


        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void RegisterResident_Click(object sender, RoutedEventArgs e)
        {
            var createResidentDto = new CreateResidentDTO
            {
                ResidentId = ResidentIdInput.Text.Trim(),
                FullName = FullNameInput.Text.Trim(),
                Gender = (GenderInput.SelectedItem as ComboBoxItem)?.Content.ToString(),
                DateOfBirth = DateOfBirthInput.SelectedDate.HasValue ? DateOnly.FromDateTime(DateOfBirthInput.SelectedDate.Value) : null,
                RelationShipWithOwner = (RelationshipInput.SelectedItem as ComboBoxItem)?.Content.ToString(),
                ApartmentCode = ApartmentCodeInput.Text.Trim()
            };

            await _apartmentService.RegisterResident(createResidentDto);

            MessageBox.Show("Đăng ký cư dân thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

            _residents = await _apartmentService.GetResidentsOfApartment(ApartmentCodeInput.Text.Trim());
            ResidentsDataGrid.ItemsSource = _residents;

            ClearForm();

        }

        private void ClearForm()
        {
            ResidentIdInput.Text = string.Empty;
            FullNameInput.Text = string.Empty;
            GenderInput.SelectedIndex = -1;
            DateOfBirthInput.SelectedDate = null;
            //RelationShipInput.Text = string.Empty;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ViewHouseholdMovement_Click(object sender, RoutedEventArgs e)
        {
            HouseholdMovementView f = new(_apartmentService, _apartment!.ApartmentCode!);
            f.ShowDialog();
        }
    }
}
