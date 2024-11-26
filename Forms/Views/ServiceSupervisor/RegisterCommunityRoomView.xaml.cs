using Services.DTOs.CommunityRoomBookingDTO;
using Services.Interfaces.ServiceSupervisorServices;
using System.Windows;
using System.Windows.Controls;


namespace Forms.Views.ServiceSupervisor
{
    public partial class RegisterCommunityRoomView : UserControl
    {
        private readonly ICommunityRoomService _communityRoomService;


        public RegisterCommunityRoomView(ICommunityRoomService communityRoomService)
        {
            InitializeComponent();
            _communityRoomService = communityRoomService;

            // Tải dữ liệu
            LoadBookings();
        }

        private async Task LoadBookings()
        {
            var bookings = await _communityRoomService.GetAllBookingsAsync();
            BookingDataGrid.ItemsSource = bookings;
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e) => await LoadBookings();

        private async void DeleteCommand_Execute(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is CommunityRoomBookingDTO booking)
            {
                var result = await _communityRoomService.DeleteBookingAsync(booking.BookingId);
                if (result)
                {
                    MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadBookings();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy dữ liệu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
    }
}