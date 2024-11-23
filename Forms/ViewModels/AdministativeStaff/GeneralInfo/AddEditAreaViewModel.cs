using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Services.DTOs.GeneralInfo.AreaDTO;
using Services.Interfaces.AdministrativeStaffServices;
using Core;
using System;
using System.Threading.Tasks;

namespace Forms.ViewModels.AdministativeStaff.GeneralInfo
{
    public partial class AddEditAreaViewModel : ObservableObject
    {
        private readonly IGeneralInfoService _generalInfoService;
        private readonly AreaResponseDTO? _existingArea;

        [ObservableProperty]
        private string _dialogTitle = string.Empty;

        [ObservableProperty]
        private string _areaName = string.Empty;

        [ObservableProperty]
        private string _location = string.Empty;

        [ObservableProperty]
        private bool _isLoading;

        public AddEditAreaViewModel(IGeneralInfoService generalInfoService)
        {
            _generalInfoService = generalInfoService ?? throw new ArgumentNullException(nameof(generalInfoService));
            DialogTitle = "Thêm khu vực mới";
        }

        public AddEditAreaViewModel(IGeneralInfoService generalInfoService, AreaResponseDTO area)
            : this(generalInfoService)
        {
            _existingArea = area ?? throw new ArgumentNullException(nameof(area));
            DialogTitle = "Chỉnh sửa khu vực";
            AreaName = area.AreaName;
            Location = area.Location;
        }

        [RelayCommand]
        private async Task Save()
        {
            if (string.IsNullOrWhiteSpace(AreaName) || string.IsNullOrWhiteSpace(Location))
            {
                await DialogHost.Show("Vui lòng điền đầy đủ thông tin", "RootDialog");
                return;
            }

            try
            {
                IsLoading = true;

                if (_existingArea == null)
                {
                    var createDto = new CreateAreaDTO
                    {
                        AreaName = AreaName.Trim(),
                        Location = Location.Trim()
                    };
                    await _generalInfoService.AddAreaAsync(createDto);
                }
                else
                {
                    var updateDto = new UpdateAreaDTO
                    {
                        AreaId = _existingArea.AreaId,
                        AreaName = AreaName.Trim(),
                        Location = Location.Trim()
                    };
                    await _generalInfoService.UpdateAreaAsync(updateDto);
                }

                DialogHost.Close("RootDialog", true);
            }
            catch (BusinessException ex)
            {
                await DialogHost.Show($"Lỗi nghiệp vụ: {ex.Message}", "RootDialog");
            }
            catch (Exception ex)
            {
                await DialogHost.Show("Đã xảy ra lỗi không mong muốn", "RootDialog");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            DialogHost.Close("RootDialog", null);
        }
    }
}