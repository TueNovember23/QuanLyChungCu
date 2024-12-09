using Core;
using Repositories.Repositories.Entities;
using Services.DTOs.HouseholdMovementDTO;
using Services.Interfaces.AdministrativeStaffServices;
using System.Windows;

namespace Forms.Views.AdministrativeStaff
{
    /// <summary>
    /// Interaction logic for HouseholdMovementView.xaml
    /// </summary>
    public partial class HouseholdMovementView : Window
    {
        private readonly IApartmentService _apartmentService;
        private Apartment? _apartment;
        private List<ResponseHouseholdMovementDTO> _movements = [];
        private List<ResponseHouseholdMovementDTO> _filters = [];

        public HouseholdMovementView(IApartmentService service, string apartmentCode)
        {
            InitializeComponent();
            _apartmentService = service;
            _ = InitializeAsync(apartmentCode);
        }

        public async Task InitializeAsync(string apartmentCode)
        {
            _apartment = await _apartmentService.GetApartmentByCode(apartmentCode)
                         ?? throw new BusinessException($"Căn hộ {apartmentCode} không tồn tại");

            ApartmentCodeTxt.Text = $"Lịch sử cư trú căn hộ {apartmentCode}";
            _movements = await _apartmentService.GetMovementByApartmentCode(apartmentCode);
            ResidentsDataGrid.ItemsSource = _movements;
        }

        private async Task ReloadData(string apartmentCode)
        {
            _movements = _filters = await _apartmentService.GetMovementByApartmentCode(apartmentCode);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            string searchText = SearchTxt.Text;
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                _filters = await _apartmentService.GetMovementByApartmentCode(_apartment!.ApartmentCode!, searchText);
            }
            else
            {
                _filters = _movements;
            }

            ResidentsDataGrid.ItemsSource = _filters;
        }


        private async void Refresh_Click(object sender, RoutedEventArgs e)
        {
            SearchTxt.Text = "";
            await ReloadData(_apartment!.ApartmentCode!);
            ResidentsDataGrid.ItemsSource = _filters;
        }

    }
}
