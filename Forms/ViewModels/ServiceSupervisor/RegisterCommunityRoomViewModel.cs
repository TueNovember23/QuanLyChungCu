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
        private int numberOfPeople;

        [ObservableProperty]
        private string? errorMessage;

        [ObservableProperty]
        private bool showErrorMessage;

        [ObservableProperty]
        private DateTime? selectedDate = DateTime.Today;

        [ObservableProperty] 
        private DateTime? selectedStartTime;

        [ObservableProperty]
        private DateTime? selectedEndTime;

        // Đổi tên các property này để phù hợp với cách sử dụng trong code
        private DateOnly BookingDate => DateOnly.FromDateTime(SelectedDate ?? DateTime.Today);
        private TimeOnly StartTime => TimeOnly.FromDateTime(SelectedStartTime ?? DateTime.Now); 
        private TimeOnly EndTime => TimeOnly.FromDateTime(SelectedEndTime ?? DateTime.Now);

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
            try 
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

                var now = TimeOnly.FromDateTime(DateTime.Now);
                var today = DateOnly.FromDateTime(DateTime.Today);

                if (BookingDate < today)
                {
                    ErrorMessage = "Không thể đặt phòng cho ngày trong quá khứ";
                    ShowErrorMessage = true;
                    return;
                }

                if (SelectedStartTime == null || SelectedEndTime == null)
                {
                    ErrorMessage = "Vui lòng chọn thời gian đặt phòng";
                    ShowErrorMessage = true;
                    return; 
                }

                if (StartTime >= EndTime)
                {
                    ErrorMessage = "Thời gian kết thúc phải sau thời gian bắt đầu";
                    ShowErrorMessage = true;
                    return;
                }

                if (BookingDate == today && StartTime < now)
                {
                    ErrorMessage = "Không thể đặt phòng cho thời gian đã qua";
                    ShowErrorMessage = true;
                    return;
                }

                var currentApartmentId = 1;

                var success = await _communityRoomService.CreateBooking(
                    SelectedRoom.CommunityRoomId,
                    currentApartmentId,
                    BookingDate,  // Sử dụng tên property đã đổi
                    StartTime,    // Sử dụng tên property đã đổi
                    EndTime,      // Sử dụng tên property đã đổi
                    NumberOfPeople
                );

                if (success)
                {
                    await LoadDataAsync();
                    ShowErrorMessage = false;
                    ErrorMessage = string.Empty;
                    // Reset form
                    SelectedRoom = null;
                    SelectedStartTime = null; // Sửa lại cách reset
                    SelectedEndTime = null;   // Sửa lại cách reset
                    NumberOfPeople = 0;
                }
                else
                {
                    ErrorMessage = "Phòng đã được đặt trong khoảng thời gian này";
                    ShowErrorMessage = true;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Có lỗi xảy ra: {ex.Message}";
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
