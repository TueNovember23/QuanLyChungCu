using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Services.DTOs.ApartmentDTO;
using Services.Interfaces.AccountantServices;
using System.Collections.ObjectModel;

namespace Forms.ViewModels.AdministrativeStaff
{
    public partial class ApartmentViewModel : ObservableObject
    {
        private readonly IApartmentService _apartmentService;

        [ObservableProperty]
        private ObservableCollection<ResponseApartmentDTO> apartments = [];

        public ApartmentViewModel(IApartmentService apartmentService)
        {
            _apartmentService = apartmentService;
            _ = LoadApartmentsAsync();
        }

        private async Task LoadApartmentsAsync()
        {
            var apartmentList = await _apartmentService.GetAll();
            Apartments = new ObservableCollection<ResponseApartmentDTO>(apartmentList);
        }

        [RelayCommand]
        private void ViewResidents(int apartmentId)
        {
            
        }

        [RelayCommand]
        private void EditApartment(int apartmentId)
        {
            
        }
    }
}
