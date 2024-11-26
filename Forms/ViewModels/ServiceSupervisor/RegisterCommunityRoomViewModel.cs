using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Repositories.Repositories.Entities;
using Services.DTOs.ApartmentDTO;
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
        private ObservableCollection<CommunityRoomBookingDTO> _bookings;

        [ObservableProperty]
        private ObservableCollection<ApartmentDTO> _apartments;

        [ObservableProperty]
        private ApartmentDTO _selectedApartment;

        [ObservableProperty]
        private DateTime? _bookingDate;

        [ObservableProperty]
        private TimeSpan? _startTime;

        [ObservableProperty]
        private TimeSpan? _endTime;

        [ObservableProperty]
        private string _numberOfPeople;

        public RegisterCommunityRoomViewModel(ICommunityRoomService communityRoomService)
        {
            _communityRoomService = communityRoomService;

            // Khởi tạo danh sách
            Bookings = new ObservableCollection<CommunityRoomBookingDTO>();
            Apartments = new ObservableCollection<ApartmentDTO>();

            // Load dữ liệu ban đầu
            Task.Run(async () =>
            {
                await LoadBookingsAsync();
            });
        }

        // Command: Làm mới danh sách đăng ký
        [RelayCommand]
        private async Task LoadBookingsAsync()
        {
            var data = await _communityRoomService.GetAllBookingsAsync();
            if (data != null)
            {
                Bookings = new ObservableCollection<CommunityRoomBookingDTO>(data);
            }
        }

        // Command: Load danh sách căn hộ
        //[RelayCommand]
        //private async Task LoadApartmentsAsync()
        //{
        //    var data = await _communityRoomService.GetAllApartmentsAsync();
        //    if (data != null)
        //    {
        //        Apartments = new ObservableCollection<ApartmentDTO>(data);
        //    }
        //}

        // Command: Đăng ký phòng
        [RelayCommand]
        private async Task RegisterAsync()
        {
            if (SelectedApartment == null || BookingDate == null || StartTime == null || EndTime == null || string.IsNullOrEmpty(NumberOfPeople))
            {
                // Thông báo lỗi nếu dữ liệu nhập thiếu
                throw new InvalidOperationException("Vui lòng điền đầy đủ thông tin đăng ký.");
            }

            var newBooking = new CommunityRoomBookingDTO
            {
                ApartmentCode = SelectedApartment.ApartmentCode,
                BookingDate = new DateOnly(BookingDate.Value.Year, BookingDate.Value.Month, BookingDate.Value.Day),

                StartTime = TimeOnly.FromTimeSpan(StartTime.Value),
                EndTime = TimeOnly.FromTimeSpan(EndTime.Value),
                NumberOfPeople = int.Parse(NumberOfPeople)
            };

            await _communityRoomService.RegisterCommunityRoomAsync(newBooking);
            await LoadBookingsAsync();
        }

        // Command: Xóa booking
        [RelayCommand]
        private async Task DeleteAsync(CommunityRoomBookingDTO booking)
        {
            if (booking == null) return;

            await _communityRoomService.DeleteBookingAsync(booking.BookingId);
            await LoadBookingsAsync();
        }

        // Command: Tìm kiếm
        [RelayCommand]
        private async Task SearchAsync()
        {
            var results = await _communityRoomService.SearchBookingsAsync(SelectedApartment?.ApartmentCode, BookingDate);
            if (results != null)
            {
                Bookings = new ObservableCollection<CommunityRoomBookingDTO>(results);
            }
        }

    }
}
