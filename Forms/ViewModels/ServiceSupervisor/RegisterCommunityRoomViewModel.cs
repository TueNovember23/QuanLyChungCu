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

        [ObservableProperty]
        private string? errorMessage;

        [ObservableProperty]
        private bool showErrorMessage;

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
            if (SelectedRoom == null)
            {
                ErrorMessage = "Vui lòng chọn phòng";
                ShowErrorMessage = true;
                return;
            }

            if (NumberOfPeople <= 0)
            {
                ErrorMessage = "Số người tham gia phải lớn hơn 0";
                ShowErrorMessage = true;
                return;
            }

            if (NumberOfPeople > SelectedRoom.RoomSize)
            {
                ErrorMessage = $"Số người tham gia vượt quá sức chứa của phòng ({SelectedRoom.RoomSize} người)";
                ShowErrorMessage = true;
                return;
            }

            if (StartTime > EndTime)
            {
                ErrorMessage = "Thời gian kết thúc phải sau thời gian bắt đầu";
                ShowErrorMessage = true;
                return;
            }

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
                ShowErrorMessage = false;
                ErrorMessage = string.Empty;
            }
            else
            {
                ErrorMessage = "Phòng đã được đặt trong khoảng thời gian này";
                ShowErrorMessage = true;
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
