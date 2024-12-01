using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Forms.Views.Accountant;
using Services.DTOs.EquipmentDTO;
using Services.Interfaces.AccountantServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Forms.ViewModels.Accountant
{
    public partial class AddEquipmentViewModel : ObservableObject
    {
        private readonly IEquipmentService _equipmentService;
        private readonly string _equipmentCode;

        [ObservableProperty]
        private ResponseEquipmentDTO newEquipment;

        [ObservableProperty]
        private ObservableCollection<string> areas;

        [ObservableProperty]
        private ObservableCollection<string> statusList = new() { "Hoạt động", "Hỏng", "Bảo trì" };

        [ObservableProperty]
        private ObservableCollection<string> areaList = new() { "Khu vực Block E", "Khu vực Gửi xe", "Khu vực Khuôn viên" };

        public event Action? EquipmentAdded;

        public AddEquipmentViewModel(IEquipmentService equipmentService, string apartmentCode)
        {
            _equipmentService = equipmentService;
            _equipmentCode = apartmentCode;
            newEquipment = CreateNewEquipmentDTO();
        }

        private ResponseEquipmentDTO CreateNewEquipmentDTO()
        {
            return new ResponseEquipmentDTO
            {
                EquipmentName = "",
                Status = "Hoạt động",
                Discription = "",
                AreaName = "",
                LastMaintenanceDate = null,
                Notes = "",
                LastCheckDate = null,
            };
        }


        [RelayCommand]
        private async Task SaveNewEquipment()
        {
            if (ValidateEquipment())
            {
                try
                {
                    await _equipmentService.Add(newEquipment);
                    EquipmentAdded?.Invoke();
                    MessageBox.Show("Thêm thiết bị thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    CloseWindow();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi thêm thiết bị: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        private void CancelAddNewEquipment()
        {
            CloseWindow();
        }

        private bool ValidateEquipment()
        {
            if (string.IsNullOrWhiteSpace(newEquipment.EquipmentName))
            {
                MessageBox.Show("Vui lòng nhập tên thiết bị!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(newEquipment.AreaName))
            {
                MessageBox.Show("Vui lòng chọn khu vực!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void CloseWindow()
        {
            if (Application.Current.Windows.OfType<AddEquipmentView>().FirstOrDefault() is Window window)
            {
                window.Close();
            }
        }
    }
}
