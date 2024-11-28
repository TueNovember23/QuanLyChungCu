using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Forms.Views.AdministrativeStaff;
using Services.DTOs.ApartmentDTO;
using Services.Interfaces.AdministrativeStaffServices;
using System.Collections.ObjectModel;

namespace Forms.ViewModels.AdministrativeStaff
{
    public partial class ApartmentViewModel : ObservableObject
    {
        private readonly IApartmentService _apartmentService;

        [ObservableProperty]
        private ObservableCollection<ResponseApartmentDTO> apartments = [];

        [ObservableProperty]
        private ObservableCollection<ResponseApartmentDTO> filteredApartments = [];

        [ObservableProperty]
        private string searchText = "";

        public ApartmentViewModel(IApartmentService apartmentService)
        {
            _apartmentService = apartmentService;
            _ = LoadApartmentsAsync();
        }

        private async Task LoadApartmentsAsync()
        {
            var apartmentList = await _apartmentService.GetAll();
            FilteredApartments = Apartments = new ObservableCollection<ResponseApartmentDTO>(apartmentList);
        }

        [RelayCommand]
        private void Refresh()
        {
            // Xóa nội dung tìm kiếm
            SearchText = string.Empty;
            // Trả lại danh sách căn hộ ban đầu
            FilteredApartments = new ObservableCollection<ResponseApartmentDTO>(Apartments);
        }

        [RelayCommand]
        private async Task Search()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredApartments = new ObservableCollection<ResponseApartmentDTO>(Apartments);
            }
            else
            {
                var result = await _apartmentService.Search(SearchText);
                FilteredApartments = new ObservableCollection<ResponseApartmentDTO>(result);
            }
        }


        [RelayCommand]
        private void ViewResidents(string apartmentCode)
        {
            var residentsView = new ResidentsOfApartmentView(_apartmentService, apartmentCode);
            residentsView.ShowDialog();
        }


        [RelayCommand]
        private void EditApartment(int apartmentId)
        {
            System.Diagnostics.Debug.WriteLine($"EditApartment called for ApartmentId: {apartmentId}");
        }

        [RelayCommand]
        private void ViewRepresentative(string apartmentCode)
        {
            var f = new UpdateRepresentativeView(_apartmentService, apartmentCode);
            f.ShowDialog();
        }

    }
}
