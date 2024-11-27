using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Services.DTOs.DepartmentDTO;
using Services.Interfaces.AdministrativeStaffServices;
using System.Collections.ObjectModel;
using Core;
using System.Threading.Tasks;
using System.Windows;

namespace Forms.ViewModels.AdministrativeStaff
{
    public partial class DepartmentViewModel : ObservableObject
    {
        private readonly IDepartmentService _departmentService;

        [ObservableProperty]
        private ObservableCollection<DepartmentDTO> departments = new();

        [ObservableProperty]
        private DepartmentDTO? selectedDepartment;

        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private bool isLoading;

        public DepartmentViewModel(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
            LoadDepartmentsAsync().ConfigureAwait(false);
        }

        [RelayCommand]
        private async Task LoadDepartmentsAsync()
        {
            try
            {
                IsLoading = true;
                var result = await _departmentService.GetAllAsync();
                Departments = new ObservableCollection<DepartmentDTO>(result);
            }
            catch (BusinessException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task SearchAsync()
        {
            try
            {
                IsLoading = true;
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    await LoadDepartmentsAsync();
                    return;
                }

                var results = await _departmentService.SearchAsync(SearchText);
                Departments = new ObservableCollection<DepartmentDTO>(results);
            }
            catch (BusinessException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void CreateNew()
        {
            SelectedDepartment = new DepartmentDTO
            {
                DepartmentName = string.Empty,
                NumberOfStaff = 0,
                Description = string.Empty,
                IsDeleted = false
            };
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            try
            {
                IsLoading = true;

                if (SelectedDepartment == null)
                    throw new BusinessException("Vui lòng nhập thông tin bộ phận");

                if (SelectedDepartment.DepartmentId == 0) 
                {
                    var createDto = new CreateDepartmentDTO
                    {
                        DepartmentName = SelectedDepartment.DepartmentName,
                        NumberOfStaff = SelectedDepartment.NumberOfStaff,
                        Description = SelectedDepartment.Description
                    };
                    await _departmentService.CreateAsync(createDto);
                }
                else // Update
                {
                    var updateDto = new UpdateDepartmentDTO
                    {
                        DepartmentId = SelectedDepartment.DepartmentId,
                        DepartmentName = SelectedDepartment.DepartmentName,
                        NumberOfStaff = SelectedDepartment.NumberOfStaff,
                        Description = SelectedDepartment.Description
                    };
                    await _departmentService.UpdateAsync(updateDto);
                }

                await LoadDepartmentsAsync();
                MessageBox.Show("Lưu thành công!");
            }
            catch (BusinessException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception)
            {
                MessageBox.Show("Đã xảy ra lỗi khi lưu. Vui lòng thử lại.");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]  
        private async Task DeleteAsync()
        {
            try
            {
                if (SelectedDepartment == null)
                    throw new BusinessException("Vui lòng chọn bộ phận cần xóa");

                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa bộ phận {SelectedDepartment.DepartmentName}?",
                    "Xác nhận xóa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    IsLoading = true;
                    await _departmentService.DeleteAsync(SelectedDepartment.DepartmentId);
                    await LoadDepartmentsAsync();
                    MessageBox.Show("Xóa thành công!");
                    
                    SelectedDepartment = null;
                }
            }
            catch (BusinessException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception)
            {
                MessageBox.Show("Đã xảy ra lỗi khi xóa. Vui lòng thử lại.");
            }
            finally
            {
                IsLoading = false;
            }
        }

        partial void OnSearchTextChanged(string value)
        {
            SearchAsync().ConfigureAwait(false);
        }
    }
}