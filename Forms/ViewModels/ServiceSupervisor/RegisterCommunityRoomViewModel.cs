using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Services.DTOs.CommunityRoomBookingDTO;
using Services.Interfaces.ServiceSupervisorServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forms.ViewModels.ServiceSupervisor
{
    public partial class RegisterCommunityRoomViewModel : ObservableObject
    {
        private readonly ICommunityRoomService _communityRoomService;

        [ObservableProperty]
        private ObservableCollection<ResponseCommunityRoomBookingDTO> bookings = [];

        [ObservableProperty]
        private ObservableCollection<ResponseCommunityRoomDTO> rooms = [];

        [ObservableProperty]
        private ResponseCommunityRoomDTO? selectedRoom;

        [ObservableProperty]
        private DateOnly bookingDate = DateOnly.FromDateTime(DateTime.Today);

        [ObservableProperty]
        private TimeOnly startTime;

        [ObservableProperty]
        private TimeOnly endTime;

        [ObservableProperty]
        private int numberOfPeople;

        public RegisterCommunityRoomViewModel(ICommunityRoomService communityRoomService)
        {
            _communityRoomService = communityRoomService;
            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            var roomList = await _communityRoomService.GetAll();
            Rooms = new ObservableCollection<ResponseCommunityRoomDTO>(roomList);

            var bookingList = await _communityRoomService.GetBookings();
            Bookings = new ObservableCollection<ResponseCommunityRoomBookingDTO>(bookingList);
        }

        [RelayCommand]
        private async Task Refresh()
        {
            await LoadDataAsync();
        }

        [RelayCommand]
        private async Task Register()
        {
            if (SelectedRoom == null) return;

            var success = await _communityRoomService.CreateBooking(
                SelectedRoom.CommunityRoomId,
                0, // Need to get current ApartmentId
                BookingDate,
                StartTime,
                EndTime,
                NumberOfPeople
            );

            if (success)
            {
                await LoadDataAsync();
                // Show success message
            }
            else
            {
                // Show error message
            }
        }

        [RelayCommand]
        private async Task DeleteBooking(int bookingId)
        {
            var success = await _communityRoomService.DeleteBooking(bookingId);
            if (success)
            {
                await LoadDataAsync();
                // Show success message
            }
            else
            {
                // Show error message
            }
        }
    }
}
