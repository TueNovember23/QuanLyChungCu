using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Repositories.Repositories.Entities;
using Services.DTOs.ApartmentDTO;
using Services.DTOs.CommunityRoomBookingDTO;
using Services.Interfaces.ServiceSupervisorServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Forms.ViewModels.ServiceSupervisor
{
    public partial class RegisterCommunityRoomViewModel : ObservableObject
    {
        private readonly ICommunityRoomService _communityRoomService;

        [ObservableProperty]
        private ObservableCollection<ApartmentDTO> apartments = [];

        [ObservableProperty]
        private ApartmentDTO? selectedApartment;

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

        [ObservableProperty]
        private string? searchApartmentCode;

        [ObservableProperty]
        private DateTime? searchBookingDate;

        [ObservableProperty]
        private string? reason; 

        [ObservableProperty]
        private int selectedPriority;

    

        [ObservableProperty]
        private bool canUseWithOtherPeople;

        [ObservableProperty]
        private List<int> priorityOptions = new List<int> { 1 , 2, 3 };

        [ObservableProperty]
        private List<string> statusOptions = new List<string> { "Đã đăng ký", "Chờ duyệt", "Đã hủy" };


        private DateOnly BookingDate => DateOnly.FromDateTime(SelectedDate ?? DateTime.Today);
        private TimeOnly StartTime => TimeOnly.FromDateTime(SelectedStartTime ?? DateTime.Now); 
        private TimeOnly EndTime => TimeOnly.FromDateTime(SelectedEndTime ?? DateTime.Now);

 

        public string AvailableCapacityText =>
            $"Số người còn lại có thể đăng ký: {SelectedRoom.RoomSize - SelectedRoom.CurrentBookings}";


        public RegisterCommunityRoomViewModel(ICommunityRoomService communityRoomService)
        {
            _communityRoomService = communityRoomService;
            _ = LoadDataAsync();
        }


        private async Task LoadDataAsync()
        {
            var roomList = await _communityRoomService.GetAll();
            Rooms = new ObservableCollection<ResponseCommunityRoomDTO>(roomList);

            var bookingList = await _communityRoomService.SearchBookings(SearchApartmentCode, SearchBookingDate.HasValue ? DateOnly.FromDateTime(SearchBookingDate.Value) : (DateOnly?)null);
            Bookings = new ObservableCollection<ResponseCommunityRoomBookingDTO>(bookingList);

            var apartmentList = await _communityRoomService.GetApartments();
            Apartments = new ObservableCollection<ApartmentDTO>(apartmentList);
        } 


        [RelayCommand]
        private async Task Search()
        {
            await LoadDataAsync();
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

                if (SelectedApartment == null)
                {
                    ErrorMessage = "Vui lòng chọn mã căn hộ đăng ký";
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

                if (BookingDate == today)
                {
                    ErrorMessage = "Vui lòng đặt phòng ít nhất 1 ngày trước ngày hiện tại";
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

                var isAvailable = await _communityRoomService.IsRoomAvailable(
                    SelectedRoom.CommunityRoomId,
                    BookingDate,
                    StartTime,
                    EndTime,
                    NumberOfPeople
                );

                if (!isAvailable)
                {
                    ErrorMessage = "Số người đăng ký vượt quá sức chứa phòng trong khung giờ này.";
                    ShowErrorMessage = true;
                    return;
                }

                var availableSlots = await _communityRoomService.GetAvailableTimeSlots(SelectedRoom.CommunityRoomId, BookingDate);

                if (!availableSlots.Any(slot => StartTime >= slot.startTime && EndTime <= slot.endTime))
                {
                    ErrorMessage = "Chỉ được đăng ký trong thời gian từ 7 giờ sáng tới 22 giờ tối";
                    ShowErrorMessage = true;
                    return;
                }


                var currentApartmentId = SelectedApartment?.ApartmentId ?? 1;


                var success = await _communityRoomService.CreateBooking(
                     SelectedRoom.CommunityRoomId,
                     currentApartmentId,
                     BookingDate,
                     StartTime,
                     EndTime,
                     NumberOfPeople,
                     Reason,
                     SelectedPriority,
                     CanUseWithOtherPeople
                 );

                if (success)
                {
                    await LoadDataAsync();
                    ShowErrorMessage = false;
                    ErrorMessage = string.Empty;
                    SelectedRoom = null;
                    SelectedStartTime = null;
                    SelectedEndTime = null;
                    NumberOfPeople = 0;
                }
                else
                {
                    ErrorMessage = "Có lỗi xảy ra khi tạo booking.";
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
            }
            else
            {
                ErrorMessage = $"Có lỗi xảy ra";
                ShowErrorMessage = true;
            }
        }

        [RelayCommand]
        private async Task CheckAvailableTimeSlots()
        {
            if (SelectedRoom == null || SelectedDate == null)
            {
                ErrorMessage = "Vui lòng chọn phòng và ngày để kiểm tra.";
                ShowErrorMessage = true;
                return;
            }

            try
            {
                var availableSlots = await _communityRoomService.GetAvailableTimeSlots(SelectedRoom.CommunityRoomId, BookingDate);

                if (availableSlots.Any())
                {
                    var message = "Các khoảng thời gian khả dụng:\n" +
                        string.Join("\n", availableSlots.Select(slot => $"{slot.startTime} - {slot.endTime}"));
                    ErrorMessage = message;
                }
                else
                {
                    ErrorMessage = "Không có khoảng thời gian nào khả dụng.";
                }

                ShowErrorMessage = true;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Có lỗi xảy ra: {ex.Message}";
                ShowErrorMessage = true;
            }
        }


    }
}
