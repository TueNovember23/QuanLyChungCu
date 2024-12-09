using Core;
using Repositories.Repositories.Entities;
using Services.DTOs.ResidentDTO;
using Services.Interfaces.AdministrativeStaffServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Forms.Views.AdministrativeStaff
{
    /// <summary>
    /// Interaction logic for HouseholdMovementView.xaml
    /// </summary>
    public partial class HouseholdMovementView : Window
    {
        private readonly IApartmentService _apartmentService;
        private Apartment? _apartment;
        private List<ResponseResidentDTO> _residents = [];
        public HouseholdMovementView(IApartmentService service, string apartmentCode)
        {
            InitializeComponent();
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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void EditResident_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
