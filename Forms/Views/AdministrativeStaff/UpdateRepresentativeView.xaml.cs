using Core;
using Microsoft.IdentityModel.Tokens;
using Repositories.Repositories.Entities;
using Services.Interfaces.AdministrativeStaffServices;
using System.Windows;

namespace Forms.Views.AdministrativeStaff
{
    /// <summary>
    /// Interaction logic for UpdateRepresentativeView.xaml
    /// </summary>
    public partial class UpdateRepresentativeView : Window
    {
        private readonly IApartmentService _service;
        private string? _representativeId;
        public UpdateRepresentativeView(IApartmentService apartmentService, string apartmentCode)
        {
            InitializeComponent();
            _service = apartmentService;
            _ = InitializeAsync(apartmentCode);
        }

        public async Task InitializeAsync(string apartmentCode)
        {
            Representative? p = await _service.GetPreresentativeByApartmentCode(apartmentCode);
            FullNameInput.Text = p is not null ? p.FullName : "";
            GenderInput.SelectedIndex = (p is not null && p.Gender?.Trim() == "Nam") ? 0 : 1;
            DateOfBirthInput.SelectedDate = p is not null ? p.DateOfBirth?.ToDateTime(new TimeOnly()):DateTime.Now;
            _representativeId = p?.RepresentativeId;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void UpdateRepresentative_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
