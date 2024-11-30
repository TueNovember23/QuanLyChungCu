using Core;
using Microsoft.IdentityModel.Tokens;
using Repositories.Repositories.Entities;
using Services.DTOs.Representative;
using Services.Interfaces.AdministrativeStaffServices;
using System.Windows;
using System.Windows.Controls;

namespace Forms.Views.AdministrativeStaff
{
    /// <summary>
    /// Interaction logic for UpdateRepresentativeView.xaml
    /// </summary>
    public partial class UpdateRepresentativeView : Window
    {
        private readonly IApartmentService _service;
        private string _apartmentCode = "";
        private string? _currentRepresentativeId;
        private List<Representative> _representatives = [];
        public UpdateRepresentativeView(IApartmentService apartmentService, string apartmentCode)
        {
            InitializeComponent();
            _service = apartmentService;
            _ = InitializeAsync(apartmentCode);
            RepresentativeIdInput.SelectionChanged += RepresentativeIdInput_SelectionChanged;
            ApartmentInfo.Text = $"Thông tin người đại diện căn hộ {apartmentCode}";
        }

        public async Task InitializeAsync(string apartmentCode)
        {
            _apartmentCode = apartmentCode;
            _representatives = await _service.GetAllRepresentative();
            RepresentativeIdInput.ItemsSource = _representatives.Select(_ => $"{_.RepresentativeId} - {_.FullName}");
            _currentRepresentativeId = _representatives.FirstOrDefault(_ => _.Apartments.Any(a => a.ApartmentCode == apartmentCode))?.RepresentativeId;

            LoadRepresentativeData(_currentRepresentativeId);

            ReId.Text = _currentRepresentativeId;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void UpdateRepresentative_Click(object sender, RoutedEventArgs e)
        {
            string id = ReId.Text;
            if(id != _currentRepresentativeId)
            {
                await _service.UpdateApartmentRepresentative(_apartmentCode, id);
            } else
            {
                var result = MessageBox.Show("Bạn có chắc chắn muốn cập nhật thông tin người đại diện này không?" +
                    "\nSau khi cập nhật, thông tin người đại diện này của các căn hộ khác cũng sẽ thay đổi.",
                                     "Xác nhận cập nhật",
                                     MessageBoxButton.YesNo,
                                     MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    UpdateRepresentativeDTO dto = new()
                    {
                        FullName = FullNameInput.Text,
                        DateOfBirth = DateOnly.FromDateTime(DateOfBirthInput.SelectedDate!.Value),
                        Gender = (GenderInput.SelectedItem as ComboBoxItem)?.Content.ToString(),
                        Email = EmailInput.Text,
                        PhoneNumber = PhoneNumberInput.Text
                    };
                    await _service.UpdateRepresentative(_currentRepresentativeId, dto);
                }
            }
            this.Close();
        }

        private void RepresentativeIdInput_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string? selectedId = RepresentativeIdInput.SelectedItem?.ToString() != null ? RepresentativeIdInput.SelectedItem.ToString()!.Split(" - ")[0] : "";
            if (!string.IsNullOrEmpty(selectedId) && selectedId != _currentRepresentativeId)
            {
                LoadRepresentativeData(selectedId);
                ReId.IsReadOnly = true;
                FullNameInput.IsReadOnly = true;
                GenderInput.IsEnabled = false;
                DateOfBirthInput.IsEnabled = false;
                EmailInput.IsReadOnly = true;
                PhoneNumberInput.IsReadOnly = true;
            } else
            {
                ReId.IsReadOnly = false;
                FullNameInput.IsReadOnly = false;
                GenderInput.IsEnabled = true;
                DateOfBirthInput.IsEnabled = true;
                EmailInput.IsReadOnly = false;
                PhoneNumberInput.IsReadOnly = false;
            }
        }

        private async void LoadRepresentativeData(string? representativeId)
        {
            if (string.IsNullOrEmpty(representativeId))
            {
                ClearForm();
                return;
            }

            Representative? p = await _service.GetRepresentativeById(representativeId);

            ReId.Text = p?.RepresentativeId?? "";
            FullNameInput.Text = p?.FullName ?? "";
            GenderInput.SelectedIndex = (p != null && p.Gender?.Trim() == "Nam") ? 0 : 1;
            DateOfBirthInput.SelectedDate = p?.DateOfBirth?.ToDateTime(new TimeOnly());
            EmailInput.Text = p?.Email ?? "";
            PhoneNumberInput.Text = p?.PhoneNumber ?? "";

            RepresentativeIdInput.SelectedValue = $"{p?.RepresentativeId} - {p?.FullName}";
        }

        private void ClearForm()
        {
            ReId.Text = "";
            FullNameInput.Text = "";
            GenderInput.SelectedIndex = -1;
            DateOfBirthInput.SelectedDate = null;
            EmailInput.Text = "";
            PhoneNumberInput.Text = "";
        }
    }
}
